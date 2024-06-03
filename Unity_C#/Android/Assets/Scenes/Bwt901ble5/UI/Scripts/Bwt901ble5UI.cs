using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;

/// <summary>
/// 蓝牙5.0示例程序
/// Bluetooth 5.0 Example Program
/// </summary>
public class Bwt901ble5UI : MonoBehaviour
{
    /// <summary>
    /// 蓝牙管理器
    /// Bluetooth manager
    /// </summary>
    BluetoothManager bluetoothManager;

    /// <summary>
    /// 找到的设备
    /// Found device
    /// </summary>
    private Dictionary<string, BWT901BLE> FoundDeviceDict = new Dictionary<string, BWT901BLE>();

    /// <summary>
    /// 更新3d Box事件
    /// Update Cube events
    /// </summary>
    /// <param name="bWT901BLE"></param>
    public delegate void UpdateBoxHandler(BWT901BLE bWT901BLE);

    public static event UpdateBoxHandler UpdateBox;

    #region 绑定属性 Binding Properties
    public Button ScanBtn;

    public Button StopBtn;

    public Button ConnectBtn;

    public Text ConnectText;

    public Text MsgText;

    public Text DeviceName;

    public Button RRateBtn_1;

    public Button RRateBtn_2;

    public Button AccBtn;

    public Text Info;

    public Text AccX;
    public Text AccY;
    public Text AccZ;
    public Text AsX;
    public Text AsY;
    public Text AsZ;
    public Text AngX;
    public Text AngY;
    public Text AngZ;
    #endregion

    string currentMac = null;

    // Start is called before the first frame update
    void Start()
    {
        MsgText.text = "维特智能蓝牙5.0示例程序";
        InitBlueManager();
        ScanBtn.onClick.AddListener(StartScan);
        StopBtn.onClick.AddListener(StopScan);
        ConnectBtn.onClick.AddListener(Connect);
        RRateBtn_1.onClick.AddListener(SetRRate10);
        RRateBtn_2.onClick.AddListener(SetRRate200);
        AccBtn.onClick.AddListener(AccCeil);
    }

    /// <summary>
    /// 初始化蓝牙管理器
    /// Initialize Bluetooth Manager
    /// </summary>
    private void InitBlueManager()
    {
        // 获得蓝牙管理器 Get Bluetooth Manager
        bluetoothManager = BluetoothManager.Instance;
        // 绑定搜索设备事件 Bind search device events
        bluetoothManager.OnDeviceFound -= WitBluetoothManager_OnDeviceFound;
        bluetoothManager.OnDeviceFound += WitBluetoothManager_OnDeviceFound;
    }

    /// <summary>
    /// 找到设备时会回调这个方法
    /// This method will be called back when the device is found
    /// </summary>
    /// <param name="mac"></param>
    /// <param name="deviceName"></param>
    private void WitBluetoothManager_OnDeviceFound(string mac, string deviceName)
    {
        // 名称过滤
        // Name filtering
        if (deviceName != null && deviceName.Contains("WT"))
        {
            if (!FoundDeviceDict.ContainsKey(mac))
            {
                BWT901BLE bWT901BLE = new BWT901BLE();
                bWT901BLE.SetDeviceAddress(mac);
                bWT901BLE.SetDeviceName(deviceName);
                FoundDeviceDict.Add(mac, bWT901BLE);
                MsgText.text = $"找到设备{deviceName}({mac})";
                DeviceName.text = $"{deviceName}({mac})";
                currentMac = mac;
            }
        }
    }

    /// <summary>
    /// 连接设备
    /// Connected Device
    /// </summary>
    private void Connect()
    {
        if (FoundDeviceDict.Count == 0) {
            MsgText.text = "请先搜索设备！";
            return;
        }
        BWT901BLE bWT901BLE = FoundDeviceDict[currentMac];
        if (ConnectText.text.Contains("Connect")) {
            // 打开这个设备
            // Open this device
            MsgText.text = "正在连接设备";
            bWT901BLE.Open();
            bWT901BLE.OnUpdate += BWT901BLE_OnUpdate;
            Info.text = bWT901BLE.GetDeviceName();
            MsgText.text = "连接设备成功";
            ConnectText.text = "Disconnect";
            UpdateBox?.Invoke(bWT901BLE);
        }
        else if (ConnectText.text.Contains("Disconnect")) {
            bWT901BLE.OnUpdate -= BWT901BLE_OnUpdate;
            bWT901BLE.Close();
            MsgText.text = "关闭设备成功";
            ConnectText.text = "Connect";
        }
    }

    /// <summary>
    /// 传感器数据更新时会回调这个方法 This method will be called back when sensor data is updated
    /// </summary>
    /// <param name="BWT901BLE"></param>
    private void BWT901BLE_OnUpdate(BWT901BLE BWT901BLE)
    {
        DisplayData(BWT901BLE);
    }

    /// <summary>
    /// 数据展示
    /// Data display
    /// </summary>
    /// <param name="bWT901BLE"></param>
    private void DisplayData(BWT901BLE bWT901BLE)
    {
        AccX.text = "AccX:" + bWT901BLE.GetDeviceData(WitSensorKey.AccX);
        AccY.text = "AccY:" + bWT901BLE.GetDeviceData(WitSensorKey.AccY);
        AccZ.text = "AccZ:" + bWT901BLE.GetDeviceData(WitSensorKey.AccZ);
        AsX.text = "AsX:" + bWT901BLE.GetDeviceData(WitSensorKey.AsX);
        AsY.text = "AsY:" + bWT901BLE.GetDeviceData(WitSensorKey.AsY);
        AsZ.text = "AsZ:" + bWT901BLE.GetDeviceData(WitSensorKey.AsZ);
        AngX.text = "AngX:" + bWT901BLE.GetDeviceData(WitSensorKey.AngleX);
        AngY.text = "AngY:" + bWT901BLE.GetDeviceData(WitSensorKey.AngleY);
        AngZ.text = "AngZ:" + bWT901BLE.GetDeviceData(WitSensorKey.AngleZ);
    }

    /// <summary>
    /// 停止扫描
    /// Stop Scan
    /// </summary>
    private void StopScan()
    {
        MsgText.text = "停止扫描";
        bluetoothManager.stopScan();
    }

    /// <summary>
    /// 开始扫描
    /// Start Scan
    /// </summary>
    private void StartScan()
    {
        MsgText.text = "开始扫描";
        FoundDeviceDict.Clear();
        bluetoothManager.startScan();
    }

    /// <summary>
    /// 加速度校准
    /// Acceleration calibration
    /// </summary>
    private void AccCeil()
    {
        MsgText.text = "正在进行加计校准，请等待5秒";
        BWT901BLE bWT901BLE = FoundDeviceDict[currentMac];
        bWT901BLE.AppliedCalibration();
        MsgText.text = "加计校准完成";
    }

    /// <summary>
    /// 200hz回传
    /// 200Hz return
    /// </summary>
    private void SetRRate200()
    {
        BWT901BLE bWT901BLE = FoundDeviceDict[currentMac];
        bWT901BLE.SetReturnRate(0x0b);
        MsgText.text = "设置200hz完成";
    }

    /// <summary>
    /// 10hz回传
    /// 10Hz return
    /// </summary>
    private void SetRRate10()
    {
        BWT901BLE bWT901BLE = FoundDeviceDict[currentMac];
        bWT901BLE.SetReturnRate(0x06);
        MsgText.text = "设置10hz完成";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
