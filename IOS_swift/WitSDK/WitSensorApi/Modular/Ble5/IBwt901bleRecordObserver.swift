//
//  bwt901ble数据记录观察者
//
//  Created by 赵文 on 2022/9/3.
//

import Foundation

public protocol IBwt901bleRecordObserver {
    
    // 记录数据事件
    func onRecord(_ bwt901ble:Bwt901ble)
    
}
