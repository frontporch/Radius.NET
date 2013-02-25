using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class LoginIPHost : Attribute
	{
		public LoginIPHost(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.LOGIN_IP_HOST;

			RawData[0] = (byte)Type;
		}

		public override string Value()
		{
			return (new IPAddress((_Data[3] << 24) + (_Data[2] << 16) + (_Data[1] << 8) + _Data[0])).ToString();
		}
	}
}
