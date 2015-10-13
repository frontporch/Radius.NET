using System;
using System.Collections.Generic;
using System.Linq;
using FP.Radius;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length != 4)
			{
				ShowUsage();
				return;
			}
			
			Authenticate(args).ContinueWith(task => 
			{
				if (task.IsFaulted)
					Console.WriteLine("Error : " + task.Exception.Message);
			});
		}

		private async static Task Authenticate(string[] args)
		{
			RadiusClient rc = new RadiusClient(args[0], args[1]);
			RadiusPacket authPacket = rc.Authenticate(args[2], args[3]);
			authPacket.SetAttribute(new VendorSpecificAttribute(10135, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
			authPacket.SetAttribute(new VendorSpecificAttribute(10135, 2, new[] { (byte)7 }));
			RadiusPacket receivedPacket = await rc.SendAndReceivePacket(authPacket);

			if (receivedPacket == null) throw new Exception("Can't contact remote radius server !");

			if (!rc.VerifyAuthenticator(authPacket, receivedPacket))
			   Console.WriteLine("Bad secret!");

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
		}

		private static void ShowUsage()
		{
			Console.WriteLine("Usage : ClientTest.exe hostname sharedsecret username password");
		}
	}
}