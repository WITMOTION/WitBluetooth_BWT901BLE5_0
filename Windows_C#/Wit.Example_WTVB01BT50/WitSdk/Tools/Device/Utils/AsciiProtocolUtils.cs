using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Wit;
using Wit.SDK;
using Wit.SDK.Modular.Sensor.Utils;
using Wit.SDK.Utils;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// ASCII协议解析
    /// </summary>
    public class AsciiProtocolUtils
    {
        /// <summary>
        /// 通过AsciiKeyPattern解析字符串，解析后返回key的字典和值
        /// </summary>
        /// <param name="input">输入的待解析字符串</param>
        /// <param name="asciiKeyPatterns">输入前必须先解析好key和pattern</param>
        /// <returns></returns>
        public static Dictionary<string, string> ResolvesAsciiByAsciiKeyPatterns(string input, List<AsciiKeyPattern> asciiKeyPatterns)
        {


            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var asciiKeyPattern in asciiKeyPatterns)
            {
                dic = ResolvesAsciiByAsciiKeyPattern(input, asciiKeyPattern);
                if (dic.Keys.Count > 0)
                {
                    break;
                }
            }

            return dic;
        }

        /// <summary>
        /// 通过AsciiKeyPattern解析字符串，解析后返回key的字典和值
        /// 
        /// </summary>
        /// <param name="input">输入的待解析字符串</param>
        /// <param name="asciiKeyPattern">输入前必须先解析好key和pattern</param>
        /// <returns></returns>
        public static Dictionary<string, string> ResolvesAsciiByAsciiKeyPattern(string input, AsciiKeyPattern asciiKeyPattern)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string matchs = @asciiKeyPattern.pattern + "\r\n";
            //string matchs = @asciiKeyPattern.pattern;
            input = input + "\r\n";
            MatchCollection mc = Regex.Matches(input, matchs);

            if (mc.Count > 0)
            {
                GroupCollection groupCollection = mc[0].Groups;

                for (int i = 0; i < asciiKeyPattern.keys.Length && i + 1 < groupCollection.Count; i++)
                {

                    dic[asciiKeyPattern.keys[i]] = groupCollection[i + 1].Value;
                }
            }

            return dic;
        }


        /// <summary>
        /// 得到带key的表达式得到它的key和正则表达式
        /// </summary>
        /// <param name="asciiKeyMatche"></param>
        /// <returns>返回解析是否成功</returns>
        public static bool GetAsciiKeyPatternKeyAndPattern(AsciiKeyPattern asciiKeyPattern)
        {

            //得到key
            List<string> list = new List<string>();

            MatchCollection mc = Regex.Matches(asciiKeyPattern.keyPattern, @"\${(.*?)}");

            foreach (Match m in mc)
                list.Add(m.Groups[1].Value);

            asciiKeyPattern.keys = list.ToArray();

            //得到正则表达式
            asciiKeyPattern.pattern = Regex.Replace(asciiKeyPattern.keyPattern, @"\${(.*?)}", "(.*?)");

            return true;
        }
    }


    /// <summary>
    /// Ascii协议解析表达式
    /// </summary>
    public class AsciiKeyPattern
    {
        public AsciiKeyPattern(string keyPattern)
        {
            this.keyPattern = keyPattern;
        }

        public AsciiKeyPattern()
        {
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


        [XmlIgnore]
        private string _keyPattern = "";

        public string keyPattern
        {

            get => _keyPattern;
            set
            {
                _keyPattern = value;

                if (value != null)
                {
                    AsciiProtocolUtils.GetAsciiKeyPatternKeyAndPattern(this);
                }

            }
        }
        [XmlIgnore]
        public string[] _keys = new string[0];


        public string[] keys
        {

            get => _keys;
            set
            {
                _keys = value;
            }
        }

        [XmlIgnore]
        public string _pattern = "";

        public string pattern
        {

            get => _pattern;
            set
            {
                _pattern = value;
            }
        }

    }




}
