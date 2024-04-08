using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wit.SDK.Modular.Sensor.Utils
{
    /// <summary>
    /// 9轴倾角传感器帮助类
    /// </summary>
    public class MWT925Helper
    {
        /// <summary>
        /// 解锁命令
        /// </summary>
        /// <returns></returns>
        public static byte[] Unlock(byte addr)
        {
            return Modbus16Utils.GetWrite(addr, 0x69, 0xB588);
        }

        /// <summary>
        /// 加计校准
        /// </summary>
        /// <returns></returns>
        public static byte[] AccelerationCalibration(byte addr)
        {
            return Modbus16Utils.GetWrite(addr, 0x01, 0x01);
        }

        /// <summary>
        /// 开始磁场校准
        /// </summary>
        /// <returns></returns>
        public static byte[] BeginFieldCalibration(byte addr)
        {
            return Modbus16Utils.GetWrite(addr, 0x01, 0x07);
        }

        /// <summary>
        /// 结束磁场校准
        /// </summary>
        /// <returns></returns>
        public static byte[] EndFieldCalibration(byte addr)
        {
            return Modbus16Utils.GetWrite(addr, 0x01, 0x00);
        }

        /// <summary>
        /// 波特率调整
        /// </summary>
        /// <returns></returns>
        public static byte[] SetBaudRate(byte addr, ushort value)
        {
            return Modbus16Utils.GetWrite(addr, 0x04, value);
        }

        /// <summary>
        /// 设置输出内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SetBackContent(byte addr, ushort value)
        {
            return Modbus16Utils.GetWrite(addr, 0x02, value);
        }

        // 带宽调整

        // 量程调整

        // 滤波参数
    }
}
