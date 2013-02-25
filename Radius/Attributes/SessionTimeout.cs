using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class SessionTimeout : RadiusAttribute
	{
		public SessionTimeout(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.SESSION_TIMEOUT;

			RawData[0] = (byte)Type;
		}
	}
}
