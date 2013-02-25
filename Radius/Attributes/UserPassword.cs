using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class UserPassword : Attribute
	{
		public UserPassword(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.USER_PASSWORD;

			RawData[0] = (byte)Type;
		}
	}
}
