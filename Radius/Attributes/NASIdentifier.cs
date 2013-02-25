using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class NASIdentifier : Attribute
	{
		public NASIdentifier(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.NAS_IDENTIFIER;

			RawData[0] = (byte)Type;
		}
	}
}
