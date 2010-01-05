using System;
using System.Collections;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* NaccacheStern Engine. For details on this cipher, please see
	* http://www.gemplus.com/smart/rd/publications/pdf/NS98pkcs.pdf
	*/
	public class NaccacheSternEngine
		: IAsymmetricBlockCipher
	{
		private bool forEncryption;

		private NaccacheSternKeyParameters key;

		private ArrayList[] lookup = null;

		private bool debug = false;

		public string AlgorithmName
		{
			get { return "NaccacheStern"; }
		}

		/**
		* Initializes this algorithm. Must be called before all other Functions.
		*
		* @see org.bouncycastle.crypto.AsymmetricBlockCipher#init(bool,
		*      org.bouncycastle.crypto.CipherParameters)
		*/
		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;

			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom) parameters).Parameters;
			}

			key = (NaccacheSternKeyParameters)parameters;

			// construct lookup table for faster decryption if necessary
			if (!this.forEncryption)
			{
				if (debug)
				{
					Console.WriteLine("Constructing lookup Array");
				}
				NaccacheSternPrivateKeyParameters priv = (NaccacheSternPrivateKeyParameters)key;
				ArrayList primes = priv.SmallPrimes;
				lookup = new ArrayList[primes.Count];
				for (int i = 0; i < primes.Count; i++)
				{
					BigInteger actualPrime = (BigInteger) primes[i];
					int actualPrimeValue = actualPrime.IntValue;

					lookup[i] = new ArrayList(actualPrimeValue);
					lookup[i].Add(BigInteger.One);

					if (debug)
					{
						Console.WriteLine("Constructing lookup ArrayList for " + actualPrimeValue);
					}

					BigInteger accJ = BigInteger.Zero;

					for (int j = 1; j < actualPrimeValue; j++)
					{
//						BigInteger bigJ = BigInteger.ValueOf(j);
//						accJ = priv.PhiN.Multiply(bigJ);
						accJ = accJ.Add(priv.PhiN);
						BigInteger comp = accJ.Divide(actualPrime);
						lookup[i].Add(priv.G.ModPow(comp, priv.Modulus));
					}
				}
			}
		}

		public bool Debug
		{
			set { this.debug = value; }
		}

		/**
		* Returns the input block size of this algorithm.
		*
		* @see org.bouncycastle.crypto.AsymmetricBlockCipher#GetInputBlockSize()
		*/
		public int GetInputBlockSize()
		{
			if (forEncryption)
			{
				// We can only encrypt values up to lowerSigmaBound
				return (key.LowerSigmaBound + 7) / 8 - 1;
			}
			else
			{
				// We pad to modulus-size bytes for easier decryption.
//				return key.Modulus.ToByteArray().Length;
				return key.Modulus.BitLength / 8 + 1;
			}
		}

		/**
		* Returns the output block size of this algorithm.
		*
		* @see org.bouncycastle.crypto.AsymmetricBlockCipher#GetOutputBlockSize()
		*/
		public int GetOutputBlockSize()
		{
			if (forEncryption)
			{
				// encrypted Data is always padded up to modulus size
//				return key.Modulus.ToByteArray().Length;
				return key.Modulus.BitLength / 8 + 1;
			}
			else
			{
				// decrypted Data has upper limit lowerSigmaBound
				return (key.LowerSigmaBound + 7) / 8 - 1;
			}
		}

		/**
		* Process a single Block using the Naccache-Stern algorithm.
		*
		* @see org.bouncycastle.crypto.AsymmetricBlockCipher#ProcessBlock(byte[],
		*      int, int)
		*/
		public byte[] ProcessBlock(
			byte[]	inBytes,
			int		inOff,
			int		length)
		{
			if (key == null)
				throw new InvalidOperationException("NaccacheStern engine not initialised");
			if (length > (GetInputBlockSize() + 1))
				throw new DataLengthException("input too large for Naccache-Stern cipher.\n");

			if (!forEncryption)
			{
				// At decryption make sure that we receive padded data blocks
				if (length < GetInputBlockSize())
				{
					throw new InvalidCipherTextException("BlockLength does not match modulus for Naccache-Stern cipher.\n");
				}
			}

			// transform input into BigInteger
			BigInteger input = new BigInteger(1, inBytes, inOff, length);

			if (debug)
			{
				Console.WriteLine("input as BigInteger: " + input);
			}

			byte[] output;
			if (forEncryption)
			{
				output = Encrypt(input);
			}
			else
			{
				ArrayList plain = new ArrayList();
				NaccacheSternPrivateKeyParameters priv = (NaccacheSternPrivateKeyParameters)key;
				ArrayList primes = priv.SmallPrimes;
				// Get Chinese Remainders of CipherText
				for (int i = 0; i < primes.Count; i++)
				{
					BigInteger exp = input.ModPow(priv.PhiN.Divide((BigInteger)primes[i]), priv.Modulus);
					ArrayList al = lookup[i];
					if (lookup[i].Count != ((BigInteger)primes[i]).IntValue)
					{
						if (debug)
						{
							Console.WriteLine("Prime is " + primes[i] + ", lookup table has size " + al.Count);
						}
						throw new InvalidCipherTextException("Error in lookup Array for "
										+ ((BigInteger)primes[i]).IntValue
										+ ": Size mismatch. Expected ArrayList with length "
										+ ((BigInteger)primes[i]).IntValue + " but found ArrayList of length "
										+ lookup[i].Count);
					}
					int lookedup = al.IndexOf(exp);

					if (lookedup == -1)
					{
						if (debug)
						{
							Console.WriteLine("Actual prime is " + primes[i]);
							Console.WriteLine("Decrypted value is " + exp);

							Console.WriteLine("LookupList for " + primes[i] + " with size " + lookup[i].Count
											+ " is: ");
							for (int j = 0; j < lookup[i].Count; j++)
							{
								Console.WriteLine(lookup[i][j]);
							}
						}
						throw new InvalidCipherTextException("Lookup failed");
					}
					plain.Add(BigInteger.ValueOf(lookedup));
				}
				BigInteger test = chineseRemainder(plain, primes);

				// Should not be used as an oracle, so reencrypt output to see
				// if it corresponds to input

				// this breaks probabilisic encryption, so disable it. Anyway, we do
				// use the first n primes for key generation, so it is pretty easy
				// to guess them. But as stated in the paper, this is not a security
				// breach. So we can just work with the correct sigma.

				// if (debug) {
				//      Console.WriteLine("Decryption is " + test);
				// }
				// if ((key.G.ModPow(test, key.Modulus)).Equals(input)) {
				//      output = test.ToByteArray();
				// } else {
				//      if(debug){
				//          Console.WriteLine("Engine seems to be used as an oracle,
				//          returning null");
				//      }
				//      output = null;
				// }

				output = test.ToByteArray();
			}

			return output;
		}

		/**
		* Encrypts a BigInteger aka Plaintext with the public key.
		*
		* @param plain
		*            The BigInteger to encrypt
		* @return The byte[] representation of the encrypted BigInteger (i.e.
		*         crypted.toByteArray())
		*/
		public byte[] Encrypt(
			BigInteger plain)
		{
			// Always return modulus size values 0-padded at the beginning
			// 0-padding at the beginning is correctly parsed by BigInteger :)
//			byte[] output = key.Modulus.ToByteArray();
//			Array.Clear(output, 0, output.Length);
			byte[] output = new byte[key.Modulus.BitLength / 8 + 1];

			byte[] tmp = key.G.ModPow(plain, key.Modulus).ToByteArray();
			Array.Copy(tmp, 0, output, output.Length - tmp.Length, tmp.Length);
			if (debug)
			{
				Console.WriteLine("Encrypted value is:  " + new BigInteger(output));
			}
			return output;
		}

		/**
		* Adds the contents of two encrypted blocks mod sigma
		*
		* @param block1
		*            the first encrypted block
		* @param block2
		*            the second encrypted block
		* @return encrypt((block1 + block2) mod sigma)
		* @throws InvalidCipherTextException
		*/
		public byte[] AddCryptedBlocks(
			byte[] block1,
			byte[] block2)
		{
			// check for correct blocksize
			if (forEncryption)
			{
				if ((block1.Length > GetOutputBlockSize())
						|| (block2.Length > GetOutputBlockSize()))
				{
					throw new InvalidCipherTextException(
							"BlockLength too large for simple addition.\n");
				}
			}
			else
			{
				if ((block1.Length > GetInputBlockSize())
						|| (block2.Length > GetInputBlockSize()))
				{
					throw new InvalidCipherTextException(
							"BlockLength too large for simple addition.\n");
				}
			}

			// calculate resulting block
			BigInteger m1Crypt = new BigInteger(1, block1);
			BigInteger m2Crypt = new BigInteger(1, block2);
			BigInteger m1m2Crypt = m1Crypt.Multiply(m2Crypt);
			m1m2Crypt = m1m2Crypt.Mod(key.Modulus);
			if (debug)
			{
				Console.WriteLine("c(m1) as BigInteger:....... " + m1Crypt);
				Console.WriteLine("c(m2) as BigInteger:....... " + m2Crypt);
				Console.WriteLine("c(m1)*c(m2)%n = c(m1+m2)%n: " + m1m2Crypt);
			}

			//byte[] output = key.Modulus.ToByteArray();
			//Array.Clear(output, 0, output.Length);
			byte[] output = new byte[key.Modulus.BitLength / 8 + 1];

			byte[] m1m2CryptBytes = m1m2Crypt.ToByteArray();
			Array.Copy(m1m2CryptBytes, 0, output,
				output.Length - m1m2CryptBytes.Length, m1m2CryptBytes.Length);

			return output;
		}

		/**
		* Convenience Method for data exchange with the cipher.
		*
		* Determines blocksize and splits data to blocksize.
		*
		* @param data the data to be processed
		* @return the data after it went through the NaccacheSternEngine.
		* @throws InvalidCipherTextException
		*/
		public byte[] ProcessData(
			byte[] data)
		{
			if (debug)
			{
				Console.WriteLine();
			}
			if (data.Length > GetInputBlockSize())
			{
				int inBlocksize = GetInputBlockSize();
				int outBlocksize = GetOutputBlockSize();
				if (debug)
				{
					Console.WriteLine("Input blocksize is:  " + inBlocksize + " bytes");
					Console.WriteLine("Output blocksize is: " + outBlocksize + " bytes");
					Console.WriteLine("Data has length:.... " + data.Length + " bytes");
				}
				int datapos = 0;
				int retpos = 0;
				byte[] retval = new byte[(data.Length / inBlocksize + 1) * outBlocksize];
				while (datapos < data.Length)
				{
					byte[] tmp;
					if (datapos + inBlocksize < data.Length)
					{
						tmp = ProcessBlock(data, datapos, inBlocksize);
						datapos += inBlocksize;
					}
					else
					{
						tmp = ProcessBlock(data, datapos, data.Length - datapos);
						datapos += data.Length - datapos;
					}
					if (debug)
					{
						Console.WriteLine("new datapos is " + datapos);
					}
					if (tmp != null)
					{
						tmp.CopyTo(retval, retpos);
						retpos += tmp.Length;
					}
					else
					{
						if (debug)
						{
							Console.WriteLine("cipher returned null");
						}
						throw new InvalidCipherTextException("cipher returned null");
					}
				}
				byte[] ret = new byte[retpos];
				Array.Copy(retval, 0, ret, 0, retpos);
				if (debug)
				{
					Console.WriteLine("returning " + ret.Length + " bytes");
				}
				return ret;
			}
			else
			{
				if (debug)
				{
					Console.WriteLine("data size is less then input block size, processing directly");
				}
				return ProcessBlock(data, 0, data.Length);
			}
		}

		/**
		* Computes the integer x that is expressed through the given primes and the
		* congruences with the chinese remainder theorem (CRT).
		*
		* @param congruences
		*            the congruences c_i
		* @param primes
		*            the primes p_i
		* @return an integer x for that x % p_i == c_i
		*/
		private static BigInteger chineseRemainder(ArrayList congruences, ArrayList primes)
		{
			BigInteger retval = BigInteger.Zero;
			BigInteger all = BigInteger.One;
			for (int i = 0; i < primes.Count; i++)
			{
				all = all.Multiply((BigInteger)primes[i]);
			}
			for (int i = 0; i < primes.Count; i++)
			{
				BigInteger a = (BigInteger)primes[i];
				BigInteger b = all.Divide(a);
				BigInteger b_ = b.ModInverse(a);
				BigInteger tmp = b.Multiply(b_);
				tmp = tmp.Multiply((BigInteger)congruences[i]);
				retval = retval.Add(tmp);
			}

			return retval.Mod(all);
		}
	}
}
