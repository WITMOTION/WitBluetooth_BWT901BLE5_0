package com.wit.example.ble5.components;

import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.witsdk.sensor.modular.resolver.entity.SendDataResult;
import com.wit.witsdk.sensor.modular.resolver.interfaces.IProtocolResolver;
import com.wit.witsdk.sensor.modular.resolver.interfaces.ISendDataCallback;
import com.wit.witsdk.utils.BitConvert;
import com.wit.witsdk.utils.StringUtils;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 * 蓝牙5.0传感器协议解析器
 *
 * @Author haungyajun
 * @Date 2022/4/26 15:26 （可以根据需要修改）
 */
public class Bwt901bleResolver implements IProtocolResolver {

    /**
     * 要解析的数据
     */
    private List<Byte> activeByteDataBuffer = new ArrayList<>();

    /**
     * 临时Byte
     */
    private List<Byte> activeByteTemp = new ArrayList<>();

    /**
     * 发送数据，并且指定等待时长
     *
     * @author huangyajun
     * @date 2022/5/23 13:53
     */
    @Override
    public void sendData(byte[] sendData, DeviceModel deviceModel, int waitTime, ISendDataCallback callback) {

        if (waitTime < 0) {
            waitTime = 100;
        }

        try {
            deviceModel.sendData(sendData, (rtnBytes) -> {
                Byte[] returnData = rtnBytes;
                if (sendData != null && sendData.length >= 5 && sendData[2] == 0x27 && returnData != null && returnData.length >= 20) {

                    returnData = findReturnData(returnData);
                    if (returnData != null && returnData.length == 20) {
                        int readReg = sendData[4] << 8 | sendData[3];
                        int rtnReg = returnData[3] << 8 | returnData[2];

                        if (readReg == rtnReg) {
                            short[] Pack = new short[9];

                            for (int i = 0; i < 4; i++) {
                                Pack[i] = BitConvert.byte2short(new byte[]{returnData[5 + (i * 2)], returnData[4 + (i * 2)]});

                                String reg = Integer.toHexString(readReg + i).toUpperCase();
                                reg = StringUtils.padLeft(reg, 2, '0');
                                deviceModel.setDeviceData(reg, Pack[i] + "");
                            }
                        }
                    }
                }
                // 调用回调方法
                Thread th = new Thread(() -> {
                    callback.run(new SendDataResult(true));
                });
                th.start();
            }, waitTime, 1);
        } catch (Exception ex) {
            // 调用回调方法
            Thread th = new Thread(() -> {
                callback.run(new SendDataResult(false));
            });
            th.start();
        }
    }

    /**
     * 发送数据
     *
     * @author huangyajun
     * @date 2022/6/29 17:06
     */
    @Override
    public void sendData(byte[] sendData, DeviceModel deviceModel) {
        deviceModel.sendData(sendData);
    }

    /**
     * 查找传感器返回的值
     *
     * @author huangyajun
     * @date 2022/5/23 14:17
     */
    public static Byte[] findReturnData(Byte[] returnData) {

        List<Byte> bytes = Arrays.asList(returnData);

        List<Byte> tempArr;

        for (int i = 0; i < bytes.size(); i++) {
            if (bytes.size() - i >= 20) {
                tempArr = bytes.subList(i, i + 20);//  .Skip(i).Take(20).ToArray(); ;
                if (tempArr.size() == 20 && tempArr.get(0) == 0x55 && tempArr.get(1) == 0x71) {
                    Byte[] rtn = tempArr.toArray(new Byte[tempArr.size()]);
                    return rtn;

                }
            }
        }
        return null;
    }

    /**
     * 解析传感器主动回传的数据
     *
     * @author huangyajun
     * @date 2022/5/23 13:53
     */
    @Override
    public void passiveReceiveData(byte[] data, DeviceModel deviceModel) {

        if (data.length < 1) {
            return;
        }

        for (int i = 0; i < data.length; i++) {
            activeByteDataBuffer.add(data[i]);
        }

        while (activeByteDataBuffer.size() > 1 && activeByteDataBuffer.get(0) != 0x55 && (activeByteDataBuffer.get(1) != 0x61 || activeByteDataBuffer.get(1) != 0x71)) {
            activeByteDataBuffer.remove(0);
        }

        while (activeByteDataBuffer.size() >= 20) {
            activeByteTemp = new ArrayList<>(activeByteDataBuffer.subList(0, 20));
            activeByteDataBuffer = new ArrayList<>(activeByteDataBuffer.subList(20, activeByteDataBuffer.size()));

            // 必须是55 61的数据包
            if (activeByteTemp.get(0) == 0x55 && activeByteTemp.get(1) == 0x61) {
                float[] fData = new float[9];
                int iStart = 0;
                for (int i = 0; i < 9; i++) {
                    fData[i] = (((short) activeByteTemp.get(iStart + i * 2 + 3)) << 8) | ((short) activeByteTemp.get(iStart + i * 2 + 2) & 0xff);
                    String Identify = Integer.toHexString(activeByteTemp.get(1));
                    Identify = StringUtils.padLeft(Identify, 2, '0');
                    deviceModel.setDeviceData(Identify + "_" + i, (fData[i]) + "");
                }
            }
            else if(activeByteTemp.get(0) == 0x55 && activeByteTemp.get(1) == 0x71){
                int readReg = activeByteTemp.get(3) << 8 | activeByteTemp.get(2) ;
                short[] Pack = new short[4];
                for (int i = 0; i < 4; i++) {
                    Pack[i] = BitConvert.byte2short(new byte[]{activeByteTemp.get(5 + (i * 2)), activeByteTemp.get(4 + (i * 2))});

                    String reg = Integer.toHexString(readReg + i).toUpperCase();
                    reg = StringUtils.padLeft(reg, 2, '0');
                    deviceModel.setDeviceData(reg, Pack[i] + "");
                }
            }
        }
    }
}
