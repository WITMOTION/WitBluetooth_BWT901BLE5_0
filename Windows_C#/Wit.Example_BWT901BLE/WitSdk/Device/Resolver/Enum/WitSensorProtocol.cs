using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Enum
{


    /// <summary>
    /// 维特智能传感器协议枚举
    /// </summary>
    public enum WitSensorProtocol
    {
        /// <summary>
        /// 维特协议
        /// </summary>
        WitProtocol = 0,
        /// <summary>
        /// 维特智能Can协议
        /// </summary>
        WitCanProtocol = 1,
        /// <summary>
        /// 维特智能蓝牙5.0协议
        /// </summary>
        WitBle5Protocol = 2,
        /// <summary>
        /// Modbus16协议
        /// </summary>
        Modbus16Protocol = 3,
        /// <summary>
        /// 维特JY61协议
        /// </summary>
        WitJY61Protocol = 4
    }
}
