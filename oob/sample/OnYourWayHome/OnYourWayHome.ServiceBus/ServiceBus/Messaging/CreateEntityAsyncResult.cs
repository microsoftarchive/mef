//---------------------------------------------------------------------------------
// Copyright (c) 2011, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.IO;
    using System.Net;
    using OnYourWayHome.AccessControl;

    internal sealed class CreateEntityAsyncResult<T> : ServiceBusRequestAsyncResult<Entity<T>>
    {
        private readonly string path;
        private readonly T requestEntity;
        private readonly Uri uri;

        public CreateEntityAsyncResult(string path, T entityDescription, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.requestEntity = entityDescription;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path);
        }

        public string Path
        {
            get { return this.path; }
        }

        public T EntityDescription
        {
            get { return this.requestEntity; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "PUT"; }
        }

        protected override void OnSendRequest(HttpWebRequest request, Stream requestStream, SimpleWebToken token)
        {
            request.ContentType = "application/atom+xml";

            var entity = new Entity<T>(this.path, this.requestEntity);
            using (var streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(entity.ToString());
                streamWriter.Flush();
            }

            base.OnSendRequest(request, requestStream, token);
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    this.Result = Entity<T>.Create(responseStream);
                }
            }

            base.OnReceiveResponse(request, response);
        }
    }
}
