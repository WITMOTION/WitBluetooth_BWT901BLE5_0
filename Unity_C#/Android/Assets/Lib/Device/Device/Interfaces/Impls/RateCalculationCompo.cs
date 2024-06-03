using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Sensor.Device.Constant;

namespace Wit.SDK.Sensor.Device.Interfaces
{
    /// <summary>
    /// 速率计算组件
    /// </summary>
    public class RateCalculationCompo : IDeviceComponent
    {

        /// <summary>
        /// 数据大小速率
        /// </summary>
        private int DataSizeRate = 0;

        /// <summary>
        /// 数据包速率
        /// </summary>
        private int PacketRate = 0;

        /// <summary>
        /// 设备模型
        /// </summary>
        private DeviceModel deviceModel;

        /// <summary>
        /// 是否在运行
        /// </summary>
        private bool IsRun = true;

        /// <summary>
        /// 线程
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 设备打开时
        /// </summary>
        /// <param name="deviceModel"></param>
        public void OnOpen(DeviceModel deviceModel)
        {
            this.deviceModel = deviceModel;
            IsRun = true;
            thread = new Thread(MyThread) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// 线程
        /// </summary>
        private void MyThread()
        {
            while (IsRun) {
                PacketRate = 0;
                DataSizeRate = 0;
                Thread.Sleep(1000);
                deviceModel.SetDeviceData(InnerKeys.PacketRate,PacketRate.ToString());
                deviceModel.SetDeviceData(InnerKeys.DataSizeRate,DataSizeRate.ToString());
            }
        }

        /// <summary>
        /// 设备关闭时
        /// </summary>
        /// <param name="deviceModel"></param>
        public void OnClose(DeviceModel deviceModel)
        {
            IsRun = false;
            try
            {
                if (thread != null) { 
                    thread.Abort();
                }
            }
            catch (Exception ex) { 
                Debug.WriteLine(ex);
            }
        }

        public void OnKeyUpdate(DeviceModel deviceModel, string key, object value)
        {
        }

        public void OnUpdate(DeviceModel deviceModel)
        {
            PacketRate++;
        }

        public void OnReceiveData(DeviceModel deviceModel, byte[] data)
        {
            DataSizeRate += data.Length;
        }

        public void OnReadData(DeviceModel deviceModel, byte[] sendData, int delay = -1)
        {
        }

        public void OnSend(DeviceModel deviceModel, byte[] data)
        {

        }

        public void OnRemove()
        {
        }
    }
}
