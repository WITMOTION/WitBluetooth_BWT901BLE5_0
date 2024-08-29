package com.wit.witsdk.sensor.modular.connector.modular.bluetooth;

import android.content.Context;

/**
 * 低功耗蓝牙连接类
 *
 * @author huangyajun
 * @date 2022/12/1 10:35
 */
public class BluetoothBLE extends InukerBluetoothBLE {

    /**
     * 构造
     *
     * @author huangyajun
     * @date 2022/12/1 11:05
     */
    public BluetoothBLE(Context context, String mac, String name) {
        super(context, mac, name);
    }
}
