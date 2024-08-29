package com.wit.witsdk.sensor.modular.connector.entity;

import java.util.UUID;

/**
 * 低功耗蓝牙连接选项
 */
public class BluetoothBLEOption {

    /**
     * 蓝牙地址
     */
    private String mac;

    /**
     * 发送uuid
     */
    private UUID sendUUID = UUID.fromString("49535343-8841-43f4-a8d4-ecbe34729bb3");

    /**
     * 读uuid
     */
    private UUID readUUID = UUID.fromString("49535343-1e4d-4bd9-ba61-23c647249616");

    /**
     * 服务uuid
     */
    private UUID serviceUUID = UUID.fromString("49535343-fe7d-4ae5-8fa9-9fafd205e455");

    public UUID getSendUUID() {
        return sendUUID;
    }

    public void setSendUUID(UUID sendUUID) {
        this.sendUUID = sendUUID;
    }

    public UUID getReadUUID() {
        return readUUID;
    }

    public void setReadUUID(UUID readUUID) {
        this.readUUID = readUUID;
    }

    public UUID getServiceUUID() {
        return serviceUUID;
    }

    public void setServiceUUID(UUID serviceUUID) {
        this.serviceUUID = serviceUUID;
    }

    public String getMac() {
        return mac;
    }

    public void setMac(String mac) {
        this.mac = mac;
    }
}