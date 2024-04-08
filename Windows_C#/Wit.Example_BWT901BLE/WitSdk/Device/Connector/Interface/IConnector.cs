using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Wit;
using Wit.SDK;
using Wit.SDK.Modular;
using Wit.SDK.Modular.Sensor;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Interface
{

    /// <summary>
    /// 核心通讯模块接口
    /// </summary>
    public abstract class IConnector
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        protected ConnectStatus ConnectStatus { get; set; } = ConnectStatus.Closed;

        /// <summary>
        /// 接收数据队列
        /// </summary>
        public ConcurrentQueue<byte[]> ReceiveQueue = new ConcurrentQueue<byte[]>();

        /// 发送数据锁
        /// </summary>
        public object OnSendDataLockObj = new object();

        /// <summary>
        /// 接收数据队列
        /// </summary>
        public Thread ReceiveThread;

        /// <summary>
        /// 收数据的人
        /// </summary>
        private List<DataReceivedInterface> receiveListenerList = new List<DataReceivedInterface>();

        /// <summary>
        /// 监听发送数据的人
        /// </summary>
        private List<SendDataInterface> sendDataInterfaceList = new List<SendDataInterface>();

        /// <summary>
        /// 构造
        /// </summary>
        public IConnector() {
            ReceiveThread =  new Thread(HandleReceiveThread) { IsBackground = true };
            ReceiveThread.Start();
        }

        /// <summary>
        /// 对象销毁时
        /// </summary>
        ~IConnector()
        {
            if (ReceiveThread != null) { 
                ReceiveThread.Abort();
            }
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public abstract void CheckConfig();

        /// <summary>
        /// 获得连接配置
        /// </summary>
        /// <returns></returns>
        public abstract IConnectConfig GetConfig();

        /// <summary>
        /// 是否已经打开连接
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return ConnectStatus == ConnectStatus.Opened;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public abstract void Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public abstract void Close();


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public abstract void SendData(byte[] data);

        /// <summary>
        /// 移除接收数据的人
        /// </summary>
        /// <param name="listener"></param>

        public void LogoutReceivedObj(DataReceivedInterface listener) { 
            receiveListenerList.Remove(listener);
        }

        /// <summary>
        /// 添加接收数据的人
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterReceivedObj(DataReceivedInterface listener) { 
            receiveListenerList.Add(listener);
        }

        /// <summary>
        /// 取消发送数据监听者
        /// </summary>
        /// <param name="listener"></param>
        public void LogoutDataDisplayObj(SendDataInterface listener) {
            sendDataInterfaceList.Remove(listener);
        }

        /// <summary>
        /// 添加发送数据监听者
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterDataDisplayObj(SendDataInterface listener) { 
            sendDataInterfaceList.Add(listener);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data"></param>
        public void onReceive(byte[] data)
        {
            ReceiveQueue.Enqueue(data);
        }

        /// <summary>
        /// 处理接收数据线程
        /// </summary>
        private void HandleReceiveThread()
        {
            while (true) {
                Thread.Sleep(1);
                while (ReceiveQueue.Count > 0) {
                    byte[] data = null;
                    ReceiveQueue.TryDequeue(out data);
                    for (int i = 0; i < receiveListenerList.Count; i++)
                    {
                        DataReceivedInterface dataReceived = receiveListenerList[i];
                        dataReceived.OnDataReceived(data);
                    }
                }
            }
        }

        /// <summary>
        /// 如果发送数据
        /// </summary>
        /// <param name="data"></param>
        public void OnSendData(byte[] data)
        {
            for (int i = 0; i < sendDataInterfaceList.Count; i++)
            {
                var item = sendDataInterfaceList[i];
                // 调用线程池完成操作
                ThreadPool.QueueUserWorkItem(new WaitCallback((p) =>
                {
                    lock (OnSendDataLockObj)
                    {
                        item.OnSendData(data);
                    }
                }), null);
            }
        }

    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnectStatus
    {
        /// <summary>
        /// 打开的
        /// </summary>
        Opened = 0,
        /// <summary>
        /// 关闭的
        /// </summary>
        Closed = 1
    }
}
