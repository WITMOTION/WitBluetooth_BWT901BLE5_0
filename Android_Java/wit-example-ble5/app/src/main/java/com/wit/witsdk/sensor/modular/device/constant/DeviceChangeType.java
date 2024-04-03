package com.wit.witsdk.sensor.modular.device.constant;

import java.io.Serializable;

/**
 * 设备状态变更常量
 *
 * @author huangyajun
 * @date 2022/6/23 14:21
 */
public enum DeviceChangeType implements Serializable {

    /**
     * 连接中
     */
    CONNECTING("0"),

    /**
     * 断开的
     */
    DISCONNECTED("1"),

    /**
     * 连接的
     */
    CONNECTED("2"),

    /**
     * 连接失败
     */
    CONNECTION_FAIL("3"),

    /**
     * 发现设备
     */
    FOUND_DEVICE("4");

    private String code;

    DeviceChangeType(String code) {
        this.code = code;
    }

    /**
     * 获得code
     *
     * @author huangyajun
     * @date 2022/6/23 14:30
     */
    public String getCode() {
        return code;
    }

}
