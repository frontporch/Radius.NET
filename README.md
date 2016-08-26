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
		Console.WriteLine("Access-Accept");
		foreach (var attr in receivedPacket.Attributes)
			Console.WriteLine(attr.Type.ToString() + " = " + attr.Value);
		break;
	case RadiusCode.ACCESS_CHALLENGE:
		Console.WriteLine("Access-Challenge");
		break;
	case RadiusCode.ACCESS_REJECT:
		Console.WriteLine("Access-Reject");
		if (!rc.VerifyAuthenticator(authPacket, receivedPacket))
			Console.WriteLine("Authenticator check failed: Check your secret");
		break;
	default:
		Console.WriteLine("Rejected");
		break;
}
```

License
=====

The MIT License (MIT)

Copyright (c) <year> <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
