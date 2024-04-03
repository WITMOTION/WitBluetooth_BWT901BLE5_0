package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;

/**
 * 蓝牙状态改变被观察者接口
 *
 * @author huangyajun
 * @date 2022/4/26 11:28
 */
public interface IUpdateRssiObserverable {

    /**
     * 添加观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void registerObserver(IUpdateRssiObserver o);

    /**
     * 删除观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void removeObserver(IUpdateRssiObserver o);

    /**
     * 更新蓝牙信号时
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void onUpdateRssi(String mac, int rssi);

}
