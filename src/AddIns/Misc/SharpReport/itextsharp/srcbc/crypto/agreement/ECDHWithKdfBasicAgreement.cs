using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Agreement
{
	public class ECDHWithKdfBasicAgreement
		: ECDHBasicAgreement
	{
		private static readonly Hashtable algorithms = new Hashtable();

		static ECDHWithKdfBasicAgreement()
		{
			algorithms.Add(NistObjectIdentifiers.IdAes128Cbc.Id, 128);
			algorithms.Add(NistObjectIdentifiers.IdAes192Cbc.Id, 192);
			algorithms.Add(NistObjectIdentifiers.IdAes256Cbc.Id, 256);
			algorithms.Add(NistObjectIdentifiers.IdAes128Wrap.Id, 128);
			algorithms.Add(NistObjectIdentifiers.IdAes192Wrap.Id, 192);
			algorithms.Add(NistObjectIdentifiers.IdAes256Wrap.Id, 256);
			algorithms.Add(PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id, 192);
		}

		private readonly string algorithm;
		private readonly IDerivationFunction kdf;

		public ECDHWithKdfBasicAgreement(
			string				algorithm,
			IDerivationFunction	kdf)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");
			if (!algorithms.Contains(algorithm))
				throw new ArgumentException("Unknown algorithm", "algorithm");
			if (kdf == null)
				throw new ArgumentNullException("kdf");

			this.algorithm = algorithm;
			this.kdf = kdf;
		}

		public override BigInteger CalculateAgreement(
			ICipherParameters pubKey)
		{
			BigInteger result = base.CalculateAgreement(pubKey);

			int keySize = (int) algorithms[algorithm];

			DHKdfParameters dhKdfParams = new DHKdfParameters(
				new DerObjectIdentifier(algorithm),
				keySize,
				// TODO Fix the way bytes are derived from the secret
				result.ToByteArrayUnsigned());

			kdf.Init(dhKdfParams);

			byte[] keyBytes = new byte[keySize / 8];
			kdf.GenerateBytes(keyBytes, 0, keyBytes.Length);

			return new BigInteger(1, keyBytes);
		}
	}
}
