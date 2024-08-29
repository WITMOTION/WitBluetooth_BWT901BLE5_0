package com.wit.witsdk.sensor.modular.device.interfaces;

/**
 * 发送数据的返回
 *
 * @author huangyajun
 * @date 2022/4/26 13:48
 */
@FunctionalInterface
public interface IDeviceSendCallback {
    void callback(Byte[] data);
}