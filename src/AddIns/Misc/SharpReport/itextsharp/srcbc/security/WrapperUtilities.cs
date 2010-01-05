using System;
using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;

namespace Org.BouncyCastle.Security
{
	/// <remarks>
	///  Utility class for creating IWrapper objects from their names/Oids
	/// </remarks>
	public sealed class WrapperUtilities
	{
		private WrapperUtilities()
		{
		}

		private static readonly Hashtable algorithms = new Hashtable();
//		private static readonly Hashtable oids = new Hashtable();

		static WrapperUtilities()
		{
			algorithms[NistObjectIdentifiers.IdAes128Wrap.Id] = "AESWRAP";
			algorithms[NistObjectIdentifiers.IdAes192Wrap.Id] = "AESWRAP";
			algorithms[NistObjectIdentifiers.IdAes256Wrap.Id] = "AESWRAP";

			algorithms[NttObjectIdentifiers.IdCamellia128Wrap.Id] = "CAMELLIAWRAP";
			algorithms[NttObjectIdentifiers.IdCamellia192Wrap.Id] = "CAMELLIAWRAP";
			algorithms[NttObjectIdentifiers.IdCamellia256Wrap.Id] = "CAMELLIAWRAP";

			algorithms[PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id] = "DESEDEWRAP";

			algorithms[PkcsObjectIdentifiers.IdAlgCmsRC2Wrap.Id] = "RC2WRAP";

			algorithms[KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id] = "SEEDWRAP";
		}

		public static IWrapper GetWrapper(
			DerObjectIdentifier oid)
		{
			return GetWrapper(oid.Id);
		}

		public static IWrapper GetWrapper(
			string algorithm)
		{
			string upper = algorithm.ToUpper(CultureInfo.InvariantCulture);
			string mechanism = (string) algorithms[upper];

			if (mechanism == null)
			{
				mechanism = upper;
			}

			switch (mechanism)
			{
				case "AESWRAP":
					return new AesWrapEngine();
				case "CAMELLIAWRAP":
					return new CamelliaWrapEngine();
				case "DESEDEWRAP":
					return new DesEdeWrapEngine();
				case "RC2WRAP":
					return new RC2WrapEngine();
				case "SEEDWRAP":
					return new SeedWrapEngine();
				case "DESEDERFC3211WRAP":
					return new Rfc3211WrapEngine(new DesEdeEngine());
				case "AESRFC3211WRAP":
					return new Rfc3211WrapEngine(new AesFastEngine());
				case "CAMELLIARFC3211WRAP":
					return new Rfc3211WrapEngine(new CamelliaEngine());
			}

			// Create an IBufferedCipher and use it as IWrapper (via BufferedCipherWrapper)
			IBufferedCipher blockCipher = CipherUtilities.GetCipher(algorithm);

			if (blockCipher != null)
				return new BufferedCipherWrapper(blockCipher);

			throw new SecurityUtilityException("Wrapper " + algorithm + " not recognised.");
		}

		public static string GetAlgorithmName(
			DerObjectIdentifier oid)
		{
			return (string) algorithms[oid.Id];
		}

		private class BufferedCipherWrapper
			: IWrapper
		{
			private readonly IBufferedCipher cipher;
			private bool forWrapping;

			public BufferedCipherWrapper(
				IBufferedCipher cipher)
			{
				this.cipher = cipher;
			}

			public string AlgorithmName
			{
				get { return cipher.AlgorithmName; }
			}

			public void Init(
				bool				forWrapping,
				ICipherParameters	parameters)
			{
				this.forWrapping = forWrapping;

				cipher.Init(forWrapping, parameters);
			}

			public byte[] Wrap(
				byte[]	input,
				int		inOff,
				int		length)
			{
				if (!forWrapping)
					throw new InvalidOperationException("Not initialised for wrapping");

				return cipher.DoFinal(input, inOff, length);
			}

			public byte[] Unwrap(
				byte[]	input,
				int		inOff,
				int		length)
			{
				if (forWrapping)
					throw new InvalidOperationException("Not initialised for Unwrapping");

				return cipher.DoFinal(input, inOff, length);
			}
		}
	}
}
