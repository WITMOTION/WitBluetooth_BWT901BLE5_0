using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Role;
using Wit.SDK.Sensor.Connector.Modular.Udp;

namespace Wit.SDK.Udp
{
    /// <summary>
    /// UDP服务器池
    /// </summary>
    public class UdpServerPool
    {
        /// <summary>
        /// 字典
        /// </summary>
        public static ConcurrentDictionary<int, UdpServer> Dict = new ConcurrentDictionary<int, UdpServer>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static object lockobj = new object();

        /// <summary>
        /// 创建UDP服务器
        /// </summary>
        public static UdpServer GetUdpServer(IPEndPoint iPEndPoint)
        {
            lock (lockobj)
            {
                if (Dict.ContainsKey(iPEndPoint.Port))
                {
                    return Dict[iPEndPoint.Port];
                }
                else
                {
                    var con = UdpServer.CreateInstance(iPEndPoint);
                    Dict.TryAdd(iPEndPoint.Port, con);
                    return con;
                }
            }
        }
    }
}
