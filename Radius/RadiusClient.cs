using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace System.Net.Radius
{
	public class RadiusClient
	{
		private static int AUTH_RETRIES = 3;
		private static int ACCT_RETRIES = 3;
		private static int DEFAULT_AUTH_PORT = 1812;
		private static int DEFAULT_ACCT_PORT = 1813;
		private static int DEFAULT_SOCKET_TIMEOUT = 6000;

		private string sharedSecret = String.Empty;
		private string hostName = String.Empty;
		private int authPort = DEFAULT_AUTH_PORT;
		private int acctPort = DEFAULT_ACCT_PORT;
		private int socketTimeout = DEFAULT_SOCKET_TIMEOUT;

		public RadiusClient(string hostName, string sharedSecret) :
			this(hostName, DEFAULT_AUTH_PORT, DEFAULT_ACCT_PORT, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, int authPort, int acctPort, string sharedSecret) :
			this(hostName, authPort, acctPort, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, int authPort, int acctPort, string sharedSecret, int sockTimeout)
		{
			this.hostName = hostName;
			this.authPort = authPort;
			this.acctPort = acctPort;
			this.sharedSecret = sharedSecret;
			this.socketTimeout = sockTimeout;
		}

		public RadiusPacket Authenticate(string username, string password)
		{
			RadiusPacket packet = new RadiusPacket(RadiusCode.ACCESS_REQUEST, this.sharedSecret);
			byte[] encryptedPass = Utils.encodePapPassword(Encoding.ASCII.GetBytes(password), packet.Authenticator,
			                                               this.sharedSecret);
			packet.SetAttribute(new RadiusAttribute(RadiusAttributeType.USER_NAME, Encoding.ASCII.GetBytes(username)));
			packet.SetAttribute(new RadiusAttribute(RadiusAttributeType.USER_PASSWORD, encryptedPass));
			return packet;
		}

		public RadiusPacket SendAndReceivePacket(RadiusPacket packet)
		{
			return SendAndReceivePacket(packet, AUTH_RETRIES);
		}

		public RadiusPacket SendAndReceivePacket(RadiusPacket packet, int retries)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			RadiusUdpClient udpClient = new RadiusUdpClient();
			udpClient.SetTimeout(this.socketTimeout);
			for (int x = 0; x < retries; x++)
			{
				try
				{
					try
					{
						udpClient.Connect(this.hostName, this.authPort);
					}
					catch (Exception e)
					{
						udpClient = new RadiusUdpClient();
						udpClient.Connect(this.hostName, this.authPort);
					}

					udpClient.Send(packet.RawData, packet.RawData.Length);
					Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
					RadiusPacket receivedPacket = new RadiusPacket(receiveBytes, sharedSecret, packet.Authenticator);
					if (receivedPacket.Valid && VerifyPacket(packet, receivedPacket))
						return receivedPacket;
					udpClient.Close();
				}
				catch (Exception e)
				{
					if (udpClient != null) udpClient.Close();
					Console.WriteLine(e.Message);
				}
			}
			return null;
		}

		private bool VerifyPacket(RadiusPacket requestedPacket, RadiusPacket receivedPacket)
		{
			if (requestedPacket.Identifier != receivedPacket.Identifier) return false;
			if (requestedPacket.Authenticator.ToString() !=
			    Utils.makeRFC2865ResponseAuthenticator(receivedPacket.RawData, requestedPacket.Authenticator, sharedSecret)
			         .ToString()) return false;
			return true;
		}

		public int SocketTimeout
		{
			get { return this.socketTimeout; }
			set { this.socketTimeout = value; }
		}
	}
}