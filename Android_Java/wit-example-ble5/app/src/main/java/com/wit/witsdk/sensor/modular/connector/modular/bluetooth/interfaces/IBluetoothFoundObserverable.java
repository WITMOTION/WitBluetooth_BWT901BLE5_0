package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;

/**
 * 蓝牙状态改变被观察者接口
 *
 * @author huangyajun
 * @date 2022/4/26 11:28
 */
public interface IBluetoothFoundObserverable {

    /**
     * 添加观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void registerObserver(IBluetoothFoundObserver o);

    /**
     * 删除观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void removeObserver(IBluetoothFoundObserver o);

    /**
     * 找到低功耗设备时
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void foundBLE(BluetoothBLE bluetoothBLE);

    /**
     * 找到低功耗蓝牙时
     *
     * @author huangyajun
     * @date 2022/5/5 20:28
     */
    void foundSPP(BluetoothSPP bluetoothSPP);

    /**
     * 找到双模蓝牙
     *
     * @author huangyajun
     * @date 2022/5/5 20:28
     */
    void foundDual(BluetoothBLE bluetoothBLE);
}
