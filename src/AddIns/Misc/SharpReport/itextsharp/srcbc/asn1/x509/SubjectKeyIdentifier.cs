using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The SubjectKeyIdentifier object.
     * <pre>
     * SubjectKeyIdentifier::= OCTET STRING
     * </pre>
     */
    public class SubjectKeyIdentifier
        : Asn1Encodable
    {
        private readonly byte[] keyIdentifier;

		public static SubjectKeyIdentifier GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1OctetString.GetInstance(obj, explicitly));
        }

		public static SubjectKeyIdentifier GetInstance(
            object obj)
        {
            if (obj is SubjectKeyIdentifier)
            {
                return (SubjectKeyIdentifier) obj;
            }

			if (obj is SubjectPublicKeyInfo)
            {
                return new SubjectKeyIdentifier((SubjectPublicKeyInfo) obj);
            }

			if (obj is Asn1OctetString)
            {
                return new SubjectKeyIdentifier((Asn1OctetString) obj);
            }

			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension) obj));
			}

			throw new ArgumentException("Invalid SubjectKeyIdentifier: " + obj.GetType().Name);
        }

		public SubjectKeyIdentifier(
            byte[] keyID)
        {
			if (keyID == null)
				throw new ArgumentNullException("keyID");

			this.keyIdentifier = keyID;
        }

		public SubjectKeyIdentifier(
            Asn1OctetString keyID)
        {
            this.keyIdentifier = keyID.GetOctets();
        }

		/**
         *
         * Calulates the keyIdentifier using a SHA1 hash over the BIT STRING
         * from SubjectPublicKeyInfo as defined in RFC2459.
         *
         **/
        public SubjectKeyIdentifier(
            SubjectPublicKeyInfo spki)
        {
            IDigest digest = new Sha1Digest();
            byte[] resBuf = new byte[digest.GetDigestSize()];

			byte[] bytes = spki.PublicKeyData.GetBytes();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            digest.DoFinal(resBuf, 0);
            this.keyIdentifier = resBuf;
        }

		public byte[] GetKeyIdentifier()
        {
            return keyIdentifier;
        }

		public override Asn1Object ToAsn1Object()
        {
            return new DerOctetString(keyIdentifier);
        }
    }
}
