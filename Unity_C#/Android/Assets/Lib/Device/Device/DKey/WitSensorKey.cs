using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant
{
    /// <summary>
    /// 倾角传感器标准key常量
    /// </summary>
    public static class WitSensorKey
    {
        // 芯片时间
        public static string ChipTime { get; } = "ChipTime";

        // 加速度X
        public static string AccX { get; } = "AccX";

        // 加速度Y
        public static string AccY { get; } = "AccY";

        // 加速度Z
        public static string AccZ { get; } = "AccZ";

        // 加速度矢量和
        public static string AccM { get; } = "AccM";

        // 角速度
        public static string AsX { get; } = "AsX";

        // 角速度
        public static string AsY { get; } = "AsY";

        // 角速度
        public static string AsZ { get; } = "AsZ";

        // 角速度Z矢量和
        public static string AsM { get; } = "AsM";

        // 角度X
        public static string AngleX { get; } = "AngleX";

        // 角度Y
        public static string AngleY { get; } = "AngleY";

        // 角度Z
        public static string AngleZ { get; } = "AngleZ";

        // 磁场X
        public static string HX { get; } = "HX";

        // 磁场Y
        public static string HY { get; } = "HY";

        // 磁场Z
        public static string HZ { get; } = "HZ";

        // 磁场矢量和
        public static string HM { get; } = "HM";

        // 温度
        public static string T { get; } = "T";

        // 扩展端口 1
        public static string D0 { get; } = "D0";

        // 扩展端口 2
        public static string D1 { get; } = "D1";

        // 扩展端口 3
        public static string D2 { get; } = "D2";

        // 扩展端口 4
        public static string D3 { get; } = "D3";

        // 气压
        public static string P { get; } = "P";

        // 高度
        public static string H { get; } = "H";

        // 经度
        public static string Lon { get; } = "Lon";

        // 经度度表现形式
        public static string LonDeg { get; } = "LonDeg";

        // 纬度
        public static string Lat { get; } = "Lat";

        // 纬度度表现形式
        public static string LatDeg { get; } = "LatDeg";

        /// <summary>
        /// GPS状态
        /// </summary>
        public static string GPSStatus { get; } = "GPSStatus";


        // GPS高度
        public static string GPSHeight { get; } = "GPSHeight";

        // GPS航向
        public static string GPSYaw { get; } = "GPSYaw";

        // GPS地速
        public static string GPSV { get; } = "GPSV";

        // 四元数0
        public static string Q0 { get; } = "Q0";

        // 四元数1
        public static string Q1 { get; } = "Q1";

        // 四元数2
        public static string Q2 { get; } = "Q2";

        // 四元数3
        public static string Q3 { get; } = "Q3";

        // 卫星数量
        public static string SN { get; } = "SN";

        // 位置定位精度
        public static string PDOP { get; } = "PDOP";

        // 水平定位精度
        public static string HDOP { get; } = "HDOP";

        // 垂直定位精度
        public static string VDOP { get; } = "VDOP";

        // 版本号
        public static string VersionNumber { get; } = "VersionNumber";

        // 序列号
        public static string SerialNumber { get; } = "SerialNumber";


    }
}
