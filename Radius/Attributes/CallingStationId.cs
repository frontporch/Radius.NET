using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CallingStationId : Attribute
	{
		public CallingStationId(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CALLING_STATION_ID;

			RawData[0] = (byte)Type;
		}
	}
}
