package com.wit.witsdk.sensor.modular.device.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;

/**
 * 观察对象
 *
 * @author huangyajun
 * @date 2022/4/26 11:28
 */
public interface IListenKeyUpdateObserverable {

    /**
     * 添加观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void registerListenKeyUpdateObserver(IListenKeyUpdateObserver o);

    /**
     * 删除观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void removeListenKeyUpdateObserver(IListenKeyUpdateObserver o);

    /**
     * 通知观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void notifyListenKeyUpdateObserver(DeviceModel deviceModel);
}
