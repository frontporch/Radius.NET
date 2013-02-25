using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class UserName : RadiusAttribute
	{
		public UserName(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.USER_NAME;

			RawData[0] = (byte)Type;
		}
	}
}
