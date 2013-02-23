using System;
using System.Text;
using System.Collections;
using System.IO;

namespace System.Net.Radius
{
	public class RadiusPacket
	{
		//1 byte for code + 1 byte for Identifier + 2 bytes for Length + 16 bytes for Request Authenticator
		private RadiusPacketType packetType;
		private int identifier;
		private long length;
		private byte[] authenticator = new byte[16];
		private ArrayList attributes = new ArrayList();
		private NasPortType nasporttype;
		//private byte[] attributesArray;
		private byte[] rawData;
		// used to build packetToSend
		public RadiusPacket(RadiusPacketType packetType, string sharedsecret)
		{
			this.packetType = packetType;
			this.identifier = (int) (Guid.NewGuid().ToByteArray())[0];
			this.authenticator = Utils.makeRFC2865RequestAuthenticator(sharedsecret);
		}

		// used to format received data
		public RadiusPacket(byte[] receivedData, string sharedsecret, byte[] requestAuthenticator)
		{
			this.rawData = receivedData;
			this.packetType = (RadiusPacketType) (int) receivedData[0];
			this.identifier = (int) receivedData[1];
			this.length = (long) (receivedData[2] << 8) + (long) receivedData[3];
			Array.Copy(receivedData, 4, this.authenticator, 0, 16);
			byte[] attributesArray = new byte[receivedData.Length - 20];
			Array.Copy(receivedData, 20, attributesArray, 0, attributesArray.Length);
			ParseAttributes(attributesArray);
		}

		public void SetAttributes(RadiusAttributeType type, byte[] data)
		{
			attributes.Add(new RadiusAttribute(type, data));
		}

		public NasPortType NasPortType
		{
			get { return this.nasporttype; }
			set
			{
				this.nasporttype = value;
				attributes.Add(new RadiusAttribute(RadiusAttributeType.NAS_PORT_TYPE, BitConverter.GetBytes((int) value)));
			}
		}

		public ArrayList Attributes
		{
			get { return this.attributes; }
		}

		public RadiusPacketType Type
		{
			get { return this.packetType; }
		}

		public byte[] Authenticator
		{
			get { return this.authenticator; }
		}

		public byte[] RawData
		{
			get { return this.rawData; }
		}

		public int Identifier
		{
			get { return this.identifier; }
		}

		public byte[] GetBytes()
		{
			this.length = 0;
			foreach (RadiusAttribute ra in attributes) this.length += ra.GetBytes().Length;
			byte[] attrs = new byte[this.length];
			int offset = 0;
			foreach (RadiusAttribute ra in attributes)
			{
				Array.Copy(ra.GetBytes(), 0, attrs, offset, ra.GetBytes().Length);
				offset += ra.GetBytes().Length;
			}
			byte[] header = new byte[20 + this.length];
			header[0] = (byte) this.packetType;
			header[1] = (byte) this.identifier;
			header[2] = (byte) 0;
			header[3] = System.Convert.ToByte(this.length + 20);
			Array.Copy(this.authenticator, 0, header, 4, 16);
			Array.Copy(attrs, 0, header, 20, attrs.Length);
			return header;
		}

		private void ParseAttributes(byte[] rawattributes)
		{
			int x = 0;
			while (x < rawattributes.Length)
			{
				RadiusAttributeType type = (RadiusAttributeType) rawattributes[x];
				int length = (int) rawattributes[x + 1];
				byte[] data = new byte[length - 2];
				Array.Copy(rawattributes, x + 2, data, 0, length - 2);
				this.attributes.Add(new RadiusAttribute(type, data));
				x += length;
			}
		}
	}

	public class RadiusAttribute
	{
		private RadiusAttributeType type;
		private byte[] data;

		public RadiusAttribute(RadiusAttributeType type, byte[] data)
		{
			this.type = type;
			this.data = data;
		}

		public RadiusAttributeType Type
		{
			get { return this.type; }
		}

		public byte[] GetBytes()
		{
			byte[] result = new byte[data.Length + 2];
			result[0] = (byte) type;
			result[1] = (byte) (data.Length + 2);
			Array.Copy(data, 0, result, 2, data.Length);
			return result;
		}

		public string Value
		{
			get
			{
				switch (type)
				{
					case RadiusAttributeType.NAS_IP_ADDRESS:
					case RadiusAttributeType.FRAMED_IP_ADDRESS:
					case RadiusAttributeType.FRAMED_IP_NETMASK:
					case RadiusAttributeType.LOGIN_IP_HOST:
						return (new IPAddress((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + data[0])).ToString();
					case RadiusAttributeType.FRAMED_PROTOCOL:
						return ((Protocol) ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3])).ToString();
					case RadiusAttributeType.FRAMED_ROUTING:
						return ((Routing) ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3])).ToString();
					case RadiusAttributeType.SERVICE_TYPE:
						return ((Service) ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3])).ToString();
					case RadiusAttributeType.FRAMED_COMPRESSION:
						return ((Compression) ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3])).ToString();
					case RadiusAttributeType.LOGIN_SERVICE:
						return ((Login) ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3])).ToString();
					case RadiusAttributeType.FILTER_ID:
					case RadiusAttributeType.CALLBACK_NUMBER:
					case RadiusAttributeType.REPLY_MESSAGE:
						return System.Text.Encoding.ASCII.GetString(data);
					case RadiusAttributeType.FRAMED_MTU:
					case RadiusAttributeType.LOGIN_TCP_PORT:
						return ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3]).ToString();
					default:
						return BitConverter.ToString(data);
				}
			}
		}
	}
}