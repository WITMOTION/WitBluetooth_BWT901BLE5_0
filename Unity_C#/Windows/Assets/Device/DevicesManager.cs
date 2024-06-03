using Assets.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Device
{
    /*
     * 通过设备管理器来处理多个已经连接的设备操作
     * Process multiple connected device operations through Device Manager
     */
    public class DevicesManager
    {
        private static DevicesManager _instance;

        // 当前设备列表 Current device list
        public Dictionary<string, DeviceModel> devicesDict = new Dictionary<string, DeviceModel>();

        // 当前配置的设备ID The current configured device ID
        public string currentKey = "";

        private DevicesManager() { }

        public static DevicesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DevicesManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 添加一个设备 Add a device
        /// </summary>
        public void AddDevice(DeviceModel device)
        {
            string key = device.deviceId;
            if (!devicesDict.ContainsKey(key)) {
                devicesDict.Add(key, device);
            }
        }

        /// <summary>
        /// 移除一个设备 Remove a device
        /// </summary>
        public void RemoveDevice(string key)
        {
            if (devicesDict.ContainsKey(key))
            {
                devicesDict.Remove(key);
            }
        }

        /// <summary>
        /// 清除所有设备 Clear all devices
        /// </summary>
        public void ClearDevice() { 
            // 关闭所有设备
            foreach (string key in devicesDict.Keys) {
                DeviceModel device = devicesDict[key];  
                device.CloseDevice();
            }    
            devicesDict.Clear();        
        }

        /// <summary>
        /// 获取设备 Get device
        /// </summary>
        public DeviceModel GetDevice(string key) {
            if (devicesDict.ContainsKey(key))
            {
                return devicesDict[key];
            }
            return null;    
        }

        /// <summary>
        /// 获取当前配置设备 Get the current configured device
        /// </summary>
        public DeviceModel GetCurrentDevice() {
            if (string.IsNullOrEmpty(currentKey) || !devicesDict.ContainsKey(currentKey)) {
                return null;
            }
            return devicesDict[currentKey]; 
        }

        /// <summary>
        /// 是否存在连接中的设备 Is there a device in the connection
        /// </summary>
        /// <returns></returns>
        public bool isHaveOpenDevice() {
            foreach(string key in devicesDict.Keys) {
                DeviceModel deviceModel = devicesDict[key];
                if (deviceModel.isOpen) {
                    return true;    
                }
            } 
            return false;
        }
    }
}
