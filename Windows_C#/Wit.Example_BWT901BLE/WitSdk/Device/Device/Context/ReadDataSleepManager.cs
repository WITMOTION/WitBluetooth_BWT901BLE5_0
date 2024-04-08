using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Sensor.Device.Interfaces;

namespace Wit.SDK.Device.Sensor.Device.Utils
{
    /// <summary>
    /// 读取数据休眠事件
    /// </summary>
    public interface IReadSleepEvent {
        public void OnSleep();
    }

    /// <summary>
    /// 读取数据休眠管理器
    /// </summary>
    public class ReadDataSleepManager
    {
        private static List<IReadSleepEvent> sleeps = new List<IReadSleepEvent>();

        /// <summary>
        /// 添加设备组件
        /// </summary>
        public static void AddReadSleepEvent(IReadSleepEvent e)
        {
            if (sleeps.Contains(e))
            {
                return;
            }
            sleeps.Add(e);
        }

        /// <summary>
        /// 移除设备组件
        /// </summary>
        public static void RemoveReadSleepEvent(IReadSleepEvent e)
        {
            sleeps.Remove(e);
        }

        /// <summary>
        /// 调用休眠事件
        /// </summary>
        public static void InvokeOnSleep()
        {
            for (int i = 0; i < sleeps.Count; i++)
            {
                try
                {
                    sleeps[i].OnSleep();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
    }
}
