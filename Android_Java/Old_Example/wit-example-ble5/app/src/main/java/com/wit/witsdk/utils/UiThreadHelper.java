package com.wit.witsdk.utils;

import android.os.Handler;
import android.os.Looper;

/**
 * ui线程帮助类
 *
 * @author huangyajun
 * @date 2022/6/18 17:49
 */
public class UiThreadHelper {

    /**
     * 调用ui线程执行工作
     *
     * @author huangyajun
     * @date 2022/6/18 17:54
     */
    public static void runUi(Runnable runnable) {
        Looper.prepare();
        Handler handler = new Handler();
        handler.post(runnable);
        Looper.loop();
    }

}
