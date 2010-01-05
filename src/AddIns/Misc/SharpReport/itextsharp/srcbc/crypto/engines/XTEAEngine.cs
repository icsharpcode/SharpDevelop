using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* An XTEA engine.
	*/
	public class XteaEngine
		: IBlockCipher
	{
		private const int
			rounds		= 32,
			block_size	= 8,
			key_size	= 16,
			delta		= unchecked((int) 0x9E3779B9),
			d_sum		= unchecked((int) 0xC6EF3720); // sum on decrypt

		/*
		* the expanded key array of 4 subkeys
		*/
		private int[] _S = new int[4];
		private bool _initialised;
		private bool _forEncryption;

		/**
		* Create an instance of the TEA encryption algorithm
		* and set some defaults
		*/
		public XteaEngine()
		{
			_initialised = false;
		}

		public string AlgorithmName
		{
			get { return "XTEA"; }
		}

		public bool IsPartialBlockOkay
		{
			get { return false; }
		}

		public int GetBlockSize()
		{
			return block_size;
		}

		/**
		* initialise
		*
		* @param forEncryption whether or not we are for encryption.
		* @param params the parameters required to set up the cipher.
		* @exception ArgumentException if the params argument is
		* inappropriate.
		*/
		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to TEA init - "
					+ parameters.GetType().FullName);
			}

			_forEncryption = forEncryption;
			_initialised = true;

			KeyParameter p = (KeyParameter) parameters;

			setKey(p.GetKey());
		}

		public int ProcessBlock(
			byte[]	inBytes,
			int		inOff,
			byte[]	outBytes,
			int		outOff)
		{
			if (!_initialised)
				throw new InvalidOperationException(AlgorithmName + " not initialised");

			if ((inOff + block_size) > inBytes.Length)
				throw new DataLengthException("input buffer too short");

			if ((outOff + block_size) > outBytes.Length)
				throw new DataLengthException("output buffer too short");

			return _forEncryption
				?	encryptBlock(inBytes, inOff, outBytes, outOff)
				:	decryptBlock(inBytes, inOff, outBytes, outOff);
		}

		public void Reset()
		{
		}

		/**
		* Re-key the cipher.
		*
		* @param  key  the key to be used
		*/
		private void setKey(
			byte[] key)
		{
			_S[0] = bytesToInt(key, 0);
			_S[1] = bytesToInt(key, 4);
			_S[2] = bytesToInt(key, 8);
			_S[3] = bytesToInt(key, 12);
		}

		private int encryptBlock(
			byte[]  inBytes,
			int     inOff,
			byte[]  outBytes,
			int     outOff)
		{
			// Pack bytes into integers
			int v0 = bytesToInt(inBytes, inOff);
			int v1 = bytesToInt(inBytes, inOff + 4);

			int sum = 0;

			for (int i = 0; i != rounds; i++)
			{
				v0    += ((v1 << 4 ^ (int)((uint)v1 >> 5)) + v1) ^ (sum + _S[sum & 3]);
				sum += delta;
				v1    += ((v0 << 4 ^ (int)((uint)v0 >> 5)) + v0) ^ (sum + _S[(int)((uint)sum >> 11) & 3]);
			}

			unpackInt(v0, outBytes, outOff);
			unpackInt(v1, outBytes, outOff + 4);

			return block_size;
		}

		private int decryptBlock(
			byte[]	inBytes,
			int		inOff,
			byte[]	outBytes,
			int		outOff)
		{
			// Pack bytes into integers
			int v0 = bytesToInt(inBytes, inOff);
			int v1 = bytesToInt(inBytes, inOff + 4);

			int sum = d_sum;

			for (int i = 0; i != rounds; i++)
			{
				v1  -= ((v0 << 4 ^ (int)((uint)v0 >> 5)) + v0) ^ (sum + _S[(int)((uint)sum >> 11) & 3]);
				sum -= delta;
				v0  -= ((v1 << 4 ^ (int)((uint)v1 >> 5)) + v1) ^ (sum + _S[sum & 3]);
			}

			unpackInt(v0, outBytes, outOff);
			unpackInt(v1, outBytes, outOff + 4);

			return block_size;
		}

		private int bytesToInt(byte[] b, int inOff)
		{
			return ((b[inOff++]) << 24) |
					((b[inOff++] & 255) << 16) |
					((b[inOff++] & 255) <<  8) |
					((b[inOff] & 255));
		}

		private void unpackInt(
			int		v,
			byte[]	b,
			int		outOff)
		{
			uint uv = (uint) v;
			b[outOff++] = (byte)(uv >> 24);
			b[outOff++] = (byte)(uv >> 16);
			b[outOff++] = (byte)(uv >>  8);
			b[outOff  ] = (byte)uv;
		}
	}
}
