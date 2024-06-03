using Assets.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Bluetooth
{
    /*
     * 蓝牙连接器，连接并接受蓝牙数据
     * Bluetooth connector, connecting and receiving Bluetooth data
     */
    public class BlueConnector
    {
        private static BlueConnector _instance;

        // 蓝牙传感器UUID Bluetooth sensor UUID
        public static readonly string UUID_SERVICE = "0000ffe5-0000-1000-8000-00805f9a34fb";
        public static readonly string UUID_READ = "0000ffe4-0000-1000-8000-00805f9a34fb";
        public static readonly string UUID_WRITE = "0000ffe9-0000-1000-8000-00805f9a34fb";

        // 接收数据线程  Receive data thread
        private Thread receiveTh;

        public bool isConnect = false;

        // 收到数据事件 Received data event
        public delegate void ReceiveEventHandler(string deviceId, byte[] data);
        public event ReceiveEventHandler OnReceive;

        private BlueConnector() { }

        public static BlueConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BlueConnector();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 开始连接 Start connecting
        /// </summary>
        public void Connect(string deviceId)
        {
            try
            {
                BleApi.SubscribeCharacteristic(deviceId, UUID_SERVICE, UUID_READ, false);
                Debug.Log("连接设备成功");
                if (isConnect) {
                    return;
                }
                isConnect = true;
                receiveTh = new Thread(ReceiveData);
                receiveTh.IsBackground = true;
                receiveTh.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.Log("连接设备失败");
            }
        }

        /// <summary>
        /// 接收数据线程 Receive data thread
        /// </summary>
        private void ReceiveData() {
            BleApi.BLEData res = new BleApi.BLEData();
            while (true) {
                while (isConnect && BleApi.PollData(out res, false))
                {
                    OnReceive?.Invoke(res.deviceId, res.buf);
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 关闭连接 Close Connection
        /// </summary>
        public void Disconnect() {
            if (DevicesManager.Instance.isHaveOpenDevice()) {
                return;
            }
            try
            {
                isConnect = false;
                Thread.Sleep(200);
                receiveTh.Abort();
                receiveTh = null;
            }
            catch (Exception)
            {
                // 捕捉异常但不处理
            }
        }    
    }
}
