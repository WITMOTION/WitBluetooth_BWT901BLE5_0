using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Entity
{
    /// <summary>
    /// tcp客户端配置
    /// </summary>
    [Serializable]
    public class TcpClientConfig : IConnectConfig
    {
        /// <summary>
        /// 通信的远程服务端ip
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 通信的远程服务端端口
        /// </summary>
        public int Port { get; set; }

    }
}
