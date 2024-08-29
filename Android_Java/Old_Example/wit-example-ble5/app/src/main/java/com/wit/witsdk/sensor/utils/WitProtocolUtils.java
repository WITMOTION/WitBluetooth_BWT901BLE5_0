package com.wit.witsdk.sensor.utils;

import java.util.Arrays;
import java.util.List;

/**
 * 维特协议工具类
 *
 * @author huangyajun
 * @date 2022/5/23 14:35
 */
public class WitProtocolUtils {

    /**
     * 获得读取的命令
     *
     * @author huangyajun
     * @date 2022/5/23 14:41
     */
    public static byte[] getRead(int reg) {
        return new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, (byte) reg, 0x00};
    }

    /**
     * 获得写入的命令
     *
     * @author huangyajun
     * @date 2022/5/23 14:41
     */
    public static byte[] getWrite(int reg, short value) {
        return new byte[]{(byte) 0xff, (byte) 0xaa, (byte) reg, (byte) value, (byte) (value >> 8)};
    }

    /**
     * 查找传感器返回的值
     *
     * @author huangyajun
     * @date 2022/5/23 14:17
     */
    public static Byte[] findReturnData(Byte[] returnData) {

        List<Byte> bytes = Arrays.asList(returnData);

        for (int i = 0; i < bytes.size() && i + 11 <= bytes.size(); i++) {
            List<Byte> tempList = bytes.subList(i, i + 11);
            Byte[] tempArr = tempList.toArray(new Byte[tempList.size()]);
            if (tempArr.length == 11 && tempArr[0] == 0x55 && tempArr[1] == 0x5f && checkSUM(tempArr)) {
                return tempArr;
            }
        }
        return null;
    }


    /**
     * 检查sum和
     *
     * @author huangyajun
     * @date 2022/5/23 14:38
     */
    private static boolean checkSUM(Byte[] dataPack) {
        if (dataPack == null || dataPack.length < 2) {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < dataPack.length - 1; i++) {
            sum = sum + dataPack[i];
        }
        //实际上num 这里已经是结果了，如果只是取int 可以直接返回了
        byte check = (byte) sum;


        //如果最后一个字节等于校验和就是通过
        if (dataPack[dataPack.length - 1] == check) {
            return true;
        }

        return false;
    }

    /**
     * 检查校验和
     *
     * @author huangyajun
     * @date 2022/8/25 15:50
     */
    public static boolean checkSum(List<Byte> activeByteTemp) {

        int sum = 0;

        for (int i = 0; i < activeByteTemp.size() - 1; i++) {
            sum += (((int) activeByteTemp.get(i)) & 0x0FF);
        }

        sum = sum & 0xff;
        if (sum == (((int) activeByteTemp.get(10)) & 0x0FF)) {
            return true;
        }
        return false;
    }
}
