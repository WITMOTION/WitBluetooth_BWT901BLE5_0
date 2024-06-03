using Assets.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Device
{
    /*
     * 设备模型，每一个传感器对应一个设备模型对象，在这里处理传感器接收到的数据
     * DeviceModel, where each sensor corresponds to a device model object, where the data received by the sensor is processed
     */
    public class DeviceModel
    {
        public bool isOpen = false; 

        private BlueConnector connector;

        public string deviceId;

        public string deviceName;

        // 设备数据字典 Device Data Dictionary
        public Dictionary<string, double> deviceData = new Dictionary<string, double>();

        // 接受到的数据缓存 Received data cache
        List<byte> ActiveByteDataBuffer = new List<byte>();

        // 临时数组 Temporary array
        private byte[] ActiveByteTemp = new byte[100];

        // 实时数据更新事件 Real time data update events
        public delegate void UpdateEventHandler(DeviceModel deviceModel);
        public event UpdateEventHandler OnKeyUpdate;

        // 发送消息队列 Send message queue
        private Queue<BleApi.BLEData> cmdQueue = new Queue<BleApi.BLEData>();

        // 读取数据线程 Read data thread
        private Thread thread1;
        // 发送数据线程 Sending data thread
        private Thread thread2;

        // 上次读取时间 Last read time
        private DateTime lastReadTime1;
        private DateTime lastReadTime2;

        public DeviceModel(string deviceName, string deviceId) {
            this.deviceName = deviceName;   
            this.deviceId = deviceId;
            connector = BlueConnector.Instance;
            lastReadTime1 = DateTime.Now;
            lastReadTime2 = DateTime.Now;
        }

        /// <summary>
        /// 开启设备 Open the device
        /// </summary>
        public void OpenDevice() {
            if (isOpen) {
                return;
            }

            connector.Connect(deviceId);
            connector.OnReceive -= OnDataReceive;
            connector.OnReceive += OnDataReceive;
            isOpen = true;
            // 清除消息队列 Clear message queue
            cmdQueue.Clear();
            // 开启读取线程 Enable read thread
            thread1 = new Thread(ReadDataTh);
            thread1.IsBackground = true;
            thread1.Start();

            // 开启发送数据线程 Enable data sending thread
            thread2 = new Thread(SendDataTh);
            thread1.IsBackground = true;
            thread2.Start();
        }

        // 关闭设备 Close the device
        public void CloseDevice() {
            if (isOpen) {
                connector.OnReceive -= OnDataReceive;
                connector.Disconnect();
                isOpen = false;

                try
                {
                    thread1?.Abort();
                }
                catch (Exception)
                {
                    // 捕捉异常不抛出
                }
                try
                {
                    thread2?.Abort();
                }
                catch (Exception)
                {
                    // 捕捉异常不抛出
                }
            }
        }

        /// <summary>
        /// 读取数据线程 Read data thread
        /// </summary>
        private void ReadDataTh() {
            Thread.Sleep(5000);
            // 读取电量 Reading battery level
            SendData(new byte[] { 0xff, 0xaa, 0x27, 0x64, 0x00 });
            while (isOpen) {
                DateTime readTime = DateTime.Now;
                if ((readTime - lastReadTime1).TotalMilliseconds > 1000) {
                    // 读取磁场 间隔1秒 Read the magnetic field with a 1-second interval
                    SendData(new byte[] { 0xff, 0xaa, 0x27, 0x3A, 0x00 });
                    lastReadTime1 = readTime;
                }

                if ((readTime - lastReadTime2).TotalMilliseconds > 30000) {
                    // 读取电量 间隔30秒 Read the battery level with a 30 second interval
                    SendData(new byte[] { 0xff, 0xaa, 0x27, 0x64, 0x00 });
                    lastReadTime2 = readTime;   
                }
                // 防止读取过快线程卡死
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 发送数据线程 Send data thread
        /// </summary>
        private void SendDataTh() {
            while(isOpen) {
                if (cmdQueue.Count > 0) {
                    BleApi.BLEData data = cmdQueue.Dequeue();
                    BleApi.SendData(in data, false);
                    Thread.Sleep(100); 
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 收到数据时 When receiving data
        /// </summary>
        private void OnDataReceive(string deviceId, byte[] data) {
            if (deviceId != this.deviceId) {
                return;
            }

            ActiveByteDataBuffer.AddRange(data);
            while (ActiveByteDataBuffer.Count > 5 && ActiveByteDataBuffer[0] != 0x55 && (ActiveByteDataBuffer[1] != 0x61 || ActiveByteDataBuffer[1] != 0x71))
            {
                ActiveByteDataBuffer.RemoveAt(0);
            }

            while (ActiveByteDataBuffer.Count >= 20)
            {
                ActiveByteTemp = ActiveByteDataBuffer.GetRange(0, 20).ToArray();

                // 主动回传数据处理 Active data feedback processing
                if (ActiveByteTemp[0] == 0x55 && ActiveByteTemp[1] == 0x61)
                {
                    short[] Pack = new short[9];
                    Pack[0] = BitConverter.ToInt16(ActiveByteTemp, 2);
                    Pack[1] = BitConverter.ToInt16(ActiveByteTemp, 4);
                    Pack[2] = BitConverter.ToInt16(ActiveByteTemp, 6);
                    Pack[3] = BitConverter.ToInt16(ActiveByteTemp, 8);
                    Pack[4] = BitConverter.ToInt16(ActiveByteTemp, 10);
                    Pack[5] = BitConverter.ToInt16(ActiveByteTemp, 12);
                    Pack[6] = BitConverter.ToInt16(ActiveByteTemp, 14);
                    Pack[7] = BitConverter.ToInt16(ActiveByteTemp, 16);
                    Pack[8] = BitConverter.ToInt16(ActiveByteTemp, 18);

                    double AccX = Math.Round((double)Pack[0] / 32768 * 16, 3);
                    double AccY = Math.Round((double)Pack[1] / 32768 * 16, 3);
                    double AccZ = Math.Round((double)Pack[2] / 32768 * 16, 3);
                    double AsX = Math.Round((double)Pack[3] / 32768 * 2000, 3);
                    double AsY = Math.Round((double)Pack[4] / 32768 * 2000, 3);
                    double AsZ = Math.Round((double)Pack[5] / 32768 * 2000, 3);
                    double AngX = Math.Round((double)Pack[6] / 32768 * 180, 2);
                    double AngY = Math.Round((double)Pack[7] / 32768 * 180, 2);
                    double AngZ = Math.Round((double)Pack[8] / 32768 * 180, 2);

                    SetDeviceData("AccX", AccX);
                    SetDeviceData("AccY", AccY);
                    SetDeviceData("AccZ", AccZ);
                    SetDeviceData("AsX", AsX);
                    SetDeviceData("AsY", AsY);
                    SetDeviceData("AsZ", AsZ);
                    SetDeviceData("AngX", AngX);
                    SetDeviceData("AngY", AngY);
                    SetDeviceData("AngZ", AngZ);
                    OnKeyUpdate?.Invoke(this);

                    ActiveByteDataBuffer.RemoveRange(0, 20);
                }
                // 读取回传数据处理 Read and return data processing
                else if (ActiveByteTemp[0] == 0x55 && ActiveByteTemp[1] == 0x71) {
                    // short[] Pack = new short[4];
                    byte reg = ActiveByteTemp[2];
                    short[] Pack = new short[4];
                    switch (reg)
                    {
                        case 0x3A:
                            Pack[0] = BitConverter.ToInt16(ActiveByteTemp, 4);
                            Pack[1] = BitConverter.ToInt16(ActiveByteTemp, 6);
                            Pack[2] = BitConverter.ToInt16(ActiveByteTemp, 8);
                            double HX = Math.Round((double)Pack[0] / 120, 3);
                            double HY = Math.Round((double)Pack[1] / 120, 3);
                            double HZ = Math.Round((double)Pack[2] / 120, 3);
                            SetDeviceData("HX", HX);
                            SetDeviceData("HY", HY);
                            SetDeviceData("HZ", HZ);
                            break;
                        case 0x64:
                            short voltage = BitConverter.ToInt16(ActiveByteTemp, 4);
                            // 获得电量百分比 Obtaining battery percentage
                            GetBatteryPercentage(voltage);                       
                            break;
                        default:
                            Pack[0] = BitConverter.ToInt16(ActiveByteTemp, 4);
                            Pack[1] = BitConverter.ToInt16(ActiveByteTemp, 6);
                            Pack[2] = BitConverter.ToInt16(ActiveByteTemp, 8);
                            Pack[3] = BitConverter.ToInt16(ActiveByteTemp, 10);
                            SetDeviceData(reg.ToString("X"), Pack[0]);
                            SetDeviceData((reg + 1).ToString("X"), Pack[1]);
                            SetDeviceData((reg + 2).ToString("X"), Pack[2]);
                            SetDeviceData((reg + 3).ToString("X"), Pack[3]);
                            break;
                    }

                    ActiveByteDataBuffer.RemoveRange(0, 20);
                }
                else
                {
                    // 不是就移除一个 Remove first
                    ActiveByteDataBuffer.RemoveAt(0);
                }

            }
        }

        /// <summary>
        /// 获取电量百分比 Obtaining battery percentage
        /// </summary>
        private void GetBatteryPercentage(short voltage)
        {
            // 计算电量百分比
            if (voltage >= 396)
            {
                SetDeviceData("Battery", 100);
            }
            else if (voltage >= 393 && voltage < 396)
            {
                SetDeviceData("Battery", 90);
            }
            else if (voltage >= 387 && voltage < 393)
            {
                SetDeviceData("Battery", 75);
            }
            else if (voltage >= 382 && voltage < 387)
            {
                SetDeviceData("Battery", 60);
            }
            else if (voltage >= 379 && voltage < 382)
            {
                SetDeviceData("Battery", 50);
            }
            else if (voltage >= 377 && voltage < 379)
            {
                SetDeviceData("Battery", 40);
            }
            else if (voltage >= 373 && voltage < 377)
            {
                SetDeviceData("Battery", 30);
            }
            else if (voltage >= 370 && voltage < 373)
            {
                SetDeviceData("Battery", 20);
            }
            else if (voltage >= 368 && voltage < 370)
            {
                SetDeviceData("Battery", 15);
            }
            else if (voltage >= 350 && voltage < 368)
            {
                SetDeviceData("Battery", 10);
            }
            else if (voltage >= 340 && voltage < 350)
            {
                SetDeviceData("Battery", 5);
            }
            else if (voltage < 340)
            {
                SetDeviceData("Battery", 0);
            }
        }

        /// <summary>
        /// 设置传感器数据 Set sensor data
        /// </summary>
        public void SetDeviceData(string key, double value) {
            deviceData[key] = value;    
        }

        /// <summary>
        /// 获得传感器数据 Get sensor data
        /// </summary>
        public double GetDeviceData(string key)
        {
            if (deviceData.ContainsKey(key)) {
                return deviceData[key]; 
            }
            return 0.0;
        }

        /// <summary> 
        /// 获取传感器演示数据 Obtain sensor demonstration data
        /// </summary>
        public string GetDataDisplay()
        {
            string Acc = $"AccX:{GetDeviceData("AccX")}g\t\tAccY:{GetDeviceData("AccY")}g\t\tAccZ:{GetDeviceData("AccZ")}g\r\n";
            string As = $"AsX:{GetDeviceData("AsX")}°/s\t\tAsY:{GetDeviceData("AsY")}°/s\t\tAsZ:{GetDeviceData("AsZ")}°/s\r\n";
            string Angle = $"AngleX:{GetDeviceData("AngX")} °\t\tAngleY: {GetDeviceData("AngY")} °\t\tAngleZ: {GetDeviceData("AngZ")}°\r\n";
            string Mag = $"HX:{GetDeviceData("HX")}ut\t\tHY:{GetDeviceData("HY")}ut\t\tHZ:{GetDeviceData("HZ")}ut\r\n";
            string Electricity = $"Electricity:{GetDeviceData("Battery")}%";
            string data = Acc + As + Angle + Mag + Electricity;
            return data;
        }

        /// <summary>
        /// 发送数据 Send data
        /// </summary>
        public void SendData(byte[] cmd) {
            BleApi.BLEData data = new BleApi.BLEData();
            data.buf = cmd;
            data.size = (short)data.buf.Length;
            data.deviceId = this.deviceId;
            data.serviceUuid = BlueConnector.UUID_SERVICE;
            data.characteristicUuid = BlueConnector.UUID_WRITE;
            // 入队
            cmdQueue.Enqueue(data); 
        }
    }
}
