using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/// <remarks>A class that provides a basic DES engine.</remarks>
    public class DesEngine
		: IBlockCipher
    {
        internal const int BLOCK_SIZE = 8;

		private int[] workingKey;

        public virtual int[] GetWorkingKey()
		{
			return workingKey;
		}

		/**
        * initialise a DES cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public virtual void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
            if (!(parameters is KeyParameter))
				throw new ArgumentException("invalid parameter passed to DES init - " + parameters.GetType().ToString());

			workingKey = GenerateWorkingKey(forEncryption, ((KeyParameter)parameters).GetKey());
        }

		public virtual string AlgorithmName
        {
            get { return "DES"; }
        }

		public bool IsPartialBlockOkay
		{
			get { return false; }
		}

		public virtual int GetBlockSize()
        {
            return BLOCK_SIZE;
        }

        public virtual int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
            if (workingKey == null)
                throw new InvalidOperationException("DES engine not initialised");
			if ((inOff + BLOCK_SIZE) > input.Length)
                throw new DataLengthException("input buffer too short");
            if ((outOff + BLOCK_SIZE) > output.Length)
                throw new DataLengthException("output buffer too short");

			DesFunc(workingKey, input, inOff, output, outOff);

			return BLOCK_SIZE;
        }

        public virtual void Reset()
        {
        }

        /**
        * what follows is mainly taken from "Applied Cryptography", by
        * Bruce Schneier, however it also bears great resemblance to Richard
        * Outerbridge's D3DES...
        */

        private static readonly short[] Df_Key =
        {
            0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef,
            0xfe,0xdc,0xba,0x98,0x76,0x54,0x32,0x10,
            0x89,0xab,0xcd,0xef,0x01,0x23,0x45,0x67
        };

		private static readonly short[] bytebit =
        {
            128, 64, 32, 16, 8, 4, 2, 1
        };

		private static readonly int[] bigbyte =
        {
            0x800000,	0x400000,	0x200000,	0x100000,
            0x80000,	0x40000,	0x20000,	0x10000,
            0x8000,		0x4000,		0x2000,		0x1000,
            0x800,		0x400,		0x200,		0x100,
            0x80,		0x40,		0x20,		0x10,
            0x8,		0x4,		0x2,		0x1
        };

		/*
        * Use the key schedule specified in the Standard (ANSI X3.92-1981).
        */
        private static readonly byte[] pc1 =
        {
            56, 48, 40, 32, 24, 16,  8,   0, 57, 49, 41, 33, 25, 17,
            9,  1, 58, 50, 42, 34, 26,  18, 10,  2, 59, 51, 43, 35,
            62, 54, 46, 38, 30, 22, 14,   6, 61, 53, 45, 37, 29, 21,
            13,  5, 60, 52, 44, 36, 28,  20, 12,  4, 27, 19, 11,  3
        };

        private static readonly byte[] totrot =
        {
            1, 2, 4, 6, 8, 10, 12, 14,
            15, 17, 19, 21, 23, 25, 27, 28
        };

		private static readonly byte[] pc2 =
        {
            13, 16, 10, 23,  0,  4,  2, 27, 14,  5, 20,  9,
            22, 18, 11,  3, 25,  7, 15,  6, 26, 19, 12,  1,
            40, 51, 30, 36, 46, 54, 29, 39, 50, 44, 32, 47,
            43, 48, 38, 55, 33, 52, 45, 41, 49, 35, 28, 31
        };

		private static readonly int[] SP1 =
		{
            unchecked((int) 0x01010400), unchecked((int) 0x00000000), unchecked((int) 0x00010000), unchecked((int) 0x01010404),
            unchecked((int) 0x01010004), unchecked((int) 0x00010404), unchecked((int) 0x00000004), unchecked((int) 0x00010000),
            unchecked((int) 0x00000400), unchecked((int) 0x01010400), unchecked((int) 0x01010404), unchecked((int) 0x00000400),
            unchecked((int) 0x01000404), unchecked((int) 0x01010004), unchecked((int) 0x01000000), unchecked((int) 0x00000004),
            unchecked((int) 0x00000404), unchecked((int) 0x01000400), unchecked((int) 0x01000400), unchecked((int) 0x00010400),
            unchecked((int) 0x00010400), unchecked((int) 0x01010000), unchecked((int) 0x01010000), unchecked((int) 0x01000404),
            unchecked((int) 0x00010004), unchecked((int) 0x01000004), unchecked((int) 0x01000004), unchecked((int) 0x00010004),
            unchecked((int) 0x00000000), unchecked((int) 0x00000404), unchecked((int) 0x00010404), unchecked((int) 0x01000000),
            unchecked((int) 0x00010000), unchecked((int) 0x01010404), unchecked((int) 0x00000004), unchecked((int) 0x01010000),
            unchecked((int) 0x01010400), unchecked((int) 0x01000000), unchecked((int) 0x01000000), unchecked((int) 0x00000400),
            unchecked((int) 0x01010004), unchecked((int) 0x00010000), unchecked((int) 0x00010400), unchecked((int) 0x01000004),
            unchecked((int) 0x00000400), unchecked((int) 0x00000004), unchecked((int) 0x01000404), unchecked((int) 0x00010404),
            unchecked((int) 0x01010404), unchecked((int) 0x00010004), unchecked((int) 0x01010000), unchecked((int) 0x01000404),
            unchecked((int) 0x01000004), unchecked((int) 0x00000404), unchecked((int) 0x00010404), unchecked((int) 0x01010400),
            unchecked((int) 0x00000404), unchecked((int) 0x01000400), unchecked((int) 0x01000400), unchecked((int) 0x00000000),
            unchecked((int) 0x00010004), unchecked((int) 0x00010400), unchecked((int) 0x00000000), unchecked((int) 0x01010004)
        };

		private static readonly int[] SP2 =
		{
            unchecked((int) 0x80108020), unchecked((int) 0x80008000), unchecked((int) 0x00008000), unchecked((int) 0x00108020),
            unchecked((int) 0x00100000), unchecked((int) 0x00000020), unchecked((int) 0x80100020), unchecked((int) 0x80008020),
            unchecked((int) 0x80000020), unchecked((int) 0x80108020), unchecked((int) 0x80108000), unchecked((int) 0x80000000),
            unchecked((int) 0x80008000), unchecked((int) 0x00100000), unchecked((int) 0x00000020), unchecked((int) 0x80100020),
            unchecked((int) 0x00108000), unchecked((int) 0x00100020), unchecked((int) 0x80008020), unchecked((int) 0x00000000),
            unchecked((int) 0x80000000), unchecked((int) 0x00008000), unchecked((int) 0x00108020), unchecked((int) 0x80100000),
            unchecked((int) 0x00100020), unchecked((int) 0x80000020), unchecked((int) 0x00000000), unchecked((int) 0x00108000),
            unchecked((int) 0x00008020), unchecked((int) 0x80108000), unchecked((int) 0x80100000), unchecked((int) 0x00008020),
            unchecked((int) 0x00000000), unchecked((int) 0x00108020), unchecked((int) 0x80100020), unchecked((int) 0x00100000),
            unchecked((int) 0x80008020), unchecked((int) 0x80100000), unchecked((int) 0x80108000), unchecked((int) 0x00008000),
            unchecked((int) 0x80100000), unchecked((int) 0x80008000), unchecked((int) 0x00000020), unchecked((int) 0x80108020),
            unchecked((int) 0x00108020), unchecked((int) 0x00000020), unchecked((int) 0x00008000), unchecked((int) 0x80000000),
            unchecked((int) 0x00008020), unchecked((int) 0x80108000), unchecked((int) 0x00100000), unchecked((int) 0x80000020),
            unchecked((int) 0x00100020), unchecked((int) 0x80008020), unchecked((int) 0x80000020), unchecked((int) 0x00100020),
            unchecked((int) 0x00108000), unchecked((int) 0x00000000), unchecked((int) 0x80008000), unchecked((int) 0x00008020),
            unchecked((int) 0x80000000), unchecked((int) 0x80100020), unchecked((int) 0x80108020), unchecked((int) 0x00108000)
        };

		private static readonly int[] SP3 =
		{
            unchecked((int) 0x00000208), unchecked((int) 0x08020200), unchecked((int) 0x00000000), unchecked((int) 0x08020008),
            unchecked((int) 0x08000200), unchecked((int) 0x00000000), unchecked((int) 0x00020208), unchecked((int) 0x08000200),
            unchecked((int) 0x00020008), unchecked((int) 0x08000008), unchecked((int) 0x08000008), unchecked((int) 0x00020000),
            unchecked((int) 0x08020208), unchecked((int) 0x00020008), unchecked((int) 0x08020000), unchecked((int) 0x00000208),
            unchecked((int) 0x08000000), unchecked((int) 0x00000008), unchecked((int) 0x08020200), unchecked((int) 0x00000200),
            unchecked((int) 0x00020200), unchecked((int) 0x08020000), unchecked((int) 0x08020008), unchecked((int) 0x00020208),
            unchecked((int) 0x08000208), unchecked((int) 0x00020200), unchecked((int) 0x00020000), unchecked((int) 0x08000208),
            unchecked((int) 0x00000008), unchecked((int) 0x08020208), unchecked((int) 0x00000200), unchecked((int) 0x08000000),
            unchecked((int) 0x08020200), unchecked((int) 0x08000000), unchecked((int) 0x00020008), unchecked((int) 0x00000208),
            unchecked((int) 0x00020000), unchecked((int) 0x08020200), unchecked((int) 0x08000200), unchecked((int) 0x00000000),
            unchecked((int) 0x00000200), unchecked((int) 0x00020008), unchecked((int) 0x08020208), unchecked((int) 0x08000200),
            unchecked((int) 0x08000008), unchecked((int) 0x00000200), unchecked((int) 0x00000000), unchecked((int) 0x08020008),
            unchecked((int) 0x08000208), unchecked((int) 0x00020000), unchecked((int) 0x08000000), unchecked((int) 0x08020208),
            unchecked((int) 0x00000008), unchecked((int) 0x00020208), unchecked((int) 0x00020200), unchecked((int) 0x08000008),
            unchecked((int) 0x08020000), unchecked((int) 0x08000208), unchecked((int) 0x00000208), unchecked((int) 0x08020000),
            unchecked((int) 0x00020208), unchecked((int) 0x00000008), unchecked((int) 0x08020008), unchecked((int) 0x00020200)
        };

		private static readonly int[] SP4 =
		{
            unchecked((int) 0x00802001), unchecked((int) 0x00002081), unchecked((int) 0x00002081), unchecked((int) 0x00000080),
            unchecked((int) 0x00802080), unchecked((int) 0x00800081), unchecked((int) 0x00800001), unchecked((int) 0x00002001),
            unchecked((int) 0x00000000), unchecked((int) 0x00802000), unchecked((int) 0x00802000), unchecked((int) 0x00802081),
            unchecked((int) 0x00000081), unchecked((int) 0x00000000), unchecked((int) 0x00800080), unchecked((int) 0x00800001),
            unchecked((int) 0x00000001), unchecked((int) 0x00002000), unchecked((int) 0x00800000), unchecked((int) 0x00802001),
            unchecked((int) 0x00000080), unchecked((int) 0x00800000), unchecked((int) 0x00002001), unchecked((int) 0x00002080),
            unchecked((int) 0x00800081), unchecked((int) 0x00000001), unchecked((int) 0x00002080), unchecked((int) 0x00800080),
            unchecked((int) 0x00002000), unchecked((int) 0x00802080), unchecked((int) 0x00802081), unchecked((int) 0x00000081),
            unchecked((int) 0x00800080), unchecked((int) 0x00800001), unchecked((int) 0x00802000), unchecked((int) 0x00802081),
            unchecked((int) 0x00000081), unchecked((int) 0x00000000), unchecked((int) 0x00000000), unchecked((int) 0x00802000),
            unchecked((int) 0x00002080), unchecked((int) 0x00800080), unchecked((int) 0x00800081), unchecked((int) 0x00000001),
            unchecked((int) 0x00802001), unchecked((int) 0x00002081), unchecked((int) 0x00002081), unchecked((int) 0x00000080),
            unchecked((int) 0x00802081), unchecked((int) 0x00000081), unchecked((int) 0x00000001), unchecked((int) 0x00002000),
            unchecked((int) 0x00800001), unchecked((int) 0x00002001), unchecked((int) 0x00802080), unchecked((int) 0x00800081),
            unchecked((int) 0x00002001), unchecked((int) 0x00002080), unchecked((int) 0x00800000), unchecked((int) 0x00802001),
            unchecked((int) 0x00000080), unchecked((int) 0x00800000), unchecked((int) 0x00002000), unchecked((int) 0x00802080)
        };

		private static readonly int[] SP5 =
		{
            unchecked((int) 0x00000100), unchecked((int) 0x02080100), unchecked((int) 0x02080000), unchecked((int) 0x42000100),
            unchecked((int) 0x00080000), unchecked((int) 0x00000100), unchecked((int) 0x40000000), unchecked((int) 0x02080000),
            unchecked((int) 0x40080100), unchecked((int) 0x00080000), unchecked((int) 0x02000100), unchecked((int) 0x40080100),
            unchecked((int) 0x42000100), unchecked((int) 0x42080000), unchecked((int) 0x00080100), unchecked((int) 0x40000000),
            unchecked((int) 0x02000000), unchecked((int) 0x40080000), unchecked((int) 0x40080000), unchecked((int) 0x00000000),
            unchecked((int) 0x40000100), unchecked((int) 0x42080100), unchecked((int) 0x42080100), unchecked((int) 0x02000100),
            unchecked((int) 0x42080000), unchecked((int) 0x40000100), unchecked((int) 0x00000000), unchecked((int) 0x42000000),
            unchecked((int) 0x02080100), unchecked((int) 0x02000000), unchecked((int) 0x42000000), unchecked((int) 0x00080100),
            unchecked((int) 0x00080000), unchecked((int) 0x42000100), unchecked((int) 0x00000100), unchecked((int) 0x02000000),
            unchecked((int) 0x40000000), unchecked((int) 0x02080000), unchecked((int) 0x42000100), unchecked((int) 0x40080100),
            unchecked((int) 0x02000100), unchecked((int) 0x40000000), unchecked((int) 0x42080000), unchecked((int) 0x02080100),
            unchecked((int) 0x40080100), unchecked((int) 0x00000100), unchecked((int) 0x02000000), unchecked((int) 0x42080000),
            unchecked((int) 0x42080100), unchecked((int) 0x00080100), unchecked((int) 0x42000000), unchecked((int) 0x42080100),
            unchecked((int) 0x02080000), unchecked((int) 0x00000000), unchecked((int) 0x40080000), unchecked((int) 0x42000000),
            unchecked((int) 0x00080100), unchecked((int) 0x02000100), unchecked((int) 0x40000100), unchecked((int) 0x00080000),
            unchecked((int) 0x00000000), unchecked((int) 0x40080000), unchecked((int) 0x02080100), unchecked((int) 0x40000100)
        };

		private static readonly int[] SP6 =
		{
            unchecked((int) 0x20000010), unchecked((int) 0x20400000), unchecked((int) 0x00004000), unchecked((int) 0x20404010),
            unchecked((int) 0x20400000), unchecked((int) 0x00000010), unchecked((int) 0x20404010), unchecked((int) 0x00400000),
            unchecked((int) 0x20004000), unchecked((int) 0x00404010), unchecked((int) 0x00400000), unchecked((int) 0x20000010),
            unchecked((int) 0x00400010), unchecked((int) 0x20004000), unchecked((int) 0x20000000), unchecked((int) 0x00004010),
            unchecked((int) 0x00000000), unchecked((int) 0x00400010), unchecked((int) 0x20004010), unchecked((int) 0x00004000),
            unchecked((int) 0x00404000), unchecked((int) 0x20004010), unchecked((int) 0x00000010), unchecked((int) 0x20400010),
            unchecked((int) 0x20400010), unchecked((int) 0x00000000), unchecked((int) 0x00404010), unchecked((int) 0x20404000),
            unchecked((int) 0x00004010), unchecked((int) 0x00404000), unchecked((int) 0x20404000), unchecked((int) 0x20000000),
            unchecked((int) 0x20004000), unchecked((int) 0x00000010), unchecked((int) 0x20400010), unchecked((int) 0x00404000),
            unchecked((int) 0x20404010), unchecked((int) 0x00400000), unchecked((int) 0x00004010), unchecked((int) 0x20000010),
            unchecked((int) 0x00400000), unchecked((int) 0x20004000), unchecked((int) 0x20000000), unchecked((int) 0x00004010),
            unchecked((int) 0x20000010), unchecked((int) 0x20404010), unchecked((int) 0x00404000), unchecked((int) 0x20400000),
            unchecked((int) 0x00404010), unchecked((int) 0x20404000), unchecked((int) 0x00000000), unchecked((int) 0x20400010),
            unchecked((int) 0x00000010), unchecked((int) 0x00004000), unchecked((int) 0x20400000), unchecked((int) 0x00404010),
            unchecked((int) 0x00004000), unchecked((int) 0x00400010), unchecked((int) 0x20004010), unchecked((int) 0x00000000),
            unchecked((int) 0x20404000), unchecked((int) 0x20000000), unchecked((int) 0x00400010), unchecked((int) 0x20004010)
        };

		private static readonly int[] SP7 =
		{
            unchecked((int) 0x00200000), unchecked((int) 0x04200002), unchecked((int) 0x04000802), unchecked((int) 0x00000000),
            unchecked((int) 0x00000800), unchecked((int) 0x04000802), unchecked((int) 0x00200802), unchecked((int) 0x04200800),
            unchecked((int) 0x04200802), unchecked((int) 0x00200000), unchecked((int) 0x00000000), unchecked((int) 0x04000002),
            unchecked((int) 0x00000002), unchecked((int) 0x04000000), unchecked((int) 0x04200002), unchecked((int) 0x00000802),
            unchecked((int) 0x04000800), unchecked((int) 0x00200802), unchecked((int) 0x00200002), unchecked((int) 0x04000800),
            unchecked((int) 0x04000002), unchecked((int) 0x04200000), unchecked((int) 0x04200800), unchecked((int) 0x00200002),
            unchecked((int) 0x04200000), unchecked((int) 0x00000800), unchecked((int) 0x00000802), unchecked((int) 0x04200802),
            unchecked((int) 0x00200800), unchecked((int) 0x00000002), unchecked((int) 0x04000000), unchecked((int) 0x00200800),
            unchecked((int) 0x04000000), unchecked((int) 0x00200800), unchecked((int) 0x00200000), unchecked((int) 0x04000802),
            unchecked((int) 0x04000802), unchecked((int) 0x04200002), unchecked((int) 0x04200002), unchecked((int) 0x00000002),
            unchecked((int) 0x00200002), unchecked((int) 0x04000000), unchecked((int) 0x04000800), unchecked((int) 0x00200000),
            unchecked((int) 0x04200800), unchecked((int) 0x00000802), unchecked((int) 0x00200802), unchecked((int) 0x04200800),
            unchecked((int) 0x00000802), unchecked((int) 0x04000002), unchecked((int) 0x04200802), unchecked((int) 0x04200000),
            unchecked((int) 0x00200800), unchecked((int) 0x00000000), unchecked((int) 0x00000002), unchecked((int) 0x04200802),
            unchecked((int) 0x00000000), unchecked((int) 0x00200802), unchecked((int) 0x04200000), unchecked((int) 0x00000800),
            unchecked((int) 0x04000002), unchecked((int) 0x04000800), unchecked((int) 0x00000800), unchecked((int) 0x00200002)
        };

		private static readonly int[] SP8 =
		{
            unchecked((int) 0x10001040), unchecked((int) 0x00001000), unchecked((int) 0x00040000), unchecked((int) 0x10041040),
            unchecked((int) 0x10000000), unchecked((int) 0x10001040), unchecked((int) 0x00000040), unchecked((int) 0x10000000),
            unchecked((int) 0x00040040), unchecked((int) 0x10040000), unchecked((int) 0x10041040), unchecked((int) 0x00041000),
            unchecked((int) 0x10041000), unchecked((int) 0x00041040), unchecked((int) 0x00001000), unchecked((int) 0x00000040),
            unchecked((int) 0x10040000), unchecked((int) 0x10000040), unchecked((int) 0x10001000), unchecked((int) 0x00001040),
            unchecked((int) 0x00041000), unchecked((int) 0x00040040), unchecked((int) 0x10040040), unchecked((int) 0x10041000),
            unchecked((int) 0x00001040), unchecked((int) 0x00000000), unchecked((int) 0x00000000), unchecked((int) 0x10040040),
            unchecked((int) 0x10000040), unchecked((int) 0x10001000), unchecked((int) 0x00041040), unchecked((int) 0x00040000),
            unchecked((int) 0x00041040), unchecked((int) 0x00040000), unchecked((int) 0x10041000), unchecked((int) 0x00001000),
            unchecked((int) 0x00000040), unchecked((int) 0x10040040), unchecked((int) 0x00001000), unchecked((int) 0x00041040),
            unchecked((int) 0x10001000), unchecked((int) 0x00000040), unchecked((int) 0x10000040), unchecked((int) 0x10040000),
            unchecked((int) 0x10040040), unchecked((int) 0x10000000), unchecked((int) 0x00040000), unchecked((int) 0x10001040),
            unchecked((int) 0x00000000), unchecked((int) 0x10041040), unchecked((int) 0x00040040), unchecked((int) 0x10000040),
            unchecked((int) 0x10040000), unchecked((int) 0x10001000), unchecked((int) 0x10001040), unchecked((int) 0x00000000),
            unchecked((int) 0x10041040), unchecked((int) 0x00041000), unchecked((int) 0x00041000), unchecked((int) 0x00001040),
            unchecked((int) 0x00001040), unchecked((int) 0x00040040), unchecked((int) 0x10000000), unchecked((int) 0x10041000)
        };

		/**
        * Generate an integer based working key based on our secret key
        * and what we processing we are planning to do.
        *
        * Acknowledgements for this routine go to James Gillogly and Phil Karn.
        *         (whoever, and wherever they are!).
        */
        protected static int[] GenerateWorkingKey(
            bool	encrypting,
            byte[]	key)
        {
            int[] newKey = new int[32];
            bool[] pc1m = new bool[56];
			bool[] pcr = new bool[56];

			for (int j = 0; j < 56; j++ )
            {
                int l = pc1[j];

				pc1m[j] = ((key[(uint) l >> 3] & bytebit[l & 07]) != 0);
            }

            for (int i = 0; i < 16; i++)
            {
                int l, m, n;

                if (encrypting)
                {
                    m = i << 1;
                }
                else
                {
                    m = (15 - i) << 1;
                }

                n = m + 1;
                newKey[m] = newKey[n] = 0;

                for (int j = 0; j < 28; j++)
                {
                    l = j + totrot[i];
                    if ( l < 28 )
                    {
                        pcr[j] = pc1m[l];
                    }
                    else
                    {
                        pcr[j] = pc1m[l - 28];
                    }
                }

                for (int j = 28; j < 56; j++)
                {
                    l = j + totrot[i];
                    if (l < 56 )
                    {
                        pcr[j] = pc1m[l];
                    }
                    else
                    {
                        pcr[j] = pc1m[l - 28];
                    }
                }

                for (int j = 0; j < 24; j++)
                {
                    if (pcr[pc2[j]])
                    {
                        newKey[m] |= bigbyte[j];
                    }

                    if (pcr[pc2[j + 24]])
                    {
                        newKey[n] |= bigbyte[j];
                    }
                }
            }

            //
            // store the processed key
            //
            for (int i = 0; i != 32; i += 2)
            {
                int i1, i2;

                i1 = newKey[i];
                i2 = newKey[i + 1];

                newKey[i] = (int) ( (uint) ((i1 & 0x00fc0000) << 6)  |
                                    (uint) ((i1 & 0x00000fc0) << 10) |
                                    ((uint) (i2 & 0x00fc0000) >> 10) |
                                    ((uint) (i2 & 0x00000fc0) >> 6));

                newKey[i + 1] = (int) ( (uint) ((i1 & 0x0003f000) << 12) |
                                        (uint) ((i1 & 0x0000003f) << 16) |
                                        ((uint) (i2 & 0x0003f000) >> 4) |
                                        (uint) (i2 & 0x0000003f));
            }

            return newKey;
        }

        /**
        * the DES engine.
        */
        internal static void DesFunc(
            int[]	wKey,
            byte[]	input,
            int		inOff,
            byte[]	outBytes,
            int		outOff)
        {
            int work, right, left;

            left = (input[inOff + 0] & 0xff) << 24;
            left |= (input[inOff + 1] & 0xff) << 16;
            left |= (input[inOff + 2] & 0xff) << 8;
            left |= (input[inOff + 3] & 0xff);

            right = (input[inOff + 4] & 0xff) << 24;
            right |= (input[inOff + 5] & 0xff) << 16;
            right |= (input[inOff + 6] & 0xff) << 8;
            right |= (input[inOff + 7] & 0xff);

            work = (int) (((uint) left >> 4) ^ right) & unchecked((int) 0x0f0f0f0f);
            right ^= work;
            left ^= (work << 4);
            work = (int) (((uint) left >> 16) ^ right) & unchecked((int) 0x0000ffff);
            right ^= work;
            left ^= (work << 16);
            work = (int) (((uint) right >> 2) ^ left) & unchecked((int) 0x33333333);
            left ^= work;
            right ^= (work << 2);
            work = (int) (((uint) right >> 8) ^ left) & unchecked((int) 0x00ff00ff);
            left ^= work;
            right ^= (work << 8);
            right = (int) (  (uint) (right << 1) |
                             (  ((uint) right >> 31)  & 1 )
                          ) &
                    unchecked((int) 0xffffffff);
            work = (left ^ right) & unchecked((int) 0xaaaaaaaa);
            left ^= work;
            right ^= work;
            left =  (int) (   (uint) (left << 1) |
                              ( ((uint) left >> 31) & 1)) &
                    unchecked((int) 0xffffffff);

            for (int round = 0; round < 8; round++)
            {
                int fval;

                work  = (int) ((uint) (right << 28) | ((uint) right >> 4));
                work ^= wKey[round * 4 + 0];
                fval  = SP7[ work      & 0x3f];
                fval |= SP5[((uint) work >>  8) & 0x3f];
                fval |= SP3[((uint) work >> 16) & 0x3f];
                fval |= SP1[((uint) work >> 24) & 0x3f];
                work  = right ^ wKey[round * 4 + 1];
                fval |= SP8[ work      & 0x3f];
                fval |= SP6[((uint) work >>  8) & 0x3f];
                fval |= SP4[((uint) work >> 16) & 0x3f];
                fval |= SP2[((uint) work >> 24) & 0x3f];
                left ^= fval;
                work  = (int) ((uint) (left << 28) | ((uint) left >> 4));
                work ^= wKey[round * 4 + 2];
                fval  = SP7[ work      & 0x3f];
                fval |= SP5[((uint) work >>  8) & 0x3f];
                fval |= SP3[((uint) work >> 16) & 0x3f];
                fval |= SP1[((uint) work >> 24) & 0x3f];
                work  = left ^ wKey[round * 4 + 3];
                fval |= SP8[ work      & 0x3f];
                fval |= SP6[((uint) work >>  8) & 0x3f];
                fval |= SP4[((uint) work >> 16) & 0x3f];
                fval |= SP2[((uint) work >> 24) & 0x3f];
                right ^= fval;
            }

            right = (int)  ((uint) (right << 31) | ((uint) right >> 1));
            work = (left ^ right) & unchecked((int) 0xaaaaaaaa);
            left ^= work;
            right ^= work;
            left = (int) ((uint) (left << 31) | ((uint) left >> 1));
            work = (int) ((((uint) left >> 8) ^ right) & 0x00ff00ff);
            right ^= work;
            left ^= (work << 8);
            work = (int) ((((uint) left >> 2) ^ right) & 0x33333333);
            right ^= work;
            left ^= (work << 2);
            work = (int) ((((uint) right >> 16) ^ left) & 0x0000ffff);
            left ^= work;
            right ^= (work << 16);
            work = (int) ((((uint) right >> 4) ^ left) & 0x0f0f0f0f);
            left ^= work;
            right ^= (work << 4);

            outBytes[outOff + 0] = (byte)(((uint) right >> 24) & 0xff);
            outBytes[outOff + 1] = (byte)(((uint) right >> 16) & 0xff);
            outBytes[outOff + 2] = (byte)(((uint) right >>  8) & 0xff);
            outBytes[outOff + 3] = (byte)( right         & 0xff);
            outBytes[outOff + 4] = (byte)(((uint) left >> 24) & 0xff);
            outBytes[outOff + 5] = (byte)(((uint) left >> 16) & 0xff);
            outBytes[outOff + 6] = (byte)(((uint) left >>  8) & 0xff);
            outBytes[outOff + 7] = (byte)( left         & 0xff);
        }
    }
}
