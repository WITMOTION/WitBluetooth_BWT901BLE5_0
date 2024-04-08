using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Wit;
using Wit.SDK;
using Wit.SDK.Modular;
using Wit.SDK.Modular.Sensor;
using Wit.SDK.Modular.Sensor.Device.Entity;

namespace Wit.SDK.Modular.Sensor.Device.Entity
{
    public class CmdBean
    {


        [XmlIgnore]
        private string _sendData = "";

        /// <summary>
        /// 命令
        /// </summary>
        public string sendData
        {
            get
            {
                return _sendData;
            }
            set
            {
                _sendData = value;
                //if (!value.Equals(_sendData))
                //{
                //    _sendData = value;

                //    NotifyPropertyChange(nameof(sendData));
                //}
            }
        }


        private int _valueLength = 0;

        public int valueLength
        {
            get
            {
                return _valueLength;
            }
            set
            {
                _valueLength = value;
                //NotifyPropertyChange(nameof(valueLength));
            }
        }


        [XmlIgnore]
        private bool _sendHex = false;

        /// <summary>
        /// 是否十六进制处理
        /// </summary>
        public bool sendHex
        {
            get
            {
                return _sendHex;
            }
            set
            {
                if (!value.Equals(_sendHex))
                {
                    _sendHex = value;

                    //NotifyPropertyChange(nameof(sendHex));
                }
            }
        }

        [XmlIgnore]
        private bool _sendNewLine = false;

        /// <summary>
        /// 是否添加换行
        /// </summary>
        public bool sendNewLine
        {
            get
            {
                return _sendNewLine;
            }
            set
            {

                _sendNewLine = value;

                //NotifyPropertyChange(nameof(sendNewLine));

            }
        }


        [XmlIgnore]
        private string _sendParseValue = "";
        public string sendParseValue
        {

            get
            {
                return _sendParseValue;
            }
            set
            {
                if (!value.Equals(_sendParseValue))
                    _sendParseValue = value;
            }

        }

        [XmlIgnore]
        private int _delay = 100;

        /// <summary>
        /// 延时时间
        /// </summary>
        public int delay
        {
            get
            {
                return _delay;
            }
            set
            {
                if (!value.Equals(_delay))
                {
                    _delay = value;

                    //NotifyPropertyChange(nameof(delay));
                }
            }
        }

        [XmlIgnore]
        private string _statsText = "";

        public string statsText
        {
            get
            {
                return _statsText;
            }
            set
            {
                _statsText = value;
                //NotifyPropertyChange(nameof(statsText));
            }
        }


        /// <summary>
        /// new Property<string>("", "发送数据完成后对上位机进行调整的指令\r\n" +
        // "SetBaud:deviceName:Baud //设置设备波特率\r\n" +
        // "SetBaud:modeName //更改上位机模式\r\n" +
        // "SearchDevice //让上位机开始搜索\r\n" +
        // "SetModbusDeviceId:deviceName:ModbusId:isHex//设置设备ModbusID", "发送数据");
        /// </summary>
        [XmlIgnore]
        private string _sendEndCmd = "";


        public string sendEndCmd
        {
            get
            {
                return _sendEndCmd;
            }
            set
            {
                _sendEndCmd = value;
            }

        }


        [XmlIgnore]
        private int _sort = 0;

        public int sort
        {
            get => _sort;
            set
            {
                _sort = value;
            }
        }
    }
}
