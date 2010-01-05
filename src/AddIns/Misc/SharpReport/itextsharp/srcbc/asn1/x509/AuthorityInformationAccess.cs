using System;
using System.Collections;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The AuthorityInformationAccess object.
     * <pre>
     * id-pe-authorityInfoAccess OBJECT IDENTIFIER ::= { id-pe 1 }
     *
     * AuthorityInfoAccessSyntax  ::=
     *      Sequence SIZE (1..MAX) OF AccessDescription
     * AccessDescription  ::=  Sequence {
     *       accessMethod          OBJECT IDENTIFIER,
     *       accessLocation        GeneralName  }
     *
     * id-ad OBJECT IDENTIFIER ::= { id-pkix 48 }
     * id-ad-caIssuers OBJECT IDENTIFIER ::= { id-ad 2 }
     * id-ad-ocsp OBJECT IDENTIFIER ::= { id-ad 1 }
     * </pre>
     */
    public class AuthorityInformationAccess
        : Asn1Encodable
    {
        internal readonly DerObjectIdentifier	accessMethod;
        internal readonly GeneralName			accessLocation;

		public static AuthorityInformationAccess GetInstance(
            object obj)
        {
            if (obj is AuthorityInformationAccess)
            {
                return (AuthorityInformationAccess) obj;
            }

			if (obj is Asn1Sequence)
			{
				return new AuthorityInformationAccess((Asn1Sequence) obj);
			}

			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension) obj));
			}

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private AuthorityInformationAccess(
            Asn1Sequence seq)
        {
			foreach (DerSequence vec in seq)
			{
                if (vec.Count != 2)
                {
                    throw new ArgumentException("wrong number of elements in inner sequence");
                }

				accessMethod = (DerObjectIdentifier) vec[0];
                accessLocation = (GeneralName) vec[1];
            }
        }

		/**
         * create an AuthorityInformationAccess with the oid and location provided.
         */
        public AuthorityInformationAccess(
            DerObjectIdentifier	oid,
            GeneralName			location)
        {
            accessMethod = oid;
            accessLocation = location;
        }

		public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(new DerSequence(accessMethod, accessLocation));
        }

		public override string ToString()
        {
            return ("AuthorityInformationAccess: Oid(" + this.accessMethod.Id + ")");
        }
    }
}
