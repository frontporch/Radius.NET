using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CalledStationId : Attribute
	{
		public CalledStationId(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CALLED_STATION_ID;

			RawData[0] = (byte)Type;
		}
	}
}
