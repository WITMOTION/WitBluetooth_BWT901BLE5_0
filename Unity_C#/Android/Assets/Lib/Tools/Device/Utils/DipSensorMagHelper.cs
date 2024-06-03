using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wit.SDK.Modular.Sensor.Utils
{

    /// <summary>
    /// 倾角传感器磁场帮助类
    /// </summary>
    public class DipSensorMagHelper
    {

        /// <summary>
        /// 磁场转换标准单位uT(微特)
        /// </summary>
        public static double GetMagToUt(short reg72, double regMag)
        {
            double dRet = regMag;
            switch (reg72)
            {
                case 2:
                    dRet = dRet * 0.15;
                    break;
                case 3:
                    dRet = dRet * 13 / 1000.0;
                    break;
                case 4:
                    dRet = dRet * 0.058;
                    break;
                case 5:
                    dRet = dRet * 0.098;
                    break;
                case 6:
                    dRet = dRet / 120;
                    break;
                case 7:
                    dRet = dRet * 20 / 1000.0;
                    break;
            }
            return Math.Round(dRet, 3);
        }

        /// <summary>
        /// 标准单位uT(微特)转换磁场数据
        /// </summary>
        public static double GetUtToMag(short reg72, double reguTMag)
        {
            double dRet = reguTMag;
            switch (reg72)
            {
                case 2:
                    dRet = dRet / 0.15;
                    break;
                case 3:
                    dRet = dRet * 1000.0  /  13.0 ;
                    break;
                case 4:
                    dRet = dRet / 0.058;
                    break;
                case 5:
                    dRet = dRet / 0.098;
                    break;
                case 6:
                    dRet = dRet * 120;
                    break;
                case 7:
                    dRet = dRet * 1000.0 / 20.0 ;
                    break;
            }
            return dRet;
        }

    }
}
