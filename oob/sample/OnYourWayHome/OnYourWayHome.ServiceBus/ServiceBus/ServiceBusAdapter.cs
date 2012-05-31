using System;
using System.Runtime.Serialization;
using OnYourWayHome.ServiceBus.Serialization;

namespace OnYourWayHome.ServiceBus
{
    public abstract class ServiceBusAdapter
    {
        public static ServiceBusAdapter Current
        {
            get;
            set;
        }

        public abstract IDataContractSerializer CreateJsonSerializer(Type type);

        public abstract byte[] ComputeHmacSha256(byte[] secretKey, byte[] data);
    }
}
