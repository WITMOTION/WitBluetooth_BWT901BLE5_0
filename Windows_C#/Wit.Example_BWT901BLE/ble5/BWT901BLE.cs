using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Roles;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Roles;
using Wit.SDK.Modular.Sensor.Utils;
using Wit.SDK.Modular.WitSensorApi.Interface;
using Wit.SDK.Sensor.Connector.Entity;
using Wit.SDK.Sensor.Connector.Role;

namespace Wit.SDK.Modular.WitSensorApi.Modular.BWT901BLE
{
    /// <summary>
    /// BWT901BLE连接类
    /// </summary>
    public class Bwt901ble : IAttitudeSensorApi
    {
        /// <summary>
        /// 设备模型
        /// </summary>
        private DeviceModel DeviceModel;

        /// <summary>
        /// 连接器
        /// </summary>
        private WinBleConnector connector = new WinBleConnector(new WinBleConfig());

        /// <summary>
        /// 记录数据委托
        /// </summary>
        public delegate void OnRecordHandler(Bwt901ble BWT901BLE);

        /// <summary>
        /// 记录数据事件
        /// </summary>
        public event OnRecordHandler OnRecord;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudrate"></param>
        public Bwt901ble(string mac,string deviceName)
        {
            // 创建一个连接蓝牙的设备模型
            DeviceModel deviceModel = new DeviceModel(mac,deviceName+$"({mac})",
                    new Bwt901bleResolver(),
                    new Bwt901bleProcessor(),
                    "61_0");
            SetMacAddr(mac);
            deviceModel.Connector = connector;
            DeviceModel = deviceModel;
        }

        /// <summary>
        /// 设置蓝牙地址
        /// </summary>
        /// <param name="macAddr"></param>
        private void SetMacAddr(string macAddr)
        {
            connector.Config.Mac = macAddr;
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        public void Open()
        {
            DeviceModel.OpenDevice();
            DeviceModel.OnListenKeyUpdate += DeviceModel_OnListenKeyUpdate;
        }

        /// <summary>
        /// 是否打开的
        /// </summary>
        public bool IsOpen()
        {
            return DeviceModel.IsOpen;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public void Close()
        {
            DeviceModel.CloseDevice();
            DeviceModel.OnListenKeyUpdate -= DeviceModel_OnListenKeyUpdate;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">需要发送出去的数据</param>
        /// <param name="returnData">传感器返回的数据</param>
        /// <param name="isWaitReturn">是否需要传感器返回数据</param>
        /// <param name="waitTime">等待传感器返回数据时间，单位ms，默认100ms</param>
        /// <param name="repetition">重复发送次数</param>
        public void SendData(byte[] data, out byte[] returnData, bool isWaitReturn = false, int waitTime = 100, int repetition = 1)
        {
            DeviceModel.SendData(data, out returnData, isWaitReturn, waitTime, repetition);
        }

        /// <summary>
        /// 发送带协议的数据，使用默认等待时长
        /// </summary>
        /// <param name="data">数据</param>
        public void SendProtocolData(byte[] data)
        {
            DeviceModel.ReadData(data);
        }

        /// <summary>
        /// 发送带协议的数据,并且指定等待时长
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="waitTime">等待时间</param>
        public void SendProtocolData(byte[] data, int waitTime)
        {
            DeviceModel.ReadData(data, waitTime);
        }

        /// <summary>
        /// 发送读取寄存器的命令
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="waitTime"></param>
        public void SendReadReg(byte reg, int waitTime)
        {
            DeviceModel.ReadData(WitProtocolUtils.GetRead(reg), waitTime);
        }

        /// <summary>
        ///  解锁寄存器
        /// </summary>
        public void UnlockReg()
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xB5, });
        }

        /// <summary>
        /// 保存寄存器
        /// </summary>
        public void SaveReg()
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x00, 0x00, 0x00, });
        }

        /// <summary>
        /// 加计校准
        /// </summary>
        public void AppliedCalibration()
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x01, 0x00, });
        }

        /// <summary>
        /// 开始磁场校准
        /// </summary>
        public void StartFieldCalibration()
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x07, 0x00, });
        }

        /// <summary>
        /// 结束磁场校准
        /// </summary>
        public void EndFieldCalibration()
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x00, 0x00, });
        }

        /// <summary>
        /// 设置回传速率
        /// </summary>
        /// <param name="rate"></param>
        public void SetReturnRate(byte rate)
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x03, rate, 0x00, });
        }

        /// <summary>
        /// 设置带宽
        /// </summary>
        /// <param name="rate"></param>
        public void SetBandWidth(byte band)
        {
            SendProtocolData(new byte[] { 0xff, 0xaa, 0x1F, band, 0x00, });
        }

        /// <summary>
        /// 获得设备名称
        /// </summary>
        /// <returns></returns>
        public string GetDeviceName()
        {
            return DeviceModel.DeviceName;
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        public string GetDeviceData(string key)
        {
            return DeviceModel.GetDeviceData(key);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        public short? GetDeviceData(ShortKey key)
        {
            return DeviceModel.GetDeviceData(key);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        public string GetDeviceData(StringKey key)
        {
            return DeviceModel.GetDeviceData(key);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        public double? GetDeviceData(DoubleKey key)
        {
            return DeviceModel.GetDeviceData(key);
        }

        /// <summary>
        /// 传感器数据更新时
        /// </summary>
        /// <param name="deviceModel"></param>
        public void DeviceModel_OnListenKeyUpdate(DeviceModel deviceModel)
        {
            OnRecord?.Invoke(this);
        }

    }
}
