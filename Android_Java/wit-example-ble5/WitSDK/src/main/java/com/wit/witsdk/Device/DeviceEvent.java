package com.wit.witsdk.Device;

import android.bluetooth.BluetoothDevice;

import com.wit.witsdk.Device.Interface.DeviceDataListener;
import com.wit.witsdk.Device.Interface.DeviceFindListener;

import java.util.ArrayList;
import java.util.List;

/**
 * 设备事件
 * */
public class DeviceEvent {
    // 搜索设备监听者列表
    private List<DeviceFindListener> findListeners = new ArrayList<>();

    // 设备事件监听者列表
    private List<DeviceDataListener> deviceListeners = new ArrayList<>();

    /**
     * 添加设备搜索订阅者
     * */
    public void AddDeviceFindListener(DeviceFindListener listener) {
        findListeners.add(listener);
    }

    /**
     * 移除设备搜索订阅者
     * */
    public void RemoveDeviceFindListener(DeviceFindListener listener) {
        findListeners.remove(listener);
    }

    /**
     * 找到设备回调
     * */
    public void FindDevice(BluetoothDevice device) {
        // 触发所有订阅了此事件的方法
        for (DeviceFindListener listener : findListeners) {
            listener.onDeviceFound(device);
        }
    }

    /**
     * 添加设备事件订阅者
     * */
    public void AddDeviceListener(DeviceDataListener listener) {
        deviceListeners.add(listener);
    }

    /**
     * 移除设备事件订阅者
     * */
    public void RemoveDeviceListener(DeviceDataListener listener) {
        deviceListeners.remove(listener);
    }

    /**
     * 回调数据事件
     * */
    public void OnReceiveDevice(String deviceName, String displayData) {
        // 触发所有订阅了此事件的方法
        for (DeviceDataListener listener : deviceListeners) {
            listener.OnReceive(deviceName, displayData);
        }
    }

    /**
     * 连接状态改变事件
     * */
    public void OnStatusChange(String deviceName, boolean status){
        for (DeviceDataListener listener : deviceListeners) {
            listener.OnStatusChange(deviceName, status);
        }
    }
}
