using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.Example_WTVB01BT50.vb01.Data
{
    public static class WTVB01SensorKey
    {
        // 芯片时间
        public static string ChipTime { get; } = "ChipTime";

        // X轴振动速度
        public static string VX { get; } = "VX";

        // Y轴振动速度
        public static string VY { get; } = "VY";

        // Z轴振动速度
        public static string VZ { get; } = "VZ";

        // X轴角度振动幅度
        public static string ADX { get; } = "ADX";

        // Y轴角度振动幅度
        public static string ADY { get; } = "ADY";

        // Z轴角度振动幅度
        public static string ADZ { get; } = "ADZ";

        // X轴振动位移
        public static string DX { get; } = "DX";

        // Y轴振动位移
        public static string DY { get; } = "DY";

        // Z轴振动位移
        public static string DZ { get; } = "DZ";

        // X轴振动频率
        public static string HZX { get; } = "HZX";

        // Y轴振动频率
        public static string HZY { get; } = "HZY";

        // Z轴振动频率
        public static string HZZ { get; } = "HZZ";

        // 温度
        public static string TEMP { get; } = "TEMP";

        /// <summary>
        /// 电量
        /// </summary>
        public static string Power { get; } = "Power";

        /// <summary>
        /// 电量%
        /// </summary>
        public static string PowerPercent { get; } = "PowerPercent";

        // 版本号
        public static string VersionNumber { get; } = "VersionNumber";

        // 序列号
        public static string SerialNumber { get; } = "SerialNumber";
    }
}
