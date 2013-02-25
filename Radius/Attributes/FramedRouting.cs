using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedRouting : RadiusAttribute
	{
		public FramedRouting(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_ROUTING;

			RawData[0] = (byte)Type;
		}
	}
}
