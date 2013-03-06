using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class VendorSpecificAttribute : RadiusAttribute
	{
		#region Constants
		private const byte VSA_TYPE_INDEX = 6;
		private const byte VSA_LENGTH_INDEX = 7;
		private const byte VSA_DATA_INDEX = 8;
		#endregion

		public byte VendorSpecificType { get; private set; }
		public byte VendorSpecificLength { get; private set; }

		#region Constructor
		public VendorSpecificAttribute(uint vendorId, byte vendorSpecificType, byte[] vendorSpecificData) : base (RadiusAttributeType.VENDOR_SPECIFIC)
		{
			_Data = vendorSpecificData;
			
			//Length is the actual data plus all the vendor specific header info
			Length = (byte)(_Data.Length + VSA_DATA_INDEX);

			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;

			// Set the Private Enterprise Number for this attribute
			// http://www.iana.org/assignments/enterprise-numbers
			byte[] vendorIdArray = BitConverter.GetBytes(vendorId);
			Array.Reverse(vendorIdArray);
			Array.Copy(vendorIdArray, 0, RawData, ATTRIBUTE_HEADER_SIZE, sizeof(uint));

			RawData[VSA_TYPE_INDEX] = vendorSpecificType;

			RawData[VSA_LENGTH_INDEX] = (byte)(_Data.Length + ATTRIBUTE_HEADER_SIZE);

			Array.Copy(vendorSpecificData, 0, RawData, VSA_DATA_INDEX, vendorSpecificData.Length);
		}

		#endregion
	}
}
