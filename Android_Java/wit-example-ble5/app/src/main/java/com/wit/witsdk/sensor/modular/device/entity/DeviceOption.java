package com.wit.witsdk.sensor.modular.device.entity;

import com.wit.witsdk.sensor.modular.processor.interfaces.IDataProcessor;
import com.wit.witsdk.sensor.modular.resolver.interfaces.IProtocolResolver;

/**
 * 设备配置
 *
 * @author huangyajun
 * @date 2023/2/1 15:25
 */
public class DeviceOption {

    /**
     * 协议处理器
     */
    private Class<? extends IProtocolResolver> protocolResolverClass;

    /**
     * 数据解析器
     */
    private Class<? extends IDataProcessor> dataProcessorClass;

    /**
     * 数据刷新键
     */
    private String listenerKey;

    public Class<? extends IProtocolResolver> getProtocolResolverClass() {
        return protocolResolverClass;
    }

    public void setProtocolResolverClass(Class<? extends IProtocolResolver> protocolResolverClass) {
        this.protocolResolverClass = protocolResolverClass;
    }

    public Class<? extends IDataProcessor> getDataProcessorClass() {
        return dataProcessorClass;
    }

    public void setDataProcessorClass(Class<? extends IDataProcessor> dataProcessorClass) {
        this.dataProcessorClass = dataProcessorClass;
    }

    public String getListenerKey() {
        return listenerKey;
    }

    public void setListenerKey(String listenerKey) {
        this.listenerKey = listenerKey;
    }
}
