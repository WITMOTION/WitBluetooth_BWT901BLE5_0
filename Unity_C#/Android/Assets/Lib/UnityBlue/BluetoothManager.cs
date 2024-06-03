using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothManager
{
    private static BluetoothManager instance;

    private BluetoothManager() { }

    public static BluetoothManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BluetoothManager();
            }
            return instance;
        }
    }

    /// <summary>
    /// 搜到设备委托
    /// </summary>
    /// <param name="mac"></param>
    /// <param name="deviceName"></param>
    public delegate void DeviceFoundEvent(string mac, string deviceName);

    public event DeviceFoundEvent OnDeviceFound;

    public void startScan() {
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                OnDeviceFound?.Invoke(address, name);
            }, null);
        }, (error) => {
            BluetoothLEHardwareInterface.Log("BLE Error: " + error);
        });
    }

    public void stopScan() {
        BluetoothLEHardwareInterface.StopScan();
    }
}
