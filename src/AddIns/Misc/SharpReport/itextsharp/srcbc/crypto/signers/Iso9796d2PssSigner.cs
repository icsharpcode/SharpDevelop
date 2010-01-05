using System;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	/// <summary> ISO9796-2 - mechanism using a hash function with recovery (scheme 2 and 3).
	/// <p>
	/// Note: the usual length for the salt is the length of the hash
	/// function used in bytes.</p>
	/// </summary>
	public class Iso9796d2PssSigner
		: ISignerWithRecovery
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

		private SecureRandom random;
		private byte[] standardSalt;

		private int hLen;
		private int trailer;
		private int keyBits;
		private byte[] block;
		private byte[] mBuf;
		private int messageLength;
		private readonly int saltLength;
		private bool fullMessage;
		private byte[] recoveredMessage;

		/// <summary>
		/// Generate a signer for the with either implicit or explicit trailers
		/// for ISO9796-2, scheme 2 or 3.
		/// </summary>
		/// <param name="cipher">base cipher to use for signature creation/verification</param>
		/// <param name="digest">digest to use.</param>
		/// <param name="saltLength">length of salt in bytes.</param>
		/// <param name="isImplicit">whether or not the trailer is implicit or gives the hash.</param>
		public Iso9796d2PssSigner(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest,
			int						saltLength,
			bool					isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;
			this.hLen = digest.GetDigestSize();
			this.saltLength = saltLength;

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
					throw new ArgumentException("no valid trailer for digest");
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
		/// <param name="saltLength">length of salt in bytes.
		/// </param>
		public Iso9796d2PssSigner(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest,
			int						saltLength)
			: this(cipher, digest, saltLength, false)
		{
		}

		public string AlgorithmName
		{
			get { return digest.AlgorithmName + "with" + "ISO9796-2S2"; }
		}

		/// <summary>Initialise the signer.</summary>
		/// <param name="forSigning">true if for signing, false if for verification.</param>
		/// <param name="parameters">parameters for signature generation/verification. If the
		/// parameters are for generation they should be a ParametersWithRandom,
		/// a ParametersWithSalt, or just an RsaKeyParameters object. If RsaKeyParameters
		/// are passed in a SecureRandom will be created.
		/// </param>
		/// <exception cref="ArgumentException">if wrong parameter type or a fixed
		/// salt is passed in which is the wrong length.
		/// </exception>
		public virtual void Init(
			bool				forSigning,
			ICipherParameters	parameters)
		{
			RsaKeyParameters kParam;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom p = (ParametersWithRandom) parameters;

				kParam = (RsaKeyParameters) p.Parameters;

				if (forSigning)
				{
					random = p.Random;
				}
			}
			else if (parameters is ParametersWithSalt)
			{
				if (!forSigning)
					throw new ArgumentException("ParametersWithSalt only valid for signing", "parameters");

				ParametersWithSalt p = (ParametersWithSalt) parameters;

				kParam = (RsaKeyParameters) p.Parameters;
				standardSalt = p.GetSalt();

				if (standardSalt.Length != saltLength)
					throw new ArgumentException("Fixed salt is of wrong length");
			}
			else
			{
				kParam = (RsaKeyParameters) parameters;

				if (forSigning)
				{
					random = new SecureRandom();
				}
			}

			cipher.Init(forSigning, kParam);

			keyBits = kParam.Modulus.BitLength;

			block = new byte[(keyBits + 7) / 8];

			if (trailer == TrailerImplicit)
			{
				mBuf = new byte[block.Length - digest.GetDigestSize() - saltLength - 1 - 1];
			}
			else
			{
				mBuf = new byte[block.Length - digest.GetDigestSize() - saltLength - 1 - 2];
			}

			Reset();
		}

		/// <summary> compare two byte arrays.</summary>
		private bool IsSameAs(byte[] a, byte[] b)
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

			return true;
		}

		/// <summary> clear possible sensitive data</summary>
		private void  ClearBlock(
			byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		/// <summary> update the internal digest with the byte b</summary>
		public virtual void Update(
			byte input)
		{
			if (messageLength < mBuf.Length)
			{
				mBuf[messageLength++] = input;
			}
			else
			{
				digest.Update(input);
			}
		}

		/// <summary> update the internal digest with the byte array in</summary>
		public virtual void BlockUpdate(
			byte[]	input,
			int		inOff,
			int		length)
		{
			while (length > 0 && messageLength < mBuf.Length)
			{
				this.Update(input[inOff]);
				inOff++;
				length--;
			}

			if (length > 0)
			{
				digest.BlockUpdate(input, inOff, length);
			}
		}

		/// <summary> reset the internal state</summary>
		public virtual void Reset()
		{
			digest.Reset();
			messageLength = 0;
			if (mBuf != null)
			{
				ClearBlock(mBuf);
			}
			if (recoveredMessage != null)
			{
				ClearBlock(recoveredMessage);
				recoveredMessage = null;
			}
			fullMessage = false;
		}

		/// <summary> Generate a signature for the loaded message using the key we were
		/// initialised with.
		/// </summary>
		public byte[] GenerateSignature()
		{
			int digSize = digest.GetDigestSize();
			byte[] m2Hash = new byte[digSize];
			digest.DoFinal(m2Hash, 0);

			byte[] C = new byte[8];
			LtoOSP(messageLength * 8, C);

			digest.BlockUpdate(C, 0, C.Length);
			digest.BlockUpdate(mBuf, 0, messageLength);
			digest.BlockUpdate(m2Hash, 0, m2Hash.Length);

			byte[] salt;
			if (standardSalt != null)
			{
				salt = standardSalt;
			}
			else
			{
				salt = new byte[saltLength];
				random.NextBytes(salt);
			}

			digest.BlockUpdate(salt, 0, salt.Length);

			byte[] hash = new byte[digest.GetDigestSize()];
			digest.DoFinal(hash, 0);

			int tLength = 2;
			if (trailer == TrailerImplicit)
			{
				tLength = 1;
			}

			int off = block.Length - messageLength - salt.Length - hLen - tLength - 1;

			block[off] = (byte) (0x01);

			Array.Copy(mBuf, 0, block, off + 1, messageLength);
			Array.Copy(salt, 0, block, off + 1 + messageLength, salt.Length);

			byte[] dbMask = MaskGeneratorFunction1(hash, 0, hash.Length, block.Length - hLen - tLength);
			for (int i = 0; i != dbMask.Length; i++)
			{
				block[i] ^= dbMask[i];
			}

			Array.Copy(hash, 0, block, block.Length - hLen - tLength, hLen);

			if (trailer == TrailerImplicit)
			{
				block[block.Length - 1] = (byte)TrailerImplicit;
			}
			else
			{
				block[block.Length - 2] = (byte) ((uint)trailer >> 8);
				block[block.Length - 1] = (byte) trailer;
			}

			block[0] &= (byte) (0x7f);

			byte[] b = cipher.ProcessBlock(block, 0, block.Length);

			ClearBlock(mBuf);
			ClearBlock(block);
			messageLength = 0;

			return b;
		}

		/// <summary> return true if the signature represents a ISO9796-2 signature
		/// for the passed in message.
		/// </summary>
		public virtual bool VerifySignature(
			byte[] signature)
		{
			byte[] block = cipher.ProcessBlock(signature, 0, signature.Length);

			//
			// adjust block size for leading zeroes if necessary
			//
			int expectedSize = (keyBits + 7) / 8;
			if (block.Length < expectedSize)
			{
				byte[] tmp = new byte[expectedSize];
				block.CopyTo(tmp, tmp.Length - block.Length);
				ClearBlock(block);
				block = tmp;
			}

			int tLength;

			if (((block[block.Length - 1] & 0xFF) ^ 0xBC) == 0)
			{
				tLength = 1;
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

				tLength = 2;
			}

			//
			// calculate H(m2)
			//
			byte[] m2Hash = new byte[hLen];
			digest.DoFinal(m2Hash, 0);

			//
			// remove the mask
			//
			byte[] dbMask = MaskGeneratorFunction1(block, block.Length - hLen - tLength, hLen, block.Length - hLen - tLength);
			for (int i = 0; i != dbMask.Length; i++)
			{
				block[i] ^= dbMask[i];
			}

			block[0] &= 0x7f;

			//
			// find out how much padding we've got
			//
			int mStart = 0;
			while (mStart < block.Length)
			{
				if (block[mStart++] == 0x01)
					break;
			}

			if (mStart >= block.Length)
			{
				ClearBlock(block);
				return false;
			}

			fullMessage = (mStart > 1);

			// TODO Should we check if a standardSalt was set and, if so, use its length instead?
			recoveredMessage = new byte[dbMask.Length - mStart - saltLength];

			Array.Copy(block, mStart, recoveredMessage, 0, recoveredMessage.Length);

			//
			// check the hashes
			//
			byte[] C = new byte[8];
			LtoOSP(recoveredMessage.Length * 8, C);

			digest.BlockUpdate(C, 0, C.Length);

			if (recoveredMessage.Length != 0)
			{
				digest.BlockUpdate(recoveredMessage, 0, recoveredMessage.Length);
			}

			digest.BlockUpdate(m2Hash, 0, m2Hash.Length);

			// Update for the salt
			digest.BlockUpdate(block, mStart + recoveredMessage.Length, saltLength);

			byte[] hash = new byte[digest.GetDigestSize()];
			digest.DoFinal(hash, 0);

			int off = block.Length - tLength - hash.Length;

			for (int i = 0; i != hash.Length; i++)
			{
				if (hash[i] != block[off + i])
				{
					ClearBlock(block);
					ClearBlock(hash);
					ClearBlock(recoveredMessage);
					fullMessage = false;

					return false;
				}
			}

			ClearBlock(block);
			ClearBlock(hash);

			//
			// if they've input a message check what we've recovered against
			// what was input.
			//
			if (messageLength != 0)
			{
				if (!IsSameAs(mBuf, recoveredMessage))
				{
					ClearBlock(mBuf);
					return false;
				}

				messageLength = 0;
			}

			ClearBlock(mBuf);
			return true;
		}

		/// <summary>
		/// Return true if the full message was recoveredMessage.
		/// </summary>
		/// <returns>true on full message recovery, false otherwise, or if not sure.</returns>
		/// <seealso cref="ISignerWithRecovery.HasFullMessage"/>
		public virtual bool HasFullMessage()
		{
			return fullMessage;
		}

		/// <summary> int to octet string.</summary>
		/// <summary> int to octet string.</summary>
		private void ItoOSP(
			int		i,
			byte[]	sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)((uint)i >> 0);
		}

		/// <summary> long to octet string.</summary>
		private void  LtoOSP(long l, byte[] sp)
		{
			sp[0] = (byte)((ulong)l >> 56);
			sp[1] = (byte)((ulong)l >> 48);
			sp[2] = (byte)((ulong)l >> 40);
			sp[3] = (byte)((ulong)l >> 32);
			sp[4] = (byte)((ulong)l >> 24);
			sp[5] = (byte)((ulong)l >> 16);
			sp[6] = (byte)((ulong)l >> 8);
			sp[7] = (byte)((ulong)l >> 0);
		}

		/// <summary> mask generator function, as described in Pkcs1v2.</summary>
		private byte[] MaskGeneratorFunction1(
			byte[]	Z,
			int		zOff,
			int		zLen,
			int		length)
		{
			byte[] mask = new byte[length];
			byte[] hashBuf = new byte[hLen];
			byte[] C = new byte[4];
			int counter = 0;

			digest.Reset();

			do
			{
				ItoOSP(counter, C);

				digest.BlockUpdate(Z, zOff, zLen);
				digest.BlockUpdate(C, 0, C.Length);
				digest.DoFinal(hashBuf, 0);

				Array.Copy(hashBuf, 0, mask, counter * hLen, hLen);
			}
			while (++counter < (length / hLen));

			if ((counter * hLen) < length)
			{
				ItoOSP(counter, C);

				digest.BlockUpdate(Z, zOff, zLen);
				digest.BlockUpdate(C, 0, C.Length);
				digest.DoFinal(hashBuf, 0);

				Array.Copy(hashBuf, 0, mask, counter * hLen, mask.Length - (counter * hLen));
			}

			return mask;
		}
	}
}
