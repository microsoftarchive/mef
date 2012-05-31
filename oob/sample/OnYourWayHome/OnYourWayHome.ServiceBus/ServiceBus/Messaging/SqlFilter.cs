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

    [DataContract(Name = "SqlFilter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    [KnownType(typeof(TrueFilter))]
    [KnownType(typeof(FalseFilter))]
    public class SqlFilter : Filter
    {
        public const int DefaultCompatibilityLevel = 20;

        public SqlFilter(string sqlExpression)
        {
            if (String.IsNullOrEmpty(sqlExpression))
            {
                throw new ArgumentNullException("sqlExpression");
            }

            this.SqlExpression = sqlExpression;
            this.CompatibilityLevel = DefaultCompatibilityLevel;
        }

        [DataMember(Order = 0x10001)]
        public string SqlExpression { get; private set; }
        
        [DataMember(Order = 0x10002)]
        public int CompatibilityLevel { get; private set; }
    }
}
