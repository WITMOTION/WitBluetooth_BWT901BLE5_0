using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Sensor.Connector.Modular.Udp;
using Wit.SDK.Udp;
using Wit.SDK.Modular.Sensor.Modular.Connector.Role;
using static Wit.SDK.Modular.Sensor.Modular.Connector.Role.TcpServer;

namespace Wit.SDK.Sensor.Connector.Modular.Tcp
{
    /// <summary>
    /// Tcp服务器代理
    /// </summary>
    public class TcpServerProxy
    {
        /// <summary>
        /// udp服务器
        /// </summary>
        private TcpServer tcpServer;

        /// <summary>
        /// 远程服务器ip
        /// </summary>
        private IPEndPoint iPEndPoint;

        /// <summary>
        /// 收到数据事件
        /// </summary>
        public event OnReceiveEvent OnReceive;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="iPEndPoint"></param>
        public TcpServerProxy(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
            tcpServer = TcpServerPool.GetTcpServer(iPEndPoint);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            tcpServer.OnReceive -= UdpReceive;
            tcpServer.Close();
        }

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            tcpServer.Open();
            tcpServer.OnReceive -= UdpReceive;
            tcpServer.OnReceive += UdpReceive;
        }

        /// <summary>
        /// udp收到数据
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="data"></param>
        private void UdpReceive(EndPoint remoteEP, byte[] data)
        {
            OnReceive?.Invoke(remoteEP, data);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="remoteIpPoint"></param>
        public void Send(EndPoint remoteIpPoint, byte[] data)
        {
            tcpServer.SendData(remoteIpPoint.ToString(), data);
        }

        /// <summary>
        /// 发送到全部
        /// </summary>
        /// <param name="data"></param>
        public void AllSend(byte[] data)
        {
            tcpServer.AllSendData(data);
        }
    }
}
