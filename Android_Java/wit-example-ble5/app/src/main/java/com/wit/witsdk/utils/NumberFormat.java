package com.wit.witsdk.utils;

import android.annotation.SuppressLint;

public class NumberFormat {


    public static String formatDoubleToString(String f, double d) {

        String format = String.format("%.3f", d, 3);

        return format.replaceAll(",",".");
    }

}
