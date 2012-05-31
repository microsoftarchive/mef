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
    using System.Text;
    using System.Xml;

    public class Entity<TDescription>
    {
        private static readonly SyndicationSerializer<TDescription> serializer = new SyndicationSerializer<TDescription>();

        private readonly SyndicationItem<TDescription> syndicationItem;

        internal Entity(string name, TDescription description)
            : this(new SyndicationItem<TDescription>())
        {
            this.SyndicationItem.Title = name;
            this.SyndicationItem.Content = description;
        }

        internal Entity(SyndicationItem<TDescription> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.syndicationItem = item;
        }

        public Uri Uri
        {
            get { return this.syndicationItem.SelfLink; }
        }

        public string Name
        {
            get { return this.syndicationItem.Title; }
        }

        public TDescription Description
        {
            get { return this.syndicationItem.Content; }
            set { this.syndicationItem.Content = value; }
        }

        internal static SyndicationSerializer<TDescription> Serializer
        {
            get { return serializer; }
        }

        internal SyndicationItem<TDescription> SyndicationItem
        {
            get { return this.syndicationItem; }
        }

        protected internal virtual string Path
        {
            get { return this.Name; }
        }

        public static Entity<TDescription> Create(Stream stream)
        {
            Entity<TDescription> result;

            using (var reader = XmlReader.Create(stream))
            {
                var syndicationItem = serializer.DeserializeItem(reader);
                result = new Entity<TDescription>(syndicationItem);
            }

            return result;
        }

        /// <summary>
        /// Returns a the Atom Xml that represents this instance.
        /// </summary>
        public override string ToString()
        {
            var xmlWriterSettings = new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stream = new MemoryStream())
            {
                this.WriteTo(stream);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public void WriteTo(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                serializer.SerializeItem(this.syndicationItem, writer);
                writer.Flush();
            }
        }
    }
}
