package com.wit.example;

import static com.inuker.bluetooth.library.Constants.STATUS_CONNECTED;
import static com.inuker.bluetooth.library.Constants.STATUS_DISCONNECTED;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.wit.example.ble5.interfaces.IBluetoothConnectStatusObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothBLE;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.BluetoothSPP;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.WitBluetoothManager;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.exceptions.BluetoothBLEException;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserver;
import com.wit.witsdk.sensor.modular.device.exceptions.OpenDeviceException;
import com.wit.witsdk.sensor.modular.processor.constant.WitSensorKey;
import com.wit.example.ble5.Bwt901ble;
import com.wit.example.ble5.interfaces.IBwt901bleRecordObserver;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Objects;

/**
 * 功能：主界面
 * Function: main interface
 * 说明：
 * Explanation：
 * 本程序是维特智能开发的蓝牙5.0sdk使用示例
 * This program is an example base on Bluetooth 5.0sdk developed by WitMotion
 * 本程序适用于维特智能以下产品
 * This program is applicable to the following products of WitMotion
 *  BWT901BLECL5.0
 *  BWT901BLE5.0
 *  WT901BLE5.0
 * 本程序只有一个页面，没有其它页面
 * This program has only one page and no other pages
 *
 * @author huangyajun
 * @date 2022/6/29 11:35
 */
public class MainActivity extends AppCompatActivity implements IBluetoothFoundObserver, IBwt901bleRecordObserver, IBluetoothConnectStatusObserver {

    /**
     * 日志标签
     * log tag
     */
    private static final String TAG = "MainActivity";

    /**
     * 设备列表
     * Device List
     */
    private List<Bwt901ble> bwt901bleList = new ArrayList<>();

    /**
     * 控制自动刷新线程是否工作
     * Controls whether the auto-refresh thread works
     */
    private boolean destroyed = true;

    /**
     * activity 创建时
     * activity when created
     *
     * @author huangyajun
     * @date 2022/6/29 8:43
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        try {
            WitBluetoothManager.requestPermissions(this);
            // 初始化蓝牙管理器，这里会申请蓝牙权限
            // Initialize the Bluetooth manager, here will apply for Bluetooth permissions
            WitBluetoothManager.initInstance(this);
        }catch (Exception e){
            Log.e("",e.getMessage());
            e.printStackTrace();
        }

        // 开始搜索按钮
        // start search button
        Button startSearchButton = findViewById(R.id.startSearchButton);
        startSearchButton.setOnClickListener((v) -> {
            startDiscovery();
        });

        // 停止搜索按钮
        // stop search button
        Button stopSearchButton = findViewById(R.id.stopSearchButton);
        stopSearchButton.setOnClickListener((v) -> {
            stopDiscovery();
        });

        // 加计校准按钮
        // Acceleration calibration button
        Button appliedCalibrationButton = findViewById(R.id.appliedCalibrationButton);
        appliedCalibrationButton.setOnClickListener((v) -> {
            handleAppliedCalibration();
        });

        // 开始磁场校准按钮
        // Start Magnetic Field Calibration button
        Button startFieldCalibrationButton = findViewById(R.id.startFieldCalibrationButton);
        startFieldCalibrationButton.setOnClickListener((v) -> {
            handleStartFieldCalibration();
        });

        // 结束磁场校准按钮
        // End Magnetic Field Calibration button
        Button endFieldCalibrationButton = findViewById(R.id.endFieldCalibrationButton);
        endFieldCalibrationButton.setOnClickListener((v) -> {
            handleEndFieldCalibration();
        });

        // 读取03寄存器按钮
        // Read 03 register button
        Button readReg03Button = findViewById(R.id.readReg03Button);
        readReg03Button.setOnClickListener((v) -> {
            handleReadReg03();
        });

        // 自动刷新数据线程
        // Auto refresh data thread
        Thread thread = new Thread(this::refreshDataTh);
        destroyed = false;
        thread.start();
    }

    /**
     * activity perish
     *
     * @author huangyajun
     * @date 2022/6/29 13:59
     */
    @Override
    protected void onDestroy() {
        super.onDestroy();

    }

    /**
     * activity 销毁时
     * Start searching for devices
     *
     * @author huangyajun
     * @date 2022/6/29 10:04
     */
    public void startDiscovery() {

        // 开始搜索设备
        // Turn off all device
        for (int i = 0; i < bwt901bleList.size(); i++) {
            Bwt901ble bwt901ble = bwt901bleList.get(i);
            bwt901ble.removeRecordObserver(this);
            bwt901ble.close();
        }

        // 清除所有设备
        // Erase all devices
        bwt901bleList.clear();

        // 开始搜索蓝牙
        // Start searching for bluetooth
        try {
            // 获得蓝牙管理器
            // get bluetooth manager
            WitBluetoothManager bluetoothManager = WitBluetoothManager.getInstance();
            // 注册监听蓝牙
            // Monitor communication signals
            bluetoothManager.registerObserver(this);
            // 指定要搜索的蓝牙名称
            // Specify the Bluetooth name to search for
            WitBluetoothManager.DeviceNameFilter = Arrays.asList("WT");
            // 开始搜索
            // start search
            bluetoothManager.startDiscovery();
        } catch (BluetoothBLEException e) {
            e.printStackTrace();
        }
    }

    /**
     * 停止搜索设备
     * Stop searching for devices
     *
     * @author huangyajun
     * @date 2022/6/29 10:04
     */
    public void stopDiscovery() {
        // 停止搜索蓝牙
        // stop searching for bluetooth
        try {
            // 获得蓝牙管理器
            // acquire Bluetooth manager
            WitBluetoothManager bluetoothManager = WitBluetoothManager.getInstance();
            // 取消注册监听蓝牙
            // Cancel monitor communication signals
            bluetoothManager.removeObserver(this);
            // 停止搜索
            // stop searching
            bluetoothManager.stopDiscovery();
        } catch (BluetoothBLEException e) {
            e.printStackTrace();
        }
    }

    /**
     * 当搜到蓝牙5.0设备时会回调这个方法
     * This method will be called back when a Bluetooth 5.0 device is found
     *
     * @author huangyajun
     * @date 2022/6/29 8:46
     */
    @Override
    public void onFoundBle(BluetoothBLE bluetoothBLE) {
        // 创建蓝牙5.0传感器连接对象
        // Create a Bluetooth 5.0 sensor connection object
        Bwt901ble bwt901ble = new Bwt901ble(bluetoothBLE);

        // 避免重复连接
        // Avoid duplicate connections
        for (int i = 0; i < bwt901bleList.size(); i++) {
            if (Objects.equals(bwt901bleList.get(i).getDeviceName(), bwt901ble.getDeviceName())) {
                return;
            }
        }

        // 添加到设备列表
        // add to device list
        bwt901bleList.add(bwt901ble);

        // 注册状态监听
        // Registration status monitoring
        bwt901ble.registerStatusObserver(this);

        // 注册数据记录
        // Registration data record
        bwt901ble.registerRecordObserver(this);

        // 打开设备
        // Turn on the device
        try {
            bwt901ble.open();
        } catch (OpenDeviceException e) {
            // 打开设备失败
            // Failed to open device
            e.printStackTrace();
        }
    }

    /**
     * 当搜索到蓝牙2.0设备时会回调这个方法
     * This method will be called back when a Bluetooth 2.0 device is found
     *
     * @author huangyajun
     * @date 2022/6/29 10:01
     */
    @Override
    public void onFoundSPP(BluetoothSPP bluetoothSPP) {
        // 不做任何处理，这个示例程序只演示如何连接蓝牙5.0设备
        // Without doing any processing, this sample program only demonstrates how to connect a Bluetooth 5.0 device
    }

    /**
     * 找到双模蓝牙时
     * This method will be called back when data needs to be recorded
     *
     * @author huangyajun
     * @date 2023/5/18 18:15
     */
    @Override
    public void onFoundDual(BluetoothBLE bluetoothBLE) {

    }

    /**
     * 当需要记录数据时会回调这个方法
     * This method will be called back when data needs to be recorded
     *
     * @author huangyajun
     * @date 2022/6/29 8:46
     */
    @Override
    public void onRecord(Bwt901ble bwt901ble) {
        String deviceData = getDeviceData(bwt901ble);
        Log.d(TAG, "device data [ " + bwt901ble.getDeviceName() + "] = " + deviceData);
    }

    /**
     * 自动刷新数据线程
     *Auto refresh data thread
     *
     * @author huangyajun
     * @date 2022/6/29 13:41
     */
    private void refreshDataTh() {

        while (!destroyed) {
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            StringBuilder text = new StringBuilder();
            for (int i = 0; i < bwt901bleList.size(); i++) {
                // 让所有设备进行加计校准
                // Make all devices accelerometer calibrated
                Bwt901ble bwt901ble = bwt901bleList.get(i);
                String deviceData = getDeviceData(bwt901ble);
                text.append(deviceData);
            }

            TextView deviceDataTextView = findViewById(R.id.deviceDataTextView);
            runOnUiThread(() -> {
                deviceDataTextView.setText(text.toString());
            });
        }
    }

    /**
     * 获得一个设备的数据
     * Get a device's data
     *
     * @author huangyajun
     * @date 2022/6/29 11:37
     */
    private String getDeviceData(Bwt901ble bwt901ble) {
        StringBuilder builder = new StringBuilder();
        builder.append(bwt901ble.getDeviceName()).append("\n");
        builder.append(bwt901ble.getDeviceData("time")).append("\n");
        builder.append(getString(R.string.accX)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AccX)).append("g \t");
        builder.append(getString(R.string.accY)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AccY)).append("g \t");
        builder.append(getString(R.string.accZ)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AccZ)).append("g \n");
        builder.append(getString(R.string.asX)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AsX)).append("°/s \t");
        builder.append(getString(R.string.asY)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AsY)).append("°/s \t");
        builder.append(getString(R.string.asZ)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AsZ)).append("°/s \n");
        builder.append(getString(R.string.angleX)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AngleX)).append("° \t");
        builder.append(getString(R.string.angleY)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AngleY)).append("° \t");
        builder.append(getString(R.string.angleZ)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.AngleZ)).append("° \n");
        builder.append(getString(R.string.hX)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.HX)).append("\t");
        builder.append(getString(R.string.hY)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.HY)).append("\t");
        builder.append(getString(R.string.hZ)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.HZ)).append("\n");
        builder.append(getString(R.string.t)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.T)).append("\n");
        builder.append(getString(R.string.electricQuantityPercentage)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.ElectricQuantityPercentage)).append("\n");
        builder.append(getString(R.string.versionNumber)).append(":").append(bwt901ble.getDeviceData(WitSensorKey.VersionNumber)).append("\n");
        return builder.toString();
    }

    /**
     * 让所有设备加计校准
     * Make all devices accelerometer calibrated
     *
     * @author huangyajun
     * @date 2022/6/29 10:25
     */
    private void handleAppliedCalibration() {
        for (int i = 0; i < bwt901bleList.size(); i++) {
            Bwt901ble bwt901ble = bwt901bleList.get(i);
            // 解锁寄存器
            // unlock register
            bwt901ble.unlockReg();
            // 发送命令
            // send command
            bwt901ble.appliedCalibration();
        }
        Toast.makeText(this, "OK", Toast.LENGTH_LONG).show();
    }

    /**
     * 让所有设备开始磁场校准
     * Let all devices begin magnetic field calibration
     *
     * @author huangyajun
     * @date 2022/6/29 10:25
     */
    private void handleStartFieldCalibration() {
        for (int i = 0; i < bwt901bleList.size(); i++) {
            Bwt901ble bwt901ble = bwt901bleList.get(i);
            // 解锁寄存器
            // unlock register
            bwt901ble.unlockReg();
            // 发送命令
            // send command
            bwt901ble.startFieldCalibration();
        }
        Toast.makeText(this, "OK", Toast.LENGTH_LONG).show();
    }

    /**
     * 让所有设备结束磁场校准
     * Let's all devices end the magnetic field calibration
     *
     * @author huangyajun
     * @date 2022/6/29 10:25
     */
    private void handleEndFieldCalibration() {
        for (int i = 0; i < bwt901bleList.size(); i++) {
            Bwt901ble bwt901ble = bwt901bleList.get(i);
            // 解锁寄存器
            // unlock register
            bwt901ble.unlockReg();
            // 发送命令
            // send command
            bwt901ble.endFieldCalibration();
        }
        Toast.makeText(this, "OK", Toast.LENGTH_LONG).show();
    }

    /**
     * 读取03寄存器的数据
     * Read 03 register data
     *
     * @author huangyajun
     * @date 2022/6/29 10:25
     */
    private void handleReadReg03() {
        for (int i = 0; i < bwt901bleList.size(); i++) {
            Bwt901ble bwt901ble = bwt901bleList.get(i);
            // 必须使用 sendProtocolData 方法，使用此方法设备才会将寄存器值读取上来
            // Must be used sendProtocolData method, and the device will read the register value when you using this method
            int waitTime = 200;
            // 发送指令的命令,并且等待200ms
            // The command to send the command, and wait 200ms
            bwt901ble.sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xAA, (byte) 0x27, (byte) 0x03, (byte) 0x00}, waitTime);
            // 获得寄存器03的值
            //get the value of register 03
            String reg03Value = bwt901ble.getDeviceData("03");
            // 如果读上来了 reg03Value 就是寄存器的值，如果没有读上来可以将 waitTime 放大,或者多读几次
            // If it is read up, reg03Value is the value of the register. If it is not read up, you can enlarge waitTime, or read it several times.v
            Toast.makeText(this, bwt901ble.getDeviceName() + " reg03Value: " + reg03Value, Toast.LENGTH_LONG).show();
        }
    }

    // 传感器连接状态回调 Sensor connection status callback
    @Override
    public void onStatusChanged(String mac, int status) {
        // 已连接 Connected
        if(status == STATUS_CONNECTED){
            // System.out.println("Device is connected");
        }
        // 断开连接 Disconnect
        else if(status == STATUS_DISCONNECTED){
            // System.out.println("Device is disconnect");
        }
        else{}
    }
}