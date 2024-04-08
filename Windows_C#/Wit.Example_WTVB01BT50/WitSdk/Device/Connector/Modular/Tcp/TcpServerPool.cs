using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Modular.Connector.Role;
using Wit.SDK.Sensor.Connector.Modular.Udp;

namespace Wit.SDK.Sensor.Connector.Modular.Tcp
{
    /// <summary>
    /// Tcp服务器代理
    /// </summary>
    public class TcpServerPool
    {
        /// <summary>
        /// 字典
        /// </summary>
        public static ConcurrentDictionary<int, TcpServer> Dict = new ConcurrentDictionary<int, TcpServer>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static object lockobj = new object();

        /// <summary>
        /// 创建UDP服务器
        /// </summary>
        public static TcpServer GetTcpServer(IPEndPoint iPEndPoint)
        {
            lock (lockobj)
            {
                if (Dict.ContainsKey(iPEndPoint.Port))
                {
                    return Dict[iPEndPoint.Port];
                }
                else
                {
                    var con = TcpServer.CreateInstance(iPEndPoint);
                    Dict.TryAdd(iPEndPoint.Port, con);
                    return con;
                }
            }
        }
    }
}
