using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class PortLimit : RadiusAttribute
	{
		public PortLimit(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.PORT_LIMIT;

			RawData[0] = (byte)Type;
		}
	}
}
