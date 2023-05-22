//
//  核心连接器
//
//  Created by huangyajun on 2022/8/31.
//
import Foundation

class WitCoreConnector {
    
    // 蓝牙连接
    var bleClient:BluetoothBLE?
    
    // 连接参数
    var config:ConnectConfig? = ConnectConfig()
    
    // 连接状态
    var connectType:ConnectType? = .BLE
    
    // 连接状态
    var connectStatus:ConnectStatus? = .Closed
    
    // 数据接收接口
    var dataReceivedList:[IDataReceivedObserver] = [IDataReceivedObserver]()
    
}


// 打开和关闭连接
extension WitCoreConnector{
    
    // MARK: 打开
    func open() throws{
        // 检查连接参数
        try checkConfig()
        
        // 如果是连接低功耗蓝牙
        if (connectType == ConnectType.BLE) {
            // 拿到蓝牙客户端
            bleClient = WitBluetoothManager.instance.bluetoothBLEDist?[config?.bluetoothBLEOption?.mac ?? ""]
            
            if (bleClient == nil) {
                throw CoreConnectError.ConnectError(message: "不存在的蓝牙设备")
            }
            
            // 连接蓝牙
            bleClient?.connect()
            bleClient?.registerDataRecevied(obj: self)
        }
        
        // 标记为打开的
        self.connectStatus = .Opened
    }
    
    // MARK: 检查参数
    func checkConfig() throws{
        // 如果是连接低功耗蓝牙
        if (connectType == ConnectType.BLE) {
            if (config?.bluetoothBLEOption == nil) {
                throw CoreConnectError.ConnectConfigError(message: "")
            }
            
            if (config?.bluetoothBLEOption?.mac == nil) {
                throw CoreConnectError.ConnectConfigError(message: "")
            }
        }
    }
    
    // MARK: 关闭
    func close(){
        // 标记为关闭的
        self.connectStatus = .Closed
        
        // 断开蓝牙5.0连接
        bleClient?.disconnect()
        bleClient?.removeDataRecevied(obj: self)
    }
    
    // MARK: 是不是打开的
    func isOpen() -> Bool {
        return self.connectStatus == .Opened
    }
}

// 发送数据
extension WitCoreConnector {
    
    // MARK: 发送数据
    func sendData (_ data:[UInt8]){
        // 如果是连接低功耗蓝牙
        if (connectType == ConnectType.BLE) {
            bleClient?.sendData(data)
        }
    }
    
}

// 接收数据
extension WitCoreConnector :IDataReceivedObserver{
    
    // 当收到数据时
    func onDataReceived(data: [UInt8]) {
        invokeDataRecevied(data: data)
    }
    
}

// 调用数据接收事件
extension WitCoreConnector : IDataObserved{
    
    // MARK: 调用需要接收数据的对象
    func invokeDataRecevied(data:[UInt8]){
        for item in dataReceivedList {
            item.onDataReceived(data: data)
        }
    }
    
    // MARK: 注册数据接收对象
    func registerDataRecevied(obj:IDataReceivedObserver){
        self.dataReceivedList.append(obj)
    }
    
    // MARK: 移除数据接收对象
    func removeDataRecevied(obj:IDataReceivedObserver){
        var i = 0
        while i < self.dataReceivedList.count {
            let item = self.dataReceivedList[i]
            
            if CompareObjectHelper.compareObjectMemoryAddress(item as AnyObject, obj as AnyObject){
                self.dataReceivedList.remove(at: i)
            }
            i = i + 1
        }
    }
}


// 连接类型
enum ConnectType{
    
    // 低功耗蓝牙
    case BLE
    
}

// 连接状态
enum ConnectStatus{
    
    // 打开的
    case Opened
    
    // 关闭的
    case Closed
    
}


// 连接参数
class ConnectConfig {
    
    // 低功耗蓝牙连接选项
    var bluetoothBLEOption:BluetoothBLEOption? = BluetoothBLEOption()
    
}


// 蓝牙5.0连接参数
class BluetoothBLEOption {
    
    // 蓝牙地址
    var mac:String?
    
}

// 连接错误
enum CoreConnectError: Error{
    
    // 连接参数错误
    case ConnectConfigError(message:String)
    
    // 连接错误
    case ConnectError(message:String)
    
}


// 数据接收观察者
protocol IDataReceivedObserver
{
    // 当收到数据时
    func onDataReceived(data:[UInt8])
    
}

// 数据接收被观察者
protocol IDataObserved {
    
    // MARK: 调用需要接收数据的对象
    func invokeDataRecevied(data:[UInt8])
    
    // MARK: 注册数据接收对象
    func registerDataRecevied(obj:IDataReceivedObserver)
    
    // MARK: 移除数据接收对象
    func removeDataRecevied(obj:IDataReceivedObserver)
    
}

// 数据回显接口
protocol SendDataInterface
{
    func onSendData(data:[UInt8])
}
