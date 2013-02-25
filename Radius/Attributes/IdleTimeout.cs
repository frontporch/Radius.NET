using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class IdleTimeout : Attribute
	{
		public IdleTimeout(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.IDLE_TIMEOUT;

			RawData[0] = (byte)Type;
		}
	}
}
