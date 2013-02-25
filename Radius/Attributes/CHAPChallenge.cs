using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CHAPChallenge : Attribute
	{
		public CHAPChallenge(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CHAP_CHALLENGE;

			RawData[0] = (byte)Type;
		}
	}
}
