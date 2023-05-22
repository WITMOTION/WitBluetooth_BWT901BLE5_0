//
//  低功耗蓝牙客户端
//  
//  Created by huangyajun on 2022/8/27.
//

import Foundation
import CoreBluetooth

public class BluetoothBLE:NSObject{
    
    // 服务uuid
    var uuidService:String?
    
    // 发送特征值uuid
    var uuidSend:String?
    
    // 读取特征值uuid
    var uuidRead:String?
    
    // 蓝牙管理器
    var bluetoothManager:WitBluetoothManager = WitBluetoothManager.instance
    
    // 当前连接的设备
    public var peripheral:CBPeripheral!
    
    //发送数据特征(连接到设备之后可以把需要用到的特征保存起来，方便使用)
    var sendCharacteristic:CBCharacteristic?
    
    // 数据接收接口
    var dataReceivedList:[IDataReceivedObserver] = [IDataReceivedObserver]()
    
    // 设备地址
    public var mac:String?
    
    init(_ peripheral:CBPeripheral){
        super.init()
        self.peripheral = peripheral
        self.peripheral.delegate = self
        self.mac = peripheral.identifier.uuidString
    }
    
    // MARK: 连接蓝牙
    func connect(){
        bluetoothManager.requestConnect(self.peripheral)
    }
    
    // MARK: 当连接成功时会回掉这个方法
    func onConnected(){
        
    }
    
    // MARK: 关闭连接
    func disconnect(){
        bluetoothManager.cancelConnect(self.peripheral)
    }
    
    // MARK: 发送数据
    func sendData(_ data: Data) {

        // 非连接中则不发送
        if (peripheral.state != .connected) {
            return
        }
        
        // 没有发送特征也不发送
        if (sendCharacteristic == nil) {
            return
        }
        
        peripheral.writeValue(data , for: sendCharacteristic!, type: CBCharacteristicWriteType.withoutResponse)
    }
    
    // MARK: 发送数据
    func sendData (_ data:[UInt8]){
        sendData(Data(data))
    }
}

// 调用数据接收事件
extension BluetoothBLE : IDataObserved{
    
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


// 发现服务委托
extension BluetoothBLE : CBPeripheralDelegate {
    
    //  MARK: - 匹配对应服务UUID
    public func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?){
        
        if error != nil {
            return
        }
        
        // 遍历所有的服务
        for service in peripheral.services! {
            // 如果是指定的服务器ID则开始寻找特征值
            // print("SERVICE UUID ID:\(service.uuid.uuidString.uppercased())")
            // 如果是低功耗单模蓝牙
            if service.uuid.uuidString.uppercased() == BLEUUID.UUID_SERVICE.uppercased() {
                self.uuidService = BLEUUID.UUID_SERVICE
                self.uuidRead = BLEUUID.UUID_READ
                self.uuidSend = BLEUUID.UUID_SEND
                peripheral.discoverCharacteristics(nil, for: service )
            }
            
            // 如果是双模蓝牙
            if service.uuid.uuidString == DualUUID.UUID_SERVICE {
                self.uuidService = DualUUID.UUID_SERVICE
                self.uuidRead = DualUUID.UUID_READ
                self.uuidSend = DualUUID.UUID_SEND
                peripheral.discoverCharacteristics(nil, for: service )
            }
        }
        
    }
    
    //MARK: - 服务下的特征
    public func peripheral(_ peripheral: CBPeripheral, didDiscoverCharacteristicsFor service: CBService, error: Error?){
        
        if (error != nil){
            return
        }
        
        for  characteristic in service.characteristics! {
            print("找到设备特征值UUID：\(characteristic.uuid.description)")
            switch characteristic.uuid.description.uppercased() {
                case self.uuidRead:
                    // 订阅特征值，订阅成功后后续所有的值变化都会自动通知
                    peripheral.setNotifyValue(true, for: characteristic)
                    break
                case "******":
                    // 读区特征值，只能读到一次
                    peripheral.readValue(for:characteristic)
                    break
                case self.uuidSend:
                    // 拿到写特征值
                    sendCharacteristic = characteristic
                    break
                default:
                    // print("扫描到其他特征")
                    break
            }
            
        }
        
    }
    
    //MARK: - 特征的订阅状体发生变化
    public func peripheral(_ peripheral: CBPeripheral, didUpdateNotificationStateFor characteristic: CBCharacteristic, error: Error?){
        
        guard error == nil  else {
            return
        }
        
    }
    
    // MARK: - 获取外设发来的数据
    // 注意，所有的，不管是 read , notify 的特征的值都是在这里读取
    public func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic, error: Error?)-> (){
        
        if(error != nil){
            return
        }
        
        switch characteristic.uuid.uuidString.uppercased() {
            
        case self.uuidRead:
//            // 打印收到数据的时间戳
//            let dformatter = DateFormatter()
//            dformatter.dateFormat = "yyyyMMdd-HH.mm.ss"
//            let current = Date()
//            let dateString = dformatter.string(from: current) + ".\((CLongLong(round(current.timeIntervalSince1970*1000)) % 1000))"
//            print(dateString)
            
            // print("接收到了设备的数据: \(String(describing: characteristic.value?.dataToHex()))")
            let bytes:[UInt8]? = characteristic.value?.dataToBytes()
            if bytes != nil {
                // 调用要接收数据的对象
                invokeDataRecevied(data: bytes ?? [UInt8]())
            }
            break
        default:
            print("收到了其他数据特征数据: \(characteristic.uuid.uuidString)")
            break
        }
    }
    
    
    
    //MARK: - 检测中心向外设写数据是否成功
    public func peripheral(_ peripheral: CBPeripheral, didWriteValueFor characteristic: CBCharacteristic, error: Error?) {
        if(error != nil){
            print("发送数据失败!error信息: \(String(describing: error))")
        }
    }
    
}

// 低功耗蓝牙UUID
class BLEUUID{
    
    // 服务uuid
    static let UUID_SERVICE:String = "0000ffe5-0000-1000-8000-00805f9a34fb".uppercased()
    
    // 发送特征值uuid
    static let UUID_SEND:String = "0000ffe9-0000-1000-8000-00805f9a34fb".uppercased()
    
    // 读取特征值uuid
    static let UUID_READ:String = "0000ffe4-0000-1000-8000-00805f9a34fb".uppercased()
}


// 双模蓝牙UUID
class DualUUID{
    
    // 服务uuid
    static let UUID_SERVICE:String = "49535343-fe7d-4ae5-8fa9-9fafd205e455".uppercased()
    
    // 发送特征值uuid
    static let UUID_SEND:String = "49535343-8841-43f4-a8d4-ecbe34729bb3".uppercased()
    
    // 读取特征值uuid
    static let UUID_READ:String = "49535343-1e4d-4bd9-ba61-23c647249616".uppercased()
}


