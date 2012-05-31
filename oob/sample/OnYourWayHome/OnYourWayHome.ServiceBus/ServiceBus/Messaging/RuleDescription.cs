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
    using System.Runtime.Serialization;

    [DataContract(Name = "RuleDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class RuleDescription
    {
        /// <summary>
        /// The default name used in creating default rule when adding subscriptions
        /// to a topic. The name is "$Default".
        /// </summary>
        public const string DefaultRuleName = "$Default";

        public RuleDescription()
            : this(new TrueFilter())
        {
        }

        public RuleDescription(Filter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            this.Filter = filter;
        }

        [DataMember(Name = "Filter", IsRequired = false, Order = 1001, EmitDefaultValue = false)]
        public Filter Filter { get; set; }

        [DataMember(Name = "Action", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public RuleAction Action { get; set; }
        
        public string TopicPath { get; internal set; }

        public string SubscriptionName { get; internal set; }

        public string Name { get; internal set; }

        public Uri Uri { get; private set; }

        internal void UpdateFromEntity(string topicPath, string subscriptionName, Entity<RuleDescription> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.TopicPath = topicPath;
            this.SubscriptionName = subscriptionName;
            this.Name = entity.Name;
            this.Uri = entity.Uri;
        }
    }
}
