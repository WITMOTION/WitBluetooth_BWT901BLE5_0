using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wit.SDK.Modular.Sensor.Device;

namespace Wit.SDK.Modular.WitSensorApi.Interface
{

    /// <summary>
    /// 姿态传感器Api
    /// </summary>
    public interface IAttitudeSensorApi
    {
        /// <summary>
        /// 打开设备
        /// </summary>
        void Open();

        /// <summary>
        /// 是否打开了设备
        /// </summary>
        /// <returns></returns>
        bool IsOpen();

        /// <summary>
        /// 关闭设备
        /// </summary>
        void Close();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">需要发送出去的数据</param>
        /// <param name="returnData">传感器返回的数据</param>
        /// <param name="isWaitReturn">是否需要传感器返回数据</param>
        /// <param name="waitTime">等待传感器返回数据时间，单位ms，默认100ms</param>
        /// <param name="repetition">重复发送次数</param>
        void SendData(byte[] data, out byte[] returnData, bool isWaitReturn = false, int waitTime = 100, int repetition = 1);

        /// <summary>
        /// 发送带协议的数据，使用默认等待时长
        /// </summary>
        /// <param name="data">数据</param>
        void SendProtocolData(byte[] data);

        /// <summary>
        /// 发送带协议的数据,并且指定等待时长
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="waitTime">等待时间</param>
        void SendProtocolData(byte[] data, int waitTime);

        /// <summary>
        ///  解锁寄存器
        /// </summary>
        void UnlockReg();

        /// <summary>
        ///  保存寄存器
        /// </summary>
        void SaveReg();

        /// <summary>
        /// 加计校准
        /// </summary>
        void AppliedCalibration();

        /// <summary>
        /// 开始磁场校准
        /// </summary>
        void StartFieldCalibration();

        /// <summary>
        /// 结束磁场校准
        /// </summary>
        void EndFieldCalibration();

        /// <summary>
        /// 设置回传速率
        /// </summary>
        /// <param name="rate"></param>
        void SetReturnRate(byte rate);

        /// <summary>
        /// 获得设备名称
        /// </summary>
        /// <returns></returns>
        string GetDeviceName();

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key">数据键值</param>
        /// <returns></returns>
        string GetDeviceData(string key);

        /// <summary>
        /// 传感器数据更新时
        /// </summary>
        /// <param name="deviceModel"></param>
        void DeviceModel_OnListenKeyUpdate(DeviceModel deviceModel);

    }
}
