using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Security
{

    /// <remarks>
    ///  Utility class for creating IDigest objects from their names/Oids
    /// </remarks>
    public sealed class DigestUtilities
    {
		private DigestUtilities()
		{
		}

        private static readonly Hashtable algorithms = new Hashtable();
        private static readonly Hashtable oids = new Hashtable();

        static DigestUtilities()
        {
            algorithms[PkcsObjectIdentifiers.MD2.Id] = "MD2";
            algorithms[PkcsObjectIdentifiers.MD4.Id] = "MD4";
            algorithms[PkcsObjectIdentifiers.MD5.Id] = "MD5";

            algorithms["SHA1"] = "SHA-1";
            algorithms[OiwObjectIdentifiers.IdSha1.Id] = "SHA-1";
            algorithms["SHA224"] = "SHA-224";
            algorithms[NistObjectIdentifiers.IdSha224.Id] = "SHA-224";
            algorithms["SHA256"] = "SHA-256";
            algorithms[NistObjectIdentifiers.IdSha256.Id] = "SHA-256";
            algorithms["SHA384"] = "SHA-384";
            algorithms[NistObjectIdentifiers.IdSha384.Id] = "SHA-384";
            algorithms["SHA512"] = "SHA-512";
            algorithms[NistObjectIdentifiers.IdSha512.Id] = "SHA-512";

            algorithms["RIPEMD-128"] = "RIPEMD128";
            algorithms[TeleTrusTObjectIdentifiers.RipeMD128.Id] = "RIPEMD128";
            algorithms["RIPEMD-160"] = "RIPEMD160";
            algorithms[TeleTrusTObjectIdentifiers.RipeMD160.Id] = "RIPEMD160";
            algorithms["RIPEMD-256"] = "RIPEMD256";
            algorithms[TeleTrusTObjectIdentifiers.RipeMD256.Id] = "RIPEMD256";
			algorithms["RIPEMD-320"] = "RIPEMD320";
//			algorithms[TeleTrusTObjectIdentifiers.RipeMD320.Id] = "RIPEMD320";

			algorithms[CryptoProObjectIdentifiers.GostR3411.Id] = "GOST3411";



            oids["MD2"] = PkcsObjectIdentifiers.MD2;
            oids["MD4"] = PkcsObjectIdentifiers.MD4;
            oids["MD5"] = PkcsObjectIdentifiers.MD5;
            oids["SHA-1"] = OiwObjectIdentifiers.IdSha1;
            oids["SHA-224"] = NistObjectIdentifiers.IdSha224;
            oids["SHA-256"] = NistObjectIdentifiers.IdSha256;
            oids["SHA-384"] = NistObjectIdentifiers.IdSha384;
            oids["SHA-512"] = NistObjectIdentifiers.IdSha512;
            oids["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
            oids["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
            oids["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
			oids["GOST3411"] = CryptoProObjectIdentifiers.GostR3411;
        }

		/// <summary>
        /// Returns a ObjectIdentifier for a given digest mechanism.
        /// </summary>
        /// <param name="mechanism">A string representation of the digest meanism.</param>
        /// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>

        public static DerObjectIdentifier GetObjectIdentifier(
			string mechanism)
        {
            mechanism = (string) algorithms[mechanism.ToUpper(CultureInfo.InvariantCulture)];

            if (mechanism != null)
            {
                return (DerObjectIdentifier)oids[mechanism];
            }

            return null;
        }

        public static ICollection Algorithms
        {
			get { return oids.Keys; }
        }

        public static IDigest GetDigest(
			DerObjectIdentifier id)
        {
            return GetDigest(id.Id);
        }

        public static IDigest GetDigest(
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
				case "GOST3411":	return new Gost3411Digest();
				case "MD2":			return new MD2Digest();
				case "MD4":			return new MD4Digest();
				case "MD5":			return new MD5Digest();
				case "RIPEMD128":	return new RipeMD128Digest();
				case "RIPEMD160":	return new RipeMD160Digest();
				case "RIPEMD256":	return new RipeMD256Digest();
				case "RIPEMD320":	return new RipeMD320Digest();
				case "SHA-1":		return new Sha1Digest();
				case "SHA-224":		return new Sha224Digest();
				case "SHA-256":		return new Sha256Digest();
				case "SHA-384":		return new Sha384Digest();
				case "SHA-512":		return new Sha512Digest();
				case "TIGER":		return new TigerDigest();
				case "WHIRLPOOL":	return new WhirlpoolDigest();
				default:
					throw new SecurityUtilityException("Digest " + mechanism + " not recognised.");
			}
        }

		public static string GetAlgorithmName(
			DerObjectIdentifier oid)
        {
            return (string) algorithms[oid.Id];
        }

		public static byte[] DoFinal(
			IDigest digest)
		{
			byte[] b = new byte[digest.GetDigestSize()];
			digest.DoFinal(b, 0);
			return b;
		}
    }
}
