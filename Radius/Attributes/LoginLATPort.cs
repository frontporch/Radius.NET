using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginLATPort : RadiusAttribute
	{
		public LoginLATPort(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_LAT_PORT;

			RawData[0] = (byte)Type;
		}
	}
}
