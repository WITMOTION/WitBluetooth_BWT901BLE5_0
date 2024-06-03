using System;
using Wit.SDK.Modular.Sensor.Modular.Connector.Entity;
using Wit.SDK.Modular.Sensor.Modular.Connector.Interface;
using Wit.SDK.Sensor.Connector.Entity;

namespace Wit.SDK.Sensor.Connector.Role 
{
    /// <summary>
    /// unity蓝牙连接器
    /// </summary>
    public class UnityBleConnect : IConnector
    {
        /// <summary>
        /// 连接器配置
        /// </summary>
        public UnityBleConfig config = new UnityBleConfig();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="config"></param>
        public UnityBleConnect(UnityBleConfig config) {
            this.config = config;
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        public override void CheckConfig()
        {
            if (config.Mac == null)
            {
                throw new Exception("未设置 Mac 地址");
            }

            if (config.ServiceGuid == null)
            {
                throw new Exception("未设置 ServiceGuid");
            }

            if (config.WriteGuid == null)
            {
                throw new Exception("未设置 WriteGuid");
            }

            if (config.NotifyGuid == null)
            {
                throw new Exception("未设置 NotifyGuid");
            }
        }

        public override void Close()
        {
            if (ConnectStatus == ConnectStatus.Closed) {
                return;
            }
            // disconnect
            BluetoothLEHardwareInterface.UnSubscribeCharacteristic(config.Mac, config.ServiceGuid, config.NotifyGuid, (characteristic) => {
                BluetoothLEHardwareInterface.DisconnectPeripheral(config.Mac, (disconnectAddress) => {
                    // 标志为关闭状态
                    ConnectStatus = ConnectStatus.Closed;
                });
            });
        }

        /// <summary>
        /// 获得配置
        /// </summary>
        /// <returns></returns>
        public override IConnectConfig GetConfig()
        {
            return config;
        }

        public override void Open()
        {
            if (ConnectStatus == ConnectStatus.Opened) 
            {
                return;
            }

            CheckConfig();

            BluetoothLEHardwareInterface.ConnectToPeripheral(config.Mac, (address) => {}, null, (address, service, characteristic) => {
                ConnectStatus = ConnectStatus.Opened;
                // 找服务
                subscribe();
            }, null);
        }

        /// <summary>
        /// 找服务
        /// </summary>
        private void subscribe()
        {
            BluetoothLEHardwareInterface.SubscribeCharacteristic(config.Mac, config.ServiceGuid, config.NotifyGuid, null, (characteristic, bytes) => {
                onReceive(bytes);
            });
        }

        public override void SendData(byte[] data)
        {
            BluetoothLEHardwareInterface.WriteCharacteristic(config.Mac, config.ServiceGuid, config.WriteGuid, data, data.Length, false, (characteristic) =>{ });
        }
    }

}