using System.Collections.Generic;

namespace System.Net.Radius
{
	public class RadiusPacket
	{
		#region Constants
		private const byte RADIUS_CODE_INDEX = 0;
		private const byte RADIUS_IDENTIFIER_INDEX = 1;
		private const byte RADIUS_LENGTH_INDEX = 2;
		private const byte RADIUS_AUTHENTICATOR_INDEX = 4;
		private const byte RADIUS_AUTHENTICATOR_FIELD_LENGTH = 16;
		private const byte ATTRIBUTES_INDEX = 20;
		private const byte HEADER_LENGTH = ATTRIBUTES_INDEX;
		#endregion

		#region Private
		private readonly List<RadiusAttribute> _Attributes = new List<RadiusAttribute>();
		private readonly byte[] _Authenticator = new byte[16];
		private ushort _Length;
		private NasPortType _NasPortType;
		#endregion

		#region Properties
		public byte[] RawData { get; private set; }
		public RadiusCode PacketType { get; private set; }
		public byte Identifier { get; private set; }
		public byte[] Header { get; private set; }
		public bool Valid { get; private set; }
		#endregion
		
		// Create a new RADIUS packet
		public RadiusPacket(RadiusCode packetType, string sharedsecret)
		{
			PacketType = packetType;
			Identifier = (Guid.NewGuid().ToByteArray())[0];
			_Length = HEADER_LENGTH;
			_Authenticator = Utils.makeRFC2865RequestAuthenticator(sharedsecret);

			RawData = new byte[HEADER_LENGTH];
			RawData[RADIUS_CODE_INDEX] = (byte)PacketType;
			RawData[RADIUS_IDENTIFIER_INDEX] = Identifier;
			Array.Copy(BitConverter.GetBytes(_Length), 0, RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Reverse(RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Copy(_Authenticator, 0, RawData, RADIUS_AUTHENTICATOR_INDEX, RADIUS_AUTHENTICATOR_FIELD_LENGTH);
		}

		// Parse received RADIUS packet
		public RadiusPacket(byte[] receivedData, string sharedsecret, byte[] requestAuthenticator)
		{
			try
			{
				Valid = true;
				
				RawData = receivedData;

				if (RawData.Length < 20 || RawData.Length > 4096)
				{
					Valid = false;
					return;
				}

				//Get the RADIUS Code
				PacketType = (RadiusCode)RawData[RADIUS_CODE_INDEX];

				//Get the RADIUS Identifier
				Identifier = RawData[RADIUS_IDENTIFIER_INDEX];

				//Get the RADIUS Length
				_Length = (ushort) ((RawData[2] << 8) + RawData[3]);

				// RADIUS length field must be equal to or greater than packet length
				if (_Length > RawData.Length)
				{
					Valid = false;
					return;
				}

				//Get the RADIUS Authenticator
				Array.Copy(receivedData, RADIUS_AUTHENTICATOR_INDEX, _Authenticator, 0, RADIUS_AUTHENTICATOR_FIELD_LENGTH);

				//GET the RADIUS Attributes
				byte[] attributesArray = new byte[_Length - ATTRIBUTES_INDEX];
				Array.Copy(receivedData, ATTRIBUTES_INDEX, attributesArray, 0, attributesArray.Length);
				ParseAttributes(attributesArray);

			}
			catch (Exception)
			{
				Valid = false;
			}
		}


		public NasPortType NasPortType
		{
			get { return _NasPortType; }
			set
			{
				_NasPortType = value;
				_Attributes.Add(new RadiusAttribute(RadiusAttributeType.NAS_PORT_TYPE, BitConverter.GetBytes((int) value)));
			}
		}

		public List<RadiusAttribute> Attributes
		{
			get { return _Attributes; }
		}

		public byte[] Authenticator
		{
			get { return _Authenticator; }
		}

		//public void SetAttribute(Attribute attribute) The future method signature
		public void SetAttribute(RadiusAttribute attribute)
		{			
			_Attributes.Add(attribute);

			AppendAttribute(attribute);
		}

		/// <summary>
		/// Method to append an attributes bytes onto RawData
		/// </summary>
		/// <param name="attribute">The attribute to append</param>
		private void AppendAttribute(RadiusAttribute attribute)
		{
			//Make an array with a size of the current RawData plus the new attribute
			byte[] newRawData = new byte[RawData.Length + attribute.Length];
			
			//Copy the current RawData into the temp array
			Array.Copy(RawData, 0, newRawData, 0, RawData.Length);

			//Copy the new attribute into the temp array
			Array.Copy(attribute.RawData, 0, newRawData, RawData.Length, attribute.Length);

			RawData = newRawData;

			//Update the length of the RadiusPacket
			_Length = (ushort) RawData.Length;
			Array.Copy(BitConverter.GetBytes(_Length), 0, RawData, RADIUS_LENGTH_INDEX, sizeof (ushort));
			Array.Reverse(RawData, RADIUS_LENGTH_INDEX, sizeof (ushort));
		}

		private void ParseAttributes(byte[] attributeByteArray)
		{
			int currentAttributeOffset = 0;

			while (currentAttributeOffset < attributeByteArray.Length)
			{
				//Get the RADIUS attribute type
				RadiusAttributeType type = (RadiusAttributeType) attributeByteArray[currentAttributeOffset];
				
				//Get the RADIUS attribute length
				byte length = attributeByteArray[currentAttributeOffset + 1];

				// Check minimum length and make sure the attribute doesn't run off the end of the packet
				if (length < 2 || currentAttributeOffset + length > _Length)
				{
					Valid = false;
					return;
				}

				//Get the RADIUS attribute data
				byte[] data = new byte[length - 2];
				Array.Copy(attributeByteArray, currentAttributeOffset + 2, data, 0, length - 2);
				_Attributes.Add(new RadiusAttribute(type, data));
				currentAttributeOffset += length;
			}
		}
	}
}