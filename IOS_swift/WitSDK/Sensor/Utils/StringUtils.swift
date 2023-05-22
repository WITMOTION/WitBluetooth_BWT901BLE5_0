//
//  
//
//  Created by huangyajun on 2022/9/1.
//

import Foundation

class StringUtils {
    
    // 判断是空串
    static func IsNullOrEmpty(_ str:String?) -> Bool{
        return str == nil || str == ""
    }
    
    // 判断不是空串
    static func IsNotNullOrEmpty(_ str:String?) -> Bool{
        return str != nil && str != ""
    }
    
    // 左边补齐
    static func padLeft(_ str:String,_ len:Int,_ char:String) -> String {
        
        if(str.count >= len){
            return str
        }

        
        let count = str.count
        var i = 0
        var append = ""
        
        while(i < len - count){
            append.append(contentsOf: char)
            i = i + 1
        }
        
        return append + str
    }
    
    // 右边补齐
    static func padRight(_ str:String,_ len:Int,_ char:String) -> String {
        
        if(str.count >= len){
            return str
        }

        var i = 0
        var append = ""
        while(i<len - str.count){
            append.append(contentsOf: char)
            i = i + 1
        }
        
        return  str + append
    }
    
    // 获得子字符串
    static func subString(_ str:String,_ start:Int,_ len:Int) -> String {
        let index = str.index(str.startIndex, offsetBy: start)
        let endIndex = str.index(str.startIndex, offsetBy: start + len)
        return String(str[index..<endIndex])
    }
}
