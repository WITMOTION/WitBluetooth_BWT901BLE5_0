package com.wit.witsdk.sensor.modular.connector.modular.bluetooth;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.location.LocationManager;
import android.os.Build;
import android.provider.Settings;
import android.util.Log;
import android.widget.Toast;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.constant.BleUUID;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.constant.DualUUID;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.exceptions.BluetoothBLEException;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IBluetoothFoundObserverable;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IUpdateRssiObserver;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.IUpdateRssiObserverable;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.impl.BluetoothFoundObserverable;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.interfaces.impl.UpdateRssiObserverable;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

/**
 * 自定义的蓝牙管理器
 *
 * @Author haungyajun
 * @Date 2022/4/28 17:31 （可以根据需要修改）
 */
public class WitBluetoothManager implements IBluetoothFoundObserverable, IUpdateRssiObserverable {

    /**
     * 蓝牙名称白名单
     */
    public static List<String> DeviceNameFilter = new ArrayList<>();

    /**
     * 单例实例
     */
    private static WitBluetoothManager instance;

    /**
     * 信号值列表
     */
    private static Map<String, Integer> rssiMap = new HashMap<>();

    /**
     * 日志标签
     */
    private static final String TAG = "WitBlue";

    private static final int ACCESS_PERMISSION = 1001;

    private static final int GPS_REQUEST_CODE = 1;

    private static final int BT_REQUEST_CODE = 2;

    // 蓝牙适配器
    public BluetoothAdapter mBtAdapter;

    // 上下文
    public Activity activity;

    // 找到的低功耗蓝牙
    private Map<String, BluetoothBLE> bluetoothBLEMap = new HashMap<>();

    // 找到的经典蓝牙
    private Map<String, BluetoothSPP> bluetoothSPPMap = new HashMap<>();

    // 蓝牙状态改变的被观察者
    private BluetoothFoundObserverable bluetoothBLEObserverable = new BluetoothFoundObserverable();

    // 蓝牙信号改变被观察者
    private UpdateRssiObserverable updateRssiObserverable = new UpdateRssiObserverable();

    /**
     * 初始化实例
     *
     * @author huangyajun
     * @date 2022/4/27 8:52
     */
    public static void initInstance(Context ctx) throws Exception {

        // 如果还没有申请权限就报错
        if (checkPermissions(ctx) == false) {
            throw new Exception("蓝牙管理器无法工作，缺少权限");
        }

        if (instance == null) {
            instance = new WitBluetoothManager(ctx);
        }
    }

    /**
     * 获得实例
     *
     * @author huangyajun
     * @date 2022/4/27 8:52
     */
    public static WitBluetoothManager getInstance() throws BluetoothBLEException {
        if (instance == null) {
            throw new BluetoothBLEException("无法获得实例，未初始化蓝牙管理器");
        } else {
            return instance;
        }
    }

    /**
     * 申请蓝牙权限
     *
     * @author huangyajun
     * @date 2022/10/13 16:42
     */
    public static void requestPermissions(Activity activity) {

        List<String> permList = new ArrayList<>();

        // 安卓6以下
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.M) {
            if (ActivityCompat.checkSelfPermission(activity,
                    Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED
                    || ActivityCompat.checkSelfPermission(activity,
                    Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                String[] strings =
                        {Manifest.permission.ACCESS_FINE_LOCATION, Manifest.permission.ACCESS_COARSE_LOCATION};
                ActivityCompat.requestPermissions(activity, strings, 1);
            }
        } else {
            // 安卓6以上
            // 申请蓝牙和定位权限
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                // 申请定位权限
                permList.add(Manifest.permission.ACCESS_COARSE_LOCATION);
                permList.add(Manifest.permission.ACCESS_FINE_LOCATION);
            }
            // 安卓12
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                // 申请蓝牙权限
                permList.add(Manifest.permission.BLUETOOTH_SCAN);
                permList.add(Manifest.permission.BLUETOOTH_CONNECT);
                permList.add(Manifest.permission.BLUETOOTH_ADVERTISE);
            }

            // 申请权限
            activity.requestPermissions(permList.toArray(new String[0]), ACCESS_PERMISSION);
        }
    }

    /**
     * 申请蓝牙权限
     *
     * @author huangyajun
     * @date 2022/10/13 16:42
     */
    public static boolean checkPermissions(Context context) {
        boolean result;
        // 检查定位
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            result = ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED;
            if (result == false) return false;
            result = ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_COARSE_LOCATION) == PackageManager.PERMISSION_GRANTED;
            if (result == false) return false;
        }

        // 检查蓝牙
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            result = ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_SCAN) == PackageManager.PERMISSION_GRANTED;
            if (result == false) return false;
            result = ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_CONNECT) == PackageManager.PERMISSION_GRANTED;
            if (result == false) return false;
            // result = ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_ADVERTISE) == PackageManager.PERMISSION_GRANTED;
            // if (result == false) return false;
        }
        return true;
    }

    /**
     * 构造函数
     *
     * @author huangyajun
     * @date 2022/5/5 17:09
     */
    public WitBluetoothManager(Context context) {
        //
        this.activity = (Activity) context;
        // 得到蓝牙适配器
        mBtAdapter = getBAdapter(context);

        // 创建更新信号的线程
        Thread thread = new Thread(this::updateRssiThread);
        thread.start();
    }

    /**
     * 获得蓝牙适配器
     *
     * @author huangyajun
     * @date 2023/2/28 17:32
     */
    public static BluetoothAdapter getBAdapter(Context context) {
        BluetoothAdapter mBluetoothAdapter = null;
        try {
            mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        } catch (Throwable t) {
            t.printStackTrace();
        }
        return mBluetoothAdapter;
    }

    /**
     * 开始扫描
     *
     * @author huangyajun
     * @date 2022/5/5 15:16
     */
    @SuppressLint("MissingPermission")
    public void startDiscovery() {
        // 检查权限
        checkHardware();

        // 注册广播接收器
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        activity.registerReceiver(mReceiver, filter);
        filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
        activity.registerReceiver(mReceiver, filter);


        int delayMs = 10;
        try {
            // 如果正在扫描就关闭扫描再重新开始
            if (mBtAdapter.isDiscovering()) {
                mBtAdapter.cancelDiscovery();
                delayMs = 1000;
            }

            int finalDelayMs = delayMs;
            Thread thread = new Thread(() -> {
                try {
                    Thread.sleep(finalDelayMs);
                    boolean b = mBtAdapter.startDiscovery();
                    if (b) {
                        Log.i(TAG, "开始扫描蓝牙成功");
                    } else {
                        Log.e(TAG, "开始扫描蓝牙失败");
                    }
                } catch (Exception err) {
                    Log.e(TAG, err.toString());
                }
            });
            thread.start();


        } catch (Exception err) {
            Log.e(TAG, err.toString());
        }
    }

    /**
     * 结束扫描
     *
     * @author huangyajun
     * @date 2022/5/5 9:21
     */
    @SuppressLint("MissingPermission")
    public void stopDiscovery() {
        // 
        if (mBtAdapter != null) {
            mBtAdapter.cancelDiscovery();
        }

        try {
            // 取消接收广播
            activity.unregisterReceiver(mReceiver);
        } catch (Exception e) {

        }
    }

    /**
     * 广播接收器
     *
     * @author huangyajun
     * @date 2022/5/5 17:33
     */
    @SuppressLint("MissingPermission")
    public final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

                // 名称过滤
                if (doNameFilter(device) == false) {
                    return;
                }

                // 更新信号
                int rssi = updateRssi(intent, device);

                int type;
                try {
                    type = device.getType();
                } catch (Exception err) {
                    Log.e(TAG, err.toString());
                    return;
                }

                // 经典蓝牙
                if (type == BluetoothDevice.DEVICE_TYPE_CLASSIC) {
                    // 通知已经找到了设备
                    BluetoothSPP bluetoothSPP = new BluetoothSPP(activity, device.getAddress(), device.getName());
                    bluetoothSPP.setRssi(rssi);
                    if (!bluetoothSPPMap.containsKey(device.getAddress())) {
                        bluetoothSPPMap.put(device.getAddress(), bluetoothSPP);
                    }
                    foundSPP(bluetoothSPP);

                    // 低功耗蓝牙
                } else if (type == BluetoothDevice.DEVICE_TYPE_LE) {
                    // 通知已经找到了设备
                    BluetoothBLE bluetoothBLE = new BluetoothBLE(activity, device.getAddress(), device.getName());
                    bluetoothBLE.setUUID(BleUUID.UUID_SERVICE.toString(), BleUUID.UUID_SEND.toString(), BleUUID.UUID_READ.toString());
                    bluetoothBLE.setRssi(rssi);
                    if (!bluetoothBLEMap.containsKey(device.getAddress())) {
                        bluetoothBLEMap.put(device.getAddress(), bluetoothBLE);
                    }
                    foundBLE(bluetoothBLE);
                    // 双模蓝牙
                } else if (type == BluetoothDevice.DEVICE_TYPE_DUAL) {

                    BluetoothBLE bluetoothBLE = new BluetoothBLE(activity, device.getAddress(), device.getName());
                    bluetoothBLE.setUUID(DualUUID.UUID_SERVICE.toString(), DualUUID.UUID_SEND.toString(), DualUUID.UUID_READ.toString());
                    bluetoothBLE.setRssi(rssi);

                    if (!bluetoothBLEMap.containsKey(device.getAddress())) {
                        bluetoothBLEMap.put(device.getAddress(), bluetoothBLE);
                    }
                    foundDual(bluetoothBLE);
                }
            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                System.out.println("搜索结束");
            }
        }
    };


    /**
     * 创建更新蓝牙信号的线程
     *
     * @author huangyajun
     * @date 2023/2/28 16:57
     */

    private void updateRssiThread() {
        // 1 秒读取一次信号
        while (true) {
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }

            Set<String> macBleSet = bluetoothBLEMap.keySet();
            for (String mac : macBleSet) {
                try {
                    BluetoothBLE bluetoothBLE = bluetoothBLEMap.get(mac);
                    //int rssi = bluetoothBLE.getRssi();
                    //updateRssi(mac, rssi);
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }

//            Set<String> macSppSet = bluetoothSPPMap.keySet();
//            for (String mac : macSppSet) {
//                try {
//                    BluetoothSPP bluetoothSPP = bluetoothSPPMap.get(mac);
//                    int rssi = bluetoothSPP.getRssi();
//                    onUpdateRssi(mac, rssi);
//                } catch (Exception e) {
//                    e.printStackTrace();
//                }
//            }
        }
    }

    /**
     * 名称过滤
     *
     * @author huangyajun
     * @date 2023/2/28 15:51
     */
    @SuppressLint("MissingPermission")
    private boolean doNameFilter(BluetoothDevice device) {

        boolean bNameCheck = false;
        String strName = "";
        try {
            strName = device.getName();
        } catch (Exception err) {
            Log.e(TAG, err.toString());
        }

        for (int i = 0; i < DeviceNameFilter.size(); i++) {
            if (strName == null) break;
            if (strName.indexOf(DeviceNameFilter.get(i)) != -1) {
                bNameCheck = true;
                break;
            }
        }

        return bNameCheck;
    }

    /**
     * 更新蓝牙信号
     *
     * @author huangyajun
     * @date 2023/2/28 15:49
     */
    private int updateRssi(Intent intent, BluetoothDevice device) {
        int rssi = intent.getExtras().getShort(BluetoothDevice.EXTRA_RSSI);
        String mac = device.getAddress();
        updateRssi(mac, rssi);
        return rssi;
    }

    /**
     * 更新信号
     *
     * @author huangyajun
     * @date 2023/2/28 18:24
     */
    private void updateRssi(String mac, int rssi) {
        if (rssiMap.containsKey(mac)) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                rssiMap.replace(mac, rssi);
            } else {
                rssiMap.put(mac, rssi);
            }
        } else {
            rssiMap.put(mac, rssi);
        }

        System.out.println("更新信号：" + mac + ": " + rssi);
        // 通知观察者
        onUpdateRssi(mac, rssi);
    }


    /**
     * 是否在扫描中
     *
     * @author huangyajun
     * @date 2022/4/28 17:36
     */
    @SuppressLint("MissingPermission")
    public boolean isDiscovering() {
        return mBtAdapter.isDiscovering();
    }

    /**
     * 得到一个低功耗蓝牙设备的连接
     *
     * @author huangyajun
     * @date 2022/5/5 20:49
     */
    public BluetoothBLE getBluetoothBLE(String mac) {
        if (bluetoothBLEMap.containsKey(mac)) {
            return bluetoothBLEMap.get(mac);
        }
        return null;
    }

    /**
     * 得到一个经典蓝牙设备的连接
     *
     * @author huangyajun
     * @date 2022/5/5 20:49
     */
    public BluetoothSPP getBluetoothSPP(String mac) {

        if (bluetoothSPPMap.containsKey(mac)) {
            return bluetoothSPPMap.get(mac);
        }
        return null;
    }

    /**
     * 检查蓝牙和gps是不是打开了,没打开就申请打开
     *
     * @author huangyajun
     * @date 2022/5/5 17:38
     */
    @SuppressLint("MissingPermission")
    private void checkHardware() {
        if (checkPermissions(activity) == false) return;

        // 申请打开定位
        LocationManager locationManager = (LocationManager) activity.getSystemService(Context.LOCATION_SERVICE);
        if (locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER) == false) {
            Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
            activity.startActivityForResult(intent, GPS_REQUEST_CODE);
        }

        mBtAdapter = BluetoothAdapter.getDefaultAdapter();
        if (mBtAdapter == null) {
            Toast.makeText(activity, "您的设备不支持蓝牙连接", Toast.LENGTH_LONG).show();
            return;
        }

        // 如果蓝牙没打开就申请打开蓝牙
        if (!mBtAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            activity.startActivityForResult(enableBtIntent, BT_REQUEST_CODE);
        }
    }

    @Override
    public void registerObserver(IBluetoothFoundObserver o) {
        bluetoothBLEObserverable.registerObserver(o);
    }

    @Override
    public void removeObserver(IBluetoothFoundObserver o) {
        bluetoothBLEObserverable.removeObserver(o);
    }

    @Override
    public void foundBLE(BluetoothBLE bluetoothBLE) {
        bluetoothBLEObserverable.foundBLE(bluetoothBLE);
    }

    @Override
    public void foundSPP(BluetoothSPP bluetoothSPP) {
        bluetoothBLEObserverable.foundSPP(bluetoothSPP);
    }

    @Override
    public void foundDual(BluetoothBLE bluetoothBLE) {
        bluetoothBLEObserverable.foundDual(bluetoothBLE);
    }

    /**
     * 获得蓝牙信号值
     *
     * @author huangyajun
     * @date 2022/5/24 19:09
     */
    public static Integer getRssi(String mac) {
        return rssiMap.get(mac);
    }

    @Override
    public void registerObserver(IUpdateRssiObserver o) {
        updateRssiObserverable.registerObserver(o);
    }

    @Override
    public void removeObserver(IUpdateRssiObserver o) {
        updateRssiObserverable.removeObserver(o);
    }

    @Override
    public void onUpdateRssi(String mac, int rssi) {
        updateRssiObserverable.onUpdateRssi(mac, rssi);
    }
}
