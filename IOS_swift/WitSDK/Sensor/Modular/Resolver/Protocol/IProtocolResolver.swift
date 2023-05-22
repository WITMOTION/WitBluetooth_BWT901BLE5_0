//
//  协议解析器借口
//
//  Created by huangyajun on 2022/8/31.
//

import Foundation

protocol IProtocolResolver{
    
    // 发送数据
    func sendData(sendData:[UInt8], deviceModel:DeviceModel, waitTime:Int64) throws
    
    // 解算传感器主动会传的数据
    func passiveReceiveData(data:[UInt8], deviceModel:DeviceModel)
    
}
