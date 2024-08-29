package com.wit.witsdk.sensor.utils;

import java.math.BigDecimal;
import java.text.DecimalFormat;

/**
 * @Author haungyajun
 * @Date 2022/4/27 15:03 （可以根据需要修改）
 */
public class DipSensorMagHelper {

    /**
     * 磁场转换标准单位uT(微特)
     *
     * @author huangyajun
     * @date 2022/4/27 15:09
     */
    public static double GetMagToUt(short reg72, double regMag)
    {
        double dRet = regMag;
        switch (reg72)
        {
            case 2:
                dRet = dRet * 0.15;
                break;
            case 3:
                dRet = dRet * 13 / 1000.0;
                break;
            case 4:
                dRet = dRet * 0.058;
                break;
            case 5:
                dRet = dRet * 0.098;
                break;
            case 6:
                dRet = dRet / 150;
                break;
            case 7:
                dRet = dRet * 20 / 1000.0;
                break;
        }
        BigDecimal bg = new BigDecimal(dRet);
        return bg.setScale(3, BigDecimal.ROUND_HALF_UP).doubleValue();
    }

}
