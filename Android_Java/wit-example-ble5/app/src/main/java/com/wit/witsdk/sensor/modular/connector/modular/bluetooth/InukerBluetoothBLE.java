package com.wit.witsdk.sensor.modular.connector.modular.bluetooth;

import static com.inuker.bluetooth.library.Constants.REQUEST_FAILED;
import static com.inuker.bluetooth.library.Constants.REQUEST_SUCCESS;
import static com.inuker.bluetooth.library.Constants.STATUS_DEVICE_CONNECTED;
import static com.inuker.bluetooth.library.Constants.STATUS_DEVICE_CONNECTING;
import static com.inuker.bluetooth.library.Constants.STATUS_DEVICE_DISCONNECTED;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.content.Context;
import android.util.Log;

import com.inuker.bluetooth.library.BluetoothClient;
import com.inuker.bluetooth.library.connect.listener.BleConnectStatusListener;
import com.inuker.bluetooth.library.connect.response.BleNotifyResponse;
import com.inuker.bluetooth.library.connect.response.BleReadRssiResponse;
import com.inuker.bluetooth.library.connect.response.BleWriteResponse;
import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.observer.interfaces.Observerable;
import com.wit.witsdk.observer.role.ObserverServer;

import java.util.UUID;

public class InukerBluetoothBLE implements Observerable {

    // 没连接上的标识
    public int STATUS_DISCONNECTED = STATUS_DEVICE_DISCONNECTED;

    // 连接上了的标识
    public int STATUS_CONNECTED = STATUS_DEVICE_CONNECTED;

    // 连接中标识
    public int STATUS_CONNECTING = STATUS_DEVICE_CONNECTING;

    // 发送uuid
    private UUID UUID_SEND = UUID.fromString("49535343-8841-43f4-a8d4-ecbe34729bb3");

    // 读取uuid
    private UUID UUID_READ = UUID.fromString("49535343-1e4d-4bd9-ba61-23c647249616");

    // 服务uuid
    private UUID UUID_SERVICE = UUID.fromString("49535343-fe7d-4ae5-8fa9-9fafd205e455");

    // 当前状态枚举
    private enum State {Work, Leisure}

    // 当前连接状态
    public int connectStatus = STATUS_DEVICE_DISCONNECTED;

    // 蓝牙信号
    private int rssi = 0;

    // 蓝牙mac地址
    private String mac;

    // 蓝牙名称
    private String name;

    // 蓝牙状态
    private State mState = State.Leisure;

    // 蓝牙客户端
    private BluetoothClient mClient;

    // 观察服务
    private ObserverServer observerServer = new ObserverServer();

    /**
     * 构造
     *
     * @author huangyajun
     * @date 2022/4/28 15:28
     */
    @SuppressLint("MissingPermission")
    public InukerBluetoothBLE(Context context, String mac, String name) {

        mClient = new BluetoothClient(context);
        final BluetoothAdapter mAdapter = BluetoothAdapter.getDefaultAdapter();
        if (mAdapter == null) return;
        if (!mAdapter.isEnabled()) {
            mAdapter.enable();
        }

        this.setName(name);
        this.setMac(mac);
    }

    /**
     * 设置uuid
     *
     * @author huangyajun
     * @date 2022/4/28 15:28
     */
    public void setUUID(String strService, String strSend, String strRead) {
        UUID_SERVICE = UUID.fromString(strService);
        UUID_SEND = UUID.fromString(strSend);
        UUID_READ = UUID.fromString(strRead);
    }

    /**
     * 获得连接状态
     *
     * @author huangyajun
     * @date 2022/4/28 15:28
     */
    public int getConnectStatus(String mac) {
        return mClient.getConnectStatus(mac);
    }

    /**
     * 连接蓝牙
     *
     * @author huangyajun
     * @date 2022/4/28 15:28
     */
    public void connect(String mac) {
        this.mac = mac;
        Log.e("BluetoothBLE", "conectmac:" + connectStatus);
        if ((connectStatus == STATUS_CONNECTING) || (connectStatus == STATUS_CONNECTED)) return;
        connectStatus = STATUS_CONNECTING;

//        BleConnectOptions.Builder builder = new BleConnectOptions.Builder();
//        builder.setConnectRetry(1);
//        builder.setConnectTimeout(10000);
//        builder.setServiceDiscoverRetry(3);
//        builder.setServiceDiscoverTimeout(10000);
//        BleConnectOptions bleConnectOptions = new BleConnectOptions(builder);
        // 收到蓝牙数据时
        mClient.connect(this.mac, (code, data) -> {
            Log.e("BluetoothBLE", "conect:" + code);
            if (code == REQUEST_SUCCESS) {
                connectStatus = STATUS_CONNECTED;
                Log.e("BluetoothBLE", "连接成功");
                Log.e("BluetoothBLE", "mac:" + mac.toString());
                Log.e("BluetoothBLE", "UUID_SERVICE:" + UUID_SERVICE.toString());
                Log.e("BluetoothBLE", "UUID_READ:" + UUID_READ.toString());

                setNotify();
            }

            if (code == REQUEST_FAILED) {
                Log.e("BluetoothBLE", "连接断开");
                connectStatus = STATUS_DISCONNECTED;
            }
        });
    }

    private void setNotify() {
        // 订阅蓝牙的数据
        mClient.notify(InukerBluetoothBLE.this.mac, UUID_SERVICE, UUID_READ, new BleNotifyResponse() {
            @Override
            public void onNotify(UUID service, UUID character, byte[] value) {
                notifyObserver(value);
            }

            @Override
            public void onResponse(int code) {
                if (code == REQUEST_SUCCESS) {
                }
            }
        });
    }

    /**
     * 断开蓝牙连接
     *
     * @author huangyajun
     * @date 2022/4/28 15:29
     */
    public void disconnect() {
        if (observerServer.observerSize() == 0) {
            mClient.disconnect(mac);
            connectStatus = STATUS_DISCONNECTED;
        }
    }

    /**
     * 设置信号值
     */
    public void setRssi(int rssi) {
        this.rssi = rssi;
    }

    /**
     * 获得蓝牙信号
     *
     * @author huangyajun
     * @date 2022/4/28 15:29
     */
    public int getRssi() {
        mClient.readRssi(mac, new BleReadRssiResponse() {
            @Override
            public void onResponse(int code, Integer data) {
                if (code != -1) {
                    rssi = data;
                }
            }
        });
        return rssi;
    }

    /**
     * 写出数据
     *
     * @author huangyajun
     * @date 2022/4/28 15:29
     */
    public void write(final byte[] data) {

        // 如果不是连接中就不允许发数据
        if (connectStatus != STATUS_CONNECTED) {
            return;
        }

        final BleWriteResponse response = new BleWriteResponse() {
            @Override
            public void onResponse(int code) {
                if (code == REQUEST_SUCCESS) {
                    mState = State.Leisure;
                    Log.e("--", "send OK");
                }
                if (code == REQUEST_FAILED) {
                    mState = State.Leisure;
                    Log.e("--", "send Error");
                }

            }
        };
        // 阻止频繁写入
        int retry = 5;
        while ((mState != State.Leisure) && (retry > 0)) {
            try {
                retry -= 1;
                Log.e("--", "bluetooth busy");
                Thread.sleep(20);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        Log.e("--", "bluetooth idle");
        mState = State.Work;
        mClient.write(mac, UUID_SERVICE, UUID_SEND, data, response);
        Log.e("--", String.format(" L:%d S: %2x %2x %2x %2x %2x", data.length, data[0], data[1], data[2], data[3], data[4]));
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
        observerServer.notifyObserver(data);
    }

    public String getMac() {
        return mac;
    }

    public void setMac(String mac) {
        this.mac = mac;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    // 连接状态监听
    public void registerConnectStatusListener(String mac, BleConnectStatusListener listener){
        mClient.registerConnectStatusListener(mac, listener);
    }

    public void unregisterConnectStatusListener(String mac, BleConnectStatusListener listener){
        mClient.unregisterConnectStatusListener(mac, listener);
    }

}
