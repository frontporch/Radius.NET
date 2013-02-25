using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class ProxyState : Attribute
	{
		public ProxyState(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.PROXY_STATE;

			RawData[0] = (byte)Type;
		}
	}
}
