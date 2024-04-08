using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Device.Device.Device.DKey
{
    /// <summary>
    /// 数据键值
    /// </summary>
    public class DataKey
    {
        public string Key { get; set; }  = "";

        public string Name { get; set; } = "";

        public string Unit { get; set; } = "";

        public DataKey(string key) { 
            this.Key = key;
        }

        public DataKey(string key,string name,string unit)
        {
            this.Key = key;
            this.Name = name;
            this.Unit = unit;
        }
    }
}
