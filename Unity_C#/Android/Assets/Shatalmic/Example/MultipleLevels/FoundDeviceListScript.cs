using UnityEngine;
using System.Collections.Generic;

public class FoundDeviceListScript : MonoBehaviour
{
	static public List<DeviceObject> DeviceAddressList;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (gameObject);
	}
}
