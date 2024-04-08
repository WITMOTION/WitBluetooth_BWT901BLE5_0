using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Entity
{

    /// <summary>
    /// UDP配置
    /// </summary>
    [Serializable]
    public class UdpConfig : IConnectConfig
    {
        /// <summary>
        /// 本地ip和端口
        /// </summary>
        public IPEndPoint localIpPoint = null;

        /// <summary>
        /// 远程ip和端口
        /// </summary>
        public IPEndPoint remoteIpPoint = null;
    }
}
