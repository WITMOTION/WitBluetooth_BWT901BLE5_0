using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.Bluetooth.Utils
{
    public class MacUtils
    {
        /// <summary>
        /// 设备id转蓝牙mac地址
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static string DeviceIdToMac(string deviceId) {
            string mac = deviceId;
            //Console.WriteLine(tstr);
            int tmpPlace = mac.LastIndexOf("-");
            if (tmpPlace > 0)
            {
                mac = mac.Substring(tmpPlace + 1);
            }
            return mac;
        }

    }
}
