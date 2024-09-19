using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScannerTestScript : MonoBehaviour
{
	public GameObject ScannedItemPrefab;

	private float _timeout;
	private float _startScanTimeout = 10f;
	private float _startScanDelay = 0.5f;
	private bool _startScan = true;
	private Dictionary<string, ScannedItemScript> _scannedItems;

	public void OnStopScanning()
	{
		BluetoothLEHardwareInterface.Log ("**************** stopping");
		BluetoothLEHardwareInterface.StopScan ();
	}

	// Use this for initialization
	void Start ()
	{
		BluetoothLEHardwareInterface.Log ("Start");
		_scannedItems = new Dictionary<string, ScannedItemScript> ();

		BluetoothLEHardwareInterface.Initialize (true, false, () => {

			_timeout = _startScanDelay;
		}, 
		(error) => {
			
			BluetoothLEHardwareInterface.Log ("Error: " + error);

			if (error.Contains ("Bluetooth LE Not Enabled"))
				BluetoothLEHardwareInterface.BluetoothEnable (true);
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				if (_startScan)
				{
					_startScan = false;
					_timeout = _startScanTimeout;

					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, null, (address, name, rssi, bytes) => {

						BluetoothLEHardwareInterface.Log ("item scanned: " + address);
						if (_scannedItems.ContainsKey (address))
						{
							var scannedItem = _scannedItems[address];
							scannedItem.TextRSSIValue.text = rssi.ToString ();
							BluetoothLEHardwareInterface.Log ("already in list " + rssi.ToString ());
						}
						else
						{
							BluetoothLEHardwareInterface.Log ("item new: " + address);
							var newItem = Instantiate (ScannedItemPrefab);
							if (newItem != null)
							{
								BluetoothLEHardwareInterface.Log ("item created: " + address);
								newItem.transform.parent = transform;
								newItem.transform.localScale = Vector3.one;

								var scannedItem = newItem.GetComponent<ScannedItemScript> ();
								if (scannedItem != null)
								{
									BluetoothLEHardwareInterface.Log ("item set: " + address);
									scannedItem.TextAddressValue.text = address;
									scannedItem.TextNameValue.text = name;
									scannedItem.TextRSSIValue.text = rssi.ToString ();

									_scannedItems[address] = scannedItem;
								}
							}
						}
					}, true);
				}
				else
				{
					BluetoothLEHardwareInterface.StopScan ();
					_startScan = true;
					_timeout = _startScanDelay;
				}
			}
		}
	}
}
