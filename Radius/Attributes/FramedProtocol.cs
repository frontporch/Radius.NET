using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedProtocol : Attribute
	{
		public FramedProtocol(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_PROTOCOL;

			RawData[0] = (byte)Type;
		}
	}
}
