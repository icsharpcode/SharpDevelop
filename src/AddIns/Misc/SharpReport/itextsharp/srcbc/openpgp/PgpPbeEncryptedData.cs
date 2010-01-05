using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>A password based encryption object.</remarks>
    public class PgpPbeEncryptedData
        : PgpEncryptedData
    {
        private readonly SymmetricKeyEncSessionPacket keyData;

		internal PgpPbeEncryptedData(
			SymmetricKeyEncSessionPacket	keyData,
			InputStreamPacket				encData)
			: base(encData)
		{
			this.keyData = keyData;
		}

		/// <summary>Return the raw input stream for the data stream.</summary>
		public override Stream GetInputStream()
		{
			return encData.GetInputStream();
		}

		/// <summary>Return the decrypted input stream, using the passed in passphrase.</summary>
        public Stream GetDataStream(
            char[] passPhrase)
        {
			try
			{
				SymmetricKeyAlgorithmTag keyAlgorithm = keyData.EncAlgorithm;

				KeyParameter key = PgpUtilities.MakeKeyFromPassPhrase(
					keyAlgorithm, keyData.S2k, passPhrase);


				byte[] secKeyData = keyData.GetSecKeyData();
				if (secKeyData != null && secKeyData.Length > 0)
				{
					IBufferedCipher keyCipher = CipherUtilities.GetCipher(
						PgpUtilities.GetSymmetricCipherName(keyAlgorithm) + "/CFB/NoPadding");

					keyCipher.Init(false,
						new ParametersWithIV(key, new byte[keyCipher.GetBlockSize()]));

					byte[] keyBytes = keyCipher.DoFinal(secKeyData);

					keyAlgorithm = (SymmetricKeyAlgorithmTag) keyBytes[0];

					key = ParameterUtilities.CreateKeyParameter(
						PgpUtilities.GetSymmetricCipherName(keyAlgorithm),
						keyBytes, 1, keyBytes.Length - 1);
				}


				IBufferedCipher c = CreateStreamCipher(keyAlgorithm);

				byte[] iv = new byte[c.GetBlockSize()];

				c.Init(false, new ParametersWithIV(key, iv));

				encStream = BcpgInputStream.Wrap(new CipherStream(encData.GetInputStream(), c, null));

				if (encData is SymmetricEncIntegrityPacket)
				{
					truncStream = new TruncatedStream(encStream);

					string digestName = PgpUtilities.GetDigestName(HashAlgorithmTag.Sha1);
					IDigest digest = DigestUtilities.GetDigest(digestName);

					encStream = new DigestStream(truncStream, digest, null);
				}

				if (Streams.ReadFully(encStream, iv, 0, iv.Length) < iv.Length)
					throw new EndOfStreamException("unexpected end of stream.");

				int v1 = encStream.ReadByte();
				int v2 = encStream.ReadByte();

				if (v1 < 0 || v2 < 0)
					throw new EndOfStreamException("unexpected end of stream.");


				// Note: the oracle attack on the "quick check" bytes is not deemed
				// a security risk for PBE (see PgpPublicKeyEncryptedData)

				bool repeatCheckPassed =
						iv[iv.Length - 2] == (byte)v1
					&&	iv[iv.Length - 1] == (byte)v2;

				// Note: some versions of PGP appear to produce 0 for the extra
				// bytes rather than repeating the two previous bytes
				bool zeroesCheckPassed =
						v1 == 0
					&&	v2 == 0;

				if (!repeatCheckPassed && !zeroesCheckPassed)
				{
					throw new PgpDataValidationException("quick check failed.");
				}


				return encStream;
			}
			catch (PgpException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new PgpException("Exception creating cipher", e);
			}
		}

		private IBufferedCipher CreateStreamCipher(
			SymmetricKeyAlgorithmTag keyAlgorithm)
		{
			string mode = (encData is SymmetricEncIntegrityPacket)
				? "CFB"
				: "OpenPGPCFB";

			string cName = PgpUtilities.GetSymmetricCipherName(keyAlgorithm)
				+ "/" + mode + "/NoPadding";

			return CipherUtilities.GetCipher(cName);
		}
	}
}
