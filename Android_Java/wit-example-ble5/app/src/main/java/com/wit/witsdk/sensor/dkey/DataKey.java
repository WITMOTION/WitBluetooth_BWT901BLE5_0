package com.wit.witsdk.sensor.dkey;

/**
 * 数据键值
 */
public abstract class DataKey {
    private String key ="";

    private String name = "";

    private String unit = "";

    public DataKey(String key) {
        this.key = key;
    }

    public DataKey(String key, String name, String unit) {
        this.key = key;
        this.name = name;
        this.unit = unit;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getUnit() {
        return unit;
    }

    public void setUnit(String unit) {
        this.unit = unit;
    }
}
