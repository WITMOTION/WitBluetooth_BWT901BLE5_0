using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Roles
{
    public class EmptyResolver : IProtocolResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="deviceModel"></param>
        public override void OnReceiveData(DeviceModel deviceModel, byte[] data)
        {
           
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="deviceModel"></param>
        public override void OnReadData(DeviceModel deviceModel, byte[] sendData, int delay = -1)
        {
        }
    }
}
