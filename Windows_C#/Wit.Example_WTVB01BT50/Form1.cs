using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wit.Bluetooth.WinBlue.Interface;
using Wit.Bluetooth.WinBlue.Utils;
using Wit.Example_WTVB01BT50.vb01;
using Wit.Example_WTVB01BT50.vb01.Data;
using Wit.SDK.Device.Device.Device.DKey;

namespace Wit.Example_WTVB01BT50
{
    /// <summary>
    /// 程序主窗口
    /// 说明：
    /// 1.本程序是维特智能开发的WTVB01-BT50震动传感器示例程序
    /// 2.适用示例程序前请咨询技术支持,询问本示例程序是否支持您的传感器
    /// 3.使用前请了解传感器的通信协议
    /// 4.本程序只有一个窗口,所有逻辑都在这里
    /// 
    /// Program Main Window
    /// Explanation:
    /// 1. This program is an example program for the WTVB01-BT50 sensor developed by Weite Intelligence
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
        private Dictionary<string, WTVB01> FoundDeviceDict = new Dictionary<string, WTVB01>();

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
                WTVB01 vb01 = keyValue.Value;
                vb01.Close();
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
            if (deviceName != null && deviceName.Contains("WTVB"))
            {
                if (!FoundDeviceDict.ContainsKey(mac))
                {
                    WTVB01 vb01 = new WTVB01(mac, deviceName);
                    FoundDeviceDict.Add(mac, vb01);
                    // 打开这个设备
                    // Open this device
                    vb01.Open();
                    vb01.OnRecord += WTVB01_OnRecord;
                }
            }
        }

        /// <summary>
        /// 当传感器数据刷新时会调用这里，您可以在这里记录数据
        /// This will be called when the sensor data is refreshed, where you can record the data
        /// </summary>
        /// <param name="BWT901BLE"></param>
        private void WTVB01_OnRecord(WTVB01 vb01)
        {
            string text = GetDeviceData(vb01);
            // Debug.WriteLine(text);
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
                    WTVB01 vb01 = keyValue.Value;
                    if (vb01.IsOpen())
                    {
                        DeviceData += GetDeviceData(vb01) + "\r\n";
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
        private string GetDeviceData(WTVB01 vb01)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(vb01.GetDeviceName()).Append("\n");
            // 振动速度
            // Vibration velocity 
            builder.Append("VX").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.VX)).Append("mm/s \t");
            builder.Append("VY").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.VY)).Append("mm/s \t");
            builder.Append("VZ").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.VZ)).Append("mm/s \n");
            // 振动角度
            // Vibration angle
            builder.Append("AngX").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.ADX)).Append("° \t");
            builder.Append("AngY").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.ADY)).Append("° \t");
            builder.Append("AngZ").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.ADZ)).Append("° \n");
            // 振动位移
            // Vibration displacement
            builder.Append("DX").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.DX)).Append("mm \t");
            builder.Append("DY").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.DY)).Append("mm \t");
            builder.Append("DZ").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.DZ)).Append("mm \n");
            // 振动频率
            // Vibration frequency
            builder.Append("FreqX").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.HZX)).Append("hz \t");
            builder.Append("FreqY").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.HZY)).Append("hz \t");
            builder.Append("FreqZ").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.HZZ)).Append("hz \n");
            // 温度
            // Temperature
            builder.Append("Temp").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.TEMP)).Append(" \n");
            // 电量
            // Electricity level
            builder.Append("Electricity").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.PowerPercent)).Append("% \n");
            // 版本号
            // VersionNumber
            builder.Append("VersionNumber").Append(":").Append(vb01.GetDeviceData(WTVB01SensorKey.VersionNumber)).Append("\n");
            return builder.ToString();
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
                WTVB01 vb01 = keyValue.Value;

                if (vb01.IsOpen() == false)
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
                    vb01.SendReadReg(0x03, waitTime);

                    // 下面这行和上面等价推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x27, 0x03, 0x00 }, waitTime);

                    // 拿到所有连接的蓝牙设备的值
                    // Get the values of all connected Bluetooth devices
                    reg03Value += vb01.GetDeviceName() + "的寄存器03值为 :" + vb01.GetDeviceData(new ShortKey("03")) + "\r\n";
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
                WTVB01 vb01 = keyValue.Value;

                if (vb01.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    vb01.UnlockReg();
                    vb01.SetReturnRate(0x06);
                    vb01.SaveReg();

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x03, 0x06, 0x00 });
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
                WTVB01 vb01 = keyValue.Value;

                if (vb01.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    vb01.UnlockReg();
                    vb01.SetReturnRate(0x08);
                    vb01.SaveReg(); 

                    // 下面两行与上面等价,推荐使用上面的
                    // The following two lines are equivalent to the above, and it is recommended to use the above one
                    //vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x69, 0x88, 0xb5 });
                    //vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x03, 0x08, 0x00 });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 设置截至频率10.55hz
        /// Set cutoff frequency 10.55hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cufoff_Click(object sender, EventArgs e)
        {
            string value = "10.55";
            string[] data = value.Split('.');

            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                WTVB01 vb01 = keyValue.Value;

                if (vb01.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    vb01.UnlockReg();
                    // 整数部分
                    // Integer part
                    vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x5d, byte.Parse(data[0]), 0x00 });
                    if (data.Length >= 2) {
                        // 小数部分
                        // Decimal part
                        vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x5e, byte.Parse(data[1]), 0x00 });
                    }
                    vb01.SaveReg();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 设置检测周期100hz
        /// Set detection cycle of 100Hz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cycle_Click(object sender, EventArgs e)
        {
            string value = "100";

            for (int i = 0; i < FoundDeviceDict.Count; i++)
            {
                var keyValue = FoundDeviceDict.ElementAt(i);
                WTVB01 vb01 = keyValue.Value;

                if (vb01.IsOpen() == false)
                {
                    return;
                }
                try
                {
                    // 解锁寄存器并发送命令
                    // Unlock register and send command
                    vb01.UnlockReg();
                    vb01.SendProtocolData(new byte[] { 0xff, 0xaa, 0x5f, byte.Parse(value), 0x00 });
                    vb01.SaveReg();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
