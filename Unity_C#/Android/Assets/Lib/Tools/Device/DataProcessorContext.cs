using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device.Entity;

namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Context
{
    /// <summary>
    /// 数据处理器上下文
    /// </summary>
    public class DataProcessorContext
    {
        /// <summary>
        /// 自动读取间隔
        /// </summary>
        private static int _AutoReadInterval = DefaultAutoReadInterval;

        /// <summary>
        /// 自动读取间隔
        /// </summary>
        public static int AutoReadInterval {
            get { return (_AutoReadInterval > 0) ? _AutoReadInterval : 10; } 
            set { _AutoReadInterval = value; } 
        }

        public static int DefaultAutoReadInterval
        {
            get { return 200; }
        }

        /// <summary>
        /// 暂停自动读取
        /// </summary>
        public static bool AutoReadPause = false;

        /// <summary>
        /// 读取的命令
        /// </summary>
        public static List<CmdBean> ReadCmdList = new List<CmdBean>();
    }
}
