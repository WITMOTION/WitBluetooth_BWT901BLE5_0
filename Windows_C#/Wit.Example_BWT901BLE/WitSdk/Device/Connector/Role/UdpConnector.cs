using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;
using Wit.SDK.Sensor.Connector.Modular.Udp;
using Wit.SDK.Udp;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Role
{
    /// <summary>
    /// UDP连接器
    /// </summary>
    public class UdpConnector : IConnector
    {
        /// <summary>
        /// UDP客户端
        /// </summary>
        private UdpServerProxy UdpServer { get; set; }

        /// <summary>
        /// 连接配置
        /// </summary>
        public UdpConfig UdpConfig = new UdpConfig();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="udpConfig"></param>
        public UdpConnector(UdpConfig udpConfig) {
            UdpConfig = udpConfig;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            if (UdpConfig == null)
            {
                throw new Exception("未设置任何udp连接参数");
            }

            if (UdpConfig.localIpPoint == null)
            {
                throw new Exception("未设置udp本地监听地址和端口");
            }

            if (UdpConfig.remoteIpPoint == null)
            {
                throw new Exception("未设置udp远程监听地址和端口");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            if (UdpServer != null)
            {
                UdpServer.Close();
            }

            //if (AutoTaskThread != null)
            //{
            //    AutoTaskRun = false;
            //    AutoTaskThread.Abort();
            //}

            // 标志为关闭状态
            ConnectStatus = ConnectStatus.Closed;
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return UdpConfig;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public override void Open()
        {
            CheckConfig();

            if (UdpServer == null)
            {
                UdpServer = new UdpServerProxy(UdpConfig.localIpPoint);
            }
            else
            {
                UdpServer.Close();
                UdpServer = new UdpServerProxy(UdpConfig.localIpPoint);
            }

            UdpServer.Open();
            UdpServer.OnReceive -= UdpServer_OnReceive;
            UdpServer.OnReceive += UdpServer_OnReceive;

            ConnectStatus = ConnectStatus.Opened;
        }

        /// <summary>
        /// 接收udp数据
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="data"></param>
        private void UdpServer_OnReceive(IPEndPoint remoteEP, byte[] data)
        {
            if (UdpConfig.remoteIpPoint.ToString().Equals(remoteEP.ToString())) { 
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
            UdpServer.Send(data, UdpConfig.remoteIpPoint);
        }
    }
}
