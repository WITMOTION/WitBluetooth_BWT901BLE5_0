using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Utils
{

    /// <summary>
    /// 记录key值切换器
    /// 功能：自动匹配记录的key
    /// </summary>
    public class RecordKeySwitch
    {
        /// <summary>
        /// 记录触发器列表
        /// </summary>
        public List<string> recordTriggerList = new List<string> {};

        /// <summary>
        /// 字典 string:key值 long：刷新时间
        /// </summary>
        private Dictionary<string, long> keyUpdateTimeDict = new Dictionary<string, long>();

        /// <summary>
        /// 是否启用切换触发器线程
        /// </summary>
        public bool BolSwitchRecrodThEnable = false;

        /// <summary>
        /// 上次更新时间
        /// </summary>
        public long LastUpdateTs = 0;

        /// <summary>
        /// 基础设备
        /// </summary>
        public DeviceModel DeviceModel { get; private set; }

        /// <summary>
        /// 初始化 启动
        /// </summary>
        /// <param name="deviceModel"></param>
        public void Open(DeviceModel deviceModel, List<string> keyList)
        {
            // 设备
            this.DeviceModel = deviceModel;
            // 记录触发器列表
            this.recordTriggerList = keyList;

            DeviceModel.OnKeyUpdate -= DeviceModel_OnKeyUpdate;
            DeviceModel.OnKeyUpdate += DeviceModel_OnKeyUpdate;
            DeviceModel.OnListenKeyUpdate -= DeviceModel_OnListenKeyUpdate;
            DeviceModel.OnListenKeyUpdate += DeviceModel_OnListenKeyUpdate;

            // 创建线程
            Thread th = new Thread(SwitchKeyTh) { IsBackground = true };
            BolSwitchRecrodThEnable = true;
            th.Start();
        }
        
        /// <summary>
        /// 切换刷新用的key值
        /// </summary>
        private void SwitchKeyTh()
        {
            // 数据刷新有问题就启动线程
            while (BolSwitchRecrodThEnable) {

                Thread.Sleep(3000);
                // 实时时间
                var ts = DateTimeUtils.GetTimeStamp();
                if (ts - LastUpdateTs > 3000)
                {
                    SwitchListenerKey();
                }
            }
        }

        /// <summary>
        /// 切换监听的key
        /// </summary>
        private void SwitchListenerKey()
        {
            if (recordTriggerList.Count == 0)
            {
                return;
            }

            // 最后切换的key值
            string keyLast = recordTriggerList[0];
            double min = 0;
            for (int i = 0; i < recordTriggerList.Count; i++)
            {
                string item = recordTriggerList[i];
                if (keyUpdateTimeDict.ContainsKey(item))
                {
                    double tsVal = keyUpdateTimeDict[item];
                    if (tsVal > min)
                    {
                        min = tsVal;
                        keyLast = item;
                    }
                }
            }
            // 切换监听的key值 让数据刷新继续下去
            DeviceModel.ListenerKey = keyLast;
        }

        /// <summary>
        /// 判断自动更新数据有无问题
        /// </summary>
        /// <param name="deviceModel"></param>
        private void DeviceModel_OnListenKeyUpdate(DeviceModel deviceModel)
        {
            LastUpdateTs = DateTimeUtils.GetTimeStamp();
        }

        /// <summary>
        ///  记录key值刷新时对应的时间
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void DeviceModel_OnKeyUpdate(DeviceModel deviceModel, string key, object value)
        {
            // 如果是可切换的key就赋值给字典
            if (recordTriggerList.Contains(key))
            {                 
                // 记录数据更新时间戳
                keyUpdateTimeDict[key] = DateTimeUtils.GetTimeStamp();
            }
        }

        /// <summary>
        /// 关闭所有线程
        /// </summary>
        public void Close() {
            // 关线程
            BolSwitchRecrodThEnable = false;

            if (DeviceModel != null && DeviceModel.IsOpen)
            {
                // 取消事件委托
                DeviceModel.OnKeyUpdate -= DeviceModel_OnKeyUpdate;
                DeviceModel.OnListenKeyUpdate -= DeviceModel_OnListenKeyUpdate;
            }

        }
    }
}
