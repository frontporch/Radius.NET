using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class NASIPAddress : RadiusAttribute
	{
		public NASIPAddress(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.NAS_IP_ADDRESS;

			RawData[0] = (byte)Type;
		}
	}
}
