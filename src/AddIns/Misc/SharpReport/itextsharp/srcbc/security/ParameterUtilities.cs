using System;
using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Security
{
	public sealed class ParameterUtilities
	{
		private ParameterUtilities()
		{
		}

		private static readonly Hashtable algorithms = new Hashtable();

		static ParameterUtilities()
		{
			AddAlgorithm("AES",
				"AESWRAP");
			AddAlgorithm("AES128",
				"2.16.840.1.101.3.4.2",
				NistObjectIdentifiers.IdAes128Cbc,
				NistObjectIdentifiers.IdAes128Cfb,
				NistObjectIdentifiers.IdAes128Ecb,
				NistObjectIdentifiers.IdAes128Ofb,
				NistObjectIdentifiers.IdAes128Wrap);
			AddAlgorithm("AES192",
				"2.16.840.1.101.3.4.22",
				NistObjectIdentifiers.IdAes192Cbc,
				NistObjectIdentifiers.IdAes192Cfb,
				NistObjectIdentifiers.IdAes192Ecb,
				NistObjectIdentifiers.IdAes192Ofb,
				NistObjectIdentifiers.IdAes192Wrap);
			AddAlgorithm("AES256",
				"2.16.840.1.101.3.4.42",
				NistObjectIdentifiers.IdAes256Cbc,
				NistObjectIdentifiers.IdAes256Cfb,
				NistObjectIdentifiers.IdAes256Ecb,
				NistObjectIdentifiers.IdAes256Ofb,
				NistObjectIdentifiers.IdAes256Wrap);
			AddAlgorithm("BLOWFISH");
			AddAlgorithm("CAMELLIA",
				"CAMELLIAWRAP");
			AddAlgorithm("CAMELLIA128",
				NttObjectIdentifiers.IdCamellia128Cbc,
				NttObjectIdentifiers.IdCamellia128Wrap);
			AddAlgorithm("CAMELLIA192",
				NttObjectIdentifiers.IdCamellia192Cbc,
				NttObjectIdentifiers.IdCamellia192Wrap);
			AddAlgorithm("CAMELLIA256",
				NttObjectIdentifiers.IdCamellia256Cbc,
				NttObjectIdentifiers.IdCamellia256Wrap);
			AddAlgorithm("CAST5",
				"1.2.840.113533.7.66.10");
			AddAlgorithm("CAST6");
			AddAlgorithm("DES",
				OiwObjectIdentifiers.DesCbc);
			AddAlgorithm("DESEDE",
				"DESEDEWRAP",
				PkcsObjectIdentifiers.IdAlgCms3DesWrap);
			AddAlgorithm("DESEDE3",
				PkcsObjectIdentifiers.DesEde3Cbc);
			AddAlgorithm("GOST28147",
				"GOST",
				"GOST-28147",
				CryptoProObjectIdentifiers.GostR28147Cbc);
			AddAlgorithm("HC128");
			AddAlgorithm("HC256");
			AddAlgorithm("IDEA",
				"1.3.6.1.4.1.188.7.1.1.2");
			AddAlgorithm("NOEKEON");
			AddAlgorithm("RC2",
				PkcsObjectIdentifiers.RC2Cbc,
				PkcsObjectIdentifiers.IdAlgCmsRC2Wrap);
			AddAlgorithm("RC4",
				"ARC4",
				"1.2.840.113549.3.4");
			AddAlgorithm("RC5",
				"RC5-32");
			AddAlgorithm("RC5-64");
			AddAlgorithm("RC6");
			AddAlgorithm("RIJNDAEL");
			AddAlgorithm("SALSA20");
			AddAlgorithm("SEED",
				KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap,
				KisaObjectIdentifiers.IdSeedCbc);
			AddAlgorithm("SERPENT");
			AddAlgorithm("SKIPJACK");
			AddAlgorithm("TEA");
			AddAlgorithm("TWOFISH");
			AddAlgorithm("VMPC");
			AddAlgorithm("VMPC-KSA3");
			AddAlgorithm("XTEA");
		}

		private static void AddAlgorithm(
			string			canonicalName,
			params object[]	aliases)
		{
			algorithms[canonicalName] = canonicalName;

			foreach (object alias in aliases)
			{
				algorithms[alias.ToString()] = canonicalName;
			}
		}

		public static string GetCanonicalAlgorithmName(
			string algorithm)
		{
			return (string) algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];
		}

		public static KeyParameter CreateKeyParameter(
			DerObjectIdentifier algOid,
			byte[]				keyBytes)
		{
			return CreateKeyParameter(algOid.Id, keyBytes, 0, keyBytes.Length);
		}

		public static KeyParameter CreateKeyParameter(
			string	algorithm,
			byte[]	keyBytes)
		{
			return CreateKeyParameter(algorithm, keyBytes, 0, keyBytes.Length);
		}

		public static KeyParameter CreateKeyParameter(
			DerObjectIdentifier algOid,
			byte[]				keyBytes,
			int					offset,
			int					length)
		{
			return CreateKeyParameter(algOid.Id, keyBytes, offset, length);
		}

		public static KeyParameter CreateKeyParameter(
			string	algorithm,
			byte[]	keyBytes,
			int		offset,
			int		length)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			string canonical = GetCanonicalAlgorithmName(algorithm);

			if (canonical == null)
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");

			switch (canonical)
			{
				case "DES":
					return new DesParameters(keyBytes, offset, length);
				case "DESEDE":
				case "DESEDE3":
					return new DesEdeParameters(keyBytes, offset, length);
				case "RC2":
					return new RC2Parameters(keyBytes, offset, length);
				default:
					return new KeyParameter(keyBytes, offset, length);
			}
		}

		public static ICipherParameters GetCipherParameters(
			DerObjectIdentifier	algOid,
			ICipherParameters	key,
			Asn1Object			asn1Params)
		{
			return GetCipherParameters(algOid.Id, key, asn1Params);
		}

		public static ICipherParameters GetCipherParameters(
			string				algorithm,
			ICipherParameters	key,
			Asn1Object			asn1Params)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			string canonical = GetCanonicalAlgorithmName(algorithm);

			if (canonical == null)
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");

			byte[] iv = null;

			try
			{
				switch (canonical)
				{
					case "AES":
					case "AES128":
					case "AES192":
					case "AES256":
					case "BLOWFISH":
					case "CAMELLIA":
					case "CAMELLIA128":
					case "CAMELLIA192":
					case "CAMELLIA256":
					case "DES":
					case "DESEDE":
					case "DESEDE3":
					case "NOEKEON":
					case "RIJNDAEL":
					case "SEED":
					case "SKIPJACK":
					case "TWOFISH":
						iv = ((Asn1OctetString) asn1Params).GetOctets();
						break;
					case "RC2":
						iv = RC2CbcParameter.GetInstance(asn1Params).GetIV();
						break;
					case "IDEA":
						iv = IdeaCbcPar.GetInstance(asn1Params).GetIV();
						break;
					case "CAST5":
						iv = Cast5CbcParameters.GetInstance(asn1Params).GetIV();
						break;
				}
			}
			catch (Exception e)
			{
				throw new ArgumentException("Could not process ASN.1 parameters", e);
			}

			if (iv != null)
			{
				return new ParametersWithIV(key, iv);
			}

			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}

		public static Asn1Encodable GenerateParameters(
			DerObjectIdentifier algID,
			SecureRandom		random)
		{
			return GenerateParameters(algID.Id, random);
		}

		public static Asn1Encodable GenerateParameters(
			string			algorithm,
			SecureRandom	random)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			string canonical = GetCanonicalAlgorithmName(algorithm);

			if (canonical == null)
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");

			switch (canonical)
			{
				// TODO These algorithms support an IV (see GetCipherParameters)
				// but JCE doesn't seem to provide an AlgorithmParametersGenerator for them
//				case "BLOWFISH":
//				case "RIJNDAEL":
//				case "SKIPJACK":
//				case "TWOFISH":

				case "AES":
				case "AES128":
				case "AES192":
				case "AES256":
					return CreateIVOctetString(random, 16);
				case "CAMELLIA":
				case "CAMELLIA128":
				case "CAMELLIA192":
				case "CAMELLIA256":
					return CreateIVOctetString(random, 16);
				case "CAST5":
					return new Cast5CbcParameters(CreateIV(random, 8), 128);
				case "DES":
				case "DESEDE":
				case "DESEDE3":
					return CreateIVOctetString(random, 8);
				case "IDEA":
					return new IdeaCbcPar(CreateIV(random, 8));
				case "NOEKEON":
					return CreateIVOctetString(random, 16);
				case "RC2":
					return new RC2CbcParameter(CreateIV(random, 8));
				case "SEED":
					return CreateIVOctetString(random, 16);
			}

			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}

		private static Asn1OctetString CreateIVOctetString(
			SecureRandom	random,
			int				ivLength)
		{
			return new DerOctetString(CreateIV(random, ivLength));
		}

		private static byte[] CreateIV(
			SecureRandom	random,
			int				ivLength)
		{
			byte[] iv = new byte[ivLength];
			random.NextBytes(iv);
			return iv;
		}
	}
}
