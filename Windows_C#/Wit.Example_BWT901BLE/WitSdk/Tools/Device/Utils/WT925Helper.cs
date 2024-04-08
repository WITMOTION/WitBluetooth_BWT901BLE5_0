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
    public class WT925Helper
    {
        /// <summary>
        /// 解锁命令
        /// </summary>
        /// <returns></returns>
        public static byte[] Unlock()
        {

            return WitProtocolUtils.GetWrite(0x69, 0xB588);
        }

        /// <summary>
        /// 加计校准
        /// </summary>
        /// <returns></returns>
        public static byte[] AccelerationCalibration()
        {
            return WitProtocolUtils.GetWrite(0x01, 0x01);
        }

        /// <summary>
        /// 开始磁场校准
        /// </summary>
        /// <returns></returns>
        public static byte[] BeginFieldCalibration()
        {
            return WitProtocolUtils.GetWrite(0x01, 0x07);
        }

        /// <summary>
        /// 结束磁场校准
        /// </summary>
        /// <returns></returns>
        public static byte[] EndFieldCalibration()
        {
            return WitProtocolUtils.GetWrite(0x01, 0x00);
        }

        /// <summary>
        /// 波特率调整
        /// </summary>
        /// <returns></returns>
        public static byte[] SetBaudRate(ushort value)
        {
            return WitProtocolUtils.GetWrite(0x04, value);
        }

        /// <summary>
        /// 设置输出内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SetBackContent(ushort value)
        {
            return WitProtocolUtils.GetWrite(0x02, value);
        }

        // 带宽调整

        // 量程调整

        // 滤波参数
    }
}
