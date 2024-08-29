package com.wit.witsdk.Device;

import java.util.Objects;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

/**
 * 设备模型管理器
 * */
public class DeviceManager extends DeviceEvent{
    // 设备模型列表
    ConcurrentMap<String, DeviceModel> deviceMap = new ConcurrentHashMap<String, DeviceModel>();

    // 设备模型管理器实例
    private static DeviceManager instance;

    // 私有构造
    private DeviceManager() {}

    /**
     * 获得设备模型管理器实例
     * */
    public static synchronized DeviceManager getInstance() {
        if (instance == null) {
            instance = new DeviceManager();
        }
        return instance;
    }

    /**
     * 添加设备
     * */
    public void AddDevice(String key, DeviceModel deviceModel){
        deviceMap.put(key, deviceModel);
    }

    /**
     * 移除设备
     * */
    public void RemoveDevice(String key){
        deviceMap.remove(key);
    }

    /**
     * 关闭所有连接并移除设备
     * */
    public void CleanAllDevice(){
        for (String key : deviceMap.keySet()) {
            Objects.requireNonNull(deviceMap.get(key)).CloseDevice();
        }
        deviceMap.clear();
    }

    /**
     * 获得设备
     * */
    public DeviceModel GetDevice(String key){
        return deviceMap.getOrDefault(key, null);
    }
}
