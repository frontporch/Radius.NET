using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class RadiusClient
	{
		private const int DEFAULT_RETRIES = 3;
		private static int DEFAULT_AUTH_PORT = 1812;
		private static int DEFAULT_ACCT_PORT = 1813;
		private static int DEFAULT_SOCKET_TIMEOUT = 3;

		private string _SharedSecret = String.Empty;
		private string _HostName = String.Empty;
		private int _AuthPort = DEFAULT_AUTH_PORT;
		private int _AcctPort = DEFAULT_ACCT_PORT;
		private uint _AuthRetries = DEFAULT_RETRIES;
		private uint _AcctRetries = DEFAULT_RETRIES;
		private int _SocketTimeout = DEFAULT_SOCKET_TIMEOUT;

		public RadiusClient(string hostName, string sharedSecret) :
			this(hostName, DEFAULT_AUTH_PORT, DEFAULT_ACCT_PORT, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, int authPort, int acctPort, string sharedSecret) :
			this(hostName, authPort, acctPort, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, int authPort, int acctPort, string sharedSecret, int sockTimeout)
		{
			_HostName = hostName;
			_AuthPort = authPort;
			_AcctPort = acctPort;
			_SharedSecret = sharedSecret;
			_SocketTimeout = sockTimeout;
		}

		public RadiusPacket Authenticate(string username, string password)
		{
			RadiusPacket packet = new RadiusPacket(RadiusCode.ACCESS_REQUEST, _SharedSecret);
			byte[] encryptedPass = Utils.EncodePapPassword(Encoding.ASCII.GetBytes(password), packet.Authenticator, _SharedSecret);
			packet.SetAttribute(new RadiusAttribute(RadiusAttributeType.USER_NAME, Encoding.ASCII.GetBytes(username)));
			packet.SetAttribute(new RadiusAttribute(RadiusAttributeType.USER_PASSWORD, encryptedPass));
			return packet;
		}

		public async Task<RadiusPacket> SendAndReceivePacket(RadiusPacket packet, int retries = DEFAULT_RETRIES)
		{
			UdpClient udpClient = new UdpClient();

			udpClient.Connect(_HostName, _AuthPort);

			for (int i = 0; i <= retries; i++)
			{
				if (!udpClient.Client.Connected)
				{
					udpClient.Connect(_HostName, _AuthPort);
				}
				
				await udpClient.SendAsync(packet.RawData, packet.RawData.Length);

				try
				{
					var result = await udpClient.ReceiveAsync().WithTimeout(TimeSpan.FromSeconds(_SocketTimeout), _HostName);
					RadiusPacket receivedPacket = new RadiusPacket(result.Buffer, _SharedSecret, packet.Authenticator);

					if (receivedPacket.Valid && VerifyPacket(packet, receivedPacket))
						return receivedPacket;
				}
				catch (Exception e)
				{
					if(udpClient.Client.Connected)
						udpClient.Close();
				}
			}

			if (udpClient.Client.Connected)
				udpClient.Close();
			
			return null;
		}

		private bool VerifyPacket(RadiusPacket requestedPacket, RadiusPacket receivedPacket)
		{
			if (requestedPacket.Identifier != receivedPacket.Identifier) return false;
			if (requestedPacket.Authenticator.ToString() !=
			    Utils.ResponseAuthenticator(receivedPacket.RawData, requestedPacket.Authenticator, _SharedSecret)
			         .ToString()) return false;
			return true;
		}

		public int SocketTimeout
		{
			get { return _SocketTimeout; }
			set { _SocketTimeout = value; }
		}
	}
}