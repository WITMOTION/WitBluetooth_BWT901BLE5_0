package com.wit.witsdk.sensor.modular.device.entity;

/**
 * 数据解算配置类
 *
 * @author huangyajun
 * @date 2022/5/20 9:50
 */
public class CalcOption {

    /**
     * 名称
     */
    public String name;

    /**
     * 名称
     */
    public String key;

    /**
     * 名称
     */
    public String csScript;

    /**
     * 名称
     */
    public String suffix;

    /**
     * 名称
     */
    public String enableOffset;

    /**
     * 名称
     */
    public int sort;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public String getCsScript() {
        return csScript;
    }

    public void setCsScript(String csScript) {
        this.csScript = csScript;
    }

    public String getSuffix() {
        return suffix;
    }

    public void setSuffix(String suffix) {
        this.suffix = suffix;
    }

    public String getEnableOffset() {
        return enableOffset;
    }

    public void setEnableOffset(String enableOffset) {
        this.enableOffset = enableOffset;
    }

    public int getSort() {
        return sort;
    }

    public void setSort(int sort) {
        this.sort = sort;
    }
}
