using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Enum;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// 维特智能传感器帮助类
    /// </summary>
    public class WitSensorHelper
    {
        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="reg"></param>
        public static void ReadReg(DeviceModel device, WitSensorProtocol sensorPotocol, int reg)
        {
            byte[] sendBytes = new byte[0];
            if (sensorPotocol == WitSensorProtocol.Modbus16Protocol)
            {
                sendBytes = Modbus16Utils.GetRead(byte.Parse(device.GetAddr()), reg, 4);
            }
            else
            {
                sendBytes = new byte[] { 0xff, 0xaa, 0x27, (byte)reg, 0 };
            }

            device.ReadData(sendBytes);
        }

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="reg"></param>
        public static void WriteReg(DeviceModel device, WitSensorProtocol sensorPotocol, int reg, ushort value)
        {
            byte[] sendBytes = new byte[8];
            if (sensorPotocol == WitSensorProtocol.Modbus16Protocol)
            {
                sendBytes = Modbus16Utils.GetWrite(byte.Parse(device.GetAddr()), reg, value);
            }
            else
            {
                sendBytes = WitProtocolUtils.GetWrite(reg, value);
            }

            device.ReadData(sendBytes, 20);
        }

    }
}
