package com.wit.witsdk.api.interfaces;

import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.device.exceptions.OpenDeviceException;
import com.wit.witsdk.sensor.modular.device.interfaces.IDeviceSendCallback;

/**
 * 姿态传感器API接口
 *
 * @author maoqiang
 * @date 2022/7/12 10:07
 */
public interface IAttitudeSensorApi {

    /**
     * 打开设备
     *
     * @author maoqiang
     * @date 2022/7/12 10:15
     */
    void open() throws OpenDeviceException;

    /**
     * 是否打开了设备
     *
     * @author maoqiang
     * @date 2022/7/12 10:17
     */
    boolean isOpen();

    /**
     * 关闭设备
     *
     * @author maoqiang
     * @date 2022/7/12 10:17
     */
    void close();

    /**
     * 发送数据
     * <param name="data">需要发送出去的数据</param>
     * <param name="callback">传感器返回的数据</param>
     * <param name="waitTime">等待传感器返回数据时间，单位ms，默认100ms</param>
     * <param name="repetition">重复发送次数</param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:09
     */
    void sendData(byte[] data, IDeviceSendCallback callback, int waitTime, int repetition);

    /**
     * 发送带协议的数据，使用默认等待时长
     * <param name="data">数据</param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:19
     */
    void sendProtocolData(byte[] data);

    /**
     * 发送带协议的数据,并且指定等待时长
     * <param name="data">数据</param>
     * <param name="waitTime">等待时间</param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:20
     */
    void sendProtocolData(byte[] data, int waitTime);

    /**
     * 解锁寄存器
     *
     * @author maoqiang
     * @date 2022/7/12 10:20
     */
    void unlockReg();

    /**
     * 保存寄存器
     *
     * @author maoqiang
     * @date 2022/7/12 10:21
     */
    void saveReg();

    /**
     * 加计校准
     *
     * @author maoqiang
     * @date 2022/7/12 10:22
     */
    void appliedCalibration();

    /**
     * 开始磁场校准
     *
     * @author maoqiang
     * @date 2022/7/12 10:22
     */
    void startFieldCalibration();

    /**
     * 结束磁场校准
     *
     * @author maoqiang
     * @date 2022/7/12 10:22
     */
    void endFieldCalibration();

    /**
     * 设置回传速率
     * <param name="rate"></param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:22
     */
    void setReturnRate(byte rate);

    /**
     * 获得设备名称
     *
     * @author maoqiang
     * @date 2022/7/12 10:22
     */
    String getDeviceName();

    /**
     * 获得数据
     * <param name="key">数据键值</param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:23
     */
    String getDeviceData(String key);

    /**
     * 传感器数据更新时
     * <param name="deviceModel">设备模型</param>
     *
     * @author maoqiang
     * @date 2022/7/12 10:23
     */
    void deviceModel_OnListenKeyUpdate(DeviceModel deviceModel);
}
