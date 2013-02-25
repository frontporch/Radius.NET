using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedIPAddress : Attribute
	{
		public FramedIPAddress(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_IP_ADDRESS;

			RawData[0] = (byte)Type;
		}
	}
}
