using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class NASPortType : Attribute
	{
		public NASPortType(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.NAS_PORT_TYPE;

			RawData[0] = (byte)Type;
		}
	}
}
