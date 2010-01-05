using System;
using System.Collections;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class ContentInfo
        : Asn1Encodable
    {
        private readonly DerObjectIdentifier	contentType;
        private readonly Asn1Encodable			content;

		public static ContentInfo GetInstance(
            object obj)
        {
            if (obj is ContentInfo)
            {
                return (ContentInfo) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new ContentInfo((Asn1Sequence) obj);
            }

			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
        }

		private ContentInfo(
            Asn1Sequence seq)
        {
			contentType = (DerObjectIdentifier) seq[0];

			if (seq.Count > 1)
			{
				content = ((Asn1TaggedObject) seq[1]).GetObject();
			}
        }

		public ContentInfo(
            DerObjectIdentifier	contentType,
            Asn1Encodable		content)
        {
            this.contentType = contentType;
            this.content = content;
        }

		public DerObjectIdentifier ContentType
		{
			get { return contentType; }
		}

		public Asn1Encodable Content
		{
			get { return content; }
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * ContentInfo ::= Sequence {
         *          contentType ContentType,
         *          content
         *          [0] EXPLICIT ANY DEFINED BY contentType OPTIONAL }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(contentType);

			if (content != null)
            {
                v.Add(new BerTaggedObject(0, content));
            }

			return new BerSequence(v);
        }
    }
}
