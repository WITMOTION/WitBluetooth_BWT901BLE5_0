//
//  对象比较帮助类
//  Wit-Example-BLE
//
//  Created by huangyajun on 2022/8/29.
//

import Foundation

class CompareObjectHelper{
    
    // 取出某个对象的地址
    static func getObjectMemoryAddress(_ object: AnyObject) -> String {
        let str = Unmanaged<AnyObject>.passUnretained(object).toOpaque()
        return String(describing: str)
    }
    
    // 对比两个对象的地址是否相同
    static func compareObjectMemoryAddress(_ object1: AnyObject,_ object2: AnyObject) -> Bool {
        let str1 = getObjectMemoryAddress(object1)
        let str2 = getObjectMemoryAddress(object2)
        
        if str1 == str2 {
            return true
        } else {
            return false
        }
    }
    
}
