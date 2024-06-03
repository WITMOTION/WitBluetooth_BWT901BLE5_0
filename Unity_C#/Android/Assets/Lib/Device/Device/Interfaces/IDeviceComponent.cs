using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;

namespace Wit.SDK.Sensor.Device.Interfaces
{
    /// <summary>
    /// 设备组件接口
    /// </summary>
    public interface IDeviceComponent
    {
        /// <summary>
        /// 设备打开时
        /// </summary>
        /// <param name="deviceModel"></param>
        void OnOpen(DeviceModel deviceModel);

        /// <summary>
        /// 设备关闭时
        /// </summary>
        /// <param name="deviceModel"></param>
        void OnClose(DeviceModel deviceModel);

        /// <summary>
        /// 被从设备中移除时
        /// </summary>
        void OnRemove();

        /// <summary>
        /// 收到数据时
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="data"></param>
        void OnReceiveData(DeviceModel deviceModel, byte[] data);

        /// <summary>
        /// 键值更新时
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void OnKeyUpdate(DeviceModel deviceModel,string key, object value);

        /// <summary>
        /// 监听键值更新时
        /// </summary>
        /// <param name="deviceModel"></param>
        void OnUpdate(DeviceModel deviceModel);

        /// <summary>
        /// 读取数据时
        /// </summary>
        void OnReadData(DeviceModel deviceModel, byte[] sendData, int delay = -1);

        /// <summary>
        /// 发送数据时
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="data"></param>
        void OnSend(DeviceModel deviceModel, byte[] data);
    }
}
