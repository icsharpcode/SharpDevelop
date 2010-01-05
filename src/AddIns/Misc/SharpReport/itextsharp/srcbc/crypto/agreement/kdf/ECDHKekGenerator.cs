using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Agreement.Kdf
{
	/**
	* X9.63 based key derivation function for ECDH CMS.
	*/
	public class ECDHKekGenerator
		: IDerivationFunction
	{
		private readonly IDerivationFunction kdf;

		private DerObjectIdentifier	algorithm;
		private int					keySize;
		private byte[]				z;

		public ECDHKekGenerator(
			IDigest digest)
		{
			this.kdf = new Kdf2BytesGenerator(digest);
		}

		public void Init(
			IDerivationParameters param)
		{
			DHKdfParameters parameters = (DHKdfParameters)param;

			this.algorithm = parameters.Algorithm;
			this.keySize = parameters.KeySize;
			this.z = parameters.GetZ(); // TODO Clone?
		}

		public IDigest Digest
		{
			get { return kdf.Digest; }
		}

		public int GenerateBytes(
			byte[]	outBytes,
			int		outOff,
			int		len)
		{
			// ECC-CMS-SharedInfo
			DerSequence s = new DerSequence(
				new AlgorithmIdentifier(algorithm, DerNull.Instance),
				new DerTaggedObject(true, 2, new DerOctetString(integerToBytes(keySize))));

			kdf.Init(new KdfParameters(z, s.GetDerEncoded()));

			return kdf.GenerateBytes(outBytes, outOff, len);
		}

		private byte[] integerToBytes(int keySize)
		{
			byte[] val = new byte[4];

			val[0] = (byte)(keySize >> 24);
			val[1] = (byte)(keySize >> 16);
			val[2] = (byte)(keySize >> 8);
			val[3] = (byte)keySize;

			return val;
		}
	}
}
