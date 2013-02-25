using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginService : Attribute
	{
		public LoginService (byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_SERVICE;

			RawData[0] = (byte)Type;
		}
	}
}
