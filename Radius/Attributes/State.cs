using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class State : RadiusAttribute
	{
		public State(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.STATE;

			RawData[0] = (byte)Type;
		}
	}
}
