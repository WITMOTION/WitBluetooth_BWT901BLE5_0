using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Entity
{
    /// <summary>
    /// 串口配置
    /// </summary>
    [Serializable]
    public class SerialPortConfig : IConnectConfig
    {
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; } = 0;

        /// <summary>
        /// com口名称
        /// </summary>
        public string PortName { get; set; } = "";

        /// <summary>
        /// 是否启用rts
        /// </summary>
        public bool RtsEnable { get; set; } = false;

        /// <summary>
        /// 是否启用dtr
        /// </summary>
        public bool DTREnable { get; set; } = false;
    }
}
