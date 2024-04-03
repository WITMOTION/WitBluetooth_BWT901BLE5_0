package com.wit.witsdk.sensor.modular.device.interfaces.impl;

import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.device.interfaces.IKeyUpdateObserver;
import com.wit.witsdk.sensor.modular.device.interfaces.IKeyUpdateObserverable;

import java.util.ArrayList;
import java.util.List;

public class KeyUpdateObserverServer implements IKeyUpdateObserverable {

    private List<IKeyUpdateObserver> list; //面向接口编程

    public KeyUpdateObserverServer() {
        list = new ArrayList<>();
    }

    @Override
    public void registerKeyUpdateObserver(IKeyUpdateObserver observer) {
        list.add(observer);
    }

    @Override
    public void removeKeyUpdateObserver(IKeyUpdateObserver observer) {
        if (!list.isEmpty()) {
            list.remove(observer);
        }
    }

    @Override
    public void notifyKeyUpdateObserver(DeviceModel deviceModel, String key, Object value) {
        for (int i = 0; i < list.size(); i++) {
            IKeyUpdateObserver observer = list.get(i);
            observer.update(deviceModel, key, value);//通知Observer调用update方法
        }
    }
}
