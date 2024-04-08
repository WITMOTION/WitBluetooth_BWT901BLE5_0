using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Sensor.Device.Interfaces;

namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface
{
    /// <summary>
    /// 数据处理器接口
    /// </summary>
    public abstract class IDataProcessor : IDeviceComponent
    {
        /// <summary>
        /// 打开设备时
        /// </summary>
        public abstract void OnOpen(DeviceModel deviceModel);

        /// <summary>
        /// 关闭传感器时
        /// </summary>
        public abstract void OnClose(DeviceModel deviceModel);

        /// <summary>
        ///  设备实时数据更新时
        /// </summary>
        public abstract void OnUpdate(DeviceModel deviceModel);

        public void OnRemove()
        {
        }

        public void OnReceiveData(DeviceModel deviceModel, byte[] data)
        {
        }

        public void OnKeyUpdate(DeviceModel deviceModel, string key, object value)
        {
        }

        public void OnReadData(DeviceModel deviceModel, byte[] sendData, int delay = -1)
        {
        }

        public void OnSend(DeviceModel deviceModel, byte[] data)
        {
        }
    }
}
