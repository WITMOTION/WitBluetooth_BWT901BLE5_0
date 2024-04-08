using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface;

namespace Wit.SDK.Modular.Sensor.Device.Entity
{
    /// <summary>
    /// 设备配置
    /// </summary>
    [Serializable]
    public class DeviceOption
    {
        /// <summary>
        /// 协议解析器类型
        /// </summary>
        public Type ProtocolResolverType { get; set; } = null;

        /// <summary>
        /// 数据解析器类型
        /// </summary>
        public Type DataProcessorType { get; set; } = null;

        /// <summary>
        /// 数据
        /// </summary>
        public string ListenerKey { get; set; } = null;

        /// <summary>
        /// 数据
        /// </summary>
        public List<string> ListenerKeyList { get; set; } = null;

        /// <summary>
        /// 默认的设备数据
        /// </summary>
        public Dictionary<string,string> DefaultData = new Dictionary<string, string>();
    }
}
