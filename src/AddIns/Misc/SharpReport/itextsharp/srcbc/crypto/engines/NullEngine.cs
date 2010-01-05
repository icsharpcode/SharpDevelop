using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* The no-op engine that just copies bytes through, irrespective of whether encrypting and decrypting.
	* Provided for the sake of completeness.
	*/
	public class NullEngine
		: IBlockCipher
	{
		private bool initialised;
		private const int BlockSize = 1;

		public NullEngine()
		{
		}

		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			// we don't mind any parameters that may come in
			initialised = true;
		}

		public string AlgorithmName
		{
			get { return "Null"; }
		}

		public bool IsPartialBlockOkay
		{
			get { return true; }
		}

		public int GetBlockSize()
		{
			return BlockSize;
		}

		public int ProcessBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			if (!initialised)
				throw new InvalidOperationException("Null engine not initialised");
			if ((inOff + BlockSize) > input.Length)
				throw new DataLengthException("input buffer too short");
			if ((outOff + BlockSize) > output.Length)
				throw new DataLengthException("output buffer too short");

			for (int i = 0; i < BlockSize; ++i)
			{
				output[outOff + i] = input[inOff + i];
			}

			return BlockSize;
		}

		public void Reset()
		{
			// nothing needs to be done
		}
	}
}
