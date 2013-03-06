using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class RadiusAttribute
	{
		#region Constants
		protected const byte ATTRIBUTE_HEADER_SIZE = 2;
		#endregion

		protected byte[] _Data;

		public RadiusAttributeType Type { get; protected set; }
		public byte Length { get; protected set; }
		public byte[] RawData { get; protected set; }

		#region Constructor

		public RadiusAttribute(RadiusAttributeType type)
		{
			Type = type;
		}

		public RadiusAttribute(RadiusAttributeType type, byte[] data)
		{
			Type = type;
			_Data = data;

			Length = (byte)(_Data.Length + ATTRIBUTE_HEADER_SIZE);

			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;
			Array.Copy(data, 0, RawData, ATTRIBUTE_HEADER_SIZE, data.Length);
		}

		#endregion

		public string Value
		{
			get
			{
				switch (Type)
				{
					case RadiusAttributeType.NAS_IP_ADDRESS:
					case RadiusAttributeType.FRAMED_IP_ADDRESS:
					case RadiusAttributeType.FRAMED_IP_NETMASK:
					case RadiusAttributeType.LOGIN_IP_HOST:
						return new IPAddress(_Data).ToString();
					case RadiusAttributeType.FRAMED_PROTOCOL:
						Array.Reverse(_Data);
						return ((Protocol)(BitConverter.ToInt32(_Data, 0))).ToString();
					case RadiusAttributeType.FRAMED_ROUTING:
						Array.Reverse(_Data);
						return ((Routing)(BitConverter.ToInt32(_Data, 0))).ToString();
					case RadiusAttributeType.SERVICE_TYPE:
						Array.Reverse(_Data);
						return ((Service)(BitConverter.ToInt32(_Data, 0))).ToString();
					case RadiusAttributeType.FRAMED_COMPRESSION:
						Array.Reverse(_Data);
						return ((Compression)(BitConverter.ToInt32(_Data, 0))).ToString();
					case RadiusAttributeType.LOGIN_SERVICE:
						Array.Reverse(_Data);
						return ((Login)(BitConverter.ToInt32(_Data, 0))).ToString();
					case RadiusAttributeType.FILTER_ID:
					case RadiusAttributeType.CALLBACK_NUMBER:
					case RadiusAttributeType.REPLY_MESSAGE:
						return Encoding.UTF8.GetString(_Data);
					case RadiusAttributeType.FRAMED_MTU:
					case RadiusAttributeType.LOGIN_TCP_PORT:
						Array.Reverse(_Data);
						return (BitConverter.ToInt32(_Data, 0)).ToString();
					default:
						return BitConverter.ToString(_Data);
				}
			}
		}
	}
}
