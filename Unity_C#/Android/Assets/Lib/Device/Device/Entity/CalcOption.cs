using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Wit;
using Wit.SDK;
using Wit.SDK.Modular;
using Wit.SDK.Modular.Sensor;
using Wit.SDK.Sensor.Device.Enum;

namespace Wit.SDK.Modular.Sensor.Device.Entity
{


    /// <summary>
    /// 数据解算配置
    /// </summary>
    public class CalcOption
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 数据key
        /// </summary>
        public string DescribeKey { get; set; }

        /// <summary>
        /// c#脚本
        /// </summary>
        public string CsScript { get; set; }

        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 是否开启零偏
        /// </summary>
        public bool EnableOffset { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType { get; set; } = DataType.String;
    }

}
