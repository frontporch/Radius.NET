using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginLATNode : RadiusAttribute
	{
		public LoginLATNode(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_LAT_NODE;

			RawData[0] = (byte)Type;
		}
	}
}
