using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
	/**
	* General class for generating a CMS enveloped-data message stream.
	* <p>
	* A simple example of usage.
	* <pre>
	*      CmsEnvelopedDataStreamGenerator edGen = new CmsEnvelopedDataStreamGenerator();
	*
	*      edGen.AddKeyTransRecipient(cert);
	*
	*      MemoryStream  bOut = new MemoryStream();
	*
	*      Stream out = edGen.Open(
	*                              bOut, CMSEnvelopedDataGenerator.AES128_CBC);*
	*      out.Write(data);
	*
	*      out.Close();
	* </pre>
	* </p>
	*/
	public class CmsEnvelopedDataStreamGenerator
		: CmsEnvelopedGenerator
	{
		private object	_originatorInfo = null;
		private object	_unprotectedAttributes = null;
		private int		_bufferSize;
		private bool	_berEncodeRecipientSet;

		public CmsEnvelopedDataStreamGenerator()
		{
		}

		/// <summary>Constructor allowing specific source of randomness</summary>
		/// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
		public CmsEnvelopedDataStreamGenerator(
			SecureRandom rand)
			: base(rand)
		{
		}

		/// <summary>Set the underlying string size for encapsulated data.</summary>
		/// <param name="bufferSize">Length of octet strings to buffer the data.</param>
		public void SetBufferSize(
			int bufferSize)
		{
			_bufferSize = bufferSize;
		}

		/// <summary>Use a BER Set to store the recipient information.</summary>
		public void SetBerEncodeRecipients(
			bool berEncodeRecipientSet)
		{
			_berEncodeRecipientSet = berEncodeRecipientSet;
		}

		private DerInteger Version
		{
			get
			{
				int version = (_originatorInfo != null || _unprotectedAttributes != null)
					?	2
					:	0;

				return new DerInteger(version);
			}
		}

		/// <summary>
		/// Generate an enveloped object that contains an CMS Enveloped Data
		/// object using the passed in key generator.
		/// </summary>
		private Stream Open(
			Stream				outStream,
			string				encryptionOid,
			CipherKeyGenerator	keyGen)
		{
			byte[] encKeyBytes = keyGen.GenerateKey();
			KeyParameter encKey = ParameterUtilities.CreateKeyParameter(encryptionOid, encKeyBytes);

			Asn1Encodable asn1Params = GenerateAsn1Parameters(encryptionOid, encKeyBytes);

			ICipherParameters cipherParameters;
			AlgorithmIdentifier encAlgID = GetAlgorithmIdentifier(
				encryptionOid, encKey, asn1Params, out cipherParameters);

			Asn1EncodableVector recipientInfos = new Asn1EncodableVector();

			foreach (RecipientInf recipient in recipientInfs)
			{
				try
				{
					recipientInfos.Add(recipient.ToRecipientInfo(encKey, rand));
				}
				catch (IOException e)
				{
					throw new CmsException("encoding error.", e);
				}
				catch (InvalidKeyException e)
				{
					throw new CmsException("key inappropriate for algorithm.", e);
				}
				catch (GeneralSecurityException e)
				{
					throw new CmsException("error making encrypted content.", e);
				}
			}

			return Open(outStream, encAlgID, cipherParameters, recipientInfos);
		}

		private Stream Open(
			Stream				outStream,
			AlgorithmIdentifier	encAlgID,
			ICipherParameters	cipherParameters,
			Asn1EncodableVector	recipientInfos)
		{
			try
			{
				//
				// ContentInfo
				//
				BerSequenceGenerator cGen = new BerSequenceGenerator(outStream);

				cGen.AddObject(CmsObjectIdentifiers.EnvelopedData);

				//
				// Encrypted Data
				//
				BerSequenceGenerator envGen = new BerSequenceGenerator(
					cGen.GetRawOutputStream(), 0, true);

				envGen.AddObject(this.Version);

				Asn1Generator recipGen = _berEncodeRecipientSet
					?	(Asn1Generator) new BerSetGenerator(envGen.GetRawOutputStream())
					:	new DerSetGenerator(envGen.GetRawOutputStream());

				foreach (Asn1Encodable ae in recipientInfos)
				{
					recipGen.AddObject(ae);
				}

				recipGen.Close();

				BerSequenceGenerator eiGen = new BerSequenceGenerator(
					envGen.GetRawOutputStream());

				eiGen.AddObject(PkcsObjectIdentifiers.Data);
				eiGen.AddObject(encAlgID);

				BerOctetStringGenerator octGen = new BerOctetStringGenerator(
					eiGen.GetRawOutputStream(), 0, false);
				Stream octetOutputStream = octGen.GetOctetOutputStream(_bufferSize);

				IBufferedCipher cipher = CipherUtilities.GetCipher(encAlgID.ObjectID);
				cipher.Init(true, new ParametersWithRandom(cipherParameters, rand));
				CipherStream cOut = new CipherStream(octetOutputStream, null, cipher);

				return new CmsEnvelopedDataOutputStream(cOut, cGen, envGen, eiGen);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e)
			{
				throw new CmsException("key invalid in message.", e);
			}
			catch (IOException e)
			{
				throw new CmsException("exception decoding algorithm parameters.", e);
			}
		}

		/**
		* generate an enveloped object that contains an CMS Enveloped Data object
		* @throws IOException
		*/
		public Stream Open(
			Stream	outStream,
			string	encryptionOid)
		{
			CipherKeyGenerator keyGen = GeneratorUtilities.GetKeyGenerator(encryptionOid);

			keyGen.Init(new KeyGenerationParameters(rand, keyGen.DefaultStrength));

			return Open(outStream, encryptionOid, keyGen);
		}

		/**
		* generate an enveloped object that contains an CMS Enveloped Data object
		* @throws IOException
		*/
		public Stream Open(
			Stream	outStream,
			string	encryptionOid,
			int		keySize)
		{
			CipherKeyGenerator keyGen = GeneratorUtilities.GetKeyGenerator(encryptionOid);

			keyGen.Init(new KeyGenerationParameters(rand, keySize));

			return Open(outStream, encryptionOid, keyGen);
		}

		private class CmsEnvelopedDataOutputStream
			: BaseOutputStream
		{
			private CipherStream			_out;
			private BerSequenceGenerator	_cGen;
			private BerSequenceGenerator	_envGen;
			private BerSequenceGenerator	_eiGen;

			public CmsEnvelopedDataOutputStream(
				CipherStream			outStream,
				BerSequenceGenerator	cGen,
				BerSequenceGenerator	envGen,
				BerSequenceGenerator	eiGen)
			{
				_out = outStream;
				_cGen = cGen;
				_envGen = envGen;
				_eiGen = eiGen;
			}

			public override void WriteByte(
				byte b)
			{
				_out.WriteByte(b);
			}

			public override void Write(
				byte[]	bytes,
				int		off,
				int		len)
			{
				_out.Write(bytes, off, len);
			}

			public override void Close()
			{
				_out.Close();
				_eiGen.Close();

				// [TODO] unprotected attributes go here

				_envGen.Close();
				_cGen.Close();
				base.Close();
			}
		}
	}
}
