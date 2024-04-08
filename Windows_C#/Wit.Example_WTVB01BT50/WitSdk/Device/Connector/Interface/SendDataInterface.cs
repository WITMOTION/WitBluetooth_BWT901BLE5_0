using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Modular.Connector.Interface
{
    /// <summary>
    /// 发送数据接口
    /// </summary>
    public interface SendDataInterface
    {
        void OnSendData(byte[] data);

    }
}
