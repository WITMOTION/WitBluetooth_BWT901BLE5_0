using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// 串口开发工具类
    /// </summary>
    public class SerialPortUtils
    {

        /// <summary>
        /// 获取串口名称
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
        
        /// <summary>
        /// 获得波特率自动适配延迟
        /// </summary>
        /// <returns></returns>
        public static int GetBaudAutoDelay(int baud)
        {
            if (baud == 0)
            {
                return 80;
            }else if (baud <= 1200)
            {
                return 1000;
            }
            else if (baud > 1200 && baud <= 2400)
            {
                return 500;
            }
            else if (baud > 2400 && baud <= 4800)
            {
                return 400;
            }
            else if (baud > 4800 && baud <= 7200)
            {
                return 250;
            }
            else if (baud > 7200 && baud <= 14400)
            {
                return 200;
            }
            else if (baud > 14400 && baud <= 19200)
            {
                return 200;
            }
            else if (baud > 19200 && baud <= 38400)
            {
                return 200;
            }
            else if (baud > 38400 && baud <= 56000)
            {
                return 200;
            }
            else if (baud > 56000 && baud <= 115200)
            {
                return 200;
            }
            else if (baud > 115200 && baud <= 230400)
            {
                return 200;
            }
            else if (baud > 230400 && baud <= 460800)
            {
                return 100;
            }
            else if (baud > 460800 && baud <= 921600)
            {
                return 100;
            }
            else if (baud > 921600)
            {
                return 100;
            }
            else
            {
                return 100;
            }
        }


        /// <summary>
        /// 获得Modbus自动适配延迟 
        /// </summary>
        /// <returns></returns>
        public static int GetModbusAutoDelay(int baud)
        {
            if (baud == 0)
            {
                return 80;
            }
            else if (baud <= 1200)
            {
                return 500;
            }
            else if (baud > 1200 && baud <= 2400)
            {
                return 300;
            }
            else if (baud > 2400 && baud <= 4800)
            {
                return 200;
            }
            else if (baud > 4800 && baud <= 7200)
            {
                return 150;
            }
            else if (baud > 7200 && baud <= 14400)
            {
                return 80;
            }
            else if (baud > 14400 && baud <= 19200)
            {
                return 70;
            }
            else if (baud > 19200 && baud <= 38400)
            {
                return 60;
            }
            else if (baud > 38400 && baud <= 56000)
            {
                return 60;
            }
            else if (baud > 56000 && baud <= 115200)
            {
                return 60;
            }
            else if (baud > 115200 && baud <= 230400)
            {
                return 60;
            }
            else if (baud > 230400 && baud <= 460800)
            {
                return 60;
            }
            else if (baud > 460800 && baud <= 921600)
            {
                return 60;
            }
            else if (baud > 921600)
            {
                return 60;
            }
            else
            {
                return 60;
            }
        }


    }
}
