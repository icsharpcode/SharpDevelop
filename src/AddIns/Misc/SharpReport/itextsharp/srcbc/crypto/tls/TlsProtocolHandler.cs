using System;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>An implementation of all high level protocols in TLS 1.0.</remarks>
	public class TlsProtocolHandler
	{
		private const short RL_CHANGE_CIPHER_SPEC = 20;
		private const short RL_ALERT = 21;
		private const short RL_HANDSHAKE = 22;
		private const short RL_APPLICATION_DATA = 23;

		/*
		hello_request(0), client_hello(1), server_hello(2),
		certificate(11), server_key_exchange (12),
		certificate_request(13), server_hello_done(14),
		certificate_verify(15), client_key_exchange(16),
		finished(20), (255)
		*/

		private const short HP_HELLO_REQUEST = 0;
		private const short HP_CLIENT_HELLO = 1;
		private const short HP_SERVER_HELLO = 2;
		private const short HP_CERTIFICATE = 11;
		private const short HP_SERVER_KEY_EXCHANGE = 12;
		private const short HP_CERTIFICATE_REQUEST = 13;
		private const short HP_SERVER_HELLO_DONE = 14;
		private const short HP_CERTIFICATE_VERIFY = 15;
		private const short HP_CLIENT_KEY_EXCHANGE = 16;
		private const short HP_FINISHED = 20;

		/*
		* Our Connection states
		*/

		private const short CS_CLIENT_HELLO_SEND = 1;
		private const short CS_SERVER_HELLO_RECEIVED = 2;
		private const short CS_SERVER_CERTIFICATE_RECEIVED = 3;
		private const short CS_SERVER_KEY_EXCHANGE_RECEIVED = 4;
		private const short CS_CERTIFICATE_REQUEST_RECEIVED = 5;
		private const short CS_SERVER_HELLO_DONE_RECEIVED = 6;
		private const short CS_CLIENT_KEY_EXCHANGE_SEND = 7;
		private const short CS_CLIENT_CHANGE_CIPHER_SPEC_SEND = 8;
		private const short CS_CLIENT_FINISHED_SEND = 9;
		private const short CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED = 10;
		private const short CS_DONE = 11;

		internal const short AP_close_notify = 0;
		internal const short AP_unexpected_message = 10;
		internal const short AP_bad_record_mac = 20;
		internal const short AP_decryption_failed = 21;
		internal const short AP_record_overflow = 22;
		internal const short AP_decompression_failure = 30;
		internal const short AP_handshake_failure = 40;
		internal const short AP_bad_certificate = 42;
		internal const short AP_unsupported_certificate = 43;
		internal const short AP_certificate_revoked = 44;
		internal const short AP_certificate_expired = 45;
		internal const short AP_certificate_unknown = 46;
		internal const short AP_illegal_parameter = 47;
		internal const short AP_unknown_ca = 48;
		internal const short AP_access_denied = 49;
		internal const short AP_decode_error = 50;
		internal const short AP_decrypt_error = 51;
		internal const short AP_export_restriction = 60;
		internal const short AP_protocol_version = 70;
		internal const short AP_insufficient_security = 71;
		internal const short AP_internal_error = 80;
		internal const short AP_user_canceled = 90;
		internal const short AP_no_renegotiation = 100;

		internal const short AL_warning = 1;
		internal const short AL_fatal = 2;

		private static readonly byte[] emptybuf = new byte[0];

		private static readonly string TLS_ERROR_MESSAGE = "Internal TLS error, this could be an attack";

		/*
		* Queues for data from some protocols.
		*/

		private ByteQueue applicationDataQueue = new ByteQueue();
		private ByteQueue changeCipherSpecQueue = new ByteQueue();
		private ByteQueue alertQueue = new ByteQueue();
		private ByteQueue handshakeQueue = new ByteQueue();

		/*
		* The Record Stream we use
		*/
		private RecordStream rs;

		private SecureRandom random;

		/*
		* The public rsa-key of the server.
		*/
		private RsaKeyParameters serverRsaKey = null;

		private TlsInputStream tlsInputStream = null;
		private TlsOuputStream tlsOutputStream = null;

		private bool closed = false;
		private bool failedWithError = false;
		private bool appDataReady = false;

		private byte[] clientRandom;
		private byte[] serverRandom;
		private byte[] ms;

		private TlsCipherSuite choosenCipherSuite = null;

		private BigInteger Yc;
		private byte[] pms;

		private ICertificateVerifyer verifyer = null;

		/*
		* Both streams can be the same object
		*/
		public TlsProtocolHandler(
			Stream	inStr,
			Stream	outStr)
		{
			/*
			 * We use a threaded seed generator to generate a good random
			 * seed. If the user has a better random seed, he should use
			 * the constructor with a SecureRandom.
			 * 
			 * Hopefully, 20 bytes in fast mode are good enough.
			 */
			byte[] seed = new ThreadedSeedGenerator().GenerateSeed(20, true);

			this.random = new SecureRandom(seed);
			this.rs = new RecordStream(this, inStr, outStr);
		}

		public TlsProtocolHandler(
			Stream			inStr,
			Stream			outStr,
			SecureRandom	sr)
		{
			this.random = sr;
			this.rs = new RecordStream(this, inStr, outStr);
		}

		private short connection_state;

		internal void ProcessData(
			short	protocol,
			byte[]	buf,
			int		offset,
			int		len)
		{
			/*
			* Have a look at the protocol type, and add it to the correct queue.
			*/
			switch (protocol)
			{
				case RL_CHANGE_CIPHER_SPEC:
					changeCipherSpecQueue.AddData(buf, offset, len);
					processChangeCipherSpec();
					break;
				case RL_ALERT:
					alertQueue.AddData(buf, offset, len);
					processAlert();
					break;
				case RL_HANDSHAKE:
					handshakeQueue.AddData(buf, offset, len);
					processHandshake();
					break;
				case RL_APPLICATION_DATA:
					if (!appDataReady)
					{
						this.FailWithError(AL_fatal, AP_unexpected_message);
					}
					applicationDataQueue.AddData(buf, offset, len);
					processApplicationData();
					break;
				default:
					/*
					* Uh, we don't know this protocol.
					*
					* RFC2246 defines on page 13, that we should ignore this.
					*/
					break;
			}
		}

		private void processHandshake()
		{
			bool read;
			do
			{
				read = false;

				/*
				* We need the first 4 bytes, they contain type and length of
				* the message.
				*/
				if (handshakeQueue.Available >= 4)
				{
					byte[] beginning = new byte[4];
					handshakeQueue.Read(beginning, 0, 4, 0);
					MemoryStream bis = new MemoryStream(beginning, false);
					short type = TlsUtilities.ReadUint8(bis);
					int len = TlsUtilities.ReadUint24(bis);

					/*
					* Check if we have enough bytes in the buffer to read
					* the full message.
					*/
					if (handshakeQueue.Available >= (len + 4))
					{
						/*
						* Read the message.
						*/
						byte[] buf = new byte[len];
						handshakeQueue.Read(buf, 0, len, 4);
						handshakeQueue.RemoveData(len + 4);

						/*
						* If it is not a finished message, update our hashes
						* we prepare for the finish message.
						*/
						if (type != HP_FINISHED)
						{
							rs.hash1.BlockUpdate(beginning, 0, 4);
							rs.hash2.BlockUpdate(beginning, 0, 4);
							rs.hash1.BlockUpdate(buf, 0, len);
							rs.hash2.BlockUpdate(buf, 0, len);
						}

						/*
						* Now, parse the message.
						*/
						MemoryStream inStr = new MemoryStream(buf, false);

						/*
						* Check the type.
						*/
						switch (type)
						{
							case HP_CERTIFICATE:
								switch (connection_state)
								{
									case CS_SERVER_HELLO_RECEIVED:
										/*
										* Parse the certificates.
										*/
										Certificate cert = Certificate.Parse(inStr);
										AssertEmpty(inStr);

										/*
										* Verify them.
										*/
										if (!this.verifyer.IsValid(cert.GetCerts()))
										{
											this.FailWithError(AL_fatal, AP_user_canceled);
										}

										/*
										* We only support RSA certificates. Lets hope
										* this is one.
										*/
										RsaPublicKeyStructure rsaKey = null;
										try
										{
											rsaKey = RsaPublicKeyStructure.GetInstance(
												cert.certs[0].TbsCertificate.SubjectPublicKeyInfo.GetPublicKey());
										}
										catch (Exception)
										{
											/*
											* Sorry, we have to fail ;-(
											*/
											this.FailWithError(AL_fatal, AP_unsupported_certificate);
										}

										/*
										* Parse the servers public RSA key.
										*/
										this.serverRsaKey = new RsaKeyParameters(
											false,
											rsaKey.Modulus,
											rsaKey.PublicExponent);

										connection_state = CS_SERVER_CERTIFICATE_RECEIVED;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_unexpected_message);
										break;
								}
								break;
							case HP_FINISHED:
								switch (connection_state)
								{
									case CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED:
										/*
										* Read the checksum from the finished message,
										* it has always 12 bytes.
										*/
										byte[] receivedChecksum = new byte[12];
										TlsUtilities.ReadFully(receivedChecksum, inStr);
										AssertEmpty(inStr);

										/*
										* Calculate our own checksum.
										*/
										byte[] checksum = new byte[12];
										byte[] md5andsha1 = new byte[16 + 20];
										rs.hash2.DoFinal(md5andsha1, 0);
										TlsUtilities.PRF(this.ms, TlsUtilities.ToByteArray("server finished"), md5andsha1, checksum);

										/*
										* Compare both checksums.
										*/
										for (int i = 0; i < receivedChecksum.Length; i++)
										{
											if (receivedChecksum[i] != checksum[i])
											{
												/*
												* Wrong checksum in the finished message.
												*/
												this.FailWithError(AL_fatal, AP_handshake_failure);
											}
										}

										connection_state = CS_DONE;

										/*
										* We are now ready to receive application data.
										*/
										this.appDataReady = true;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_unexpected_message);
										break;
								}
								break;
							case HP_SERVER_HELLO:
								switch (connection_state)
								{
									case CS_CLIENT_HELLO_SEND:
										/*
										* Read the server hello message
										*/
										TlsUtilities.CheckVersion(inStr, this);

										/*
										* Read the server random
										*/
										this.serverRandom = new byte[32];
										TlsUtilities.ReadFully(this.serverRandom, inStr);

										/*
										* Currently, we don't support session ids
										*/
										short sessionIdLength = TlsUtilities.ReadUint8(inStr);
										byte[] sessionId = new byte[sessionIdLength];
										TlsUtilities.ReadFully(sessionId, inStr);

										/*
										* Find out which ciphersuite the server has
										* chosen. If we don't support this ciphersuite,
										* the TlsCipherSuiteManager will throw an
										* exception.
										*/
										this.choosenCipherSuite = TlsCipherSuiteManager.GetCipherSuite(
											TlsUtilities.ReadUint16(inStr), this);

										/*
										* We support only the null compression which
										* means no compression.
										*/
										short compressionMethod = TlsUtilities.ReadUint8(inStr);
										if (compressionMethod != 0)
										{
											this.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_illegal_parameter);
										}
										AssertEmpty(inStr);

										connection_state = CS_SERVER_HELLO_RECEIVED;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_unexpected_message);
										break;
								}
								break;
							case HP_SERVER_HELLO_DONE:
								switch (connection_state)
								{
									case CS_SERVER_CERTIFICATE_RECEIVED:
									case CS_SERVER_KEY_EXCHANGE_RECEIVED:
									case CS_CERTIFICATE_REQUEST_RECEIVED:

										// NB: Original code used case label fall-through
										if (connection_state == CS_SERVER_CERTIFICATE_RECEIVED)
										{
											/*
											* There was no server key exchange message, check
											* that we are doing RSA key exchange.
											*/
											if (this.choosenCipherSuite.KeyExchangeAlgorithm != TlsCipherSuite.KE_RSA)
											{
												this.FailWithError(AL_fatal, AP_unexpected_message);
											}
										}

										AssertEmpty(inStr);
										bool isCertReq = (connection_state == CS_CERTIFICATE_REQUEST_RECEIVED);
										connection_state = CS_SERVER_HELLO_DONE_RECEIVED;

										if (isCertReq)
										{
											sendClientCertificate();
										}

										/*
										* Send the client key exchange message, depending
										* on the key exchange we are using in our
										* ciphersuite.
										*/
										short ke = this.choosenCipherSuite.KeyExchangeAlgorithm;

										switch (ke)
										{
											case TlsCipherSuite.KE_RSA:
												/*
												* We are doing RSA key exchange. We will
												* choose a pre master secret and send it
												* rsa encrypted to the server.
												*
												* Prepare pre master secret.
												*/
												pms = new byte[48];
												pms[0] = 3;
												pms[1] = 1;
												random.NextBytes(pms, 2, 46);

												/*
												* Encode the pms and send it to the server.
												*
												* Prepare an Pkcs1Encoding with good random
												* padding.
												*/
												RsaBlindedEngine rsa = new RsaBlindedEngine();
												Pkcs1Encoding encoding = new Pkcs1Encoding(rsa);
												encoding.Init(true, new ParametersWithRandom(this.serverRsaKey, this.random));
												byte[] encrypted = null;
												try
												{
													encrypted = encoding.ProcessBlock(pms, 0, pms.Length);
												}
												catch (InvalidCipherTextException)
												{
													/*
													* This should never happen, only during decryption.
													*/
													this.FailWithError(AL_fatal, AP_internal_error);
												}

												/*
												* Send the encrypted pms.
												*/
												MemoryStream bos = new MemoryStream();
												TlsUtilities.WriteUint8(HP_CLIENT_KEY_EXCHANGE, bos);
												TlsUtilities.WriteUint24(encrypted.Length + 2, bos);
												TlsUtilities.WriteUint16(encrypted.Length, bos);
												bos.Write(encrypted, 0, encrypted.Length);
												byte[] message = bos.ToArray();

												rs.WriteMessage((short)RL_HANDSHAKE, message, 0, message.Length);
												break;
											case TlsCipherSuite.KE_DHE_RSA:
												/*
												* Send the Client Key Exchange message for
												* DHE key exchange.
												*/
												byte[] YcByte = this.Yc.ToByteArray();
												MemoryStream DHbos = new MemoryStream();
												TlsUtilities.WriteUint8(HP_CLIENT_KEY_EXCHANGE, DHbos);
												TlsUtilities.WriteUint24(YcByte.Length + 2, DHbos);
												TlsUtilities.WriteUint16(YcByte.Length, DHbos);
												DHbos.Write(YcByte, 0, YcByte.Length);
												byte[] DHmessage = DHbos.ToArray();

												rs.WriteMessage((short)RL_HANDSHAKE, DHmessage, 0, DHmessage.Length);

												break;
											default:
												/*
												* Problem during handshake, we don't know
												* how to handle this key exchange method.
												*/
												this.FailWithError(AL_fatal, AP_unexpected_message);
												break;

										}

										connection_state = CS_CLIENT_KEY_EXCHANGE_SEND;

										/*
										* Now, we send change cipher state
										*/
										byte[] cmessage = new byte[1];
										cmessage[0] = 1;
										rs.WriteMessage((short)RL_CHANGE_CIPHER_SPEC, cmessage, 0, cmessage.Length);

										connection_state = CS_CLIENT_CHANGE_CIPHER_SPEC_SEND;

										/*
										* Calculate the ms
										*/
										this.ms = new byte[48];
										byte[] randBytes = new byte[clientRandom.Length + serverRandom.Length];
										Array.Copy(clientRandom, 0, randBytes, 0, clientRandom.Length);
										Array.Copy(serverRandom, 0, randBytes, clientRandom.Length, serverRandom.Length);
										TlsUtilities.PRF(pms, TlsUtilities.ToByteArray("master secret"), randBytes, this.ms);

										/*
										* Initialize our cipher suite
										*/
										rs.writeSuite = this.choosenCipherSuite;
										rs.writeSuite.Init(this.ms, clientRandom, serverRandom);

										/*
										* Send our finished message.
										*/
										byte[] checksum = new byte[12];
										byte[] md5andsha1 = new byte[16 + 20];
										rs.hash1.DoFinal(md5andsha1, 0);
										TlsUtilities.PRF(this.ms, TlsUtilities.ToByteArray("client finished"), md5andsha1, checksum);

										MemoryStream bos2 = new MemoryStream();
										TlsUtilities.WriteUint8(HP_FINISHED, bos2);
										TlsUtilities.WriteUint24(12, bos2);
										bos2.Write(checksum, 0, checksum.Length);
										byte[] message2 = bos2.ToArray();

										rs.WriteMessage((short)RL_HANDSHAKE, message2, 0, message2.Length);

										this.connection_state = CS_CLIENT_FINISHED_SEND;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_handshake_failure);
										break;
								}
								break;
							case HP_SERVER_KEY_EXCHANGE:
								switch (connection_state)
								{
									case CS_SERVER_CERTIFICATE_RECEIVED:
										/*
										* Check that we are doing DHE key exchange
										*/
										if (this.choosenCipherSuite.KeyExchangeAlgorithm != TlsCipherSuite.KE_DHE_RSA)
										{
											this.FailWithError(AL_fatal, AP_unexpected_message);
										}

										/*
										* Parse the Structure
										*/
										int pLength = TlsUtilities.ReadUint16(inStr);
										byte[] pByte = new byte[pLength];
										TlsUtilities.ReadFully(pByte, inStr);

										int gLength = TlsUtilities.ReadUint16(inStr);
										byte[] gByte = new byte[gLength];
										TlsUtilities.ReadFully(gByte, inStr);

										int YsLength = TlsUtilities.ReadUint16(inStr);
										byte[] YsByte = new byte[YsLength];
										TlsUtilities.ReadFully(YsByte, inStr);

										int sigLength = TlsUtilities.ReadUint16(inStr);
										byte[] sigByte = new byte[sigLength];
										TlsUtilities.ReadFully(sigByte, inStr);

										AssertEmpty(inStr);

										/*
										* Verify the Signature.
										*
										* First, calculate the hash.
										*/
										CombinedHash sigDigest = new CombinedHash();
										MemoryStream signedData = new MemoryStream();
										TlsUtilities.WriteUint16(pLength, signedData);
										signedData.Write(pByte, 0, pByte.Length);
										TlsUtilities.WriteUint16(gLength, signedData);
										signedData.Write(gByte, 0, gByte.Length);
										TlsUtilities.WriteUint16(YsLength, signedData);
										signedData.Write(YsByte, 0, YsByte.Length);
										byte[] signed = signedData.ToArray();

										sigDigest.BlockUpdate(this.clientRandom, 0, this.clientRandom.Length);
										sigDigest.BlockUpdate(this.serverRandom, 0, this.serverRandom.Length);
										sigDigest.BlockUpdate(signed, 0, signed.Length);
										byte[] hash = new byte[sigDigest.GetDigestSize()];
										sigDigest.DoFinal(hash, 0);

										/*
										* Now, do the RSA operation
										*/
										RsaBlindedEngine rsa = new RsaBlindedEngine();
										Pkcs1Encoding encoding = new Pkcs1Encoding(rsa);
										encoding.Init(false, this.serverRsaKey);

										/*
										* The data which was signed
										*/
										byte[] sigHash = null;

										try
										{
											sigHash = encoding.ProcessBlock(sigByte, 0, sigByte.Length);
										}
										catch (InvalidCipherTextException)
										{
											this.FailWithError(AL_fatal, AP_bad_certificate);
										}

										/*
										* Check if the data which was signed is equal to
										* the hash we calculated.
										*/
										if (sigHash.Length != hash.Length)
										{
											this.FailWithError(AL_fatal, AP_bad_certificate);
										}

										for (int i = 0; i < sigHash.Length; i++)
										{
											if (sigHash[i] != hash[i])
											{
												this.FailWithError(AL_fatal, AP_bad_certificate);
											}
										}

										/*
										* OK, Signature was correct.
										*
										* Do the DH calculation.
										*/
										BigInteger p = new BigInteger(1, pByte);
										BigInteger g = new BigInteger(1, gByte);
										BigInteger Ys = new BigInteger(1, YsByte);
										BigInteger x = new BigInteger(p.BitLength - 1, this.random);
										Yc = g.ModPow(x, p);
										this.pms = Ys.ModPow(x, p).ToByteArrayUnsigned();

										this.connection_state = CS_SERVER_KEY_EXCHANGE_RECEIVED;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_unexpected_message);
										break;
								}
								break;
							case HP_CERTIFICATE_REQUEST:
								switch (connection_state)
								{
									case CS_SERVER_CERTIFICATE_RECEIVED:
									case CS_SERVER_KEY_EXCHANGE_RECEIVED:

										// NB: Original code used case label fall-through
										if (connection_state == CS_SERVER_CERTIFICATE_RECEIVED)
										{
											/*
											* There was no server key exchange message, check
											* that we are doing RSA key exchange.
											*/
											if (this.choosenCipherSuite.KeyExchangeAlgorithm != TlsCipherSuite.KE_RSA)
											{
												this.FailWithError(AL_fatal, AP_unexpected_message);
											}
										}

										int typesLength = TlsUtilities.ReadUint8(inStr);
										byte[] types = new byte[typesLength];
										TlsUtilities.ReadFully(types, inStr);

										int authsLength = TlsUtilities.ReadUint16(inStr);
										byte[] auths = new byte[authsLength];
										TlsUtilities.ReadFully(auths, inStr);

										AssertEmpty(inStr);

										this.connection_state = CS_CERTIFICATE_REQUEST_RECEIVED;
										read = true;
										break;
									default:
										this.FailWithError(AL_fatal, AP_unexpected_message);
										break;
								}
								break;
							case HP_HELLO_REQUEST:
							case HP_CLIENT_KEY_EXCHANGE:
							case HP_CERTIFICATE_VERIFY:
							case HP_CLIENT_HELLO:
							default:
								// We do not support this!
								this.FailWithError(AL_fatal, AP_unexpected_message);
								break;

						}

					}
				}
			}
			while (read);

		}

		private void processApplicationData()
		{
			/*
			* There is nothing we need to do here.
			* 
			* This function could be used for callbacks when application
			* data arrives in the future.
			*/
		}

		private void processAlert()
		{
			while (alertQueue.Available >= 2)
			{
				/*
				* An alert is always 2 bytes. Read the alert.
				*/
				byte[] tmp = new byte[2];
				alertQueue.Read(tmp, 0, 2, 0);
				alertQueue.RemoveData(2);
				short level = tmp[0];
				short description = tmp[1];
				if (level == AL_fatal)
				{
					/*
					* This is a fatal error.
					*/
					this.failedWithError = true;
					this.closed = true;
					/*
					* Now try to Close the stream, ignore errors.
					*/
					try
					{
						rs.Close();
					}
					catch (Exception)
					{
					}
					throw new IOException(TLS_ERROR_MESSAGE);
				}
				else
				{
					/*
					* This is just a warning.
					*/
					if (description == AP_close_notify)
					{
						/*
						* Close notify
						*/
						this.FailWithError(AL_warning, AP_close_notify);
					}
					/*
					* If it is just a warning, we continue.
					*/
				}
			}

		}

		/**
		* This method is called, when a change cipher spec message is received.
		*
		* @throws IOException If the message has an invalid content or the
		*                     handshake is not in the correct state.
		*/
		private void processChangeCipherSpec()
		{
			while (changeCipherSpecQueue.Available > 0)
			{
				/*
				* A change cipher spec message is only one byte with the value 1.
				*/
				byte[] b = new byte[1];
				changeCipherSpecQueue.Read(b, 0, 1, 0);
				changeCipherSpecQueue.RemoveData(1);
				if (b[0] != 1)
				{
					/*
					* This should never happen.
					*/
					this.FailWithError(AL_fatal, AP_unexpected_message);

				}
				else
				{
					/*
					* Check if we are in the correct connection state.
					*/
					if (this.connection_state == CS_CLIENT_FINISHED_SEND)
					{
						rs.readSuite = rs.writeSuite;
						this.connection_state = CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED;
					}
					else
					{
						/*
						* We are not in the correct connection state.
						*/
						this.FailWithError(AL_fatal, AP_handshake_failure);
					}

				}
			}
		}

		private void sendClientCertificate()
		{
			/*
			* just write back the "no client certificate" message
			* see also gnutls, auth_cert.c:643 (0B 00 00 03 00 00 00)
			*/
			MemoryStream bos = new MemoryStream();
			TlsUtilities.WriteUint8(HP_CERTIFICATE, bos);
			TlsUtilities.WriteUint24(3, bos);
			TlsUtilities.WriteUint24(0, bos);
			byte[] message = bos.ToArray();

			rs.WriteMessage((short)RL_HANDSHAKE, message, 0, message.Length);
		}

		/// <summary>Connects to the remote system.</summary>
		/// <param name="verifyer">Will be used when a certificate is received to verify
		/// that this certificate is accepted by the client.</param>
		/// <exception cref="IOException">If handshake was not successful</exception>
		public virtual void Connect(
			ICertificateVerifyer verifyer)
		{
			this.verifyer = verifyer;

			/*
			* Send Client hello
			*
			* First, generate some random data.
			*/
			this.clientRandom = new byte[32];

			/*
			* TLS 1.0 requires a unix-timestamp in the first 4 bytes
			*/
			int t = (int)(DateTimeUtilities.CurrentUnixMs() / 1000L);
			this.clientRandom[0] = (byte)(t >> 24);
			this.clientRandom[1] = (byte)(t >> 16);
			this.clientRandom[2] = (byte)(t >> 8);
			this.clientRandom[3] = (byte)t;

			random.NextBytes(this.clientRandom, 4, 28);


			MemoryStream outStr = new MemoryStream();
			TlsUtilities.WriteVersion(outStr);
			outStr.Write(this.clientRandom, 0, this.clientRandom.Length);

			/*
			* Length of Session id
			*/
			TlsUtilities.WriteUint8((short)0, outStr);

			/*
			* Cipher suites
			*/
			TlsCipherSuiteManager.WriteCipherSuites(outStr);

			/*
			* Compression methods, just the null method.
			*/
			byte[] compressionMethods = new byte[]{0x00};
			TlsUtilities.WriteUint8((short)compressionMethods.Length, outStr);
			outStr.Write(compressionMethods,0, compressionMethods.Length);


			MemoryStream bos = new MemoryStream();
			TlsUtilities.WriteUint8(HP_CLIENT_HELLO, bos);
			TlsUtilities.WriteUint24((int) outStr.Length, bos);
			byte[] outBytes = outStr.ToArray();
			bos.Write(outBytes, 0, outBytes.Length);
			byte[] message = bos.ToArray();
			rs.WriteMessage(RL_HANDSHAKE, message, 0, message.Length);
			connection_state = CS_CLIENT_HELLO_SEND;

			/*
			* We will now read data, until we have completed the handshake.
			*/
			while (connection_state != CS_DONE)
			{
				rs.ReadData();
			}

			this.tlsInputStream = new TlsInputStream(this);
			this.tlsOutputStream = new TlsOuputStream(this);
		}

		/**
		* Read data from the network. The method will return immed, if there is
		* still some data left in the buffer, or block untill some application
		* data has been read from the network.
		*
		* @param buf    The buffer where the data will be copied to.
		* @param offset The position where the data will be placed in the buffer.
		* @param len    The maximum number of bytes to read.
		* @return The number of bytes read.
		* @throws IOException If something goes wrong during reading data.
		*/
		internal int ReadApplicationData(byte[] buf, int offset, int len)
		{
			while (applicationDataQueue.Available == 0)
			{
				/*
				* We need to read some data.
				*/
				if (this.failedWithError)
				{
					/*
					* Something went terribly wrong, we should throw an IOException
					*/
					throw new IOException(TLS_ERROR_MESSAGE);
				}
				if (this.closed)
				{
					/*
					* Connection has been closed, there is no more data to read.
					*/
					return 0;
				}

				try
				{
					rs.ReadData();
				}
				catch (IOException e)
				{
					if (!this.closed)
					{
						this.FailWithError(AL_fatal, AP_internal_error);
					}
					throw e;
				}
				catch (Exception e)
				{
					if (!this.closed)
					{
						this.FailWithError(AL_fatal, AP_internal_error);
					}
					throw e;
				}
			}
			len = System.Math.Min(len, applicationDataQueue.Available);
			applicationDataQueue.Read(buf, offset, len, 0);
			applicationDataQueue.RemoveData(len);
			return len;
		}

		/**
		* Send some application data to the remote system.
		* <p/>
		* The method will handle fragmentation internally.
		*
		* @param buf    The buffer with the data.
		* @param offset The position in the buffer where the data is placed.
		* @param len    The length of the data.
		* @throws IOException If something goes wrong during sending.
		*/
		internal void WriteData(byte[] buf, int offset, int len)
		{
			if (this.failedWithError)
			{
				throw new IOException(TLS_ERROR_MESSAGE);
			}
			if (this.closed)
			{
				throw new IOException("Sorry, connection has been closed, you cannot write more data");
			}

			/*
			* Protect against known IV attack!
			*
			* DO NOT REMOVE THIS LINE, EXCEPT YOU KNOW EXACTLY WHAT
			* YOU ARE DOING HERE.
			*/
			rs.WriteMessage(RL_APPLICATION_DATA, emptybuf, 0, 0);

			do
			{
				/*
				* We are only allowed to write fragments up to 2^14 bytes.
				*/
				int toWrite = System.Math.Min(len, 1 << 14);

				try
				{
					rs.WriteMessage(RL_APPLICATION_DATA, buf, offset, toWrite);
				}
				catch (IOException e)
				{
					if (!closed)
					{
						this.FailWithError(AL_fatal, AP_internal_error);
					}
					throw e;
				}
				catch (Exception e)
				{
					if (!closed)
					{
						this.FailWithError(AL_fatal, AP_internal_error);
					}
					throw e;
				}


				offset += toWrite;
				len -= toWrite;
			}
			while (len > 0);

		}

		[Obsolete("Use 'OutputStream' property instead")]
		public TlsOuputStream TlsOuputStream
		{
			get { return this.tlsOutputStream; }
		}

		/// <summary>A Stream which can be used to send data.</summary>
		public virtual Stream OutputStream
		{
			get { return this.tlsOutputStream; }
		}

		[Obsolete("Use 'InputStream' property instead")]
		public TlsInputStream TlsInputStream
		{
			get { return this.tlsInputStream; }
		}

		/// <summary>A Stream which can be used to read data.</summary>
		public virtual Stream InputStream
		{
			get { return this.tlsInputStream; }
		}

		/**
		* Terminate this connection whith an alert.
		* <p/>
		* Can be used for normal closure too.
		*
		* @param alertLevel       The level of the alert, an be AL_fatal or AL_warning.
		* @param alertDescription The exact alert message.
		* @throws IOException If alert was fatal.
		*/
		internal void FailWithError(
			short	alertLevel,
			short	alertDescription)
		{
			/*
			* Check if the connection is still open.
			*/
			if (!closed)
			{
				/*
				* Prepare the message
				*/
				byte[] error = new byte[2];
				error[0] = (byte)alertLevel;
				error[1] = (byte)alertDescription;
				this.closed = true;

				if (alertLevel == AL_fatal)
				{
					/*
					* This is a fatal message.
					*/
					this.failedWithError = true;
				}
				rs.WriteMessage(RL_ALERT, error, 0, 2);
				rs.Close();
				if (alertLevel == AL_fatal)
				{
					throw new IOException(TLS_ERROR_MESSAGE);
				}

			}
			else
			{
				throw new IOException(TLS_ERROR_MESSAGE);
			}
		}

		/// <summary>Closes this connection</summary>
		/// <exception cref="IOException">If something goes wrong during closing.</exception>
		public virtual void Close()
		{
			if (!closed)
			{
				this.FailWithError((short)1, (short)0);
			}
		}

		/**
		* Make sure the Stream is now empty. Fail otherwise.
		*
		* @param is The Stream to check.
		* @throws IOException If is is not empty.
		*/
		internal void AssertEmpty(
			MemoryStream inStr)
		{
//			if (inStr.available() > 0)
			if (inStr.Position < inStr.Length)
			{
				this.FailWithError(AL_fatal, AP_decode_error);
			}
		}

		internal void Flush()
		{
			rs.Flush();
		}
	}
}
