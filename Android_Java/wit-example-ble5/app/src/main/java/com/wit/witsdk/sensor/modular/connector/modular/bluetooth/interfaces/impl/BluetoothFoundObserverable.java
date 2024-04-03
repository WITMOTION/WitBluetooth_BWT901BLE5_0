package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.impl;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserverable;

import java.util.ArrayList;
import java.util.List;

/**
 * 蓝牙状态改变被观察者
 *
 * @Author haungyajun
 * @Date 2022/5/5 17:58 （可以根据需要修改）
 */
public class BluetoothFoundObserverable implements IBluetoothFoundObserverable {

    private List<IBluetoothFoundObserver> list;

    public BluetoothFoundObserverable() {
        list = new ArrayList<>();
    }

    @Override
    public void registerObserver(IBluetoothFoundObserver o) {
        list.add(o);
    }

    @Override
    public void removeObserver(IBluetoothFoundObserver o) {
        if (!list.isEmpty()) {
            list.remove(o);
        }
    }

    @Override
    public void foundBLE(BluetoothBLE bluetoothBLE) {
        for (int i = 0; i < list.size(); i++) {
            IBluetoothFoundObserver observer = list.get(i);
            observer.onFoundBle(bluetoothBLE);//通知Observer调用update方法
        }
    }

    @Override
    public void foundSPP(BluetoothSPP bluetoothSPP) {
        for (int i = 0; i < list.size(); i++) {
            IBluetoothFoundObserver observer = list.get(i);
            observer.onFoundSPP(bluetoothSPP);//通知Observer调用update方法
        }
    }

    @Override
    public void foundDual(BluetoothBLE bluetoothBLE) {
        for (int i = 0; i < list.size(); i++) {
            IBluetoothFoundObserver observer = list.get(i);
            observer.onFoundDual(bluetoothBLE);//通知Observer调用update方法
        }
    }
}
