using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Encodings
{
	/**
	* this does your basic Pkcs 1 v1.5 padding - whether or not you should be using this
	* depends on your application - see Pkcs1 Version 2 for details.
	*/
	public class Pkcs1Encoding
		: IAsymmetricBlockCipher
	{
		/**
		 * some providers fail to include the leading zero in PKCS1 encoded blocks. If you need to
		 * work with one of these set the system property Org.BouncyCastle.Pkcs1.Strict to false.
		 */
		public const string StrictLengthEnabledProperty = "Org.BouncyCastle.Pkcs1.Strict";

		private const int HeaderLength = 10;

		/**
		 * The same effect can be achieved by setting the static property directly
		 * <p>
		 * The static property is checked during construction of the encoding object, it is set to
		 * true by default.
		 * </p>
		 */
		public static bool StrictLengthEnabled
		{
			get { return strictLengthEnabled[0]; }
			set { strictLengthEnabled[0] = value; }
		}

		private static readonly bool[] strictLengthEnabled;

		static Pkcs1Encoding()
		{
			string strictProperty = Platform.GetEnvironmentVariable(StrictLengthEnabledProperty);

			strictLengthEnabled = new bool[]{ strictProperty == null || strictProperty.Equals("true")};
		}


		private SecureRandom			random;
		private IAsymmetricBlockCipher	engine;
		private bool					forEncryption;
		private bool					forPrivateKey;
		private bool					useStrictLength;

		/**
		 * Basic constructor.
		 * @param cipher
		 */
		public Pkcs1Encoding(
			IAsymmetricBlockCipher cipher)
		{
			this.engine = cipher;
			this.useStrictLength = StrictLengthEnabled;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return engine;
		}

		public string AlgorithmName
		{
			get { return engine.AlgorithmName + "/PKCS1Padding"; }
		}

		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			AsymmetricKeyParameter kParam;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom rParam = (ParametersWithRandom)parameters;

				this.random = rParam.Random;
				kParam = (AsymmetricKeyParameter)rParam.Parameters;
			}
			else
			{
				this.random = new SecureRandom();
				kParam = (AsymmetricKeyParameter)parameters;
			}

			engine.Init(forEncryption, parameters);

			this.forPrivateKey = kParam.IsPrivate;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int baseBlockSize = engine.GetInputBlockSize();

			return forEncryption
				?	baseBlockSize - HeaderLength
				:	baseBlockSize;
		}

		public int GetOutputBlockSize()
		{
			int baseBlockSize = engine.GetOutputBlockSize();

			return forEncryption
				?	baseBlockSize
				:	baseBlockSize - HeaderLength;
		}

		public byte[] ProcessBlock(
			byte[]	input,
			int		inOff,
			int		length)
		{
			return forEncryption
				?	EncodeBlock(input, inOff, length)
				:	DecodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(
			byte[]	input,
			int		inOff,
			int		inLen)
		{
			byte[] block = new byte[engine.GetInputBlockSize()];

			if (forPrivateKey)
			{
				block[0] = 0x01;                        // type code 1

				for (int i = 1; i != block.Length - inLen - 1; i++)
				{
					block[i] = (byte)0xFF;
				}
			}
			else
			{
				random.NextBytes(block);                // random fill

				block[0] = 0x02;                        // type code 2

				//
				// a zero byte marks the end of the padding, so all
				// the pad bytes must be non-zero.
				//
				for (int i = 1; i != block.Length - inLen - 1; i++)
				{
					while (block[i] == 0)
					{
						block[i] = (byte)random.NextInt();
					}
				}
			}

			block[block.Length - inLen - 1] = 0x00;       // mark the end of the padding
			Array.Copy(input, inOff, block, block.Length - inLen, inLen);

			return engine.ProcessBlock(block, 0, block.Length);
		}

		/**
		* @exception InvalidCipherTextException if the decrypted block is not in Pkcs1 format.
		*/
		private byte[] DecodeBlock(
			byte[]	input,
			int		inOff,
			int		inLen)
		{
			byte[] block = engine.ProcessBlock(input, inOff, inLen);

			if (block.Length < GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}

			byte type = block[0];

			if (type != 1 && type != 2)
			{
				throw new InvalidCipherTextException("unknown block type");
			}

			if (useStrictLength && block.Length != engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}

			//
			// find and extract the message block.
			//
			int start;
			for (start = 1; start != block.Length; start++)
			{
				byte pad = block[start];

				if (pad == 0)
				{
					break;
				}

				if (type == 1 && pad != (byte)0xff)
				{
					throw new InvalidCipherTextException("block padding incorrect");
				}
			}

			start++;           // data should start at the next byte

			if (start >= block.Length || start < HeaderLength)
			{
				throw new InvalidCipherTextException("no data in block");
			}

			byte[] result = new byte[block.Length - start];

			Array.Copy(block, start, result, 0, result.Length);

			return result;
		}
	}

}
