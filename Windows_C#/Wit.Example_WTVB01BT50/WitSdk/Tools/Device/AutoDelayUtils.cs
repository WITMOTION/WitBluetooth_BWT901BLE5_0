using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.Connector.Role;
using Wit.SDK.Modular.Sensor.Utils;

namespace Wit.SDK.Modular.Sensor.Modular.Resolver.Utils
{
    /// <summary>
    /// 自动延时工具类
    /// </summary>
    public class AutoDelayUtils
    {
        /// <summary>
        /// 获得延迟
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public static int GetAutoDelay(int delay, DeviceModel deviceModel)
        {
            if (delay == -1)
            {
                if (deviceModel.Connector is SPConnector)
                {
                    var spc = deviceModel.Connector as SPConnector;
                    delay = SerialPortUtils.GetBaudAutoDelay(spc.SerialPortConfig.BaudRate);
                }
                else
                {
                    delay = 100;
                }
            }
            return delay;
        }

        /// <summary>
        /// 获得延迟
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public static int GetModbusAutoDelay(int delay, DeviceModel deviceModel)
        {
            if (delay == -1)
            {
                if (deviceModel.Connector is SPConnector)
                {
                    var spc = deviceModel.Connector as SPConnector;
                    delay = SerialPortUtils.GetModbusAutoDelay(spc.SerialPortConfig.BaudRate);
                }
                else
                {
                    delay = 100;
                }
            }
            return delay;
        }
    }
}
