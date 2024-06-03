using Assets.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Bluetooth
{
    /*
     * 蓝牙搜索器，在这里扫描蓝牙设备并回调给UI
     * Bluetooth searcher, scan Bluetooth devices here and call back to the UI
     */
    public class BlueScanner
    {
        private static BlueScanner _instance;

        // 搜索到的设备列表 List of searched devices
        Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();

        public delegate void ScanEventHandler(string arg1, string arg2);
        // 找到设备事件 Find device events
        public event ScanEventHandler OnFindDevice;
        // 扫描结束事件 Scan end event
        public event ScanEventHandler OnScanFinished;

        private BlueScanner(){}

        public static BlueScanner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BlueScanner();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 开始扫描 Start scanning
        /// </summary>
        public void StartScan()
        {
            try
            {
                // 关闭现有设备
                DevicesManager.Instance.ClearDevice();
                BleApi.Quit();  
                devices.Clear();
                Thread.Sleep(200);

                BleApi.StartDeviceScan();
                new Thread(ScanDevice).Start();   
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 搜索设备线程 Search Device Thread
        /// </summary>
        private void ScanDevice()
        {
            Thread.Sleep(500);  
            BleApi.DeviceUpdate res = new BleApi.DeviceUpdate();
            BleApi.ScanStatus status;
            do
            {
                // 推出一个设备 Launch a device
                status = BleApi.PollDevice(ref res, false);
                if (status == BleApi.ScanStatus.AVAILABLE)
                {
                    // 如果已经在devices中就不新建 If it is already in devices, do not create a new one
                    if (!devices.ContainsKey(res.id))
                        devices[res.id] = new Dictionary<string, string>() {
                            { "name", "" },
                            { "isConnectable", "False" }
                        };
                    // 更新名称 Update Name
                    if (res.nameUpdated)
                        devices[res.id]["name"] = res.name;
                    // 更新连接状态 Update connection status
                    if (res.isConnectableUpdated)
                        devices[res.id]["isConnectable"] = res.isConnectable.ToString();
                    // 仅考虑WT的设备和可连接的设备 Only consider WT devices and connected devices
                    if (devices[res.id]["name"].Contains("WT") && devices[res.id]["isConnectable"] == "True")
                    {
                        // 搜索到设备事件 Search for device events
                        OnFindDevice?.Invoke(devices[res.id]["name"], res.id);  
                    }
                }
                // 搜索完成 Search complete
                else if (status == BleApi.ScanStatus.FINISHED)
                {
                    Debug.Log("搜索设备完成");
                    // 搜索完成事件
                    OnScanFinished?.Invoke("", ""); 
                }
            }
            // 持续搜索直到搜索BleApi.ScanStatus状态改变  Until searching for BleApi ScanStatus status changed
            while (status == BleApi.ScanStatus.AVAILABLE);
        }

        /// <summary>
        /// 结束扫描 End scan
        /// </summary>
        public void StopScan() {
            BleApi.StopDeviceScan();
        }  
    }
}
