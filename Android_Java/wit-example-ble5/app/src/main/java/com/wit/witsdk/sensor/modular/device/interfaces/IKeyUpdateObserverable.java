package com.wit.witsdk.sensor.modular.device.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;

/**
 * 观察对象
 *
 * @author huangyajun
 * @date 2022/4/26 11:28
 */
public interface IKeyUpdateObserverable {

    /**
     * 添加观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void registerKeyUpdateObserver(IKeyUpdateObserver o);

    /**
     * 删除观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void removeKeyUpdateObserver(IKeyUpdateObserver o);

    /**
     * 通知观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void notifyKeyUpdateObserver(DeviceModel deviceModel, String key, Object value);
}
