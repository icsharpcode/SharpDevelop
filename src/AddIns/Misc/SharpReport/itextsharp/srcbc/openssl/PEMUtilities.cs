using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.OpenSsl
{
	internal sealed class PemUtilities
	{
		internal static bool ParseDekAlgName(
			string		dekAlgName,
			out string	baseAlg,
			out string	mode)
		{
			baseAlg = dekAlgName;
			mode = "ECB";

			if (dekAlgName == "DES-EDE" || dekAlgName == "DES-EDE3")
				return true;

			int pos = dekAlgName.LastIndexOf('-');
			if (pos < 0)
				return false;

			baseAlg = dekAlgName.Substring(0, pos);
			mode = dekAlgName.Substring(pos + 1);

			return true;
		}

		internal static byte[] Crypt(
			bool	encrypt,
			byte[]	bytes,
			char[]	password,
			string	dekAlgName,
			byte[]	iv)
		{
			string baseAlg, mode;
			if (!ParseDekAlgName(dekAlgName, out baseAlg, out mode))
				throw new ArgumentException("Unknown DEK algorithm: " + dekAlgName, "dekAlgName");

			string padding;
			switch (mode)
			{
				case "CBC":
				case "ECB":
					padding = "PKCS5Padding";
					break;
				case "CFB":
				case "OFB":
					padding = "NoPadding";
					break;
				default:
					throw new ArgumentException("Unknown DEK algorithm: " + dekAlgName, "dekAlgName");
			}

			string algorithm;

			byte[] salt = iv;
			switch (baseAlg)
			{
				case "AES-128":
				case "AES-192":
				case "AES-256":
					algorithm = "AES";
					if (salt.Length > 8)
					{
						salt = new byte[8];
						Array.Copy(iv, 0, salt, 0, salt.Length);
					}
					break;
				case "BF":
					algorithm = "BLOWFISH";
					break;
				case "DES":
					algorithm = "DES";
					break;
				case "DES-EDE":
				case "DES-EDE3":
					algorithm = "DESede";
					break;
				case "RC2":
				case "RC2-40":
				case "RC2-64":
					algorithm = "RC2";
					break;
				default:
					throw new ArgumentException("Unknown DEK algorithm: " + dekAlgName, "dekAlgName");
			}

			string cipherName = algorithm + "/" + mode + "/" + padding;
			IBufferedCipher cipher = CipherUtilities.GetCipher(cipherName);

			ICipherParameters cParams = GetCipherParameters(password, baseAlg, salt);

			if (mode != "ECB")
			{
				cParams = new ParametersWithIV(cParams, iv);
			}

			cipher.Init(encrypt, cParams);

			return cipher.DoFinal(bytes);
		}

		private static ICipherParameters GetCipherParameters(
			char[]	password,
			string	baseAlg,
			byte[]	salt)
		{
			string algorithm;
			int keyBits;
			switch (baseAlg)
			{
				case "AES-128":		keyBits = 128;	algorithm = "AES128";	break;
				case "AES-192":		keyBits = 192;	algorithm = "AES192";	break;
				case "AES-256":		keyBits = 256;	algorithm = "AES256";	break;
				case "BF":			keyBits = 128;	algorithm = "BLOWFISH";	break;
				case "DES":			keyBits = 64;	algorithm = "DES";		break;
				case "DES-EDE":		keyBits = 128;	algorithm = "DESEDE";	break;
				case "DES-EDE3":	keyBits = 192;	algorithm = "DESEDE3";	break;
				case "RC2":			keyBits = 128;	algorithm = "RC2";		break;
				case "RC2-40":		keyBits = 40;	algorithm = "RC2";		break;
				case "RC2-64":		keyBits = 64;	algorithm = "RC2";		break;
				default:
					return null;
			}

			OpenSslPbeParametersGenerator pGen = new OpenSslPbeParametersGenerator();

			pGen.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password), salt);

			return pGen.GenerateDerivedParameters(algorithm, keyBits);
		}
	}
}
