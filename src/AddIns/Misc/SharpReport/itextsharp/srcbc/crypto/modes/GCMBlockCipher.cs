using System;

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes
{
	/// <summary>
	/// Implements the Galois/Counter mode (GCM) detailed in
	/// NIST Special Publication 800-38D.
	/// </summary>
	public class GcmBlockCipher
		: IAeadBlockCipher
	{
		private const int					BlockSize = 16;
		private static readonly byte[]		Zeroes = new byte[BlockSize];
		private static readonly BigInteger	R = new BigInteger("11100001", 2).ShiftLeft(120);

		private readonly IBlockCipher cipher;

		// These fields are set by Init and not modified by processing
		private bool				forEncryption;
		private int                 macSize;
		private byte[]              nonce;
		private byte[]              A;
		private KeyParameter        keyParam;
	//    private int                 tagLength;
		private BigInteger          H;
		private BigInteger          initS;
		private byte[]              J0;

		// These fields are modified during processing
		private byte[]		bufBlock;
		private byte[]		macBlock;
		private BigInteger  S;
		private byte[]      counter;
		private int         bufOff;
		private long        totalLength;

		// Debug variables
	//    private int nCount, xCount, yCount;

		public GcmBlockCipher(
			IBlockCipher c)
		{
			if (c.GetBlockSize() != BlockSize)
				throw new ArgumentException("cipher required with a block size of " + BlockSize + ".");

			this.cipher = c;
		}

		public virtual string AlgorithmName
		{
			get { return cipher.AlgorithmName + "/GCM"; }
		}

		public virtual int GetBlockSize()
		{
			return BlockSize;
		}

		public virtual void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;
			this.macSize = 16; // TODO Make configurable?
			this.macBlock = null;

			// TODO If macSize limitation is removed, be very careful about bufBlock
			int bufLength = forEncryption ? BlockSize : (BlockSize + macSize); 
			this.bufBlock = new byte[bufLength];

			if (parameters is AeadParameters)
			{
				AeadParameters param = (AeadParameters)parameters;

				nonce = param.GetNonce();
				A = param.GetAssociatedText();
	//            macSize = param.getMacSize() / 8;
				if (param.MacSize != 128)
				{
					// TODO Make configurable?
					throw new ArgumentException("only 128-bit MAC supported currently");
				}
				keyParam = param.Key;
			}
			else if (parameters is ParametersWithIV)
			{
				ParametersWithIV param = (ParametersWithIV)parameters;

				nonce = param.GetIV();
				A = null;
				keyParam = (KeyParameter)param.Parameters;
			}
			else
			{
				throw new ArgumentException("invalid parameters passed to GCM");
			}

			if (nonce == null || nonce.Length < 1)
			{
				throw new ArgumentException("IV must be at least 1 byte");
			}

			if (A == null)
			{
				// Avoid lots of null checks
				A = new byte[0];
			}

			// Cipher always used input forward mode
			cipher.Init(true, keyParam);

			// TODO This should be configurable by Init parameters
			// (but must be 16 if nonce length not 12) (BlockSize?)
	//        this.tagLength = 16;

			byte[] h = new byte[BlockSize];
			cipher.ProcessBlock(Zeroes, 0, h, 0);
			//trace("H: " + new string(Hex.encode(h)));
			this.H = new BigInteger(1, h);
			this.initS = gHASH(A, false);

			if (nonce.Length == 12)
			{
				this.J0 = new byte[16];
				Array.Copy(nonce, 0, J0, 0, nonce.Length);
				this.J0[15] = 0x01;
			}
			else
			{
				BigInteger N = gHASH(nonce, true);
				BigInteger X = BigInteger.ValueOf(nonce.Length * 8);
				//trace("len({})||len(IV): " + dumpBigInt(X));

				N = multiply(N.Xor(X), H);
				//trace("GHASH(H,{},IV): " + dumpBigInt(N));
				this.J0 = asBlock(N);
			}

			this.S = initS;
			this.counter = Arrays.Clone(J0);
			//trace("Y" + yCount + ": " + new string(Hex.encode(counter)));
			this.bufOff = 0;
			this.totalLength = 0;
		}

		public virtual byte[] GetMac()
		{
			return Arrays.Clone(macBlock);
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

		public virtual int GetUpdateOutputSize(
			int len)
		{
			return ((len + bufOff) / BlockSize) * BlockSize;
		}

		public virtual int ProcessByte(
			byte	input,
			byte[]	output,
			int		outOff)
		{
			return Process(input, output, outOff);
		}

		public virtual int ProcessBytes(
			byte[]	input,
			int		inOff,
			int		len,
			byte[]	output,
			int		outOff)
		{
			int resultLen = 0;

			for (int i = 0; i != len; i++)
			{
				resultLen += Process(input[inOff + i], output, outOff + resultLen);
			}

			return resultLen;
		}

		private int Process(
			byte	input,
			byte[]	output,
			int		outOff)
		{
			bufBlock[bufOff++] = input;

			if (bufOff == bufBlock.Length)
			{
				gCTRBlock(bufBlock, BlockSize, output, outOff);
				if (!forEncryption)
				{
					Array.Copy(bufBlock, BlockSize, bufBlock, 0, BlockSize);
				}
	//            bufOff = 0;
				bufOff = bufBlock.Length - BlockSize;
	//            return bufBlock.Length;
				return BlockSize;
			}

			return 0;
		}

		public int DoFinal(byte[] output, int outOff)
		{
			int extra = bufOff;
			if (!forEncryption)
			{
				if (extra < macSize)
					throw new InvalidCipherTextException("data too short");

				extra -= macSize;
			}

			if (extra > 0)
			{
				byte[] tmp = new byte[BlockSize];
				Array.Copy(bufBlock, 0, tmp, 0, extra);
				gCTRBlock(tmp, extra, output, outOff);
			}

			// Final gHASH
			BigInteger X = BigInteger.ValueOf(A.Length * 8).ShiftLeft(64).Add(
				BigInteger.ValueOf(totalLength * 8));
			//trace("len(A)||len(C): " + dumpBigInt(X));

			S = multiply(S.Xor(X), H);
			//trace("GHASH(H,A,C): " + dumpBigInt(S));

			// T = MSBt(GCTRk(J0,S))
			byte[] tBytes = new byte[BlockSize];
			cipher.ProcessBlock(J0, 0, tBytes, 0);
			//trace("E(K,Y0): " + new string(Hex.encode(tmp)));
			BigInteger T = S.Xor(new BigInteger(1, tBytes));

			// TODO Fix this if tagLength becomes configurable
			byte[] tag = asBlock(T);
			//trace("T: " + new string(Hex.encode(tag)));

			int resultLen = extra;

			if (forEncryption)
			{
				this.macBlock = tag;
				Array.Copy(tag, 0, output, outOff + bufOff, tag.Length);
				resultLen += tag.Length;
			}
			else
			{
				this.macBlock = new byte[macSize];
				Array.Copy(bufBlock, extra, macBlock, 0, macSize);
				if (!Arrays.AreEqual(tag, this.macBlock))
					throw new InvalidCipherTextException("mac check input GCM failed");
			}

			Reset(false);

			return resultLen;
		}

		public virtual void Reset()
		{
			Reset(true);
		}

		private void Reset(
			bool clearMac)
		{
			// Debug
	//        nCount = xCount = yCount = 0;

			S = initS;
			counter = Arrays.Clone(J0);
			bufOff = 0;
			totalLength = 0;

			if (bufBlock != null)
			{
				Array.Clear(bufBlock, 0, bufBlock.Length);
			}

			if (clearMac)
			{
				macBlock = null;
			}

			cipher.Reset();
		}

		private void gCTRBlock(byte[] buf, int bufCount, byte[] output, int outOff)
		{
			inc(counter);
			//trace("Y" + ++yCount + ": " + new string(Hex.encode(counter)));

			byte[] tmp = new byte[BlockSize];
			cipher.ProcessBlock(counter, 0, tmp, 0);
			//trace("E(K,Y" + yCount + "): " + new string(Hex.encode(tmp)));

			if (forEncryption)
			{
				Array.Copy(Zeroes, bufCount, tmp, bufCount, BlockSize - bufCount);

				for (int i = bufCount - 1; i >= 0; --i)
				{
					tmp[i] ^= buf[i];
					output[outOff + i] = tmp[i];
				}

				gHASHBlock(tmp);
			}
			else
			{
				for (int i = bufCount - 1; i >= 0; --i)
				{
					tmp[i] ^= buf[i];
					output[outOff + i] = tmp[i];
				}

				gHASHBlock(buf);
			}

			totalLength += bufCount;
		}

		private BigInteger gHASH(byte[] b, bool nonce)
		{
			//trace("" + b.Length);
			BigInteger Y = BigInteger.Zero;

			for (int pos = 0; pos < b.Length; pos += 16)
			{
				byte[] x = new byte[16];
				int num = System.Math.Min(b.Length - pos, 16);
				Array.Copy(b, pos, x, 0, num);
				BigInteger X = new BigInteger(1, x);
				Y = multiply(Y.Xor(X), H);
	//            if (nonce)
	//            {
	//                trace("N" + ++nCount + ": " + dumpBigInt(Y));
	//            }
	//            else
	//            {
	//                trace("X" + ++xCount + ": " + dumpBigInt(Y) + " (gHASH)");
	//            }
			}

			return Y;
		}

		private void gHASHBlock(byte[] block)
		{
			if (block.Length > BlockSize)
			{
				byte[] tmp = new byte[BlockSize];
				Array.Copy(block, 0, tmp, 0, BlockSize);
				block = tmp;
			}

			BigInteger X = new BigInteger(1, block);
			S = multiply(S.Xor(X), H);
			//trace("X" + ++xCount + ": " + dumpBigInt(S) + " (gHASHBlock)");
		}

		private static void inc(byte[] block)
		{
	//        assert block.Length == 16;

			for (int i = 15; i >= 12; --i)
			{
				byte b = (byte)((block[i] + 1) & 0xff);
				block[i] = b;

				if (b != 0)
				{
					break;
				}
			}
		}

		private BigInteger multiply(
			BigInteger	X,
			BigInteger	Y)
		{
			BigInteger Z = BigInteger.Zero;
			BigInteger V = X;

			for (int i = 0; i < 128; ++i)
			{
				if (Y.TestBit(127 - i))
				{
					Z = Z.Xor(V);
				}

				bool lsb = V.TestBit(0);
				V = V.ShiftRight(1);
				if (lsb)
				{
					V = V.Xor(R);
				}
			}

			return Z;
		}

		private byte[] asBlock(
			BigInteger bi)
		{
			byte[] b = BigIntegers.AsUnsignedByteArray(bi);
			if (b.Length < 16)
			{
				byte[] tmp = new byte[16];
				Array.Copy(b, 0, tmp, tmp.Length - b.Length, b.Length);
				b = tmp;
			}
			return b;
		}

	//    private string dumpBigInt(BigInteger bi)
	//    {
	//        byte[] b = asBlock(bi);
	//
	//        return new string(Hex.encode(b));         
	//    }
	//
	//    private void trace(string msg)
	//    {
	//        System.err.println(msg);
	//    }
	}
}