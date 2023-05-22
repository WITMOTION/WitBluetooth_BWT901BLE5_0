//
//  蓝牙事件观察者
//
//
//  Created by huangyajun on 2022/8/29.
//

import Foundation

public protocol IBluetoothEventObserver {
    
    // MARK: 当搜索到蓝牙时
    func onFoundBle(bluetoothBLE: BluetoothBLE?)
    
    // MARK: 当蓝牙连接成功时
    func onConnected(bluetoothBLE: BluetoothBLE?)
    
    // MARK: 当蓝牙断开时
    func onDisconnected(bluetoothBLE: BluetoothBLE?)
    
    // MARK: 当连接失败时
    func onConnectionFailed(bluetoothBLE: BluetoothBLE?)
}
