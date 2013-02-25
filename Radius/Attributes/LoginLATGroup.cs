using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginLATGroup : RadiusAttribute
	{
		public LoginLATGroup(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_LAT_GROUP;

			RawData[0] = (byte)Type;
		}
	}
}
