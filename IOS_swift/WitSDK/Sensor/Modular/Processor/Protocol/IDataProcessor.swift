//
//  数据处理器协议
//
//  Created by huangyajun on 2022/8/31.
//

import Foundation

protocol IDataProcessor{
    
    // 打开设备时
    func onOpen(deviceModel:DeviceModel)
    
    // 关闭设备时
    func onClose()
    
    // 设备数据实时更新时
    func onUpdate(deviceModel:DeviceModel)
}
