using System;
using System.Collections.Generic;
using System.Linq;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Context;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Roles
{

    /**
     * 蓝牙5.0协议解析器
     */
    public class Bwt901bleResolver : IProtocolResolver
    {
        
        /// <summary>
        /// 主动接收的byte缓存
        /// </summary>
        private List<byte> ActiveByteDataBuffer = new List<byte>();

        /// <summary>
        /// 临时Byte
        /// </summary>
        private byte[] ActiveByteTemp = new byte[1000];

        /// <summary>
        /// 解算数据锁
        /// </summary>
        private object lockobj = new object();

        /// <summary>
        /// 处理被动接收的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="deviceModel"></param>
        public override void OnReceiveData(DeviceModel deviceModel, byte[] data)
        {
            lock (lockobj)
            {

                ActiveByteDataBuffer.AddRange(data);

                while (ActiveByteDataBuffer.Count > 5 && ActiveByteDataBuffer[0] != 0x55 && ActiveByteDataBuffer[1] != 0x61)
                {
                    ActiveByteDataBuffer.RemoveAt(0);
                }

                while (ActiveByteDataBuffer.Count >= 20)
                {

                    ActiveByteTemp = ActiveByteDataBuffer.GetRange(0, 20).ToArray();

                    //必须是55 61的数据包
                    if (ActiveByteTemp[0] == 0x55 && ActiveByteTemp[1] == 0x61)
                    {
                        short[] Pack = new short[9];
                        string Identify = ActiveByteTemp[1].ToString("X");

                        Pack[0] = BitConverter.ToInt16(ActiveByteTemp, 2);
                        Pack[1] = BitConverter.ToInt16(ActiveByteTemp, 4);
                        Pack[2] = BitConverter.ToInt16(ActiveByteTemp, 6);
                        Pack[3] = BitConverter.ToInt16(ActiveByteTemp, 8);
                        Pack[4] = BitConverter.ToInt16(ActiveByteTemp, 10);
                        Pack[5] = BitConverter.ToInt16(ActiveByteTemp, 12);
                        Pack[6] = BitConverter.ToInt16(ActiveByteTemp, 14);
                        Pack[7] = BitConverter.ToInt16(ActiveByteTemp, 16);
                        Pack[8] = BitConverter.ToInt16(ActiveByteTemp, 18);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_0"), Pack[0]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_1"), Pack[1]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_2"), Pack[2]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_3"), Pack[3]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_4"), Pack[4]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_5"), Pack[5]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_6"), Pack[6]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_7"), Pack[7]);
                        deviceModel.SetDeviceData(new ShortKey(Identify + "_8"), Pack[8]);
                        ActiveByteDataBuffer.RemoveRange(0, 20);
                    }
                    else
                    {
                        // 不是就移除一个
                        ActiveByteDataBuffer.RemoveAt(0);
                    }

                }
            }


        }


        /// <summary>
        /// 发送数据 
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="deviceModel"></param>
        public override void OnReadData(DeviceModel deviceModel, byte[] sendData,  int delay = -1)
        {
            byte[] returnData;

            deviceModel.SendData(sendData, out returnData, true, 100 + ((DataProcessorContext.AutoReadPause)?600:0));

            if (sendData != null && sendData.Length >= 5 && sendData[2] == 0x27 && returnData != null && returnData.Length >= 11)
            {
                returnData = findReturnData(returnData);
                if (returnData != null && returnData.Length == 20)
                {
                    int readReg = sendData[4] << 8 | sendData[3];
                    int rtnReg = returnData[3] << 8 | returnData[2];

                    if (readReg == rtnReg)
                    {
                        short[] Pack = new short[9];
                        Pack[0] = BitConverter.ToInt16(returnData, 4);
                        Pack[1] = BitConverter.ToInt16(returnData, 6);
                        Pack[2] = BitConverter.ToInt16(returnData, 8);
                        Pack[3] = BitConverter.ToInt16(returnData, 10);
                        deviceModel.SetDeviceData(new ShortKey((readReg + 0).ToString("X2")), Pack[0]);
                        deviceModel.SetDeviceData(new ShortKey((readReg + 1).ToString("X2")), Pack[1]);
                        deviceModel.SetDeviceData(new ShortKey((readReg + 2).ToString("X2")), Pack[2]);
                        deviceModel.SetDeviceData(new ShortKey((readReg + 3).ToString("X2")), Pack[3]);
                    }
                }
            }
        }


        /// <summary>
        ///  找到返回的数据
        /// </summary>
        /// <param name="returnData"></param>
        /// <returns></returns>
        public static byte[] findReturnData(byte[] returnData)
        {
            byte[] tempArr = new byte[0];

            for (int i = 0; i < returnData.Length; i++)
            {
                tempArr = returnData.Skip(i).Take(20).ToArray(); ;
                if (tempArr.Length == 20 && tempArr[0] == 0x55 && tempArr[1] == 0x71)
                {
                    return tempArr;
                }
            }
            return null;
        }
    }
}
