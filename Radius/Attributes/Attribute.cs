using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class Attribute
	{
		#region Constants
		private const byte ATTRIBUTE_HEADER_SIZE = 2;
		#endregion

		protected byte[] _Data;

		public RadiusAttributeType Type { get; protected set; }
		public byte Length { get; private set; }
		public byte[] RawData { get; protected set; }

		#region Constructor
		protected Attribute(byte[] data)
		{
			_Data = data;

			Length = (byte) (_Data.Length + ATTRIBUTE_HEADER_SIZE);

			RawData = new byte[Length];

			RawData[1] = Length;
			Array.Copy(data, 0, RawData, ATTRIBUTE_HEADER_SIZE, data.Length);
		}

		public Attribute(RadiusAttributeType type, byte[] data) : this(data)
		{
			Type = type;
		}

		#endregion

		public virtual string Value()
		{
			return BitConverter.ToString(_Data);
		}

		//public string Value
		//{
		//	get
		//	{
		//		switch (Type)
		//		{
		//			case RadiusAttributeType.NAS_IP_ADDRESS:
		//			case RadiusAttributeType.FRAMED_IP_ADDRESS:
		//			case RadiusAttributeType.FRAMED_IP_NETMASK:
		//			case RadiusAttributeType.LOGIN_IP_HOST:
		//				return (new IPAddress((_Data[3] << 24) + (_Data[2] << 16) + (_Data[1] << 8) + _Data[0])).ToString();
		//			case RadiusAttributeType.FRAMED_PROTOCOL:
		//				return ((Protocol)((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3])).ToString();
		//			case RadiusAttributeType.FRAMED_ROUTING:
		//				return ((Routing)((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3])).ToString();
		//			case RadiusAttributeType.SERVICE_TYPE:
		//				return ((Service)((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3])).ToString();
		//			case RadiusAttributeType.FRAMED_COMPRESSION:
		//				return ((Compression)((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3])).ToString();
		//			case RadiusAttributeType.LOGIN_SERVICE:
		//				return ((Login)((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3])).ToString();
		//			case RadiusAttributeType.FILTER_ID:
		//			case RadiusAttributeType.CALLBACK_NUMBER:
		//			case RadiusAttributeType.REPLY_MESSAGE:
		//				return Encoding.ASCII.GetString(_Data);
		//			case RadiusAttributeType.FRAMED_MTU:
		//			case RadiusAttributeType.LOGIN_TCP_PORT:
		//				return ((_Data[0] << 24) + (_Data[1] << 16) + (_Data[2] << 8) + _Data[3]).ToString();
		//			default:
		//				return BitConverter.ToString(_Data);
		//		}
		//	}
		//}
	}
}
