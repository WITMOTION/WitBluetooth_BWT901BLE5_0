using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Wit.Bluetooth.Utils;
using Wit.Bluetooth.WinBlue.Entity;

namespace Wit.Bluetooth.WinBlue.Interface
{
    /// <summary>
    /// 蓝牙管理器
    /// </summary>
    public abstract class IWinBlueManager
    {
        /// <summary>
        /// 是否搜索中
        /// </summary>
        public bool BolScaning { get; protected set; } = false;

        /// <summary>
        /// 获取设备列表委托
        /// </summary>
        public delegate void DeviceFoundEvent(string Mac, string DeviceName);
        /// <summary>
        /// 获取设备列表事件
        /// </summary>
        public event DeviceFoundEvent OnDeviceFound;

        /// <summary>
        /// 获得设备正确回调
        /// </summary>
        /// <param name="result"></param>
        public delegate void OnGetDeviceSuccess(BluetoothLEDevice result);

        /// <summary>
        /// 获得设备错误回调
        /// </summary>
        public delegate void OnGetDeviceError();

        /// <summary>
        /// 构造
        /// </summary>
        public IWinBlueManager()
        {

        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        public abstract void StartScan();

        /// <summary>
        /// 停止搜索蓝牙
        /// </summary>
        public abstract void StopScan();

        /// <summary>
        /// 按MAC地址查找系统中配对设备
        /// </summary>
        /// <param name="mac"></param>
        public abstract BluetoothLEDevice GetDevice(string mac);
        /// <summary>
        /// 按MAC地址查找系统中配对蓝牙
        /// </summary>
        /// <param name="winBleOption"></param>
        /// <returns></returns>
        public abstract WinBlueClient GetWinBlueClient(WinBleOption winBleOption);

        /// <summary>
        /// 调用发现设备事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>

        protected void InvokeOnDeviceFound(string key, string name)
        {
            OnDeviceFound?.Invoke(key, name);
        }
    }

}
