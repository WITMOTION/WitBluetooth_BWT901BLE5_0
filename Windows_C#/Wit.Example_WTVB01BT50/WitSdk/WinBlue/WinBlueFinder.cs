using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Wit.Bluetooth.Utils;

namespace Wit.Bluetooth.WinBlue
{
    /// <summary>
    /// 蓝牙管理器
    /// </summary>
    public class WinBlueFinder
    {

        /// <summary>
        /// 蓝牙适配器
        /// </summary>
        private BluetoothLEAdvertisementWatcher deviceWatcher;

        /// <summary>
        /// 存储检测到的设备
        /// </summary>
        //private ConcurrentDictionary<string, BluetoothLEDevice> BluetoothLEDeviceList { get; set; } = new ConcurrentDictionary<string, BluetoothLEDevice>();
        
        /// <summary>
        /// 是否搜索中
        /// </summary>
        public bool BolScaning { get; private set; } = false;

        /// <summary>
        /// 获取设备列表委托
        /// </summary>
        public delegate void DeviceFoundEvent(string mac, string deviceName, BluetoothLEDevice device);

        /// <summary>
        /// 获取设备列表事件
        /// </summary>
        public event DeviceFoundEvent OnDeviceFound;

        /// <summary>
        /// 构造
        /// </summary>
        internal WinBlueFinder()
        {

        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        public void StartScan()
        {
            BolScaning = true;
            deviceWatcher = new BluetoothLEAdvertisementWatcher();
            deviceWatcher.ScanningMode = BluetoothLEScanningMode.Active;
            deviceWatcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;
            deviceWatcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;
            deviceWatcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            deviceWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);
            deviceWatcher.Received += DeviceWatcher_Received;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            try
            {
                deviceWatcher.Start();
            }
            catch (Exception ex) { 
                Debug.Write(ex.Message);
                Debug.Write(ex.StackTrace);
            }
        }

        /// <summary>
        /// 搜索结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            BolScaning = false;
        }

        /// <summary>
        /// 搜索蓝牙列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    if (asyncInfo.GetResults() != null)
                    {
                        BluetoothLEDevice currentDevice = asyncInfo.GetResults();
                        string mac = MacUtils.DeviceIdToMac(currentDevice.DeviceId);
                        Debug.WriteLine($"{mac}      {currentDevice.Name}");
                        OnDeviceFound?.Invoke(mac, currentDevice.Name, currentDevice);
                    }
                }
            };
        }

        /// <summary>
        /// 停止搜索蓝牙
        /// </summary>
        public void StopScan()
        {
            if (deviceWatcher != null) {
                if ((BluetoothLEAdvertisementWatcherStatus.Started == deviceWatcher.Status ||
                    BluetoothLEAdvertisementWatcherStatus.Created == deviceWatcher.Status))
                {
                    deviceWatcher.Stop();
                    deviceWatcher = null;
                    BolScaning = false;
                }
            }
        }
    }

}
