package com.wit.witsdk.sensor.modular.connector.modular.bluetooth;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;
import android.content.Context;
import android.os.Build;
import android.util.Log;

import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.observer.interfaces.Observerable;
import com.wit.witsdk.observer.role.ObserverServer;

import java.util.UUID;


/**
 * 低功耗蓝牙连接
 *
 * @author huangyajun
 * @date 2022/5/24 19:10
 */
public class CustomBluetoothBLE implements Observerable {
    private final String TAG = "BlueBLE";
    private Context context;
    private String mac;
    private String name;

    // 发送uuid
    private UUID UUID_SEND = UUID.fromString("49535343-8841-43f4-a8d4-ecbe34729bb3");

    // 读取uuid
    private UUID UUID_READ = UUID.fromString("49535343-1e4d-4bd9-ba61-23c647249616");

    // 服务uuid
    private UUID UUID_SERVICE = UUID.fromString("49535343-fe7d-4ae5-8fa9-9fafd205e455");

    private BluetoothGatt mBluetoothGatt;

    private BluetoothGattCharacteristic mBluetoothGattCharacteristicWrite;

    private BluetoothGattCharacteristic mBluetoothGattCharacteristicNotify;

    // 当前是否连接状态
    private boolean isOpened = false;

    // 打开的时间戳
    private long openTimestamp = 0;


    // 观察服务
    private ObserverServer observerServer = new ObserverServer();

    private BluetoothGattCallback mBluetoothGattCallback = new BluetoothGattCallback() {

        @SuppressLint("MissingPermission")
        @Override
        public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {

            if (newState == BluetoothGatt.STATE_CONNECTED) {
                gatt.discoverServices();//四.连接蓝牙成功之后，发现服务
            } else if (newState == BluetoothGatt.STATE_DISCONNECTED) {
                isOpened = false;
                Log.e(TAG, "--->BluetoothGattService" + ":蓝牙断开了");
            }
            super.onConnectionStateChange(gatt, status, newState);
        }

        @Override
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {
            // 发现服务成功之后，去找需要的特征值
            if (status == BluetoothGatt.GATT_SUCCESS) {
                initService(gatt);
                // write(new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x00, (byte) 0xff, (byte) 0x00});
                // write(new byte[]{(byte) 0xff, (byte) 0xaa});
            }
            super.onServicesDiscovered(gatt, status);
        }

        /**
         * 初始化特征
         *
         * @author huangyajun
         * @date 2022/5/16 20:32
         */
        private void initService(BluetoothGatt gatt) {
            if (gatt == null) {
                return;
            }
            //遍历所有服务
            for (BluetoothGattService gattService : gatt.getServices()) {
                Log.e(TAG, "--->BluetoothGattService" + gattService.getUuid().toString());

                String inService = gattService.getUuid().toString();

                // 只找对应的服务
                if (UUID_SERVICE.toString().equals(gattService.getUuid().toString()) == false) {
                    continue;
                }

                //遍历所有特征
                for (BluetoothGattCharacteristic bluetoothGattCharacteristic : gattService.getCharacteristics()) {
                    Log.e("---->gattCharacteristic", bluetoothGattCharacteristic.getUuid().toString());

                    String str = bluetoothGattCharacteristic.getUuid().toString();
                    if (str.equals(UUID_SEND.toString())) {
                        // 根据写UUID找到写特征
                        mBluetoothGattCharacteristicWrite = bluetoothGattCharacteristic;
                        // 标识为连接的
                        isOpened = true;
                        openTimestamp = System.currentTimeMillis();
                    } else if (str.equals(UUID_READ.toString())) {
                        // 根据通知UUID找到通知特征
                        mBluetoothGattCharacteristicNotify = bluetoothGattCharacteristic;
                        // 开启读数据
                        enableNotification(gatt, mBluetoothGattCharacteristicNotify);
                        // 标识为连接的
                        isOpened = true;
                        openTimestamp = System.currentTimeMillis();
                    }
                }
            }
        }

        /**
         * 启用数据接收
         *
         * @author huangyajun
         * @date 2022/5/16 20:30
         */
        @SuppressLint("MissingPermission")
        private boolean enableNotification(BluetoothGatt gatt, BluetoothGattCharacteristic mBluetoothGattCharacteristicNotify) {
            // 来源：http://stackoverflow.com/questions/38045294/oncharacteristicchanged-not-called-with-ble

            boolean success = false;
            BluetoothGattCharacteristic characteristic = mBluetoothGattCharacteristicNotify;
            if (characteristic != null) {
                success = gatt.setCharacteristicNotification(characteristic, true);
                if (success) {
                    for (BluetoothGattDescriptor dp : characteristic.getDescriptors()) {
                        if (dp != null) {
                            if ((characteristic.getProperties() & BluetoothGattCharacteristic.PROPERTY_NOTIFY) != 0) {
                                dp.setValue(BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE);
                            } else if ((characteristic.getProperties() & BluetoothGattCharacteristic.PROPERTY_INDICATE) != 0) {
                                dp.setValue(BluetoothGattDescriptor.ENABLE_INDICATION_VALUE);
                            }
                            gatt.writeDescriptor(dp);
                        }
                    }
                }
            }
            return success;
        }

        /**
         * 读取蓝牙特征值时
         *
         * @author huangyajun
         * @date 2022/5/16 21:20
         */
        @Override
        public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
            super.onCharacteristicRead(gatt, characteristic, status);
            byte[] value = characteristic.getValue();
            //Log.i(TAG, "onCharacteristicRead" + ByteArrayConvert.ByteArrayToHexString(value));
        }

        /**
         *  写出数据给蓝牙时
         *
         * @author huangyajun
         * @date 2022/5/16 21:20
         */
        @Override
        public void onCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
            super.onCharacteristicWrite(gatt, characteristic, status);
            //Log.i(TAG, "onCharacteristicWrite status:" + status);
        }

        /**
         * 收到蓝牙数据时
         *
         * @author huangyajun
         * @date 2022/5/16 21:20
         */
        @Override
        public void onCharacteristicChanged(BluetoothGatt gatt, final BluetoothGattCharacteristic characteristic) {
            super.onCharacteristicChanged(gatt, characteristic);
            byte[] value = characteristic.getValue();
            // String s = ByteArrayConvert.ByteArrayToHexString(value);
            notifyObserver(value);
        }

    };

    /**
     * 构造
     *
     * @author huangyajun
     * @date 2022/5/16 18:58
     */
    @SuppressLint("MissingPermission")
    public CustomBluetoothBLE(Context context, String address, String name) {
        this.context = context;
        this.mac = address;
        this.name = name;
        final BluetoothAdapter mAdapter = WitBluetoothManager.getBAdapter(context);
        if (mAdapter == null) return;
        if (!mAdapter.isEnabled()) mAdapter.enable();
    }

    /**
     * 连接
     *
     * @author huangyajun
     * @date 2022/5/16 19:07
     */
    @SuppressLint("MissingPermission")
    public void connect(String mac) {
        this.setMac(mac);
        final BluetoothAdapter mAdapter =  WitBluetoothManager.getBAdapter(context);
        BluetoothDevice device = mAdapter.getRemoteDevice(this.mac);

        if (device != null && mBluetoothGatt == null) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                mBluetoothGatt = device.connectGatt(context, true, mBluetoothGattCallback, BluetoothDevice.TRANSPORT_LE);
//                mBluetoothGatt = device.connectGatt(context, true, mBluetoothGattCallback);
            } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                mBluetoothGatt = device.connectGatt(context, true, mBluetoothGattCallback, BluetoothDevice.TRANSPORT_LE);
//                mBluetoothGatt = device.connectGatt(context, true, mBluetoothGattCallback);
            } else {
                mBluetoothGatt = device.connectGatt(context, true, mBluetoothGattCallback);
            }
        }
    }

    /**
     * 断开连接
     *
     * @author huangyajun
     * @date 2022/5/16 19:07
     */
    @SuppressLint("MissingPermission")
    public void disconnect() {
        if (mBluetoothGatt != null && observerServer.observerSize() == 0) {
            mBluetoothGatt.disconnect();
            mBluetoothGatt = null;
        }
    }

    /**
     * 写出数据
     *
     * @author huangyajun
     * @date 2022/5/16 19:25
     */
    @SuppressLint("MissingPermission")
    public void write(final byte[] data) {

        // 连接1秒后才能发送信息
        if (System.currentTimeMillis() - openTimestamp < 1000 || !isOpened) {
            return;
        }

        if (mBluetoothGatt != null && mBluetoothGattCharacteristicWrite != null) {
            mBluetoothGattCharacteristicWrite.setValue(data);
            // mBluetoothGatt.setCharacteristicNotification(mBluetoothGattCharacteristicNotify, true);
            mBluetoothGatt.writeCharacteristic(mBluetoothGattCharacteristicWrite);
        }
    }

    public void setUUID(String strService, String strSend, String strRead) {
        UUID_SERVICE = UUID.fromString(strService);
        UUID_SEND = UUID.fromString(strSend);
        UUID_READ = UUID.fromString(strRead);
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
}
