using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class EncryptionScheme
        : AlgorithmIdentifier
    {
        private readonly Asn1Object	objectID, obj;

		internal EncryptionScheme(
			Asn1Sequence seq)
			: base(seq)
        {
            objectID = (Asn1Object) seq[0];
            obj = (Asn1Object) seq[1];
        }

		public Asn1Object Asn1Object
		{
			get { return obj; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(objectID, obj);
        }
    }
}
