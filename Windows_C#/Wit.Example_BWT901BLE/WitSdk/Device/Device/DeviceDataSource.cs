using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Device.Device.Device.DKey;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Sensor.Device;
using Wit.SDK.Sensor.Device.Constant;

namespace Wit.SDK.Modular.Sensor.Device
{

    /// <summary>
    /// 设备数据源操作
    /// </summary>
    public class DeviceDataSource : DeviceEvents
    {
        /// <summary>
        /// 传感器数据
        /// </summary>
        public Dictionary<string, object> DeviceData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// 设备的历史数据
        /// </summary>
        public Dictionary<string, List<string>> HistoryDeviceData { get; } = new Dictionary<string, List<string>>();


        #region 数据源操作

        /// <summary>
        /// 删除一个key
        /// </summary>
        public bool RemoveKey(string key)
        {
            return DeviceData.Remove(key);
        }

        /// <summary>
        /// 删除所有设备的key，上位机的保留key不会清除
        /// </summary>
        public bool ClearDeviceKey()
        {
            for (int i = 0; i < DeviceData.Count; i++)
            {
                KeyValuePair<string, object> keyvalue = DeviceData.ElementAt(i);

                //如果不是保留的key值那么都清除
                if (!InnerKeys.ADDR_KEY.Equals(keyvalue.Key))
                {
                    DeviceData.Remove(keyvalue.Key);
                    i--;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取设备数据
        /// </summary>
        /// <param name="key">需要的数据</param>
        /// <returns>数据的值,如果没有就返回null</returns>
        public string GetDeviceData(string key)
        {
            if (DeviceData.ContainsKey(key))
            {
                return DeviceData[key].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取设备历史数据
        /// </summary>
        /// <param name="i">历史第几个数据</param>
        /// <param name="key">数据key</param>
        /// <returns>返回数据，如果没有数据就返回null</returns>
        public string GetHistoryDeviceData(int i, string key)
        {
            if (i < 0)
            {
                return null;
            }

            if (HistoryDeviceData.ContainsKey(key))
            {
                int position = HistoryDeviceData[key].Count - 1 - i;

                if (position >= 0)
                {
                    return HistoryDeviceData[key][position];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(string key, string value)
        {
            Put(key, value);
        }

        /// <summary>
        /// 设置设备地址
        /// </summary>
        /// <param name="addr"></param>
        public void SetAddr(string addr) {

            SetDeviceData(InnerKeys.ADDR_KEY, addr);
        }

        /// <summary>
        /// 获得地址
        /// </summary>
        /// <returns></returns>
        public string GetAddr()
        {
           return GetDeviceData(InnerKeys.ADDR_KEY);
        }

        #endregion
        
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, object value)
        {
            // 当前数据
            DeviceData[key] = value;
            if (this is DeviceModel)
            {
                // Key刷新通知
                InvokeOnKeyUpdate((DeviceModel)this, key, value);
            }

            //// 保存到历史数据
            //if (HistoryDeviceData.ContainsKey(key))
            //{
            //    HistoryDeviceData[key].Add(value);
            //    if (HistoryDeviceData[key].Count > 200)
            //    {
            //        HistoryDeviceData[key].RemoveAt(0);
            //    }
            //}
            //else
            //{
            //    HistoryDeviceData[key] = new List<string>() { value };
            //}
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(ByteKey key, byte value) {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public byte? GetDeviceData(ByteKey dataKey) {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is byte) {
                    return (byte)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(DoubleKey key, double value)
        {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public double? GetDeviceData(DoubleKey dataKey)
        {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is double)
                {
                    return (double)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(FloatKey key, float value)
        {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public float? GetDeviceData(FloatKey dataKey)
        {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is float)
                {
                    return (float)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(IntKey key, int value)
        {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public int? GetDeviceData(IntKey dataKey)
        {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is int)
                {
                    return (int)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(ShortKey key, short value)
        {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public short? GetDeviceData(ShortKey dataKey)
        {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is short)
                {
                    return (short)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDeviceData(StringKey key, string value)
        {
            Put(key.Key, value);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public string? GetDeviceData(StringKey dataKey)
        {
            string key = dataKey.Key;
            if (DeviceData.ContainsKey(key))
            {
                object o = DeviceData[key];
                if (o is string)
                {
                    return (string)o;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
