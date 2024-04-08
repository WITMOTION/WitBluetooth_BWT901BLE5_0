using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Wit.Bluetooth.Utils;
using Wit.Bluetooth.WinBlue.Entity;
using Wit.Bluetooth.WinBlue.Enums;
using Wit.Bluetooth.WinBlue.Interface;
using Wit.Bluetooth.WinBlue.Utils;

namespace Wit.Bluetooth.WinBlue
{
    /// <summary>
    /// 蓝牙连接器
    /// </summary>
    public class WinBlueClient
    {
        // 蓝牙管理器
        public IWinBlueManager bluetoothManager = null;

        // 存储检测到的主服务。
        public GattDeviceService CurrentService { get; set; }

        // 蓝牙设备。
        public BluetoothLEDevice BluetoothDevice { get; set; }

        // 存储检测到的写特征对象。
        public GattCharacteristic CurrentWriteCharacteristic { get; set; }

        // 存储检测到的通知特征对象。
        public GattCharacteristic CurrentNotifyCharacteristic { get; set; }

        // 定义一个委托
        public delegate void ReceiveDataDelegate(BluetoothEvent type, string mac, byte[] data = null);

        // 定义一个事件
        public event ReceiveDataDelegate OnReceive;

        /// <summary>
        /// 是否连接蓝牙
        /// </summary>
        public bool IsConnect = false;

        // 配置
        private WinBleOption Config;

        // 特性通知类型通知启用
        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="config"></param>
        public WinBlueClient(WinBleOption config)
        {
            Config = config;
            bluetoothManager = WinBlueFactory.GetInstance(); ;
        }

        /// <summary>
        /// 按MAC地址直接组装设备ID查找设备
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public async Task Connect()
        {
            BluetoothDevice = bluetoothManager.GetDevice(Config.Mac);
            // 连接状态改变事件
            BluetoothDevice.ConnectionStatusChanged -= CurrentDevice_ConnectionStatusChanged;
            BluetoothDevice.ConnectionStatusChanged += CurrentDevice_ConnectionStatusChanged;
            Guid guid = new Guid(Config.ServiceGuid);

            // 连接中
            OnReceive(BluetoothEvent.Connecting, Config.Mac);

            BluetoothDevice.GetGattServicesForUuidAsync(guid).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    try
                    {
                        GattDeviceServicesResult result = asyncInfo.GetResults();
                        if (result.Services.Count > 0)
                        {
                            CurrentService = result.Services[0];
                            if (CurrentService != null)
                            {
                                await GetCurrentWriteCharacteristic();
                                await GetCurrentNotifyCharacteristic();
                            }
                            IsConnect = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            };
        }

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                string msg = "设备已连接";
                OnReceive(BluetoothEvent.Connected, Config.Mac);
            }
            else
            {
                OnReceive(BluetoothEvent.Disconnected, Config.Mac);
            }
        }

        /// <summary>
        /// 设置写特征对象。
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentWriteCharacteristic()
        {
            if (CurrentService == null) return;
            Guid guid = new Guid(Config.WriteGuid);

            CurrentService.GetCharacteristicsForUuidAsync(guid).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCharacteristicsResult result = asyncInfo.GetResults();
                    if (result.Characteristics.Count > 0)
                    {
                        CurrentWriteCharacteristic = result.Characteristics[0];
                    }
                    else
                    {
                        Thread.Sleep(10);
                        GetCurrentWriteCharacteristic();
                    }
                }
            };
        }

        /// <summary>
        /// 设置通知特征对象。
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentNotifyCharacteristic()
        {
            if (CurrentService == null) return;
            Guid guid = new Guid(Config.NotifyGuid);
            CurrentService.GetCharacteristicsForUuidAsync(guid).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCharacteristicsResult result = asyncInfo.GetResults();
                    if (result.Characteristics.Count > 0)
                    {
                        CurrentNotifyCharacteristic = result.Characteristics[0];
                        CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                        CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;
                        EnableNotifications(CurrentNotifyCharacteristic);

                    }
                    else
                    {
                        OnReceive(BluetoothEvent.Connecting, Config.Mac);
                        Thread.Sleep(10);
                        GetCurrentNotifyCharacteristic();
                    }
                }
            };
        }


        /// <summary>
        /// 特征值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (!IsConnect) return;
            byte[] data;
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
            string mac = "";
            if (sender != null)
            {
                mac = MacUtils.DeviceIdToMac(sender.Service.Device.DeviceId);
                OnReceive(BluetoothEvent.Data, mac, data);
            }
        }

        /// <summary>
        /// 设置特征对象为接收通知对象
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        public void EnableNotifications(GattCharacteristic characteristic)
        {
            characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCommunicationStatus status = asyncInfo.GetResults();
                    if (status == GattCommunicationStatus.Unreachable)
                    {
                        OnReceive(BluetoothEvent.Connecting, Config.Mac);
                        if (CurrentNotifyCharacteristic != null)
                        {
                            EnableNotifications(CurrentNotifyCharacteristic);
                        }
                    }
                    OnReceive(BluetoothEvent.Connecting, Config.Mac);
                }
            };
        }

        /// <summary>
        /// 写出数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Write(byte[] data)
        {
            if (CurrentWriteCharacteristic != null)
            {
                CurrentWriteCharacteristic?.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data), GattWriteOption.WriteWithResponse);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            IsConnect = false;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose() {

            IsConnect = false;
            // 放弃写特征值
            if (CurrentWriteCharacteristic != null)
            {
                CurrentWriteCharacteristic = null;
            }

            // 放弃读特征值
            if (CurrentNotifyCharacteristic != null)
            {
                CurrentNotifyCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                CurrentNotifyCharacteristic = null;
            }

            CurrentService?.Dispose();
            BluetoothDevice?.Dispose();
            BluetoothDevice = null;
            CurrentService = null;
            CurrentWriteCharacteristic = null;
            CurrentNotifyCharacteristic = null;
        }

        /// <summary>
        /// 程序关闭时
        /// </summary>
        ~WinBlueClient() {
            Dispose();
        }
    }
}
