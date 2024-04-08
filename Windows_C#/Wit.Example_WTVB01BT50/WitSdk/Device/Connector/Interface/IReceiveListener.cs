using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Interface
{
    /// <summary>
    /// 接收数据监听者
    /// </summary>
    public interface DataReceivedInterface
    {
        void OnDataReceived(byte[] data);
        
    }
}
