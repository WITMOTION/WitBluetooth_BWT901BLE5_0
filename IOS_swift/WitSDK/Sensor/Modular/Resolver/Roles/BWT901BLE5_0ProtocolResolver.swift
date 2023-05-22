//
//  蓝牙5.0协议处理器
//
//  Created by huangyajun on 2022/9/1.
//

import Foundation

class BWT901BLE5_0ProtocolResolver : IProtocolResolver {
    
    // 要解析的数据
    var activeByteDataBuffer:[UInt8] = [UInt8]()
    
    // 临时Byte
    var activeByteTemp:[UInt8] = [UInt8]()
    
    // 发送数据
    func sendData(sendData: [UInt8], deviceModel: DeviceModel, waitTime: Int64) throws{
        
        // 回掉方法
        let callback:(_ rtnData:[UInt8]) -> Void = { rtnBytes in
            
            if sendData[3] == 0x03 {
                print("")
            }
            
            if (sendData.count >= 5 && sendData[2] == 0x27 && rtnBytes.count >= 20) {
                let returnData:[UInt8]? = self.findReturnData(rtnBytes)
                if (returnData != nil && returnData?.count == 20) {
                    let readReg:Int = Int(sendData[4] << 8 | sendData[3])
                    let rtnReg:Int = Int((returnData?[3] ?? 0) << 8 | (returnData?[2] ?? 0))
                    
                    if (readReg == rtnReg) {
                        var Pack:[Int16] = [Int16](repeating: 0, count: 9)
                        var i = 0
                        while (i < 4) {
                            Pack[i] = Int16(Int16(returnData?[5 + (i*2)] ?? 0) << 8 | Int16(returnData?[4 + (i*2)] ?? 0))
                            var reg:String = String(format: "%02X",readReg + i).uppercased()
                            reg = StringUtils.padLeft(reg, 2, "0")
                            deviceModel.setDeviceData(reg, String(Pack[i]));
                            i = i+1
                        }
                    }
                }
            }
            
        }
        // 发送数据
       try deviceModel.sendData(data: sendData,callback: callback, waitTime: waitTime)
        
    }
    
    
    /**
     * 查找传感器返回的值
     *
     * @author huangyajun
     * @date 2022/5/23 14:17
     */
    func findReturnData(_ returnData:[UInt8]) -> [UInt8]? {
        let bytes:[UInt8] = returnData
        var tempArr:[UInt8];
        var i:Int = 0
        while (i < bytes.count) {
            if (bytes.count - i >= 20) {
                tempArr = bytes[i..<(i + 20)].reversed()
                if (tempArr.count == 20 && tempArr[0] == 0x55 && tempArr[1] == 0x71) {
                    return tempArr;
                    
                }
            }
            
            i = i + 1
        }
        return nil;
    }
    
    
    // 解算实时数据
    func passiveReceiveData(data: [UInt8], deviceModel: DeviceModel) {
        
        if (data.count < 1) {
            return;
        }
        
        
        activeByteDataBuffer.append(contentsOf: data)
        
        // 移除非法数据
        while (activeByteDataBuffer.count > 0
               && activeByteDataBuffer[0] != 0x55
               && activeByteDataBuffer[1] != 0x61) {
            activeByteDataBuffer.remove(at: 0)
        }
        
        while (activeByteDataBuffer.count >= 20) {
            activeByteTemp = activeByteDataBuffer[0..<20].reversed()
            activeByteDataBuffer = activeByteDataBuffer[20..<activeByteDataBuffer.count].reversed()
            
            // 必须是55 61的数据包
            if (activeByteTemp[0] == 0x55 && activeByteTemp[1] == 0x61 && activeByteTemp.count == 20) {
                var fData:[Int16] = [Int16](repeating: 0, count: 9)
                var i:Int = 0
                while (i < 9) {
                    
                    let h:Int16 = Int16(activeByteTemp[i * 2 + 3])
                    let l:Int16 = Int16(activeByteTemp[i * 2 + 2])
                    fData[i] = Int16(h << 8 | l & 0xff)
                    let Identify:String = String(format: "%2X",activeByteTemp[1])
                    deviceModel.setDeviceData("\(Identify)_\(i)", "\(fData[i])")
                    i = i+1
                }
            }
        }
        
    }
}


