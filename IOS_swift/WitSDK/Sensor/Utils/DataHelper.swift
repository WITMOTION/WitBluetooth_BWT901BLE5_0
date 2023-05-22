//
//  数据帮助类
//  Wit-Example-BLE
//
//  Created by huangyajun on 2022/8/31.
//

import Foundation


class DataHelper {
    
    // 将十六进制字符串转化为 Data
    static func data(from hexStr: String) -> Data {
        var hexStr1 = ""
        if hexStr.count % 2 != 0 {
            hexStr1 = "0" + hexStr
        }else {
            hexStr1 = hexStr
        }
        let bytes = hexToBytes(from: hexStr1)
        return Data(bytes)
    }
    
    // 16进制字符串转byte数组
    static func hexToBytes(from hexStr: String) -> [UInt8] {
        assert(hexStr.count % 2 == 0, "输入字符串格式不对，8位代表一个字符")
        var bytes = [UInt8]()
        var sum = 0
        // 整形的 utf8 编码范围
        let intRange = 48...57
        // 小写 a~f 的 utf8 的编码范围
        let lowercaseRange = 97...102
        // 大写 A~F 的 utf8 的编码范围
        let uppercasedRange = 65...70
        for (index, c) in hexStr.utf8CString.enumerated() {
            var intC = Int(c.byteSwapped)
            if intC == 0 {
                break
            } else if intRange.contains(intC) {
                intC -= 48
            } else if lowercaseRange.contains(intC) {
                intC -= 87
            } else if uppercasedRange.contains(intC) {
                intC -= 55
            } else {
                assertionFailure("输入字符串格式不对，每个字符都需要在0~9，a~f，A~F内")
            }
            sum = sum * 16 + intC
            // 每两个十六进制字母代表8位，即一个字节
            if index % 2 != 0 {
                bytes.append(UInt8(sum))
                sum = 0
            }
        }
        //    print(bytes)
        print(bytes.count)
        return bytes
    }
}


extension String {
    
    // hex 转 data
    func hexToData() -> Data? {
        var data = Data(capacity: self.count / 2)
        
        let regex = try! NSRegularExpression(pattern: "[0-9a-f]{1,2}", options: .caseInsensitive)
        regex.enumerateMatches(in: self, range: NSMakeRange(0, utf16.count)) { match, flags, stop in
            let byteString = (self as NSString).substring(with: match!.range)
            var num = UInt8(byteString, radix: 16)!
            data.append(&num, count: 1)
        }
        
        guard data.count > 0 else { return nil }
        
        return data
    }
    
}


extension Data {
    
    // data 转 hex
    func dataToHex() -> String {
        return map { String(format: "%02x", $0) }
            .joined(separator: "")
    }
    
    // data 转 bytes
    func dataToBytes() -> [UInt8] {
        return [UInt8](self)
    }
}

