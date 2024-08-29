package com.wit.witsdk.sensor.modular.device.interfaces.impl;

import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.device.interfaces.IListenKeyUpdateObserver;
import com.wit.witsdk.sensor.modular.device.interfaces.IListenKeyUpdateObserverable;

import java.util.ArrayList;
import java.util.List;

public class ListenKeyUpdateObserverServer implements IListenKeyUpdateObserverable {

    private List<IListenKeyUpdateObserver> list;

    private String msg;

    public ListenKeyUpdateObserverServer() {
        list = new ArrayList<>();
    }

    @Override
    public void registerListenKeyUpdateObserver(IListenKeyUpdateObserver observer) {
        list.add(observer);
    }

    @Override
    public void removeListenKeyUpdateObserver(IListenKeyUpdateObserver observer) {
        if (!list.isEmpty()) {
            list.remove(observer);
        }
    }

    @Override
    public void notifyListenKeyUpdateObserver(DeviceModel deviceModel){
        for (int i = 0; i < list.size(); i++) {
            IListenKeyUpdateObserver observer = list.get(i);
            //通知Observer调用update方法
            observer.update(deviceModel);
        }
    }

}
