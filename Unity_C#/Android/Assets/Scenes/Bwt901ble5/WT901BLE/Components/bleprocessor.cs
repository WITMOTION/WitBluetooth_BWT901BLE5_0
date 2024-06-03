using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Wit.SDK.Modular.Sensor.Device;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Interface;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Utils;

public class WitBleProcessor : IDataProcessor
{
    /// <summary>
    /// 设备模型
    /// Device Model
    /// </summary>
    public DeviceModel DeviceModel { get; private set; }

    /// <summary>
    /// 设备打开时
    /// When the device is turned on
    /// </summary>
    /// <param name="deviceModel"></param>
    public override void OnOpen(DeviceModel deviceModel)
    {
        this.DeviceModel = deviceModel;
    }

    /// <summary>
    /// 设备关闭时
    /// When the device is turned off
    /// </summary>
    public override void OnClose(){ }

    /// <summary>
    /// 更新
    /// When updating data
    /// </summary>
    /// <param name="baseDeviceModel"></param>
    public override void OnUpdate(DeviceModel baseDeviceModel)
    {
        // 解算寄存器 Solving register
        ParseRegData();
    }

    /// <summary>
    /// 解算寄存器
    /// Solving register
    /// </summary>
    private void ParseRegData()
    {
        // 加速度 Acceleration
        var regAx = DeviceModel.GetDeviceData("61_0");
        var regAy = DeviceModel.GetDeviceData("61_1");
        var regAz = DeviceModel.GetDeviceData("61_2");
        // 角速度 Angular velocity
        var regWx = DeviceModel.GetDeviceData("61_3");
        var regWy = DeviceModel.GetDeviceData("61_4");
        var regWz = DeviceModel.GetDeviceData("61_5");
        // 角度 Angle
        var regAngleX = DeviceModel.GetDeviceData("61_6");
        var regAngleY = DeviceModel.GetDeviceData("61_7");
        var regAngleZ = DeviceModel.GetDeviceData("61_8");

        // 加速度解算 Acceleration
        if (!string.IsNullOrEmpty(regAx))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AccX, Math.Round(double.Parse(regAx) / 32768 * 16, 3).ToString());
        }
        if (!string.IsNullOrEmpty(regAy))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AccY, Math.Round(double.Parse(regAy) / 32768 * 16, 3).ToString());
        }
        if (!string.IsNullOrEmpty(regAz))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AccZ, Math.Round(double.Parse(regAz) / 32768 * 16, 3).ToString());
        }

        // 角速度解算 Angular velocity
        if (!string.IsNullOrEmpty(regWx))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AsX, Math.Round(double.Parse(regWx) / 32768 * 2000, 3).ToString());
        }
        if (!string.IsNullOrEmpty(regWy))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AsY, Math.Round(double.Parse(regWy) / 32768 * 2000, 3).ToString());
        }
        if (!string.IsNullOrEmpty(regWz))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AsZ, Math.Round(double.Parse(regWz) / 32768 * 2000, 3).ToString());
        }

        // 角度 Angle
        if (!string.IsNullOrEmpty(regAngleX))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AngleX, Math.Round(double.Parse(regAngleX) / 32768 * 180, 2).ToString());
        }
        if (!string.IsNullOrEmpty(regAngleY))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AngleY, Math.Round(double.Parse(regAngleY) / 32768 * 180, 2).ToString());
        }
        if (!string.IsNullOrEmpty(regAngleZ))
        {
            DeviceModel.SetDeviceData(WitSensorKey.AngleZ, Math.Round(double.Parse(regAngleZ) / 32768 * 180, 2).ToString());
        }
    }
}
