using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class PbeS2Parameters
        : Asn1Encodable
    {
        private readonly KeyDerivationFunc func;
        private readonly EncryptionScheme scheme;

		public PbeS2Parameters(
            Asn1Sequence obj)
        {
            IEnumerator e = obj.GetEnumerator();

			e.MoveNext();
            Asn1Sequence funcSeq = (Asn1Sequence) e.Current;

			if (funcSeq[0].Equals(PkcsObjectIdentifiers.IdPbkdf2))
            {
                func = new KeyDerivationFunc(PkcsObjectIdentifiers.IdPbkdf2, funcSeq[1]);
            }
            else
            {
                func = new KeyDerivationFunc(funcSeq);
            }

			e.MoveNext();
            scheme = new EncryptionScheme((Asn1Sequence) e.Current);
        }

		public KeyDerivationFunc KeyDerivationFunc
		{
			get { return func; }
		}

		public EncryptionScheme EncryptionScheme
		{
			get { return scheme; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(func, scheme);
        }
    }
}
