using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Roles
{

    /// <summary>
    /// 空的数据处理器
    /// </summary>
    public class EmptyProcessor : IDataProcessor
    {
        public override void OnClose(DeviceModel deviceModel)
        {
        }

        public override void OnOpen(DeviceModel deviceModel)
        {
        }

        public override void OnUpdate(DeviceModel baseDeviceModel)
        {
        }
    }
}
