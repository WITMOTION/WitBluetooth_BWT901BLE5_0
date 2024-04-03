package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;

/**
 * 蓝牙状态改变观察者
 *
 * @author huangyajun
 * @date 2022/4/26 11:27
 */
public interface IUpdateRssiObserver {
    /**
     * 更新蓝牙信号时
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void onUpdateRssi(String mac, int rssi);

}
