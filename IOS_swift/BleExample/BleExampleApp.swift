//
//  欢迎您使用维特智能蓝牙5.0示例程序
//  1.为了方便您使用，本程序只有这一个代码文件
//  2.本程序适用于维特智能蓝牙5.0倾角传感器
//  3.本程序将演示如何获得传感器的数据和控制传感器
//  4.如果您有疑问可以查看程序配套说明文档，或者咨询我们技术人员
//
//  Welcome to the Witte Smart Bluetooth 5.0 sample program
//  1. For your convenience, this program has only this code file
//  2. This program is suitable for Witte Smart Bluetooth 5.0 inclination sensor
//  3. This program will demonstrate how to obtain sensor data and control the sensor
//  4. If you have any questions, you can check the program supporting documentation, or consult our technical staff
//
//  Created by huangyajun on 2022/8/26.
//


import SwiftUI
import CoreBluetooth
import WitSDK


// **********************************************************
// MARK: App主视图
// MARK: App main view
// **********************************************************
@main
struct AppMainView : App {
    
    // MARK: tab页面枚举
    // MARK: tab page enumeration
    enum Tab {
        case connect
        case home
    }
    
    // MARK: 当前选择的tab页面
    // MARK: The currently selected tab page
    @State private var selection: Tab = .home
    
    // MARK: App上下文
    // MARK: App the context
    var appContext:AppContext = AppContext()
    
    // MARK: UI页面
    // MARK: UI Page
    var body: some Scene {
        WindowGroup {
            if (UIDevice.current.userInterfaceIdiom == .phone){
                TabView(selection: $selection) {
                    NavigationView {
                        ConnectView(appContext)
                            
                    }
                    .tabItem {
                        Label {
                            Text("连接设备 Connect the device", comment: "在这连接设备 Connect device here")
                        } icon: {
                            Image(systemName: "list.bullet")
                        }
                    }
                    .tag(Tab.connect)
                    
                    NavigationView {
                        HomeView(appContext)
                    }
                    .tabItem {
                        Label {
                            Text("设备数据 device data", comment: "在这查看设备的数据 View device data here")
                        } icon: {
                            Image(systemName: "heart.fill")
                        }
                    }
                    .tag(Tab.connect)
                }
            } else {
                NavigationView{
                    List{
                        NavigationLink() {
                            ConnectView(appContext)
                        } label: {
                            Label("连接设备 Connect the device", systemImage: "list.bullet")
                        }
                        
                        NavigationLink() {
                            HomeView(appContext)
                        } label: {
                            Label("主页面 main page", systemImage: "heart")
                        }
                    }
                }
            }
        }
    }
}


// **********************************************************
// MARK: App上下文
// MARK: App the context
// **********************************************************
class AppContext: ObservableObject ,IBluetoothEventObserver, IBwt901bleRecordObserver{
    
    // 获得蓝牙管理器
    // Get bluetooth manager
    var bluetoothManager:WitBluetoothManager = WitBluetoothManager.instance
    
    // 是否扫描设备中
    // Whether to scan the device
    @Published
    var enableScan = false
    
    // 蓝牙5.0传感器对象
    // Bluetooth 5.0 sensor object
    @Published
    var deviceList:[Bwt901ble] = [Bwt901ble]()
    
    // 要显示的设备数据
    // Device data to display
    @Published
    var deviceData:String = "未连接设备 device not connected"
    
    init(){
        // 当前扫描状态
        // Current scan status
        self.enableScan = self.bluetoothManager.isScaning
        // 开启自动刷新线程
        // start auto refresh thread
        startRefreshThread()
    }
    
    // MARK: 开始扫描设备
    // MARK: Start scanning for devices
    func scanDevices() {
        print("开始扫描周围蓝牙设备 Start scanning for surrounding bluetooth devices")
        // 移除所有的设备，在这里会关闭所有设备并且从列表中移除
        // Remove all devices, here all devices are turned off and removed from the list
        removeAllDevice()
        // 注册蓝牙事件观察者
        // Registering a Bluetooth event observer
        self.bluetoothManager.registerEventObserver(observer: self)
        // 开启蓝牙扫描
        // Turn on bluetooth scanning
        self.bluetoothManager.startScan()
    }
    
    // MARK: 如果找到低功耗蓝牙传感器会调用这个方法
    // MARK: This method is called if a Bluetooth Low Energy sensor is found
    func onFoundBle(bluetoothBLE: BluetoothBLE?) {
        if isNotFound(bluetoothBLE) {
            print("\(String(describing: bluetoothBLE?.peripheral.name)) 找到一个蓝牙设备 found a bluetooth device")
            self.deviceList.append(Bwt901ble(bluetoothBLE: bluetoothBLE))
        }
    }
    
    // 判断设备还未找到
    // Judging that the device has not been found
    func isNotFound(_ bluetoothBLE: BluetoothBLE?) -> Bool{
        for device in deviceList {
            if device.mac == bluetoothBLE?.mac {
                return false
            }
        }
        return true
    }
    
    // MARK: 当连接成功时会在这里通知您
    // MARK: You will be notified here when the connection is successful
    func onConnected(bluetoothBLE: BluetoothBLE?) {
        print("\(String(describing: bluetoothBLE?.peripheral.name)) 连接成功")
    }
    
    // MARK: 当连接失败时会在这里通知您
    // MARK: Notifies you here when the connection fails
    func onConnectionFailed(bluetoothBLE: BluetoothBLE?) {
        print("\(String(describing: bluetoothBLE?.peripheral.name)) 连接失败")
    }
    
    // MARK: 当连接断开时会在这里通知您
    // MARK: You will be notified here when the connection is lost
    func onDisconnected(bluetoothBLE: BluetoothBLE?) {
        print("\(String(describing: bluetoothBLE?.peripheral.name)) 连接断开")
    }
    
    // MARK: 停止扫描设备
    // MARK: Stop scanning for devices
    func stopScan(){
        // 删除蓝牙事件观察器
        self.bluetoothManager.removeEventObserver(observer: self)
        // 移除监听新找到的传感器
        self.bluetoothManager.stopScan()
    }
    
    // MARK: 打开设备
    // MARK: Turn on the device
    func openDevice(bwt901ble: Bwt901ble?){
        print("打开设备 MARK: Turn on the device")
        
        do {
            try bwt901ble?.openDevice()
            // 监听数据
            // Monitor data
            bwt901ble?.registerListenKeyUpdateObserver(obj: self)
        }
        catch{
            print("打开设备失败 Failed to open device")
        }
    }
    
    // MARK: 移除所有设备
    // MARK: Remove all devices
    func removeAllDevice(){
        for item in deviceList {
            closeDevice(bwt901ble: item)
        }
        deviceList.removeAll()
    }
    
    // MARK: 关闭设备
    // MARK: Turn off the device
    func closeDevice(bwt901ble: Bwt901ble?){
        print("关闭设备 Turn off the device")
        bwt901ble?.closeDevice()
    }
    
    // MARK: 当需要记录传感器的数据时会在这里通知您
    // MARK: You will be notified here when data from the sensor needs to be recorded
    func onRecord(_ bwt901ble: Bwt901ble) {
        // 您可以在这里获得传感器的数据
        // You can get sensor data here
        let deviceData =  getDeviceDataToString(bwt901ble)
        // 打印到控制台,您也可以在这里把数据记录到您的文件中
        // Prints to the console, where you can also log the data to your file
        print(deviceData)
    }
    
    // MARK: 开启自动执行线程
    // MARK: Enable automatic execution thread
    func startRefreshThread(){
        // 启动一个线程 start a thread
        let thread = Thread(target: self,
                            selector: #selector(refreshView),
                            object: nil)
        thread.start()
    }
    
    // MARK: 刷新视图线程,会在这里刷新传感器数据显示在页面上
    // MARK: Refresh the view thread, which will refresh the sensor data displayed on the page here
    @objc func refreshView (){
        // 一直运行这个线程
        // Keep running this thread
        while true {
            // 每秒刷新5次
            // Refresh 5 times per second
            Thread.sleep(forTimeInterval: 1 / 5)
            // 临时保存传感器数据
            // Temporarily save sensor data
            var tmpDeviceData:String = ""
            // 打印每一个设备的数据
            // Print the data of each device
            for device in deviceList {
                if (device.isOpen){
                    // 获得设备的数据，并且拼接为字符串
                    // Get the data of the device and concatenate it into a string
                    let deviceData =  getDeviceDataToString(device)
                    tmpDeviceData = "\(tmpDeviceData)\r\n\(deviceData)"
                }
            }
            
            // 刷新ui
            // Refresh ui
            DispatchQueue.main.async {
                self.deviceData = tmpDeviceData
            }
            
        }
    }
    
    // MARK: 获得设备的数据，并且拼接为字符串
    // MARK: Get the data of the device and concatenate it into a string
    func getDeviceDataToString(_ device:Bwt901ble) -> String {
        var s = ""
        s  = "\(s)name:\(device.name ?? "")\r\n"
        s  = "\(s)mac:\(device.mac ?? "")\r\n"
        s  = "\(s)version:\(device.getDeviceData(WitSensorKey.VersionNumber) ?? "")\r\n"
        s  = "\(s)AX:\(device.getDeviceData(WitSensorKey.AccX) ?? "") g\r\n"
        s  = "\(s)AY:\(device.getDeviceData(WitSensorKey.AccY) ?? "") g\r\n"
        s  = "\(s)AZ:\(device.getDeviceData(WitSensorKey.AccZ) ?? "") g\r\n"
        s  = "\(s)GX:\(device.getDeviceData(WitSensorKey.GyroX) ?? "") °/s\r\n"
        s  = "\(s)GY:\(device.getDeviceData(WitSensorKey.GyroY) ?? "") °/s\r\n"
        s  = "\(s)GZ:\(device.getDeviceData(WitSensorKey.GyroZ) ?? "") °/s\r\n"
        s  = "\(s)AngX:\(device.getDeviceData(WitSensorKey.AngleX) ?? "") °\r\n"
        s  = "\(s)AngY:\(device.getDeviceData(WitSensorKey.AngleY) ?? "") °\r\n"
        s  = "\(s)AngZ:\(device.getDeviceData(WitSensorKey.AngleZ) ?? "") °\r\n"
        s  = "\(s)HX:\(device.getDeviceData(WitSensorKey.MagX) ?? "") μt\r\n"
        s  = "\(s)HY:\(device.getDeviceData(WitSensorKey.MagY) ?? "") μt\r\n"
        s  = "\(s)HZ:\(device.getDeviceData(WitSensorKey.MagZ) ?? "") μt\r\n"
        s  = "\(s)Electric:\(device.getDeviceData(WitSensorKey.ElectricQuantityPercentage) ?? "") %\r\n"
        s  = "\(s)Temp:\(device.getDeviceData(WitSensorKey.Temperature) ?? "") °C\r\n"
        return s
    }
    
    // MARK: 加计校准
    // MARK: Addition calibration
    func appliedCalibration(){
        for device in deviceList {
            
            do {
                // 解锁寄存器
                // Unlock register
                try device.unlockReg()
                // 加计校准
                // Addition calibration
                try device.appliedCalibration()
                // 保存
                // save
                try device.saveReg()
                
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
    
    // MARK: 开始磁场校准
    // MARK: Start magnetic field calibration
    func startFieldCalibration(){
        for device in deviceList {
            do {
                // 解锁寄存器
                // Unlock register
                try device.unlockReg()
                // 开始磁场校准
                // Start magnetic field calibration
                try device.startFieldCalibration()
                // 保存
                // save
                try device.saveReg()
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
    
    // MARK: 结束磁场校准
    // MARK: End magnetic field calibration
    func endFieldCalibration(){
        for device in deviceList {
            do {
                // 解锁寄存器
                // Unlock register
                try device.unlockReg()
                // 结束磁场校准
                // End magnetic field calibration
                try device.endFieldCalibration()
                // 保存
                // save
                try device.saveReg()
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
    
    // MARK: 读取03寄存器
    // MARK: Read the 03 register
    func readReg03(){
        for device in deviceList {
            do {
                // 读取03寄存器，等待200ms，如果没读到可以把读取时间延长或多读几次
                // Read the 03 register and wait for 200ms. If it is not read out, you can extend the reading time or read it several times
                try device.readRge([0xff ,0xaa, 0x27, 0x03, 0x00], 200, {
                    let reg03value = device.getDeviceData("03")
                    // 输出结果到控制台
                    // Output the result to the console
                    print("\(String(describing: device.mac)) reg03value: \(String(describing: reg03value))")
                })
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
    
    // MARK: 设置50hz回传
    // MARK: Set 50hz postback
    func setBackRate50hz(){
        for device in deviceList {
            do {
                // 解锁寄存器
                // unlock register
                try device.unlockReg()
                // 设置50hz回传,并等待10ms
                // Set 50hz postback and wait 10ms
                try device.writeRge([0xff ,0xaa, 0x03, 0x08, 0x00], 10)
                // 保存
                // save
                try device.saveReg()
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
    
    // MARK: 设置10hz回传
    // MARK: Set 10hz postback
    func setBackRate10hz(){
        for device in deviceList {
            do {
                // 解锁寄存器
                // unlock register
                try device.unlockReg()
                // 设置10hz回传,并等待10ms
                // Set 10hz postback and wait 10ms
                try device.writeRge([0xff ,0xaa, 0x03, 0x06, 0x00], 100)
                // 保存
                // save
                try device.saveReg()
            }catch{
                print("设置失败 Set failed")
            }
        }
    }
}

// **********************************************************
// MARK: Home视图开始
// MARK: Home view start
// **********************************************************
struct HomeView: View {
    
    // App上下文
    // App the context
    @ObservedObject var viewModel:AppContext
    
    // MARK: 构造方法
    // MARK: Constructor
    init(_ viewModel:AppContext) {
        // 视图模型
        // View model
        self.viewModel = viewModel
    }
    
    // MARK: UI界面
    // MARK: UI page
    var body: some View {
        ZStack(alignment: .leading) {
            VStack(alignment: .center){
                HStack {
                    Text("控制设备 Control device")
                        .font(.title)
                }
                HStack{
                    VStack{
                        Button("加计校准 Acc cali") {
                            viewModel.appliedCalibration()
                        }.padding(10)
                        Button("开始磁场校准 Start mag cali"){
                            viewModel.startFieldCalibration()
                        }.padding(10)
                        Button("结束磁场校准 Stop mag cali"){
                            viewModel.endFieldCalibration()
                        }.padding(10)
                    }
                    VStack{
                        Button("读取03寄存器 Read 03 reg"){
                            viewModel.readReg03()
                        }.padding(10)
                        Button("设置50hz回传 Set 50hz rate"){
                            viewModel.setBackRate50hz()
                        }.padding(10)
                        Button("设置10hz回传 Set 10hz rate"){
                            viewModel.setBackRate10hz()
                        }.padding(10)
                    }
                }
                
                HStack {
                    Text("设备数据 Device data")
                        .font(.title)
                }
                ScrollViewReader { proxy in
                    List{
                        Text(self.viewModel.deviceData)
                            .fontWeight(.light)
                            .font(.body)
                    }
                }
            }
        }.navigationBarHidden(true)
    }
}


struct Home_Previews: PreviewProvider {
    static var previews: some View {
        HomeView(AppContext())
    }
}


// **********************************************************
// MARK: 接视图开始
// MARK: Start with the view
// **********************************************************
struct ConnectView: View {
    
    // App上下文
    // App the context
    @ObservedObject var viewModel:AppContext
    
    // MARK: 构造方法
    // MARK: Constructor
    init(_ viewModel:AppContext) {
        // 视图模型
        // View model
        self.viewModel = viewModel
    }
    
    // MARK: UI页面
    // MARK: UI page
    var body: some View {
        ZStack(alignment: .leading) {
            VStack{
                Toggle(isOn: $viewModel.enableScan){
                    Text("开启扫描周围设备 Turn on scanning for surrounding devices")
                }.onChange(of: viewModel.enableScan) { value in
                    if value {
                        viewModel.scanDevices()
                    }else{
                        viewModel.stopScan()
                    }
                }.padding(10)
                ScrollViewReader { proxy in
                    List{
                        ForEach (self.viewModel.deviceList){ device in
                            Bwt901bleView(device, viewModel)
                        }
                    }
                }
            }
        }.navigationBarHidden(true)
    }
}


struct ConnectView_Previews: PreviewProvider {
    static var previews: some View {
        ConnectView(AppContext())
    }
}

// **********************************************************
// MARK: 显示蓝牙5.0传感器的视图
// MARK: View showing Bluetooth 5.0 sensor
// **********************************************************
struct Bwt901bleView: View{
    
    // bwt901ble实例
    // bwt901ble instance
    @ObservedObject var device:Bwt901ble
    
    // App上下文
    // App the context
    @ObservedObject var viewModel:AppContext
    
    // MARK: 构造方法
    // MARK: Constructor
    init(_ device:Bwt901ble,_ viewModel:AppContext){
        self.device = device
        self.viewModel = viewModel
    }
    
    // MARK: UI页面
    // MARK: UI page
    var body: some View {
        VStack {
            Toggle(isOn: $device.isOpen) {
                VStack {
                    Text("\(device.name ?? "")")
                        .font(.headline)
                    Text("\(device.mac ?? "")")
                        .font(.subheadline)
                }
            }.onChange(of: device.isOpen) { value in
                if value {
                    viewModel.openDevice(bwt901ble: device)
                }else{
                    viewModel.closeDevice(bwt901ble: device)
                }
            }
            .padding(10)
        }
    }
}
