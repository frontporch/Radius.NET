using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedIPXNetwork : Attribute
	{
		public FramedIPXNetwork(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_IPX_NETWORK;

			RawData[0] = (byte)Type;
		}
	}
}
