package com.wit.witsdk.sensor.modular.device;

import android.os.Build;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

/**
 * 设备模型管理器
 *
 * @author huangyajun
 * @date 2022/5/9 20:25
 */
public class DeviceModelManager {

    private static DeviceModelManager instance;

    public static DeviceModelManager getInstance() {
        if (instance == null) {
            instance = new DeviceModelManager();
        }
        return instance;
    }

    /**
     * 设备模型列表
     *
     * @author huangyajun
     * @date 2022/5/9 20:34
     */
    private Map<String, DeviceModel> deviceModelMap = new HashMap<>();

    /**
     * 获得设备模型
     *
     * @author huangyajun
     * @date 2022/5/9 20:26
     */
    public synchronized DeviceModel getDeviceModel(String name) {
        return deviceModelMap.get(name);
    }

    /**
     * 保存设备模型
     *
     * @author huangyajun
     * @date 2022/5/9 20:26
     */
    public synchronized void putDeviceModel(String name, DeviceModel deviceModel) {
        if (deviceModelMap.containsKey(name)) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                deviceModelMap.replace(name, deviceModel);
            } else {
                deviceModelMap.put(name, deviceModel);
            }
        } else {
            deviceModelMap.put(name, deviceModel);
        }
    }

    /**
     * 清除所有设备模型
     *
     * @author huangyajun
     * @date 2022/5/9 20:26
     */
    public synchronized void clearAllDeviceModel() {
        Collection<DeviceModel> values = deviceModelMap.values();
        Iterator<DeviceModel> iterator = values.iterator();
        while (iterator.hasNext()) {
            DeviceModel next = iterator.next();
            next.closeDevice();
        }
        deviceModelMap.clear();
    }

    /**
     * 清除某设备模型
     *
     * @author huangyajun
     * @date 2022/5/9 20:26
     */
    public synchronized void clearDeviceModel(String name) {
        DeviceModel deviceModel = getDeviceModel(name);
        if (deviceModel != null) {
            deviceModel.closeDevice();
            deviceModelMap.remove(name);
        }
    }

    /**
     * 获得所有设备
     *
     * @author huangyajun
     * @date 2022/5/9 20:33
     */
    public synchronized List<DeviceModel> getAllByList() {
        ArrayList<DeviceModel> arrayList = new ArrayList<>();
        Collection<DeviceModel> values = deviceModelMap.values();
        arrayList.addAll(values);
        return arrayList;
    }
}
