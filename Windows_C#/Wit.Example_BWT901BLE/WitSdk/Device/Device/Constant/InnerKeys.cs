using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Sensor.Device.Constant
{
    /// <summary>
    /// 内部的键值
    /// </summary>
    public class InnerKeys
    {
        // 串口端口
        public static string ComPort { get; } = "_ComPort";

        // 串口can波特率
        public static string ComBaud { get; } = "_ComBaud";

        // 设备地址Key值
        public static string ADDR_KEY { get; } = "ADDR";

        // can波特率
        public static string CanBaud { get; } = "_CanBaud";

        // 数据包速率
        public static string PacketRate { get; } = "_PacketRate";

        // 数据大小速率
        public static string DataSizeRate { get; } = "_DataSizeRate";
    }
}
