using System;

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes
{
	/**
	* A Two-Pass Authenticated-Encryption Scheme Optimized for Simplicity and 
	* Efficiency - by M. Bellare, P. Rogaway, D. Wagner.
	* 
	* http://www.cs.ucdavis.edu/~rogaway/papers/eax.pdf
	* 
	* EAX is an AEAD scheme based on CTR and OMAC1/CMAC, that uses a single block 
	* cipher to encrypt and authenticate data. It's on-line (the length of a 
	* message isn't needed to begin processing it), has good performances, it's
	* simple and provably secure (provided the underlying block cipher is secure).
	* 
	* Of course, this implementations is NOT thread-safe.
	*/
	public class EaxBlockCipher
		: IAeadBlockCipher
	{
		private enum Tag : byte { N, H, C };

		private SicBlockCipher cipher;

		private bool forEncryption;

		private int blockSize;

		private IMac mac;

		private byte[] nonceMac;
		private byte[] associatedTextMac;
		private byte[] macBlock;

		private int macSize;
		private byte[] bufBlock;
		private int bufOff;

		/**
		* Constructor that accepts an instance of a block cipher engine.
		*
		* @param cipher the engine to use
		*/
		public EaxBlockCipher(
			IBlockCipher cipher)
		{
			blockSize = cipher.GetBlockSize();
			mac = new CMac(cipher);
			macBlock = new byte[blockSize];
			bufBlock = new byte[blockSize * 2];
			associatedTextMac = new byte[mac.GetMacSize()];
			nonceMac = new byte[mac.GetMacSize()];
			this.cipher = new SicBlockCipher(cipher);
		}

		public virtual string AlgorithmName
		{
			get { return cipher.GetUnderlyingCipher().AlgorithmName + "/EAX"; }
		}

		public virtual int GetBlockSize()
		{
			return cipher.GetBlockSize();
		}

		public virtual void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;

			byte[] nonce, associatedText;
			ICipherParameters keyParam;

			if (parameters is AeadParameters)
			{
				AeadParameters param = (AeadParameters) parameters;

				nonce = param.GetNonce();
				associatedText = param.GetAssociatedText();
				macSize = param.MacSize / 8;
				keyParam = param.Key;
			}
			else if (parameters is ParametersWithIV)
			{
				ParametersWithIV param = (ParametersWithIV) parameters;

				nonce = param.GetIV();
				associatedText = new byte[0];
				macSize = mac.GetMacSize() / 2;
				keyParam = param.Parameters;
			}
			else
			{
				throw new ArgumentException("invalid parameters passed to EAX");
			}

			byte[] tag = new byte[blockSize];

			mac.Init(keyParam);
			tag[blockSize - 1] = (byte) Tag.H;
			mac.BlockUpdate(tag, 0, blockSize);
			mac.BlockUpdate(associatedText, 0, associatedText.Length);
			mac.DoFinal(associatedTextMac, 0);

			tag[blockSize - 1] = (byte) Tag.N;
			mac.BlockUpdate(tag, 0, blockSize);
			mac.BlockUpdate(nonce, 0, nonce.Length);
			mac.DoFinal(nonceMac, 0);

			tag[blockSize - 1] = (byte) Tag.C;
			mac.BlockUpdate(tag, 0, blockSize);

			cipher.Init(true, new ParametersWithIV(keyParam, nonceMac));
		}

		private void calculateMac()
		{
			byte[] outC = new byte[blockSize];
			mac.DoFinal(outC, 0);

			for (int i = 0; i < macBlock.Length; i++)
			{
				macBlock[i] = (byte)(nonceMac[i] ^ associatedTextMac[i] ^ outC[i]);
			}
		}

		public virtual void Reset()
		{
			Reset(true);
		}

		private void Reset(
			bool clearMac)
		{
			cipher.Reset();
			mac.Reset();

			bufOff = 0;
			Array.Clear(bufBlock, 0, bufBlock.Length);

			if (clearMac)
			{
				Array.Clear(macBlock, 0, macBlock.Length);
			}

			byte[] tag = new byte[blockSize];
			tag[blockSize - 1] = (byte) Tag.C;
			mac.BlockUpdate(tag, 0, blockSize);
		}

		public virtual int ProcessByte(
			byte	input,
			byte[]	outBytes,
			int		outOff)
		{
			return process(input, outBytes, outOff);
		}

		public virtual int ProcessBytes(
			byte[]	inBytes,
			int		inOff,
			int		len,
			byte[]	outBytes,
			int		outOff)
		{
			int resultLen = 0;

			for (int i = 0; i != len; i++)
			{
				resultLen += process(inBytes[inOff + i], outBytes, outOff + resultLen);
			}

			return resultLen;
		}

		public virtual int DoFinal(
			byte[]	outBytes,
			int		outOff)
		{
			int extra = bufOff;
			byte[] tmp = new byte[bufBlock.Length];

			bufOff = 0;

			if (forEncryption)
			{
				cipher.ProcessBlock(bufBlock, 0, tmp, 0);
				cipher.ProcessBlock(bufBlock, blockSize, tmp, blockSize);

				Array.Copy(tmp, 0, outBytes, outOff, extra);

				mac.BlockUpdate(tmp, 0, extra);

				calculateMac();

				Array.Copy(macBlock, 0, outBytes, outOff + extra, macSize);

				Reset(false);

				return extra + macSize;
			}
			else
			{
				if (extra > macSize)
				{
					mac.BlockUpdate(bufBlock, 0, extra - macSize);

					cipher.ProcessBlock(bufBlock, 0, tmp, 0);
					cipher.ProcessBlock(bufBlock, blockSize, tmp, blockSize);

					Array.Copy(tmp, 0, outBytes, outOff, extra - macSize);
				}

				calculateMac();

				if (!verifyMac(bufBlock, extra - macSize))
					throw new InvalidCipherTextException("mac check in EAX failed");

				Reset(false);

				return extra - macSize;
			}
		}

		public virtual byte[] GetMac()
		{
			byte[] mac = new byte[macSize];

			Array.Copy(macBlock, 0, mac, 0, macSize);

			return mac;
		}

		public virtual int GetUpdateOutputSize(
			int len)
		{
			return ((len + bufOff) / blockSize) * blockSize;
		}

		public virtual int GetOutputSize(
			int len)
		{
			if (forEncryption)
			{
				return len + bufOff + macSize;
			}

			return len + bufOff - macSize;
		}

		private int process(
			byte	b,
			byte[]	outBytes,
			int		outOff)
		{
			bufBlock[bufOff++] = b;

			if (bufOff == bufBlock.Length)
			{
				int size;

				if (forEncryption)
				{
					size = cipher.ProcessBlock(bufBlock, 0, outBytes, outOff);

					mac.BlockUpdate(outBytes, outOff, blockSize);
				}
				else
				{
					mac.BlockUpdate(bufBlock, 0, blockSize);

					size = cipher.ProcessBlock(bufBlock, 0, outBytes, outOff);
				}

				bufOff = blockSize;
				Array.Copy(bufBlock, blockSize, bufBlock, 0, blockSize);

				return size;
			}

			return 0;
		}

		private bool verifyMac(byte[] mac, int off)
		{
			for (int i = 0; i < macSize; i++)
			{
				if (macBlock[i] != mac[off + i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
