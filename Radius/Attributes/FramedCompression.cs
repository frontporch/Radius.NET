using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedCompression : RadiusAttribute
	{
		public FramedCompression(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_COMPRESSION;

			RawData[0] = (byte)Type;
		}
	}
}
