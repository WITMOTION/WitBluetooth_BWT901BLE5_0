using System;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;

namespace Wit.SDK.Sensor.Connector.Entity
{
    [Serializable]
    public class UnityBleConfig : IConnectConfig
    {
        public string Mac { get; set; }
        public string DeviceName { get; set; }
        public string ServiceGuid { get; set; }
        public string WriteGuid { get; set; }
        public string NotifyGuid { get; set; }

        public UnityBleConfig()
        {
            ServiceGuid = "0000ffe5-0000-1000-8000-00805f9a34fb";
            WriteGuid = "0000ffe9-0000-1000-8000-00805f9a34fb";
            NotifyGuid = "0000ffe4-0000-1000-8000-00805f9a34fb";
            Mac = Mac;
            DeviceName = DeviceName;
        }
    }
}