using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedAppleTalkLink : RadiusAttribute
	{
		public FramedAppleTalkLink(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_APPLETALK_LINK;

			RawData[0] = (byte)Type;
		}
	}
}
