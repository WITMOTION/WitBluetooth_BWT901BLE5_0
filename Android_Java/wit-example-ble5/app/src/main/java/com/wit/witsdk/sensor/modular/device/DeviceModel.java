package com.wit.witsdk.sensor.modular.device;

import android.util.Log;

import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.sensor.modular.device.exceptions.OpenDeviceException;
import com.wit.witsdk.sensor.modular.device.interfaces.IDeviceSendCallback;
import com.wit.witsdk.sensor.modular.connector.roles.WitCoreConnect;
import com.wit.witsdk.sensor.modular.device.interfaces.IKeyUpdateObserver;
import com.wit.witsdk.sensor.modular.processor.interfaces.IDataProcessor;
import com.wit.witsdk.sensor.modular.resolver.interfaces.IProtocolResolver;
import com.wit.witsdk.sensor.modular.resolver.interfaces.ISendDataCallback;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

/**
 * 设备模型，在程序中作为设备的代理
 *
 * @author huangyajun
 * @date 2022/4/21 10:09
 */
public class DeviceModel extends DeviceDataSource implements Observer, IKeyUpdateObserver {

    /**
     * 设备名称
     */
    private String deviceName;

    /**
     * 是否打开的
     */
    private boolean isOpen;

    /**
     * 是否正在关闭
     */
    private boolean closing;

    /**
     * 核心连接器
     */
    private WitCoreConnect coreConnect;

    /**
     * 数据处理器
     */
    private IDataProcessor dataProcessor;

    /**
     * 协议解析器
     */
    private IProtocolResolver protocolResolver;

    /**
     * 监听Key值
     */
    private String listenerKey;

    /**
     * 是否接收返回数据
     */
    private boolean bolWaitReturn;

    /**
     * 返回的数据
     */
    private List<Byte> returnDataBuffer = new ArrayList<Byte>();

    /**
     * 发送数据锁
     */
    private Object sendLock = new Object();

    private Object revLock = new Object();

    private Object sendLock2 = new Object();

    /**
     * 构造方法
     *
     * @author huangyajun
     * @date 2022/4/21 11:30
     */
    public DeviceModel(String deviceName, IProtocolResolver protocolResolver, IDataProcessor dataProcessor, String listenerKey) {
        this.dataProcessor = dataProcessor;
        this.protocolResolver = protocolResolver;
        this.listenerKey = listenerKey;
        this.deviceName = deviceName;
        this.registerKeyUpdateObserver(this);
    }

    /**
     * 打开设备
     *
     * @author huangyajun
     * @date 2022/4/25 20:38
     */
    public void openDevice() throws OpenDeviceException {

        try {
            if (coreConnect.isOpen() == false) {
                coreConnect.open();
                coreConnect.removeObserver(this);
                coreConnect.registerObserver(this);
            }

            isOpen = true;
            // 打开事件
            if (this.dataProcessor != null) {
                new Thread(() -> {
                    try {
                        Thread.sleep(3000);
                        this.dataProcessor.OnOpen(this);
                    } catch (InterruptedException e) {
                        Log.e("", "设备打开事件错误");
                    }
                }).start();
            }
        } catch (Exception e) {
            e.printStackTrace();
            throw new OpenDeviceException("打开设备出错：" + e.getMessage());
        }
    }

    /**
     * 重新打开
     *
     * @author huangyajun
     * @date 2022/4/26 9:09
     */
    public void reOpen() throws OpenDeviceException {
        closeDevice();
        openDevice();
    }

    /**
     * 关闭设备
     *
     * @author huangyajun
     * @date 2022/4/26 9:09
     */
    public void closeDevice() {
        closing = true;
        if (coreConnect != null
                && coreConnect.isOpen()) {
            coreConnect.close();
        }
        isOpen = false;
        closing = false;

        // 关闭事件
        if (this.dataProcessor != null)
            this.dataProcessor.OnClose();
    }

    /**
     * 发送数据（有返回的数据）
     *
     * @author huangyajun
     * @date 2022/4/26 13:43
     */
    public boolean sendData(byte[] data, IDeviceSendCallback callback, int waitTime, int repetition) {
        synchronized (sendLock) {
            if (waitTime == 0) {
                waitTime = 100;
            }

            if (repetition == 0) {
                repetition = 1;
            }
            int count = 0;
            returnDataBuffer.clear();
            bolWaitReturn = true;
            try {
                while (count < repetition && bolWaitReturn) {

                    count++;
                    sendData(data);
                    Thread.sleep(waitTime);
                    if (returnDataBuffer.size() > 0) {
                        int rtnSize = returnDataBuffer.size();
                        callback.callback(returnDataBuffer.toArray(new Byte[rtnSize]));
                        break;
                    }
                }
                returnDataBuffer.clear();
                bolWaitReturn = false;
                return true;
            } catch (Exception e) {
                e.printStackTrace();
                returnDataBuffer.clear();
                bolWaitReturn = false;
                return false;
            }
        }
    }

    /**
     * 发送数据(没有返回的数据)
     *
     * @author huangyajun
     * @date 2022/4/26 13:43
     */
    public synchronized boolean sendData(byte[] data) {
        synchronized (sendLock2) {
            if (closing || coreConnect == null || !coreConnect.isOpen()) {
                return false;
            }
            try {
                coreConnect.sendData(data);
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
            return true;
        }
    }

    /**
     * 发送数据需要回调
     *
     * @Author zhangpingxiang
     * @Date 2023/2/25 0025 15:37
     */
    public boolean sendProtocolData(byte[] data, int waitTime, ISendDataCallback callback) {
        try {
            Thread thread = new Thread(() -> {
                protocolResolver.sendData(data, this, waitTime, callback);
            });
            thread.start();
            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    /**
     * 发送协议数据
     *
     * @author huangyajun
     * @date 2022/4/26 14:06
     */
    public boolean sendProtocolData(byte[] data, int waitTime) {
        return sendProtocolData(data, waitTime, (res) -> {
        });
    }

    /**
     * 发送协议数据
     *
     * @author huangyajun
     * @date 2022/4/26 14:06
     */
    public boolean sendProtocolData(byte[] data) {
        try {
            Thread thread = new Thread(() -> {
                protocolResolver.sendData(data, this);
            });
            thread.start();
            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    /**
     * 接收数据
     *
     * @author huangyajun
     * @date 2022/4/26 11:36
     */
    @Override
    public synchronized void update(byte[] message) {

        synchronized (revLock) {
            if (closing) {
                return;
            }

            if (bolWaitReturn) {
                for (int i = 0; i < message.length; i++) {
                    returnDataBuffer.add(message[i]);
                }
            }
            protocolResolver.passiveReceiveData(message, this);
        }
    }

    public String getDeviceName() {
        return deviceName;
    }

    public void setDeviceName(String deviceName) {
        this.deviceName = deviceName;
    }

    public boolean isOpen() {
        return isOpen;
    }

    public void setOpen(boolean open) {
        isOpen = open;
    }

    public boolean isClosing() {
        return closing;
    }

    public WitCoreConnect getCoreConnect() {
        return coreConnect;
    }

    public void setCoreConnect(WitCoreConnect coreConnect) {
        this.coreConnect = coreConnect;
    }

    public IDataProcessor getDataProcessor() {
        return dataProcessor;
    }

    public void setDataProcessor(IDataProcessor dataProcessor) {
        this.dataProcessor = dataProcessor;
    }

    public IProtocolResolver getProtocolResolver() {
        return protocolResolver;
    }

    public void setProtocolResolver(IProtocolResolver protocolResolver) {
        this.protocolResolver = protocolResolver;
    }

    public String getListenerKey() {
        return listenerKey;
    }

    public void setListenerKey(String listenerKey) {
        this.listenerKey = listenerKey;
    }

    @Override
    public void update(DeviceModel deviceModel, String key, Object value) {
        // 触发实时数据更新
        if (listenerKey != null && listenerKey.equals(key)) {
            // 刷新数据处理器
            if (dataProcessor != null) dataProcessor.OnUpdate(this);
            // 数据记录事件
            notifyListenKeyUpdateObserver(this);
        }
    }
}
