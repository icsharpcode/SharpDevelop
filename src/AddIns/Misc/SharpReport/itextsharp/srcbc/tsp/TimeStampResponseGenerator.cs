using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Tsp
{
	/**
	 * Generator for RFC 3161 Time Stamp Responses.
	 */
	public class TimeStampResponseGenerator
	{
		private PkiStatus				status;

		private Asn1EncodableVector		statusStrings;

		private int						failInfo;
		private TimeStampTokenGenerator	tokenGenerator;
		private IList					acceptedAlgorithms;
		private IList					acceptedPolicies;
		private IList					acceptedExtensions;

		public TimeStampResponseGenerator(
			TimeStampTokenGenerator	tokenGenerator,
			IList					acceptedAlgorithms)
			: this(tokenGenerator, acceptedAlgorithms, null, null)
		{
		}

		public TimeStampResponseGenerator(
			TimeStampTokenGenerator	tokenGenerator,
			IList					acceptedAlgorithms,
			IList					acceptedPolicy)
			: this(tokenGenerator, acceptedAlgorithms, acceptedPolicy, null)
		{
		}

		public TimeStampResponseGenerator(
			TimeStampTokenGenerator	tokenGenerator,
			IList					acceptedAlgorithms,
			IList					acceptedPolicies,
			IList					acceptedExtensions)
		{
			this.tokenGenerator = tokenGenerator;
			this.acceptedAlgorithms = acceptedAlgorithms;
			this.acceptedPolicies = acceptedPolicies;
			this.acceptedExtensions = acceptedExtensions;

			statusStrings = new Asn1EncodableVector();
		}

		private void addStatusString(
			string statusString)
		{
			statusStrings.Add(new DerUtf8String(statusString));
		}

		private void setFailInfoField(int field)
		{
			failInfo = failInfo | field;
		}

		private PkiStatusInfo getPkiStatusInfo()
		{
			Asn1EncodableVector	v = new Asn1EncodableVector(
				new DerInteger((int) status));

			if (statusStrings.Count > 0)
			{
				v.Add(new PkiFreeText(new DerSequence(statusStrings)));
			}

			if (failInfo != 0)
			{
				v.Add(new FailInfo(failInfo));
			}

			return new PkiStatusInfo(new DerSequence(v));
		}

		public TimeStampResponse Generate(
			TimeStampRequest	request,
			BigInteger			serialNumber,
			DateTime			genTime)
		{
			TimeStampResp resp;

			try
			{
				request.Validate(acceptedAlgorithms, acceptedPolicies, acceptedExtensions);

				status = PkiStatus.Granted;
				this.addStatusString("Operation Okay");

				PkiStatusInfo pkiStatusInfo = getPkiStatusInfo();

				ContentInfo tstTokenContentInfo;
				try
				{
					TimeStampToken token = tokenGenerator.Generate(request, serialNumber, genTime);
					byte[] encoded = token.ToCmsSignedData().GetEncoded();

					tstTokenContentInfo = ContentInfo.GetInstance(Asn1Object.FromByteArray(encoded));
				}
				catch (IOException ioEx)
				{
					throw new TspException(
						"Timestamp token received cannot be converted to ContentInfo", ioEx);
				}

				resp = new TimeStampResp(pkiStatusInfo, tstTokenContentInfo);
			}
			catch (TspValidationException e)
			{
				status = PkiStatus.Rejection;

				this.setFailInfoField(e.FailureCode);
				this.addStatusString(e.Message);

				PkiStatusInfo pkiStatusInfo = getPkiStatusInfo();

				resp = new TimeStampResp(pkiStatusInfo, null);
			}

			try
			{
				return new TimeStampResponse(resp);
			}
			catch (IOException)
			{
				throw new TspException("created badly formatted response!");
			}
		}

		class FailInfo
			: DerBitString
		{
			internal FailInfo(
				int failInfoValue)
				: base(GetBytes(failInfoValue), GetPadBits(failInfoValue))
			{
			}
		}
	}
}
