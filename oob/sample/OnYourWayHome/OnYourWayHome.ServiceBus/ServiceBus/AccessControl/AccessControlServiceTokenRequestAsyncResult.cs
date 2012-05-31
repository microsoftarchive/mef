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

namespace OnYourWayHome.AccessControl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using OnYourWayHome.ServiceBus;

    /// <summary>
    /// Asynchronously requests a token from the Access Control Service
    /// </summary>
    internal sealed class AccessControlServiceTokenRequestAsyncResult : RestAsyncResult<SimpleWebToken>
    {
        private readonly TokenRequest tokenRequest;
        private readonly Uri stsUri;

        public AccessControlServiceTokenRequestAsyncResult(string serviceNamespace, TokenRequest tokenRequest)
        {
            if (tokenRequest == null)
            {
                throw new ArgumentNullException("tokenRequest");
            }

            this.tokenRequest = tokenRequest;

            this.stsUri = ServiceBusEnvironment.CreateAccessControlUri(serviceNamespace, this.tokenRequest.Path);
        }
        
        public TokenRequest TokenRequest
        {
            get { return this.tokenRequest; }
        }

        protected override string Method
        {
            get { return "POST"; }
        }

        protected override Uri Uri
        {
            get { return this.stsUri; }
        }

        protected override void OnCreateRequest(HttpWebRequest request)
        {
            request.ContentType = "application/x-www-form-urlencoded";

            base.OnCreateRequest(request);
        }

        protected override void OnSendRequest(HttpWebRequest request, Stream requestStream)
        {
            using (var streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(this.tokenRequest.Body);
                streamWriter.Flush();
            }

            base.OnSendRequest(request, requestStream);
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var tokenResponse = TokenResponse.Create(this.tokenRequest.Format, responseStream);
                    this.Result = new SimpleWebToken(tokenResponse.AccessToken);
                }
            }

            base.OnReceiveResponse(request, response);
        }
    }
}
