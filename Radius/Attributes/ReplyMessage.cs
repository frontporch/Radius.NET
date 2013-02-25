using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Radius
{
	public class ReplyMessage : Attribute
	{
		public ReplyMessage(byte[] data) : base(data)
		{
			Type = RadiusAttributeType.REPLY_MESSAGE;

			RawData[0] = (byte)Type;
		}
	}
}
