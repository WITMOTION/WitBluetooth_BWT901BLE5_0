package com.wit.witsdk.sensor.modular.device.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;

/**
 * 观察者
 *
 * @author huangyajun
 * @date 2022/4/26 11:27
 */
public interface IListenKeyUpdateObserver {

    /**
     * 接收通知
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void update(DeviceModel deviceModel);

}
