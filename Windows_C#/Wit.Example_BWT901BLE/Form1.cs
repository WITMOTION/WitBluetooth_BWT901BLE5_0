using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;
using Wit.SDK.Modular.WitSensorApi.Modular.BWT901BLE;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.Bluetooth.WinBlue.Utils;
using Wit.Bluetooth.WinBlue.Interface;

namespace Wit.Example_BWT901BLE
{   
    /// <summary>
    /// 程序主窗口
    /// 说明：
    /// 1.本程序是维特智能开发的BWT901BLE九轴传感器示例程序
    /// 2.适用示例程序前请咨询技术支持,询问本示例程序是否支持您的传感器
    /// 3.使用前请了解传感器的通信协议
    /// 4.本程序只有一个窗口,所有逻辑都在这里
    /// 
    /// Program Main Window
    /// Explanation:
    /// 1. This program is an example program for the BWT901BLE nine axis sensor developed by Weite Intelligence
    /// 2. Before applying the sample program, please consult technical support and ask if this sample program supports your sensor
    /// 3. Please understand the communication protocol of the sensor before use
    /// 4. This program only has one window, all logic is here
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// 蓝牙管理器
        /// Bluetooth manager
        /// </summary>
        private IWinBlueManager WitBluetoothManager = WinBlueFactory.GetInstance();

        /// <summary>
        /// 找到的设备
        /// Found device
        /// </summary>
        private Dictionary<string, Bwt901ble> FoundDeviceDict = new Dictionary<string, Bwt901ble>();

        /// <summary>
        /// 控制自动刷新数据线程是否工作
        /// Control whether the automatic refresh data thread works
        /// </summary>
        public bool EnableRefreshDataTh { get; private set; }

        /// <summary>
        /// 构造
        /// Structure
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载时
        /// When the form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 开启数据刷新线程
            // Enable data refresh thread
            Thread thread = new Thread(RefreshDataTh);
            thread.IsBackground = true;
            EnableRefreshDataTh = true;
            thread.Start();
        }

        /// <summary>
        /// 窗体关闭时
        /// When the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭刷新数据线程
            // Close refresh data thread
            EnableRefreshDataTh = false;
            // 关闭蓝牙搜索
            // Turn off Bluetooth search
            stopScanButton_Click(null, null);
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 开始搜索
        /// Starting the Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startScanButton_Click(object sender, EventArgs e)
        {
            // 清除找到的设备
            // Clear found devices
            FoundDeviceDict.Clear();

            // 关闭之前打开的设备
            // Close previously opened devices
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;
                bWT901BLE.Close();
            }

            WitBluetoothManager.OnDeviceFound += this.WitBluetoothManager_OnDeviceFound;
            WitBluetoothManager.StartScan();
        }

        /// <summary>
        /// 当搜索到蓝牙设备时会回调这个方法
        /// Call back this method when Bluetooth devices are found
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
                    Bwt901ble bWT901BLE = new Bwt901ble(mac,deviceName);
                    FoundDeviceDict.Add(mac, bWT901BLE);
                    // 打开这个设备
                    // Open this device
                    bWT901BLE.Open();
                    bWT901BLE.OnRecord += BWT901BLE_OnRecord;
                }
            }
        }

        /// <summary>
        /// 当传感器数据刷新时会调用这里，您可以在这里记录数据
        /// This will be called when the sensor data is refreshed, where you can record the data
        /// </summary>
        /// <param name="BWT901BLE"></param>
        private void BWT901BLE_OnRecord(Bwt901ble BWT901BLE)
        {
            string text = GetDeviceData(BWT901BLE);
            Debug.WriteLine(text);
        }

        /// <summary>
        /// 停止搜索
        /// stop searching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopScanButton_Click(object sender, EventArgs e)
        {
            // 让蓝牙管理器停止搜索
            // Stop Bluetooth Manager Search
            WitBluetoothManager.StopScan();
        }

        /// <summary>
        /// 设备状态发生时会调这个方法
        /// This method will be called when the device status occurs
        /// </summary>
        /// <param name="macAddr"></param>
        /// <param name="mType"></param>
        /// <param name="sMsg"></param>
        private void OnDeviceStatu(string macAddr, int mType, string sMsg)
        {
            if (mType == 20)
            {
                // 断开连接
                // Disconnect
                Debug.WriteLine(macAddr + "Disconnect");
            }

            if (mType == 11)
            {
                // 连接失败
                // Connect failed
                Debug.WriteLine(macAddr + "Connect failed");
            }

            if (mType == 10)
            {
                // 连接成功
                // Successfully connected
                Debug.WriteLine(macAddr + "Successfully connected");
            }
        }

        /// <summary>
        /// 刷新数据线程
        /// Refresh Data Thread
        /// </summary>
        private void RefreshDataTh()
        {
            while (EnableRefreshDataTh)
            {
                // 多设备的展示数据
                // Display data for multiple devices
                string DeviceData = "";
                Thread.Sleep(100);
                // 刷新所有连接设备的数据
                // Refresh data for all connected devices
                for (int i = 0; i < FoundDeviceDict.Count; i++)
                {
                    var keyValue = FoundDeviceDict.ElementAt(i);
                    Bwt901ble bWT901BLE = keyValue.Value;
                    if (bWT901BLE.IsOpen())
                    {
                        DeviceData += GetDeviceData(bWT901BLE) + "\r\n";
                    }
                }
                dataRichTextBox.Invoke(new Action(() =>
                {
                    dataRichTextBox.Text = DeviceData;
                }));
            }
        }

        /// <summary>
        /// 获得设备的数据
        /// Obtaining device data
        /// </summary>
        private string GetDeviceData(Bwt901ble BWT901BLE)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(BWT901BLE.GetDeviceName()).Append("\n");
            // 加速度
            // Acc
            builder.Append("AccX").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AccX)).Append("g \t");
            builder.Append("AccY").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AccY)).Append("g \t");
            builder.Append("AccZ").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AccZ)).Append("g \n");
            // 角速度
            // Gyro
            builder.Append("GyroX").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AsX)).Append("°/s \t");
            builder.Append("GyroY").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AsY)).Append("°/s \t");
            builder.Append("GyroZ").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AsZ)).Append("°/s \n");
            // 角度
            // Angle
            builder.Append("AngleX").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AngleX)).Append("° \t");
            builder.Append("AngleY").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AngleY)).Append("° \t");
            builder.Append("AngleZ").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.AngleZ)).Append("° \n");
            // 磁场
            // Mag
            builder.Append("MagX").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.HX)).Append("uT \t");
            builder.Append("MagY").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.HY)).Append("uT \t");
            builder.Append("MagZ").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.HZ)).Append("uT \n");
            // 版本号
            // VersionNumber
            builder.Append("VersionNumber").Append(":").Append(BWT901BLE.GetDeviceData(WitSensorKey.VersionNumber)).Append("\n");
            return builder.ToString();
        }

        /// <summary>
        /// 加计校准
        /// Acceleration calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appliedCalibrationButton_Click(object sender, EventArgs e)
        {
            // 所有连接的蓝牙设备都加计校准
            // All connected Bluetooth devices are calibrated
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }

                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.AppliedCalibration();

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x01, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// 读取03寄存器
        /// Read 03 register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readReg03Button_Click(object sender, EventArgs e)
        {
            string reg03Value = "";
            // 读取所有连接的蓝牙设备的03寄存器
            // Read the 03 register of all connected Bluetooth devices
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 等待时长
                    // Waiting time
                    int waitTime = 3000;
                    // 发送读取命令，并且等待传感器返回数据，如果没读上来可以将 waitTime 延长，或者多读几次
                    // Send a read command and wait for the sensor to return data. If it is not read, the waitTime can be extended or read several more times
                    bWT901BLE.SendReadReg(0x03, waitTime);

                    // 下面这行和上面等价推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x27, 0x03, 0x00 }, waitTime);

                    // 拿到所有连接的蓝牙设备的值
                    // Get the values of all connected Bluetooth devices
                    reg03Value += bWT901BLE.GetDeviceName() + "的寄存器03值为 :" + bWT901BLE.GetDeviceData(new ShortKey("03")) + "\r\n";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            MessageBox.Show(reg03Value);
        }

        /// <summary>
        /// 设置回传速率10Hz
        /// Set the return rate to 10Hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returnRate10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.SetReturnRate(0x06);

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x03, 0x06, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 设置回传速率50Hz
        /// Set the return rate to 50Hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void returnRate50_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.SetReturnRate(0x08);

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x03, 0x08, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 设置带宽20Hz
        /// Set bandwidth of 20Hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandWidth20_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.SetBandWidth(0x04);

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x1F, 0x04, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 设置带宽256Hz
        /// Set bandwidth of 256Hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandWidth256_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.SetBandWidth(0x00);

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x1F, 0x00, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 开始磁场校准
        /// Start magnetic field calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startFieldCalibrationButton_Click(object sender, EventArgs e)
        {
            // 开始所有连接的蓝牙设备的磁场校准
            // Start magnetic field calibration for all connected Bluetooth devices
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.StartFieldCalibration();

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x07, 0x00 });
                    MessageBox.Show("开始磁场校准,请绕传感器XYZ三轴各转一圈,转完以后点击【结束磁场校准】");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 结束磁场校准
        /// End magnetic field calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endFieldCalibrationButton_Click(object sender, EventArgs e)
        {

            // 结束所有连接的蓝牙设备的磁场校准
            // End the magnetic field calibration of all connected Bluetooth devices
            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                Bwt901ble bWT901BLE = keyValue.Value;

                if (bWT901BLE.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    bWT901BLE.UnlockReg();
                    bWT901BLE.EndFieldCalibration();

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //bWT901BLE.SendProtocolData(new byte[] { 0xff, 0xaa, 0x01, 0x00, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
