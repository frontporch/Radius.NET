using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class NASPort : RadiusAttribute
	{
		public NASPort(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.NAS_PORT;

			RawData[0] = (byte)Type;
		}
	}
}
