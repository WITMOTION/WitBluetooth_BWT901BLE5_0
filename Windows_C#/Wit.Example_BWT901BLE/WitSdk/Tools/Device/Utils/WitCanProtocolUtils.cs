using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Utils
{
    /// <summary>
    /// 维特CAN传感器协议工具类（需要使用TTL-CAN）
    /// </summary>
    public class WitCanProtocolUtils
    {
        /// <summary>
        /// 获得帧ID
        /// </summary>
        /// <param name="id">帧ID(10进制)</param>
        /// <param name="frameFormat">帧格式 0=标准 1=扩展</param>
        /// <param name="frmaeType">帧类型 0=数据 ; 1=远程</param>
        public static int GetFrameId(int id, int frameFormat, int frmaeType)
        {
            int frameId = frameFormat == 1 ? id << 3 | 0x04 : id <<= 21;
            if (frmaeType == 1) { frameId |= 0x02; }

            return frameId;
        }


        #region 获得读写命令

        /// <summary>
        /// 获得标准数据桢写命令
        /// </summary>
        /// <param name="canId"></param>
        /// <param name="reg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetStandWrite(int canId, byte reg, short value)
        {   
            // 返回结果
            List<byte> byteList = new List<byte>();
            // 数据
            byte[] dataFrame = new byte[] { 0xff, 0xaa, reg, (byte)(value), (byte)(value >> 8) };

            // AT（2byte）
            byteList.AddRange(Encoding.Default.GetBytes("AT"));
            // 帧ID（4byte）
            int frameId = GetFrameId(canId, 0, 0);
            byteList.AddRange(BitConverter.GetBytes(frameId));
            // 数据长度（2byte）
            byteList.AddRange(new byte[] { (byte)(dataFrame.Length >> 8), (byte)(dataFrame.Length) });
            // 帧数据 
            byteList.AddRange(dataFrame);
            // 回车换行（2byte）
            byteList.AddRange(Encoding.Default.GetBytes("\r\n"));
            return byteList.ToArray();
        }



        /// <summary>
        /// 获得标准数据桢读命令
        /// </summary>
        /// <param name="canId"></param>
        /// <param name="reg"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] GetStandRead(int canId, byte reg) 
        {
            // 返回结果
            List<byte> byteList = new  List<byte>();
            // 数据
            byte[] dataFrame = new byte[] { 0xff, 0xaa, 0x27, reg, 00 };

            // AT（2byte）
            byteList.AddRange(Encoding.Default.GetBytes("AT"));
            // 帧ID（4byte）
           int frameId =  GetFrameId(canId, 0, 0);
            byteList.AddRange(BitConverter.GetBytes(frameId));
            // 数据长度（2byte）
            byteList.AddRange(new byte[] { (byte)(dataFrame.Length >> 8),(byte)(dataFrame.Length) });
            // 帧数据 
            byteList.AddRange(dataFrame);
            // 回车换行（2byte）
            byteList.AddRange(Encoding.Default.GetBytes("\r\n"));
            return byteList.ToArray();
        }

        #endregion


    }
}
