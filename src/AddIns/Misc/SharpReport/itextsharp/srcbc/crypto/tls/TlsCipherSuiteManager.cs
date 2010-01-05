using System;
using System.IO;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>
	/// A manager for ciphersuite. This class does manage all ciphersuites
	/// which are used by MicroTLS.
	/// </remarks>
	public class TlsCipherSuiteManager
	{
		private const int TLS_RSA_WITH_3DES_EDE_CBC_SHA = 0x000a;
		private const int TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA = 0x0016;
		private const int TLS_RSA_WITH_AES_128_CBC_SHA = 0x002f;
		private const int TLS_DHE_RSA_WITH_AES_128_CBC_SHA = 0x0033;
		private const int TLS_RSA_WITH_AES_256_CBC_SHA = 0x0035;
		private const int TLS_DHE_RSA_WITH_AES_256_CBC_SHA = 0x0039;

		internal static void WriteCipherSuites(
			Stream outStr)
		{
			TlsUtilities.WriteUint16(2 * 6, outStr);

			TlsUtilities.WriteUint16(TLS_DHE_RSA_WITH_AES_256_CBC_SHA, outStr);
			TlsUtilities.WriteUint16(TLS_DHE_RSA_WITH_AES_128_CBC_SHA, outStr);
			TlsUtilities.WriteUint16(TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA, outStr);

			TlsUtilities.WriteUint16(TLS_RSA_WITH_AES_256_CBC_SHA, outStr);
			TlsUtilities.WriteUint16(TLS_RSA_WITH_AES_128_CBC_SHA, outStr);
			TlsUtilities.WriteUint16(TLS_RSA_WITH_3DES_EDE_CBC_SHA, outStr);

		}

		internal static TlsCipherSuite GetCipherSuite(
			int					number,
			TlsProtocolHandler	handler)
		{
			switch (number)
			{
				case TLS_RSA_WITH_3DES_EDE_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new DesEdeEngine()), new CbcBlockCipher(new DesEdeEngine()), new Sha1Digest(), new Sha1Digest(), 24, TlsCipherSuite.KE_RSA);

				case TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new DesEdeEngine()), new CbcBlockCipher(new DesEdeEngine()), new Sha1Digest(), new Sha1Digest(), 24, TlsCipherSuite.KE_DHE_RSA);

				case TLS_RSA_WITH_AES_128_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new AesFastEngine()), new CbcBlockCipher(new AesFastEngine()), new Sha1Digest(), new Sha1Digest(), 16, TlsCipherSuite.KE_RSA);

				case TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new AesFastEngine()), new CbcBlockCipher(new AesFastEngine()), new Sha1Digest(), new Sha1Digest(), 16, TlsCipherSuite.KE_DHE_RSA);

				case TLS_RSA_WITH_AES_256_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new AesFastEngine()), new CbcBlockCipher(new AesFastEngine()), new Sha1Digest(), new Sha1Digest(), 32, TlsCipherSuite.KE_RSA);

				case TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
					return new TlsBlockCipherCipherSuite(new CbcBlockCipher(new AesFastEngine()), new CbcBlockCipher(new AesFastEngine()), new Sha1Digest(), new Sha1Digest(), 32, TlsCipherSuite.KE_DHE_RSA);

				default:
					handler.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_handshake_failure);

					/*
					* Unreachable Code, failWithError will always throw an exception!
					*/
					return null;
			}
		}
	}
}
