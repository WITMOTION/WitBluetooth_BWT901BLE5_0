//
//  key刷新事件观察者
//
//  Created by 赵文 on 2022/9/3.
//

import Foundation

protocol IKeyUpdateObserver {
    
    // key 刷新事件
    func onKeyUpdate(_ deviceModel:DeviceModel, _ key:String,_ value:String)
    
}
