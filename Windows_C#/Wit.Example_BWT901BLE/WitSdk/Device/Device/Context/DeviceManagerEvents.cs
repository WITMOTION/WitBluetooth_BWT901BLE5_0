using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.SDK.Device.Sensor.Device.Utils;
using Wit.SDK.Modular.Sensor.Device;

namespace Wit.SDK.Sensor.Device.Context
{
    /// <summary>
    /// 设备管理器事件
    /// </summary>
    public abstract class DeviceManagerEvents: ReadDataSleepManager
    {
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void CloseDeviceEvent(DeviceModel deviceModel);
        public event CloseDeviceEvent OnCloseDevice;
        public void InvokeOnCloseDevice(DeviceModel deviceModel) {
            new Thread(() =>
            {
                OnCloseDevice?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }
        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void OpenDeviceEvent(DeviceModel deviceModel);
        public event OpenDeviceEvent OnOpenDevice;
        public void InvokeOnOpenDevice(DeviceModel deviceModel)
        {
            new Thread(() =>
            {
                OnOpenDevice?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }


        /// <summary>
        /// 移除设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void RemoveDeviceEvent(DeviceModel deviceModel);
        public event RemoveDeviceEvent OnRemoveDevice;
        public void InvokeOnRemoveDevice(DeviceModel deviceModel)
        {
            new Thread(() =>
            {
                OnRemoveDevice?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void AppendDeviceEvent(DeviceModel deviceModel);
        public event AppendDeviceEvent OnAppendDevice;
        public void InvokeOnAppendDevice(DeviceModel deviceModel)
        {
            new Thread(() =>
            {
                OnAppendDevice?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }
    }
}
