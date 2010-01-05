using System;
using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;

namespace Org.BouncyCastle.Security
{
    /// <remarks>
    ///  Cipher Utility class contains methods that can not be specifically grouped into other classes.
    /// </remarks>
    public sealed class CipherUtilities
    {
        private static readonly Hashtable algorithms = new Hashtable();
        private static readonly Hashtable oids = new Hashtable();

		static CipherUtilities()
		{
			// TODO Flesh out the list of aliases

			algorithms[NistObjectIdentifiers.IdAes128Ecb.Id] = "AES/ECB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes192Ecb.Id] = "AES/ECB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes256Ecb.Id] = "AES/ECB/PKCS7PADDING";
			algorithms["AES/ECB/PKCS7"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES//PKCS7"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES//PKCS7PADDING"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES/ECB/PKCS5"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES/ECB/PKCS5PADDING"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES//PKCS5"] = "AES/ECB/PKCS7PADDING";
			algorithms["AES//PKCS5PADDING"] = "AES/ECB/PKCS7PADDING";

			algorithms[NistObjectIdentifiers.IdAes128Cbc.Id] = "AES/CBC/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes192Cbc.Id] = "AES/CBC/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes256Cbc.Id] = "AES/CBC/PKCS7PADDING";
			algorithms["AES/CBC/PKCS7"] = "AES/CBC/PKCS7PADDING";
			algorithms["AES/CBC/PKCS5"] = "AES/CBC/PKCS7PADDING";
			algorithms["AES/CBC/PKCS5PADDING"] = "AES/CBC/PKCS7PADDING";

			algorithms[NistObjectIdentifiers.IdAes128Ofb.Id] = "AES/OFB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes192Ofb.Id] = "AES/OFB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes256Ofb.Id] = "AES/OFB/PKCS7PADDING";
			algorithms["AES/OFB/PKCS7"] = "AES/OFB/PKCS7PADDING";
			algorithms["AES/OFB/PKCS5"] = "AES/OFB/PKCS7PADDING";
			algorithms["AES/OFB/PKCS5PADDING"] = "AES/OFB/PKCS7PADDING";

			algorithms[NistObjectIdentifiers.IdAes128Cfb.Id] = "AES/CFB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes192Cfb.Id] = "AES/CFB/PKCS7PADDING";
			algorithms[NistObjectIdentifiers.IdAes256Cfb.Id] = "AES/CFB/PKCS7PADDING";
			algorithms["AES/CFB/PKCS7"] = "AES/CFB/PKCS7PADDING";
			algorithms["AES/CFB/PKCS5"] = "AES/CFB/PKCS7PADDING";
			algorithms["AES/CFB/PKCS5PADDING"] = "AES/CFB/PKCS7PADDING";

			algorithms["RSA//PKCS1"] = "RSA//PKCS1PADDING";
			algorithms["RSA/ECB/PKCS1"] = "RSA//PKCS1PADDING";
			algorithms["RSA/ECB/PKCS1PADDING"] = "RSA//PKCS1PADDING";
			algorithms[PkcsObjectIdentifiers.RsaEncryption.Id] = "RSA//PKCS1PADDING";

			algorithms[OiwObjectIdentifiers.DesCbc.Id] = "DES/CBC";
			algorithms[PkcsObjectIdentifiers.DesEde3Cbc.Id] = "DESEDE/CBC";
			algorithms[PkcsObjectIdentifiers.RC2Cbc.Id] = "RC2/CBC";
			algorithms["1.3.6.1.4.1.188.7.1.1.2"] = "IDEA/CBC";
			algorithms["1.2.840.113533.7.66.10"] = "CAST5/CBC";

			algorithms["RC4"] = "ARC4";
			algorithms["ARCFOUR"] = "ARC4";
			algorithms["1.2.840.113549.3.4"] = "ARC4";



			algorithms["PBEWITHSHA1AND128BITRC4"] = "PBEWITHSHAAND128BITRC4";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd128BitRC4.Id] = "PBEWITHSHAAND128BITRC4";
			algorithms["PBEWITHSHA1AND40BITRC4"] = "PBEWITHSHAAND40BITRC4";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd40BitRC4.Id] = "PBEWITHSHAAND40BITRC4";

			algorithms["PBEWITHSHA1ANDDES"] = "PBEWITHSHA1ANDDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithSha1AndDesCbc.Id] = "PBEWITHSHA1ANDDES-CBC";
			algorithms["PBEWITHSHA1ANDRC2"] = "PBEWITHSHA1ANDRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithSha1AndRC2Cbc.Id] = "PBEWITHSHA1ANDRC2-CBC";

			algorithms["PBEWITHSHA1AND3-KEYTRIPLEDES-CBC"] = "PBEWITHSHAAND3-KEYTRIPLEDES-CBC";
			algorithms["PBEWITHSHAAND3KEYTRIPLEDES"] = "PBEWITHSHAAND3-KEYTRIPLEDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc.Id] = "PBEWITHSHAAND3-KEYTRIPLEDES-CBC";
			algorithms["PBEWITHSHA1ANDDESEDE"] = "PBEWITHSHAAND3-KEYTRIPLEDES-CBC";

			algorithms["PBEWITHSHA1AND2-KEYTRIPLEDES-CBC"] = "PBEWITHSHAAND2-KEYTRIPLEDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd2KeyTripleDesCbc.Id] = "PBEWITHSHAAND2-KEYTRIPLEDES-CBC";

			algorithms["PBEWITHSHA1AND128BITRC2-CBC"] = "PBEWITHSHAAND128BITRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd128BitRC2Cbc.Id] = "PBEWITHSHAAND128BITRC2-CBC";

			algorithms["PBEWITHSHA1AND40BITRC2-CBC"] = "PBEWITHSHAAND40BITRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc.Id] = "PBEWITHSHAAND40BITRC2-CBC";

			algorithms["PBEWITHSHA1AND128BITAES-CBC-BC"] = "PBEWITHSHAAND128BITAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND128BITAES-CBC-BC"] = "PBEWITHSHAAND128BITAES-CBC-BC";

			algorithms["PBEWITHSHA1AND192BITAES-CBC-BC"] = "PBEWITHSHAAND192BITAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND192BITAES-CBC-BC"] = "PBEWITHSHAAND192BITAES-CBC-BC";

			algorithms["PBEWITHSHA1AND256BITAES-CBC-BC"] = "PBEWITHSHAAND256BITAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND256BITAES-CBC-BC"] = "PBEWITHSHAAND256BITAES-CBC-BC";

			algorithms["PBEWITHSHA-256AND128BITAES-CBC-BC"] = "PBEWITHSHA256AND128BITAES-CBC-BC";
			algorithms["PBEWITHSHA-256AND192BITAES-CBC-BC"] = "PBEWITHSHA256AND192BITAES-CBC-BC";
			algorithms["PBEWITHSHA-256AND256BITAES-CBC-BC"] = "PBEWITHSHA256AND256BITAES-CBC-BC";


			algorithms["GOST"] = "GOST28147";
			algorithms["GOST-28147"] = "GOST28147";
			algorithms[CryptoProObjectIdentifiers.GostR28147Cbc.Id] = "GOST28147/CBC/PKCS7PADDING";

			algorithms["RC5-32"] = "RC5";

			algorithms[NttObjectIdentifiers.IdCamellia128Cbc.Id] = "CAMELLIA/CBC/PKCS7PADDING";
			algorithms[NttObjectIdentifiers.IdCamellia192Cbc.Id] = "CAMELLIA/CBC/PKCS7PADDING";
			algorithms[NttObjectIdentifiers.IdCamellia256Cbc.Id] = "CAMELLIA/CBC/PKCS7PADDING";

			algorithms[KisaObjectIdentifiers.IdSeedCbc.Id] = "SEED/CBC/PKCS7PADDING";
		}

		private CipherUtilities()
        {
        }

		/// <summary>
        /// Returns a ObjectIdentifier for a give encoding.
        /// </summary>
        /// <param name="mechanism">A string representation of the encoding.</param>
        /// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>
		// TODO Don't really want to support this
		public static DerObjectIdentifier GetObjectIdentifier(
            string mechanism)
        {
			if (mechanism == null)
				throw new ArgumentNullException("mechanism");

			mechanism = mechanism.ToUpper(CultureInfo.InvariantCulture);
			string aliased = (string) algorithms[mechanism];

			if (aliased != null)
				mechanism = aliased;

			return (DerObjectIdentifier) oids[mechanism];
        }

		public static ICollection Algorithms
        {
            get { return oids.Keys; }
        }

		public static IBufferedCipher GetCipher(
            DerObjectIdentifier oid)
        {
            return GetCipher(oid.Id);
        }

		public static IBufferedCipher GetCipher(
            string algorithm)
        {
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			algorithm = algorithm.ToUpper(CultureInfo.InvariantCulture);

			string aliased = (string) algorithms[algorithm];

			if (aliased != null)
				algorithm = aliased;



			IBasicAgreement iesAgreement = null;
			if (algorithm == "IES")
			{
				iesAgreement = new DHBasicAgreement();
			}
			else if (algorithm == "ECIES")
			{
				iesAgreement = new ECDHBasicAgreement();
			}

			if (iesAgreement != null)
			{
				return new BufferedIesCipher(
					new IesEngine(
						iesAgreement,
						new Kdf2BytesGenerator(
							new Sha1Digest()),
						new HMac(
							new Sha1Digest())));
			}



			if (algorithm.StartsWith("PBE"))
			{
				switch (algorithm)
				{
					case "PBEWITHSHAAND2-KEYTRIPLEDES-CBC":
					case "PBEWITHSHAAND3-KEYTRIPLEDES-CBC":
						return new PaddedBufferedBlockCipher(
							new CbcBlockCipher(new DesEdeEngine()));

					case "PBEWITHSHAAND128BITRC2-CBC":
					case "PBEWITHSHAAND40BITRC2-CBC":
						return new PaddedBufferedBlockCipher(
							new CbcBlockCipher(new RC2Engine()));

					case "PBEWITHSHAAND128BITAES-CBC-BC":
					case "PBEWITHSHAAND192BITAES-CBC-BC":
					case "PBEWITHSHAAND256BITAES-CBC-BC":
					case "PBEWITHSHA256AND128BITAES-CBC-BC":
					case "PBEWITHSHA256AND192BITAES-CBC-BC":
					case "PBEWITHSHA256AND256BITAES-CBC-BC":
					case "PBEWITHMD5AND128BITAES-CBC-OPENSSL":
					case "PBEWITHMD5AND192BITAES-CBC-OPENSSL":
					case "PBEWITHMD5AND256BITAES-CBC-OPENSSL":
						return new PaddedBufferedBlockCipher(
							new CbcBlockCipher(new AesFastEngine()));

					case "PBEWITHSHA1ANDDES-CBC":
						return new PaddedBufferedBlockCipher(
							new CbcBlockCipher(new DesEngine()));

					case "PBEWITHSHA1ANDRC2-CBC":
						return new PaddedBufferedBlockCipher(
							new CbcBlockCipher(new RC2Engine()));
				}
			}



			string[] parts = algorithm.Split('/');

			IBlockCipher blockCipher = null;
            IAsymmetricBlockCipher asymBlockCipher = null;
			IStreamCipher streamCipher = null;

			switch (parts[0])
			{
				case "AES":
					blockCipher = new AesFastEngine();
					break;
				case "ARC4":
					streamCipher = new RC4Engine();
					break;
				case "BLOWFISH":
					blockCipher = new BlowfishEngine();
					break;
				case "CAMELLIA":
					blockCipher = new CamelliaEngine();
					break;
				case "CAST5":
					blockCipher = new Cast5Engine();
					break;
				case "CAST6":
					blockCipher = new Cast6Engine();
					break;
				case "DES":
					blockCipher = new DesEngine();
					break;
				case "DESEDE":
					blockCipher = new DesEdeEngine();
					break;
				case "ELGAMAL":
					asymBlockCipher = new ElGamalEngine();
					break;
				case "GOST28147":
					blockCipher = new Gost28147Engine();
					break;
				case "HC128":
					streamCipher = new HC128Engine();
					break;
				case "HC256":
					streamCipher = new HC256Engine();
					break;
				case "IDEA":
					blockCipher = new IdeaEngine();
					break;
				case "NOEKEON":
					blockCipher = new NoekeonEngine();
					break;
				case "PBEWITHSHAAND128BITRC4":
				case "PBEWITHSHAAND40BITRC4":
					streamCipher = new RC4Engine();
					break;
				case "RC2":
					blockCipher = new RC2Engine();
					break;
				case "RC5":
					blockCipher = new RC532Engine();
					break;
				case "RC5-64":
					blockCipher = new RC564Engine();
					break;
				case "RC6":
					blockCipher = new RC6Engine();
					break;
				case "RIJNDAEL":
					blockCipher = new RijndaelEngine();
					break;
				case "RSA":
					asymBlockCipher = new RsaBlindedEngine();
					break;
				case "SALSA20":
					streamCipher = new Salsa20Engine();
					break;
				case "SEED":
					blockCipher = new SeedEngine();
					break;
				case "SERPENT":
					blockCipher = new SerpentEngine();
					break;
				case "SKIPJACK":
					blockCipher = new SkipjackEngine();
					break;
				case "TEA":
					blockCipher = new TeaEngine();
					break;
				case "TWOFISH":
					blockCipher = new TwofishEngine();
					break;
				case "VMPC":
					streamCipher = new VmpcEngine();
					break;
				case "VMPC-KSA3":
					streamCipher = new VmpcKsa3Engine();
					break;
				case "XTEA":
					blockCipher = new XteaEngine();
					break;
				default:
					throw new SecurityUtilityException("Cipher " + algorithm + " not recognised.");
			}

			if (streamCipher != null)
			{
				if (parts.Length > 1)
					throw new ArgumentException("Modes and paddings not used for stream ciphers");

				return new BufferedStreamCipher(streamCipher);
			}


			bool cts = false;
			bool padded = true;
			IBlockCipherPadding padding = null;
			IAeadBlockCipher aeadBlockCipher = null;

			if (parts.Length > 2)
            {
				if (streamCipher != null)
					throw new ArgumentException("Paddings not used for stream ciphers");

				switch (parts[2])
				{
					case "NOPADDING":
						padded = false;
						break;
					case "":
					case "RAW":
						break;
					case "ISO10126PADDING":
					case "ISO10126D2PADDING":
					case "ISO10126-2PADDING":
						padding = new ISO10126d2Padding();
						break;
					case "ISO7816-4PADDING":
					case "ISO9797-1PADDING":
						padding = new ISO7816d4Padding();
						break;
					case "ISO9796-1":
					case "ISO9796-1PADDING":
						asymBlockCipher = new ISO9796d1Encoding(asymBlockCipher);
						break;
					case "OAEP":
					case "OAEPPADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher);
						break;
					case "OAEPWITHMD5ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new MD5Digest());
						break;
					case "OAEPWITHSHA1ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new Sha1Digest());
						break;
					case "OAEPWITHSHA224ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new Sha224Digest());
						break;
					case "OAEPWITHSHA256ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new Sha256Digest());
						break;
					case "OAEPWITHSHA384ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new Sha384Digest());
						break;
					case "OAEPWITHSHA512ANDMGF1PADDING":
						asymBlockCipher = new OaepEncoding(asymBlockCipher, new Sha512Digest());
						break;
					case "PKCS1":
					case "PKCS1PADDING":
						asymBlockCipher = new Pkcs1Encoding(asymBlockCipher);
						break;
					case "PKCS5":
					case "PKCS5PADDING":
					case "PKCS7":
					case "PKCS7PADDING":
						// NB: Padding defaults to Pkcs7Padding already
						break;
					case "TBCPADDING":
						padding = new TbcPadding();
						break;
					case "WITHCTS":
						cts = true;
						break;
					case "X9.23PADDING":
					case "X923PADDING":
						padding = new X923Padding();
						break;
					case "ZEROBYTEPADDING":
						padding = new ZeroBytePadding();
						break;
					default:
						throw new SecurityUtilityException("Cipher " + algorithm + " not recognised.");
				}
            }

			string mode = "";
			if (parts.Length > 1)
            {
				mode = parts[1];

				int di = GetDigitIndex(mode);
				string modeName = di >= 0 ? mode.Substring(0, di) : mode;

				switch (modeName)
				{
					case "":
					case "ECB":
					case "NONE":
						break;
					case "CBC":
						blockCipher = new CbcBlockCipher(blockCipher);
						break;
					case "CCM":
						aeadBlockCipher = new CcmBlockCipher(blockCipher);
						break;
					case "CFB":
					{
						int bits = (di < 0)
							?	8 * blockCipher.GetBlockSize()
							:	int.Parse(mode.Substring(di));

						blockCipher = new CfbBlockCipher(blockCipher, bits);
						break;
					}
					case "CTR":
						blockCipher = new SicBlockCipher(blockCipher);
						break;
					case "CTS":
						cts = true;
						blockCipher = new CbcBlockCipher(blockCipher);
						break;
					case "EAX":
						aeadBlockCipher = new EaxBlockCipher(blockCipher);
						break;
					case "GCM":
						aeadBlockCipher = new GcmBlockCipher(blockCipher);
						break;
					case "GOFB":
						blockCipher = new GOfbBlockCipher(blockCipher);
						break;
					case "OFB":
					{
						int bits = (di < 0)
							?	8 * blockCipher.GetBlockSize()
							:	int.Parse(mode.Substring(di));

						blockCipher = new OfbBlockCipher(blockCipher, bits);
						break;
					}
					case "OPENPGPCFB":
						blockCipher = new OpenPgpCfbBlockCipher(blockCipher);
						break;
					case "SIC":
						if (blockCipher.GetBlockSize() < 16)
						{
							throw new ArgumentException("Warning: SIC-Mode can become a twotime-pad if the blocksize of the cipher is too small. Use a cipher with a block size of at least 128 bits (e.g. AES)");
						}
						blockCipher = new SicBlockCipher(blockCipher);
						break;
					default:
						throw new SecurityUtilityException("Cipher " + algorithm + " not recognised.");
				}
            }

			if (aeadBlockCipher != null)
			{
				if (cts)
					throw new SecurityUtilityException("CTS mode not valid for AEAD ciphers.");
				if (padded && parts.Length > 1 && parts[2] != "")
					throw new SecurityUtilityException("Bad padding specified for AEAD cipher.");

				return new BufferedAeadBlockCipher(aeadBlockCipher);
			}

			if (blockCipher != null)
            {
				if (cts)
				{
					return new CtsBlockCipher(blockCipher);
				}

				if (!padded || blockCipher.IsPartialBlockOkay)
				{
					return new BufferedBlockCipher(blockCipher);
				}

				if (padding != null)
				{
					return new PaddedBufferedBlockCipher(blockCipher, padding);
				}

				return new PaddedBufferedBlockCipher(blockCipher);
            }

			if (asymBlockCipher != null)
            {
                return new BufferedAsymmetricBlockCipher(asymBlockCipher);
            }

			throw new SecurityUtilityException("Cipher " + algorithm + " not recognised.");
        }

        public static string GetAlgorithmName(
			DerObjectIdentifier oid)
        {
            return (string) algorithms[oid.Id];
        }

		private static int GetDigitIndex(
			string s)
		{
			for (int i = 0; i < s.Length; ++i)
			{
				if (char.IsDigit(s[i]))
					return i;
			}

			return -1;
		}
	}
}
