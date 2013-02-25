using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CHAPPassword : Attribute
	{
		public CHAPPassword(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CHAP_PASSWORD;

			RawData[0] = (byte)Type;
		}
	}
}
