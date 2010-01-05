using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>A generic class for ciphersuites in TLS 1.0.</remarks>
	public abstract class TlsCipherSuite
	{
		internal const short KE_RSA = 1;
		internal const short KE_RSA_EXPORT = 2;
		internal const short KE_DHE_DSS = 3;
		internal const short KE_DHE_DSS_EXPORT = 4;
		internal const short KE_DHE_RSA = 5;
		internal const short KE_DHE_RSA_EXPORT = 6;
		internal const short KE_DH_DSS = 7;
		internal const short KE_DH_RSA = 8;
		internal const short KE_DH_anon = 9;

		internal abstract void Init(byte[] ms, byte[] cr, byte[] sr);

		internal abstract byte[] EncodePlaintext(short type, byte[] plaintext, int offset, int len);

		internal abstract byte[] DecodeCiphertext(short type, byte[] plaintext, int offset, int len, TlsProtocolHandler handler);

		internal abstract short KeyExchangeAlgorithm { get; }
	}
}
