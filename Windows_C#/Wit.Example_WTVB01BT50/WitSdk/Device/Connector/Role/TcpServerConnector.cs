using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;
using Wit.SDK.Sensor.Connector.Modular.Tcp;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Role
{
    /// <summary>
    /// TCP服务器
    /// </summary>
    public class TcpServerConnector : IConnector
    {
        /// <summary>
        /// tcp服务器
        /// </summary>
        private TcpServerProxy TcpServer { get; set; }

        /// <summary>
        /// 连接配置
        /// </summary>
        public TcpServerConfig TcpServerConfig = new TcpServerConfig();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tcpServerConfig"></param>
        public TcpServerConnector(TcpServerConfig tcpServerConfig) {
            TcpServerConfig = tcpServerConfig;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            if (TcpServerConfig == null)
            {
                throw new Exception("未设置任何tcp服务端连接参数");
            }
            if (TcpServerConfig.IPEndPoint == null)
            {
                throw new Exception("未设置监听地址和端口");
            }
            if (TcpServerConfig.RemoteEP == null)
            {
                throw new Exception("未设置远程端口");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            if (TcpServer != null)
            {
                TcpServer.Close();
            }
            // 标志为关闭状态
            ConnectStatus = ConnectStatus.Closed;
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return TcpServerConfig;
        }

        /// <summary>
        /// 打开
        /// </summary>
        public override void Open()
        {
            // 创建包含ip和端口号的网络节点对象；
            IPEndPoint endPoint = TcpServerConfig.IPEndPoint;

            if (TcpServer == null)
            {
                TcpServer = new TcpServerProxy(endPoint);
            }
            TcpServer.Open();
            TcpServer.OnReceive -= AcceptData;
            TcpServer.OnReceive += AcceptData;
            ConnectStatus = ConnectStatus.Opened;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data"></param>
        private void AcceptData(EndPoint remoteEP, byte[] data)
        {
            if (remoteEP.ToString() == TcpServerConfig.RemoteEP.ToString()) { 
                onReceive(data);
            }
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
            }
            OnSendData(data);
            TcpServer.Send(TcpServerConfig.RemoteEP, data);
        }
    }
}
