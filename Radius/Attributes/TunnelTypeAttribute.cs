using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	/// <summary>
	/// http://tools.ietf.org/html/rfc2868
	/// </summary>
	public class TunnelTypeAttribute : RadiusAttribute
	{
		private const byte TUNNEL_TYPE_LENGTH = 6;
		private const int TUNNEL_TYPE_VALUE_INDEX = 3;
		private const int TUNNEL_TYPE_VALUE_LENGTH = 3;

		public byte Tag { get; private set; }
		public TunnelType TunnelType { get; private set; }

		public TunnelTypeAttribute(byte tag, TunnelType tunnelType) : base(RadiusAttributeType.TUNNEL_TYPE)
		{
			Tag = tag;
			TunnelType = tunnelType;
			Data = BitConverter.GetBytes((uint)tunnelType);
			
			Length = TUNNEL_TYPE_LENGTH;
			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;
			RawData[2] = ((tag & 0xFF) == 0) ? (byte)0x00 : tag;

			Array.Copy(Utils.IntTo3Byte((int)tunnelType), 0, RawData, TUNNEL_TYPE_VALUE_INDEX, TUNNEL_TYPE_VALUE_LENGTH);
		}
	}
}
