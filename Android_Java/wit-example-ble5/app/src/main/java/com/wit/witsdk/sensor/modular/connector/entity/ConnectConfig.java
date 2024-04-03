package com.wit.witsdk.sensor.modular.connector.entity;

/**
 * 连接配置
 *
 * @author huangyajun
 * @date 2022/4/21 10:29
 */
public class ConnectConfig {

    private BluetoothBLEOption bluetoothBLEOption = new BluetoothBLEOption();

    private BluetoothSPPOption bluetoothSPPOption = new BluetoothSPPOption();

    public BluetoothBLEOption getBluetoothBLEOption() {
        return bluetoothBLEOption;
    }

    public void setBluetoothBLEOption(BluetoothBLEOption bluetoothBLEOption) {
        this.bluetoothBLEOption = bluetoothBLEOption;
    }

    public BluetoothSPPOption getBluetoothSPPOption() {
        return bluetoothSPPOption;
    }

    public void setBluetoothSPPOption(BluetoothSPPOption bluetoothSPPOption) {
        this.bluetoothSPPOption = bluetoothSPPOption;
    }
}
