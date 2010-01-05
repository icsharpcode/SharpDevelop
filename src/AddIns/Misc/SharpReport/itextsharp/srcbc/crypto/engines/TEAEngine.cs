using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* An TEA engine.
	*/
	public class TeaEngine
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
		private int _a, _b, _c, _d;
		private bool _initialised;
		private bool _forEncryption;

		/**
		* Create an instance of the TEA encryption algorithm
		* and set some defaults
		*/
		public TeaEngine()
		{
			_initialised = false;
		}

		public string AlgorithmName
		{
			get { return "TEA"; }
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
			byte[]  inBytes,
			int     inOff,
			byte[]  outBytes,
			int     outOff)
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
			_a = bytesToInt(key, 0);
			_b = bytesToInt(key, 4);
			_c = bytesToInt(key, 8);
			_d = bytesToInt(key, 12);
		}

		private int encryptBlock(
			byte[]	inBytes,
			int		inOff,
			byte[]	outBytes,
			int		outOff)
		{
			// Pack bytes into integers
			int v0 = bytesToInt(inBytes, inOff);
			int v1 = bytesToInt(inBytes, inOff + 4);
	        
			int sum = 0;
	        
			for (int i = 0; i != rounds; i++)
			{
				sum += delta;
//				v0  += ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >>> 5) + _b);
				v0  += ((v1 << 4) + _a) ^ (v1 + sum) ^ ((int)((uint)v1 >> 5) + _b);
//				v1  += ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >>> 5) + _d);
				v1  += ((v0 << 4) + _c) ^ (v0 + sum) ^ ((int)((uint)v0 >> 5) + _d);
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
//				v1  -= ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >>> 5) + _d);
				v1  -= ((v0 << 4) + _c) ^ (v0 + sum) ^ ((int)((uint)v0 >> 5) + _d);
//				v0  -= ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >>> 5) + _b);
				v0  -= ((v1 << 4) + _a) ^ (v1 + sum) ^ ((int)((uint)v1 >> 5) + _b);
				sum -= delta;
			}
	        
			unpackInt(v0, outBytes, outOff);
			unpackInt(v1, outBytes, outOff + 4);
	        
			return block_size;
		}

		private int bytesToInt(
			byte[]	b,
			int		inOff)
		{
			return ((b[inOff++]) << 24)
				|	((b[inOff++] & 255) << 16)
				|	((b[inOff++] & 255) <<  8)
				|	((b[inOff] & 255));
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
