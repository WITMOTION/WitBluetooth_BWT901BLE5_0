using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Role
{
    /// <summary>
    /// 串口连接器
    /// </summary>
    public class SPConnector :IConnector
    {
        /// <summary>
        /// 串口
        /// </summary>
        public SerialPort SerialPort { get; set; }

        /// <summary>
        /// 连接配置
        /// </summary>
        public SerialPortConfig SerialPortConfig = new SerialPortConfig();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="serialPortConfig"></param>
        public SPConnector(SerialPortConfig serialPortConfig) {
            SerialPortConfig = serialPortConfig;
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            if (SerialPort != null)
            {
                SerialPort.Close();
            }

            // 标志为关闭状态
            ConnectStatus = ConnectStatus.Closed;
        }

        /// <summary>
        /// 打开
        /// </summary>
        public override void Open()
        {
            CheckConfig();
            //检查状态
            if (ConnectStatus != ConnectStatus.Closed)
            {
                throw new Exception("打开连接失败,现在已经打开");
            }

            if (SerialPort == null) { 
                SerialPort = new SerialPort();
            }
            // 串口号
            SerialPort.BaudRate = SerialPortConfig.BaudRate;
            // 波特率
            SerialPort.PortName = SerialPortConfig.PortName;
            // 数据位
            SerialPort.DataBits = 8;
            // 停止位
            SerialPort.StopBits = StopBits.One;
            // 校验位
            SerialPort.Parity = Parity.None;
            // 读取缓存
            SerialPort.ReadBufferSize = 10240 * 100;
            // 写出缓存
            SerialPort.WriteBufferSize = 10240 * 100;
            // DTS
            SerialPort.RtsEnable = SerialPortConfig.RtsEnable;
            // DTR
            SerialPort.DtrEnable = SerialPortConfig.DTREnable;

            //打开串口
            if (!SerialPort.IsOpen) { 
                SerialPort.Open();
            }

            //取消注册
            SerialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialDataReceive);
            //注册
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceive);

            ConnectStatus = ConnectStatus.Opened;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            // 检查配置
            if (SerialPortConfig == null)
            {
                throw new Exception("未设置任何串口连接参数");
            }

            if (SerialPortConfig.BaudRate < 0)
            {

                throw new Exception("未设置连接参数,波特率不能为负数");
            }

            if (SerialPortConfig.PortName == null || SerialPortConfig.PortName == string.Empty)
            {
                throw new Exception("未设置连接参数,串口名称不能为空");
            }
        }


        /// <summary>
        /// 收到串口数据时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialDataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort _SerialPort = (SerialPort)sender;
            try
            {
                int _bytesToRead = _SerialPort.BytesToRead;
                byte[] recvData = new byte[_bytesToRead];
                _SerialPort.Read(recvData, 0, _bytesToRead);
                //Debug.WriteLine(ByteArrayConvert.ByteArrayToHexString(recvData));
                onReceive(recvData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public override void SendData(byte[] data)
        {
            // 检查状态
            if (ConnectStatus != ConnectStatus.Opened)
            {
                throw new Exception("发送失败，未打开连接");
            }

            if (SerialPort.IsOpen)
            {
                OnSendData(data);
                SerialPort.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        /// <param name="baudRate"></param>
        public void SetBaud(int baudRate)
        {
            SerialPortConfig.BaudRate = baudRate;
            if (SerialPort != null) { 
            SerialPort.BaudRate = baudRate;
            }
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return SerialPortConfig;
        }
    }
}
