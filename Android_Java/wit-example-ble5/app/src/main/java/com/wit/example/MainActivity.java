package com.wit.example;

import androidx.appcompat.app.AppCompatActivity;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Switch;
import android.widget.Toast;

import com.wit.example.UI.CustomAdapter;
import com.wit.example.UI.ListItem;
import com.wit.witsdk.Bluetooth.WitBluetoothManager;
import com.wit.witsdk.Device.DeviceManager;
import com.wit.witsdk.Device.DeviceModel;
import com.wit.witsdk.Device.Interface.DeviceDataListener;
import com.wit.witsdk.Device.Interface.DeviceFindListener;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Timer;
import java.util.TimerTask;

/**
 * 示例主界面，展示搜索列表和传感器数据
 * Example main interface, displaying search list and sensor data
 * */
public class MainActivity extends AppCompatActivity implements DeviceDataListener, DeviceFindListener {
    // region 属性字段 attribute field
    // Wit日志 Wit logs
    private static final String TAG = "WitLOG";

    // 找到的设备列表 List of devices found
    private final List<ListItem> findList = new ArrayList<>();

    // 数据刷新定时器 Data Refresh Timer
    private Timer timer;

    // 自定义ListView适配器 ListView adapter
    private CustomAdapter customAdapter;

    // 设备管理器 Device Manager
    private final DeviceManager deviceManager = DeviceManager.getInstance();
    // endregion

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // 申请蓝牙及定位权限 Apply for Bluetooth and location permissions
        WitBluetoothManager.requestPermissions(this);

        // 搜索开关 Search switch
        @SuppressLint("UseSwitchCompatOrMaterialCode")
        Switch swi = findViewById(R.id.switch1);
        swi.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                StartScan();
            } else {
                StopScan();
            }
        });

        // 搜索列表 search list
        ListView listView = findViewById(R.id.scanlist);
        customAdapter = new CustomAdapter(this, findList);
        listView.setAdapter(customAdapter);
        // 跳转设备配置 Jump device configuration
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener(){
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                ListItem clickedItem = (ListItem) parent.getItemAtPosition(position);
                ShowDevice(clickedItem.getTitle());
            }
        });

        // 监听设备事件 Monitoring device events
        deviceManager.AddDeviceListener(this);
        deviceManager.AddDeviceFindListener(this);
    }

    @Override
    protected void onResume() {
        super.onResume();
        // 取消定时任务 Cancel scheduled tasks
        if(timer!=null){
            timer.cancel();
            timer = null;
        }
        startUpdate();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        // 取消定时任务 Cancel scheduled tasks
        if(timer!=null){
            timer.cancel();
            timer = null;
        }

        // 取消订阅设备事件 Unsubscribe device event
        deviceManager.RemoveDeviceListener(this);
        deviceManager.RemoveDeviceFindListener(this);
    }

    /**
     * 更新主界面数据
     * Update main interface data
     * */
    private void startUpdate(){
        timer = new Timer();
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
                for (ListItem lis : findList){
                    DeviceModel deviceModel = DeviceManager.getInstance().GetDevice(lis.getTitle());
                    lis.setData(deviceModel.GetDataDisplayLine());
                }
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        customAdapter.notifyDataSetChanged();
                    }
                });
            }
        }, 1000, 100);
    }

    /**
     * 开始搜索蓝牙
     * Start searching for Bluetooth
     * */
    private void StartScan(){
        // 清除现有设备 Clear existing devices
        findList.clear();
        ListView listView = findViewById(R.id.scanlist);
        CustomAdapter adapter = (CustomAdapter) listView.getAdapter();
        adapter.notifyDataSetChanged();
        DeviceManager.getInstance().CleanAllDevice();

        try {
            WitBluetoothManager witBluetoothManager = WitBluetoothManager.getInstance(this);
            witBluetoothManager.startScan();
        } catch (Exception e) {
            Log.e(TAG, "Start searching for anomalies：" + e.getMessage());
        }
    }

    /**
     * 结束搜索蓝牙
     * End search for Bluetooth
     * */
    private void StopScan(){
        try {
            WitBluetoothManager witBluetoothManager = WitBluetoothManager.getInstance(this);
            witBluetoothManager.stopScan();
        } catch (Exception e) {
            Log.e(TAG, "Search Error："+ e.getMessage());
        }
    }

    /**
     * 进入设备界面
     * Enter the device interface
     * */
    private void ShowDevice(String deviceName){
        // 跳转数据页面
        Intent intent = new Intent(this, DeviceActivity.class);
        intent.putExtra("DeviceName", deviceName);
        startActivity(intent);
    }

    /**
     * 找到设备时回调此方法
     * Call back this method when the device is found
     * */
    @SuppressLint("MissingPermission")
    @Override
    public void onDeviceFound(BluetoothDevice device) {
        String deviceName = device.getName();
        if(deviceName != null && deviceName.startsWith("WT")){
            String name = deviceName + "(" + device.getAddress() +")";
            for(ListItem item : findList){
                if(Objects.equals(item.getTitle(), name)){
                    return;
                }
            }
            DeviceModel deviceModel = new DeviceModel(name, device);
            deviceManager.AddDevice(name, deviceModel);
            findList.add(new ListItem(name, "data"));
            runOnUiThread(new Runnable() {
                @SuppressLint("SetTextI18n")
                @Override
                public void run() {
                    ListView listView = findViewById(R.id.scanlist);
                    CustomAdapter adapter = (CustomAdapter) listView.getAdapter();
                    adapter.notifyDataSetChanged();
                }
            });

            try {
                deviceModel.Connect(MainActivity.this);
            } catch (Exception e) {
                Log.e(TAG, "Connect Error：" + e.getMessage());
            }
        }
    }

    /**
     * 设备实时数据回调
     * Real time data callback for devices
     * */
    @Override
    public void OnReceive(String deviceName, String displayData) {

    }

    /**
     * 设备状态改变时
     * When the device status changes
     * */
    @Override
    public void OnStatusChange(String deviceName, boolean status) {
        if(status){
            Toast.makeText(MainActivity.this, deviceName + "  Connected", Toast.LENGTH_SHORT);
        }
        else {
            Toast.makeText(MainActivity.this, deviceName + "  Disconnect", Toast.LENGTH_SHORT);
        }
    }
}