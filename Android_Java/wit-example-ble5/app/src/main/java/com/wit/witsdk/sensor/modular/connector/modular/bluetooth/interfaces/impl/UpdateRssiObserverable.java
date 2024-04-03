package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.impl;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserverable;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IUpdateRssiObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IUpdateRssiObserverable;

import java.util.ArrayList;
import java.util.List;

/**
 * 蓝牙状态改变被观察者
 *
 * @Author haungyajun
 * @Date 2022/5/5 17:58 （可以根据需要修改）
 */
public class UpdateRssiObserverable implements IUpdateRssiObserverable {

    private List<IUpdateRssiObserver> list;

    public UpdateRssiObserverable() {
        list = new ArrayList<>();
    }

    @Override
    public void registerObserver(IUpdateRssiObserver o) {
        list.add(o);
    }

    @Override
    public void removeObserver(IUpdateRssiObserver o) {
        if (!list.isEmpty()) {
            list.remove(o);
        }
    }

    @Override
    public void onUpdateRssi(String mac, int rssi) {
        for (int i = 0; i < list.size(); i++) {
            IUpdateRssiObserver observer = list.get(i);
            observer.onUpdateRssi(mac, rssi);
        }
    }
}
