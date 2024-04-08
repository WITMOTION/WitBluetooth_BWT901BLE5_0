using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Wit.Bluetooth.WinBlue.Entity;
using Wit.Bluetooth.WinBlue.Interface;

namespace Wit.Bluetooth.WinBlue
{
    /// <summary>
    /// 蓝牙管理器
    /// </summary>
    class WinBlueManager: IWinBlueManager
    {
        /// <summary>
        /// 搜索器
        /// </summary>
        private WinBlueFinder finder = new WinBlueFinder();

        /// <summary>
        /// 存储检测到的设备
        /// </summary>
        private ConcurrentDictionary<string, BluetoothLEDevice> BluetoothLEDeviceList { get; set; } = new ConcurrentDictionary<string, BluetoothLEDevice>();

        private ConcurrentDictionary<string, WinBlueClient> WinBlueClientList = new ConcurrentDictionary<string, WinBlueClient>();
        /// <summary>
        /// 构造
        /// </summary>
        public WinBlueManager()
        {

        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        public override void StartScan()
        {
            BolScaning = true;

            if (finder != null)
            {
                finder.StopScan();
            }

            // 移除断开的设备
            DisposeDevice();

            // 开始搜索
            finder = new WinBlueFinder();
            finder.OnDeviceFound += Finder_OnDeviceFound;

            // 搜索旧设备
            new Thread(() =>
            {
                // 搜索新设备
                finder.StartScan();
            })
            { IsBackground = true }.Start();
        }


        /// <summary>
        /// 移除断开的设备
        /// </summary>
        private void DisposeDevice()
        {
            var kt = BluetoothLEDeviceList.Keys;
            List<string> keys = new List<string>();

            foreach (var key in kt)
            {
                keys.Add(key);
            }

            for (int i = 0; i < keys.Count; i++)
            {
                string k = keys[i];
                if (WinBlueClientList.ContainsKey(k))
                {
                    if (WinBlueClientList.TryRemove(k, out WinBlueClient winBlueClient))
                    {
                        winBlueClient.Dispose();
                    }
                }
                BluetoothLEDevice device;
                if (BluetoothLEDeviceList.TryGetValue(k, out device))
                {
                    if (device.ConnectionStatus == BluetoothConnectionStatus.Connected)
                    {
                        Console.WriteLine(k);
                        device.Dispose();
                    }
                    BluetoothLEDeviceList.TryRemove(k, out device);
                }

            }
        }

        /// <summary>
        /// 找到蓝牙设备时
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="deviceName"></param>
        /// <param name="device"></param>
        private void Finder_OnDeviceFound(string mac, string deviceName, BluetoothLEDevice device)
        {
            if (!BluetoothLEDeviceList.ContainsKey(mac))
            {
                if (BluetoothLEDeviceList.TryAdd(mac, device))
                {
                    InvokeOnDeviceFound(mac, device.Name);
                }
            }
        }

        /// <summary>
        /// 停止搜索蓝牙
        /// </summary>
        public override void StopScan()
        {
            if (finder != null)
            {
                finder.StopScan();
                BolScaning = false;
            }
        }

        /// <summary>
        /// 按MAC地址查找系统中配对设备
        /// </summary>
        /// <param name="mac"></param>
        public override BluetoothLEDevice GetDevice(string mac)
        {
            if (BluetoothLEDeviceList.ContainsKey(mac))
            {
                return BluetoothLEDeviceList[mac];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 按MAC地址查找系统中配对设备
        /// </summary>
        /// <param name="winBleOption"></param>
        public override WinBlueClient GetWinBlueClient(WinBleOption  winBleOption)
        {
            if (!WinBlueClientList.ContainsKey(winBleOption.Mac))
            {
                WinBlueClientList.TryAdd(winBleOption.Mac, new WinBlueClient(winBleOption));
            }

            return WinBlueClientList[winBleOption.Mac];
        }

    }
}