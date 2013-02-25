using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FilterId : Attribute
	{
		public FilterId(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FILTER_ID;

			RawData[0] = (byte)Type;
		}
	}
}
