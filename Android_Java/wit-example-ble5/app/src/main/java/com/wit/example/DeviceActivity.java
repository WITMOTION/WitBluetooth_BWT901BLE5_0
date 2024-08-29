package com.wit.example;

import androidx.appcompat.app.AppCompatActivity;

import android.app.AlertDialog;
import android.content.Intent;
import android.os.Bundle;
import android.text.InputType;
import android.util.Log;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.wit.witsdk.Device.DeviceManager;
import com.wit.witsdk.Device.DeviceModel;
import com.wit.witsdk.Device.Interface.DeviceDataListener;

import java.util.ArrayList;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

public class DeviceActivity extends AppCompatActivity implements DeviceDataListener {
    // Wit日志 Wit logs
    private static final String TAG = "WitLOG";

    // 设备模型 device model
    private DeviceModel deviceModel;

    // 设备管理器 Device Manager
    private DeviceManager deviceManager = DeviceManager.getInstance();

    // 设备数据视图文本 Device Data View
    private TextView dataView;

    // 定时器 Refresh data timer
    private Timer timer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_device);

        // 拿到设备模型 Get the device model
        Intent intent = getIntent();
        String deviceName = intent.getStringExtra("DeviceName");
        deviceModel = deviceManager.GetDevice(deviceName);

        // 设备名 Device name
        TextView textView = findViewById(R.id.textView);
        textView.setText(deviceName);

        // 设备数据视图文本 Device Data View
        dataView = findViewById(R.id.textView2);

        // 恢复出厂 Reset button
        Button btn = findViewById(R.id.button);
        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(deviceModel != null){
                    deviceModel.ReSet();
                }
            }
        });

        // 回传速率 Return rate button
        Button btn1 = findViewById(R.id.button1);
        btn1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                ArrayList<String> item = new ArrayList<>();
                item.add("1");
                item.add("5");
                item.add("10");
                item.add("50");
                item.add("100");
                item.add("200");
                SendDeviceDataByMsg((byte) 0x03, "retrieval rate", item);
            }
        });

        // 带宽 Bandwidth button
        Button btn2 = findViewById(R.id.button2);
        btn2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                ArrayList<String> item = new ArrayList<>();
                item.add("5");
                item.add("10");
                item.add("20");
                item.add("42");
                item.add("98");
                item.add("188");
                SendDeviceDataByMsg((byte) 0x1f, "bandwidth", item);
            }
        });

        // 角度参考 Angle reference button
        Button btn3 = findViewById(R.id.button3);
        btn3.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(deviceModel != null){
                    deviceModel.SetAngle0();
                }
            }
        });

        // 磁场校准 Magnetic field calibration button
        Button btn4 = findViewById(R.id.button4);
        btn4.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                MagCalibration(btn4);
            }
        });

        deviceManager.AddDeviceListener(this);
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
        deviceManager.RemoveDeviceListener(this);
    }

    /**
     * 开启数据刷新
     * Enable data refresh
     * */
    private void startUpdate(){
        if(deviceModel == null){
            return;
        }

        timer = new Timer();
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        dataView.setText(deviceModel.GetDataDisplay());
                    }
                });
            }
        }, 1000, 100);
    }

    /**
     * 发送传感器数据
     * Sending sensor data
     * */
    private void SendDeviceDataByMsg(byte reg, String msg, ArrayList<String> list){
        if(deviceModel == null){
            return;
        }

        Spinner spinner = new Spinner(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, list);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);

        // 创建AlertDialog并设置标题 Create AlertDialog and set the title
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setTitle(msg)
                .setView(spinner)
                .setPositiveButton("OK", null)  // 设置点击事件为null，稍后设置自定义监听器
                .setNegativeButton("Cancel", (dialogInterface, which) -> dialogInterface.cancel())
                .create();

        dialog.show();

        dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v -> {
            // 获取用户输入的文本 Retrieve the text input by the user
            String userInput = (String) spinner.getSelectedItem();
            if (!userInput.isEmpty()) {
                try {
                    int value = Integer.parseInt(userInput);
                    if(reg == 0x03){
                        deviceModel.SetRRate(value);
                    }
                    else if(reg == 0x1f){
                        deviceModel.SetBandwidth(value);
                    }
                }
                catch (Exception ex){
                    Log.i(TAG, "Error", ex);
                }
            }
            // close Dialog
            dialog.dismiss();
        });
    }

    /**
     * 磁场校准
     * Magnetic field calibration
     * */
    public void MagCalibration(Button btn){
        if(deviceModel == null){
            return;
        }
        if(btn.getText().equals(getString(R.string.magstart))){
            btn.setText(getString(R.string.magstop));
            deviceModel.SetMagStart();
        }
        else {
            btn.setText(getString(R.string.magstart));
            deviceModel.SetMagStop();
        }
    }

    /**
     * 传感器实时数据回调
     * Real time data callback of sensors
     * */
    @Override
    public void OnReceive(String deviceName, String displayData) {

    }

    /**
     * 传感器连接状态改变时
     * When the sensor connection status changes
     * */
    @Override
    public void OnStatusChange(String deviceName, boolean status) {
        if(status){
            Log.i(TAG, deviceName + "  Connected");
        }
        else {
            Log.i(TAG, deviceName + "   Disconnect");
        }
    }
}