using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Role
{
    /// <summary>
    /// Tcp客户端
    /// </summary>
    public class TcpClientConnector : IConnector
    {
        /// <summary>
        /// TCP通讯客户端
        /// </summary>
        private Socket TcpClient { get; set; }

        /// <summary>
        /// 连接配置
        /// </summary>
        public TcpClientConfig TcpClientConfig = new TcpClientConfig();

        /// <summary>
        /// 自动任务线程
        /// </summary>
        private Thread AutoTaskThread = null;

        /// <summary>
        /// 是否运行任务线程
        /// </summary>
        private bool AutoTaskRun = false;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tcpClientConfig"></param>
        public TcpClientConnector(TcpClientConfig tcpClientConfig)
        {
            TcpClientConfig = tcpClientConfig;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            if (TcpClientConfig == null)
            {
                throw new Exception("未设置任何tcp客户端连接参数");
            }
            if (TcpClientConfig.Port < 0 || TcpClientConfig.Port > 65535)
            {
                throw new Exception("未设置连接参数,端口不能小于0不能大于65535");
            }
            if (TcpClientConfig.IP == null || TcpClientConfig.IP == string.Empty)
            {
                throw new Exception("未设置连接参数,IP不能为空");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            // 标志为关闭状态
            ConnectStatus = ConnectStatus.Closed;

            if (TcpClient != null)
            {
                TcpClient.Close();
            }

            if (AutoTaskThread != null)
            {
                AutoTaskRun = false;
                AutoTaskThread.Abort();
            }
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return TcpClientConfig;
        }


        /// <summary>
        /// 打开
        /// </summary>
        public override void Open()
        {
            if (TcpClient == null)
            {
                TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            TcpClient.Connect(TcpClientConfig.IP, TcpClientConfig.Port);


            // 读取数据线程
            AutoTaskThread = new Thread(AutoTaskMethod);
            AutoTaskRun = true;
            AutoTaskThread.IsBackground = true;
            AutoTaskThread.Start();

            ConnectStatus = ConnectStatus.Opened;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public override void SendData(byte[] data)
        {
            //检查状态
            if (ConnectStatus != ConnectStatus.Opened)
            {
                throw new Exception("发送失败，未打开连接");
                return;
            }

            if (TcpClient != null && TcpClient.Connected)
            {
                OnSendData(data);
                TcpClient.Send(data);
            }
        }


        /// <summary>
        /// 自动任务线程方法
        /// </summary>
        private void AutoTaskMethod()
        {
            while (AutoTaskRun)
            {
                // 休眠1毫秒防止cpu占用率过高
                Thread.Sleep(1);
                byte[] data = new byte[1024];

                if (TcpClient.Connected)
                {
                    int len = TcpClient.Receive(data);

                    if (len > 0)
                    {
                        onReceive(data.Take(len).ToArray());
                    }
                }
                else
                {
                    ConnectStatus = ConnectStatus.Closed;
                }

            }
        }
    }
}
