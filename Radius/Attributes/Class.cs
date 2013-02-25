using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class Class : Attribute
	{
		public Class(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CLASS;

			RawData[0] = (byte)Type;
		}
	}
}
