package com.wit.witsdk.sensor.modular.resolver.interfaces;

import com.wit.witsdk.sensor.modular.resolver.entity.SendDataResult;

/**
 * 读取数据的返回结构
 *
 * @Author zhangpingxiang
 * @Date 2023/2/25 0025 15:51
 */
public interface ISendDataCallback {

    /**
     * 执行回调方法
     *
     * @Author zhangpingxiang
     * @Date 2023/2/25 0025 15:52
     */
    void run(SendDataResult result);

}
