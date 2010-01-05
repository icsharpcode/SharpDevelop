using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Ocsp
{
	/**
	 * Generator for basic OCSP response objects.
	 */
	public class BasicOcspRespGenerator
	{
		private readonly IList list = new ArrayList();

		private X509Extensions responseExtensions;
		private RespID responderID;

		private class ResponseObject
		{
			internal CertificateID         certId;
			internal CertStatus            certStatus;
			internal DerGeneralizedTime    thisUpdate;
			internal DerGeneralizedTime    nextUpdate;
			internal X509Extensions        extensions;

			public ResponseObject(
				CertificateID		certId,
				CertificateStatus	certStatus,
				DateTime			thisUpdate,
				X509Extensions		extensions)
				: this(certId, certStatus, new DerGeneralizedTime(thisUpdate), null, extensions)
			{
			}

			public ResponseObject(
				CertificateID		certId,
				CertificateStatus	certStatus,
				DateTime			thisUpdate,
				DateTime			nextUpdate,
				X509Extensions		extensions)
				: this(certId, certStatus, new DerGeneralizedTime(thisUpdate), new DerGeneralizedTime(nextUpdate), extensions)
			{
			}

			private ResponseObject(
				CertificateID		certId,
				CertificateStatus	certStatus,
				DerGeneralizedTime	thisUpdate,
				DerGeneralizedTime	nextUpdate,
				X509Extensions		extensions)
			{
				this.certId = certId;

				if (certStatus == null)
				{
					this.certStatus = new CertStatus();
				}
				else if (certStatus is UnknownStatus)
				{
					this.certStatus = new CertStatus(2, DerNull.Instance);
				}
				else
				{
					RevokedStatus rs = (RevokedStatus) certStatus;
					CrlReason revocationReason = rs.HasRevocationReason
						?	new CrlReason(rs.RevocationReason)
						:	null;

					this.certStatus = new CertStatus(
						new RevokedInfo(new DerGeneralizedTime(rs.RevocationTime), revocationReason));
				}

				this.thisUpdate = thisUpdate;
				this.nextUpdate = nextUpdate;

				this.extensions = extensions;
			}

			public SingleResponse ToResponse()
			{
				return new SingleResponse(certId.ToAsn1Object(), certStatus, thisUpdate, nextUpdate, extensions);
			}
		}

		/**
		 * basic constructor
		 */
		public BasicOcspRespGenerator(
			RespID responderID)
		{
			this.responderID = responderID;
		}

		/**
		 * construct with the responderID to be the SHA-1 keyHash of the passed in public key.
		 */
		public BasicOcspRespGenerator(
			AsymmetricKeyParameter publicKey)
		{
			this.responderID = new RespID(publicKey);
		}

		/**
		 * Add a response for a particular Certificate ID.
		 *
		 * @param certID certificate ID details
		 * @param certStatus status of the certificate - null if okay
		 */
		public void AddResponse(
			CertificateID		certID,
			CertificateStatus	certStatus)
		{
			list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, null));
		}

		/**
		 * Add a response for a particular Certificate ID.
		 *
		 * @param certID certificate ID details
		 * @param certStatus status of the certificate - null if okay
		 * @param singleExtensions optional extensions
		 */
		public void AddResponse(
			CertificateID		certID,
			CertificateStatus	certStatus,
			X509Extensions		singleExtensions)
		{
			list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, singleExtensions));
		}

		/**
		 * Add a response for a particular Certificate ID.
		 *
		 * @param certID certificate ID details
		 * @param nextUpdate date when next update should be requested
		 * @param certStatus status of the certificate - null if okay
		 * @param singleExtensions optional extensions
		 */
		public void AddResponse(
			CertificateID		certID,
			CertificateStatus	certStatus,
			DateTime			nextUpdate,
			X509Extensions		singleExtensions)
		{
			list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, nextUpdate, singleExtensions));
		}

		/**
		 * Add a response for a particular Certificate ID.
		 *
		 * @param certID certificate ID details
		 * @param thisUpdate date this response was valid on
		 * @param nextUpdate date when next update should be requested
		 * @param certStatus status of the certificate - null if okay
		 * @param singleExtensions optional extensions
		 */
		public void AddResponse(
			CertificateID		certID,
			CertificateStatus	certStatus,
			DateTime			thisUpdate,
			DateTime			nextUpdate,
			X509Extensions		singleExtensions)
		{
			list.Add(new ResponseObject(certID, certStatus, thisUpdate, nextUpdate, singleExtensions));
		}

		/**
		 * Set the extensions for the response.
		 *
		 * @param responseExtensions the extension object to carry.
		 */
		public void SetResponseExtensions(
			X509Extensions responseExtensions)
		{
			this.responseExtensions = responseExtensions;
		}

		private BasicOcspResp GenerateResponse(
			string					signatureName,
			AsymmetricKeyParameter	privateKey,
			X509Certificate[]		chain,
			DateTime				producedAt,
			SecureRandom			random)
		{
			DerObjectIdentifier signingAlgorithm;
			try
			{
				signingAlgorithm = OcspUtilities.GetAlgorithmOid(signatureName);
			}
			catch (Exception e)
			{
				throw new ArgumentException("unknown signing algorithm specified", e);
			}

			Asn1EncodableVector responses = new Asn1EncodableVector();

			foreach (ResponseObject respObj in list)
			{
				try
				{
					responses.Add(respObj.ToResponse());
				}
				catch (Exception e)
				{
					throw new OcspException("exception creating Request", e);
				}
			}

			ResponseData tbsResp = new ResponseData(responderID.ToAsn1Object(), new DerGeneralizedTime(producedAt), new DerSequence(responses), responseExtensions);

			ISigner sig = null;

			try
			{
				sig = SignerUtilities.GetSigner(signatureName);

				if (random != null)
				{
					sig.Init(true, new ParametersWithRandom(privateKey, random));
				}
				else
				{
					sig.Init(true, privateKey);
				}
			}
			catch (Exception e)
			{
				throw new OcspException("exception creating signature: " + e, e);
			}

			DerBitString bitSig = null;

			try
			{
				byte[] encoded = tbsResp.GetDerEncoded();
				sig.BlockUpdate(encoded, 0, encoded.Length);

				bitSig = new DerBitString(sig.GenerateSignature());
			}
			catch (Exception e)
			{
				throw new OcspException("exception processing TBSRequest: " + e, e);
			}

			AlgorithmIdentifier sigAlgId = OcspUtilities.GetSigAlgID(signingAlgorithm);

			if (chain != null && chain.Length > 0)
			{
				Asn1EncodableVector v = new Asn1EncodableVector();
				try
				{
					for (int i = 0; i != chain.Length; i++)
					{
						v.Add(
							X509CertificateStructure.GetInstance(
								Asn1Object.FromByteArray(chain[i].GetEncoded())));
					}
				}
				catch (IOException e)
				{
					throw new OcspException("error processing certs", e);
				}
				catch (CertificateEncodingException e)
				{
					throw new OcspException("error encoding certs", e);
				}

				return new BasicOcspResp(new BasicOcspResponse(tbsResp, sigAlgId, bitSig, new DerSequence(v)));
			}
			else
			{
				return new BasicOcspResp(new BasicOcspResponse(tbsResp, sigAlgId, bitSig, null));
			}
		}

		public BasicOcspResp Generate(
			string					signingAlgorithm,
			AsymmetricKeyParameter	privateKey,
			X509Certificate[]		chain,
			DateTime				thisUpdate)
		{
			return Generate(signingAlgorithm, privateKey, chain, thisUpdate, null);
		}

		public BasicOcspResp Generate(
			string					signingAlgorithm,
			AsymmetricKeyParameter	privateKey,
			X509Certificate[]		chain,
			DateTime				producedAt,
			SecureRandom			random)
		{
			if (signingAlgorithm == null)
			{
				throw new ArgumentException("no signing algorithm specified");
			}

			return GenerateResponse(signingAlgorithm, privateKey, chain, producedAt, random);
		}

		/**
		 * Return an IEnumerable of the signature names supported by the generator.
		 *
		 * @return an IEnumerable containing recognised names.
		 */
		public IEnumerable SignatureAlgNames
		{
			get { return OcspUtilities.AlgNames; }
		}
	}
}
