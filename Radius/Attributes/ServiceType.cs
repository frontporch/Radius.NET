using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class ServiceType : RadiusAttribute
	{
		public ServiceType(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.SERVICE_TYPE;

			RawData[0] = (byte)Type;
		}
	}
}
