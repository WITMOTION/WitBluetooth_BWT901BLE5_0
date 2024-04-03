package com.wit.witsdk.observer.role;

import com.wit.witsdk.observer.interfaces.Observer;
import com.wit.witsdk.observer.interfaces.Observerable;

import java.util.ArrayList;
import java.util.List;

public class ObserverServer implements Observerable {

    private List<Observer> list; //面向接口编程

    public ObserverServer() {
        list = new ArrayList<>();
    }

    @Override
    public void registerObserver(Observer observer) {
        list.add(observer);
    }

    @Override
    public void removeObserver(Observer observer) {
        if (!list.isEmpty()) {
            list.remove(observer);
        }
    }

    /**
     * 遍历
     *
     * @author huangyajun
     * @date 2022/4/26 14:21
     */
    @Override
    public void notifyObserver(byte[] data) {
        for (int i = 0; i < list.size(); i++) {
            Observer observer = list.get(i);
            observer.update(data);//通知Observer调用update方法
        }
    }

    /**
     * 现有观察者数量
     *
     * @author huangyajun
     * @date 2022/5/10 18:09
     */
    public int observerSize() {
        return list.size();
    }
}
