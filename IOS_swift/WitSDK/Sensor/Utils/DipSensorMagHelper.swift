//
//  倾角传感器磁场帮助类
//
//  Created by huangyajun on 2022/9/2.
//

import Foundation

class DipSensorMagHelper {
    
    //
    // 磁场转换标准单位uT(微特)
    //
    //
    static func GetMagToUt(_ reg72:Int16,_ regMag:Double) -> Double
    {
        var dRet:Double = regMag;
        switch (reg72)
        {
        case 2:
            dRet = dRet * 0.15;
            break;
        case 3:
            dRet = dRet * 13 / 1000.0;
            break;
        case 4:
            dRet = dRet * 0.058;
            break;
        case 5:
            dRet = dRet * 0.098;
            break;
        case 6:
            dRet = dRet / 150;
            break;
        case 7:
            dRet = dRet * 20 / 1000.0;
            break;
        default:
            break
        }
        return (String(format: "%.3f", dRet) as NSString).doubleValue;
    }
    
    
}
