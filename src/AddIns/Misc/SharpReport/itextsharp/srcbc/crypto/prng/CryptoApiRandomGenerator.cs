#if !NETCF_1_0

using System;
using System.Security.Cryptography;

namespace Org.BouncyCastle.Crypto.Prng
{
	/// <summary>
	/// Uses Microsoft's RNGCryptoServiceProvider
	/// </summary>
	public class CryptoApiRandomGenerator
		: IRandomGenerator
	{
		private readonly RNGCryptoServiceProvider rndProv;

		public CryptoApiRandomGenerator()
		{
			rndProv = new RNGCryptoServiceProvider();
		}

		#region IRandomGenerator Members

		public virtual void AddSeedMaterial(byte[] seed)
		{
			// We don't care about the seed
		}

		public virtual void AddSeedMaterial(long seed)
		{
			// We don't care about the seed
		}

		public virtual void NextBytes(byte[] bytes)
		{
			rndProv.GetBytes(bytes);
		}

		public virtual void NextBytes(byte[] bytes, int start, int len)
		{
			if (start < 0)
				throw new ArgumentException("Start offset cannot be negative", "start");
			if (bytes.Length < (start + len))
				throw new ArgumentException("Byte array too small for requested offset and length");

			if (bytes.Length == len && start == 0) 
			{
				NextBytes(bytes);
			}
			else 
			{
				byte[] tmpBuf = new byte[len];
				rndProv.GetBytes(tmpBuf);
				Array.Copy(tmpBuf, 0, bytes, start, len);
			}
		}

		#endregion
	}
}

#endif
