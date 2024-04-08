using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Wit.SDK.Modular.Sensor.Device.Entity;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// 脚本解释器
    /// </summary>
    public class CmdUtils
    {

        /// <summary>
        /// 构建指令
        /// </summary>
        /// <returns></returns>
        public static string GenerationCmd(string cmd, Dictionary<string, object> dataResource, string value, string deviceName, string text, int valueLength = 0)
        {
            return Encoding.Default.GetString(GenerationCmd(cmd, false, false, dataResource, value, deviceName, text, valueLength));
        }

        /// <summary>
        /// 生成要发送的byte数组
        /// </summary>
        /// <param name="cmdBean"></param>
        /// <returns></returns>
        public static byte[] GenerationCmd(CmdBean cmdBean, Dictionary<string, object> dataResource) {
            return GenerationCmd(cmdBean.sendData, cmdBean.sendHex, cmdBean.sendNewLine, dataResource);
        }

        /// <summary>
        /// 生成要发送的byte数组
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sendHex"></param>
        /// <param name="sendNewLine"></param>
        /// <param name="dataResource"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GenerationCmd(string cmd, bool sendHex, bool sendNewLine, Dictionary<string, object> dataResource, string value = null, string deviceName = null, string text = null, int valueLegnth = 0)
        {

            try
            {
                if (sendHex && !string.IsNullOrEmpty(value))
                {
                    value = int.Parse(value).ToString("X") + "";

                    if (valueLegnth != 0 && value.Length < valueLegnth)
                    {
                        int t = valueLegnth - value.Length;
                        value = value.PadLeft(valueLegnth, '0');
                    }

                    if (value.Length > 2)
                    {
                        value = value.Substring(0, 2) + " " + value.Substring(2);
                    }
                }


                if (value != null)
                    //替换值到命令里
                    cmd = cmd.Replace("${VAL}", value);


                if (deviceName != null)
                    //替换值到命令里
                    cmd = cmd.Replace("${DEVICE_NAME}", deviceName);

                if (text != null)
                    //替换值到命令里
                    cmd = cmd.Replace("${TEXT}", text);


                if (sendHex)
                {
                    //替换变量到命令里
                    cmd = cmdParse(cmd, dataResource, true);

                }
                else
                {
                    cmd = cmdParse(cmd, dataResource, false);
                }


                //要发送的数据
                byte[] data = new byte[0];

                //如果发送回车换行
                if (sendNewLine)
                {
                    cmd = cmd + "\r\n";
                }

                //检查是不是要加crc校验，如果要就先把占位符换出来
                bool isCrc16 = false;
                if (cmd.Contains("${CRC16}"))
                {
                    isCrc16 = true;
                    cmd = cmd.Replace("${CRC16}", "");
                }

                //去除前面和后面的空格
                cmd = cmd.Trim(' ');

                //如果是16进制发送
                if (sendHex)
                {
                    data = ByteArrayConvert.HexStringToByteArray(cmd);
                }
                else
                {
                    data = ByteArrayConvert.StringToByteArray(cmd);
                }

                //如果加crc16校验
                if (isCrc16)
                {
                    byte[] crc16 = Modbus16Utils.GetCrc16(data);
                    List<byte> l = data.ToList();
                    l.AddRange(crc16);
                    data = l.ToArray();
                }
                return data;
            }
            catch (Exception ex)
            {
                return Encoding.Default.GetBytes(ex.Message);
            }
        }



        /// <summary>
        /// 将变量代入到命令里
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string cmdParse(string cmd, Dictionary<string, object> dataResource, bool ishex)
        {

            string[] keyArr = dataResource.Keys.ToArray();

            for (int i = 0; i < keyArr.Length; i++)
            {

                string key = keyArr[i];
                string value = dataResource[key]!=null ? dataResource[key].ToString(): "";

                //变量名称
                string vname = "${" + key + "}";

                //如果命令里包含变量就把值代入进去
                if (cmd.Contains(vname))
                {
                    if (ishex)
                    {
                        cmd = cmd.Replace(vname, int.Parse(value).ToString("X"));
                    }
                    else
                    {
                        cmd = cmd.Replace(vname, value);
                    }

                }

            }

            return cmd;
        }
    }
}
