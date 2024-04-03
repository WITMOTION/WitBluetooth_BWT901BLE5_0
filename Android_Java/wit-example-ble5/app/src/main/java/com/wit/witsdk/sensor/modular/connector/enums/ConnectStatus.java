package com.wit.witsdk.sensor.modular.connector.enums;

/**
 * 连接状态枚举
 *
 * @author huangyajun
 * @date 2022/4/21 10:27
 */
public enum ConnectStatus {

    /**
     * 打开的
     */
    Opened(0),

    /**
     * 关闭的
     */
    Closed(1);

    private final int code;

    ConnectStatus(int code) {
        this.code = code;
    }

    public int getCode() {
        return code;
    }
}
