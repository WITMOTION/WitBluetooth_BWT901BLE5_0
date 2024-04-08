using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Sensor.Device.Interfaces;

namespace Wit.SDK.Sensor.Device
{
    /// <summary>
    /// 设备事件
    /// </summary>
    public abstract class DeviceEvents
    {
        /// <summary>
        /// 设备组件列表
        /// </summary>
        private List<IDeviceComponent> DeviceComponents = new List<IDeviceComponent>();

        /// <summary>
        /// 监听的key值改变事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void OnListenKeyUpdateHandler(DeviceModel deviceModel);

        public event OnListenKeyUpdateHandler OnListenKeyUpdate;

        /// <summary>
        /// 收到数据事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void OnReceiveDataHandler(DeviceModel deviceModel, byte[] data);

        public event OnReceiveDataHandler OnReceiveData;

        /// <summary>
        /// 读取数据事件
        /// </summary>
        public delegate void OnReadDataEvent(DeviceModel deviceModel, byte[] sendData, int delay = -1);

        public event OnReadDataEvent OnReadData;

        /// <summary>
        /// 发送数据事件
        /// </summary>
        public delegate void OnSendDataEvent(DeviceModel deviceModel, byte[] data);

        public event OnSendDataEvent OnSendData;

        /// <summary>
        /// key刷新事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void OnKeyUpdateHandler(DeviceModel deviceModel, string key, object value);

        /// <summary>
        ///  key刷新事件
        /// </summary>
        public event OnKeyUpdateHandler OnKeyUpdate;

        /// <summary>
        /// 打开后事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void OpenedEvent(DeviceModel deviceModel);

        public event OpenedEvent OnOpened;

        /// <summary>
        /// 关闭后事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public delegate void ClosedEvent(DeviceModel deviceModel);

        public event ClosedEvent OnClosed;


        /// <summary>
        /// 添加设备组件
        /// </summary>
        public void AddComponent(IDeviceComponent compo) {
            if (DeviceComponents.Contains(compo)) {
                return;
            }
            DeviceComponents.Add(compo);
        }

        /// <summary>
        /// 移除设备组件
        /// </summary>
        public void RemoveComponent(IDeviceComponent compo)
        {
            DeviceComponents.Remove(compo);
            compo.OnRemove();
        }

        /// <summary>
        /// 调用关闭设备事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public void InvokeOnClosed(DeviceModel deviceModel)
        {
            new Thread(() =>
            {
                for (int i = 0; i < DeviceComponents.Count; i++)
                {
                    try
                    {
                        var compo = DeviceComponents[i];
                        compo.OnClose(deviceModel);
                    }
                    catch (Exception ex) { 
                        Debug.WriteLine(ex);
                    }
                }

                OnClosed?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 调用键值更新事件
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void InvokeOnKeyUpdate(DeviceModel deviceModel,string key,object value) {

            for (int i = 0; i < DeviceComponents.Count; i++)
            {
                try
                {
                    var compo = DeviceComponents[i];
                    compo.OnKeyUpdate(deviceModel,key,value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            OnKeyUpdate?.Invoke(deviceModel,key,value);
        }

        /// <summary>
        /// 调用监听键值更新事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public void InvokeOnListenKeyUpdate(DeviceModel deviceModel)
        {

            for (int i = 0; i < DeviceComponents.Count; i++)
            {
                try
                {
                    var compo = DeviceComponents[i];
                    compo.OnUpdate(deviceModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            OnListenKeyUpdate?.Invoke(deviceModel);
        }

        /// <summary>
        /// 调用收到数据事件
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="data"></param>
        public void InvokeOnReceiveData(DeviceModel deviceModel, byte[] data)
        {
            for (int i = 0; i < DeviceComponents.Count; i++)
            {
                try
                {
                    var compo = DeviceComponents[i];
                    compo.OnReceiveData(deviceModel,data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            OnReceiveData?.Invoke(deviceModel,data);
        }


        /// <summary>
        /// 调用读取数据事件
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="data"></param>
        /// <param name="delay"></param>
        public void InvokeOnReadData(DeviceModel deviceModel, byte[] data, int delay)
        {
            for (int i = 0; i < DeviceComponents.Count; i++)
            {
                try
                {
                    var compo = DeviceComponents[i];
                    compo.OnReadData(deviceModel, data, delay);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            OnReadData?.Invoke(deviceModel, data, delay);
        }

        /// <summary>
        /// 调用发送设备事件
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="data"></param>
        public void InvokeOnSendData(DeviceModel deviceModel, byte[] data)
        {
            new Thread(() =>
            {
                for (int i = 0; i < DeviceComponents.Count; i++)
                {
                    try
                    {
                        var compo = DeviceComponents[i];
                        compo.OnSend(deviceModel, data);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                OnSendData?.Invoke(deviceModel, data);
            })
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 调用打开设备事件
        /// </summary>
        /// <param name="deviceModel"></param>
        public void InvokeOnOpened(DeviceModel deviceModel) {

            new Thread(() =>
            {
                for (int i = 0; i < DeviceComponents.Count; i++)
                {
                    try
                    {
                        var compo = DeviceComponents[i];
                        compo.OnOpen(deviceModel);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                OnOpened?.Invoke(deviceModel);
            })
            { IsBackground = true }.Start();
        }
    }
}
