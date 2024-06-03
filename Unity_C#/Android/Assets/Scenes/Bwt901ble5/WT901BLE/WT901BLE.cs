using System.Threading;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.WitSensorApi.Interface;
using Wit.SDK.Sensor.Connector.Entity;
using Wit.SDK.Sensor.Connector.Role;

public class BWT901BLE : IAttitudeSensorApi
{
    /// <summary>
    /// 设备模型
    /// Device Model
    /// </summary>
    private DeviceModel DeviceModel;

    /// <summary>
    /// 连接器
    /// Device Connector
    /// </summary>
    private UnityBleConnect connector = new UnityBleConnect(new UnityBleConfig());

    /// <summary>
    /// 指定键值更新事件
    /// Specify key value update events
    /// </summary>
    /// <param name="BWT901BLE"></param>
    public delegate void OnUpdateHandler(BWT901BLE BWT901BLE);

    public event OnUpdateHandler OnUpdate;

    /// <summary>
    /// 设备
    /// Device
    /// </summary>
    public BWT901BLE()
    {
        DeviceModel = new DeviceModel("50", "", new WitBleResolver(), new WitBleProcessor(), "61_0")
        {
            Connector = connector
        };
    }

    /// <summary>
    /// 设置设备地址
    /// Set device address
    /// </summary>
    /// <param name="mac"></param>
    public void SetDeviceAddress(string mac) {
        connector.config.Mac = mac;
    }

    /// <summary>
    /// 设置设备名称
    /// Set device name
    /// </summary>
    /// <param name="name"></param>
    public void SetDeviceName(string name) {
        connector.config.DeviceName = name;
        DeviceModel.DeviceName = name;
    }

    /// <summary>
    /// 打开设备
    /// Open device
    /// </summary>
    public void Open()
    {
        DeviceModel.OpenDevice();
        DeviceModel.OnListenKeyUpdate += DeviceModel_OnListenKeyUpdate;
    }

    /// <summary>
    /// 关闭设备
    /// Close device
    /// </summary>
    public void Close()
    {
        DeviceModel.CloseDevice();
        DeviceModel.OnListenKeyUpdate -= DeviceModel_OnListenKeyUpdate;
    }

    /// <summary>
    /// 是否打开
    /// Is it open
    /// </summary>
    /// <returns></returns>
    public bool IsOpen()
    {
        return DeviceModel.IsOpen;
    }

    /// <summary>
    /// 传感器数据更新时会回调这里
    /// When sensor data is updated, it will be called back here
    /// </summary>
    /// <param name="deviceModel"></param>
    public void DeviceModel_OnListenKeyUpdate(DeviceModel deviceModel)
    {
        this.OnUpdate?.Invoke(this);
    }

    /// <summary>
    /// 获得设备数据
    /// Obtaining device data
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetDeviceData(string key)
    {
        return DeviceModel.GetDeviceData(key);
    }

    /// <summary>
    /// 获得设备名称
    /// Obtaining device name
    /// </summary>
    /// <returns></returns>
    public string GetDeviceName()
    {
        return DeviceModel.DeviceName;
    }


    /// <summary>
    /// 加速度校准
    /// Acceleration calibration
    /// </summary>
    public void AppliedCalibration()
    {
        UnlockReg();
        Thread.Sleep(100);
        DeviceModel.SendData(new byte[5] { 255, 170, 1, 1, 0 });
        Thread.Sleep(5000);
        SaveReg();
    }

    public void StartFieldCalibration()
    {
        
    }

    public void EndFieldCalibration()
    {
        
    }


    public void SendData(byte[] data, out byte[] returnData, bool isWaitReturn = false, int waitTime = 100, int repetition = 1)
    {
        returnData = new byte[] { };
    }

    /// <summary>
    /// 发送数据，不等待回复
    /// Send data without waiting for a reply
    /// </summary>
    /// <param name="data"></param>
    public void SendData(byte[] data) {
        DeviceModel.SendData(data);
    }

    /// <summary>
    /// 发送协议数据
    /// </summary>
    /// <param name="data"></param>
    public void SendProtocolData(byte[] data)
    {

    }

    /// <summary>
    /// 发送协议数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="waitTime"></param>
    public void SendProtocolData(byte[] data, int waitTime)
    {

    }

    /// <summary>
    /// 设置回传速率
    /// Set the return rate
    /// </summary>
    /// <param name="rate"></param>
    public void SetReturnRate(byte rate)
    {
        UnlockReg();
        Thread.Sleep(100);
        DeviceModel.SendData(new byte[5] { 255, 170, 3, rate, 0 });
        Thread.Sleep(100);
        SaveReg();
    }

    /// <summary>
    /// 保存
    /// Save
    /// </summary>
    public void SaveReg()
    {
        DeviceModel.SendData(new byte[5] { 255, 170, 0, 0, 0 });
    }

    /// <summary>
    /// 解锁
    /// Unlock
    /// </summary>
    public void UnlockReg()
    {
        DeviceModel.SendData(new byte[5] { 255, 170, 105, 136, 181 });
    }
}