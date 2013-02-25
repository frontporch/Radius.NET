using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class CallbackNumber : RadiusAttribute
	{
		public CallbackNumber(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.CALLBACK_NUMBER;

			RawData[0] = (byte)Type;
		}
	}
}
