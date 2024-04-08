using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wit.SDK.Modular.Sensor.Device;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// AT指令帮助类
    /// </summary>
    public class ATCommandHelper
    {
        /// <summary>
        /// 发送读取的命令，并且查找返回结果
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string SendReadCommandAndFindLine(DeviceModel deviceModel, string sendData, string[] condition, int repeat = 3)
        {
            string result = null;
            int i = 0;
            // 如果没有结果就重复指定次数
            while (++i <= repeat)
            {
                string data = SendReadCommand(deviceModel, sendData);

                for (int j = 0; j < condition.Length; j++)
                {
                    result = FindLine(data, condition[j]);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// 发送读取的命令
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public static string SendReadCommand(DeviceModel deviceModel, string sendData)
        {
            byte[] returnData;
            deviceModel.SendData(Encoding.Default.GetBytes(sendData), out returnData, true, 900, 2);
            string result = Encoding.Default.GetString(returnData);
            return result;
        }

        /// <summary>
        /// 查找返回结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string FindLine(string data, string condition)
        {
            string[] rows = data.Split('\r', '\n');

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Contains(condition))
                {
                    return rows[i];
                }
            }
            return null;
        }
    }
}
