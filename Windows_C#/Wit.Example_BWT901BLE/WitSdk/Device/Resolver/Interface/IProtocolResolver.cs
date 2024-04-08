using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Sensor.Device.Interfaces;

namespace Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface
{

    /// <summary>
    /// 协议解析器接口
    /// </summary>
    public abstract class IProtocolResolver : IDeviceComponent
    {

        /// <summary>
        /// 读取数据时
        /// </summary>
        public abstract void OnReadData(DeviceModel deviceModel, byte[] sendData, int delay = -1);

        /// <summary>
        /// 收到数据时
        /// </summary>
        /// <param name="data"></param>
        public abstract void OnReceiveData(DeviceModel deviceModel, byte[] data);

        public void OnClose(DeviceModel deviceModel)
        {
        }

        public void OnKeyUpdate(DeviceModel deviceModel, string key, object value)
        {
        }

        public void OnOpen(DeviceModel deviceModel)
        {
        }

        public void OnRemove()
        {
        }

        public void OnSend(DeviceModel deviceModel, byte[] data)
        {
        }

        public void OnUpdate(DeviceModel deviceModel)
        {
        }
    }

}
