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

    internal sealed class PeekLockMessageAsyncResult : ServiceBusRequestAsyncResult<BrokeredMessage>
    {
        private readonly string path;
        private readonly Uri uri;
        private readonly TimeSpan timeout;

        public PeekLockMessageAsyncResult(string path, TimeSpan timeout, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.timeout = timeout;

            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path + "/Messages/Head?timeout=" + timeout.TotalSeconds);
        }

        public string Path
        {
            get { return this.path; }
        }

        public TimeSpan Timeout
        {
            get { return this.timeout; }
        }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "POST"; }
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var bodyStream = new MemoryStream();
                using (var responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = responseStream.Read(buffer, 0, 1024)) > 0)
                    {
                        bodyStream.Write(buffer, 0, bytesRead);
                    }
                }

                bodyStream.Flush();
                bodyStream.Position = 0;

                this.Result = new BrokeredMessage(bodyStream, response.Headers) { ContentType = response.ContentType };
            }

            base.OnReceiveResponse(request, response);
        }
    }
}
