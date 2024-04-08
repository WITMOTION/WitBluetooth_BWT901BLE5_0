using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Wit.Example_WTVB01BT50.vb01.Data;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Context;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Utils;
using Wit.SDK.Modular.Sensor.Utils;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Roles
{
    /// <summary>
    /// 蓝牙5.0数据处理
    /// </summary>
    public class Wtvb01Processor : IDataProcessor
    {

        public static string PowerPercent { get; } = "PowerPercent";

        public static string Power { get; } = "Power";

        /// <summary>
        /// 读取数据线程是否运行
        /// </summary>
        private bool ReadDataThreadRuning = false;

        /// <summary>
        /// 记录key值切换器
        /// </summary>
        private RecordKeySwitch RecordKeySwitch = new RecordKeySwitch();

        /// <summary>
        /// 数据刷新的key值
        /// </summary>
        private List<string> UpdateKeys = new List<string>() { "61_0", "61_1"};

        /// <summary>
        /// 设备模型
        /// </summary>
        public DeviceModel DeviceModel { get; private set; }

        public override void OnOpen(DeviceModel deviceModel)
        {
            this.DeviceModel = deviceModel;

            // 传入刷新数据的key值
            RecordKeySwitch.Open(deviceModel, UpdateKeys);
            //启动读取线程
            Thread th = new Thread(ReadDataThread) { IsBackground = true };
            ReadDataThreadRuning = true;
            th.Start();
        }

        public override void OnClose(DeviceModel deviceModel)
        {
            ReadDataThreadRuning = false;
            // 关闭key值切换器
            RecordKeySwitch.Close();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="baseDeviceModel"></param>
        public override void OnUpdate(DeviceModel baseDeviceModel)
        {
            //解算寄存器
            ParseRegData();
        }

        /// <summary>
        /// 读取数据线程
        /// </summary>
        private void ReadDataThread()
        {
            int count = 0;

            while (ReadDataThreadRuning)
            {
                try
                {
                    // 暂停读取
                    while (DataProcessorContext.AutoReadPause) Thread.Sleep(1000);
                    
                    // 控制读取频率
                    Thread.Sleep(DataProcessorContext.AutoReadInterval);

                    // 不需要读那么快的数据
                    if (++count % 50 == 0) { 
                        DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 64 00"));//电量
                    }

                    // 读版本号
                    ReadVersionNumberReg(DeviceModel);
                }
                catch (Exception e)
                {
                    //有异常但不处理
                    Debug.WriteLine("BWT901BLECL5_0DataProcessor:自动读取数据异常");
                }
            }
        }

        /// <summary>
        /// 解算寄存器
        /// </summary>
        private void ParseRegData()
        {
            // 版本号
            var reg2e = DeviceModel.GetDeviceData("2E");// 版本号
            var reg2f = DeviceModel.GetDeviceData("2F");// 版本号

            // 如果有版本号
            if (string.IsNullOrEmpty(reg2e) == false &&
                string.IsNullOrEmpty(reg2f) == false)
            {
                var reg2eValue = (ushort)short.Parse(reg2e);
                var vbytes = BitConverter.GetBytes((ushort)short.Parse(reg2e)).Concat(BitConverter.GetBytes((ushort)short.Parse(reg2f))).ToArray();
                UInt32 tempVerSion = BitConverter.ToUInt32(vbytes, 0);
                string sbinary = Convert.ToString(tempVerSion, 2);
                sbinary = ("").PadLeft((32 - sbinary.Length), '0') + sbinary;
                if (sbinary.StartsWith("1"))//新版本号
                {
                    string tempNewVS = Convert.ToUInt32(sbinary.Substring(4 - 3, 14 + 3), 2).ToString();
                    tempNewVS += "." + Convert.ToUInt32(sbinary.Substring(18, 6), 2);
                    tempNewVS += "." + Convert.ToUInt32(sbinary.Substring(24), 2);
                    DeviceModel.SetDeviceData(WTVB01SensorKey.VersionNumber, tempNewVS);
                }
                else
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.VersionNumber, reg2eValue.ToString());
                }
            }

            //振动速度
            var reg61_0 = DeviceModel.GetDeviceData("61_0");// 
            var reg61_1 = DeviceModel.GetDeviceData("61_1");// 
            var reg61_2 = DeviceModel.GetDeviceData("61_2");// 
            // 如果回传了数据包
            if (!string.IsNullOrEmpty(reg61_0) &&
                !string.IsNullOrEmpty(reg61_1) &&
                !string.IsNullOrEmpty(reg61_2)
                )
            {
                // 解算数据,并且保存到设备数据里
                DeviceModel.SetDeviceData(WTVB01SensorKey.VX, ((ushort)short.Parse(reg61_0)).ToString());
                DeviceModel.SetDeviceData(WTVB01SensorKey.VY, ((ushort)short.Parse(reg61_1)).ToString());
                DeviceModel.SetDeviceData(WTVB01SensorKey.VZ, ((ushort)short.Parse(reg61_2)).ToString());
            }

            //振动角度
            var reg61_3 = DeviceModel.GetDeviceData("61_3");// 
            var reg61_4 = DeviceModel.GetDeviceData("61_4");// 
            var reg61_5 = DeviceModel.GetDeviceData("61_5");// 
            // 如果回传了数据包
            if (!string.IsNullOrEmpty(reg61_3) &&
                !string.IsNullOrEmpty(reg61_4) &&
                !string.IsNullOrEmpty(reg61_5)
                )
            {
                // 解算数据,并且保存到设备数据里
                DeviceModel.SetDeviceData(WTVB01SensorKey.ADX, (double.Parse(reg61_3) / 32768.0 * 180).ToString("f3"));
                DeviceModel.SetDeviceData(WTVB01SensorKey.ADY, (double.Parse(reg61_4) / 32768.0 * 180).ToString("f3"));
                DeviceModel.SetDeviceData(WTVB01SensorKey.ADZ, (double.Parse(reg61_5) / 32768.0 * 180).ToString("f3"));
            }

            // 温度
            var reg61_6 = DeviceModel.GetDeviceData("61_6");
            // 如果回传了温度数据包
            if (!string.IsNullOrEmpty(reg61_6))
            {
                DeviceModel.SetDeviceData(WTVB01SensorKey.TEMP, (double.Parse(reg61_6) / 100.0).ToString("f2"));
            }

            //振动位移
            var reg61_7 = DeviceModel.GetDeviceData("61_7");// 
            var reg61_8 = DeviceModel.GetDeviceData("61_8");// 
            var reg61_9 = DeviceModel.GetDeviceData("61_9");// 
            // 如果回传了数据包
            if (!string.IsNullOrEmpty(reg61_7) &&
                !string.IsNullOrEmpty(reg61_8) &&
                !string.IsNullOrEmpty(reg61_9)
                )
            {
                // 解算数据,并且保存到设备数据里
                DeviceModel.SetDeviceData(WTVB01SensorKey.DX, reg61_7);
                DeviceModel.SetDeviceData(WTVB01SensorKey.DY, reg61_8);
                DeviceModel.SetDeviceData(WTVB01SensorKey.DZ, reg61_9);
            }

            //振动频率
            var reg61_10 = DeviceModel.GetDeviceData("61_10");// 
            var reg61_11 = DeviceModel.GetDeviceData("61_11");// 
            var reg61_12 = DeviceModel.GetDeviceData("61_12");// 
            // 如果回传了数据包
            if (!string.IsNullOrEmpty(reg61_10) &&
                !string.IsNullOrEmpty(reg61_11) &&
                !string.IsNullOrEmpty(reg61_12)
                )
            {
                // 解算数据,并且保存到设备数据里
                DeviceModel.SetDeviceData(WTVB01SensorKey.HZX, reg61_10);
                DeviceModel.SetDeviceData(WTVB01SensorKey.HZY, reg61_11);
                DeviceModel.SetDeviceData(WTVB01SensorKey.HZZ, reg61_12);
            }


            // 电量
            var regPower = DeviceModel.GetDeviceData("64");
            if (!string.IsNullOrEmpty(regPower))
            {

                int regPowerValue = int.Parse(regPower);

                // 计算电量百分比
                if (regPowerValue >= 396)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "100");
                }
                else if (regPowerValue >= 393 && regPowerValue < 396)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "90");
                }
                else if (regPowerValue >= 387 && regPowerValue < 393)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "75");
                }
                else if (regPowerValue >= 382 && regPowerValue < 387)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "60");
                }
                else if (regPowerValue >= 379 && regPowerValue < 382)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "50");
                }
                else if (regPowerValue >= 377 && regPowerValue < 379)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "40");
                }
                else if (regPowerValue >= 373 && regPowerValue < 377)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "30");
                }
                else if (regPowerValue >= 370 && regPowerValue < 373)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "20");
                }
                else if (regPowerValue >= 368 && regPowerValue < 370)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "15");
                }
                else if (regPowerValue >= 350 && regPowerValue < 368)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "10");
                }
                else if (regPowerValue >= 340 && regPowerValue < 350)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "5");
                }
                else if (regPowerValue < 340)
                {
                    DeviceModel.SetDeviceData(WTVB01SensorKey.PowerPercent, "0");
                }

                // 电量原始值
                DeviceModel.SetDeviceData(WTVB01SensorKey.Power, regPowerValue.ToString());
            }

        }

        /// <summary>
        /// 读取版本号寄存器
        /// </summary>
        /// <param name="deviceModel"></param>
        private void ReadVersionNumberReg(DeviceModel deviceModel)
        {
            // 读版本号
            if (deviceModel.GetDeviceData("2E") == null)
            {
                // 读版本号
                deviceModel.ReadData(new byte[] { 0xff, 0xaa, 0x27, 0x2E, 0x00 });
                Thread.Sleep(20);
            }
        }
    }
}
