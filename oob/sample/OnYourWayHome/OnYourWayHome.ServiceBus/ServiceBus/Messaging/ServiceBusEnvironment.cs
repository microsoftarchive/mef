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

namespace OnYourWayHome.ServiceBus
{
    using System;

    public static class ServiceBusEnvironment
    {
        private const string StsHostName = "accesscontrol.windows.net";
        private const string RelayHostName = "servicebus.windows.net";

        public static Uri CreateAccessControlUri(string serviceNamespace, string endpointPath)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("https://{0}-sb.{1}/{2}", serviceNamespace, StsHostName, endpointPath), UriKind.Absolute);
        }

        public static Uri CreateServiceUri(string serviceNamespace, string servicePath)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("https://{0}.{1}/{2}", serviceNamespace, RelayHostName, servicePath), UriKind.Absolute);
        }

        public static Uri CreateDefaultServiceRealmUri(string serviceNamespace)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("http://{0}.{1}/", serviceNamespace, RelayHostName), UriKind.Absolute);
        }
    }
}
