using System;

using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>
	/// This should be implemented by any class which can find out, if a given
	/// certificate chain is beeing accepted by an client.
	/// </remarks>
	public interface ICertificateVerifyer
	{
		/// <param name="certs">The certs, which are part of the chain.</param>
		/// <returns>True, if the chain is accepted, false otherwise</returns>
		bool IsValid(X509CertificateStructure[] certs);
	}
}
