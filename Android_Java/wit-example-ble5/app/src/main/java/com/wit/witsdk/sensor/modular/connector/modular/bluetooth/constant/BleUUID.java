package com.wit.witsdk.sensor.modular.connector.modular.bluetooth.constant;

import java.util.UUID;

/**
 * 低功耗蓝牙uuid
 *
 * @author huangyajun
 * @date 2023/2/28 15:58
 */
public class BleUUID {

    // 发送uuid
    public static final UUID UUID_SEND = UUID.fromString("0000ffe9-0000-1000-8000-00805f9a34fb");

    // 读取uuid
    public static final UUID UUID_READ = UUID.fromString("0000ffe4-0000-1000-8000-00805f9a34fb");

    // 服务uuid
    public static final UUID UUID_SERVICE = UUID.fromString("0000ffe5-0000-1000-8000-00805f9a34fb");

}
