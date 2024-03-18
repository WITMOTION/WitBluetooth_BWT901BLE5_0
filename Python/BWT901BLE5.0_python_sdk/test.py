import asyncio
import bleak
import device_model

# 扫描到的设备 Scanned devices
devices = []


# 扫描蓝牙设备并过滤名称 Scan Bluetooth devices and filter names
async def scan():
    global devices
    find = []
    print("Searching for Bluetooth devices......")
    try:
        devices = await bleak.BleakScanner.discover()
        print("Search ended")
        for d in devices:
            if d.name is not None and "WT" in d.name:
                find.append(d)
                print(d)
        if len(find) == 0:
            print("No devices found in this search!")
    except Exception as ex:
        print("Bluetooth search failed to start")
        print(ex)


# 数据更新时会调用此方法 This method will be called when data is updated
def updateData(DeviceModel):
    # 直接打印出设备数据字典 Directly print out the device data dictionary
    print(DeviceModel.deviceData)
    # 获得X轴加速度 Obtain X-axis acceleration
    # print(DeviceModel.get("AccX"))


if __name__ == '__main__':
    # 搜索设备 Search Device
    asyncio.run(scan())
    # 选择要连接的设备 Select the device to connect to
    device_mac = None
    user_input = input("Please enter the Mac address you want to connect to (e.g. DF:E9:1F:2C:BD:59)：")
    for device in devices:
        if device.address == user_input:
            device_mac = device.address
            break
    if device_mac is not None:
        # 创建设备 Create device
        device = device_model.DeviceModel("MyBle5.0", device_mac, updateData)
        asyncio.run(device.openDevice())
    else:
        print("No Bluetooth device corresponding to Mac address found!!")
