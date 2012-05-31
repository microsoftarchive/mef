using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ServiceBus.Parts
{
    public abstract class DefaultAzureServiceBusConfiguration : IAzureServiceBusConfiguration
    {
        protected DefaultAzureServiceBusConfiguration()
        {
        }

        public abstract string Topic
        {
            get;
        }

        public abstract int DeviceId
        {
            get;
        }

        public string ServiceNamespace
        {
            // TODO: Enter your Service Namespace from Azure
            get { return null; }
        }

        public string IssuerName
        {
            // TODO: Enter the issuer name from Azure
            get { return null; }
        }

        public string IssuerSecret
        {
            // TODO: Enter the issuer secret
            get { return null; }
        }
    }
}
