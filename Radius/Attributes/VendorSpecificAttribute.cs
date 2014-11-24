using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class VendorSpecificAttribute : RadiusAttribute
	{
		#region Constants
		private const uint VSA_ID_INDEX = 2;
		private const byte VSA_TYPE_INDEX = 6;
		private const byte VSA_LENGTH_INDEX = 7;
		private const byte VSA_DATA_INDEX = 8;
		#endregion

		#region Properties
		public byte VendorSpecificType { get; private set; }
		public byte VendorSpecificLength { get; private set; }
		public uint VendorId { get; private set; }
		#endregion

		#region Constructor
		public VendorSpecificAttribute(uint vendorId, byte vendorSpecificType, byte[] vendorSpecificData) : base (RadiusAttributeType.VENDOR_SPECIFIC)
		{
			VendorId = vendorId;
			VendorSpecificType = vendorSpecificType;
			Data = vendorSpecificData;

			//Length is the actual data plus all the vendor specific header info
			Length = (byte)(Data.Length + VSA_DATA_INDEX);

			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;

			// Set the Private Enterprise Number for this attribute
			// http://www.iana.org/assignments/enterprise-numbers
			byte[] vendorIdArray = BitConverter.GetBytes(vendorId);
			Array.Reverse(vendorIdArray);
			Array.Copy(vendorIdArray, 0, RawData, ATTRIBUTE_HEADER_SIZE, sizeof(uint));

			RawData[VSA_TYPE_INDEX] = vendorSpecificType;

			RawData[VSA_LENGTH_INDEX] = (byte)(Data.Length + ATTRIBUTE_HEADER_SIZE);

			Array.Copy(vendorSpecificData, 0, RawData, VSA_DATA_INDEX, vendorSpecificData.Length);
		}

		public VendorSpecificAttribute(byte[] rawData, int offset) : base (RadiusAttributeType.VENDOR_SPECIFIC)
		{
			byte[] vendorIDArray = new byte[sizeof(uint)];
			Array.Copy(rawData, offset + VSA_ID_INDEX, vendorIDArray, 0, sizeof(uint));
			Array.Reverse(vendorIDArray);
			VendorId = BitConverter.ToUInt32(vendorIDArray, 0);

			VendorSpecificType = rawData[VSA_TYPE_INDEX + offset];
			VendorSpecificLength = rawData[VSA_LENGTH_INDEX + offset];
			
			Data = new byte[VendorSpecificLength - 2];
			Array.Copy(rawData, offset + VSA_DATA_INDEX, Data, 0, VendorSpecificLength - 2);

			Length = (byte)(Data.Length + VSA_DATA_INDEX);

			RawData = new byte[VendorSpecificLength];
			Array.Copy(rawData, offset, RawData, 0, VendorSpecificLength);
		}

		public static VendorSpecificAttribute CreateInt16(uint vendorId, byte vendorSpecificType, short vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		public static VendorSpecificAttribute CreateUInt16(uint vendorId, byte vendorSpecificType, ushort vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		public static VendorSpecificAttribute CreateInt32(uint vendorId, byte vendorSpecificType, int vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		public static VendorSpecificAttribute CreateUInt32(uint vendorId, byte vendorSpecificType, uint vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		public static VendorSpecificAttribute CreateInt64(uint vendorId, byte vendorSpecificType, long vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		public static VendorSpecificAttribute CreateUInt64(uint vendorId, byte vendorSpecificType, ulong vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Utils.GetNetworkBytes(vendorSpecificData));
		}

		/// <summary>
		/// Creates a vendor specific attribute that has a string value type
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="vendorSpecificType">Type of vendor avp</param>
		/// <param name="vendorSpecificData">UTF8 will be used for encoding</param>
		/// <returns></returns>
		public static VendorSpecificAttribute CreateString(uint vendorId, byte vendorSpecificType, string vendorSpecificData)
		{
			return new VendorSpecificAttribute(vendorId, vendorSpecificType, Encoding.UTF8.GetBytes(vendorSpecificData));
		}

		#endregion
	}
}
