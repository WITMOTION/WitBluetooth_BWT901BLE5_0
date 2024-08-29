package com.wit.witsdk.sensor.modular.device;

import com.wit.witsdk.sensor.dkey.ByteKey;
import com.wit.witsdk.sensor.dkey.DoubleKey;
import com.wit.witsdk.sensor.dkey.FloatKey;
import com.wit.witsdk.sensor.dkey.ShortKey;
import com.wit.witsdk.sensor.dkey.StringKey;
import com.wit.witsdk.sensor.modular.device.interfaces.IKeyUpdateObserver;
import com.wit.witsdk.sensor.modular.device.interfaces.IKeyUpdateObserverable;
import com.wit.witsdk.sensor.modular.device.interfaces.IListenKeyUpdateObserver;
import com.wit.witsdk.sensor.modular.device.interfaces.IListenKeyUpdateObserverable;
import com.wit.witsdk.sensor.modular.device.interfaces.impl.KeyUpdateObserverServer;
import com.wit.witsdk.sensor.modular.device.interfaces.impl.ListenKeyUpdateObserverServer;

import java.util.HashMap;
import java.util.Map;

/**
 * 设备数据源操作类
 *
 * @author huangyajun
 * @date 2022/5/20 18:44
 */
public class DeviceDataSource implements IKeyUpdateObserverable, IListenKeyUpdateObserverable {

    /**
     * 设备地址Key值
     */
    public final static String ADDR_KEY = "ADDR";

    /**
     * 设备数据
     */
    private Map<String, Object> deviceData = new HashMap<>();

    /**
     * key改变时
     */
    private KeyUpdateObserverServer onKeyUpdate = new KeyUpdateObserverServer();

    /**
     * 监听key改变时
     */
    private ListenKeyUpdateObserverServer onListenKeyUpdate = new ListenKeyUpdateObserverServer();


    /**
     * 设置设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void put(String key, Object value) {
        deviceData.put(key, value);
        if (this instanceof DeviceModel) {
            // Key刷新通知
            notifyKeyUpdateObserver((DeviceModel) this, key, value);
        }
    }

    /**
     * 设置设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(String key, String value) {
        put(key, (Object) value);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public String getDeviceData(String key) {
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            return o.toString();
        } else {
            return null;
        }
    }

    /**
     * 设置设备数据(字符串)
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(StringKey key, String value) {
        put(key.getKey(), value);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public String getDeviceData(StringKey dataKey) {
        String key = dataKey.getKey();
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            if (o instanceof String) {
                return (String) o;
            }
            return null;
        } else {
            return null;
        }
    }

    /**
     * 设置设备数据(浮点型)
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(FloatKey key, Float value) {
        put(key.getKey(), value);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public Float getDeviceData(FloatKey dataKey) {
        String key = dataKey.getKey();
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            if (o instanceof Float) {
                return (Float) o;
            }
            return null;
        } else {
            return null;
        }
    }

    /**
     * 设置设备数据(双精度浮点型)
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(DoubleKey key, double value) {
        put(key.getKey(), value);
    }


    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public Double getDeviceData(DoubleKey dataKey) {
        String key = dataKey.getKey();
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            if (o instanceof Double) {
                return (Double) o;
            }
            return null;
        } else {
            return null;
        }
    }

    /**
     * 设置设备数据(短整型)
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(ShortKey key, short value) {
        put(key.getKey(), value);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public Short getDeviceData(ShortKey dataKey) {
        String key = dataKey.getKey();
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            if (o instanceof Short) {
                return (Short) o;
            }
            return null;
        } else {
            return null;
        }
    }

    /**
     * 设置设备数据(短整型)
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public void setDeviceData(ByteKey key, byte value) {
        put(key.getKey(), value);
    }

    /**
     * 获得设备数据
     *
     * @author huangyajun
     * @date 2022/4/25 15:12
     */
    public Byte getDeviceData(ByteKey dataKey) {
        String key = dataKey.getKey();
        if (deviceData.containsKey(key)) {
            Object o = deviceData.get(key);
            if (o instanceof Byte) {
                return (Byte) o;
            }
            return null;
        } else {
            return null;
        }
    }

    /**
     * 设置设备地址
     *
     * @author huangyajun
     * @date 2022/4/25 15:17
     */
    public void setAddr(String addr) {
        setDeviceData(ADDR_KEY, addr);
    }

    /**
     * 获得地址
     *
     * @author huangyajun
     * @date 2022/4/25 15:17
     */
    public String getAddr() {
        return getDeviceData(ADDR_KEY);
    }


    @Override
    public void registerKeyUpdateObserver(IKeyUpdateObserver o) {
        onKeyUpdate.registerKeyUpdateObserver(o);
    }

    @Override
    public void removeKeyUpdateObserver(IKeyUpdateObserver o) {
        onKeyUpdate.removeKeyUpdateObserver(o);
    }

    @Override
    public void notifyKeyUpdateObserver(DeviceModel deviceModel, String key, Object value) {
        onKeyUpdate.notifyKeyUpdateObserver(deviceModel, key, value);
    }

    @Override
    public void registerListenKeyUpdateObserver(IListenKeyUpdateObserver o) {
        onListenKeyUpdate.registerListenKeyUpdateObserver(o);
    }

    @Override
    public void removeListenKeyUpdateObserver(IListenKeyUpdateObserver o) {
        onListenKeyUpdate.removeListenKeyUpdateObserver(o);
    }

    @Override
    public void notifyListenKeyUpdateObserver(DeviceModel deviceModel) {
        onListenKeyUpdate.notifyListenKeyUpdateObserver(deviceModel);
    }
}
