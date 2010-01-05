using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>A NULL CipherSuite in java, this should only be used during handshake.</remarks>
	public class TlsNullCipherSuite
		: TlsCipherSuite
	{
		internal override void Init(
			byte[]	ms,
			byte[]	cr,
			byte[]	sr)
		{
			throw new TlsException("Sorry, init of TLS_NULL_WITH_NULL_NULL is forbidden");
		}

		internal override byte[] EncodePlaintext(
			short	type,
			byte[]	plaintext,
			int		offset,
			int		len)
		{
			byte[] result = new byte[len];
			Array.Copy(plaintext, offset, result, 0, len);
			return result;
		}

		internal override byte[] DecodeCiphertext(
			short				type,
			byte[]				plaintext,
			int					offset,
			int					len,
			TlsProtocolHandler	handler)
		{
			byte[] result = new byte[len];
			Array.Copy(plaintext, offset, result, 0, len);
			return result;
		}

		internal override short KeyExchangeAlgorithm
		{
			get { return 0; }
		}
	}
}
