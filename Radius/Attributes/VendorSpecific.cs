using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class VendorSpecific : RadiusAttribute
	{
		public VendorSpecific(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.VENDOR_SPECIFIC;

			RawData[0] = (byte)Type;
		}
	}
}
