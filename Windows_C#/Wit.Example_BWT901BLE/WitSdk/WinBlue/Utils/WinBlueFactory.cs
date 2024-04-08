using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Wit.Bluetooth.Utils;
using Wit.Bluetooth.WinBlue.Interface;

namespace Wit.Bluetooth.WinBlue.Utils
{
    /// <summary>
    /// 蓝牙管理器工厂类
    /// </summary>
    public class WinBlueFactory
    {
        /// <summary>
        /// 实例
        /// </summary>
        private static IWinBlueManager Instance = null;

        /// <summary>
        /// 获得实例
        /// </summary>
        /// <returns></returns>
        public static IWinBlueManager GetInstance()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = new WinBlueManager();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Instance = new EmptyManager();
            }

            return Instance;
        }
    }

}
