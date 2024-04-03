package com.wit.witsdk.sensor.modular.connector.enums;

/**
 * 连接类型枚举
 *
 * @author huangyajun
 * @date 2022/4/21 10:20
 */
public enum ConnectType {

    /**
     * 串口
     */
    SerialPort(0),

    /**
     * TCP服务器
     */
    TCPServer(1),

    /**
     * TCP客户端
     */
    TCPClient(2),

    /**
     * UDP通信
     */
    UDP(3),

    /**
     * BLE蓝牙连接
     */
    BluetoothBLE(4),

    /**
     * HId
     */
//    HID(5),


    /**
     * CH340USB
     */
    CH340USB(6),

    /**
     * SPP蓝牙连接
     */
    BluetoothSPP(7);

    public final int code;

    ConnectType(int code) {
        this.code = code;
    }

    public int getCode() {
        return code;
    }
}
