using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CallbackId : Attribute
	{
		public CallbackId(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CALLBACK_ID;

			RawData[0] = (byte)Type;
		}
	}
}
