package com.wit.example.ble5.components;

import android.util.Log;

import com.wit.witsdk.sensor.modular.connector.entity.BluetoothBLEOption;
import com.wit.witsdk.sensor.modular.connector.modular.bluetooth.WitBluetoothManager;
import com.wit.witsdk.sensor.modular.connector.roles.WitCoreConnect;
import com.wit.witsdk.sensor.modular.device.DeviceModel;
import com.wit.example.ble5.data.WitSensorKey;
import com.wit.witsdk.sensor.modular.processor.interfaces.IDataProcessor;
import com.wit.witsdk.sensor.utils.DipSensorMagHelper;
import com.wit.witsdk.utils.BitConvert;
import com.wit.witsdk.utils.NumberFormat;
import com.wit.witsdk.utils.StringUtils;

/**
 * 蓝牙5.0传感器数据解析
 *
 * @Author haungyajun
 * @Date 2022/4/26 15:25
 */
public class Bwt901bleProcessor implements IDataProcessor {

    /**
     * 控制自动读取线程
     */
    private boolean readDataThreadRuning = false;

    // 设备模型
    private DeviceModel deviceModel;

    @Override
    public void OnOpen(DeviceModel deviceModel) {
        this.deviceModel = deviceModel;
        readDataThreadRuning = true;
        Thread thread = new Thread(this::readDataThread);
        thread.start();
    }

    /**
     * 发送协议数据
     *
     * @author huangyajun
     * @date 2023/2/27 19:23
     */
    private void sendProtocolData(DeviceModel deviceModel, byte[] bytes, int delay) {
        deviceModel.sendProtocolData(bytes, delay);// 磁场
        try {
            Thread.sleep(delay);
        } catch (InterruptedException e) {
            e.printStackTrace();
            System.out.println(e.getMessage());
            return;
        }
    }

    /**
     * 读取数据线程
     *
     * @author huangyajun
     * @date 2022/5/24 11:39
     */
    private void readDataThread() {
        int count = 0;
        try {
            Thread.sleep(5000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        while (readDataThreadRuning) {
            try {

                String magType = deviceModel.getDeviceData("72");// 磁场类型
                if (StringUtils.IsNullOrEmpty(magType)) {
                    // 读取72磁场类型寄存器,后面解析磁场的时候要用到
                    sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, 0x72, 0x00}, 150);
                    sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, 0x72, 0x00}, 150);
                }

                String reg2e = deviceModel.getDeviceData("2E");// 版本号
                String reg2f = deviceModel.getDeviceData("2F");// 版本号
                if (StringUtils.IsNullOrEmpty(reg2e) || StringUtils.IsNullOrEmpty(reg2f)) {
                    // 读版本号
                    sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, 0x2E, 0x00}, 150);
                }

                sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x27, (byte) 0x3a, (byte) 0x00}, 150);// 磁场
                sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x27, (byte) 0x51, (byte) 0x00}, 150);// 四元数

                // 不需要读那么快的数据
                if (count++ % 50 == 0 || count < 5) {
                    sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x27, (byte) 0x64, (byte) 0x00}, 150);// 电量
                    sendProtocolData(deviceModel, new byte[]{(byte) 0xff, (byte) 0xaa, (byte) 0x27, (byte) 0x40, (byte) 0x00}, 150);// 温度


                }

                // 读取信号
                if (count % 5 == 0) {
                    WitCoreConnect coreConnect = deviceModel.getCoreConnect();
                    BluetoothBLEOption bluetoothBLEOption = coreConnect.getConfig().getBluetoothBLEOption();
                    deviceModel.setDeviceData(WitSensorKey.Rssi, WitBluetoothManager.getRssi(bluetoothBLEOption.getMac()) + "");
                }
            } catch (Exception e) {
                e.printStackTrace();
                Log.i("", "BWT901BLECL5_0DataProcessor:自动读取数据异常");
            }
        }
    }

    /**
     * 关闭设备时
     *
     * @author huangyajun
     * @date 2022/5/24 11:42
     */
    @Override
    public void OnClose() {
        // 停止线程
        readDataThreadRuning = false;
    }

    /**
     * 解算数据
     *
     * @author huangyajun
     * @date 2022/4/27 14:39
     */
    @Override
    public void OnUpdate(DeviceModel deviceModel) {
        // 加速度
        String regAx = deviceModel.getDeviceData("61_0");
        String regAy = deviceModel.getDeviceData("61_1");
        String regAz = deviceModel.getDeviceData("61_2");
        // 角速度
        String regWx = deviceModel.getDeviceData("61_3");
        String regWy = deviceModel.getDeviceData("61_4");
        String regWz = deviceModel.getDeviceData("61_5");
        // 角度
        String regAngleX = deviceModel.getDeviceData("61_6");
        String regAngleY = deviceModel.getDeviceData("61_7");
        String regAngleZ = deviceModel.getDeviceData("61_8");

        // 四元数
        String regQ1 = deviceModel.getDeviceData("51");
        String regQ2 = deviceModel.getDeviceData("52");
        String regQ3 = deviceModel.getDeviceData("53");
        String regQ4 = deviceModel.getDeviceData("54");
        // 温度和电量
        String regTemperature = deviceModel.getDeviceData("40");
        String regPower = deviceModel.getDeviceData("64");


        // 版本号
        String reg2e = deviceModel.getDeviceData("2E");// 版本号
        String reg2f = deviceModel.getDeviceData("2F");// 版本号

        // 如果有版本号
        if (StringUtils.IsNullOrEmpty(reg2e) == false &&
                StringUtils.IsNullOrEmpty(reg2f) == false) {
            short reg2eValue = Short.parseShort(reg2e);
            short reg2fValue = Short.parseShort(reg2f);

            int tempVerSion = BitConvert.byte2Int(new byte[]{
                    BitConvert.short2byte(reg2fValue)[0],
                    BitConvert.short2byte(reg2fValue)[1],
                    BitConvert.short2byte(reg2eValue)[0],
                    BitConvert.short2byte(reg2eValue)[1]
            });

            // UInt32 tempVerSion = BitConverter.ToUInt32(Buffer, 2);
            String sbinary = Integer.toBinaryString(tempVerSion);// Convert.ToString(tempVerSion, 2);
            sbinary = StringUtils.padLeft(sbinary, 32, '0');
            if (sbinary.substring(0, 1).equals("1"))//新版本号
            {
                String tempNewVS = Integer.parseInt(sbinary.substring(2, 18), 2) + "";
                tempNewVS += "." + Integer.parseInt(sbinary.substring(19, 19 + 5), 2);
                tempNewVS += "." + Integer.parseInt(sbinary.substring(25), 2);
                //Public.Common.Version_Number = tempNewVS;
                deviceModel.setDeviceData(WitSensorKey.VersionNumber, tempNewVS);
            } else {
                int tempNewVS = BitConvert.byte2Int(new byte[]{
                        0,
                        0,
                        BitConvert.short2byte(reg2eValue)[0],
                        BitConvert.short2byte(reg2eValue)[1]
                });
                deviceModel.setDeviceData(WitSensorKey.VersionNumber, tempNewVS + "");
            }
        }

        // 加速度解算
        if (!StringUtils.IsNullOrEmpty(regAx)) {
            deviceModel.setDeviceData(WitSensorKey.AccX, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAx) / 32768 * 16) + "");
        }
        if (!StringUtils.IsNullOrEmpty(regAy)) {
            deviceModel.setDeviceData(WitSensorKey.AccY, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAy) / 32768 * 16));
        }
        if (!StringUtils.IsNullOrEmpty(regAz)) {
            deviceModel.setDeviceData(WitSensorKey.AccZ, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAz) / 32768 * 16));
        }

        // 角速度解算
        if (!StringUtils.IsNullOrEmpty(regWx)) {
            deviceModel.setDeviceData(WitSensorKey.AsX, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regWx) / 32768 * 2000));
        }
        if (!StringUtils.IsNullOrEmpty(regWy)) {
            deviceModel.setDeviceData(WitSensorKey.AsY, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regWy) / 32768 * 2000));
        }
        if (!StringUtils.IsNullOrEmpty(regWz)) {
            deviceModel.setDeviceData(WitSensorKey.AsZ, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regWz) / 32768 * 2000));
        }

        // 角度
        if (!StringUtils.IsNullOrEmpty(regAngleX)) {
            deviceModel.setDeviceData(WitSensorKey.AngleX, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAngleX) / 32768 * 180));
        }
        if (!StringUtils.IsNullOrEmpty(regAngleY)) {
            deviceModel.setDeviceData(WitSensorKey.AngleY, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAngleY) / 32768 * 180));
        }
        if (!StringUtils.IsNullOrEmpty(regAngleZ)) {
            String anZ = NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regAngleZ) / 32768 * 180);
            deviceModel.setDeviceData(WitSensorKey.AngleZ, anZ);
        }

        // 磁场
        String regHX = deviceModel.getDeviceData("3A");
        String regHY = deviceModel.getDeviceData("3B");
        String regHZ = deviceModel.getDeviceData("3C");
        // 磁场类型
        String magType = deviceModel.getDeviceData("72");
        if (!StringUtils.IsNullOrEmpty(regHX) &&
                !StringUtils.IsNullOrEmpty(regHY) &&
                !StringUtils.IsNullOrEmpty(regHZ) &&
                !StringUtils.IsNullOrEmpty(magType)
        ) {
            short type = Short.parseShort(magType);
            // 解算数据,并且保存到设备数据里
            deviceModel.setDeviceData(WitSensorKey.HX, DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHX)) + "");
            deviceModel.setDeviceData(WitSensorKey.HY, DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHY)) + "");
            deviceModel.setDeviceData(WitSensorKey.HZ, DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHZ)) + "");
        }


        // 温度
        if (!StringUtils.IsNullOrEmpty(regTemperature)) {
            deviceModel.setDeviceData(WitSensorKey.T, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regTemperature) / 100));
        }

        // 电量
        if (!StringUtils.IsNullOrEmpty(regPower)) {

            int regPowerValue = Integer.parseInt(regPower);

            float eqPercent = getEqPercent((float) (regPowerValue / 100.0));
            deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, eqPercent + "");
            // 电量原始值
            deviceModel.setDeviceData(WitSensorKey.ElectricQuantity, regPowerValue + "");
        }

        // 四元数
        if (!StringUtils.IsNullOrEmpty(regQ1)) {
            deviceModel.setDeviceData(WitSensorKey.Q0, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regQ1) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ2)) {
            deviceModel.setDeviceData(WitSensorKey.Q1, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regQ2) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ3)) {
            deviceModel.setDeviceData(WitSensorKey.Q2, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regQ3) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ4)) {
            deviceModel.setDeviceData(WitSensorKey.Q3, NumberFormat.formatDoubleToString("%.3f", Double.parseDouble(regQ4) / 32768.0));
        }
    }

    /**
     * 获得电流值
     *
     * @author huangyajun
     * @date 2022/8/16 18:26
     */
    public float getEqPercent(float eq) {
        float p = 0;
        if (eq > 5.50) {
            p = Interp(eq,
                    new float[]{6.5f, 6.8f, 7.35f, 7.75f, 8.5f, 8.8f},
                    new float[]{0, 10, 30, 60, 90, 100});
        } else {
            p = Interp(eq,
                    new float[]{3.4f, 3.5f, 3.68f, 3.7f, 3.73f, 3.77f, 3.79f, 3.82f, 3.87f, 3.93f, 3.96f, 3.99f},
                    new float[]{0, 5, 10, 15, 20, 30, 40, 50, 60, 75, 90, 100});
        }
        return p;
    }

    public float Interp(float a, float[] x, float[] y) {
        float v = 0;
        int L = x.length;
        if (a < x[0]) v = y[0];
        else if (a > x[L - 1]) v = y[L - 1];
        else {
            for (int i = 0; i < y.length - 1; i++) {
                if (a > x[i + 1]) continue;
                v = y[i] + (a - x[i]) / (x[i + 1] - x[i]) * (y[i + 1] - y[i]);
                break;
            }
        }
        return v;
    }

    private void ReadMagType(DeviceModel deviceModel) {
        // 读取72磁场类型寄存器,后面解析磁场的时候要用到
        deviceModel.sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, 0x72, 0x00});
    }
}
