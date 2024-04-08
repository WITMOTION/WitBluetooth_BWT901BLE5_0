using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Utils
{
    /// <summary>
    /// Modbus16工具类
    /// </summary>
    public class Modbus16Utils
    {

        /// <summary>
        /// 获得CRC16_Modbus效验,低位在前,高位在后
        /// </summary>
        /// <param name="byteData">要进行计算的字节数组</param>
        /// <returns>计算后的数组</returns>
        public static byte[] GetCrc16(byte[] bytes)
        {
            byte crcRegister_H = 0xFF, crcRegister_L = 0xFF;// 预置一个值为 0xFFFF 的 16 位寄存器

            byte polynomialCode_H = 0xA0, polynomialCode_L = 0x01;// 多项式码 0xA001

            for (int i = 0; i < bytes.Length; i++)
            {
                crcRegister_L = (byte)(crcRegister_L ^ bytes[i]);

                for (int j = 0; j < 8; j++)
                {
                    byte tempCRC_H = crcRegister_H;
                    byte tempCRC_L = crcRegister_L;

                    crcRegister_H = (byte)(crcRegister_H >> 1);
                    crcRegister_L = (byte)(crcRegister_L >> 1);
                    // 高位右移前最后 1 位应该是低位右移后的第 1 位：如果高位最后一位为 1 则低位右移后前面补 1
                    if ((tempCRC_H & 0x01) == 0x01)
                    {
                        crcRegister_L = (byte)(crcRegister_L | 0x80);
                    }

                    if ((tempCRC_L & 0x01) == 0x01)
                    {
                        crcRegister_H = (byte)(crcRegister_H ^ polynomialCode_H);
                        crcRegister_L = (byte)(crcRegister_L ^ polynomialCode_L);
                    }
                }
            }

            return new byte[] { crcRegister_L, crcRegister_H };

        }



        /// <summary>
        /// 获得CRC16_Modbus效验,低位在前,高位在后
        /// </summary>
        /// <param name="byteData">要进行计算的字节数组</param>
        /// <param name="length">长度</param>
        /// <returns>计算后的数组</returns>
        public static byte[] GetCrc16(byte[] byteData, int length)
        {
            byte[] CRC = new byte[2];

            ushort wCrc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;//异或多项式
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);//高位在后
            CRC[0] = (byte)(wCrc & 0x00FF);       //低位在前
            return CRC;

        }

        /// <summary>
        /// 获得写入的modbus指令
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="reg"></param>
        /// <param name="value"></param>
        /// <param name="resultBytes"></param>
        /// <returns>成功返回0</returns>
        public static int GetWrite(byte addr, int reg, int value, out byte[] resultBytes)
        {
            byte[] reghl = GetHL(reg);
            byte[] valuehl = GetHL(value);
            resultBytes = new byte[] { addr, 0x06, reghl[0], reghl[1], valuehl[0], valuehl[1] };
            resultBytes = resultBytes.Concat(GetCrc16(resultBytes)).ToArray();
            return 0;
        }

        /// <summary>
        /// 写入的modbus指令
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="reg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetWrite(byte addr, int reg, int value)
        {
            byte[] resultBytes = null;
            GetWrite(addr, reg, value, out resultBytes);
            return resultBytes;
        }


        /// <summary>
        /// 读取的modbus指令
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="reg"></param>
        /// <param name="count"></param>
        /// <param name="resultBytes"></param>
        /// <returns>成功返回0</returns>
        public static int GetRead(byte addr, int reg, int count, out byte[] resultBytes)
        {
            byte[] reghl = GetHL(reg);
            byte[] counthl = GetHL(count);
            resultBytes = new byte[] { addr, 0x03, reghl[0], reghl[1], counthl[0], counthl[1] };
            resultBytes = resultBytes.Concat(GetCrc16(resultBytes)).ToArray();
            return 0;
        }

        /// <summary>
        /// 读取的modbus指令
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="reg"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] GetRead(byte addr, int reg, int count)
        {
            byte[] resultBytes = null;
            GetRead(addr,reg,count, out resultBytes);
            return resultBytes;
        }


        /// <summary>
        /// 获得高低位,高位在前
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetHL(int value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value << 8 >> 8) };
        }

        /// <summary>
        /// 获得高低位,低位在前
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetLH(int value)
        {
            return new byte[] { (byte)(value << 8 >> 8), (byte)(value >> 8) };
        }


        /// <summary>
        /// 检查校验位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckModbusCrc16(byte[] data)
        {

            if (data == null)
            {

                return false;
            }
            if (data.Length < 3)
            {

                return false;
            }

            byte[] v = GetCrc16(data, data.Length - 2);


            if (v.Length < 2)
            {
                return false;
            }

            if (v[0] != data[data.Length - 2])
            {
                return false;
            }

            if (v[1] != data[data.Length - 1])
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// 查询返回的字节里有没有发送的命令的回传
        /// </summary>
        /// <param name="sendByte"></param>
        /// <param name="returnByte"></param>
        /// <param name="modbus"></param>
        /// <returns>查找成功返回true，查找失败返回false</returns>
        public static bool FindModbus(byte[] sendByte, byte[] returnByte, out byte[] modbus)
        {

            int height = sendByte[4];
            int low = sendByte[5];

            // 得到长度
            int len = 5 + (height << 8 | low) * 2;

            // 如果没有返回结果，或者返回结果根本不够长
            if (returnByte == null || returnByte.Length < len)
            {
                modbus = new byte[0];
                return false;
            }

            // 遍历返回结果查找
            for (int i = 0; i <= returnByte.Length - len; i++)
            {

                byte rAddr = returnByte[i];
                byte mark = 0x03;

                byte[] cCrc = GetCrc16(returnByte.Skip(i).Take(len - 2).ToArray());
                byte rCrcH = returnByte[i + len - 2];
                byte rCrcL = returnByte[i + len - 1];
                // 如果全部通过
                if (sendByte[0] == rAddr && mark == sendByte[1] && rCrcH == cCrc[0] && rCrcL == cCrc[1])
                {
                    modbus = returnByte.Skip(i).Take(len).ToArray();
                    return true;
                }
            }

            modbus = new byte[0];
            return false;
        }
    }
}
