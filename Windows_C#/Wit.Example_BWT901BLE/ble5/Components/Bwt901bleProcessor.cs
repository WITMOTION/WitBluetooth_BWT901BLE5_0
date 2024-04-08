using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;
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
    public class Bwt901bleProcessor : IDataProcessor
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

                    // 读磁场类型
                    ReadMagType(DeviceModel);
                    
                    // 控制读取频率
                    Thread.Sleep(DataProcessorContext.AutoReadInterval);

                    DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 3A 00"));//磁场
                    DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 51 00"));//四元数


                    // 不需要读那么快的数据
                    if (++count % 50 == 0) { 
                        DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 64 00"));//电量
                        DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 40 00"));//温度
                    }

                    DeviceModel.ReadData(ByteArrayConvert.HexStringToByteArray("FF AA 27 30 00"));//时间

                    // 读产品序列号
                    ReadSerialNumberReg(DeviceModel);
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
            // 序列号
            short? reg7f = DeviceModel.GetDeviceData(new ShortKey("7F"));// 序列号
            short? reg80 = DeviceModel.GetDeviceData(new ShortKey("80"));// 序列号
            short? reg81 = DeviceModel.GetDeviceData(new ShortKey("81"));// 序列号
            short? reg82 = DeviceModel.GetDeviceData(new ShortKey("82"));// 序列号
            short? reg83 = DeviceModel.GetDeviceData(new ShortKey("83"));// 序列号
            short? reg84 = DeviceModel.GetDeviceData(new ShortKey("84"));// 序列号
            if (reg7f !=null &&
                reg80 !=null &&
                reg81 !=null &&
                reg82 !=null &&
                reg83 !=null &&
                reg84 != null)
            {
                var sbytes = BitConverter.GetBytes((short)reg7f)
                    .Concat(BitConverter.GetBytes((short)reg80))
                    .Concat(BitConverter.GetBytes((short)reg81))
                    .Concat(BitConverter.GetBytes((short)reg82))
                    .Concat(BitConverter.GetBytes((short)reg83))
                    .Concat(BitConverter.GetBytes((short)reg84))
                    .ToArray();
                string sn = Encoding.Default.GetString(sbytes);
                DeviceModel.SetDeviceData(WitSensorKey.SerialNumber, sn);
            }


            var reg50_0 = DeviceModel.GetDeviceData(new ShortKey("30"));// 年月
            var reg50_1 = DeviceModel.GetDeviceData(new ShortKey("31"));// 日时
            var reg50_2 = DeviceModel.GetDeviceData(new ShortKey("32"));// 分秒
            var reg50_3 = DeviceModel.GetDeviceData(new ShortKey("33"));// 毫秒
            // 如果回传了时间数据包就解算时间
            if (reg50_0 != null &&
                reg50_1 != null &&
                reg50_2 != null &&
                reg50_3 != null
                )
            {
                // 解算数据,并且保存到设备数据里
                var yy = 2000 + (byte)reg50_0;
                var MM = (byte)(((short)reg50_0) >> 8);
                var dd = (byte)reg50_1;
                var hh = (byte)((short)(reg50_1) >> 8);
                var mm = (byte)reg50_2;
                var ss = (byte)((short)(reg50_2) >> 8);
                var ms = ((short)reg50_3).ToString("000");


                DeviceModel.SetDeviceData(WitSensorKey.ChipTime, $"{yy}-{MM}-{dd} {hh}:{mm}:{ss}.{ms}");
            }


            // 版本号
            var reg2e = DeviceModel.GetDeviceData(new ShortKey("2E"));// 版本号
            var reg2f = DeviceModel.GetDeviceData(new ShortKey("2F"));// 版本号

            // 如果有版本号
            if (reg2e != null &&
                reg2f != null)   
            {
                var reg2eValue = (ushort)reg2e;
                var vbytes = BitConverter.GetBytes((ushort)reg2e).Concat(BitConverter.GetBytes((ushort)reg2f)).ToArray();
                UInt32 tempVerSion = BitConverter.ToUInt32(vbytes, 0);
                string sbinary = Convert.ToString(tempVerSion, 2);
                sbinary = ("").PadLeft((32 - sbinary.Length), '0') + sbinary;
                if (sbinary.StartsWith("1"))//新版本号
                {
                    string tempNewVS = Convert.ToUInt32(sbinary.Substring(4 - 3, 14 + 3), 2).ToString();
                    tempNewVS += "." + Convert.ToUInt32(sbinary.Substring(18, 6), 2);
                    tempNewVS += "." + Convert.ToUInt32(sbinary.Substring(24), 2);
                    DeviceModel.SetDeviceData(WitSensorKey.VersionNumber, tempNewVS);
                }
                else
                {
                    DeviceModel.SetDeviceData(WitSensorKey.VersionNumber, reg2eValue.ToString());
                }
            }

            //加速度
            var regAx = DeviceModel.GetDeviceData(new ShortKey("61_0"));
            var regAy = DeviceModel.GetDeviceData(new ShortKey("61_1"));
            var regAz = DeviceModel.GetDeviceData(new ShortKey("61_2"));
            // 角速度
            var regWx = DeviceModel.GetDeviceData(new ShortKey("61_3"));
            var regWy = DeviceModel.GetDeviceData(new ShortKey("61_4"));
            var regWz = DeviceModel.GetDeviceData(new ShortKey("61_5"));
            // 角度
            var regAngleX = DeviceModel.GetDeviceData(new ShortKey("61_6"));
            var regAngleY = DeviceModel.GetDeviceData(new ShortKey("61_7"));
            var regAngleZ = DeviceModel.GetDeviceData(new ShortKey("61_8"));
            // 四元数
            var regQ1 = DeviceModel.GetDeviceData(new ShortKey("51"));
            var regQ2 = DeviceModel.GetDeviceData(new ShortKey("52"));
            var regQ3 = DeviceModel.GetDeviceData(new ShortKey("53"));
            var regQ4 = DeviceModel.GetDeviceData(new ShortKey("54"));
            // 温度和电量
            var regTemperature = DeviceModel.GetDeviceData(new ShortKey("40"));
            var regPower = DeviceModel.GetDeviceData(new ShortKey("64"));


            // 加速度解算
            if (regAx!=null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AccX, Math.Round((short)(regAx) / 32768.0 * 16, 3));
            }
            if (regAy != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AccY, Math.Round((short)(regAy) / 32768.0 * 16, 3));
            }
            if (regAz != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AccZ, Math.Round((short)(regAz) / 32768.0 * 16, 3));
            }

            // 角速度解算
            if (regWx != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AsX, Math.Round((short)(regWx) / 32768.0 * 2000, 3));
            }
            if (regWy != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AsY, Math.Round((short)(regWy) / 32768.0 * 2000, 3));
            }
            if (regWz != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AsZ, Math.Round((short)(regWz) / 32768.0 * 2000, 3));
            }

            // 角度
            if (regAngleX != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AngleX, Math.Round((short)(regAngleX) / 32768.0 * 180, 3));
            }
            if (regAngleY != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AngleY, Math.Round((short)(regAngleY) / 32768.0 * 180, 3));
            }
            if (regAngleZ != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.AngleZ, Math.Round((short)(regAngleZ) / 32768.0 * 180, 3));
            }

            // 磁场
            var regHX = DeviceModel.GetDeviceData(new ShortKey("3A"));
            var regHY = DeviceModel.GetDeviceData(new ShortKey("3B"));
            var regHZ = DeviceModel.GetDeviceData(new ShortKey("3C"));
            var magType = DeviceModel.GetDeviceData("72");// 磁场类型
            if (regHX != null &&
                regHY != null &&
                regHZ != null &&
                magType != null)
            {
                short type = short.Parse(magType);
                DeviceModel.SetDeviceData(WitSensorKey.HX, DipSensorMagHelper.GetMagToUt(type, (short)(regHX)));
                DeviceModel.SetDeviceData(WitSensorKey.HY, DipSensorMagHelper.GetMagToUt(type, (short)(regHY)));
                DeviceModel.SetDeviceData(WitSensorKey.HZ, DipSensorMagHelper.GetMagToUt(type, (short)(regHZ)));
                DeviceModel.SetDeviceData(WitSensorKey.HM, Math.Round(Math.Sqrt(Math.Pow(DipSensorMagHelper.GetMagToUt(type, (short)(regHX)), 2) + 
                                                           Math.Pow(DipSensorMagHelper.GetMagToUt(type, (short)(regHY)), 2) + 
                                                           Math.Pow(DipSensorMagHelper.GetMagToUt(type, (short)(regHZ)), 2)), 2));
            }


            // 温度
            if (regTemperature!=null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.T, Math.Round((short)(regTemperature) / 100.0, 2));
            }

            // 电量
            if (regPower != null)
            {

                int regPowerValue = (short)(regPower);

                // 计算电量百分比
                if (regPowerValue >= 396)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 100);
                }
                else if (regPowerValue >= 393 && regPowerValue < 396)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 90);
                }
                else if (regPowerValue >= 387 && regPowerValue < 393)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 75);
                }
                else if (regPowerValue >= 382 && regPowerValue < 387)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 60);
                }
                else if (regPowerValue >= 379 && regPowerValue < 382)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 50);
                }
                else if (regPowerValue >= 377 && regPowerValue < 379)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 40);
                }
                else if (regPowerValue >= 373 && regPowerValue < 377)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 30);
                }
                else if (regPowerValue >= 370 && regPowerValue < 373)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 20);
                }
                else if (regPowerValue >= 368 && regPowerValue < 370)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 15);
                }
                else if (regPowerValue >= 350 && regPowerValue < 368)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 10);
                }
                else if (regPowerValue >= 340 && regPowerValue < 350)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 5);
                }
                else if (regPowerValue < 340)
                {
                    DeviceModel.SetDeviceData(WitSensorKey.PowerPercent, 0);
                }


                // 电量原始值
                DeviceModel.SetDeviceData(Power, regPowerValue.ToString());
            }


            // 四元数
            if (regQ1 != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.Q0, Math.Round((short)regQ1 / 32768.0, 5));
            }
            if (regQ2 != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.Q1, Math.Round((short)regQ2 / 32768.0, 5));
            }
            if (regQ3 != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.Q2, Math.Round((short)regQ3 / 32768.0, 5));
            }
            if (regQ4 != null)
            {
                DeviceModel.SetDeviceData(WitSensorKey.Q3, Math.Round((short)regQ4 / 32768.0, 5));
            }
        }

        /// <summary>
        /// 读取磁场类型寄存器
        /// </summary>
        private void ReadMagType(DeviceModel deviceModel)
        {
            // 读磁场类型
            if (deviceModel.GetDeviceData("72") == null)
            {
                // 读取72磁场类型寄存器,后面解析磁场的时候要用到
                deviceModel.ReadData(new byte[] { 0xff, 0xaa, 0x27, 0x72, 0x00 });
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// 读取序列号寄存器
        /// </summary>
        /// <param name="deviceModel"></param>
        private void ReadSerialNumberReg(DeviceModel deviceModel)
        {
            // 读序列号
            if (deviceModel.GetDeviceData("7F") == null && deviceModel.GetDeviceData("82") == null)
            {
                // 读序列号
                deviceModel.ReadData(new byte[] { 0xff, 0xaa, 0x27, 0x7F, 0x00 });
                deviceModel.ReadData(new byte[] { 0xff, 0xaa, 0x27, 0x7F + 3, 0x00 });
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
