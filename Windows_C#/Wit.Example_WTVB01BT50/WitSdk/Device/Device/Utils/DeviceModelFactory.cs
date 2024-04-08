using System;
using Wit.SDK.Modular.Sensor.Device.Entity;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface;

namespace Wit.SDK.Modular.Sensor.Device.Utils
{
    /// <summary>
    /// 设备模型工厂
    /// </summary>
    public class DeviceModelFactory
    {

        /// <summary>
        /// 随机设备id
        /// </summary>
        /// <returns></returns>
        public static string RandomDeviceId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 从配置创建设备模型 
        /// </summary>
        /// <param name="deviceOption">设备配置</param>
        /// <returns></returns>
        public static DeviceModel CreateDeviceModel(string deviceId, string deviceName, DeviceOption deviceOption)
        {

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new Exception("无法创建设备,设备没有id");
            }

            IProtocolResolver protocolResolver = null;
            IDataProcessor dataProcessor = null;

            try
            {
                if (deviceOption.ProtocolResolverType != null)
                    protocolResolver = (IProtocolResolver)deviceOption.ProtocolResolverType.Assembly.CreateInstance(deviceOption.ProtocolResolverType.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception("无法创建协议解析器", ex);
            }

            try
            {
                if (deviceOption.DataProcessorType != null)
                    dataProcessor = (IDataProcessor)deviceOption.DataProcessorType.Assembly.CreateInstance(deviceOption.DataProcessorType.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception("无法创建数据处理器", ex);
            }

            DeviceModel deviceModel = new DeviceModel(
                deviceId,
                deviceName,
                protocolResolver,
                dataProcessor,
                deviceOption.ListenerKey
                );
            return deviceModel;
        }


        /// <summary>
        /// 从配置创建设备模型
        /// </summary>
        /// <param name="deviceOption">设备配置</param>
        /// <returns></returns>
        public static DeviceModel CreateDeviceModel(DeviceOption deviceOption)
        {
            return CreateDeviceModel(RandomDeviceId(), "Unknown", deviceOption);
        }

        /// <summary>
        /// 创建设备模型
        /// </summary>
        /// <param name="deviceId">设备名称</param>
        /// <param name="protocolResolver">解析解析器</param>
        /// <param name="dataProcessor">数据解析器</param>
        /// <param name="dataUpdateListener">监听key</param>
        /// <returns></returns>
        public static DeviceModel CreateDeviceModel(string deviceId, string deviceName, IProtocolResolver protocolResolver, IDataProcessor dataProcessor, string dataUpdateListener)
        {
            DeviceModel deviceModel = new DeviceModel(
                deviceId,
                deviceName,
                protocolResolver,
                dataProcessor,
                dataUpdateListener
                );
            return deviceModel;
        }

    }
}
