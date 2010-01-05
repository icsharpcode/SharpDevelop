using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Signers
{

	/// <summary> ISO9796-2 - mechanism using a hash function with recovery (scheme 1)</summary>
	public class Iso9796d2Signer : ISignerWithRecovery
	{
		/// <summary>
		/// Return a reference to the recoveredMessage message.
		/// </summary>
		/// <returns>The full/partial recoveredMessage message.</returns>
		/// <seealso cref="ISignerWithRecovery.GetRecoveredMessage"/>
		public byte[] GetRecoveredMessage()
		{
			return recoveredMessage;
		}

		public const int TrailerImplicit = 0xBC;
		public const int TrailerRipeMD160 = 0x31CC;
		public const int TrailerRipeMD128 = 0x32CC;
		public const int TrailerSha1 = 0x33CC;

		private IDigest digest;
		private IAsymmetricBlockCipher cipher;

		private int trailer;
		private int keyBits;
		private byte[] block;
		private byte[] mBuf;
		private int messageLength;
		private bool fullMessage;
		private byte[] recoveredMessage;

		/// <summary>
		/// Generate a signer for the with either implicit or explicit trailers
		/// for ISO9796-2.
		/// </summary>
		/// <param name="cipher">base cipher to use for signature creation/verification</param>
		/// <param name="digest">digest to use.</param>
		/// <param name="isImplicit">whether or not the trailer is implicit or gives the hash.</param>
		public Iso9796d2Signer(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest,
			bool					isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;

			if (isImplicit)
			{
				trailer = TrailerImplicit;
			}
			else
			{
				if (digest is Sha1Digest)
				{
					trailer = TrailerSha1;
				}
				else if (digest is RipeMD160Digest)
				{
					trailer = TrailerRipeMD160;
				}
				else if (digest is RipeMD128Digest)
				{
					trailer = TrailerRipeMD128;
				}
				else
				{
					throw new System.ArgumentException("no valid trailer for digest");
				}
			}
		}

		/// <summary> Constructor for a signer with an explicit digest trailer.
		///
		/// </summary>
		/// <param name="cipher">cipher to use.
		/// </param>
		/// <param name="digest">digest to sign with.
		/// </param>
		public Iso9796d2Signer(IAsymmetricBlockCipher cipher, IDigest digest):this(cipher, digest, false)
		{
		}

		public string AlgorithmName
		{
			get { return digest.AlgorithmName + "with" + "ISO9796-2S1"; }
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			RsaKeyParameters kParam = (RsaKeyParameters) parameters;

			cipher.Init(forSigning, kParam);

			keyBits = kParam.Modulus.BitLength;

			block = new byte[(keyBits + 7) / 8];
			if (trailer == TrailerImplicit)
			{
				mBuf = new byte[block.Length - digest.GetDigestSize() - 2];
			}
			else
			{
				mBuf = new byte[block.Length - digest.GetDigestSize() - 3];
			}

			Reset();
		}

		/// <summary> compare two byte arrays.</summary>
		private bool IsSameAs(byte[] a, byte[] b)
		{
			if (messageLength > mBuf.Length)
			{
				if (mBuf.Length > b.Length)
				{
					return false;
				}

				for (int i = 0; i != mBuf.Length; i++)
				{
					if (a[i] != b[i])
					{
						return false;
					}
				}
			}
			else
			{
				if (messageLength != b.Length)
				{
					return false;
				}

				for (int i = 0; i != b.Length; i++)
				{
					if (a[i] != b[i])
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary> clear possible sensitive data</summary>
		private void  ClearBlock(
			byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		/// <summary> update the internal digest with the byte b</summary>
		public void Update(
			byte input)
		{
			digest.Update(input);

			if (messageLength < mBuf.Length)
			{
				mBuf[messageLength] = input;
			}

			messageLength++;
		}

		/// <summary> update the internal digest with the byte array in</summary>
		public void BlockUpdate(
			byte[]	input,
			int		inOff,
			int		length)
		{
			digest.BlockUpdate(input, inOff, length);

			if (messageLength < mBuf.Length)
			{
				for (int i = 0; i < length && (i + messageLength) < mBuf.Length; i++)
				{
					mBuf[messageLength + i] = input[inOff + i];
				}
			}

			messageLength += length;
		}

		/// <summary> reset the internal state</summary>
		public virtual void Reset()
		{
			digest.Reset();
			messageLength = 0;
			ClearBlock(mBuf);

			if (recoveredMessage != null)
			{
				ClearBlock(recoveredMessage);
			}

			recoveredMessage = null;
			fullMessage = false;
		}

		/// <summary> Generate a signature for the loaded message using the key we were
		/// initialised with.
		/// </summary>
		public virtual byte[] GenerateSignature()
		{
			int digSize = digest.GetDigestSize();

			int t = 0;
			int delta = 0;

			if (trailer == TrailerImplicit)
			{
				t = 8;
				delta = block.Length - digSize - 1;
				digest.DoFinal(block, delta);
				block[block.Length - 1] = (byte) TrailerImplicit;
			}
			else
			{
				t = 16;
				delta = block.Length - digSize - 2;
				digest.DoFinal(block, delta);
				block[block.Length - 2] = (byte) ((uint)trailer >> 8);
				block[block.Length - 1] = (byte) trailer;
			}

			byte header = 0;
			int x = (digSize + messageLength) * 8 + t + 4 - keyBits;

			if (x > 0)
			{
				int mR = messageLength - ((x + 7) / 8);
				header = (byte) (0x60);

				delta -= mR;

				Array.Copy(mBuf, 0, block, delta, mR);
			}
			else
			{
				header = (byte) (0x40);
				delta -= messageLength;

				Array.Copy(mBuf, 0, block, delta, messageLength);
			}

			if ((delta - 1) > 0)
			{
				for (int i = delta - 1; i != 0; i--)
				{
					block[i] = (byte) 0xbb;
				}
				block[delta - 1] ^= (byte) 0x01;
				block[0] = (byte) 0x0b;
				block[0] |= header;
			}
			else
			{
				block[0] = (byte) 0x0a;
				block[0] |= header;
			}

			byte[] b = cipher.ProcessBlock(block, 0, block.Length);

			ClearBlock(mBuf);
			ClearBlock(block);

			return b;
		}

		/// <summary> return true if the signature represents a ISO9796-2 signature
		/// for the passed in message.
		/// </summary>
		public virtual bool VerifySignature(byte[] signature)
		{
			byte[] block = cipher.ProcessBlock(signature, 0, signature.Length);

			if (((block[0] & 0xC0) ^ 0x40) != 0)
			{
				ClearBlock(mBuf);
				ClearBlock(block);

				return false;
			}

			if (((block[block.Length - 1] & 0xF) ^ 0xC) != 0)
			{
				ClearBlock(mBuf);
				ClearBlock(block);

				return false;
			}

			int delta = 0;

			if (((block[block.Length - 1] & 0xFF) ^ 0xBC) == 0)
			{
				delta = 1;
			}
			else
			{
				int sigTrail = ((block[block.Length - 2] & 0xFF) << 8) | (block[block.Length - 1] & 0xFF);

				switch (sigTrail)
				{
					case TrailerRipeMD160:
						if (!(digest is RipeMD160Digest))
						{
							throw new ArgumentException("signer should be initialised with RipeMD160");
						}
						break;
					case TrailerSha1:
						if (!(digest is Sha1Digest))
						{
							throw new ArgumentException("signer should be initialised with SHA1");
						}
						break;
					case TrailerRipeMD128:
						if (!(digest is RipeMD128Digest))
						{
							throw new ArgumentException("signer should be initialised with RipeMD128");
						}
						break;
					default:
						throw new ArgumentException("unrecognised hash in signature");
				}

				delta = 2;
			}

			//
			// find out how much padding we've got
			//
			int mStart = 0;
			for (; mStart != block.Length; mStart++)
			{
				if (((block[mStart] & 0x0f) ^ 0x0a) == 0)
				{
					break;
				}
			}

			mStart++;

			//
			// check the hashes
			//
			byte[] hash = new byte[digest.GetDigestSize()];

			int off = block.Length - delta - hash.Length;

			//
			// there must be at least one byte of message string
			//
			if ((off - mStart) <= 0)
			{
				ClearBlock(mBuf);
				ClearBlock(block);

				return false;
			}

			//
			// if we contain the whole message as well, check the hash of that.
			//
			if ((block[0] & 0x20) == 0)
			{
				fullMessage = true;

				digest.Reset();
				digest.BlockUpdate(block, mStart, off - mStart);
				digest.DoFinal(hash, 0);

				for (int i = 0; i != hash.Length; i++)
				{
					block[off + i] ^= hash[i];
					if (block[off + i] != 0)
					{
						ClearBlock(mBuf);
						ClearBlock(block);

						return false;
					}
				}

				recoveredMessage = new byte[off - mStart];
				Array.Copy(block, mStart, recoveredMessage, 0, recoveredMessage.Length);
			}
			else
			{
				fullMessage = false;

				digest.DoFinal(hash, 0);

				for (int i = 0; i != hash.Length; i++)
				{
					block[off + i] ^= hash[i];
					if (block[off + i] != 0)
					{
						ClearBlock(mBuf);
						ClearBlock(block);

						return false;
					}
				}

				recoveredMessage = new byte[off - mStart];
				Array.Copy(block, mStart, recoveredMessage, 0, recoveredMessage.Length);
			}

			//
			// if they've input a message check what we've recovered against
			// what was input.
			//
			if (messageLength != 0)
			{
				if (!IsSameAs(mBuf, recoveredMessage))
				{
					ClearBlock(mBuf);
					ClearBlock(block);
					ClearBlock(recoveredMessage);

					return false;
				}
			}

			ClearBlock(mBuf);
			ClearBlock(block);

			return true;
		}

		/// <summary>
		/// Return true if the full message was recoveredMessage.
		/// </summary>
		/// <returns> true on full message recovery, false otherwise.</returns>
		/// <seealso cref="ISignerWithRecovery.HasFullMessage"/>
		public virtual bool HasFullMessage()
		{
			return fullMessage;
		}
	}
}
