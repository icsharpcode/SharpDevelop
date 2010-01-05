using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Security
{
	/// <remarks>
	///  Utility class for creating IBasicAgreement objects from their names/Oids
	/// </remarks>
	public sealed class AgreementUtilities
	{
		private AgreementUtilities()
		{
		}

		private static readonly Hashtable algorithms = new Hashtable();
		//		private static readonly Hashtable oids = new Hashtable();

		static AgreementUtilities()
		{
			//algorithms[X9ObjectIdentifiers.DHSinglePassCofactorDHSha1KdfScheme.Id] = ?;
			algorithms[X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme.Id] = "DHWITHSHA1KDF";
			//algorithms[X9ObjectIdentifiers.MqvSinglePassSha1KdfScheme.Id] = ?;
		}

		public static IBasicAgreement GetBasicAgreement(
			DerObjectIdentifier oid)
		{
			return GetBasicAgreement(oid.Id);
		}

		public static IBasicAgreement GetBasicAgreement(
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
				case "DH":
					return new DHBasicAgreement();
				case "ECDH":
					return new ECDHBasicAgreement();
				case "ECDHC":
					return new ECDHCBasicAgreement();
			}

			throw new SecurityUtilityException("Basic Agreement " + algorithm + " not recognised.");
		}

		public static IBasicAgreement GetBasicAgreementWithKdf(
			DerObjectIdentifier oid,
			string				wrapAlgorithm)
		{
			return GetBasicAgreementWithKdf(oid.Id, wrapAlgorithm);
		}

		public static IBasicAgreement GetBasicAgreementWithKdf(
			string agreeAlgorithm,
			string wrapAlgorithm)
		{
			string upper = agreeAlgorithm.ToUpper(CultureInfo.InvariantCulture);
			string mechanism = (string) algorithms[upper];

			if (mechanism == null)
			{
				mechanism = upper;
			}

			switch (mechanism)
			{
				case "DHWITHSHA1KDF":
					return new ECDHWithKdfBasicAgreement(
						wrapAlgorithm,
						new ECDHKekGenerator(
							new Sha1Digest()));
			}

			throw new SecurityUtilityException("Basic Agreement (with KDF) " + agreeAlgorithm + " not recognised.");
		}

		public static string GetAlgorithmName(
			DerObjectIdentifier oid)
		{
			return (string) algorithms[oid.Id];
		}
	}
}
