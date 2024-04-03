package com.wit.witsdk.sensor.modular.resolver.entity;

/**
 * @author Zhangpingxiang
 * @Date: 2023/2/25 0025
 */
public class SendDataResult {

    private boolean success = false;

    public SendDataResult(boolean success) {
        this.success = success;
    }

    public boolean isSuccess() {
        return success;
    }
}
