using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Level2Script : MonoBehaviour
{
	public List<Text> Buttons;
	public List<string> Services;
	public List<string> Characteristics;

	// Use this for initialization
	void Start ()
	{
		int buttonID = 0;
		foreach (var device in FoundDeviceListScript.DeviceAddressList)
		{
			Buttons[buttonID++].text = device.Name;
			if (buttonID == 4)
				break;
		}
	}

	void OnCharacteristic (string characteristic, byte[] bytes)
	{
		BluetoothLEHardwareInterface.Log ("received: " + characteristic);
	}

	public void OnSubscribeClick (int buttonID)
	{
		if (buttonID >= 0 && buttonID < 4)
		{
			DeviceObject device = FoundDeviceListScript.DeviceAddressList[buttonID];
			string subscribedService = Services[buttonID];
			string subscribedCharacteristic = Characteristics[buttonID];

			if (!string.IsNullOrEmpty (subscribedService) && !string.IsNullOrEmpty (subscribedCharacteristic))
			{
				BluetoothLEHardwareInterface.Log ("subscribing to: " + subscribedService + ", " + subscribedCharacteristic);

				BluetoothLEHardwareInterface.SubscribeCharacteristic (device.Address, subscribedService, subscribedCharacteristic, null, (characteristic, bytes) => {

					BluetoothLEHardwareInterface.Log ("received data: " + characteristic);
				});
			}
		}
	}

	public void OnButtonClick (int buttonID)
	{
		if (buttonID >= 0 && buttonID < 4)
		{
			DeviceObject device = FoundDeviceListScript.DeviceAddressList[buttonID];
			Text button = Buttons[buttonID];
			string subscribedService = Services[buttonID];
			string subscribedCharacteristic = Characteristics[buttonID];

			if (device != null && button != null)
			{
				if (button.text.Contains ("connected"))
				{
					if (!string.IsNullOrEmpty (subscribedService) && !string.IsNullOrEmpty (subscribedCharacteristic))
					{
						BluetoothLEHardwareInterface.UnSubscribeCharacteristic (device.Address, subscribedService, subscribedCharacteristic, (characteristic) => {
							
							Services[buttonID] = null;
							Characteristics[buttonID] = null;
							
							BluetoothLEHardwareInterface.DisconnectPeripheral (device.Address, (disconnectAddress) => {
								
								button.text = device.Name;
							});
						});
					}
					else
					{
						BluetoothLEHardwareInterface.DisconnectPeripheral (device.Address, (disconnectAddress) => {
							
							button.text = device.Name;
						});
					}
				}
				else
				{
					BluetoothLEHardwareInterface.ConnectToPeripheral (device.Address, (address) => {

					}, null, (address, service, characteristic) => {

						if (string.IsNullOrEmpty (Services[buttonID]) && string.IsNullOrEmpty (Characteristics[buttonID]))
						{
							Services[buttonID] = FullUUID (service);
							Characteristics[buttonID] = FullUUID (characteristic);
							button.text = device.Name + " connected";
						}

					}, null);
				}
			}
		}
	}
	
	string FullUUID (string uuid)
	{
		if (uuid.Length == 4)
			return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";

		return uuid;
	}
}
