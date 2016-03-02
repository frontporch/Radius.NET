using FP.Radius;
using System;
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

            //args = new string[4];
            //args[0] = "192.168.1.1";
            //args[1] = "secret";
            //args[2] = "username";
            //args[3] = "password";

            try
            {
                Authenticate(args).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private async static Task Authenticate(string[] args)
        {
            RadiusClient rc = new RadiusClient(args[0], args[1]);
            RadiusPacket authPacket = rc.Authenticate(args[2], args[3]);
            authPacket.SetAttribute(new VendorSpecificAttribute(10135, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
            authPacket.SetAttribute(new VendorSpecificAttribute(10135, 2, new[] { (byte)7 }));
            RadiusPacket receivedPacket = await rc.SendAndReceivePacket(authPacket);
            if (receivedPacket == null) throw new Exception("Can't contact remote radius server !");

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
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage : ClientTest.exe hostname sharedsecret username password");
        }
    }
}