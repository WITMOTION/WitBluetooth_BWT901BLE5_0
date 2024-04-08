using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Device.Device.Device.DKey
{
    public class DoubleKey : DataKey
    {
        public DoubleKey(string key) : base(key)
        {
        }

        public DoubleKey(string key, string name, string unit) : base(key, name, unit)
        {
        }
    }
}
