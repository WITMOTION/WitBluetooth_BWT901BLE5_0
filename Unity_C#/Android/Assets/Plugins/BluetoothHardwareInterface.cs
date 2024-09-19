#define EXPERIMENTAL_MACOS_EDITOR
/*

This build includes an experimental implementation for the macOS editor of Unity
It is experiemental because of the way that the Unity editor hangs on to plugin
instances after leaving play mode. This causes this plugin to not free up its
resources and therefore can cause crashes in the Unity editor on macOS.

Since Unity does not give plugins or apps a chance to do anything when the user
hits the play / stop button in the Editor there isn't a chance for the app to
deinitialize this plugin.

What I have found in my own use of this is that if you put a button on your app
somewhere that you can press before hitting the stop button in the editor and
then in that button handler call this plugin's Deinitialize method it seems to
minimize how often the editor crashes.

WARNING: using the macOS editor can cause the editor to crash an loose your work
and settings. Save often. You have been warned, so please don't contact me if
you have lost work becausee of this problem. This is experimental only. Use at
your own risk.

*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

#if UNITY_2018_3_OR_NEWER
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
#endif

public class BluetoothLEHardwareInterface
{
	public enum CBCharacteristicProperties
	{
		CBCharacteristicPropertyBroadcast = 0x01,
		CBCharacteristicPropertyRead = 0x02,
		CBCharacteristicPropertyWriteWithoutResponse = 0x04,
		CBCharacteristicPropertyWrite = 0x08,
		CBCharacteristicPropertyNotify = 0x10,
		CBCharacteristicPropertyIndicate = 0x20,
		CBCharacteristicPropertyAuthenticatedSignedWrites = 0x40,
		CBCharacteristicPropertyExtendedProperties = 0x80,
		CBCharacteristicPropertyNotifyEncryptionRequired = 0x100,
		CBCharacteristicPropertyIndicateEncryptionRequired = 0x200,
	};

	public enum ScanMode
	{
		LowPower = 0,
		Balanced = 1,
		LowLatency = 2
	}

	public enum ConnectionPriority
	{
		LowPower = 0,
		Balanced = 1,
		High = 2,
	}

	public enum AdvertisingMode
	{
		LowPower = 0,
		Balanced = 1,
		LowLatency = 2
	}

	public enum AdvertisingPower
	{
		UltraLow = 0,
		Low = 1,
		Medium = 2,
		High = 3,
	}

	public enum iOSProximity
	{
		Unknown = 0,
		Immediate = 1,
		Near = 2,
		Far = 3,
	}

	public struct iBeaconData
	{
		public string UUID;
		public int Major;
		public int Minor;
		public int RSSI;
		public int AndroidSignalPower;
		public iOSProximity iOSProximity;
	}

#if UNITY_ANDROID
	public enum CBAttributePermissions
	{
		CBAttributePermissionsReadable = 0x01,
		CBAttributePermissionsWriteable = 0x10,
		CBAttributePermissionsReadEncryptionRequired = 0x02,
		CBAttributePermissionsWriteEncryptionRequired = 0x20,
	};
#else
	public  enum CBAttributePermissions
	{
		CBAttributePermissionsReadable = 0x01,
		CBAttributePermissionsWriteable = 0x02,
		CBAttributePermissionsReadEncryptionRequired = 0x04,
		CBAttributePermissionsWriteEncryptionRequired = 0x08,
	};
#endif

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)

	public delegate void UnitySendMessageCallbackDelegate (IntPtr objectName, IntPtr commandName, IntPtr commandData);

	[DllImport ("BluetoothLEOSX")]
	private static extern void ConnectUnitySendMessageCallback ([MarshalAs (UnmanagedType.FunctionPtr)]UnitySendMessageCallbackDelegate callbackMethod);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLELog (string message);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEInitialize ([MarshalAs (UnmanagedType.Bool)]bool asCentral, [MarshalAs (UnmanagedType.Bool)]bool asPeripheral);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEDeInitialize ();

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEPauseMessages (bool isPaused);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEScanForPeripheralsWithServices (string serviceUUIDsString, bool allowDuplicates, bool rssiOnly, bool clearPeripheralList);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLERetrieveListOfPeripheralsWithServices (string serviceUUIDsString);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEStopScan ();

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEConnectToPeripheral (string name);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEDisconnectAll ();

	[DllImport("BluetoothLEOSX")]
	private static extern void OSXBluetoothLERequestMtu (string name, int mtu);

	[DllImport("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEReadRSSI (string name);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEDisconnectPeripheral (string name);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEReadCharacteristic (string name, string service, string characteristic);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEWriteCharacteristic (string name, string service, string characteristic, byte[] data, int length, bool withResponse);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLESubscribeCharacteristic (string name, string service, string characteristic);

	[DllImport ("BluetoothLEOSX")]
	private static extern void OSXBluetoothLEUnSubscribeCharacteristic (string name, string service, string characteristic);

#endif

#if UNITY_IOS || UNITY_TVOS
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLELog (string message);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEInitialize (bool asCentral, bool asPeripheral);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEDeInitialize ();
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEPauseMessages (bool isPaused);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEScanForPeripheralsWithServices (string serviceUUIDsString, bool allowDuplicates, bool rssiOnly, bool clearPeripheralList);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLERetrieveListOfPeripheralsWithServices (string serviceUUIDsString);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEStopScan ();

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEConnectToPeripheral (string name);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEDisconnectPeripheral (string name);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEReadCharacteristic (string name, string service, string characteristic);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEWriteCharacteristic (string name, string service, string characteristic, byte[] data, int length, bool withResponse);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLESubscribeCharacteristic (string name, string service, string characteristic);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEUnSubscribeCharacteristic (string name, string service, string characteristic);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEDisconnectAll ();

	[DllImport("__Internal")]
	private static extern void _iOSBluetoothLERequestMtu(string name, int mtu);

	[DllImport("__Internal")]
	private static extern void _iOSBluetoothLEReadRSSI(string name);

#if !UNITY_TVOS
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEScanForBeacons (string proximityUUIDsString);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEStopBeaconScan ();

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEPeripheralName (string newName);

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLECreateService (string uuid, bool primary);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLERemoveService (string uuid);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLERemoveServices ();

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLECreateCharacteristic (string uuid, int properties, int permissions, byte[] data, int length);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLERemoveCharacteristic (string uuid);
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLERemoveCharacteristics ();

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEStartAdvertising ();
	
	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEStopAdvertising ();

	[DllImport ("__Internal")]
	private static extern void _iOSBluetoothLEUpdateCharacteristicValue (string uuid, byte[] data, int length);
#endif
#elif UNITY_ANDROID
    static AndroidJavaObject _android = null;
#endif


	private static BluetoothDeviceScript bluetoothDeviceScript;

	public static void Log (string message)
	{
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		Debug.Log(message);
#else
        if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLELog (message);
#elif UNITY_ANDROID
			if (_android == null)
			{
				AndroidJavaClass javaClass = new AndroidJavaClass ("com.shatalmic.unityandroidbluetoothlelib.UnityBluetoothLE");
				_android = javaClass.CallStatic<AndroidJavaObject> ("getInstance");
			}

			if (_android != null)
				_android.Call ("androidBluetoothLog", message);
#endif
		}
#endif
	}

#if UNITY_2018_3_OR_NEWER
#if UNITY_ANDROID
	private static IEnumerator AskForPermissions(Action afterPermissionAction, bool needLocation = false)
	{
		bool scanAsked = false;
		bool locationAsked = false;
		bool connectAsked = false;
		bool permissionsGranted = false;
		float timerValue = 0f;

		while (timerValue < 5f)
		{
			if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN"))
			{
				if (!scanAsked)
				{
					Permission.RequestUserPermission("android.permission.BLUETOOTH_SCAN");
					scanAsked = true;
					timerValue = 0;
				}
			}
			else
			{
				if (needLocation && !Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION"))
				{
					if (!locationAsked)
					{
						Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
                        locationAsked = false;
						timerValue = 2;
					}
				}
				else
				{
					if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
					{
						if (!connectAsked)
						{
							Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
							connectAsked = false;
							timerValue = 2;
						}
					}
					else
					{
						permissionsGranted = true;
						break;
					}
				}
			}

			timerValue += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

        if (!permissionsGranted)
        {
            if (bluetoothDeviceScript.ErrorAction != null)
				bluetoothDeviceScript.ErrorAction("Error~Permissions Not Granted");
        }
		else
		{
			if (afterPermissionAction != null)
				afterPermissionAction();
		}
    }
#endif
#endif

	public static BluetoothDeviceScript Initialize(bool asCentral, bool asPeripheral, Action action, Action<string> errorAction, bool needLocation = false)
	{
		bluetoothDeviceScript = null;

		GameObject bluetoothLEReceiver = GameObject.Find("BluetoothLEReceiver");
		if (bluetoothLEReceiver == null)
			bluetoothLEReceiver = new GameObject ("BluetoothLEReceiver");

		if (bluetoothLEReceiver != null)
		{
			bluetoothDeviceScript = bluetoothLEReceiver.GetComponent<BluetoothDeviceScript> ();
			if (bluetoothDeviceScript == null)
				bluetoothDeviceScript = bluetoothLEReceiver.AddComponent<BluetoothDeviceScript> ();

			if (bluetoothDeviceScript != null)
			{
				bluetoothDeviceScript.InitializedAction = action;
				bluetoothDeviceScript.ErrorAction = errorAction;
			}
		}

		GameObject.DontDestroyOnLoad (bluetoothLEReceiver);

#if UNITY_2018_3_OR_NEWER
#if UNITY_ANDROID

		Log($"API: {SystemInfo.operatingSystem}");

		int apiVersion = 30;
		int apiIndex = SystemInfo.operatingSystem.IndexOf(" API-");
		if (apiIndex >= 0)
		{
			Log($"API Index: {apiIndex}");
			string versionString = SystemInfo.operatingSystem.Substring(apiIndex + 5, 2);
			Log($"API Version String: {versionString}");
			if (!int.TryParse(versionString, out apiVersion))
			{
				Log($"int parse failed: {versionString}");
				apiVersion = 30;
			}
		}

		Log($"API Version: {apiVersion}");
#endif
		Action afterPermissionAction = () =>
		{
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			ConnectUnitySendMessageCallback((objectName, commandName, commandData) => {
				string name = Marshal.PtrToStringAuto (objectName);
				string command = Marshal.PtrToStringAuto (commandName);
				string data = Marshal.PtrToStringAuto (commandData);

				GameObject foundObject = GameObject.Find (name);
				if (foundObject != null)
					foundObject.SendMessage (command, data);
			});

			BluetoothLEHardwareInterface.OSXBluetoothLEInitialize (asCentral, asPeripheral);
#else
			if (Application.isEditor)
			{
				if (bluetoothDeviceScript != null)
					bluetoothDeviceScript.SendMessage ("OnBluetoothMessage", "Initialized");
			}
			else
			{
#if UNITY_IOS || UNITY_TVOS
				_iOSBluetoothLEInitialize (asCentral, asPeripheral);
#elif UNITY_ANDROID
				if (_android == null)
				{
					AndroidJavaClass javaClass = new AndroidJavaClass ("com.shatalmic.unityandroidbluetoothlelib.UnityBluetoothLE");
					_android = javaClass.CallStatic<AndroidJavaObject> ("getInstance");
				}

				if (_android != null)
					_android.Call ("androidBluetoothInitialize", asCentral, asPeripheral);
#endif
			}
#endif
		};

#if UNITY_ANDROID
		if (apiVersion >= 31)
		{
			if (asCentral)
				bluetoothDeviceScript.StartCoroutine(AskForPermissions(afterPermissionAction, needLocation));

			if (asPeripheral)
			{
				if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE"))
					Permission.RequestUserPermission("android.permission.BLUETOOTH_ADVERTISE");
			}
		}
        else
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                Permission.RequestUserPermission(Permission.FineLocation);

			afterPermissionAction();
        }
#else
		afterPermissionAction();
#endif
#endif

		return bluetoothDeviceScript;
	}
	
	public static void DeInitialize (Action action)
	{
		if (bluetoothDeviceScript != null)
			bluetoothDeviceScript.DeinitializedAction = action;

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		BluetoothLEHardwareInterface.OSXBluetoothLEDeInitialize ();
#else
        if (Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.SendMessage ("OnBluetoothMessage", "DeInitialized");
		}
		else
		{
#if UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEDeInitialize ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothDeInitialize");
#endif
		}
#endif
	}

	public static void FinishDeInitialize ()
	{
		GameObject bluetoothLEReceiver = GameObject.Find("BluetoothLEReceiver");
		if (bluetoothLEReceiver != null)
			GameObject.Destroy(bluetoothLEReceiver);
	}

	public static void BluetoothEnable (bool enable)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
			//_iOSBluetoothLELog (message);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothEnable", enable);
#endif
		}
	}

	public static void BluetoothScanMode (ScanMode scanMode)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothScanMode", (int)scanMode);
#endif
		}
	}

	public static void BluetoothConnectionPriority (ConnectionPriority connectionPriority)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothConnectionPriority", (int)connectionPriority);
#endif
		}
	}

	public static void BluetoothAdvertisingMode (AdvertisingMode advertisingMode)
	{
		if (!Application.isEditor)
		{
#if UNITY_IPHONE || UNITY_TVOS
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothAdvertisingMode", (int)advertisingMode);
#endif
		}
	}

	public static void BluetoothAdvertisingPower (AdvertisingPower advertisingPower)
	{
		if (!Application.isEditor)
		{
#if UNITY_IPHONE || UNITY_TVOS
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothAdvertisingPower", (int)advertisingPower);
#endif
		}
	}

	public static void PauseMessages (bool isPaused)
	{
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		OSXBluetoothLEPauseMessages (isPaused);
#else
        if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEPauseMessages (isPaused);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothPause", isPaused);
#endif
		}
#endif
	}

	// scanning for beacons requires that you know the Proximity UUID
	public static void ScanForBeacons (string[] proximityUUIDs, Action<iBeaconData> actionBeaconResponse)
	{
		if (proximityUUIDs != null && proximityUUIDs.Length >= 0)
		{
			if (!Application.isEditor)
			{
				if (bluetoothDeviceScript != null)
					bluetoothDeviceScript.DiscoveredBeaconAction = actionBeaconResponse;

				string proximityUUIDsString = null;

				if (proximityUUIDs != null && proximityUUIDs.Length > 0)
				{
					proximityUUIDsString = "";

					foreach (string proximityUUID in proximityUUIDs)
						proximityUUIDsString += proximityUUID + "|";

					proximityUUIDsString = proximityUUIDsString.Substring (0, proximityUUIDsString.Length - 1);
				}

#if UNITY_IOS
				_iOSBluetoothLEScanForBeacons (proximityUUIDsString);
#elif UNITY_ANDROID
				if (_android != null)
					_android.Call ("androidBluetoothScanForBeacons", proximityUUIDsString);
#endif
			}
		}
	}

    public static void RequestMtu(string name, int mtu, Action<string, int> action)
    {
		if (bluetoothDeviceScript != null)
        {
			bluetoothDeviceScript.RequestMtuAction = action;
        }

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		if (mtu > 184)
			mtu = 184;
		OSXBluetoothLERequestMtu(name, mtu);
#elif UNITY_IOS || UNITY_TVOS
        if (mtu > 180)
            mtu = 180;
	    _iOSBluetoothLERequestMtu (name, mtu);
#elif UNITY_ANDROID
        if (_android != null)
		{
			_android.Call ("androidBluetoothRequestMtu", name, mtu);
		}
#endif
	}

	public static void ReadRSSI(string name, Action<string, int> action)
    {
		if (bluetoothDeviceScript != null)
        {
			bluetoothDeviceScript.ReadRSSIAction = action;
        }

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		OSXBluetoothLEReadRSSI(name);
#elif UNITY_IOS || UNITY_TVOS
		_iOSBluetoothLEReadRSSI(name);
#elif UNITY_ANDROID
        if (_android != null)
		{
			_android.Call ("androidBluetoothReadRSSI", name);
		}
#endif
	}

	public static void ScanForPeripheralsWithServices (string[] serviceUUIDs, Action<string, string> action, Action<string, string, int, byte[]> actionAdvertisingInfo = null, bool rssiOnly = false, bool clearPeripheralList = true, int recordType = 0xFF)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
		if (!Application.isEditor)
		{
#endif
            if (bluetoothDeviceScript != null)
			{
				bluetoothDeviceScript.DiscoveredPeripheralAction = action;
				bluetoothDeviceScript.DiscoveredPeripheralWithAdvertisingInfoAction = actionAdvertisingInfo;

				if (bluetoothDeviceScript.DiscoveredDeviceList != null)
					bluetoothDeviceScript.DiscoveredDeviceList.Clear ();
			}

			string serviceUUIDsString = null;

			if (serviceUUIDs != null && serviceUUIDs.Length > 0)
			{
				serviceUUIDsString = "";

				foreach (string serviceUUID in serviceUUIDs)
					serviceUUIDsString += serviceUUID + "|";

				serviceUUIDsString = serviceUUIDsString.Substring (0, serviceUUIDsString.Length - 1);
			}

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLEScanForPeripheralsWithServices (serviceUUIDsString, (actionAdvertisingInfo != null), rssiOnly, clearPeripheralList);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEScanForPeripheralsWithServices (serviceUUIDsString, (actionAdvertisingInfo != null), rssiOnly, clearPeripheralList);
#elif UNITY_ANDROID
            if (_android != null)
			{
				if (serviceUUIDsString == null)
					serviceUUIDsString = "";

				_android.Call ("androidBluetoothScanForPeripheralsWithServices", serviceUUIDsString, rssiOnly, recordType);
			}
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
		}
#endif
        }

    public static void RetrieveListOfPeripheralsWithServices (string[] serviceUUIDs, Action<string, string> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				bluetoothDeviceScript.RetrievedConnectedPeripheralAction = action;
				
				if (bluetoothDeviceScript.DiscoveredDeviceList != null)
					bluetoothDeviceScript.DiscoveredDeviceList.Clear ();
			}
			
			string serviceUUIDsString = serviceUUIDs.Length > 0 ? "" : null;
			
			foreach (string serviceUUID in serviceUUIDs)
				serviceUUIDsString += serviceUUID + "|";
			
			// strip the last delimeter
			serviceUUIDsString = serviceUUIDsString.Substring (0, serviceUUIDsString.Length - 1);

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLERetrieveListOfPeripheralsWithServices (serviceUUIDsString);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLERetrieveListOfPeripheralsWithServices (serviceUUIDsString);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidBluetoothRetrieveListOfPeripheralsWithServices", serviceUUIDsString);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }

	public static void StopScan ()
	{
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		OSXBluetoothLEStopScan ();
#else
        if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEStopScan ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothStopScan");
#endif
		}
#endif
	}

	public static void StopBeaconScan ()
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLEStopBeaconScan ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothStopBeaconScan");
#endif
		}
	}

	public static void DisconnectAll ()
	{
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
		OSXBluetoothLEDisconnectAll ();
#else
        if (!Application.isEditor)
		{
#if UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEDisconnectAll ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidBluetoothDisconnectAll");
#endif
		}
#endif
	}

	public static void ConnectToPeripheral (string name, Action<string> connectAction, Action<string, string> serviceAction, Action<string, string, string> characteristicAction, Action<string> disconnectAction = null)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				bluetoothDeviceScript.ConnectedPeripheralAction = connectAction;
				bluetoothDeviceScript.DiscoveredServiceAction = serviceAction;
				bluetoothDeviceScript.DiscoveredCharacteristicAction = characteristicAction;
				bluetoothDeviceScript.ConnectedDisconnectPeripheralAction = disconnectAction;
			}

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLEConnectToPeripheral (name);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEConnectToPeripheral (name);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidBluetoothConnectToPeripheral", name);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }

	public static void DisconnectPeripheral (string name, Action<string> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.DisconnectedPeripheralAction = action;

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLEDisconnectPeripheral (name);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEDisconnectPeripheral (name);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidBluetoothDisconnectPeripheral", name);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }

	public static void ReadCharacteristic (string name, string service, string characteristic, Action<string, byte[]> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				if (!bluetoothDeviceScript.DidUpdateCharacteristicValueAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateCharacteristicValueAction[name] = new Dictionary<string, Action<string, byte[]>>();

#if UNITY_IOS || UNITY_TVOS || (EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX))
				bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] [characteristic] = action;
#elif UNITY_ANDROID
            bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] [FullUUID (characteristic).ToLower ()] = action;
#endif
			}

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLEReadCharacteristic (name, service, characteristic);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEReadCharacteristic (name, service, characteristic);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidReadCharacteristic", name, service, characteristic);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }
	
	public static void WriteCharacteristic (string name, string service, string characteristic, byte[] data, int length, bool withResponse, Action<string> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.DidWriteCharacteristicAction = action;

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
    		OSXBluetoothLEWriteCharacteristic(name, service, characteristic, data, length, withResponse);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEWriteCharacteristic (name, service, characteristic, data, length, withResponse);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidWriteCharacteristic", name, service, characteristic, data, length, withResponse);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }
	
	public static void SubscribeCharacteristic (string name, string service, string characteristic, Action<string> notificationAction, Action<string, byte[]> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				name = name.ToUpper ();
				service = service.ToUpper ();
				characteristic = characteristic.ToUpper ();

#if UNITY_IOS || UNITY_TVOS || (EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX))
				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction [name] = new Dictionary<string, Action<string>> ();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction [name] [characteristic] = notificationAction;

				if (!bluetoothDeviceScript.DidUpdateCharacteristicValueAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] = new Dictionary<string, Action<string, byte[]>> ();
				bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] [characteristic] = action;
#elif UNITY_ANDROID
            if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction [name] = new Dictionary<string, Action<string>> ();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction [name] [FullUUID (characteristic).ToLower ()] = notificationAction;

				if (!bluetoothDeviceScript.DidUpdateCharacteristicValueAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] = new Dictionary<string, Action<string, byte[]>> ();
				bluetoothDeviceScript.DidUpdateCharacteristicValueAction [name] [FullUUID (characteristic).ToLower ()] = action;
#endif
			}

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLESubscribeCharacteristic (name, service, characteristic);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLESubscribeCharacteristic (name, service, characteristic);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidSubscribeCharacteristic", name, service, characteristic);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }
	
	public static void SubscribeCharacteristicWithDeviceAddress (string name, string service, string characteristic, Action<string, string> notificationAction, Action<string, string, byte[]> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				name = name.ToUpper ();
				service = service.ToUpper ();
				characteristic = characteristic.ToUpper ();

#if UNITY_IOS || UNITY_TVOS || (EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX))
				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string>>();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name][characteristic] = notificationAction;

				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey (name))
                    bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name] = new Dictionary<string, Action<string>>();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name][characteristic] = null;

				if (!bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string, byte[]>>();
				bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction[name][characteristic] = action;
#elif UNITY_ANDROID
                if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string>>();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name][FullUUID (characteristic).ToLower ()] = notificationAction;

                if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey(name))
                    bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name] = new Dictionary<string, Action<string>>();
                bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name][FullUUID (characteristic).ToLower ()] = null;
				
				if (!bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string, byte[]>>();
				bluetoothDeviceScript.DidUpdateCharacteristicValueWithDeviceAddressAction[name][FullUUID (characteristic).ToLower ()] = action;
#endif
			}

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
			OSXBluetoothLESubscribeCharacteristic (name, service, characteristic);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLESubscribeCharacteristic (name, service, characteristic);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidSubscribeCharacteristic", name, service, characteristic);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }

	public static void UnSubscribeCharacteristic (string name, string service, string characteristic, Action<string> action)
	{
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        if (!Application.isEditor)
		{
#endif
			if (bluetoothDeviceScript != null)
			{
				name = name.ToUpper ();
				service = service.ToUpper ();
				characteristic = characteristic.ToUpper ();

#if UNITY_IOS || UNITY_TVOS || (EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX))
				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string>>();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name][characteristic] = null;

				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name] = new Dictionary<string, Action<string>> ();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name][characteristic] = action;
#elif UNITY_ANDROID
                if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name] = new Dictionary<string, Action<string, string>>();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicWithDeviceAddressAction[name][FullUUID (characteristic).ToLower ()] = null;
				
				if (!bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction.ContainsKey (name))
					bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name] = new Dictionary<string, Action<string>> ();
				bluetoothDeviceScript.DidUpdateNotificationStateForCharacteristicAction[name][FullUUID (characteristic).ToLower ()] = action;
#endif
        }

#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
        OSXBluetoothLEUnSubscribeCharacteristic (name, service, characteristic);
#elif UNITY_IOS || UNITY_TVOS
			_iOSBluetoothLEUnSubscribeCharacteristic (name, service, characteristic);
#elif UNITY_ANDROID
        if (_android != null)
				_android.Call ("androidUnsubscribeCharacteristic", name, service, characteristic);
#endif
#if !UNITY_EDITOR_OSX || !EXPERIMENTAL_MACOS_EDITOR
        }
#endif
    }

	public static void PeripheralName (string newName)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLEPeripheralName (newName);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidPeripheralName", newName);
#endif
		}
	}

	public static void CreateService (string uuid, bool primary, Action<string> action)
	{
		if (!Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.ServiceAddedAction = action;

#if UNITY_IOS
			_iOSBluetoothLECreateService (uuid, primary);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidCreateService", uuid, primary);
#endif
		}
	}
	
	public static void RemoveService (string uuid)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLERemoveService (uuid);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidRemoveService", uuid);
#endif
		}
	}

	public static void RemoveServices ()
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLERemoveServices ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidRemoveServices");
#endif
		}
	}

	public static void CreateCharacteristic (string uuid, CBCharacteristicProperties properties, CBAttributePermissions permissions, byte[] data, int length, Action<string, byte[]> action)
	{
		if (!Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.PeripheralReceivedWriteDataAction = action;

#if UNITY_IOS
			_iOSBluetoothLECreateCharacteristic (uuid, (int)properties, (int)permissions, data, length);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidCreateCharacteristic", uuid, (int)properties, (int)permissions, data, length);
#endif
		}
	}

	public static void RemoveCharacteristic (string uuid)
	{
		if (!Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.PeripheralReceivedWriteDataAction = null;

#if UNITY_IOS
			_iOSBluetoothLERemoveCharacteristic (uuid);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidRemoveCharacteristic", uuid);
#endif
		}
	}

	public static void RemoveCharacteristics ()
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLERemoveCharacteristics ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidRemoveCharacteristics");
#endif
		}
	}
	
	public static void StartAdvertising (Action action, bool isConnectable = true, bool includeName = true, int manufacturerId = 0, byte[] manufacturerSpecificData = null)
	{
		if (!Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.StartedAdvertisingAction = action;

#if UNITY_IOS
			_iOSBluetoothLEStartAdvertising ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidStartAdvertising", isConnectable, includeName, manufacturerId, manufacturerSpecificData);
#endif
		}
	}
	
	public static void StopAdvertising (Action action)
	{
		if (!Application.isEditor)
		{
			if (bluetoothDeviceScript != null)
				bluetoothDeviceScript.StoppedAdvertisingAction = action;

#if UNITY_IOS
			_iOSBluetoothLEStopAdvertising ();
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidStopAdvertising");
#endif
		}
	}
	
	public static void UpdateCharacteristicValue (string uuid, byte[] data, int length)
	{
		if (!Application.isEditor)
		{
#if UNITY_IOS
			_iOSBluetoothLEUpdateCharacteristicValue (uuid, data, length);
#elif UNITY_ANDROID
			if (_android != null)
				_android.Call ("androidUpdateCharacteristicValue", uuid, data, length);
#endif
		}
	}
	
	public static string FullUUID (string uuid)
	{
		if (uuid.Length == 4)
			return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
		return uuid;
	}
}
