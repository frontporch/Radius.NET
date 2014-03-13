Overview
========

Radius.NET is a Radius client built on the .NET 4.5 runtime.  It currently only supports the PAP protocol.
Other protocols will be added if needed.  Pull requests are also welcome.

Requirements
============

.NET 4.5 installed

Example Usage
=============

```csharp
var hostname = "UBUNTU-RADIUS";
var sharedKey = "test";
var username = "User1";
var password = "test";

var rc = new RadiusClient(hostname, sharedKey);
var authPacket = rc.Authenticate(username, password);
authPacket.SetAttribute(new VendorSpecificAttribute(10, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
authPacket.SetAttribute(new VendorSpecificAttribute(10, 2, new[] {(byte)7}));
var receivedPacket = rc.SendAndReceivePacket(authPacket).Result;

if (receivedPacket == null) 
	throw new Exception("Can't contact remote radius server !");

switch (receivedPacket.PacketType)
{
	case RadiusCode.ACCESS_ACCEPT:
		Console.WriteLine("Accepted");
		foreach (var attr in receivedPacket.Attributes)
			Console.WriteLine(attr.Type.ToString() + " = " + attr.Value);
		break;
	case RadiusCode.ACCESS_CHALLENGE:
		Console.WriteLine("Challenged");
		break;
	default:
		Console.WriteLine("Rejected");
		break;
}
```
