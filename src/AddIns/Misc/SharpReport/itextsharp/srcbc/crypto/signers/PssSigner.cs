using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	/// <summary> RSA-PSS as described in Pkcs# 1 v 2.1.
	/// <p>
	/// Note: the usual value for the salt length is the number of
	/// bytes in the hash function.</p>
	/// </summary>
	public class PssSigner
		: ISigner
	{
		public const byte TrailerImplicit = (byte)0xBC;

		private readonly IDigest digest;
		private readonly IAsymmetricBlockCipher cipher;

		private SecureRandom random;

		private int hLen;
		private int sLen;
		private int emBits;
		private byte[] salt;
		private byte[] mDash;
		private byte[] block;
		private byte trailer;

		public PssSigner(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest)
			: this(cipher, digest, digest.GetDigestSize())
		{
		}

		/// <summary>Basic constructor</summary>
		/// <param name="cipher">the asymmetric cipher to use.</param>
		/// <param name="digest">the digest to use.</param>
		/// <param name="saltLen">the length of the salt to use (in bytes).</param>
		public PssSigner(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest,
			int						saltLen)
			: this(cipher, digest, saltLen, TrailerImplicit)
		{
		}

		public PssSigner(
			IAsymmetricBlockCipher	cipher,
			IDigest					digest,
			int						saltLen,
			byte					trailer)
		{
			this.cipher = cipher;
			this.digest = digest;
			this.hLen = digest.GetDigestSize();
			this.sLen = saltLen;
			this.salt = new byte[saltLen];
			this.mDash = new byte[8 + saltLen + hLen];
			this.trailer = trailer;
		}

		public string AlgorithmName
		{
			get { return digest.AlgorithmName + "withRSAandMGF1"; }
		}

		public virtual void Init(
			bool				forSigning,
			ICipherParameters	parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom p = (ParametersWithRandom) parameters;

				parameters = p.Parameters;
				random = p.Random;
			}
			else
			{
				if (forSigning)
				{
					random = new SecureRandom();
				}
			}

			cipher.Init(forSigning, parameters);

			RsaKeyParameters kParam;
			if (parameters is RsaBlindingParameters)
			{
				kParam = ((RsaBlindingParameters) parameters).PublicKey;
			}
			else
			{
				kParam = (RsaKeyParameters) parameters;
			}

			emBits = kParam.Modulus.BitLength - 1;

			block = new byte[(emBits + 7) / 8];
		}

		/// <summary> clear possible sensitive data</summary>
		private void ClearBlock(
			byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		/// <summary> update the internal digest with the byte b</summary>
		public virtual void Update(
			byte input)
		{
			digest.Update(input);
		}

		/// <summary> update the internal digest with the byte array in</summary>
		public virtual void BlockUpdate(
			byte[]	input,
			int		inOff,
			int		length)
		{
			digest.BlockUpdate(input, inOff, length);
		}

		/// <summary> reset the internal state</summary>
		public virtual void Reset()
		{
			digest.Reset();
		}

		/// <summary> Generate a signature for the message we've been loaded with using
		/// the key we were initialised with.
		/// </summary>
		public virtual byte[] GenerateSignature()
		{
			if (emBits < (8 * hLen + 8 * sLen + 9))
			{
				throw new DataLengthException("encoding error");
			}

			digest.DoFinal(mDash, mDash.Length - hLen - sLen);

			if (sLen != 0)
			{
				random.NextBytes(salt);
				salt.CopyTo(mDash, mDash.Length - sLen);
			}

			byte[] h = new byte[hLen];

			digest.BlockUpdate(mDash, 0, mDash.Length);

			digest.DoFinal(h, 0);

			block[block.Length - sLen - 1 - hLen - 1] = (byte) (0x01);
			salt.CopyTo(block, block.Length - sLen - hLen - 1);

			byte[] dbMask = MaskGeneratorFunction1(h, 0, h.Length, block.Length - hLen - 1);
			for (int i = 0; i != dbMask.Length; i++)
			{
				block[i] ^= dbMask[i];
			}

			block[0] &= (byte) ((0xff >> ((block.Length * 8) - emBits)));

			h.CopyTo(block, block.Length - hLen - 1);

			block[block.Length - 1] = trailer;

			byte[] b = cipher.ProcessBlock(block, 0, block.Length);

			ClearBlock(block);

			return b;
		}

		/// <summary> return true if the internal state represents the signature described
		/// in the passed in array.
		/// </summary>
		public virtual bool VerifySignature(
			byte[] signature)
		{
			if (emBits < (8 * hLen + 8 * sLen + 9))
			{
				return false;
			}

			digest.DoFinal(mDash, mDash.Length - hLen - sLen);

			byte[] b = cipher.ProcessBlock(signature, 0, signature.Length);
			b.CopyTo(block, block.Length - b.Length);

			if (block[block.Length - 1] != trailer)
			{
				ClearBlock(block);
				return false;
			}

			byte[] dbMask = MaskGeneratorFunction1(block, block.Length - hLen - 1, hLen, block.Length - hLen - 1);

			for (int i = 0; i != dbMask.Length; i++)
			{
				block[i] ^= dbMask[i];
			}

			block[0] &= (byte) ((0xff >> ((block.Length * 8) - emBits)));

			for (int i = 0; i != block.Length - hLen - sLen - 2; i++)
			{
				if (block[i] != 0)
				{
					ClearBlock(block);
					return false;
				}
			}

			if (block[block.Length - hLen - sLen - 2] != 0x01)
			{
				ClearBlock(block);
				return false;
			}

			Array.Copy(block, block.Length - sLen - hLen - 1, mDash, mDash.Length - sLen, sLen);

			digest.BlockUpdate(mDash, 0, mDash.Length);
			digest.DoFinal(mDash, mDash.Length - hLen);

			for (int i = block.Length - hLen - 1, j = mDash.Length - hLen; j != mDash.Length; i++, j++)
			{
				if ((block[i] ^ mDash[j]) != 0)
				{
					ClearBlock(mDash);
					ClearBlock(block);
					return false;
				}
			}

			ClearBlock(mDash);
			ClearBlock(block);

			return true;
		}

		/// <summary> int to octet string.</summary>
		private void ItoOSP(
			int		i,
			byte[]	sp)
		{
			sp[0] = (byte)((uint) i >> 24);
			sp[1] = (byte)((uint) i >> 16);
			sp[2] = (byte)((uint) i >> 8);
			sp[3] = (byte)((uint) i >> 0);
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

			while (counter < (length / hLen))
			{
				ItoOSP(counter, C);

				digest.BlockUpdate(Z, zOff, zLen);
				digest.BlockUpdate(C, 0, C.Length);
				digest.DoFinal(hashBuf, 0);

				hashBuf.CopyTo(mask, counter * hLen);
				++counter;
			}

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
