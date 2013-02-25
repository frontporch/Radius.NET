using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginLATService : Attribute
	{
		public LoginLATService(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_LAT_SERVICE;

			RawData[0] = (byte)Type;
		}
	}
}
