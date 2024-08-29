package com.wit.witsdk.sensor.modular.connector.roles;


import android.util.Log;

import com.wit.witsdk.sensor.modular.connector.entity.BluetoothSPPOption;
import com.wit.witsdk.sensor.modular.connector.enums.ConnectStatus;
import com.wit.witsdk.sensor.modular.connector.enums.ConnectType;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.WitBluetoothManager;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.exceptions.BluetoothBLEException;
import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.observer.interfaces.Observerable;
import com.wit.witsdk.observer.role.ObserverServer;
import com.wit.witsdk.sensor.modular.connector.entity.BluetoothBLEOption;
import com.wit.witsdk.sensor.modular.connector.entity.ConnectConfig;
import com.wit.witsdk.sensor.modular.connector.exceptions.ConnectConfigException;
import com.wit.witsdk.sensor.modular.connector.exceptions.ConnectOpenException;
import com.wit.witsdk.sensor.modular.connector.interfaces.IWitCoreConnect;
import com.wit.witsdk.sensor.modular.connector.enums.*;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;
import com.wit.witsdk.utils.StringUtils;
import java.net.SocketException;

/**
 * 核心连接器
 *
 * @author huangyajun
 * @date 2022/4/21 10:20
 */
public class WitCoreConnect implements IWitCoreConnect, Observerable, Observer {

    // spp蓝牙
    private BluetoothSPP bluetoothSPP;

    // ble蓝牙
    private BluetoothBLE bluetoothBLE;

    // 连接类型
    private ConnectType connectType = ConnectType.UDP;

    // 连接状态
    private ConnectStatus connectStatus = ConnectStatus.Closed;

    // 连接配置
    private ConnectConfig config = new ConnectConfig();

    // 观察服务
    private ObserverServer observerServer = new ObserverServer();


    /**
     * 打开设备
     *
     * @author huangyajun
     * @date 2022/4/25 15:48
     */
    @Override
    public void open() throws ConnectConfigException, ConnectOpenException, SocketException {
        if (config == null) {
            throw new ConnectConfigException("连接参数不能为空");
        }

        // 检查配置
        checkConfig();

        // BLE蓝牙连接时
        if (connectType.getCode() == ConnectType.BluetoothBLE.getCode()) {
            bluetoothBLEOpen();
        } else if (connectType.getCode() == ConnectType.BluetoothSPP.getCode()) {
            bluetoothSPPOpen();
        }else {
            throw new ConnectConfigException("没有连接类型");
        }

        // 设置为已经连接状态
        connectStatus = ConnectStatus.Opened;
    }

    /**
     * 打开ble蓝牙
     *
     * @author huangyajun
     * @date 2022/4/28 16:21
     */
    private void bluetoothBLEOpen() throws ConnectOpenException {

        BluetoothBLEOption bluetoothBLEOption = config.getBluetoothBLEOption();

        WitBluetoothManager bluetoothManager = null;
        try {
            bluetoothManager = WitBluetoothManager.getInstance();
        } catch (BluetoothBLEException e) {
            e.printStackTrace();
        }

        bluetoothBLE = bluetoothManager.getBluetoothBLE(bluetoothBLEOption.getMac());

        if (bluetoothBLE != null) {
            bluetoothBLE.registerObserver(this);
            bluetoothBLE.connect(bluetoothBLEOption.getMac());
        } else {
            throw new ConnectOpenException("无法打开此蓝牙设备");
        }

    }

    /**
     * 打开经典蓝牙
     *
     * @author huangyajun
     * @date 2022/6/17 9:27
     */
    private void bluetoothSPPOpen() throws ConnectOpenException {
        BluetoothSPPOption bluetoothSPPOption = config.getBluetoothSPPOption();
        WitBluetoothManager bluetoothManager = null;
        try {
            bluetoothManager = WitBluetoothManager.getInstance();
        } catch (BluetoothBLEException e) {
            e.printStackTrace();
        }

        bluetoothSPP = bluetoothManager.getBluetoothSPP(bluetoothSPPOption.getMac());

        if (bluetoothSPP != null) {
            bluetoothSPP.registerObserver(this);
            bluetoothSPP.connect(bluetoothSPPOption.getMac());
        } else {
            throw new ConnectOpenException("无法打开此蓝牙设备");
        }
    }

    /**
     * 检查连接配置
     *
     * @author huangyajun
     * @date 2022/4/25 15:55
     */
    private void checkConfig() throws ConnectConfigException {

        if (connectType.getCode() == ConnectType.BluetoothBLE.getCode()) {
            // BLE蓝牙连接时
            BluetoothBLEOption bluetoothBLEOption = config.getBluetoothBLEOption();
            if (StringUtils.isBlank(bluetoothBLEOption.getMac())) {
                throw new ConnectConfigException("bluetoothBLEOption 缺少mac地址");
            }
        } else if (connectType.getCode() == ConnectType.BluetoothSPP.getCode()) {
            // 经典蓝牙连接时
            BluetoothSPPOption bluetoothSPPOption = config.getBluetoothSPPOption();
            if (StringUtils.isBlank(bluetoothSPPOption.getMac())) {
                throw new ConnectConfigException("bluetoothSPPOption 缺少mac地址");
            }
        } else {
            throw new ConnectConfigException("没有连接类型");
        }
    }

    /**
     * 是否打开的
     *
     * @author huangyajun
     * @date 2022/4/25 15:57
     */
    @Override
    public boolean isOpen() {
        return connectStatus.getCode() == ConnectStatus.Opened.getCode();
    }

    /**
     * 关闭设备
     *
     * @author huangyajun
     * @date 2022/4/25 19:15
     */
    @Override
    public void close() {

        if (bluetoothBLE != null) {
            bluetoothBLE.removeObserver(this);
            bluetoothBLE.disconnect();
        }

        if (bluetoothSPP != null) {
            bluetoothSPP.removeObserver(this);
            bluetoothSPP.stop();
        }

        // 设置为已经连接状态
        connectStatus = ConnectStatus.Closed;
    }

    /**
     * 发送数据
     *
     * @author huangyajun
     * @date 2022/4/25 19:15
     */
    @Override
    public void sendData(byte[] data) {
        // 蓝牙连接时
        if (connectType.getCode() == ConnectType.BluetoothBLE.getCode()) {
            bluetoothBLE.write(data);
        } else if (connectType.getCode() == ConnectType.BluetoothSPP.getCode()) {
            bluetoothSPP.write(data);
        }
    }

    @Override
    public boolean setConnectType(ConnectType connectType) {
        this.connectType = connectType;
        return false;
    }

    @Override
    public ConnectType getConnectType() {
        return connectType;
    }

    @Override
    public ConnectStatus getConnectStatus() {
        return null;
    }

    public ConnectConfig getConfig() {
        return config;
    }

    @Override
    public void update(byte[] data) {
        notifyObserver(data);
    }

    @Override
    public void registerObserver(Observer o) {
        observerServer.registerObserver(o);
    }

    @Override
    public void removeObserver(Observer o) {
        observerServer.removeObserver(o);
    }

    @Override
    public void notifyObserver(byte[] data) {

        Thread thread = new Thread(() -> {
            observerServer.notifyObserver(data);
        });
        thread.start();
    }

    public BluetoothSPP getBluetoothSPP() {
        return bluetoothSPP;
    }

    public BluetoothBLE getBluetoothBLE() {
        return bluetoothBLE;
    }
}
