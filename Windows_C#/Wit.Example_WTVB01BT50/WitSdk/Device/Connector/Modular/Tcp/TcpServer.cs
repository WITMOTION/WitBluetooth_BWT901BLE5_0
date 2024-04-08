using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Wit.SDK.Modular.Sensor.Modular.Connector.Role
{

    /// <summary>
    /// 服务端
    /// </summary>
    public class TcpServer
    {
        /// <summary>
        /// 服务端
        /// </summary>
        public Socket ServerSocket = null;

        /// <summary>
        /// tcp客户端字典
        /// </summary>
        public ConcurrentDictionary<string, ClientSession> ClientSocketDic = new ConcurrentDictionary<string, ClientSession>();
        
        /// <summary>
        /// 监听客户端连接的标志
        /// </summary>
        private bool FlagListen = false;
        
        /// <summary>
        /// 监听的IP和端口
        /// </summary>
        private IPEndPoint iPEndPoint;

        /// <summary>
        /// 收到数据委托
        /// </summary>
        /// <param name="data"></param>
        public delegate void OnReceiveEvent(EndPoint remoteEP, byte[] data);

        /// <summary>
        /// 客户端数据接收事件
        /// </summary>
        public event OnReceiveEvent OnReceive;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="iPEndPoint"></param>
        public TcpServer(IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="iPEndPoint"></param>
        /// <returns></returns>
        internal static TcpServer CreateInstance(IPEndPoint iPEndPoint)
        {
            TcpServer server = new TcpServer(iPEndPoint);
            return server;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port">端口号</param>
        public void Open(IPEndPoint endPoint) {
            iPEndPoint = endPoint;
            Open();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port">端口号</param>
        public void Open()
        {
            OpenOrClose(true);
        }


        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            OpenOrClose(false);
        }


        /// <summary>
        /// 打开或者关闭
        /// </summary>
        /// <param name="_open"></param>
        private void OpenOrClose(bool _open) {
            lock (ClientSocketDic) {
                if (_open)
                {
                    ToOpen();
                }
                else { 
                    ToClose();
                }
            }
        }

        /// <summary>
        /// 打开逻辑
        /// </summary>
        /// <returns></returns>
        private void ToOpen()
        {
            if (FlagListen) {
                return;
            }

            FlagListen = true;
            // 创建负责监听的套接字，注意其中的参数；
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 将负责监听的套接字绑定到唯一的ip和端口上；
            ServerSocket.Bind(iPEndPoint);
            // 设置监听队列的长度；
            ServerSocket.Listen(100);
            // 创建负责监听的线程；
            Thread Thread_ServerListen = new Thread(ListenConnecting);
            Thread_ServerListen.IsBackground = true;
            Thread_ServerListen.Start();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void ToClose()
        {
            if (OnReceive != null)
            {
                return;
            }

            FlagListen = false;

            // 关闭服务器
            if (ServerSocket != null)
            {
                try
                {
                    //ServerSocket.Shutdown(SocketShutdown.Both);
                    //ServerSocket.Close();
                    ServerSocket.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }


            // 关闭每一个客户端
            List<ClientSession> list = new List<ClientSession>();
            foreach (var item in ClientSocketDic)
            {
                list.Add(item.Value);
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Close();
            }
            ClientSocketDic.Clear();
        }

        /// <summary>
        /// 监听客户端请求的方法；
        /// </summary>
        private void ListenConnecting()
        {
            while (FlagListen)
            {
                // 每100ms找一次
                Thread.Sleep(100);

                try
                {
                    // 一旦监听到一个客户端的请求，就返回一个与该客户端通信的 套接字；
                    Socket sokConnection = ServerSocket.Accept();
                    // 将与客户端连接的 套接字 对象添加到集合中；
                    string str_EndPoint = sokConnection.RemoteEndPoint.ToString();
                    ClientSession myTcpClient = new ClientSession() { 
                        id = str_EndPoint,
                        remoteEP = sokConnection.RemoteEndPoint,
                        tcpSocket = sokConnection,
                        tcpServer = this,
                    };
                    //创建线程接收数据
                    ClientSocketDic.TryAdd(str_EndPoint, myTcpClient);
                }
                catch(Exception ex)
                {
                    if (FlagListen) { 
                        Debug.WriteLine(ex);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// 发送数据给指定的客户端
        /// </summary>
        /// <param name="_endPoint">客户端套接字</param>
        /// <param name="_buf">发送的数组</param>
        /// <returns></returns>
        public bool SendData(string _endPoint, byte[] _buf)
        {
            ClientSession myT = null;
            if (ClientSocketDic.TryGetValue(_endPoint, out myT))
            {
                myT.Send(_buf);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 发送数据给所有的客户端
        /// </summary>
        /// <param name="_buf">发送的数组</param>
        /// <returns></returns>
        public bool AllSendData(byte[] _buf)
        {
            foreach (var item in ClientSocketDic)
            {
                ClientSession myT = item.Value;
                myT.Send(_buf);
            }
            return true;
        }

        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="remoteEP"></param>
        /// <param name="buf"></param>
        public void InvokeOnReceive(EndPoint remoteEP, byte[] buf)
        {
            OnReceive?.Invoke(remoteEP, buf);
        }
    }

    /// <summary>
    /// 会话端
    /// </summary>
    public class ClientSession
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public TcpServer tcpServer;

        /// <summary>
        /// 客户端
        /// </summary>
        public Socket tcpSocket;//socket对象

        /// <summary>
        /// 数据缓存
        /// </summary>
        public List<byte> DataBuffer = new List<byte>();//数据缓存区
        
        /// <summary>
        /// 远程IP
        /// </summary>
        public EndPoint remoteEP;
           
        public string id;

        /// <summary>
        /// 是否启动的
        /// </summary>
        private bool Flag_Receive;

        public ClientSession()
        {
            Thread th = new Thread(this.ReceiveData) { 
                IsBackground = true,
            };
            th.Start();
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveData()
        {
            Flag_Receive = true;
            while (Flag_Receive)
            {
                // 接收数据延时
                Thread.Sleep(10);
                try
                {
                    // 定义一个2M的缓存区；
                    byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                    // 将接受到的数据存入到输入  arrMsgRec中；
                    int length = -1;
                    length = tcpSocket.Receive(arrMsgRec); // 接收数据，并返回数据的长度

                    if (length > 0)
                    {
                        byte[] buf = new byte[length];
                        Array.Copy(arrMsgRec, buf, length);
                        tcpServer.InvokeOnReceive(remoteEP, buf);
                    }
                }
                catch (Exception ex)
                {
                    if (Flag_Receive) {
                        Debug.WriteLine(ex);
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buf"></param>
        public void Send(byte[] buf)
        {
            if (buf != null)
            {
                tcpSocket.Send(buf);
            }
        }

        /// <summary>
        /// 获取连接的ip
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            IPEndPoint clientipe = (IPEndPoint)tcpSocket.RemoteEndPoint;
            string _ip = clientipe.Address.ToString();
            return _ip;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (Flag_Receive) {
                try
                {
                    Flag_Receive = false;
                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                // 从通信线程集合中删除被中断连接的通信线程对象；
                ClientSession session = null;
                tcpServer.ClientSocketDic.TryRemove(id, out session);//删除客户端字典中该socket
            }
        }

        /// <summary>
        /// 提取正确数据包
        /// </summary>
        public byte[] GetBuffer(int startIndex, int size)
        {
            byte[] buf = new byte[size];
            DataBuffer.CopyTo(startIndex, buf, 0, size);
            DataBuffer.RemoveRange(0, startIndex + size);
            return buf;
        }

        /// <summary>
        /// 添加队列数据
        /// </summary>
        /// <param name="buffer"></param>
        public void AddQueue(byte[] buffer)
        {
            DataBuffer.AddRange(buffer);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearQueue()
        {
            DataBuffer.Clear();
        }
    }
}
