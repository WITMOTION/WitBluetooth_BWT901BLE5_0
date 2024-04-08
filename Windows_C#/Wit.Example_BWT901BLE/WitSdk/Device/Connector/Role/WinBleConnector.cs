using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wit.Bluetooth.WinBlue;
using Wit.Bluetooth.WinBlue.Entity;
using Wit.Bluetooth.WinBlue.Enums;
using Wit.Bluetooth.WinBlue.Utils;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;
using Wit.SDK.Sensor.Connector.Entity;

namespace Wit.SDK.Sensor.Connector.Role
{

    /// <summary>
    /// Window蓝牙连接器
    /// </summary>
    public class WinBleConnector : IConnector
    {
        // 配置
        public WinBleConfig Config = new WinBleConfig();

        // 连接对象
        public WinBlueClient WinBlueClient;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Config"></param>
        public WinBleConnector(WinBleConfig Config) {
            this.Config = Config;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            if (Config.Mac == null)
            {
                throw new Exception("未设置 Mac 地址");
            }

            if (Config.ServiceGuid == null)
            {
                throw new Exception("未设置 ServiceGuid");
            }

            if (Config.WriteGuid == null)
            {
                throw new Exception("未设置 WriteGuid");
            }

            if (Config.NotifyGuid == null)
            {
                throw new Exception("未设置 NotifyGuid");
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            new Thread(() =>
            {
                if (WinBlueClient != null)
                {
                    WinBlueClient.Disconnect();
                }
            }) { IsBackground = true }.Start();

            // 标志为关闭状态
            ConnectStatus = ConnectStatus.Closed;
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return Config;
        }

        /// <summary>
        /// 打开
        /// </summary>
        public override void Open()
        {
            CheckConfig();

            WinBlueClient = WinBlueFactory.GetInstance().GetWinBlueClient(
                new WinBleOption()
                {
                    ServiceGuid = Config.ServiceGuid,
                    WriteGuid = Config.WriteGuid,
                    NotifyGuid = Config.NotifyGuid,
                    Mac = Config.Mac,
                    DeviceName = Config.DeviceName,
                }
            );
            WinBlueClient.OnReceive -= WinBlueClient_OnReceive;
            WinBlueClient.OnReceive += WinBlueClient_OnReceive;
            WinBlueClient.Connect();

            ConnectStatus = ConnectStatus.Opened;
        }

        /// <summary>
        /// 收到蓝牙数据时
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        private void WinBlueClient_OnReceive(BluetoothEvent type, string mac, byte[] data = null)
        {
            if (type == BluetoothEvent.Data && data!=null) { 
                onReceive(data);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public override void SendData(byte[] data)
        {
            if (WinBlueClient != null)
            {
                OnSendData(data);
                WinBlueClient.Write(data);
            }
        }
    }
}
