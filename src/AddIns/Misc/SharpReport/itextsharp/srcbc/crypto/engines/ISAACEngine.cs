using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* Implementation of Bob Jenkin's ISAAC (Indirection Shift Accumulate Add and Count).
	* see: http://www.burtleburtle.net/bob/rand/isaacafa.html
	*/
	public class IsaacEngine
		: IStreamCipher
	{
		// Constants
		private static readonly int sizeL          = 8,
									stateArraySize = sizeL<<5; // 256

		// Cipher's internal state
		private uint[]   engineState   = null, // mm                
						results       = null; // randrsl
		private uint     a = 0, b = 0, c = 0;

		// Engine state
		private int     index         = 0;
		private byte[]  keyStream     = new byte[stateArraySize<<2], // results expanded into bytes
						workingKey    = null;
		private bool	initialised   = false;

		/**
		* initialise an ISAAC cipher.
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
				throw new ArgumentException(
					"invalid parameter passed to ISAAC Init - " + parameters.GetType().Name,
					"parameters");

			/* 
			* ISAAC encryption and decryption is completely
			* symmetrical, so the 'forEncryption' is 
			* irrelevant.
			*/
			KeyParameter p = (KeyParameter) parameters;
			setKey(p.GetKey());
		}

		public byte ReturnByte(
			byte input)
		{
			if (index == 0) 
			{
				isaac();
				keyStream = intToByteLittle(results);
			}

			byte output = (byte)(keyStream[index]^input);
			index = (index + 1) & 1023;

			return output;
		}

		public void ProcessBytes(
			byte[]	input, 
			int		inOff, 
			int		len, 
			byte[]	output, 
			int		outOff)
		{
			if (!initialised)
				throw new InvalidOperationException(AlgorithmName + " not initialised");
			if ((inOff + len) > input.Length)
				throw new DataLengthException("input buffer too short");
			if ((outOff + len) > output.Length)
				throw new DataLengthException("output buffer too short");

			for (int i = 0; i < len; i++)
			{
				if (index == 0) 
				{
					isaac();
					keyStream = intToByteLittle(results);
				}
				output[i+outOff] = (byte)(keyStream[index]^input[i+inOff]);
				index = (index + 1) & 1023;
			}
		}

		public string AlgorithmName
		{
			get { return "ISAAC"; }
		}

		public void Reset()
		{
			setKey(workingKey);
		}

		// Private implementation
		private void setKey(
			byte[] keyBytes)
		{
			workingKey = keyBytes;

			if (engineState == null)
			{
				engineState = new uint[stateArraySize];
			}

			if (results == null)
			{
				results = new uint[stateArraySize];
			}

			int i, j, k;

			// Reset state
			for (i = 0; i < stateArraySize; i++)
			{
				engineState[i] = results[i] = 0;
			}
			a = b = c = 0;

			// Reset index counter for output
			index = 0;

			// Convert the key bytes to ints and put them into results[] for initialization
			byte[] t = new byte[keyBytes.Length + (keyBytes.Length & 3)];
			Array.Copy(keyBytes, 0, t, 0, keyBytes.Length);
			for (i = 0; i < t.Length; i+=4)
			{
				results[i>>2] = byteToIntLittle(t, i);
			}

			// It has begun?
			uint[] abcdefgh = new uint[sizeL];

			for (i = 0; i < sizeL; i++)
			{
				abcdefgh[i] = 0x9e3779b9; // Phi (golden ratio)
			}

			for (i = 0; i < 4; i++)
			{
				mix(abcdefgh);
			}

			for (i = 0; i < 2; i++)
			{
				for (j = 0; j < stateArraySize; j+=sizeL)
				{
					for (k = 0; k < sizeL; k++)
					{
						abcdefgh[k] += (i<1) ? results[j+k] : engineState[j+k];
					}

					mix(abcdefgh);

					for (k = 0; k < sizeL; k++)
					{
						engineState[j+k] = abcdefgh[k];
					}
				}
			}

			isaac();

			initialised = true;
		}    

		private void isaac()
		{
			uint x, y;

			b += ++c;
			for (int i = 0; i < stateArraySize; i++)
			{
				x = engineState[i];
				switch (i & 3)
				{
					case 0: a ^= (a << 13); break;
					case 1: a ^= (a >>  6); break;
					case 2: a ^= (a <<  2); break;
					case 3: a ^= (a >> 16); break;
				}
				a += engineState[(i+128) & 0xFF];
				engineState[i] = y = engineState[(int)((uint)x >> 2) & 0xFF] + a + b;
				results[i] = b = engineState[(int)((uint)y >> 10) & 0xFF] + x;
			}
		}

		private void mix(uint[] x)
		{
//			x[0]^=x[1]<< 11; x[3]+=x[0]; x[1]+=x[2];
//			x[1]^=x[2]>>> 2; x[4]+=x[1]; x[2]+=x[3];
//			x[2]^=x[3]<<  8; x[5]+=x[2]; x[3]+=x[4];
//			x[3]^=x[4]>>>16; x[6]+=x[3]; x[4]+=x[5];
//			x[4]^=x[5]<< 10; x[7]+=x[4]; x[5]+=x[6];
//			x[5]^=x[6]>>> 4; x[0]+=x[5]; x[6]+=x[7];
//			x[6]^=x[7]<<  8; x[1]+=x[6]; x[7]+=x[0];
//			x[7]^=x[0]>>> 9; x[2]+=x[7]; x[0]+=x[1];
			x[0]^=x[1]<< 11; x[3]+=x[0]; x[1]+=x[2];
			x[1]^=x[2]>>  2; x[4]+=x[1]; x[2]+=x[3];
			x[2]^=x[3]<<  8; x[5]+=x[2]; x[3]+=x[4];
			x[3]^=x[4]>> 16; x[6]+=x[3]; x[4]+=x[5];
			x[4]^=x[5]<< 10; x[7]+=x[4]; x[5]+=x[6];
			x[5]^=x[6]>>  4; x[0]+=x[5]; x[6]+=x[7];
			x[6]^=x[7]<<  8; x[1]+=x[6]; x[7]+=x[0];
			x[7]^=x[0]>>  9; x[2]+=x[7]; x[0]+=x[1];
		}

		private uint byteToIntLittle(
			byte[]	x,
			int		offset)
		{
			uint result = (byte) x[offset + 3];
			result = (result << 8) | x[offset + 2];
			result = (result << 8) | x[offset + 1];
			result = (result << 8) | x[offset + 0];
			return result;
		}

		private byte[] intToByteLittle(
			uint x)
		{
			byte[] output = new byte[4];
			output[3] = (byte)x;
			output[2] = (byte)(x >> 8);
			output[1] = (byte)(x >> 16);
			output[0] = (byte)(x >> 24);
			return output;
		} 

		private byte[] intToByteLittle(
			uint[] x)
		{
			byte[] output = new byte[4*x.Length];
			for (int i = 0, j = 0; i < x.Length; i++,j+=4)
			{
				Array.Copy(intToByteLittle(x[i]), 0, output, j, 4);
			}
			return output;
		}
	}
}
