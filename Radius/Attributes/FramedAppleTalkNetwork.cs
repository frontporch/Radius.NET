using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class FramedAppleTalkNetwork : Attribute
	{
		public FramedAppleTalkNetwork(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.FRAMED_APPLETALK_NETWORK;

			RawData[0] = (byte)Type;
		}
	}
}
