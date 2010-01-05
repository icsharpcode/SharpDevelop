using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms
{
	/**
	* Parsing class for an CMS Signed Data object from an input stream.
	* <p>
	* Note: that because we are in a streaming mode only one signer can be tried and it is important
	* that the methods on the parser are called in the appropriate order.
	* </p>
	* <p>
	* A simple example of usage for an encapsulated signature.
	* </p>
	* <p>
	* Two notes: first, in the example below the validity of
	* the certificate isn't verified, just the fact that one of the certs
	* matches the given signer, and, second, because we are in a streaming
	* mode the order of the operations is important.
	* </p>
	* <pre>
	*      CmsSignedDataParser     sp = new CmsSignedDataParser(encapSigData);
	*
	*      sp.GetSignedContent().Drain();
	*
	*      IX509Store              certs = sp.GetCertificates();
	*      SignerInformationStore  signers = sp.GetSignerInfos();
	*
	*      foreach (SignerInformation signer in signers.GetSigners())
	*      {
	*          ArrayList       certList = new ArrayList(certs.GetMatches(signer.SignerID));
	*          X509Certificate cert = (X509Certificate) certList[0];
	*
	*          Console.WriteLine("verify returns: " + signer.Verify(cert));
	*      }
	* </pre>
	*  Note also: this class does not introduce buffering - if you are processing large files you should create
	*  the parser with:
	*  <pre>
	*          CmsSignedDataParser     ep = new CmsSignedDataParser(new BufferedInputStream(encapSigData, bufSize));
	*  </pre>
	*  where bufSize is a suitably large buffer size.
	*/
	public class CmsSignedDataParser
		: CmsContentInfoParser
	{
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private SignedDataParser        _signedData;
		private CmsTypedStream          _signedContent;
		private IDictionary _digests;

		private SignerInformationStore  _signerInfoStore;
		private Asn1Set                 _certSet, _crlSet;
		private bool					_isCertCrlParsed;
		private IX509Store				_attributeStore;
		private IX509Store				_certificateStore;
		private IX509Store				_crlStore;

		public CmsSignedDataParser(
			byte[] sigBlock)
			: this(new MemoryStream(sigBlock, false))
		{
		}

		public CmsSignedDataParser(
			CmsTypedStream	signedContent,
			byte[]			sigBlock)
			: this(signedContent, new MemoryStream(sigBlock, false))
		{
		}

		/**
		* base constructor - with encapsulated content
		*/
		public CmsSignedDataParser(
			Stream sigData)
			: this(null, sigData)
		{
		}

		/**
		* base constructor
		*
		* @param signedContent the content that was signed.
		* @param sigData the signature object.
		*/
		public CmsSignedDataParser(
			CmsTypedStream	signedContent,
			Stream			sigData)
			: base(sigData)
		{
			try
			{
				this._signedContent = signedContent;
				this._signedData = SignedDataParser.GetInstance(this.contentInfo.GetContent(Asn1Tags.Sequence));
				this._digests = new Hashtable();

				Asn1SetParser digAlgs = _signedData.GetDigestAlgorithms();
				IAsn1Convertible o;

				while ((o = digAlgs.ReadObject()) != null)
				{
					AlgorithmIdentifier id = AlgorithmIdentifier.GetInstance(o.ToAsn1Object());

					try
					{
						string digestName = Helper.GetDigestAlgName(id.ObjectID.Id);
						IDigest dig = Helper.GetDigestInstance(digestName);

						this._digests[digestName] = dig;
					}
					catch (SecurityUtilityException)
					{
						//  ignore
					}
				}

				//
				// If the message is simply a certificate chain message GetContent() may return null.
				//
				ContentInfoParser cont = _signedData.GetEncapContentInfo();
				Asn1OctetStringParser octs = (Asn1OctetStringParser)
					cont.GetContent(Asn1Tags.OctetString);

				if (octs != null)
				{
					CmsTypedStream ctStr = new CmsTypedStream(
						cont.ContentType.Id, octs.GetOctetStream());

					if (_signedContent == null)
					{
						this._signedContent = ctStr;
					}
					else
					{
						//
						// content passed in, need to read past empty encapsulated content info object if present
						//
						ctStr.Drain();
					}
				}
			}
			catch (IOException e)
			{
				throw new CmsException("io exception: " + e.Message, e);
			}

			if (_digests.Count < 1)
			{
				throw new CmsException("no digests could be created for message.");
			}
		}

		/**
		 * Return the version number for the SignedData object
		 *
		 * @return the version number
		 */
		public int Version
		{
			get { return _signedData.Version.Value.IntValue; }
		}

		/**
		* return the collection of signers that are associated with the
		* signatures for the message.
		* @throws CmsException
		*/
		public SignerInformationStore GetSignerInfos()
		{
			if (_signerInfoStore == null)
			{
				PopulateCertCrlSets();

				IList signerInfos = new ArrayList();
				IDictionary	hashes = new Hashtable();

				foreach (object digestKey in _digests.Keys)
				{
					hashes[digestKey] = DigestUtilities.DoFinal(
						(IDigest)_digests[digestKey]);
				}

				try
				{
					Asn1SetParser s = _signedData.GetSignerInfos();
					IAsn1Convertible o;

					while ((o = s.ReadObject()) != null)
					{
						SignerInfo info = SignerInfo.GetInstance(o.ToAsn1Object());
						string digestName = Helper.GetDigestAlgName(
							info.DigestAlgorithm.ObjectID.Id);

						byte[] hash = (byte[]) hashes[digestName];
						DerObjectIdentifier oid = new DerObjectIdentifier(_signedContent.ContentType);

						signerInfos.Add(new SignerInformation(info, oid, null, new BaseDigestCalculator(hash)));
					}
				}
				catch (IOException e)
				{
					throw new CmsException("io exception: " + e.Message, e);
				}

				_signerInfoStore = new SignerInformationStore(signerInfos);
			}

			return _signerInfoStore;
		}

		/**
		 * return a X509Store containing the attribute certificates, if any, contained
		 * in this message.
		 *
		 * @param type type of store to create
		 * @return a store of attribute certificates
		 * @exception org.bouncycastle.x509.NoSuchStoreException if the store type isn't available.
		 * @exception CmsException if a general exception prevents creation of the X509Store
		 */
		public IX509Store GetAttributeCertificates(
			string type)
		{
			if (_attributeStore == null)
			{
				PopulateCertCrlSets();

				_attributeStore = Helper.CreateAttributeStore(type, _certSet);
			}

			return _attributeStore;
		}

		/**
		* return a X509Store containing the public key certificates, if any, contained
		* in this message.
		*
		* @param type type of store to create
		* @return a store of public key certificates
		* @exception NoSuchStoreException if the store type isn't available.
		* @exception CmsException if a general exception prevents creation of the X509Store
		*/
		public IX509Store GetCertificates(
			string type)
		{
			if (_certificateStore == null)
			{
				PopulateCertCrlSets();

				_certificateStore = Helper.CreateCertificateStore(type, _certSet);
			}

			return _certificateStore;
		}

		/**
		* return a X509Store containing CRLs, if any, contained
		* in this message.
		*
		* @param type type of store to create
		* @return a store of CRLs
		* @exception NoSuchStoreException if the store type isn't available.
		* @exception CmsException if a general exception prevents creation of the X509Store
		*/
		public IX509Store GetCrls(
			string type)
		{
			if (_crlStore == null)
			{
				PopulateCertCrlSets();

				_crlStore = Helper.CreateCrlStore(type, _crlSet);
			}

			return _crlStore;
		}

		private void PopulateCertCrlSets()
		{
			if (_isCertCrlParsed)
				return;

			_isCertCrlParsed = true;

			try
			{
				// care! Streaming - Must process the GetCertificates() result before calling GetCrls()
				_certSet = GetAsn1Set(_signedData.GetCertificates());
				_crlSet = GetAsn1Set(_signedData.GetCrls());
			}
			catch (IOException e)
			{
				throw new CmsException("problem parsing cert/crl sets", e);
			}
		}

		public CmsTypedStream GetSignedContent()
		{
			if (_signedContent == null)
			{
				return null;
			}

			Stream digStream = _signedContent.ContentStream;

			foreach (IDigest digest in _digests.Values)
			{
				digStream = new DigestStream(digStream, digest, null);
			}

			return new CmsTypedStream(_signedContent.ContentType, digStream);
		}

		/**
		 * Replace the signerinformation store associated with the passed
		 * in message contained in the stream original with the new one passed in.
		 * You would probably only want to do this if you wanted to change the unsigned
		 * attributes associated with a signer, or perhaps delete one.
		 * <p>
		 * The output stream is returned unclosed.
		 * </p>
		 * @param original the signed data stream to be used as a base.
		 * @param signerInformationStore the new signer information store to use.
		 * @param out the stream to Write the new signed data object to.
		 * @return out.
		 */
		public static Stream ReplaceSigners(
			Stream					original,
			SignerInformationStore	signerInformationStore,
			Stream					outStr)
		{
			Asn1StreamParser inStr = new Asn1StreamParser(original, CmsUtilities.MaximumMemory);
			ContentInfoParser contentInfo = new ContentInfoParser((Asn1SequenceParser)inStr.ReadObject());
			SignedDataParser signedData = SignedDataParser.GetInstance(contentInfo.GetContent(Asn1Tags.Sequence));

			BerSequenceGenerator sGen = new BerSequenceGenerator(outStr);

			sGen.AddObject(CmsObjectIdentifiers.SignedData);

			BerSequenceGenerator sigGen = new BerSequenceGenerator(sGen.GetRawOutputStream(), 0, true);

			// version number
			sigGen.AddObject(signedData.Version);

			// digests
			signedData.GetDigestAlgorithms().ToAsn1Object();  // skip old ones

			Asn1EncodableVector digestAlgs = new Asn1EncodableVector();

			foreach (SignerInformation signer in signerInformationStore.GetSigners())
			{
				digestAlgs.Add(FixAlgID(signer.DigestAlgorithmID));
			}

			WriteToGenerator(sigGen, new DerSet(digestAlgs));

			// encap content info
			ContentInfoParser encapContentInfo = signedData.GetEncapContentInfo();

			BerSequenceGenerator eiGen = new BerSequenceGenerator(sigGen.GetRawOutputStream());

			eiGen.AddObject(encapContentInfo.ContentType);

			Asn1OctetStringParser octs = (Asn1OctetStringParser)encapContentInfo.GetContent(Asn1Tags.OctetString);

			if (octs != null)
			{
				PipeOctetString(octs, eiGen.GetRawOutputStream());
			}

			eiGen.Close();


			WriteSetToGeneratorTagged(sigGen, signedData.GetCertificates(), 0);
			WriteSetToGeneratorTagged(sigGen, signedData.GetCrls(), 1);


			Asn1EncodableVector signerInfos = new Asn1EncodableVector();
			foreach (SignerInformation signer in signerInformationStore.GetSigners())
			{
				signerInfos.Add(signer.ToSignerInfo());
			}

			WriteToGenerator(sigGen, new DerSet(signerInfos));

			sigGen.Close();

			sGen.Close();

			return outStr;
		}

		/**
		 * Replace the certificate and CRL information associated with this
		 * CMSSignedData object with the new one passed in.
		 * <p>
		 * The output stream is returned unclosed.
		 * </p>
		 * @param original the signed data stream to be used as a base.
		 * @param certsAndCrls the new certificates and CRLs to be used.
		 * @param out the stream to Write the new signed data object to.
		 * @return out.
		 * @exception CmsException if there is an error processing the CertStore
		 */
		public static Stream ReplaceCertificatesAndCrls(
			Stream			original,
			IX509Store		x509Certs,
			IX509Store		x509Crls,
			IX509Store		x509AttrCerts,
			Stream			outStr)
		{
			if (x509AttrCerts != null)
				throw Platform.CreateNotImplementedException("Currently can't replace attribute certificates");

			Asn1StreamParser inStr = new Asn1StreamParser(original, CmsUtilities.MaximumMemory);
			ContentInfoParser contentInfo = new ContentInfoParser((Asn1SequenceParser)inStr.ReadObject());
			SignedDataParser signedData = SignedDataParser.GetInstance(contentInfo.GetContent(Asn1Tags.Sequence));

			BerSequenceGenerator sGen = new BerSequenceGenerator(outStr);

			sGen.AddObject(CmsObjectIdentifiers.SignedData);

			BerSequenceGenerator sigGen = new BerSequenceGenerator(sGen.GetRawOutputStream(), 0, true);

			// version number
			sigGen.AddObject(signedData.Version);

			// digests
			WriteToGenerator(sigGen, signedData.GetDigestAlgorithms().ToAsn1Object());

			// encap content info
			ContentInfoParser encapContentInfo = signedData.GetEncapContentInfo();

			BerSequenceGenerator eiGen = new BerSequenceGenerator(sigGen.GetRawOutputStream());

			eiGen.AddObject(encapContentInfo.ContentType);

			Asn1OctetStringParser octs = (Asn1OctetStringParser)
				encapContentInfo.GetContent(Asn1Tags.OctetString);

			if (octs != null)
			{
				PipeOctetString(octs, eiGen.GetRawOutputStream());
			}

			eiGen.Close();

			//
			// skip existing certs and CRLs
			//
			GetAsn1Set(signedData.GetCertificates());
			GetAsn1Set(signedData.GetCrls());

			//
			// replace the certs and crls in the SignedData object
			//
			Asn1Set certs;
			try
			{
				certs = CmsUtilities.CreateBerSetFromList(
					CmsUtilities.GetCertificatesFromStore(x509Certs));
			}
			catch (X509StoreException e)
			{
				throw new CmsException("error getting certs from certStore", e);
			}

			if (certs.Count > 0)
			{
				WriteToGenerator(sigGen, new DerTaggedObject(false, 0, certs));
			}

			Asn1Set crls;
			try
			{
				crls = CmsUtilities.CreateBerSetFromList(
					CmsUtilities.GetCrlsFromStore(x509Crls));
			}
			catch (X509StoreException e)
			{
				throw new CmsException("error getting crls from certStore", e);
			}

			if (crls.Count > 0)
			{
				WriteToGenerator(sigGen, new DerTaggedObject(false, 1, crls));
			}

			WriteToGenerator(sigGen, signedData.GetSignerInfos().ToAsn1Object());

			sigGen.Close();

			sGen.Close();

			return outStr;
		}

		private static AlgorithmIdentifier FixAlgID(
			AlgorithmIdentifier algId)
		{
			if (algId.Parameters == null)
				return new AlgorithmIdentifier(algId.ObjectID, DerNull.Instance);

			return algId;
		}

		private static void WriteSetToGeneratorTagged(
			Asn1Generator		asn1Gen,
			Asn1SetParser		asn1SetParser,
			int					tagNo)
		{
			Asn1Set asn1Set = GetAsn1Set(asn1SetParser);

			if (asn1Set != null)
			{
				Asn1TaggedObject taggedObj = (asn1SetParser is BerSetParser)
					?	new BerTaggedObject(false, tagNo, asn1Set)
					:	new DerTaggedObject(false, tagNo, asn1Set);

				WriteToGenerator(asn1Gen, taggedObj);
			}
		}

		private static Asn1Set GetAsn1Set(
			Asn1SetParser asn1SetParser)
		{
			return asn1SetParser == null
				?	null
				:	Asn1Set.GetInstance(asn1SetParser.ToAsn1Object());
		}

		private static void WriteToGenerator(
			Asn1Generator	ag,
			Asn1Encodable	ae)
		{
			byte[] encoded = ae.GetEncoded();
			ag.GetRawOutputStream().Write(encoded, 0, encoded.Length);
		}

		private static void PipeOctetString(
			Asn1OctetStringParser	octs,
			Stream					output)
		{
			BerOctetStringGenerator octGen = new BerOctetStringGenerator(output, 0, true);
			// TODO Allow specification of a specific fragment size?
			Stream outOctets = octGen.GetOctetOutputStream();
			Streams.PipeAll(octs.GetOctetStream(), outOctets);
			outOctets.Close();
		}
	}
}
