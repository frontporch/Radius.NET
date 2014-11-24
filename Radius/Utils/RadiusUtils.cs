using System;
using System.Security.Cryptography;

namespace FP.Radius
{
	public class Utils
	{
		public static byte[] AccountingRequestAuthenticator(byte[] data, string sharedSecret)
		{
			byte[] sharedS = System.Text.Encoding.ASCII.GetBytes(sharedSecret);
			byte[] sum = new byte[data.Length + sharedS.Length];

			Array.Copy(data, 0, sum, 0, data.Length);
			Array.Copy(sharedS, 0, sum, data.Length, sharedS.Length);
			Array.Clear(data, 4, 16);
			MD5 md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(sum, 0, sum.Length);
			return md5.Hash;
		}
		
		public static byte[] AccessRequestAuthenticator(string sharedSecret)
		{
			byte[] sharedS = System.Text.Encoding.ASCII.GetBytes(sharedSecret);
			byte[] requestAuthenticator = new byte[16 + sharedS.Length];
			Random r = new Random();
			for (int i = 0; i < 16; i++)
				requestAuthenticator[i] = (byte) r.Next();
			Array.Copy(sharedS, 0, requestAuthenticator, 16, sharedS.Length);
			MD5 md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(requestAuthenticator);
			return md5.Hash;
		}

		public static byte[] ResponseAuthenticator(byte[] data, byte[] requestAuthenticator, string sharedSecret)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] ssArray = System.Text.Encoding.ASCII.GetBytes(sharedSecret);
			byte[] sum = new byte[data.Length + ssArray.Length];
			Array.Copy(data, 0, sum, 0, data.Length);
			Array.Copy(requestAuthenticator, 0, sum, 4, 16);
			Array.Copy(ssArray, 0, sum, data.Length, ssArray.Length);
			md5.ComputeHash(sum);
			return md5.Hash;
		}

		public static byte[] EncodePapPassword(byte[] userPassBytes, byte[] requestAuthenticator, string sharedSecret)
		{
			if (userPassBytes.Length > 128)
				throw new InvalidOperationException("the PAP password cannot be greater than 128 bytes...");

			byte[] encryptedPass = userPassBytes.Length%16 == 0 
				? new byte[userPassBytes.Length] 
				: new byte[((userPassBytes.Length/16)*16) + 16];

			Array.Copy(userPassBytes, 0, encryptedPass, 0, userPassBytes.Length);

			for (int i = userPassBytes.Length; i < encryptedPass.Length; i++) 
				encryptedPass[i] = 0;

			byte[] sharedSecretBytes = System.Text.Encoding.ASCII.GetBytes(sharedSecret);

			for (int chunk = 0; chunk < (encryptedPass.Length/16); chunk++)
			{
				MD5 md5 = new MD5CryptoServiceProvider();

				md5.TransformBlock(sharedSecretBytes, 0, sharedSecretBytes.Length, sharedSecretBytes, 0);
				if (chunk == 0)
					md5.TransformFinalBlock(requestAuthenticator, 0, requestAuthenticator.Length);
				else
					md5.TransformFinalBlock(encryptedPass, (chunk - 1)*16, 16);

				byte[] hash = md5.Hash;

				for (int i = 0; i < 16; i++)
				{
					int j = i + chunk*16;
					encryptedPass[j] = (byte) (hash[i] ^ encryptedPass[j]);
				}
			}

			return encryptedPass;
		}

		/// <summary>
		/// Converts a uint to a 3-byte value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static byte[] UintTo3Byte(uint val)
		{
			return new byte[] { (byte)(val >> 16 & 0xFF), (byte)(val >> 8 & 0xFF), (byte)(val & 0xFF) };
		}

		/// <summary>
		/// Converts an int to a 3-byte value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static byte[] IntTo3Byte(int val)
		{
			return UintTo3Byte((uint)val);
		}

		public static uint ThreeBytes2UInt(byte[] bytes, int offset)
		{
			return (uint)
			 (bytes[offset + 2] << 16
			| bytes[offset + 1] << 8
			| bytes[offset]);
		}

		public static byte[] GetNetworkBytes(short value)
		{
			int sizeOf = sizeof(short);
			byte[] shortArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				shortArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = (short)(value >> 8);
			}

			return shortArray;
		}

		public static byte[] GetNetworkBytes(ushort value)
		{
			int sizeOf = sizeof(ushort);
			byte[] ushortArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				ushortArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = (ushort)(value >> 8);
			}

			return ushortArray;
		}
		
		public static byte[] GetNetworkBytes(int value)
		{
			int sizeOf = sizeof(int);
			byte[] intArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				intArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = value >> 8;
			}
			
			return intArray;
		}

		public static byte[] GetNetworkBytes(uint value)
		{
			int sizeOf = sizeof(uint);
			byte[] uintArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				uintArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = value >> 8;
			}
			
			return uintArray;
		}

		public static byte[] GetNetworkBytes(long value)
		{
			int sizeOf = sizeof(long);
			byte[] longArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				longArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = value >> 8;
			}

			return longArray;
		}

		public static byte[] GetNetworkBytes(ulong value)
		{
			int sizeOf = sizeof(ulong);
			byte[] ulongArray = new byte[sizeOf];
			for (int i = 0; i < sizeOf; i++)
			{
				ulongArray[(i * -1) + sizeOf - 1] = (byte)(value & 0xFF);
				value = value >> 8;
			}
			
			return ulongArray;
		}
	}
}