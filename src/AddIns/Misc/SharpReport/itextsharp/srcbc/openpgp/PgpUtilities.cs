using System;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>Basic utility class.</remarks>
    public sealed class PgpUtilities
    {
        private PgpUtilities()
        {
        }

		public static MPInteger[] DsaSigToMpi(
			byte[] encoding)
		{
			DerInteger i1, i2;

			try
			{
				Asn1Sequence s = (Asn1Sequence) Asn1Object.FromByteArray(encoding);

				i1 = (DerInteger) s[0];
				i2 = (DerInteger) s[1];
			}
			catch (IOException e)
			{
				throw new PgpException("exception encoding signature", e);
			}

			return new MPInteger[]{ new MPInteger(i1.Value), new MPInteger(i2.Value) };
		}

		public static MPInteger[] RsaSigToMpi(
			byte[] encoding)
		{
			return new MPInteger[]{ new MPInteger(new BigInteger(1, encoding)) };
		}

		public static string GetDigestName(
            HashAlgorithmTag hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
				case HashAlgorithmTag.Sha1:
					return "SHA1";
				case HashAlgorithmTag.MD2:
					return "MD2";
				case HashAlgorithmTag.MD5:
					return "MD5";
				case HashAlgorithmTag.RipeMD160:
					return "RIPEMD160";
				case HashAlgorithmTag.Sha224:
					return "SHA224";
				case HashAlgorithmTag.Sha256:
					return "SHA256";
				case HashAlgorithmTag.Sha384:
					return "SHA384";
				case HashAlgorithmTag.Sha512:
					return "SHA512";
				default:
					throw new PgpException("unknown hash algorithm tag in GetDigestName: " + hashAlgorithm);
			}
        }

		public static string GetSignatureName(
            PublicKeyAlgorithmTag	keyAlgorithm,
            HashAlgorithmTag		hashAlgorithm)
        {
            string encAlg;
			switch (keyAlgorithm)
            {
				case PublicKeyAlgorithmTag.RsaGeneral:
				case PublicKeyAlgorithmTag.RsaSign:
					encAlg = "RSA";
					break;
				case PublicKeyAlgorithmTag.Dsa:
					encAlg = "DSA";
					break;
				case PublicKeyAlgorithmTag.ElGamalEncrypt: // in some malformed cases.
				case PublicKeyAlgorithmTag.ElGamalGeneral:
					encAlg = "ElGamal";
					break;
				default:
					throw new PgpException("unknown algorithm tag in signature:" + keyAlgorithm);
            }

			return GetDigestName(hashAlgorithm) + "with" + encAlg;
        }

		public static string GetSymmetricCipherName(
            SymmetricKeyAlgorithmTag algorithm)
        {
            switch (algorithm)
            {
				case SymmetricKeyAlgorithmTag.Null:
					return null;
				case SymmetricKeyAlgorithmTag.TripleDes:
					return "DESEDE";
				case SymmetricKeyAlgorithmTag.Idea:
					return "IDEA";
				case SymmetricKeyAlgorithmTag.Cast5:
					return "CAST5";
				case SymmetricKeyAlgorithmTag.Blowfish:
					return "Blowfish";
				case SymmetricKeyAlgorithmTag.Safer:
					return "SAFER";
				case SymmetricKeyAlgorithmTag.Des:
					return "DES";
				case SymmetricKeyAlgorithmTag.Aes128:
					return "AES";
				case SymmetricKeyAlgorithmTag.Aes192:
					return "AES";
				case SymmetricKeyAlgorithmTag.Aes256:
					return "AES";
				case SymmetricKeyAlgorithmTag.Twofish:
					return "Twofish";
				default:
					throw new PgpException("unknown symmetric algorithm: " + algorithm);
            }
        }

		public static int GetKeySize(SymmetricKeyAlgorithmTag algorithm)
        {
            int keySize;
            switch (algorithm)
            {
                case SymmetricKeyAlgorithmTag.Des:
                    keySize = 64;
                    break;
                case SymmetricKeyAlgorithmTag.Idea:
                case SymmetricKeyAlgorithmTag.Cast5:
                case SymmetricKeyAlgorithmTag.Blowfish:
                case SymmetricKeyAlgorithmTag.Safer:
                case SymmetricKeyAlgorithmTag.Aes128:
                    keySize = 128;
                    break;
                case SymmetricKeyAlgorithmTag.TripleDes:
                case SymmetricKeyAlgorithmTag.Aes192:
                    keySize = 192;
                    break;
                case SymmetricKeyAlgorithmTag.Aes256:
                case SymmetricKeyAlgorithmTag.Twofish:
                    keySize = 256;
                    break;
                default:
                    throw new PgpException("unknown symmetric algorithm: " + algorithm);
            }

			return keySize;
        }

		public static KeyParameter MakeKey(
			SymmetricKeyAlgorithmTag	algorithm,
			byte[]						keyBytes)
		{
			string algName = GetSymmetricCipherName(algorithm);

			return ParameterUtilities.CreateKeyParameter(algName, keyBytes);
		}

		public static KeyParameter MakeRandomKey(
            SymmetricKeyAlgorithmTag	algorithm,
            SecureRandom				random)
        {
            int keySize = GetKeySize(algorithm);
            byte[] keyBytes = new byte[(keySize + 7) / 8];
            random.NextBytes(keyBytes);
			return MakeKey(algorithm, keyBytes);
        }

		public static KeyParameter MakeKeyFromPassPhrase(
            SymmetricKeyAlgorithmTag	algorithm,
            S2k							s2k,
            char[]						passPhrase)
        {
			int keySize = GetKeySize(algorithm);
			byte[] pBytes = Strings.ToByteArray(new string(passPhrase));
			byte[] keyBytes = new byte[(keySize + 7) / 8];

			int generatedBytes = 0;
            int loopCount = 0;

			while (generatedBytes < keyBytes.Length)
            {
				IDigest digest;
				if (s2k != null)
                {
                    try
                    {
                        switch (s2k.HashAlgorithm)
                        {
							case HashAlgorithmTag.Sha1:
								digest = DigestUtilities.GetDigest("SHA1");
								break;
							default:
								throw new PgpException("unknown hash algorithm: " + s2k.HashAlgorithm);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new PgpException("can't find S2k digest", e);
                    }

					for (int i = 0; i != loopCount; i++)
                    {
                        digest.Update(0);
                    }

					byte[] iv = s2k.GetIV();

					switch (s2k.Type)
                    {
						case S2k.Simple:
							digest.BlockUpdate(pBytes, 0, pBytes.Length);
							break;
						case S2k.Salted:
							digest.BlockUpdate(iv, 0, iv.Length);
							digest.BlockUpdate(pBytes, 0, pBytes.Length);
							break;
						case S2k.SaltedAndIterated:
							long count = s2k.IterationCount;
							digest.BlockUpdate(iv, 0, iv.Length);
							digest.BlockUpdate(pBytes, 0, pBytes.Length);

							count -= iv.Length + pBytes.Length;

							while (count > 0)
							{
								if (count < iv.Length)
								{
									digest.BlockUpdate(iv, 0, (int)count);
									break;
								}
								else
								{
									digest.BlockUpdate(iv, 0, iv.Length);
									count -= iv.Length;
								}

								if (count < pBytes.Length)
								{
									digest.BlockUpdate(pBytes, 0, (int)count);
									count = 0;
								}
								else
								{
									digest.BlockUpdate(pBytes, 0, pBytes.Length);
									count -= pBytes.Length;
								}
							}
							break;
						default:
							throw new PgpException("unknown S2k type: " + s2k.Type);
                    }
                }
                else
                {
                    try
                    {
                        digest = DigestUtilities.GetDigest("MD5");

						for (int i = 0; i != loopCount; i++)
                        {
                            digest.Update(0);
                        }

						digest.BlockUpdate(pBytes, 0, pBytes.Length);
                    }
                    catch (Exception e)
                    {
                        throw new PgpException("can't find MD5 digest", e);
                    }
                }

				byte[] dig = DigestUtilities.DoFinal(digest);

				if (dig.Length > (keyBytes.Length - generatedBytes))
                {
                    Array.Copy(dig, 0, keyBytes, generatedBytes, keyBytes.Length - generatedBytes);
                }
                else
                {
                    Array.Copy(dig, 0, keyBytes, generatedBytes, dig.Length);
                }

				generatedBytes += dig.Length;

				loopCount++;
            }

			Array.Clear(pBytes, 0, pBytes.Length);

			return MakeKey(algorithm, keyBytes);
        }

		/// <summary>Write out the passed in file as a literal data packet.</summary>
        public static void WriteFileToLiteralData(
            Stream		outputStream,
            char		fileType,
            FileInfo	file)
        {
			Stream inStr = file.OpenRead();
			Stream outStr = new PgpLiteralDataGenerator().Open(
				outputStream, fileType, file.Name, file.Length, file.LastWriteTime);

			Streams.PipeAll(inStr, outStr);

			inStr.Close();
			outStr.Close();
        }

		/// <summary>Write out the passed in file as a literal data packet in partial packet format.</summary>
        public static void WriteFileToLiteralData(
            Stream		outputStream,
            char		fileType,
            FileInfo	file,
            byte[]		buffer)
        {
            PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
            Stream pOut = lData.Open(outputStream, fileType, file.Name, file.LastWriteTime, buffer);
            FileStream inputStream = file.OpenRead();
            byte[] buf = new byte[buffer.Length];

			int len;
            while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
            {
                pOut.Write(buf, 0, len);
            }

			lData.Close();
            inputStream.Close();
        }

		private const int ReadAhead = 60;

		private static bool IsPossiblyBase64(
            int ch)
        {
            return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z')
                    || (ch >= '0' && ch <= '9') || (ch == '+') || (ch == '/')
                    || (ch == '\r') || (ch == '\n');
        }

		/// <summary>
		/// Return either an ArmoredInputStream or a BcpgInputStream based on whether
		/// the initial characters of the stream are binary PGP encodings or not.
		/// </summary>
        public static Stream GetDecoderStream(
            Stream inputStream)
        {
			// TODO Remove this restriction?
			if (!inputStream.CanSeek)
				throw new ArgumentException("inputStream must be seek-able", "inputStream");

			long markedPos = inputStream.Position;

			int ch = inputStream.ReadByte();
            if ((ch & 0x80) != 0)
            {
                inputStream.Position = markedPos;

				return inputStream;
            }
            else
            {
                if (!IsPossiblyBase64(ch))
                {
                    inputStream.Position = markedPos;

					return new ArmoredInputStream(inputStream);
                }

				byte[]	buf = new byte[ReadAhead];
                int		count = 1;
                int		index = 1;

				buf[0] = (byte)ch;
                while (count != ReadAhead && (ch = inputStream.ReadByte()) >= 0)
                {
                    if (!IsPossiblyBase64(ch))
                    {
                        inputStream.Position = markedPos;

						return new ArmoredInputStream(inputStream);
                    }

					if (ch != '\n' && ch != '\r')
                    {
                        buf[index++] = (byte)ch;
                    }

					count++;
                }

				inputStream.Position = markedPos;

				//
                // nothing but new lines, little else, assume regular armoring
                //
                if (count < 4)
                {
                    return new ArmoredInputStream(inputStream);
                }

				//
                // test our non-blank data
                //
                byte[] firstBlock = new byte[8];
				Array.Copy(buf, 0, firstBlock, 0, firstBlock.Length);
				byte[] decoded = Base64.Decode(firstBlock);

				//
                // it's a base64 PGP block.
                //
				bool hasHeaders = (decoded[0] & 0x80) == 0;

				return new ArmoredInputStream(inputStream, hasHeaders);
            }
        }
    }
}
