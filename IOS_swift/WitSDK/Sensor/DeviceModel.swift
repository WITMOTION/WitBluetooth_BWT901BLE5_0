//
//  设备模型
//
//  Created by huangyajun on 2022/8/27.
//

import Foundation

public class DeviceModel {
    
    // MARK: 设备名称
    var deviceName:String?
    
    // MARK: 监听的key值
    var listenerKey:String?
    
    // MARK: 连接器
    var coreConnect:WitCoreConnector?
    
    // MARK: 数据处理器
    var dataProcessor:IDataProcessor
    
    // MARK: 协议解析器
    var protocolResolver:IProtocolResolver
    
    // MARK: 是否打开的
    var isOpen:Bool = false
    
    // MARK: 是否关闭中
    var closing:Bool = false
    
    // MARK: 设备数据
    var deviceData:[String:String] = [String:String]()
    
    // 修改设备数据锁
    let deviceDataLock = NSLock()
    
    // 发送数据锁
    let sendDataLock = NSLock()

    // 是否等待返回
    var bolWaitReturn = false
    
    // 返回的数据
    var returnDataBuffer:[UInt8] = [UInt8]()
    
    // 接收数据锁
    // let returnDataBufferLock = NSLock()
    
    // 监听的key刷新观察者
    var listenKeyUpdateObserverList:[IListenKeyUpdateObserver] = [IListenKeyUpdateObserver]()
    
    // key刷新事件观察者
    var keyUpdateObserverList:[IKeyUpdateObserver] = [IKeyUpdateObserver]()
    
    // MARK: 构造方法
    init(deviceName:String,protocolResolver:IProtocolResolver,dataProcessor:IDataProcessor, listenerKey:String){
        self.deviceName = deviceName
        self.protocolResolver = protocolResolver
        self.dataProcessor = dataProcessor
        self.listenerKey = listenerKey
    }
}

// 打开设备和关闭设操作
extension DeviceModel{
    
    // MARK: 设置连接对象
    func setCoreConnector(coreConnector:WitCoreConnector){
        self.coreConnect = coreConnector
    }
    
    // MARK: 打开设备
    func openDevice() throws{
        
        if (coreConnect != nil) {
            // 打开连接器
            try coreConnect?.open()
            coreConnect?.registerDataRecevied(obj: self)
            
            // 调用数据处理打开方法
            dataProcessor.onOpen(deviceModel: self)
            
        }else{
            throw DeviceModelError.openError(msaage: "打开设备错误,没有连接对象")
        }
        
    }
    
    // MARK: 重新打开
    func reOpen() throws{
        self.closeDevice()
        try self.openDevice()
    }
    
    // MARK: 关闭设备
    func closeDevice(){
        // 调用数据处理关闭方法
        dataProcessor.onClose()
        // 关闭连接器
        coreConnect?.close()
        coreConnect?.removeDataRecevied(obj: self)
    }
}

// 设备数据操作
extension DeviceModel{
    
    // MARK: 设置数据
    func setDeviceData(_ key:String,_ value:String){
        
        deviceDataLock.lock()
        deviceData[key] = value
        deviceDataLock.unlock()
        
        // 触发监听的key值
        if key == listenerKey {
            // 调用数据处理器更新数据
            dataProcessor.onUpdate(deviceModel: self)
            // 调用key更新观察者
            invokeListenKeyUpdateObserver(self)
        }
        
        // 调用监听key更新观察者
        invokeKeyUpdateObserver(self, key, value)
    }
    
    // MARK: 获得设备数据
    func getDeviceData(_ key:String) -> String?{
        
        var value:String? = nil
        
        deviceDataLock.lock()
        // 如果不包含就返回空
        if deviceData.keys.contains(key) {
            value = deviceData[key]
        }
        
        deviceDataLock.unlock()
        
        return value
    }
}


// 收到数据时
extension DeviceModel:IDataReceivedObserver{
    // MARK: 当收到设备的数据时
    func onDataReceived(data: [UInt8]) {
        
        // 如果在等待返回
        if (bolWaitReturn) {
            //returnDataBufferLock.lock()
            returnDataBuffer.append(contentsOf: data)
            //returnDataBufferLock.unlock()
        }
        
        // MARK: 调用协议处理器
        protocolResolver.passiveReceiveData(data: data, deviceModel: self)
    }
}


// 发送数据
extension DeviceModel {
    
    // MARK: 发送数据，需要返回数据
    func sendData(data: [UInt8], callback:(_ rtnData:[UInt8]) -> Void, waitTime:Int64) throws {
        // 开启线程锁
        sendDataLock.lock()
        bolWaitReturn = true
        returnDataBuffer.removeAll()
        do{
            // 发送读取命令
            try sendData(data: data)
            // 等待返回
            Thread.sleep(forTimeInterval: Double(waitTime) / 1000.0)
            
            bolWaitReturn = false
            // 调用回掉方法
            let copyList = returnDataBuffer
            callback(copyList)
        } catch {
            bolWaitReturn = false
        }
        // 取消线程锁
        sendDataLock.unlock()
    }
    
    // MARK: 发送数据, 不需要返回数据
    func sendData(data: [UInt8]) throws{
        coreConnect?.sendData(data)
    }
    
    // MARK: 发送协议数据 (同步)
    func sendProtocolData(_ data: [UInt8],_ waitTime:Int64) throws{
        try self.protocolResolver.sendData(sendData: data, deviceModel: self, waitTime: waitTime)
    }
    
    // MARK: 发送协议数据 (异步)
    func asyncSendProtocolData(_ data: [UInt8],_ waitTime:Int64,_ callback:@escaping () -> Void) throws{
        // 启动一个线程
        let thread = Thread(block: {
            do{
                try self.sendProtocolData(data, waitTime)
                callback()
            }catch{
                
            }
        })
        thread.start()
    }
}


// 事件处理
extension DeviceModel {
    
    // MARK: 调用key更新观察者
    func invokeKeyUpdateObserver(_ deviceModel:DeviceModel, _ key:String,_ value:String){
        for item in self.keyUpdateObserverList {
            item.onKeyUpdate(deviceModel, key, value)
        }
    }
    
    // MARK: 注册key更新观察者
    func registerKeyUpdateObserver(_ obj:IKeyUpdateObserver){
        self.keyUpdateObserverList.append(obj)
    }
    
    // MARK: 移除key更新观察者
    func removeKeyUpdateObserver(_ obj:IKeyUpdateObserver){
        var i = 0
        while i < self.keyUpdateObserverList.count {
            let item = self.keyUpdateObserverList[i]
            
            if CompareObjectHelper.compareObjectMemoryAddress(item as AnyObject, obj as AnyObject){
                self.keyUpdateObserverList.remove(at: i)
            }
            i = i + 1
        }
    }
    
    // MARK: 调用监听key更新观察者
    func invokeListenKeyUpdateObserver(_ deviceModel:DeviceModel){
        for item in self.listenKeyUpdateObserverList {
            item.onListenKeyUpdate(deviceModel)
        }
    }
    
    // MARK: 注册监听key更新观察者
    func registerListenKeyUpdateObserver(obj:IListenKeyUpdateObserver){
        self.listenKeyUpdateObserverList.append(obj)
    }
    
    // MARK: 移除监听key更新观察者
    func removeListenKeyUpdateObserver(obj:IListenKeyUpdateObserver){
        var i = 0
        while i < self.listenKeyUpdateObserverList.count {
            let item = self.listenKeyUpdateObserverList[i]
            
            if CompareObjectHelper.compareObjectMemoryAddress(item as AnyObject, obj as AnyObject){
                self.listenKeyUpdateObserverList.remove(at: i)
            }
            i = i + 1
        }
    }
}
