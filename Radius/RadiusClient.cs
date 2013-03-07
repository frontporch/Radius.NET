using System;
using System.IO;
using System.Linq;
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
		private static uint DEFAULT_AUTH_PORT = 1812;
		private static uint DEFAULT_ACCT_PORT = 1813;
		private static int DEFAULT_SOCKET_TIMEOUT = 3000;

		private string _SharedSecret = String.Empty;
		private string _HostName = String.Empty;
		private uint _AuthPort = DEFAULT_AUTH_PORT;
		private uint _AcctPort = DEFAULT_ACCT_PORT;
		private uint _AuthRetries = DEFAULT_RETRIES;
		private uint _AcctRetries = DEFAULT_RETRIES;
		private int _SocketTimeout = DEFAULT_SOCKET_TIMEOUT;

		public int SocketTimeout
		{
			get { return _SocketTimeout; }
			set { _SocketTimeout = value; }
		}

		public RadiusClient(string hostName, string sharedSecret) :
			this(hostName, DEFAULT_AUTH_PORT, DEFAULT_ACCT_PORT, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, uint authPort, uint acctPort, string sharedSecret) :
			this(hostName, authPort, acctPort, sharedSecret, DEFAULT_SOCKET_TIMEOUT) {}

		public RadiusClient(string hostName, uint authPort, uint acctPort, string sharedSecret, int sockTimeout)
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
			using (UdpClient udpClient = new UdpClient())
			{
				udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _SocketTimeout);
				
				udpClient.Connect(_HostName, (int) _AuthPort);

				var endPoint = (IPEndPoint)udpClient.Client.RemoteEndPoint;

				for (int i = 0; i < retries; i++)
				{
					await udpClient.SendAsync(packet.RawData, packet.RawData.Length);

					try
					{
						var result = udpClient.Receive(ref endPoint);
						RadiusPacket receivedPacket = new RadiusPacket(result);

						if (receivedPacket.Valid && VerifyAuthenticator(packet, receivedPacket))
							return receivedPacket;
					}
					catch (SocketException e)
					{
						//Server isn't responding
					}
				}
			}

			return null;
		}

		private bool VerifyAuthenticator(RadiusPacket requestedPacket, RadiusPacket receivedPacket)
		{
			return requestedPacket.Identifier == receivedPacket.Identifier 
				&& receivedPacket.Authenticator.SequenceEqual(Utils.ResponseAuthenticator(receivedPacket.RawData, requestedPacket.Authenticator, _SharedSecret));
		}
	}
}