package com.wit.witsdk.sensor.modular.resolver.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;

/**
 * 协议解析器接口
 *
 * @author huangyajun
 * @date 2022/4/21 11:25
 */
public interface IProtocolResolver {

    /**
     * 发送数据
     *
     * @author huangyajun
     * @date 2022/4/21 11:26
     */
    void sendData(byte[] sendData, DeviceModel deviceModel, int waitTime, ISendDataCallback callback);

    /**
     * 发送数据
     *
     * @author huangyajun
     * @date 2022/4/21 11:26
     */
    void sendData(byte[] sendData, DeviceModel deviceModel);

    /**
     * 解算被动接收的数据
     *
     * @author huangyajun
     * @date 2022/4/21 11:26
     */
    void passiveReceiveData(byte[] data, DeviceModel deviceModel);

}
