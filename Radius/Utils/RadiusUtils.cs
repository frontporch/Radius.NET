using System;
using System.Security.Cryptography;

namespace FP.Radius
{
	internal class Utils
	{
		public static byte[] RequestAuthenticator(string sharedSecret)
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

	}
}