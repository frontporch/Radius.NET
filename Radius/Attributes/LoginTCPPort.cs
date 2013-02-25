using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginTCPPort : Attribute
	{
		public LoginTCPPort(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_TCP_PORT;

			RawData[0] = (byte)Type;
		}
	}
}
