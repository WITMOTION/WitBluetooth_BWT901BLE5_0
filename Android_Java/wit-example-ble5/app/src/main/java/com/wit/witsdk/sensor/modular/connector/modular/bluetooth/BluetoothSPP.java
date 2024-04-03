package com.wit.witsdk.sensor.modular.connector.modular.bluetooth;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

import java.util.Date;

import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.observer.interfaces.Observerable;
import com.wit.witsdk.observer.role.ObserverServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Arrays;
import java.util.UUID;

/**
 * 经典蓝牙连接
 *
 * @author huangyajun
 * @date 2022/6/21 13:42
 */
public class BluetoothSPP implements Observerable {
    //
//    private static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
    private static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB"); // 新uuid
    private static final String NAME = "BluetoothData";
    private final BluetoothAdapter mAdapter;
    private String mac;
    private String name;

    private final Context context;
    private AcceptThread mAcceptThread;// 请求连接的监听进程
    private ConnectThread mConnectThread;// 连接一个设备的进程
    public ConnectedThread mConnectedThread;// 已经连接之后的管理进程
    private int mState;// 当前状态
    public static final int STATE_NONE = 0;
    public static final int STATE_LISTEN = 1;
    public static final int STATE_CONNECTING = 2;
    public static final int STATE_CONNECTED = 3;
    public static final int MESSAGE_STATE_CHANGE = 1;
    public static final int MESSAGE_READ = 2;
    public static final int MESSAGE_WRITE = 3;
    public static final int MESSAGE_DEVICE_NAME = 4;
    public static final int MESSAGE_CONNECT_FAILED = 5;
    public static final int MESSAGE_DEVICE_LOST = 6;
    // 观察服务
    private ObserverServer observerServer = new ObserverServer();
    // 重新连接锁
    private Object reconnectLock = new Object();
    private Object startReconnectLock = new Object();
    private long reConnectTime = -1;
    private Thread reconnectThread = null;

    private int rssi;

    @SuppressLint("HandlerLeak")
    private final Handler mHandler =
            new Handler() {
                @Override        // 匿名内部类写法，实现接口Handler的一些
                public void handleMessage(Message msg) {
                    switch (msg.what) {
                        case BluetoothSPP.MESSAGE_STATE_CHANGE:
                            switch (msg.arg1) {
                                case BluetoothSPP.STATE_CONNECTED:
                                    Log.d("", "连接蓝牙成功");
                                    break;
                                case BluetoothSPP.STATE_CONNECTING:
                                    Log.d("", "连接蓝牙中");

                                    break;
                                case BluetoothSPP.STATE_LISTEN:
                                case BluetoothSPP.STATE_NONE:
//                                    Log.d("", "无蓝牙连接");
                                    break;
                            }
                            break;
                        case BluetoothSPP.MESSAGE_READ:
                            break;
                        case BluetoothSPP.MESSAGE_DEVICE_NAME:
                            String mConnectedDeviceName = msg.getData().getString("device_name");
                            //Toast.makeText(context, "getString(R.string.connect_to)+ mConnectedDeviceName", Toast.LENGTH_SHORT).show();
                            break;
                        case BluetoothSPP.MESSAGE_DEVICE_LOST:
                            //Toast.makeText(context, "getString(R.string.device_lost)", Toast.LENGTH_SHORT).show();
                            break;
                        case BluetoothSPP.MESSAGE_CONNECT_FAILED:
                            //Toast.makeText(context, "getString(R.string.cant_connect)", Toast.LENGTH_SHORT).show();
                            break;
                    }
                }
            };

    @SuppressLint("MissingPermission")
    public BluetoothSPP(Context contextIn) {
        this.context = contextIn;
        mState = STATE_NONE;
        mAdapter = BluetoothAdapter.getDefaultAdapter();
        if (mAdapter == null) return;
        if (!mAdapter.isEnabled()) mAdapter.enable();
    }

    public BluetoothSPP(Context context, String mac, String name) {
        this(context);
        this.setName(name);
        this.setMac(mac);
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

    /**
     * 写出数据
     *
     * @author huangyajun
     * @date 2022/4/28 15:32
     */
    public void write(byte[] buffer) {
        if (mState == STATE_CONNECTED)
            mConnectedThread.write(buffer);
    }

    /**
     * 设置状态
     *
     * @author huangyajun
     * @date 2022/4/28 15:33
     */
    private synchronized void setState(int state) {
        mState = state;
        Log.e("--", "state:" + state);
        mHandler.obtainMessage(MESSAGE_STATE_CHANGE, state, -1).sendToTarget();
    }

    /**
     * 获得状态
     *
     * @author huangyajun
     * @date 2022/4/28 15:33
     */
    public synchronized int getState() {
        return mState;
    }

    public synchronized void start() {
        // Cancel any thread attempting to make a connection
        if (mConnectThread != null) {
            mConnectThread.cancel();
            mConnectThread = null;
        }
        // Start the thread to listen on a BluetoothServerSocket
        if (mAcceptThread == null) {
            mAcceptThread = new AcceptThread();
            mAcceptThread.start();
        }
        setState(STATE_LISTEN);
    }

    /**
     * 连接蓝牙
     *
     * @author huangyajun
     * @date 2022/8/25 16:54
     */
    public synchronized void connect(String mac) {
        this.mac = mac;
        // Cancel any thread currently running a connection
        BluetoothDevice device = mAdapter.getRemoteDevice(mac);
        if (mConnectedThread != null) {
            mConnectedThread.cancel();
            mConnectedThread = null;
        }

        // Start the thread to connect with the given device
        mConnectThread = new ConnectThread(device);
        mConnectThread.start();
        setState(STATE_CONNECTING);
    }

    /**
     * 开始重新连接
     *
     * @author huangyajun
     * @date 2022/8/25 16:58
     */
    private void startReConnect() {

        synchronized (startReconnectLock) {
            long time = new Date().getTime();

            // 如果现在没有重新连接线程，就开始重新连接
            if (reconnectThread == null) {
                reConnectTime = time + 1000;

                // 开始执行重新连接线程
                reconnectThread = new Thread(() -> {
                    while (true) {
                        // 每100ms检测是不是到了重新连接时间
                        try {
                            Thread.sleep(100);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                        // 时间到了就执行重新连接
                        if (new Date().getTime() > reConnectTime) {
                            reConnect();
                            break;
                        }
                    }
                    reconnectThread = null;
                });
                reconnectThread.start();
            } else {
                // 如果现在有重新连接线程，就延后重新连接时间
                reConnectTime = time + 1000;
            }
        }
    }

    /**
     * 设置信号值
     */
    public void setRssi(int rssi){
        this.rssi = rssi;
    }

    /**
     * 读取信号
     *
     * @author huangyajun
     * @date 2023/2/28 18:16
     */
    public int getRssi() {
        return this.rssi;
    }

    /**
     * 重新连接
     *
     * @author huangyajun
     * @date 2022/8/25 16:54
     */
    public void reConnect() {
        // 线程锁，防止重复重新连接
        synchronized (reconnectLock) {
            // 关闭连接
            this.stop();
            // 等待100ms，重连太快会不生效
            try {
                Thread.sleep(200);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            // 重新连接
            this.connect(mac);
        }
    }

    @SuppressLint("MissingPermission")
    public synchronized void connected(BluetoothSocket socket, BluetoothDevice device) {
        // Cancel the thread that completed the connection
        if (mConnectThread != null) {
            mConnectThread.cancel();
            mConnectThread = null;
        }
        // Cancel the accept thread because we only want to connect to one device
        if (mAcceptThread != null) {
            mAcceptThread.cancel();
            mAcceptThread = null;
        }
        // Start the thread to manage the connection and perform transmissions
        mConnectedThread = new ConnectedThread(socket, this);
        mConnectedThread.start();
        // Send the name of the connected device back to the UI Activity
        Message msg = mHandler.obtainMessage(MESSAGE_DEVICE_NAME);
        Bundle bundle = new Bundle();
        bundle.putString("device_name", device.getName());
        msg.setData(bundle);
        mHandler.sendMessage(msg);
        setState(STATE_CONNECTED);
    }

    public synchronized void stop() {
        if (mConnectedThread != null) {
            mConnectedThread.cancel();
            mConnectedThread = null;
        }
        if (mAcceptThread != null) {
            mAcceptThread.cancel();
            mAcceptThread = null;
        }
        setState(STATE_NONE);
    }

    private void connectionFailed() {
        setState(STATE_LISTEN);
        Message msg = mHandler.obtainMessage(MESSAGE_CONNECT_FAILED);

        mHandler.sendMessage(msg);
    }

    private void connectionLost() {
        setState(STATE_LISTEN);
        Message msg = mHandler.obtainMessage(MESSAGE_DEVICE_LOST);
        mHandler.sendMessage(msg);
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


    /**
     * This thread runs while listening for incoming connections. It behaves
     * like a server-side client. It runs until a connection is accepted (or until cancelled).
     */
    private class AcceptThread extends Thread {
        // The local server socket
        private final BluetoothServerSocket mmServerSocket;

        @SuppressLint("MissingPermission")
        public AcceptThread() {
            BluetoothServerSocket tmp = null;
            try {
                tmp = mAdapter.listenUsingRfcommWithServiceRecord(NAME, MY_UUID);
            } catch (IOException e) {
            }
            mmServerSocket = tmp;
        }


        public void run() {
            setName("AcceptThread");
            BluetoothSocket socket = null;
            while (mState != STATE_CONNECTED) {
                try {
                    socket = mmServerSocket.accept();
                } catch (IOException e) {
                    break;
                }
                // If a connection was accepted
                if (socket != null) {
                    synchronized (BluetoothSPP.this) {
                        switch (mState) {
                            case STATE_LISTEN:
                            case STATE_CONNECTING:// Situation normal. Start the connected thread.
                                connected(socket, socket.getRemoteDevice());
                                break;
                            case STATE_NONE:
                            case STATE_CONNECTED:
                                try {
                                    socket.close();
                                } catch (IOException e) {
                                }
                                break;
                        }
                    }
                }
            }

        }

        public void cancel() {
            try {
                mmServerSocket.close();
            } catch (IOException e) {
            }
        }
    }

    private class ConnectThread extends Thread {
        private final BluetoothSocket mmSocket;
        private final BluetoothDevice mmDevice;

        @SuppressLint("MissingPermission")
        public ConnectThread(BluetoothDevice device) {
            mmDevice = device;
            BluetoothSocket tmp = null;
            try {
                tmp = device.createRfcommSocketToServiceRecord(MY_UUID);// Get a BluetoothSocket for a connection with the given BluetoothDevice
            } catch (IOException e) {
            }
            mmSocket = tmp;
        }

        @SuppressLint("MissingPermission")
        public void run() {
            setName("ConnectThread");
            mAdapter.cancelDiscovery();// Always cancel discovery because it will slow down a connection
            // Make a connection to the BluetoothSocket
            try {
                mmSocket.connect();// This is a blocking call and will only return on a successful connection or an exception
            } catch (IOException e) {
                connectionFailed();
                try {
                    mmSocket.close();
                } catch (IOException e2) {
                }

                BluetoothSPP.this.start();// 引用来说明要调用的是外部类的方法 run
                return;
            }
            synchronized (BluetoothSPP.this) {// Reset the ConnectThread because we're done
                mConnectThread = null;
            }
            connected(mmSocket, mmDevice);// Start the connected thread
        }

        public void cancel() {
            try {
                mmSocket.close();
            } catch (IOException e) {

            }
        }
    }

    /**
     * This thread runs during a connection with a remote device. It handles all
     * incoming and outgoing transmissions.
     */
    class ConnectedThread extends Thread {
        private final BluetoothSocket mmSocket;
        private final InputStream mmInStream;
        private final OutputStream mmOutStream;
        private BluetoothSPP mbluetoothSPP;

        public ConnectedThread(BluetoothSocket socket, BluetoothSPP bluetoothSPP) {
            mmSocket = socket;
            mbluetoothSPP = bluetoothSPP;

            InputStream tmpIn = null;
            OutputStream tmpOut = null;
            try {
                tmpIn = socket.getInputStream();
                tmpOut = socket.getOutputStream();
            } catch (IOException e) {
            }

            mmInStream = tmpIn;
            mmOutStream = tmpOut;
        }

        @Override
        public void run() {
            byte[] buffer = new byte[1024];
            int acceptedLen = 0;
            // Keep listening to the InputStream while connected
            while (true) {
                try {
                    acceptedLen = mmInStream.read(buffer);
                    if (acceptedLen > 0) {
                        notifyObserver(Arrays.copyOf(buffer, acceptedLen));
                    }
                } catch (IOException e) {
                    connectionLost();
                    break;
                }
            }
        }

        @SuppressLint("MissingPermission")
        public void write(byte[] buffer) {
            try {
                mmOutStream.write(buffer);
                mmOutStream.flush();
                mbluetoothSPP.startReConnect();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

        public void cancel() {
            try {
                mmSocket.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}


