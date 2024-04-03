package com.wit.witsdk.utils;

/**
 * @Author haungyajun
 * @Date 2022/8/30 17:33 （可以根据需要修改）
 */
public class BitConvert {

    /**
     * short转byte
     *
     * @author huangyajun
     * @date 2022/8/30 17:33
     */
    public static byte[] short2byte(short s) {
        byte[] b = new byte[2];
        for (int i = 0; i < 2; i++) {
            int offset = 16 - (i + 1) * 8; //因为byte占4个字节，所以要计算偏移量
            b[i] = (byte) ((s >> offset) & 0xff); //把16位分为2个8位进行分别存储
        }
        return b;
    }

    /**
     * byte转short
     *
     * @author huangyajun
     * @date 2022/8/30 17:33
     */
    public static short byte2short(byte[] b) {
        short l = 0;
        for (int i = 0; i < 2; i++) {
            l <<= 8; //<<=和我们的 +=是一样的，意思就是 l = l << 8
            l |= (b[i] & 0xff); //和上面也是一样的  l = l | (b[i]&0xff)
        }
        return l;
    }

    /**
     * byte转int
     *
     * @author huangyajun
     * @date 2022/8/30 17:33
     */
    public static int byte2Int(byte[] b) {
        int l = 0;
        for (int i = 0; i < 4; i++) {
            l <<= 8; //<<=和我们的 +=是一样的，意思就是 l = l << 8
            l |= (b[i] & 0xff); //和上面也是一样的  l = l | (b[i]&0xff)
        }
        return l;
    }
}
