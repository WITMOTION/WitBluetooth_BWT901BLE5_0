using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Utils
{
    /// <summary>
    /// 维特协议工具类
    /// </summary>
    public class WitProtocolUtils
    {
        #region SUM和校验
        /// <summary>
        /// 获取SUM和校验结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte ToSUM(byte[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum = sum + data[i];
            }
            //实际上num 这里已经是结果了，如果只是取int 可以直接返回了
            return (byte)sum;
        }


        /// <summary>
        /// 校验数据包和校验
        /// </summary>
        /// <param name="dataPack"></param>
        /// <returns></returns>
        public static bool CheckSUM(byte[] dataPack)
        {
            if (dataPack == null || dataPack.Length < 2)
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < dataPack.Length - 1; i++)
            {
                sum = sum + dataPack[i];
            }
            //实际上num 这里已经是结果了，如果只是取int 可以直接返回了
            byte check = (byte)sum;


            //如果最后一个字节等于校验和就是通过
            if (dataPack[dataPack.Length - 1] == check)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 校验数据包和校验
        /// </summary>
        /// <param name="hexPackData">一包16进制字符串数据</param>
        /// <returns></returns>
        public static bool CheckPackSUM(string hexPackData)
        {
            byte[] packData = ByteArrayConvert.HexStringToByteArray(hexPackData);
            return CheckSUM(packData);
        }

        #endregion

        /// <summary>
        /// 获得读取的命令
        /// 功能：传入寄存器获得,读取寄存器的命令
        /// </summary>
        /// <param name="reg">寄存器</param>
        /// <returns></returns>
        public static byte[] GetRead(int reg)
        {
            return new byte[] { 0xff,0xaa,0x27,(byte)reg, 0x00};
        }


        /// <summary>
        /// 获得写入的命令
        /// 功能：传入寄存器和值,得到写入寄存器的命令
        /// </summary>
        /// <param name="reg">寄存器</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] GetWrite(int reg, ushort value)
        {
            byte[] vs = new byte[] { 0xff, 0xaa, (byte)reg, (byte)value, (byte)(value >> 8) };
            return new byte[] { 0xff, 0xaa, (byte)reg, (byte)value, (byte)(value>>8) };
        }

        /// <summary>
        /// 查找返回的数据
        /// 功能:从传感器返回的数据里找到第一包55 5F开头的数据包
        /// </summary>
        /// <param name="returnData">设备返回的数据</param>
        /// <returns>成功返回55 5f数据包,失败返回null</returns>
        public static byte[] FindReturnData(byte[] returnData)
        {
            byte[] tempArr = new byte[0];

            for (int i = 0; i < returnData.Length; i++)
            {
                tempArr = returnData.Skip(i).Take(11).ToArray(); ;

                if (tempArr.Length == 11 && tempArr[0] == 0x55 && tempArr[1] == 0x5F && CheckSUM(tempArr))
                {
                    return tempArr;

                }
            }
            return null;
        }
    }
}
