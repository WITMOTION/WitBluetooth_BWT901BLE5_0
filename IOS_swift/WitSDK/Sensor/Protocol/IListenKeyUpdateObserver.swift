//
//  被监听的key刷新观察者
//
//  Created by huangyajun on 2022/9/3.
//

import Foundation


protocol IListenKeyUpdateObserver {
    
    // MARK: 监听的key刷新时
    func onListenKeyUpdate(_ deviceModel:DeviceModel)
    
}
