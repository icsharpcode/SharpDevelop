using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	/**
	 * <code>NoticeReference</code> class, used in
	 * <code>CertificatePolicies</code> X509 V3 extensions
	 * (in policy qualifiers).
	 *
	 * <pre>
	 *  NoticeReference ::= Sequence {
	 *      organization     DisplayText,
	 *      noticeNumbers    Sequence OF Integer }
	 *
	 * </pre>
	 *
	 * @see PolicyQualifierInfo
	 * @see PolicyInformation
	 */
	public class NoticeReference
		: Asn1Encodable
	{
		internal readonly DisplayText organization;
		internal readonly Asn1Sequence noticeNumbers;

		/**
		* Creates a new <code>NoticeReference</code> instance.
		*
		* @param orgName a <code>string</code> value
		* @param numbers a <code>ArrayList</code> value
		*/
		public NoticeReference(
			string		orgName,
			ArrayList	numbers)
		{
			organization = new DisplayText(orgName);

			object o = numbers[0];

			Asn1EncodableVector av = new Asn1EncodableVector();
			if (o is int)
			{
				foreach (int nm in numbers)
				{
					av.Add(new DerInteger(nm));
				}
			}

			noticeNumbers = new DerSequence(av);
		}

		/**
		 * Creates a new <code>NoticeReference</code> instance.
		 *
		 * @param orgName a <code>string</code> value
		 * @param numbers an <code>Asn1Sequence</code> value
		 */
		public NoticeReference(
			string			orgName,
			Asn1Sequence	numbers)
		{
			organization = new DisplayText(orgName);
			noticeNumbers = numbers;
		}

		/**
		 * Creates a new <code>NoticeReference</code> instance.
		 *
		 * @param displayTextType an <code>int</code> value
		 * @param orgName a <code>string</code> value
		 * @param numbers an <code>Asn1Sequence</code> value
		 */
		public NoticeReference(
			int				displayTextType,
			string			orgName,
			Asn1Sequence	numbers)
		{
			organization = new DisplayText(displayTextType, orgName);
			noticeNumbers = numbers;
		}

		/**
		 * Creates a new <code>NoticeReference</code> instance.
		 * <p>Useful for reconstructing a <code>NoticeReference</code>
		 * instance from its encodable/encoded form.</p>
		 *
		 * @param as an <code>Asn1Sequence</code> value obtained from either
		 * calling @{link ToAsn1Object()} for a <code>NoticeReference</code>
		 * instance or from parsing it from a Der-encoded stream.
		 */
		private NoticeReference(
			Asn1Sequence seq)
		{
			if (seq.Count != 2)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			organization = DisplayText.GetInstance(seq[0]);
			noticeNumbers = Asn1Sequence.GetInstance(seq[1]);
		}

		public static NoticeReference GetInstance(
			object obj)
		{
			if (obj is NoticeReference)
			{
				return (NoticeReference) obj;
			}

			if (obj is Asn1Sequence)
			{
				return new NoticeReference((Asn1Sequence) obj);
			}

			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		/**
		 * Describe <code>ToAsn1Object</code> method here.
		 *
		 * @return a <code>Asn1Object</code> value
		 */
		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(organization, noticeNumbers);
		}
	}
}
