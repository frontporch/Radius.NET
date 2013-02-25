using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class TerminationAction : Attribute
	{
		public TerminationAction(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.TERMINATION_ACTION;

			RawData[0] = (byte)Type;
		}
	}
}
