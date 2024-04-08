using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Udp;
using static Wit.SDK.Sensor.Connector.Modular.Udp.UdpServer;

namespace Wit.SDK.Sensor.Connector.Modular.Udp
{
    /// <summary>
    /// Udp服务器代理
    /// </summary>
    public class UdpServerProxy
    {
        /// <summary>
        /// udp服务器
        /// </summary>
        private UdpServer udpServer;

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
        public UdpServerProxy(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
            udpServer = UdpServerPool.GetUdpServer(iPEndPoint);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            udpServer.OnReceive -= UdpReceive;
            udpServer.Close();
        }

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            udpServer.Open();
            udpServer.OnReceive-= UdpReceive;
            udpServer.OnReceive+= UdpReceive;
        }

        /// <summary>
        /// udp收到数据
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="data"></param>
        private void UdpReceive(IPEndPoint remoteEP, byte[] data)
        {
            OnReceive?.Invoke(remoteEP, data);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="remoteIpPoint"></param>
        public void Send(byte[] data, IPEndPoint remoteIpPoint)
        {
            udpServer.Send(data, remoteIpPoint);
        }
    }
}
