using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.Bluetooth.WinBlue.Enums
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum BluetoothEvent
    {
        // 连接中
        Connecting,
        // 连接的
        Connected,
        // 关闭的
        Disconnected,
        // 数据
        Data
    }
}
