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
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml;
    using OnYourWayHome.AccessControl;
    using System.Collections.ObjectModel;

    internal sealed class GetEntityCollectionAsyncResult<T> : ServiceBusRequestAsyncResult<IEnumerable<Entity<T>>>
    {
        private readonly string path;
        private readonly Uri uri;

        public GetEntityCollectionAsyncResult(string path, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            this.path = path;
            this.uri = ServiceBusEnvironment.CreateServiceUri(this.TokenProvider.ServiceNamespace, path);
        }

        public string Path
        {
            get { return this.path; }
        }

        public string TopicPath { get; set; }

        protected override Uri Uri
        {
            get { return this.uri; }
        }

        protected override string Method
        {
            get { return "GET"; }
        }

        protected override void OnReceiveResponse(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var result = new List<Entity<T>>();
                    var syndicationFeed = ReadAtomFeed(responseStream);

                    foreach (var item in syndicationFeed.Items)
                    {
                        result.Add(new Entity<T>(item));
                    }

                    this.Result = new ReadOnlyCollection<Entity<T>>(result);
                }
            }

            base.OnReceiveResponse(request, response);
        }

        private static SyndicationFeed<T> ReadAtomFeed(Stream stream)
        {
            SyndicationFeed<T> result;

            using (var reader = XmlReader.Create(stream))
            {
                result = Entity<T>.Serializer.DeserializeFeed(reader);
            }

            return result;
        }
    }
}
