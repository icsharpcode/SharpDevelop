using System;

using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>
	/// A certificate verifyer, that will always return true.
	/// <pre>
	/// DO NOT USE THIS FILE UNLESS YOU KNOW EXACTLY WHAT YOU ARE DOING.
	/// </pre>
	/// </remarks>
	public class AlwaysValidVerifyer
		: ICertificateVerifyer
	{
		/// <summary>Return true.</summary>
		public bool IsValid(
			X509CertificateStructure[] certs)
		{
			return true;
		}
	}
}
