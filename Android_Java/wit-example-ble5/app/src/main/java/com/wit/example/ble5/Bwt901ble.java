package com.wit.example.ble5;

import com.wit.example.ble5.components.Bwt901bleProcessor;
import com.wit.example.ble5.components.Bwt901bleResolver;
import com.wit.example.ble5.interfaces.IBwt901bleRecordObserver;
import com.wit.witsdk.api.interfaces.IAttitudeSensorApi;
import com.wit.witsdk.sensor.dkey.DoubleKey;
import com.wit.witsdk.sensor.dkey.ShortKey;
import com.wit.witsdk.sensor.dkey.StringKey;
import com.wit.witsdk.sensor.modular.connector.enums.ConnectType;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.roles.WitCoreConnect;
import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.device.exceptions.OpenDeviceException;
import com.wit.witsdk.sensor.modular.device.interfaces.IDeviceSendCallback;
import com.wit.witsdk.sensor.modular.device.interfaces.IListenKeyUpdateObserver;

import java.util.ArrayList;
import java.util.List;

/**
 * 蓝牙5.0传感器模型
 *
 * @author huangyajun
 * @date 2022/6/28 20:50
 */
public class Bwt901ble implements IListenKeyUpdateObserver, IAttitudeSensorApi {

    /**
     * 设备模型
     */
    private DeviceModel deviceModel;

    /**
     * 蓝牙连接
     */
    private BluetoothBLE bluetoothBLE;

    /**
     * 监控数据的人
     */
    private List<IBwt901bleRecordObserver> recordObservers = new ArrayList<>();

    /**
     * 构造方法
     *
     * @author huangyajun
     * @date 2022/6/28 20:53
     */
    public Bwt901ble(BluetoothBLE bluetoothBLE) {
        // 创建一个连接蓝牙的设备模型
        DeviceModel deviceModel = new DeviceModel(bluetoothBLE.getName() + "(" + bluetoothBLE.getMac() + ")",
                new Bwt901bleResolver(),
                new Bwt901bleProcessor(),
                "61_0");
        WitCoreConnect witCoreConnect = new WitCoreConnect();
        witCoreConnect.setConnectType(ConnectType.BluetoothBLE);
        witCoreConnect.getConfig().getBluetoothBLEOption().setMac(bluetoothBLE.getMac());
        deviceModel.setCoreConnect(witCoreConnect);
        deviceModel.setDeviceData("Mac", bluetoothBLE.getMac());

        this.deviceModel = deviceModel;
        this.bluetoothBLE = bluetoothBLE;
    }

    /**
     * 打开连接
     *
     * @author huangyajun
     * @date 2022/6/28 20:51
     */
    public void open() throws OpenDeviceException {
        deviceModel.openDevice();
    }

    /**
     * 关闭连接
     *
     * @author huangyajun
     * @date 2022/6/28 20:51
     */
    public void close() {
        deviceModel.closeDevice();
    }

    
    /**
     * 是否打开的
     *
     * @author huangyajun
     * @date 2022/7/2 17:51
     */
    public boolean isOpen() {
        return deviceModel.isOpen();
    }

    /**
     * 发送数据
     *
     * @author huangyajun
     * @date 2022/6/28 20:51
     */
    public void sendData(byte[] data, IDeviceSendCallback callback, int waitTime, int repetition) {
        deviceModel.sendData(data, callback, waitTime, repetition);
    }

    /**
     * 发送带协议的数据，使用默认等待时长
     *
     * @author huangyajun
     * @date 2022/6/28 20:51
     */
    public void sendProtocolData(byte[] data) {
        deviceModel.sendProtocolData(data);
    }

    /**
     * 发送带协议的数据,并且指定等待时长
     *
     * @author huangyajun
     * @date 2022/6/28 20:51
     */
    public void sendProtocolData(byte[] data, int waitTime) {
        deviceModel.sendProtocolData(data, waitTime);
    }

    /**
     * 解锁寄存器
     *
     * @author huangyajun
     * @date 2022/6/29 11:23
     */
    public void unlockReg() {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x69, (byte) 0x88, (byte) 0xB5,});
    }

    /**
     * 保存寄存器
     *
     * @author maoqiang
     * @date 2022/7/12 10:12
     */
    @Override
    public void saveReg() {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x00, (byte) 0x00, (byte) 0x00,});
    }
    
    /**
     * 加计校准
     *
     * @author huangyajun
     * @date 2022/6/28 20:52
     */
    public void appliedCalibration() {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x01, (byte) 0x01, (byte) 0x00,});
    }

    /**
     * 开始磁场校准
     *
     * @author huangyajun
     * @date 2022/6/28 21:01
     */
    public void startFieldCalibration() {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x01, (byte) 0x07, (byte) 0x00,});
    }

    /**
     * 结束磁场校准
     *
     * @author huangyajun
     * @date 2022/6/28 21:01
     */
    public void endFieldCalibration() {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x01, (byte) 0x00, (byte) 0x00,});
    }

    /**
     * 设置回传速率
     *
     * @author huangyajun
     * @date 2022/6/28 21:01
     */
    public void setReturnRate(byte rate) {
        sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x03, rate, (byte) 0x00,});
    }

    /**
     * 获得设备名称
     *
     * @author huangyajun
     * @date 2022/6/29 11:44
     */
    public String getDeviceName() {
        return deviceModel.getDeviceName();
    }

    /**
     * 获得设备名称
     *
     * @author huangyajun
     * @date 2022/6/29 11:44
     */
    public String getMac() {
        return bluetoothBLE.getMac();
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/6/28 21:07
     */
    public String getDeviceData(String key) {
        return deviceModel.getDeviceData(key);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/6/28 21:07
     */
    public Short getDeviceData(ShortKey key) {
        return deviceModel.getDeviceData(key);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/6/28 21:07
     */
    public String getDeviceData(StringKey key) {
        return deviceModel.getDeviceData(key);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/6/28 21:07
     */
    public Double getDeviceData(DoubleKey key) {
        return deviceModel.getDeviceData(key);
    }

    /**
     * 注册数据记录
     *
     * @author huangyajun
     * @date 2022/6/28 21:02
     */
    public void registerRecordObserver(IBwt901bleRecordObserver record) {
        deviceModel.registerListenKeyUpdateObserver(this);
        recordObservers.add(record);
    }

    /**
     * 移除数据记录监听
     *
     * @author huangyajun
     * @date 2022/6/28 21:02
     */
    public void removeRecordObserver(IBwt901bleRecordObserver record) {
        deviceModel.removeListenKeyUpdateObserver(this);

        if (!recordObservers.isEmpty()) {
            recordObservers.remove(record);
        }
    }

    /**
     * 记录数据
     *
     * @author huangyajun
     * @date 2022/6/29 10:27
     */
    @Override
    public void update(DeviceModel deviceModel) {
        deviceModel_OnListenKeyUpdate(deviceModel);
    }
    
    /**
     * 记录数据
     *
     * @author maoqiang
     * @date 2022/7/12 10:13
     */
    @Override
    public void deviceModel_OnListenKeyUpdate(DeviceModel deviceModel) {
        for (int i = 0; i < recordObservers.size(); i++) {
            IBwt901bleRecordObserver iBwt901bleRecordObserver = recordObservers.get(i);
            iBwt901bleRecordObserver.onRecord(this);
        }
    }
}
