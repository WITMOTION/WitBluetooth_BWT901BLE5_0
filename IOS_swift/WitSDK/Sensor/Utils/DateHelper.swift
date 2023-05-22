//
//  日期帮助类
//
//  Created by huangyajun on 2022/9/16.
//

import Foundation


public class DateHelper {
    
    // MARK: 将日期转为字符串
    public static func format(_ date:Date,_ dateFormat:String,_ appedMs:Bool) -> String{
        let dformatter = DateFormatter()
        dformatter.dateFormat = dateFormat
        var dateString = dformatter.string(from: date)
        if appedMs {
            let ms = StringUtils.padLeft("\((CLongLong(round(date.timeIntervalSince1970*1000)) % 1000))", 3, "0")
            dateString = "\(dateString).\(ms)"
        }
        
        return dateString
    }
    
    // MARK: 获得当前时间戳
    public static func getTimestamp() -> CLongLong{
        return CLongLong(round(Date().timeIntervalSince1970*1000)) % 1000
    }
    
}
