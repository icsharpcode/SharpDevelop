using System.Collections;

using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Pkcs
{
    public class X509CertificateEntry
        : Pkcs12Entry
    {
        private readonly X509Certificate cert;

		public X509CertificateEntry(
            X509Certificate cert)
			: base(new Hashtable())
        {
            this.cert = cert;
        }

		public X509CertificateEntry(
            X509Certificate	cert,
            Hashtable		attributes)
			: base(attributes)
        {
            this.cert = cert;
        }

		public X509Certificate Certificate
        {
			get { return this.cert; }
        }
    }
}
