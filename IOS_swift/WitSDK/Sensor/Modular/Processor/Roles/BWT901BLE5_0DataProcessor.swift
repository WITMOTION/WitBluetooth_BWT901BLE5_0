//
//  蓝牙5.0数据处理器
//
//  Created by huangyajun on 2022/9/1.
//

import Foundation


class BWT901BLE5_0DataProcessor : IDataProcessor {
    
    // 控制自动读取数据线程
    var readDataThreadRuning:Bool = false
    
    // 设备模型
    var deviceModel:DeviceModel?
    
    // 传感器打开时
    func onOpen(deviceModel: DeviceModel) {
        self.deviceModel = deviceModel
        // 开启读取数据线程
        let thread = Thread(target: self,
                            selector: #selector(readDataThread),
                            object: nil)
        readDataThreadRuning = true
        thread.start()
    }
    
    // 自动读取数据线程
    @objc func readDataThread(){
        
        var count:Int = 0;
        while (readDataThreadRuning) {
            do {
                
                let magType:String? = deviceModel?.getDeviceData("72");// 磁场类型
                if (StringUtils.IsNullOrEmpty(magType)) {
                    // 读取72磁场类型寄存器,后面解析磁场的时候要用到
                    try deviceModel?.sendProtocolData([0xff, 0xaa, 0x27, 0x72, 0x00], 150);
                }
                
                let reg2e:String? = deviceModel?.getDeviceData("2E");// 版本号
                let reg2f:String? = deviceModel?.getDeviceData("2F");// 版本号
                if (StringUtils.IsNullOrEmpty(reg2e) || StringUtils.IsNullOrEmpty(reg2f)) {
                    // 读版本号
                    try deviceModel?.sendProtocolData([0xff,0xaa, 0x27, 0x2E, 0x00], 150);
                }
                
                try deviceModel?.sendProtocolData([0xff, 0xaa, 0x27, 0x3a, 0x00], 150);// 磁场
                try deviceModel?.sendProtocolData([0xff, 0xaa, 0x27, 0x51, 0x00], 150);// 四元数
                // 不需要读那么快的数据
                count = count + 1
                if (count % 50 == 0 || count < 5) {
                    try deviceModel?.sendProtocolData([ 0xff, 0xaa, 0x27, 0x64, 0x00], 150);// 电量
                    try deviceModel?.sendProtocolData([ 0xff, 0xaa, 0x27, 0x40, 0x00], 150);// 温度
//                    WitCoreConnect coreConnect = deviceModel.getCoreConnect();
//                    BluetoothBLEOption bluetoothBLEOption = coreConnect.getConfig().getBluetoothBLEOption();
//                    deviceModel.setDeviceData(WitSensorKey.SignalValue, WitBluetoothManager.getRssi(bluetoothBLEOption.getMac()) + "");
                }
            } catch {
                print("BWT901BLECL5_0DataProcessor:自动读取数据异常");
            }
        }
        
    }
    
    // 传感器关闭时
    func onClose() {
        readDataThreadRuning = false
    }
    
    // 传感器更新时
    func onUpdate(deviceModel:DeviceModel) {
        
        // 加速度
        let regAx:String? = deviceModel.getDeviceData("61_0");
        let regAy:String? = deviceModel.getDeviceData("61_1");
        let regAz:String? = deviceModel.getDeviceData("61_2");
        // 角速度
        let regWx:String? = deviceModel.getDeviceData("61_3");
        let regWy:String? = deviceModel.getDeviceData("61_4");
        let regWz:String? = deviceModel.getDeviceData("61_5");
        // 角度
        let regAngleX:String? = deviceModel.getDeviceData("61_6");
        let regAngleY:String? = deviceModel.getDeviceData("61_7");
        let regAngleZ:String? = deviceModel.getDeviceData("61_8");
        
        // 四元数
        let regQ1:String? = deviceModel.getDeviceData("51");
        let regQ2:String? = deviceModel.getDeviceData("52");
        let regQ3:String? = deviceModel.getDeviceData("53");
        let regQ4:String? = deviceModel.getDeviceData("54");
        // 温度和电量
        let regTemperature:String? = deviceModel.getDeviceData("40");
        let regPower:String? = deviceModel.getDeviceData("64");
        
        
        // 版本号
        let reg2e:String? = deviceModel.getDeviceData("2E");// 版本号
        let reg2f:String? = deviceModel.getDeviceData("2F");// 版本号
        
   
        // 如果有版本号
        if (reg2e != nil &&
            reg2f != nil) {
            let reg2eValue:Int16 = Int16((reg2e as NSString?)?.intValue ?? 0)
            let reg2fValue:Int16 = Int16((reg2f as NSString?)?.intValue ?? 0)
            
            let tempVersion:UInt32 = UInt32(UInt32(reg2eValue) << 16 | UInt32(reg2fValue))
            var sbinary:String =  String(tempVersion, radix: 2)
            sbinary = StringUtils.padLeft(sbinary, 32, "0")
            if (sbinary.first == "1")// 新版本号
            {
                
                var tempNewVS:String = String(UInt32(StringUtils.subString(sbinary, (4 - 3), (14 + 3)), radix: 2) ?? 0)
                tempNewVS = tempNewVS + "." + String(UInt32(StringUtils.subString(sbinary, 18, 6), radix: 2) ?? 0)
                tempNewVS = tempNewVS + "." + String(UInt32(StringUtils.subString(sbinary, 24, 2), radix: 2) ?? 0)
                deviceModel.setDeviceData(WitSensorKey.VersionNumber, tempNewVS)
            } else {
                deviceModel.setDeviceData(WitSensorKey.VersionNumber, "\(reg2eValue)")
            }
        }
        
        // 加速度解算
        if (!StringUtils.IsNullOrEmpty(regAx)) {
            deviceModel.setDeviceData(WitSensorKey.AccX, String(format:"%.3f", Double.parseDouble(regAx) / 32768 * 16, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regAy)) {
            deviceModel.setDeviceData(WitSensorKey.AccY, String(format:"%.3f", Double.parseDouble(regAy) / 32768 * 16, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regAz)) {
            deviceModel.setDeviceData(WitSensorKey.AccZ, String(format:"%.3f", Double.parseDouble(regAz) / 32768 * 16, 3));
        }
        
        // 角速度解算
        if (!StringUtils.IsNullOrEmpty(regWx)) {
            deviceModel.setDeviceData(WitSensorKey.GyroX, String(format:"%.3f", Double.parseDouble(regWx) / 32768 * 2000, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regWy)) {
            deviceModel.setDeviceData(WitSensorKey.GyroY, String(format:"%.3f", Double.parseDouble(regWy) / 32768 * 2000, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regWz)) {
            deviceModel.setDeviceData(WitSensorKey.GyroZ, String(format:"%.3f", Double.parseDouble(regWz) / 32768 * 2000, 3));
        }
        
        // 角度
        if (!StringUtils.IsNullOrEmpty(regAngleX)) {
            deviceModel.setDeviceData(WitSensorKey.AngleX, String(format:"%.3f", Double.parseDouble(regAngleX) / 32768 * 180, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regAngleY)) {
            deviceModel.setDeviceData(WitSensorKey.AngleY, String(format:"%.3f", Double.parseDouble(regAngleY) / 32768 * 180, 3));
        }
        if (!StringUtils.IsNullOrEmpty(regAngleZ)) {
            let anZ:String = String(format:"%.3f", Double.parseDouble(regAngleZ) / 32768 * 180, 3)
            deviceModel.setDeviceData(WitSensorKey.AngleZ, anZ);
        }
        
        // 磁场
        let regHX:String? = deviceModel.getDeviceData("3A");
        let regHY:String? = deviceModel.getDeviceData("3B");
        let regHZ:String? = deviceModel.getDeviceData("3C");
        // 磁场类型
        let magType:String? = deviceModel.getDeviceData("72");
        if (!StringUtils.IsNullOrEmpty(regHX) &&
            !StringUtils.IsNullOrEmpty(regHY) &&
            !StringUtils.IsNullOrEmpty(regHZ) &&
            !StringUtils.IsNullOrEmpty(magType)
        ) {
            let type:Int16 = Int16(magType ?? "0", radix: 10) ?? 0
            // 解算数据,并且保存到设备数据里
            deviceModel.setDeviceData(WitSensorKey.MagX, String(DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHX))));
            deviceModel.setDeviceData(WitSensorKey.MagY, String(DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHY))));
            deviceModel.setDeviceData(WitSensorKey.MagZ, String(DipSensorMagHelper.GetMagToUt(type, Double.parseDouble(regHZ))));
        }
        
        // 温度
        if (!StringUtils.IsNullOrEmpty(regTemperature)) {
            deviceModel.setDeviceData(WitSensorKey.Temperature, String(format: "%.3f", Double.parseDouble(regTemperature) / 100, 2));
        }
        
        // 电量
        if (!StringUtils.IsNullOrEmpty(regPower)) {
            
            let regPowerValue:Int = Int(regPower ?? "0", radix: 10) ?? 0
            let eqPercent:Float = getEqPercent(Float(Float(regPowerValue) / 100.0));
            deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage,  String(eqPercent));
            
            
            // 计算电量百分比
            // if (regPowerValue >= 830) {
            //     deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, "100");
            // } else if (regPowerValue >= 750 && regPowerValue < 830) {
            //     deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, "75");
            // } else if (regPowerValue >= 715 && regPowerValue < 750) {
            //     deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, "50");
            // } else if (regPowerValue >= 675 && regPowerValue < 715) {
            //     deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, "25");
            // } else if (regPowerValue <= 675) {
            //     deviceModel.setDeviceData(WitSensorKey.ElectricQuantityPercentage, "0");
            // }
            
            // 电量原始值
            deviceModel.setDeviceData(WitSensorKey.ElectricQuantity, String(regPowerValue) );
        }
        
        // 四元数
        if (!StringUtils.IsNullOrEmpty(regQ1)) {
            deviceModel.setDeviceData(WitSensorKey.Q0, String(format:"%.3f", Double.parseDouble(regQ1) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ2)) {
            deviceModel.setDeviceData(WitSensorKey.Q1, String(format:"%.3f", Double.parseDouble(regQ2) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ3)) {
            deviceModel.setDeviceData(WitSensorKey.Q2, String(format:"%.3f", Double.parseDouble(regQ3) / 32768.0));
        }
        if (!StringUtils.IsNullOrEmpty(regQ4)) {
            deviceModel.setDeviceData(WitSensorKey.Q3, String(format:"%.3f", Double.parseDouble(regQ4) / 32768.0));
        }
        
    }
    
    // 获得电流值
    func getEqPercent(_ eq:Float) -> Float {
        var p:Float = 0;
        if (eq > 5.50) {
            p = Interp(eq, [6.5, 6.8, 7.35, 7.75, 8.5, 8.8], [0.0, 10.0,30.0, 60.0, 90.0, 100.0]);
        } else {
            p = Interp(eq,
                       [3.4, 3.5, 3.68, 3.7, 3.73, 3.77, 3.79, 3.82, 3.87, 3.93, 3.96, 3.99],
                       [0.0, 5.0, 10.0, 15.0, 20.0, 30.0, 40.0, 50.0, 60.0, 75.0, 90.0, 100.0])
        }
        return p;
    }
    
    // 匹配百分比
    func Interp(_ a:Float,_ x:[Float],_ y:[Float]) -> Float {
        var v:Float = 0;
        let L:Int = x.count;
        if (a < x[0]) { v = y[0]}
        else if (a > x[L - 1]) {v = y[L - 1]}
        else {
            var i:Int = 0
            while (i < y.count - 1) {
                if (a > x[i + 1]) { i = i+1; continue; }
                v = y[i] + (a - x[i]) / (x[i + 1] - x[i]) * (y[i + 1] - y[i]);
                break;
            }
        }
        return v;
    }
    
    func ReadMagType( deviceModel:DeviceModel) {
        // 读取72磁场类型寄存器,后面解析磁场的时候要用到
        //deviceModel.sendProtocolData(new byte[]{(byte) 0xff, (byte) 0xaa, 0x27, 0x72, 0x00});
    }
}


// 扩展double
extension Double {
    static func parseDouble(_ str:String) -> Double{
        return ((str as NSString).doubleValue)
    }
    
    static func parseDouble(_ str:String?) -> Double{
        return ((str as NSString?)?.doubleValue ?? 0)
    }
}

