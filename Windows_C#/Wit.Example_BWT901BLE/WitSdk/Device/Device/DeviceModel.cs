using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Wit.SDK.Device.Sensor.Device.Utils;
using Wit.SDK.Modular.Sensor.Device.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;
using Wit.SDK.Modular.Sensor.Modular.Connector.Role;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Modular.Sensor.Modular.ProtocolResolver.Interface;
using Wit.SDK.Sensor.Device.Interfaces;
namespace Wit.SDK.Modular.Sensor.Device
{
    /// <summary>
    /// 基础设备
    /// </summary>
    public class DeviceModel : DeviceDataSource, DataReceivedInterface
    {

        /// <summary>
        /// 设备ID，设备的唯一名称
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 是否是回放的设备
        /// </summary>
        public bool IsPlaybackDevice { get; set; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public IConnector Connector { get; set; }
        
        /// <summary>
        /// 监听Key值
        /// </summary>
        public string ListenerKey { get; set; } = "";

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeviceModel(string deviceId,string deviceName,IProtocolResolver protocolResolver, IDataProcessor dataProcessor, string listenerKey)
        {
            ListenerKey = listenerKey == null ? "": listenerKey;
            DeviceName = deviceName;
            DeviceId = deviceId;

            // 设备配置
            DeviceOption = new DeviceOption()
            {
                ListenerKey = ListenerKey,
                DataProcessorType = dataProcessor != null? dataProcessor.GetType(): null,
                ProtocolResolverType = protocolResolver != null? protocolResolver.GetType(): null,
            };

            // 注册计数据速率组件
            RateCalculationCompo rateCalculationCompo = new RateCalculationCompo();
            AddComponent(rateCalculationCompo);
            // 注册数据处理器
            AddComponent(dataProcessor);
            // 注册协议解析器
            AddComponent(protocolResolver);

            this.OnKeyUpdate += DeviceModel_OnKeyUpdate;

        }

        /// <summary>
        /// 键值更新时
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void DeviceModel_OnKeyUpdate(DeviceModel deviceModel, string key, object value)
        {
            // 触发实时数据更新
            if (ListenerKey.Equals(key) || string.IsNullOrEmpty(ListenerKey))
            {
                // 键值改变事件
                InvokeOnListenKeyUpdate(this);
            }
        }


        #region 打开和关闭设备的操作

        /// <summary>
        /// 是否打开
        /// </summary>
        private bool _IsOpen { get; set; } = false;

        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsOpen { get { return _IsOpen; } }

        /// <summary>
        /// 是否正在关闭
        /// </summary>
        public bool Closing { get; set; }

        /// <summary>
        /// 设备配置
        /// </summary>
        public DeviceOption DeviceOption { get; private set; }

        /// <summary>
        /// 打开设备
        /// </summary>
        public void OpenDevice()
        {
            //如果是回放的设备就直接标为打开,不用做其它事情
            if (IsPlaybackDevice)
            {
                // 标记为已经打开
                _IsOpen = true;
                InvokeOnOpened(this);
                return;
            }

            if (Connector != null) {
                //注销接收数据
                Connector.LogoutReceivedObj(this);
                //注册接收数据
                Connector.RegisterReceivedObj(this);
                //如果核心连接没有打开就打开连接
                if (Connector.IsOpen() == false) Connector.Open();
                _IsOpen = true;
                InvokeOnOpened(this);
                return;
            }
            else
            {
                _IsOpen = false;
                throw new Exception("打开设备错误,没有coreConnenct对象");
            }
        }

        /// <summary>
        /// 重新打开
        /// </summary>
        /// <param name="v"></param>
        public void ReOpen()
        {
            CloseDevice();
            OpenDevice();
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public void CloseDevice()
        {
            Closing = true;
            if (Connector != null)
            {
                //注销接收数据
                Connector.LogoutReceivedObj(this);
                //关闭连接
                Connector.Close();

            }

            if (_IsOpen) {
                // 关闭事件
                InvokeOnClosed(this);
            }
            _IsOpen = false;
            Closing = false;
        }

        /// <summary>
        /// 设置串口波特率
        /// </summary>
        /// <param name="baudRate"></param>
        public void SetSerialPortBaud(int baudRate)
        {
            if (Connector is SPConnector) {
                SPConnector sP = Connector as SPConnector;
                sP.SetBaud(baudRate);
            }
        }

        #endregion

        #region 发送数据和接收数据

        /// <summary>
        /// 发送数据锁
        /// </summary>
        private readonly object SendLock = new object();

        /// <summary>
        /// 接收数据锁
        /// </summary>
        private readonly object ReceiveLock = new object();

        /// <summary>
        /// 是否接收返回数据
        /// </summary>
        private bool BolWaitReturn;

        /// <summary>
        /// 返回的数据
        /// </summary>
        private List<byte> ReturnDataBuffer = new List<byte>();

        /// <summary>
        /// 发送数据，不等待返回
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(byte[] data) {
            byte[] outData = new byte[0];   
            return SendData(data, out outData, false);
        }

        /// <summary>
        /// 发送数据，并等待返回
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="returnData">返回的数据</param>
        /// <param name="isWaitReturn">是否等待返回</param>
        /// <param name="waitTime">等待时间</param>
        /// <param name="repetition">等待次数</param>
        /// <returns></returns>
        public bool SendData(byte[] data, out byte[] returnData, bool isWaitReturn = false, int waitTime = 100, int repetition = 1) {

            // 发送数据锁
            lock (SendLock)
            {
                byte[] rtnData = new byte[0];
                bool success = false;
                // 标记线程释是否结束
                bool newThreadRuning = true;
                // 开启线程池发送数据
                ThreadPool.QueueUserWorkItem(new WaitCallback((p) =>
                {
                    try
                    {
                        success = SendDataTh(data, out rtnData, isWaitReturn, waitTime, repetition);
                    }
                    catch (Exception ex) {
                        Debug.WriteLine(ex.Message);
                        Debug.WriteLine(ex.StackTrace);
                    }
                    // 标记线程结束
                    newThreadRuning = false;
                }), null);

                // 线程没有结束就一直等
                while (newThreadRuning) {
                    Thread.Sleep(1);
                    // 防止ui线程卡住
                    ReadDataSleepManager.InvokeOnSleep();
                }
                returnData = rtnData;
                return success;
            }
        }

        /// <summary>
        /// 发送数据，可以有返回值
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="returnData">返回的数据</param>
        /// <param name="isWaitReturn">是否等待返回</param>
        /// <param name="waitTime">等待时间</param>
        /// <param name="repetition">等待次数</param>
        /// <returns></returns>
        private bool SendDataTh(byte[] data, out byte[] returnData, bool isWaitReturn = false, int waitTime = 100, int repetition = 1)
        {
            // 如果正在关闭就不发送
            if (Closing ||
            Connector == null ||
            Connector.IsOpen() == false
            )
            {
                returnData = new byte[0];
                return false;
            }
            try
            {
                ReturnDataBuffer.Clear();
                BolWaitReturn = isWaitReturn;
                returnData = new byte[0];
                int count = 0;
                if (BolWaitReturn)
                {
                    while (count < repetition && BolWaitReturn)
                    {
                        count++;
                        // 发送数据
                        Connector.SendData(data);
                        // 发送数据事件
                        InvokeOnSendData(this, data);
                        Thread.Sleep(waitTime);
                        returnData = ReturnDataBuffer.ToArray();
                        if (returnData != null && returnData.Length > 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    InvokeOnSendData(this, data);
                    // 发送数据
                    Connector.SendData(data);
                }
                BolWaitReturn = false;
                return true;
            }
            catch
            {
                returnData = new byte[0];
                BolWaitReturn = false;
                return false;
            }
        }

        /// <summary>
        /// 使用数据解析器发送数据
        /// </summary>
        public bool ReadData(byte[] data, int delay = -1)
        {
            if (this.IsPlaybackDevice)
            {
                return false;
            }

            try
            {
                // 发送数据
                InvokeOnReadData(this, data, delay);
                return true;
            }
            catch (Exception e) {
                Debug.WriteLine("发送数据出错：" + e.Message);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delay"></param>
        public void AsyncReadData(byte[] data, Action callback, int delay = -1)
        {
            if (this.IsPlaybackDevice) { return; }

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((p) =>
                {
                    // 发送数据
                    InvokeOnReadData(this, data, delay);
                    // 调用回调方法
                    callback.Invoke();
                }), null);
            }
            catch (Exception e)
            {
                Debug.WriteLine("发送数据出错：" + e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="data"></param>
        public void OnDataReceived(byte[] data) {
            // 如果正在关闭就不接收数据
            if (Closing)
            {
                return;
            }


            //如果需要返回
            if (BolWaitReturn)
            {
                try
                {
                    ReturnDataBuffer.AddRange(data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }


            // 线程锁
            lock (ReceiveLock)
            {
                // 触发原始数据事件
                InvokeOnReceiveData(this, data);
            }
        }

        #endregion
    }
}
