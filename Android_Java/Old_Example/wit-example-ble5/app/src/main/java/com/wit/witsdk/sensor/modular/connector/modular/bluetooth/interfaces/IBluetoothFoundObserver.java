package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;

/**
 * 蓝牙状态改变观察者
 *
 * @author huangyajun
 * @date 2022/4/26 11:27
 */
public interface IBluetoothFoundObserver {

    /**
     * 当搜到BLE设备时
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void onFoundBle(BluetoothBLE bluetoothBLE);

    /**
     * 找到经典蓝牙时
     *
     * @author huangyajun
     * @date 2022/5/5 20:27
     */
    void onFoundSPP(BluetoothSPP bluetoothSPP);

    /**
     * 找到双模蓝牙时
     *
     * @author huangyajun
     * @date 2022/5/5 20:27
     */
    void onFoundDual(BluetoothBLE bluetoothBLE);
}
