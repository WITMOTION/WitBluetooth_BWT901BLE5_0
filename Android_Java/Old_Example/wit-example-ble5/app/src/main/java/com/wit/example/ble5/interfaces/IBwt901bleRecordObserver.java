package com.wit.example.ble5.interfaces;


import com.wit.example.ble5.Bwt901ble;

/**
 * 记录数据通知
 *
 * @author huangyajun
 * @date 2022/4/26 11:27
 */
public interface IBwt901bleRecordObserver {

    /**
     * 接收通知
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void onRecord(Bwt901ble bwt901ble);

}
