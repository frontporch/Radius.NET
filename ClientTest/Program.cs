using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Radius;
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
			try
			{
				RadiusClient rc = new RadiusClient(args[0], args[1]);
				RadiusPacket authPacket = rc.Authenticate(args[2], args[3]);
				RadiusPacket receivedPacket = rc.SendAndReceivePacket(authPacket);
				if (receivedPacket == null) throw new Exception("Can't contact remote radius server !");
				switch (receivedPacket.Type)
				{
					case RadiusPacketType.ACCESS_ACCEPT:
						Console.WriteLine("accepted");
						foreach (RadiusAttribute attr in receivedPacket.Attributes)
							Console.WriteLine(attr.Type.ToString() + " = " + attr.Value);
						break;
					default:
						Console.WriteLine("rejected");
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error : " + e.Message);
			}
		}

		private static void ShowUsage()
		{
			Console.WriteLine("Usage : mono radiusauth.exe hostname sharedsecret username password");
		}
	}
}