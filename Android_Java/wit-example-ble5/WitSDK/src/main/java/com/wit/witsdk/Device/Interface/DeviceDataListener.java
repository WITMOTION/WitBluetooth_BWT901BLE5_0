package com.wit.witsdk.Device.Interface;

/**
 * 设备事件监听者
 * */
public interface DeviceDataListener {
    // 收到数据
    void OnReceive(String deviceName, String displayData);

    // 连接状态改变
    void OnStatusChange(String deviceName, boolean status);
}
