using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Entity
{
    /// <summary>
    /// tcp服务端配置
    /// </summary>
    [Serializable]
    public class TcpServerConfig : IConnectConfig
    {
        /// <summary>
        /// 监听的IP和端口号
        /// </summary>
        public IPEndPoint IPEndPoint = null;

        /// <summary>
        /// 远程网络地址
        /// </summary>
        public EndPoint RemoteEP = null;
    }
}
