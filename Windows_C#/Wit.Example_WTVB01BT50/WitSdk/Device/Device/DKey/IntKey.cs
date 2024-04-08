using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Device.Device.Device.DKey
{
    public class IntKey : DataKey
    {
        public IntKey(string key) : base(key)
        {
        }

        public IntKey(string key, string name, string unit) : base(key, name, unit)
        {
        }
    }
}
