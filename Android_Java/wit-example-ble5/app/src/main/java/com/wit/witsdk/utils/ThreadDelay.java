package com.wit.witsdk.utils;

import java.util.concurrent.Callable;

/**
 * @author Zhangpingxiang
 * @Date: 2023/2/25 0025
 */
public class ThreadDelay implements Callable {
    private  int delayTime= 100;
    public ThreadDelay(int delayTime){
        this.delayTime = delayTime;
    }
    @Override
    public Object call() throws Exception {
        Thread.sleep(delayTime);
        return 0;
    }
}
