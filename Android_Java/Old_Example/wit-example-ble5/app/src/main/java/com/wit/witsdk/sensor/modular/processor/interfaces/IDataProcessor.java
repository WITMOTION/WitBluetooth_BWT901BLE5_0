package com.wit.witsdk.sensor.modular.processor.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;

/**
 * 数据处理器接口
 *
 * @author huangyajun
 * @date 2022/4/21 11:22
 */
public interface IDataProcessor {

    /**
     * 打开设备时
     *
     * @author huangyajun
     * @date 2022/4/21 11:24
     */
    void OnOpen(DeviceModel deviceModel);

    /**
     * 关闭传感器时
     *
     * @author huangyajun
     * @date 2022/4/21 11:24
     */
    void OnClose();

    /**
     * 设备实时数据更新时
     *
     * @author huangyajun
     * @date 2022/4/21 11:24
     */
    void OnUpdate(DeviceModel deviceModel);

}
