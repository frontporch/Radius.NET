using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedMTU : RadiusAttribute
	{
		public FramedMTU(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_MTU;

			RawData[0] = (byte)Type;
		}
	}
}
