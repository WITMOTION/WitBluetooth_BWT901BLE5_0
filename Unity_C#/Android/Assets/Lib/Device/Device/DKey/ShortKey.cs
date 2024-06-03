using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Device.Device.Device.DKey
{
    public class ShortKey : DataKey
    {
        public ShortKey(string key) : base(key)
        {
        }

        public ShortKey(string key, string name, string unit) : base(key, name, unit)
        {
        }
    }
}
