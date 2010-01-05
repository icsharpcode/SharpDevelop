using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Modes
{
	/**
	* Implements the Segmented Integer Counter (SIC) mode on top of a simple
	* block cipher.
	*/
	public class SicBlockCipher
		: IBlockCipher
	{
		private readonly IBlockCipher cipher;
		private readonly int blockSize;
		private readonly byte[] IV;
		private readonly byte[] counter;
		private readonly byte[] counterOut;

		/**
		* Basic constructor.
		*
		* @param c the block cipher to be used.
		*/
		public SicBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			this.blockSize = cipher.GetBlockSize();
			this.IV = new byte[blockSize];
			this.counter = new byte[blockSize];
			this.counterOut = new byte[blockSize];
		}

		/**
		* return the underlying block cipher that we are wrapping.
		*
		* @return the underlying block cipher that we are wrapping.
		*/
		public IBlockCipher GetUnderlyingCipher()
		{
			return cipher;
		}

		public void Init(
			bool				forEncryption, //ignored by this CTR mode
			ICipherParameters	parameters)
		{
			if (parameters is ParametersWithIV)
			{
				ParametersWithIV ivParam = (ParametersWithIV) parameters;
				byte[] iv = ivParam.GetIV();
				Array.Copy(iv, 0, IV, 0, IV.Length);

				Reset();
				cipher.Init(true, ivParam.Parameters);
			}
		}

		public string AlgorithmName
		{
			get { return cipher.AlgorithmName + "/SIC"; }
		}

		public bool IsPartialBlockOkay
		{
			get { return true; }
		}

		public int GetBlockSize()
		{
			return cipher.GetBlockSize();
		}

		public int ProcessBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			cipher.ProcessBlock(counter, 0, counterOut, 0);

			//
			// XOR the counterOut with the plaintext producing the cipher text
			//
			for (int i = 0; i < counterOut.Length; i++)
			{
				output[outOff + i] = (byte)(counterOut[i] ^ input[inOff + i]);
			}

			// Increment the counter
			int j = counter.Length;
			while (--j >= 0 && ++counter[j] == 0)
			{
			}

			return counter.Length;
		}

		public void Reset()
		{
			Array.Copy(IV, 0, counter, 0, counter.Length);
			cipher.Reset();
		}
	}
}
