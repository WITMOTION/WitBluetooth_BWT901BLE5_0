package com.wit.witsdk.sensor.modular.connector.entity;

import java.util.UUID;

/**
 * spp蓝牙连接选项
 *
 * @author huangyajun
 * @date 2022/4/28 15:23
 */
public class BluetoothSPPOption {
    private static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");

    /**
     * 蓝牙地址
     */
    private String mac;

    public String getMac() {
        return mac;
    }

    public void setMac(String mac) {
        this.mac = mac;
    }
}