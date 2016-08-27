using FP.Radius;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string sessionId = Guid.NewGuid().ToString();

			for (int i = 0; i < 10000; i++)
			{
				RadiusPacket packet = new RadiusPacket(RadiusCode.ACCESS_REQUEST);
				packet.SetAuthenticator("labuser");
				packet.SetAttribute(RadiusAttribute.CreateUInt16(RadiusAttributeType.NAS_PORT, 56));
				packet.SetAttribute(RadiusAttribute.CreateString(RadiusAttributeType.ACCT_SESSION_ID, sessionId));
				packet.SetAttribute(RadiusAttribute.CreateString(RadiusAttributeType.USER_NAME, "Testing123"));
				packet.SetAttribute(RadiusAttribute.CreateString(RadiusAttributeType.USER_PASSWORD, "Testing123"));
				packet.Encode();
				packet.SetMessageAuthenticator("labuser");
			}
		}
	}
}
