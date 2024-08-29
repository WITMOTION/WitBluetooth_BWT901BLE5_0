package com.wit.witsdk.Bluetooth;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.le.BluetoothLeScanner;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
import android.bluetooth.le.ScanSettings;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.location.LocationManager;
import android.os.Build;
import android.os.Handler;
import android.provider.Settings;
import android.util.Log;

import androidx.core.content.ContextCompat;

import com.wit.witsdk.Device.DeviceManager;
import com.wit.witsdk.Device.DeviceModel;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

/**
 * WIT蓝牙管理器，控制蓝牙搜索
 * */
public class WitBluetoothManager {
    // region 属性字段
    // 管理器实例
    private static WitBluetoothManager instance;

    // 蓝牙适配器
    private BluetoothAdapter mBtAdapter;

    // 蓝牙搜索器
    private BluetoothLeScanner scanner;

    // 上下文
    private Activity activity;

    // 是否搜索中
    private boolean isScan = false;

    // 蓝牙搜索回调
    private ScanCallback callback;

    // 日志标识
    private static final String TAG = "WitLOG";

    // 功能码
    private static final int ACCESS_PERMISSION = 1001;
    private static final int GPS_REQUEST_CODE = 1;
    private static final int BT_REQUEST_CODE = 2;
    // endregion

    /**
     * 私有构造方法
     * */
    private WitBluetoothManager(Context context) {
        this.activity = (Activity) context;
        mBtAdapter = getBAdapter();
    }

    /**
     * 获得蓝牙适配器
     * */
    private static BluetoothAdapter getBAdapter() {
        BluetoothAdapter mBluetoothAdapter = null;
        try {
            mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        } catch (Throwable t) {
            t.printStackTrace();
        }
        return mBluetoothAdapter;
    }

    /**
     * 获得蓝牙管理器实例
     * */
    public static WitBluetoothManager getInstance(Context ctx) throws Exception {
        // 如果还没有申请权限就报错
        if (!checkPermissions(ctx)) {
            throw new Exception("Bluetooth Manager is not working and lacks permissions");
        }

        if (instance == null) {
            instance = new WitBluetoothManager(ctx);
        }
        return instance;
    }

    /**
     * 申请蓝牙权限
     * */
    public static void requestPermissions(Activity activity) {
        List<String> permList = new ArrayList<>();

        // 对于 Android 6.0 (API 23) 及以上版本，申请定位权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            permList.add(Manifest.permission.ACCESS_FINE_LOCATION);
        }

        // 对于 Android 9.0 (API 28) 及以下版本，申请粗略定位权限
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.P) {
            permList.add(Manifest.permission.ACCESS_COARSE_LOCATION);
        }

        // 对于 Android 12 (API 31) 及以上版本，申请新的蓝牙权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            permList.add(Manifest.permission.BLUETOOTH_SCAN);
            permList.add(Manifest.permission.BLUETOOTH_CONNECT);
        }

        // 对于 Android 11 (API 30) 及以下版本，申请旧的蓝牙权限
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.R) {
            permList.add(Manifest.permission.BLUETOOTH);
            permList.add(Manifest.permission.BLUETOOTH_ADMIN);
        }

        // 请求权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            activity.requestPermissions(permList.toArray(new String[0]), ACCESS_PERMISSION);
        }
    }

    /**
     * 检查蓝牙权限
     * */
    public static boolean checkPermissions(Context context) {
        // 检查定位权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
        }

        // 对于 Android 9.0 (API 28) 及以下版本，检查粗略定位权限
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.P) {
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
        }

        // 对于 Android 12 (API 31) 及以上版本，检查新的蓝牙权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_SCAN) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
        }

        // 对于 Android 11 (API 30) 及以下版本，检查旧的蓝牙权限
        if (Build.VERSION.SDK_INT <= Build.VERSION_CODES.R) {
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
            if (ContextCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_ADMIN) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
        }

        return true;
    }

    /**
     * 开始搜索
     * */
    @SuppressLint("MissingPermission")
    public void startScan(){
        checkHardware();
        if(scanner == null){
            scanner = mBtAdapter.getBluetoothLeScanner();
        }

        callback = new ScanCallback() {
            @Override
            public void onScanResult(int callbackType, ScanResult result) {
                // 得到设备
                BluetoothDevice device = result.getDevice();
                Log.i(TAG, "Find " + device.getName());
                DeviceManager.getInstance().FindDevice(device);
            }

            @Override
            public void onScanFailed(int errorCode) {
                super.onScanFailed(errorCode);
                Log.e(TAG, "Search Error" + errorCode);
            }
        };

        // 设置扫描模式 低延迟模式
        ScanSettings scanSettings = new ScanSettings.Builder()
                .setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY)
                .setReportDelay(0)
                .build();
        scanner.startScan(null, scanSettings, callback);
        isScan = true;
        Log.i(TAG, "Start scanning Bluetooth ---------------");

        // 搜索10秒后自动关闭扫描
        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                stopScan();
            }

        }, 10000);
    }

    /**
     * 停止搜索
     * */
    @SuppressLint("MissingPermission")
    public void stopScan(){
        if(!isScan || callback == null){
            return;
        }
        scanner.stopScan(callback);
        isScan = false;
        Log.i(TAG, "Stop scanning Bluetooth ---------------");
    }

    /**
     * 检查蓝牙和定位开关
     * */
    @SuppressLint("MissingPermission")
    private void checkHardware() {
        if (!checkPermissions(activity)) return;

        // 申请打开定位
        LocationManager locationManager = (LocationManager) activity.getSystemService(Context.LOCATION_SERVICE);
        if (!locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
            Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
            activity.startActivityForResult(intent, GPS_REQUEST_CODE);
        }

        mBtAdapter = BluetoothAdapter.getDefaultAdapter();
        if (mBtAdapter == null) {
            Log.e(TAG,"Your device does not support Bluetooth connection");
            return;
        }

        // 如果蓝牙没打开就申请打开蓝牙
        if (!mBtAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            activity.startActivityForResult(enableBtIntent, BT_REQUEST_CODE);
        }
    }
}
