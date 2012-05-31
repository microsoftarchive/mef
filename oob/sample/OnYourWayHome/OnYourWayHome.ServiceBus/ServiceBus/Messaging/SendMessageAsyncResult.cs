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

    internal sealed class SendMessageAsyncResult : ServiceBusRequestAsyncResult<bool>
    {
        private readonly string path;
        private readonly BrokeredMessage message;
        private readonly Uri uri;

        public SendMessageAsyncResult(string path, BrokeredMessage message, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.message = message;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path + "/Messages");
        }

        public string Path
        {
            get { return this.path; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "POST"; }
        }

        protected override void OnSendRequest(HttpWebRequest request, Stream requestStream, SimpleWebToken token)
        {
            this.message.UpdateHeaderDictionary();
            foreach (var header in this.message.Headers.Keys)
            {
                var value = this.message.Headers[header];
                request.Headers[header] = value != null ? value.ToString() : null;
            }

            request.ContentType = this.message.ContentType;

            var buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = this.message.BodyStream.Read(buffer, 0, 1024)) > 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }

            base.OnSendRequest(request, requestStream, token);
        }
    }
}
