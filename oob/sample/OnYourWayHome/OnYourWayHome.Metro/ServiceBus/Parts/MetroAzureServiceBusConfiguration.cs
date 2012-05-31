using System;

namespace OnYourWayHome.ServiceBus.Parts
{
    internal class MetroAzureServiceBusConfiguration : DefaultAzureServiceBusConfiguration
    {
        public MetroAzureServiceBusConfiguration()
        {
        }

        public override int DeviceId
        {
            get { return 1; }
        }

        public override string Topic
        {
            get { return "Dave"; }
        }
    }
}
