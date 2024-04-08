using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Sensor.Device.Context;

namespace Wit.SDK.Modular.Sensor.DeviceManager
{

    /// <summary>
    /// 设备管理器
    /// </summary>
    public class DeviceManager: DeviceManagerEvents
    {
        #region 单例设计模式
        // 实例
        private static DeviceManager instance;
        /// <summary>
        /// 获得实例
        /// </summary>
        /// <returns></returns>
        public static DeviceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new DeviceManager();
            }
            return instance;
        }

        /// <summary>
        /// 私有构造
        /// </summary>
        private DeviceManager()
        {

        }
        #endregion

        /// <summary>
        /// 当前设备
        /// </summary>
        public List<DeviceModel> DeviceModels
        {
            get
            {
                return DeviceDict.Values.ToList();
            }
        }

        /// <summary>
        /// 设备字典
        /// </summary>
        public ConcurrentDictionary<string,DeviceModel> DeviceDict = new ConcurrentDictionary<string,DeviceModel>();

        /// <summary>
        /// 第一个设备
        /// </summary>
        public DeviceModel FirstDeviceModel
        {
            get
            {
                if (DeviceDict.Count == 0)
                {
                    return null;
                }
                return DeviceDict.ElementAt(0).Value;
            }
        }

        /// <summary>
        /// 用设备名称查找设备
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public DeviceModel FindDeviceById(string deviceId)
        {
            DeviceModel result;

            if (DeviceDict.TryGetValue(deviceId, out result)) { 
                return result;
            }
            return null;
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        public void AppendDevice(DeviceModel deviceModel) {
            if (FindDeviceById(deviceModel.DeviceId) == null) { 
                DeviceDict.TryAdd(deviceModel.DeviceId, deviceModel);
                InvokeOnAppendDevice(deviceModel);
                deviceModel.OnClosed += DeviceModel_OnClosed;
                deviceModel.OnOpened += DeviceModel_OnOpened;
            }
        }

        /// <summary>
        /// 设备打开事件
        /// </summary>
        /// <param name="deviceModel"></param>
        private void DeviceModel_OnOpened(DeviceModel deviceModel)
        {
            InvokeOnOpenDevice(deviceModel);
        }

        /// <summary>
        /// 设备关闭事件
        /// </summary>
        /// <param name="deviceModel"></param>
        private void DeviceModel_OnClosed(DeviceModel deviceModel)
        {
            InvokeOnCloseDevice(deviceModel);
        }

        /// <summary>
        /// 添加一批设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public void AppendDevice(List<DeviceModel> deviceModels)
        {
            for (int i = 0; i < deviceModels.Count; i++)
            {
                AppendDevice(deviceModels[i]);
            }
        }

        /// <summary>
        /// 移除设备
        /// </summary>
        public void RemoveDevice(DeviceModel deviceModel) {
            CloseDevice(deviceModel.DeviceId);
            if (ContainsDevice(deviceModel)) {
                DeviceModel de;
                DeviceDict.TryRemove(deviceModel.DeviceId,out de);
                InvokeOnRemoveDevice(deviceModel);
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public void CloseDevice(string deviceId)
        {
            DeviceModel deviceModel = FindDeviceById(deviceId);
            if (deviceModel != null) { 
                CloseDevice(deviceModel);
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public void CloseDevice(DeviceModel deviceModel)
        {
            deviceModel.CloseDevice();
        }

        /// <summary>
        /// 清除所有设备
        /// </summary>
        public void ClearDevice()
        {
            List<DeviceModel> copyList = new List<DeviceModel> ();

            // 关闭所有设备
            for (int i = 0; i < DeviceDict.Count; i++)
            {
                copyList.Add(DeviceDict.ElementAt(i).Value);
            }

            for (int i = 0; i < copyList.Count; i++)
            {
                DeviceModel deviceModel = copyList[i];
                RemoveDevice(deviceModel);
            }
        }

        /// <summary>
        /// 是否存在某个设备
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public bool ContainsDevice(DeviceModel deviceModel)
        {
            return FindDeviceById(deviceModel.DeviceId)!= null;
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="deviceModel"></param>
        public void OpenDevice(DeviceModel deviceModel)
        {
            AppendDevice(deviceModel);
            deviceModel.OpenDevice();
        }
    }
}
