//
//  蓝牙管理器
//
//  Created by huangyajun on 2022/8/27.
//


import Foundation
import CoreBluetooth

//用于看发送数据是否成功!`
public class WitBluetoothManager:NSObject {
    
    // 单例对象
    public static let instance = WitBluetoothManager()
    
    // 中心对象
    var central: CBCentralManager?
    
    // 中心扫描到的设备都可以保存起来，
    // 扫描到新设备后可以通过通知的方式发送出去，连接设备界面可以接收通知，实时刷新设备列表
    var deviceList: NSMutableArray?
    
    // 低功耗蓝牙客户端
    var bluetoothBLEDist: [String:BluetoothBLE]?
    
    // 观察者集合
    var observerList:[IBluetoothEventObserver] = [IBluetoothEventObserver]()
    
    // 是否扫描中
    public var isScaning = false
    
    // MARK: 构造方法
    private override init() {
        
        super.init()
        
        self.central = CBCentralManager.init(delegate:self, queue:nil, options:[CBCentralManagerOptionShowPowerAlertKey:false])
        self.deviceList = NSMutableArray()
        self.bluetoothBLEDist = [String:BluetoothBLE]()
        
    }
    
    // MARK: 扫描设备的方法,指定搜索条件
    public func startScan(_ serviceUUIDS:[CBUUID]?, options:[String: AnyObject]?){
        // 清除所有已经找到的设备
        self.bluetoothBLEDist?.removeAll()
        // 开启扫描
        self.central?.scanForPeripherals(withServices: serviceUUIDS, options: options)
        // 标识为扫描中
        self.isScaning = true
    }
    
    // MARK: 扫描设备的方法，无搜索条件会搜索到周围所有低功耗蓝牙设备
    public func startScan(){
        // 清除所有已经找到的设备
        self.bluetoothBLEDist?.removeAll()
        // 开启扫描
        self.central?.scanForPeripherals(withServices: nil, options: nil)
        // 标识为扫描中
        self.isScaning = true
    }
    
    // MARK: 停止扫描
    public func stopScan() {
        self.isScaning = false
        self.central?.stopScan()
    }
    
    // MARK: 请求连接
    func requestConnect(_ model:CBPeripheral) {
        if (model.state != CBPeripheralState.connected) {
            central?.connect(model , options: nil)
        }
    }
    
    // MARK: 取消连接
    func cancelConnect(_ model:CBPeripheral) {
        if (model.state == CBPeripheralState.connected) {
            central?.cancelPeripheralConnection(model)
        }
    }
    
}



// MARK: 注册监听蓝牙事件
extension WitBluetoothManager{
    
    // MARK: 添加蓝牙事件观察者
    public func registerEventObserver(observer: IBluetoothEventObserver){
        self.observerList.append(observer)
    }
    
    // MARK: 移除指定蓝牙事件观察者
    public func removeEventObserver(observer: IBluetoothEventObserver){
        var i = 0
    
        // 遍历移除设备
        while i < self.observerList.count {
            let item = self.observerList[i]
            if CompareObjectHelper.compareObjectMemoryAddress(item as AnyObject, observer as AnyObject) {
                self.observerList.remove(at: i)
                continue
            }
            i = i + 1
        }
    }
    
    
    // MARK: 移除所有蓝牙事件观察者
    public func removeAllObserver(){
        self.observerList.removeAll()
    }
    
    
    // MARK: 通知蓝牙事件观察者，找到了低功耗蓝牙设备
    func notifyObserverOnFoundBle(_ bluetoothBLE: BluetoothBLE?){
        for item in self.observerList{
            item.onFoundBle(bluetoothBLE: bluetoothBLE)
        }
    }
    
    
    // MARK: 通知蓝牙事件观察者，低功耗蓝牙连接成功
    func notifyObserverOnConnected(_ bluetoothBLE: BluetoothBLE?){
        for item in self.observerList{
            item.onConnected(bluetoothBLE: bluetoothBLE)
        }
    }
    
}



// MARK: -- 中心管理器的代理
extension WitBluetoothManager : CBCentralManagerDelegate{
    
    // MARK: 检查运行这个App的设备是不是支持BLE。
    public func centralManagerDidUpdateState(_ central: CBCentralManager){
        
        if #available(iOS 10.0, *) {
            switch central.state {
                
            case CBManagerState.poweredOn:
                print("当前设备蓝牙状态：打开的")
                
            case CBManagerState.unauthorized:
                print("当前设备蓝牙状态：没有蓝牙功能")
                
            case CBManagerState.poweredOff:
                print("当前设备蓝牙状态：关闭的")
                
            default:
                print("当前设备蓝牙状态：未知状态")
            }
        }
        // 手机蓝牙状态发生变化，可以发送通知出去。提示用户
        
    }
    
    
    // MARK: 中心管理器扫描到了设备   开始扫描之后会扫描到蓝牙设备，扫描到之后走到这个代理方法
    public func centralManager(_ central: CBCentralManager, didDiscover peripheral: CBPeripheral, advertisementData: [String : Any], rssi RSSI: NSNumber) {
        
        //  在这个地方可以判读是不是自己本公司的设备,这个是根据设备的名称过滤的
        guard peripheral.name != nil , peripheral.name!.contains("WT") else {
            return
        }
        
        var ble:BluetoothBLE?
        
        //  这里判断重复，加到设备列表中。发出通知。
        if self.bluetoothBLEDist?.keys.contains(peripheral.identifier.uuidString) == false {
            self.bluetoothBLEDist?[peripheral.identifier.uuidString] = BluetoothBLE(peripheral)
        }
        
        // 拿到蓝牙5.0连接对象
        ble = self.bluetoothBLEDist?[peripheral.identifier.uuidString]
        
        // 通知观察者已经搜索到了设备
        notifyObserverOnFoundBle(ble)
    }
    
    
    // MARK: 连接外设成功，开始发现服务
    public func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral){
        
        // 开始发现服务
        peripheral.discoverServices(nil)
        
        // 这里可以发通知出去告诉设备连接界面连接成功
        let ble = self.bluetoothBLEDist?[peripheral.identifier.uuidString]
        if(ble != nil){
            //ble?.onConnected()
            //notifyObserverOnConnected(ble)
        }
    }
    
    
    // MARK: 连接外设失败
    public func centralManager(_ central: CBCentralManager, didFailToConnect peripheral: CBPeripheral, error: Error?) {
        
        // 这里可以发通知出去告诉设备连接界面连接失败
        
    }
    
    
    // MARK: 连接丢失
    public func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        NotificationCenter.default.post(name: Notification.Name(rawValue: "DidDisConnectPeriphernalNotification"), object: nil, userInfo: ["deviceList": self.deviceList as AnyObject])
        
        // 这里可以发通知出去告诉设备连接界面连接丢失
        
    }
    
}
