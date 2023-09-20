package com.wit.example.ble5.data;

import com.wit.witsdk.sensor.dkey.DoubleKey;
import com.wit.witsdk.sensor.dkey.StringKey;

/**
 * 传感器标准key常量
 *
 * @Author haungyajun
 * @Date 2022/4/26 15:50 （可以根据需要修改）
 */
public class WitSensorKey {

    public static final StringKey ChipTime = new StringKey("ChipTime", "片上时间", "");
    public static final DoubleKey AccX = new DoubleKey("AccX", "加速度X", "g");
    public static final DoubleKey AccY = new DoubleKey("AccY", "加速度Y", "g");
    public static final DoubleKey AccZ = new DoubleKey("AccZ", "加速度Z", "g");
    public static final DoubleKey AsX = new DoubleKey("AsX", "加速度X", "°/s");
    public static final DoubleKey AsY = new DoubleKey("AsY", "加速度Y", "°/s");
    public static final DoubleKey AsZ = new DoubleKey("AsZ", "加速度Z", "°/s");
    public static final DoubleKey AngleX = new DoubleKey("AngleX", "加速度Z", "°");
    public static final DoubleKey AngleY = new DoubleKey("AngleY", "加速度Z", "°");
    public static final DoubleKey AngleZ = new DoubleKey("AngleZ", "加速度Z", "°");
    public static final DoubleKey HX = new DoubleKey("HX", "磁场X", "μT");
    public static final DoubleKey HY = new DoubleKey("HY", "磁场Y", "μT");
    public static final DoubleKey HZ = new DoubleKey("HZ", "磁场Z", "μT");
    public static final DoubleKey T = new DoubleKey("T", "温度", "℃");

    public static final DoubleKey Q0 = new DoubleKey("Q0", "四元数0", "");
    public static final DoubleKey Q1 = new DoubleKey("Q1", "四元数1", "");
    public static final DoubleKey Q2 = new DoubleKey("Q2", "四元数2", "");
    public static final DoubleKey Q3 = new DoubleKey("Q3", "四元数3", "");

    public static final DoubleKey Rssi = new DoubleKey("Rssi", "信号", "");
    public static final DoubleKey ElectricQuantityPercentage = new DoubleKey("ElectricQuantityPercentage", "电量", "%");
    public static final DoubleKey ElectricQuantity = new DoubleKey("ElectricQuantity", "电量(电压)", "V");
    public static final StringKey VersionNumber = new StringKey("VersionNumber", "版本号", "");

}
