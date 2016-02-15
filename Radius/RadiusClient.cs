using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class RadiusClient
	{
		#region Constants
		private const int DEFAULT_RETRIES = 3;
		private const uint DEFAULT_AUTH_PORT = 1812;
		private const uint DEFAULT_ACCT_PORT = 1813;
		private const int DEFAULT_SOCKET_TIMEOUT = 3000;
		#endregion

		#region Private
		private string _SharedSecret = String.Empty;
		private string _HostName = String.Empty;
		private uint _AuthPort = DEFAULT_AUTH_PORT;
		private uint _AcctPort = DEFAULT_ACCT_PORT;
		private uint _AuthRetries = DEFAULT_RETRIES;
		private uint _AcctRetries = DEFAULT_RETRIES;
		private int _SocketTimeout = DEFAULT_SOCKET_TIMEOUT;
		private IPEndPoint _LocalEndPoint;
		#endregion

		#region Properties
		public int SocketTimeout
		{
			get { return _SocketTimeout; }
			set { _SocketTimeout = value; }
		}
		#endregion

		#region Constructors
		public RadiusClient(string hostName, string sharedSecret, 
							int sockTimeout = DEFAULT_SOCKET_TIMEOUT, 
							uint authPort = DEFAULT_AUTH_PORT, 
							uint acctPort = DEFAULT_ACCT_PORT,
							IPEndPoint localEndPoint = null)
		{
			_HostName = hostName;
			_AuthPort = authPort;
			_AcctPort = acctPort;
			_LocalEndPoint = localEndPoint;
			_SharedSecret = sharedSecret;
			_SocketTimeout = sockTimeout;
		}
		#endregion

		#region Public Methods
		public RadiusPacket Authenticate(string username, string password)
		{
			RadiusPacket packet = new RadiusPacket(RadiusCode.ACCESS_REQUEST);
			packet.SetAuthenticator(_SharedSecret);
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

                IPAddress hostIP = null;

                try
				{
					// Starting with Vista, we are able to bind to a local endpoint to guarantee the packet
					// will be sent out a particular interface
					// This is explained in the following blog
					// http://blogs.technet.com/b/networking/archive/2009/04/25/source-ip-address-selection-on-a-multi-homed-windows-computer.aspx
					if(_LocalEndPoint != null)
						udpClient.Client.Bind(_LocalEndPoint);
					
					if(!IPAddress.TryParse(_HostName, out hostIP))
                    {
                        //Try performing a DNS lookup
                        var host = Dns.GetHostEntry(_HostName);
                        var ipv4Addresses = host.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork);
                        if (ipv4Addresses.Count() == 0)
                            throw new Exception("Resolving " + _HostName + " returned no hits in DNS");

                        hostIP = ipv4Addresses.First();
                    }
				}
				catch (SocketException e)
				{
					int hr = Marshal.GetHRForException(e);
					string hexValue = hr.ToString("X");

					//The requested name is valid, but no data of the requested type was found
					if (hexValue == "80004005")
						return null;
				}

				var endPoint = new IPEndPoint(hostIP, (int)_AuthPort);
				int numberOfAttempts = 0;

				do
				{
					await udpClient.SendAsync(packet.RawData, packet.RawData.Length, endPoint);

					try
					{
						// Using the synchronous method for the timeout features
						var result = udpClient.Receive(ref endPoint);
						RadiusPacket receivedPacket = new RadiusPacket(result);

						if (receivedPacket.Valid && VerifyAuthenticator(packet, receivedPacket))
							return receivedPacket;
					}
					catch (SocketException)
					{
						//Server isn't responding
					}

					numberOfAttempts++;

				} while (numberOfAttempts < retries);
			}

			return null;
		}
		#endregion

		#region Private Methods
		private bool VerifyAuthenticator(RadiusPacket requestedPacket, RadiusPacket receivedPacket)
		{
			return requestedPacket.Identifier == receivedPacket.Identifier 
				&& receivedPacket.Authenticator.SequenceEqual(Utils.ResponseAuthenticator(receivedPacket.RawData, requestedPacket.Authenticator, _SharedSecret));
		}

		public static bool VerifyAccountingAuthenticator(byte[] radiusPacket, string secret)
		{
			var secretBytes = Encoding.ASCII.GetBytes(secret);

			byte[] sum = new byte[radiusPacket.Length + secretBytes.Length];

			byte[] authenticator = new byte[16];
			Array.Copy(radiusPacket, 4, authenticator, 0, 16);

			Array.Copy(radiusPacket, 0, sum, 0, radiusPacket.Length);
			Array.Copy(secretBytes, 0, sum, radiusPacket.Length, secretBytes.Length);
			Array.Clear(sum, 4, 16);

			MD5 md5 = new MD5CryptoServiceProvider();

			var hash = md5.ComputeHash(sum, 0, sum.Length);
			return authenticator.SequenceEqual(hash);
		}
		#endregion
	}
}