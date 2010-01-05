using System;
using System.Collections;

using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	/// <remarks>
	/// RFC 3126: 4.3.2 Revocation Values Attribute Definition
	/// <code>
	/// RevocationValues ::=  SEQUENCE {
	///		crlVals			[0] SEQUENCE OF CertificateList     OPTIONAL,
	///		ocspVals		[1] SEQUENCE OF BasicOCSPResponse   OPTIONAL,
	///		otherRevVals	[2] OtherRevVals
	/// }
	/// </code>
	/// </remarks>
	public class RevocationValues
		: Asn1Encodable
	{
		private readonly Asn1Sequence	crlVals;
		private readonly Asn1Sequence	ocspVals;
		private readonly OtherRevVals	otherRevVals;

		public static RevocationValues GetInstance(
			object obj)
		{
			if (obj == null || obj is RevocationValues)
				return (RevocationValues) obj;

			if (obj is Asn1Sequence)
				return new RevocationValues((Asn1Sequence) obj);

			throw new ArgumentException(
				"Unknown object in 'RevocationValues' factory: "
				+ obj.GetType().Name,
				"obj");
		}

		private RevocationValues(
			Asn1Sequence seq)
		{
			if (seq == null)
				throw new ArgumentNullException("seq");
			if (seq.Count < 1 || seq.Count > 3)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			bool otherRevValsFound = false;

			foreach (Asn1TaggedObject taggedObj in seq)
			{
				Asn1Object asn1Obj = taggedObj.GetObject();
				switch (taggedObj.TagNo)
				{
					case 0:
						Asn1Sequence crlValsSeq = (Asn1Sequence) asn1Obj;
						foreach (Asn1Encodable ae in crlValsSeq)
						{
							CertificateList.GetInstance(ae.ToAsn1Object());
						}
						this.crlVals = crlValsSeq;
						break;
					case 1:
						Asn1Sequence ocspValsSeq = (Asn1Sequence) asn1Obj;
						foreach (Asn1Encodable ae in ocspValsSeq)
						{
							BasicOcspResponse.GetInstance(ae.ToAsn1Object());
						}
						this.ocspVals = ocspValsSeq;
						break;
					case 2:
						this.otherRevVals = OtherRevVals.GetInstance(asn1Obj);
						otherRevValsFound = true;
						break;
					default:
						throw new ArgumentException("Illegal tag in RevocationValues", "seq");
				}
			}

			if (!otherRevValsFound)
				throw new ArgumentException("No otherRevVals found", "seq");
		}

		public RevocationValues(
			CertificateList[]	crlVals,
			BasicOcspResponse[]	ocspVals,
			OtherRevVals		otherRevVals)
		{
			if (otherRevVals == null)
				throw new ArgumentNullException("otherRevVals");

			if (crlVals != null)
			{
				this.crlVals = new DerSequence(crlVals);
			}

			if (ocspVals != null)
			{
				this.ocspVals = new DerSequence(ocspVals);
			}

			this.otherRevVals = otherRevVals;
		}

		public RevocationValues(
			IEnumerable			crlVals,
			IEnumerable			ocspVals,
			OtherRevVals		otherRevVals)
		{
			if (otherRevVals == null)
				throw new ArgumentNullException("otherRevVals");

			if (crlVals != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(crlVals, typeof(CertificateList)))
					throw new ArgumentException("Must contain only 'CertificateList' objects", "crlVals");

				this.crlVals = new DerSequence(
					Asn1EncodableVector.FromEnumerable(crlVals));
			}

			if (ocspVals != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(ocspVals, typeof(BasicOcspResponse)))
					throw new ArgumentException("Must contain only 'BasicOcspResponse' objects", "ocspVals");

				this.ocspVals = new DerSequence(
					Asn1EncodableVector.FromEnumerable(ocspVals));
			}

			this.otherRevVals = otherRevVals;
		}

		public CertificateList[] GetCrlVals()
		{
			CertificateList[] result = new CertificateList[crlVals.Count];
			for (int i = 0; i < crlVals.Count; ++i)
			{
				result[i] = CertificateList.GetInstance(crlVals[i].ToAsn1Object());
			}
			return result;
		}

		public BasicOcspResponse[] GetOcspVals()
		{
			BasicOcspResponse[] result = new BasicOcspResponse[ocspVals.Count];
			for (int i = 0; i < ocspVals.Count; ++i)
			{
				result[i] = BasicOcspResponse.GetInstance(ocspVals[i].ToAsn1Object());
			}
			return result;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (crlVals != null)
			{
				v.Add(new DerTaggedObject(true, 0, crlVals));
			}

			if (ocspVals != null)
			{
				v.Add(new DerTaggedObject(true, 1, ocspVals));
			}

			v.Add(new DerTaggedObject(true, 2, otherRevVals.ToAsn1Object()));

			return new DerSequence(v);
		}
	}
}
