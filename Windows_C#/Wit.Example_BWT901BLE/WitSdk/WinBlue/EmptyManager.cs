using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Wit.Bluetooth.WinBlue.Entity;
using Wit.Bluetooth.WinBlue.Interface;

namespace Wit.Bluetooth.WinBlue
{
    /// <summary>
    /// 空实现
    /// </summary>
    public class EmptyManager : IWinBlueManager
    {
        public override BluetoothLEDevice GetDevice(string mac)
        {
            return null;
        }

        public override WinBlueClient GetWinBlueClient(WinBleOption winBleOption)
        {
            return null;
        }

        public override void StartScan()
        {
        }

        public override void StopScan()
        {
        }
    }
}
