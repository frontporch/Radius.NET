using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedIPNetmask : Attribute
	{
		public FramedIPNetmask(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_IP_NETMASK;

			RawData[0] = (byte)Type;
		}
	}
}
