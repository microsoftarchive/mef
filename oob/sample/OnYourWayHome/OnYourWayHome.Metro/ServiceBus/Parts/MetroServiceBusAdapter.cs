using System;
using System.Runtime.InteropServices.WindowsRuntime;
using OnYourWayHome.ServiceBus;
using OnYourWayHome.ServiceBus.Serialization;
using Windows.Security.Cryptography.Core;

namespace OnYourWayHome.ServiceBus.Parts
{
    internal class MetroServiceBusAdapter : ServiceBusAdapter
    {
        public override IDataContractSerializer CreateJsonSerializer(Type type)
        {
            return new DataContractJsonSerializerAdapter(type);
        }

        public override byte[] ComputeHmacSha256(byte[] secretKey, byte[] data)
        {
            const string HmacSha256AlgorithmName = "HMAC_SHA256";

            MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(HmacSha256AlgorithmName);
            var key = provider.CreateKey(secretKey.AsBuffer());
            var hashed = CryptographicEngine.Sign(key, data.AsBuffer());

            return hashed.ToArray();
        }
    }
}
