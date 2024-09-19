public class DeviceObject
{
	public string Address;
	public string Name;

	public DeviceObject ()
	{
		Address = "";
		Name = "";
	}

	public DeviceObject (string address, string name)
	{
		Address = address;
		Name = name;
	}
}
