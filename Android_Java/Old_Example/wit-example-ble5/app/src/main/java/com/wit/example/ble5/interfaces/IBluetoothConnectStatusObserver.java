package com.wit.example.ble5.interfaces;

// 状态观察者 Status Observer
public interface IBluetoothConnectStatusObserver {
    // 状态改变时 When the state changes
    void onStatusChanged(String mac, int status);
}
