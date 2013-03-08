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

		#region Protected
		public byte[] Data;
		#endregion

		#region Properties
		public RadiusAttributeType Type { get; protected set; }
		public byte Length { get; protected set; }
		public byte[] RawData { get; protected set; }
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
						return new IPAddress(Data).ToString();
					case RadiusAttributeType.FRAMED_PROTOCOL:
						Array.Reverse(Data);
						return ((Protocol)(BitConverter.ToInt32(Data, 0))).ToString();
					case RadiusAttributeType.FRAMED_ROUTING:
						Array.Reverse(Data);
						return ((Routing)(BitConverter.ToInt32(Data, 0))).ToString();
					case RadiusAttributeType.SERVICE_TYPE:
						Array.Reverse(Data);
						return ((Service)(BitConverter.ToInt32(Data, 0))).ToString();
					case RadiusAttributeType.FRAMED_COMPRESSION:
						Array.Reverse(Data);
						return ((Compression)(BitConverter.ToInt32(Data, 0))).ToString();
					case RadiusAttributeType.LOGIN_SERVICE:
						Array.Reverse(Data);
						return ((Login)(BitConverter.ToInt32(Data, 0))).ToString();
					case RadiusAttributeType.FILTER_ID:
					case RadiusAttributeType.CALLBACK_NUMBER:
					case RadiusAttributeType.REPLY_MESSAGE:
						return Encoding.UTF8.GetString(Data);
					case RadiusAttributeType.FRAMED_MTU:
					case RadiusAttributeType.LOGIN_TCP_PORT:
						Array.Reverse(Data);
						return (BitConverter.ToInt32(Data, 0)).ToString();
					default:
						return BitConverter.ToString(Data);
				}
			}
		}
		#endregion

		#region Constructor

		public RadiusAttribute(RadiusAttributeType type)
		{
			Type = type;
		}

		public RadiusAttribute(RadiusAttributeType type, byte[] data)
		{
			Type = type;
			Data = data;

			Length = (byte)(Data.Length + ATTRIBUTE_HEADER_SIZE);

			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;
			Array.Copy(data, 0, RawData, ATTRIBUTE_HEADER_SIZE, data.Length);
		}

		#endregion

	}
}
