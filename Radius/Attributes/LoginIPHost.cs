using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginIPHost : RadiusAttribute
	{
		public LoginIPHost(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_IP_HOST;

			RawData[0] = (byte)Type;
		}
	}
}
