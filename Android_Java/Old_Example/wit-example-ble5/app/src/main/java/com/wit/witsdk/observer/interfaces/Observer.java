package com.wit.witsdk.observer.interfaces;

/**
 * 观察者
 *
 * @author huangyajun
 * @date 2022/4/26 11:27
 */
public interface Observer {

    /**
     * 接收通知
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void update(byte[] data);
}
