using Assets.Bluetooth;
using Assets.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * 示例程序的唯一场景脚本，场景中的所有逻辑均在这里实现
 * The script file corresponding to the unique scenario of the sample program, where all logic in the scenario is implemented
 */
public class Demo : MonoBehaviour
{
    #region 绑定字段 Binding Fields
    // 扫描按钮 Scan button
    public Text deviceScanButtonText;

    // 扫描状态 Scanning status
    public Text deviceScanStatusText;

    // 当前配置的设备名称 The name of the currently configured device
    public Text configDeviceName;

    // 输入框数据 Input Box Data
    public InputField writeInput;

    // 搜索列表 Search List
    public GameObject deviceScanResult;
    Transform scanResultRoot;

    // 数据列表 Data List
    public GameObject deviceDataResult;
    Transform dataResultRoot;
    #endregion

    #region 属性字段 Attribute Field
    private bool isScan = false;

    // 找到的设备列表 List of devices found
    private List<DeviceModel> findList = new List<DeviceModel>();

    // 数据组件列表 Data component list
    private Dictionary<string, GameObject> dataDict = new Dictionary<string, GameObject>();

    // 设备管理器 Device Manager
    private DevicesManager devicesManager;

    // 上次更新时间 Last update time
    private DateTime lastUpdate;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // 初始化扫描结果和数据列表 Initialize scan results and data list
        scanResultRoot = deviceScanResult.transform.parent;
        deviceScanResult.transform.SetParent(null);
        dataResultRoot = deviceDataResult.transform.parent;
        deviceDataResult.transform.SetParent(null);
        // 获得设备管理器 Obtain Device Manager
        devicesManager = DevicesManager.Instance;
        // 初始化刷新时间 Initialize refresh time
        lastUpdate = DateTime.Now;  
    }

    // Update is called once per frame
    void Update()
    {
        // 添加搜索到的设备 Add searched devices
        if (isScan) {
            if (findList.Count > 0) {
                DeviceModel model = findList[0];
                GameObject g = Instantiate(deviceScanResult, scanResultRoot);
                g.name = model.deviceId;
                g.transform.GetChild(0).GetComponent<Text>().text = model.deviceName;
                g.transform.GetChild(1).GetComponent<Text>().text = model.deviceId;
                g.transform.GetChild(2).GetComponent<Toggle>().isOn = false;
                findList.RemoveAt(0);   
            }
        }

        // 刷新数据 Refresh data
        UpdateData();
    }

    private void UpdateData()
    {
        // UI降频  UI frequency reduction
        //if ((DateTime.Now - lastUpdate).TotalMilliseconds < 100)
        //{
        //    return;
        //}
        try
        {
            foreach (string key in dataDict.Keys)
            {
                GameObject g = dataDict[key];
                DeviceModel model = devicesManager.GetDevice(key);
                if (g != null && model != null)
                {
                    g.transform.GetChild(1).GetComponent<Text>().text = model.GetDataDisplay();
                }
            }
            lastUpdate = DateTime.Now;  
        }
        catch (Exception)
        {
            Debug.LogError("更新数据出错！");
        }
    }

    /// <summary>
    /// 开始和结束扫描 Starting and ending scanning
    /// </summary>
    public void Sacn() {
        BlueScanner scanner = BlueScanner.Instance;
        if (!isScan)
        {
            findList.Clear();
            configDeviceName.text = "No Device";
            dataDict.Clear();
            // 清除扫描列表 Clear scan list
            for (int i = scanResultRoot.childCount - 1; i >= 0; i--) {
                Destroy(scanResultRoot.GetChild(i).gameObject);
            }
            for (int i = dataResultRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(dataResultRoot.GetChild(i).gameObject);
            }

            Debug.Log("开始搜索设备");
            scanner.StartScan();
            scanner.OnFindDevice -= OnFindDevice;
            scanner.OnFindDevice += OnFindDevice;
            isScan = true;
            deviceScanButtonText.text = "Stop Scan";
            deviceScanStatusText.text = "Scanning";
        }
        else
        {
            scanner.OnFindDevice -= OnFindDevice;
            Debug.Log("结束搜索设备");
            scanner.StopScan(); 
            isScan = false;
            deviceScanButtonText.text = "Start Scan";
            deviceScanStatusText.text = "Finished";
        }
    }

    /// <summary>
    /// 找到设备后会回调此方法 After finding the device, this method will be called back
    /// </summary>
    /// <param name="deviceId"></param>
    private void OnFindDevice(string deviceName, string deviceId) {
        DeviceModel device = new DeviceModel(deviceName, deviceId);
        devicesManager.AddDevice(device);
        findList.Add(device);
    }

    /// <summary>
    /// 连接设备 Connecting devices
    /// </summary>
    public void OpenDevice(GameObject g) {
        
        bool isOpen = g.transform.GetChild(2).GetComponent<Toggle>().isOn;
        string key = g.transform.GetChild(1).GetComponent<Text>().text;
        DeviceModel deviceModel = devicesManager.GetDevice(key);

        if (isOpen) 
        {
            Debug.Log("正在打开设备" + deviceModel.deviceName);
            deviceModel?.OpenDevice();
            AddDataObject(deviceModel);

        } else {
            deviceModel?.CloseDevice();
            DestroyDataObject(deviceModel);
        }
    }

    /// <summary>
    /// 在数据列表中添加一个设备 Add a data component
    /// </summary>
    private void AddDataObject(DeviceModel deviceModel) {
        if (deviceModel == null) { return; }
        if (!dataDict.ContainsKey(deviceModel.deviceId)) {
            GameObject g = Instantiate(deviceDataResult, dataResultRoot);
            g.name = deviceModel.deviceId + "_data";
            g.transform.GetChild(0).GetComponent<Text>().text = deviceModel.deviceName;
            g.transform.GetChild(1).GetComponent<Text>().text = "正在加载设备数据。。。";
            dataDict.Add(deviceModel.deviceId, g);
        }
    }

    /// <summary>
    /// 在数据列表中移除一个设备 Remove a data component
    /// </summary>
    private void DestroyDataObject(DeviceModel deviceModel) {
        if (deviceModel == null) { return; }
        if (dataDict.ContainsKey(deviceModel.deviceId)) {
            GameObject g = dataDict[deviceModel.deviceId];
            try
            {
                Destroy(g);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                dataDict.Remove(deviceModel.deviceId);  
            }
        }
    }

    /// <summary>
    /// 选择设备进行配置 Select device for configuration
    /// </summary>
    public void SelectDevice(GameObject obj) {
        string deviceID = obj.name.Replace("_data", "");
        devicesManager.currentKey = deviceID;
        configDeviceName.text = devicesManager.GetCurrentDevice().deviceName;
    }

    /// <summary>
    /// 设置加速度校准 Set acceleration calibration
    /// </summary>
    public void BtnAccCalibration_Click() {
        DeviceModel deviceModel = devicesManager.GetCurrentDevice();
        if (deviceModel != null) {
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x01, 0x01, 0x00 });
        }
    }

    /// <summary>
    /// 设置角度参考 Set angle reference
    /// </summary>
    public void BtnAngleReference_Click() {
        DeviceModel deviceModel = devicesManager.GetCurrentDevice();
        if (deviceModel != null)
        {
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x01, 0x08, 0x00 });
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x00, 0x00, 0x00 });
        }
    }

    /// <summary>
    /// 设置回传速率10hz Set the return rate to 10Hz
    /// </summary>
    public void BtnRRate10_Click() {
        DeviceModel deviceModel = devicesManager.GetCurrentDevice();
        if (deviceModel != null)
        {
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x03, 0x06, 0x00 });
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x00, 0x00, 0x00 });
        }
    }

    /// <summary>
    /// 设置回传速率100hz Set the return rate to 100Hz
    /// </summary>
    public void BtnRRate100_Click() {
        DeviceModel deviceModel = devicesManager.GetCurrentDevice();
        if (deviceModel != null)
        {
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x03, 0x09, 0x00 });
            deviceModel.SendData(new byte[] { 0xff, 0xaa, 0x00, 0x00, 0x00 });
        }
    }

    /// <summary>
    /// 发送原始数据指令 Send raw data instruction
    /// </summary>
    public void SendHexData() {
        DeviceModel deviceModel = devicesManager.GetCurrentDevice();
        if (deviceModel != null) {
            string input = writeInput.text.Replace(" ", "").Replace("-", "");
            byte[] payload = HexStringToByteArray(input);
            deviceModel.SendData(payload);  
        }
    }

    /// <summary>
    /// 字符串转byte数组 String to byte array
    /// </summary>
    private byte[] HexStringToByteArray(string s)
    {
        int NumberChars = s.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
        return bytes;
    }
}
