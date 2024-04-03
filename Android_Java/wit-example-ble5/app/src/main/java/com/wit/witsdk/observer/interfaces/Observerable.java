package com.wit.witsdk.observer.interfaces;

/**
 * 观察对象
 *
 * @author huangyajun
 * @date 2022/4/26 11:28
 */
public interface Observerable {

    /**
     * 添加观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void registerObserver(Observer o);

    /**
     * 删除观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void removeObserver(Observer o);

    /**
     * 通知观察者
     *
     * @author huangyajun
     * @date 2022/4/26 13:42
     */
    void notifyObserver(byte[] data);
}
