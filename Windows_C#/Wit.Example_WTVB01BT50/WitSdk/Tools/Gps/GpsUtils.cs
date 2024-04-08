using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// GPS工具类
/// </summary>
namespace Wit.SDK.Utils
{
    public class GpsUtils
    {
        /// <summary>
        /// 经纬度度分秒转换为度 选定保留小数
        /// </summary>
        public static double DmsToD(double value, int digits)
        {
            //度
            double Degree = Math.Floor(value / 100);
            //分
            double points = (value / 100 - Degree) * 100;
            Degree = Degree + points / 60;

            return Math.Round(Degree, digits);
        }

        /// <summary>
        /// 经纬度度分秒转换为度
        /// </summary>
        public static double DmsToD(double value)
        {
            return DmsToD(value,7);
        }

        /// <summary>
        ///经纬度 度转换为度分
        /// </summary>
        public static double DToDms(double value)
        {
            //度
            double Degree = Math.Floor(value);
            //分
            double points = (value - Degree) * 60.0;
            Degree = Degree * 100 + points;

            return Math.Round(Degree, 7);
        }

    }
}
