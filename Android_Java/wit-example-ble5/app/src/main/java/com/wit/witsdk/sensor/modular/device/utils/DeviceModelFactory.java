package com.wit.witsdk.sensor.modular.device.utils;

import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.device.entity.DeviceOption;
import com.wit.witsdk.sensor.modular.processor.interfaces.IDataProcessor;
import com.wit.witsdk.sensor.modular.resolver.interfaces.IProtocolResolver;

/**
 * 设备模型工厂
 *
 * @author huangyajun
 * @date 2023/2/2 13:34
 */
public class DeviceModelFactory {

    /**
     * 创建设备模型
     *
     * @author huangyajun
     * @date 2023/2/2 13:35
     */
    public static DeviceModel createDevice(String deviceName, DeviceOption deviceOption) {
        Class<? extends IDataProcessor> dataProcessorClass = deviceOption.getDataProcessorClass();
        Class<? extends IProtocolResolver> protocolResolverClass = deviceOption.getProtocolResolverClass();

        IDataProcessor iDataProcessor = null;
        IProtocolResolver iProtocolResolver = null;
        String listenerKey = deviceOption.getListenerKey();

        try {
            iDataProcessor = dataProcessorClass.newInstance();
            iProtocolResolver = protocolResolverClass.newInstance();

        } catch (Exception e) {
            e.printStackTrace();
        }

        DeviceModel deviceModel = new DeviceModel(deviceName,
                iProtocolResolver,
                iDataProcessor,
                listenerKey);
        return deviceModel;
    }
}
