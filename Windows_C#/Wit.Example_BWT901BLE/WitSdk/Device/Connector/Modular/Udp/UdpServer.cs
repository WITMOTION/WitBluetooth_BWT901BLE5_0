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

namespace Wit.SDK.Sensor.Connector.Modular.Udp
{

    /// <summary>
    /// Udp服务器
    /// </summary>
    public class UdpServer
    {
        /// <summary>
        /// UDP客户端
        /// </summary>
        private Socket newsock;

        //private UdpClient UdpClient { get; set; }

        /// <summary>
        /// 是否打开的
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// 本地ip和端口
        /// </summary>
        public IPEndPoint LocalIpPoint = null;

        /// <summary>
        /// 自动任务线程
        /// </summary>
        private Thread AutoTaskThread = null;

        /// <summary>
        /// 是否运行任务线程
        /// </summary>
        private bool AutoTaskRun = false;
        
        /// <summary>
        /// 打开设备锁
        /// </summary>
        private object openlock = new object();

        /// <summary>
        /// 收到数据委托
        /// </summary>
        /// <param name="data"></param>
        public delegate void OnReceiveEvent(IPEndPoint remoteEP,byte[] data);

        /// <summary>
        /// 收到数据事件
        /// </summary>
        public event OnReceiveEvent OnReceive;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="localIpPoint"></param>
        private UdpServer(IPEndPoint localIpPoint)
        {
            LocalIpPoint = localIpPoint;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public void CheckConfig()
        {
            if (LocalIpPoint == null)
            {
                throw new Exception("未设置udp本地监听地址和端口");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (OnReceive != null) {
                return;
            }

            if (newsock != null)
            {
                try
                {
                    newsock.Close();
                }
                catch (Exception ex) { 
                    Debug.WriteLine(ex);
                }
            }

            if (AutoTaskThread != null)
            {
                AutoTaskRun = false;
                // AutoTaskThread.Abort();
            }

            // 标志为关闭状态
            IsOpen = false;
        }
        

        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            lock (openlock) {
                // 不重复打开
                if (IsOpen)
                {
                    return;
                }

                CheckConfig();

                if (newsock != null)
                {
                    try
                    {
                        newsock.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                }
                // 新建套接字,基于IPv和4UDP协议
                newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // 绑定本机ip地址
                newsock.Bind(LocalIpPoint);

                // 读取数据线程
                AutoTaskThread = new Thread(AutoTaskMethod);
                AutoTaskRun = true;
                AutoTaskThread.IsBackground = true;
                AutoTaskThread.Start();

                IsOpen = true;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="remoteEP"></param>
        /// <exception cref="Exception"></exception>
        public void Send(byte[] data, IPEndPoint remoteEP)
        {
            //检查状态
            if (IsOpen == false)
            {
                throw new Exception("发送失败，未打开连接");
            }
            newsock.SendTo(data, data.Length, SocketFlags.None, remoteEP);
        }

        /// <summary>
        /// 自动任务线程方法
        /// </summary>
        private void AutoTaskMethod()
        {
            int length;
            //存储数据
            byte[] data = new byte[1024];

            while (AutoTaskRun)
            {
                // 休眠1毫秒防止cpu占用率过高
                //Thread.Sleep(1);
                try
                {
                    //得到客户机IP
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remoteEP = (EndPoint)(sender);

                    length = newsock.ReceiveFrom(data, ref remoteEP);
                    if (length > 0)
                    {
                        byte[] buf = new byte[length];
                        Array.Copy(data, buf, length);
                        InvokeOnReceive((IPEndPoint)remoteEP, buf);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 收到数据时
        /// </summary>
        /// <param name="data"></param>
        private void InvokeOnReceive(IPEndPoint remoteEP, byte[] data)
        {
            OnReceive?.Invoke(remoteEP,data);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="iPEndPoint"></param>
        /// <returns></returns>
        public static UdpServer CreateInstance(IPEndPoint iPEndPoint)
        {
            return new UdpServer(iPEndPoint);
        }
    }
}
