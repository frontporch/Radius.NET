using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FP.Radius
{
	public class RadiusPacket
	{
		#region Constants
		private const byte RADIUS_CODE_INDEX = 0;
		private const byte RADIUS_IDENTIFIER_INDEX = 1;
		private const byte RADIUS_LENGTH_INDEX = 2;
		private const byte RADIUS_AUTHENTICATOR_INDEX = 4;
		private const byte RADIUS_AUTHENTICATOR_FIELD_LENGTH = 16;
		private const byte RADIUS_MESSAGE_AUTH_HASH_LENGTH = 16;
		private const byte RADIUS_MESSAGE_AUTHENTICATOR_LENGTH = 18;
		private const byte ATTRIBUTES_INDEX = 20;
		private const byte RADIUS_HEADER_LENGTH = ATTRIBUTES_INDEX;
		#endregion

		#region Private
		private readonly List<RadiusAttribute> _Attributes = new List<RadiusAttribute>();
		private byte[] _Authenticator = new byte[RADIUS_AUTHENTICATOR_FIELD_LENGTH];
		private ushort _Length;
		private NasPortType _NasPortType;
		#endregion

		#region Properties
		public byte[] RawData { get; private set; }
		public RadiusCode PacketType { get; private set; }
		public byte Identifier { get; private set; }
		public byte[] Header { get; private set; }
		public bool Valid { get; private set; }

		public NasPortType NasPortType
		{
			get { return _NasPortType; }
			set
			{
				_NasPortType = value;
				_Attributes.Add(new RadiusAttribute(RadiusAttributeType.NAS_PORT_TYPE, BitConverter.GetBytes((int)value)));
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
		#endregion

		#region Constructors
		// Create a new RADIUS packet
		public RadiusPacket(RadiusCode packetType)
		{
			PacketType = packetType;
			Identifier = (Guid.NewGuid().ToByteArray())[0];
			_Length = RADIUS_HEADER_LENGTH;

			RawData = new byte[RADIUS_HEADER_LENGTH];
			RawData[RADIUS_CODE_INDEX] = (byte)PacketType;
			RawData[RADIUS_IDENTIFIER_INDEX] = Identifier;
			Array.Copy(BitConverter.GetBytes(_Length), 0, RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Reverse(RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
		}

		public RadiusPacket(RadiusCode packetType, byte identifier)
		{
			PacketType = packetType;
			Identifier = identifier;
			_Length = RADIUS_HEADER_LENGTH;

			RawData = new byte[RADIUS_HEADER_LENGTH];
			RawData[RADIUS_CODE_INDEX] = (byte)PacketType;
			RawData[RADIUS_IDENTIFIER_INDEX] = Identifier;
			Array.Copy(BitConverter.GetBytes(_Length), 0, RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Reverse(RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
		}

		// Parse received RADIUS packet
		public RadiusPacket(byte[] receivedData)
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
				_Length = (ushort)((RawData[2] << 8) + RawData[3]);

				// RADIUS length field must be equal to or greater than packet length
				if (_Length > RawData.Length)
				{
					Valid = false;
					return;
				}

				//Get the RADIUS Authenticator
				Array.Copy(RawData, RADIUS_AUTHENTICATOR_INDEX, _Authenticator, 0, RADIUS_AUTHENTICATOR_FIELD_LENGTH);

				//GET the RADIUS Attributes
				byte[] attributesArray = new byte[_Length - ATTRIBUTES_INDEX];
				Array.Copy(receivedData, ATTRIBUTES_INDEX, attributesArray, 0, attributesArray.Length);
				ParseAttributes(attributesArray);

			}
			catch
			{
				Valid = false;
			}
		}
		#endregion

		public void SetAuthenticator(string sharedsecret, byte[] requestAuthenticator = null)
		{
			switch (PacketType)
			{
				case RadiusCode.ACCESS_REQUEST:
					_Authenticator = Utils.AccessRequestAuthenticator(sharedsecret);
					break;
				case RadiusCode.ACCESS_ACCEPT:
					_Authenticator = Utils.ResponseAuthenticator(RawData, requestAuthenticator, sharedsecret);
					break;
				case RadiusCode.ACCESS_REJECT:
					break;
				case RadiusCode.ACCOUNTING_REQUEST:
					_Authenticator = Utils.AccountingRequestAuthenticator(RawData, sharedsecret);
					break;
				case RadiusCode.ACCOUNTING_RESPONSE:
					_Authenticator = Utils.ResponseAuthenticator(RawData, requestAuthenticator, sharedsecret);
					break;
				case RadiusCode.ACCOUNTING_STATUS:
					break;
				case RadiusCode.PASSWORD_REQUEST:
					break;
				case RadiusCode.PASSWORD_ACCEPT:
					break;
				case RadiusCode.PASSWORD_REJECT:
					break;
				case RadiusCode.ACCOUNTING_MESSAGE:
					break;
				case RadiusCode.ACCESS_CHALLENGE:
					break;
				case RadiusCode.SERVER_STATUS:
					_Authenticator = Utils.AccessRequestAuthenticator(sharedsecret);
					break;
				case RadiusCode.COA_REQUEST:
					_Authenticator = Utils.AccountingRequestAuthenticator(RawData, sharedsecret);
					break;
				case RadiusCode.DISCONNECT_REQUEST:
					_Authenticator = Utils.AccountingRequestAuthenticator(RawData, sharedsecret);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Array.Copy(_Authenticator, 0, RawData, RADIUS_AUTHENTICATOR_INDEX, RADIUS_AUTHENTICATOR_FIELD_LENGTH);
		}

		public void SetIdentifier(byte id)
		{
			Identifier = id;
			RawData[RADIUS_IDENTIFIER_INDEX] = Identifier;
		}

		public void SetAttribute(RadiusAttribute attribute)
		{
			_Attributes.Add(attribute);

			//Make an array with a size of the current RawData plus the new attribute
			byte[] newRawData = new byte[RawData.Length + attribute.Length];

			//Copy the current RawData into the temp array
			Array.Copy(RawData, 0, newRawData, 0, RawData.Length);

			//Copy the new attribute into the temp array
			Array.Copy(attribute.RawData, 0, newRawData, RawData.Length, attribute.Length);

			RawData = newRawData;

			//Update the length of the RadiusPacket
			_Length = (ushort)RawData.Length;
			Array.Copy(BitConverter.GetBytes(_Length), 0, RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Reverse(RawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
		}

		/// <summary>
		/// Sets the Message-Autheticator attribute on a RADIUS packet.  This should be called as a last step after all attributes have been added
		/// </summary>
		/// <param name="sharedSecret"></param>
		public void SetMessageAuthenticator(string sharedSecret)
		{
			// We need to add the Message-Authenticator attribute with 16 octects of zero
			byte[] newRawData = new byte[RawData.Length + RADIUS_MESSAGE_AUTHENTICATOR_LENGTH];
			// Copy the current packet into the new array
			Array.Copy(RawData, 0, newRawData, 0, RawData.Length);
			// Adjust the length field of the packet to account for the new attribute
			Array.Copy(BitConverter.GetBytes(newRawData.Length), 0, newRawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			Array.Reverse(newRawData, RADIUS_LENGTH_INDEX, sizeof(ushort));
			// Set the type and length of the Message-Authenticator attribute
			newRawData[RawData.Length] = (byte)RadiusAttributeType.MESSAGE_AUTHENTICATOR;
			newRawData[RawData.Length + 1] = RADIUS_MESSAGE_AUTHENTICATOR_LENGTH;
			// Calculate the hash of the new array using the shared secret
			HMACMD5 hmacmd5 = new HMACMD5(Encoding.ASCII.GetBytes(sharedSecret));
			var hash = hmacmd5.ComputeHash(newRawData);
			// Copy the hash value into the 16 octects to replace the 0's with the actual hash
			Array.Copy(hash, 0, newRawData, newRawData.Length - RADIUS_MESSAGE_AUTH_HASH_LENGTH, hash.Length);
			// Set the final result as the new RawData
			RawData = newRawData;
			// Update the Length to include the Message-Authenticator attribute
			_Length += RADIUS_MESSAGE_AUTHENTICATOR_LENGTH;
		}

		private void ParseAttributes(byte[] attributeByteArray)
		{
			int currentAttributeOffset = 0;

			while (currentAttributeOffset < attributeByteArray.Length)
			{
				//Get the RADIUS attribute type
				RadiusAttributeType type = (RadiusAttributeType)attributeByteArray[currentAttributeOffset];

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

				_Attributes.Add(type == RadiusAttributeType.VENDOR_SPECIFIC
									? new VendorSpecificAttribute(attributeByteArray, currentAttributeOffset)
									: new RadiusAttribute(type, data));

				currentAttributeOffset += length;
			}
		}
	}
}