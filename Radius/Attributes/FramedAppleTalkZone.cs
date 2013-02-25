using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedAppleTalkZone : Attribute
	{
		public FramedAppleTalkZone(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_APPLETALK_ZONE;

			RawData[0] = (byte)Type;
		}
	}
}
