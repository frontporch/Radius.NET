using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedRoute : RadiusAttribute
	{
		public FramedRoute(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_ROUTE;

			RawData[0] = (byte)Type;
		}	
	}
}
