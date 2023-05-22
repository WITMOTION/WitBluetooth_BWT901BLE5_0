//
//  Bwt901ble传感器对象
//  Wit-Example-BLE
//
//  Created by huangyajun on 2022/8/29.
//

import Foundation
import SwiftUI
import CoreBluetooth

public class Bwt901ble :Identifiable, ObservableObject{
    
    // 蓝牙管理器
    var bluetoothManager: WitBluetoothManager = WitBluetoothManager.instance
    
    // 蓝牙连接对象
    var bluetoothBLE: BluetoothBLE?
    
    // 设备模型
    var deviceModel:DeviceModel?
    
    // 数据记录观察者
    var recordObserverList:[IBwt901bleRecordObserver] = [IBwt901bleRecordObserver]()
    
    // 名称
    @Published public var name:String?
    
    // 蓝牙地址
    @Published public var mac:String?
    
    // 当前是否打开的
    @Published public var isOpen: Bool = false
    
    // MARK: 构造方法
    public init(bluetoothBLE: BluetoothBLE?){
        self.bluetoothBLE = bluetoothBLE
        self.name = bluetoothBLE?.peripheral.name
        self.mac = bluetoothBLE?.peripheral.identifier.uuidString
        
        // 设备模型
        deviceModel = DeviceModel(
            deviceName: self.mac ?? "",
            protocolResolver: BWT901BLE5_0ProtocolResolver(),
            dataProcessor: BWT901BLE5_0DataProcessor(),
            listenerKey: "61_0"
        )
        
        // 核心连接器
        let coreConnector = WitCoreConnector()
        coreConnector.config?.bluetoothBLEOption?.mac = self.mac
        deviceModel?.setCoreConnector(coreConnector: coreConnector)
    }
    
    // MARK: 打开设备
    public func openDevice() throws{
        try deviceModel?.openDevice()
        // 监听数据
        deviceModel?.registerListenKeyUpdateObserver(obj: self)
    }
    
    // MARK: 关闭设备
    public func closeDevice(){
        deviceModel?.closeDevice()
        // 取消监听数据
        deviceModel?.removeListenKeyUpdateObserver(obj: self)
    }

    // MARK: 获得设备数据
    public func getDeviceData(_ key:String) -> String?{
        return deviceModel?.getDeviceData(key)
    }
}

// 控制设备
extension Bwt901ble {
    
    // MARK: 加计校准
    public func appliedCalibration() throws{
        try sendData([0xFF ,0xAA ,0x01 ,0x01 ,0x00], 10)
    }
    
    // MARK: 开始磁场校准
    public func startFieldCalibration() throws{
        try sendData([0xFF ,0xAA ,0x01 ,0x07 ,0x00], 10)
    }
    
    // MARK: 结束磁场校准
    public func endFieldCalibration() throws{
        try sendData([0xFF ,0xAA ,0x01 ,0x00 ,0x00], 10)
    }
    
    // MARK: 发送协议数据
    public func sendProtocolData(_ data:[UInt8],_ waitTime:Int64) throws{
        try deviceModel?.sendProtocolData(data, waitTime)
    }
    
    // MARK: 发送数据
    public func sendData(_ data:[UInt8],_ waitTime:Int64) throws{
       try deviceModel?.sendData(data: data)
        Thread.sleep(forTimeInterval: (Double(waitTime) / 1000))
    }
    
    // MARK: 读取寄存器
    public func readRge(_ data:[UInt8],_ waitTime:Int64,_ callback: @escaping () -> Void) throws{
        try deviceModel?.asyncSendProtocolData(data, waitTime, callback)
    }
    
    // MARK: 写入寄存器
    public func writeRge(_ data:[UInt8],_ waitTime:Int64) throws{
        try sendData(data, waitTime)
    }
    
    // MARK: 解锁寄存器
    public func unlockReg() throws{
        try sendData([0xFF ,0xAA ,0x69 ,0x88 ,0xB5], 10)
    }
    
    // MARK: 保存寄存器
    public func saveReg() throws{
        try sendData([0xFF ,0xAA ,0x00 ,0x00 ,0x00], 10)
    }
    
}

// 操作数据记录观察者
extension Bwt901ble :IListenKeyUpdateObserver{
    
    // MARK: 设备模型监听的key刷新时调用这里
    public func onListenKeyUpdate(_ deviceModel: DeviceModel) {
        invokeListenKeyUpdateObserver(self)
    }
   
    // MARK: 调用数据记录观察者
    public func invokeListenKeyUpdateObserver(_ bwt901ble:Bwt901ble){
        for item in self.recordObserverList {
            item.onRecord(bwt901ble)
        }
    }
    
    // MARK: 注册数据记录观察者
    public func registerListenKeyUpdateObserver(obj:IBwt901bleRecordObserver){
        self.recordObserverList.append(obj)
    }
    
    // MARK: 移除数据记录观察者
    public func removeListenKeyUpdateObserver(obj:IBwt901bleRecordObserver){
        var i = 0
        while i < self.recordObserverList.count {
            let item = self.recordObserverList[i]
            
            if CompareObjectHelper.compareObjectMemoryAddress(item as AnyObject, obj as AnyObject){
                self.recordObserverList.remove(at: i)
            }
            i = i + 1
        }
    }
    
}


enum SensorLabel: String, CaseIterable {
    case left_hand = "left hand"
    case right_hand = "right hand"
    case waist = "waist"
    case left_leg = "left leg"
    case right_leg = "right leg"
}
