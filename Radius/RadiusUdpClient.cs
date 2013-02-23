using System;
using System.Net;
using System.Net.Sockets;

namespace System.Net.Radius
{
	public class RadiusUdpClient : UdpClient
	{
		private int socketTimeout = 6000; // set default to 6 s

		public RadiusUdpClient() : base() {}

		public RadiusUdpClient(string hostname, int port) : base(hostname, port) {}

		public void SetTimeout(int timeout)
		{
			this.socketTimeout = timeout;
		}

		public new byte[] Receive(ref IPEndPoint remoteEP)
		{
			base.Client.Poll(this.socketTimeout*1000, SelectMode.SelectRead);
			base.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.socketTimeout);

			byte[] recBuffer;

			int available = base.Client.Available;
			recBuffer = new byte[available];

			EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
			int dataRead = base.Client.ReceiveFrom(recBuffer, ref endPoint);
			byte[] data = new byte[dataRead];
			Array.Copy(recBuffer, 0, data, 0, dataRead);
			remoteEP = (IPEndPoint) endPoint;

			return data;
		}
	}
}