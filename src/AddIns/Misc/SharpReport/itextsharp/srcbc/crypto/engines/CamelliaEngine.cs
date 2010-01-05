using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* Camellia - based on RFC 3713.
	*/
	public class CamelliaEngine
		: IBlockCipher
	{
		private bool initialised;
		private bool _keyIs128;

		private const int  BLOCK_SIZE = 16;

		private const long MASK8 = 0xff;
		private const long MASK32 = 0xffffffffL;

		private const long SIGMA1 = unchecked((long) 0xA09E667F3BCC908BL);
		private const long SIGMA2 = unchecked((long) 0xB67AE8584CAA73B2L);
		private const long SIGMA3 = unchecked((long) 0xC6EF372FE94F82BEL);
		private const long SIGMA4 = unchecked((long) 0x54FF53A5F1D36F1CL);
		private const long SIGMA5 = unchecked((long) 0x10E527FADE682D1DL);
		private const long SIGMA6 = unchecked((long) 0xB05688C2B3E6C1FDL);

		private long   _kw1, _kw2, _kw3, _kw4;
		private long   _k1, _k2, _k3, _k4, _k5, _k6, _k7, _k8, _k9, _k10, _k11, _k12,
					_k13, _k14, _k15, _k16, _k17, _k18, _k19, _k20, _k21, _k22, _k23, _k24;
		private long   _ke1, _ke2, _ke3, _ke4, _ke5, _ke6;

		private readonly byte[] SBOX1 = {
				(byte)112, (byte)130,  (byte)44,  (byte)236,  (byte)179 ,  (byte)39,  (byte)192,  (byte)229,  (byte)228,  (byte)133 ,  (byte)87 ,  (byte)53,  (byte)234 ,  (byte)12,  (byte)174 , (byte)65,
				(byte)35, (byte)239,  (byte)107,  (byte)147 ,  (byte)69 ,  (byte)25,  (byte)165 ,  (byte)33,  (byte)237 ,  (byte)14 ,  (byte)79 ,  (byte)78 ,  (byte)29,  (byte)101,  (byte)146,  (byte)189,
				(byte)134, (byte)184,   (byte)175,  (byte)143,  (byte)124,  (byte)235 ,  (byte)31,  (byte)206 ,  (byte)62 ,  (byte)48,  (byte)220 ,  (byte)95 ,  (byte)94,  (byte)197 ,  (byte)11 ,  (byte)26,
				(byte)166, (byte)225,  (byte)57,  (byte)202,  (byte)213 ,  (byte)71 ,  (byte)93 ,  (byte)61,  (byte)217  ,  (byte)1 ,  (byte)90,  (byte)214 ,  (byte)81 ,  (byte)86,  (byte)108 ,  (byte)77,
				(byte)139, (byte)13,  (byte)154,  (byte)102,  (byte)251,  (byte)204,  (byte)176 ,  (byte)45,  (byte)116 ,  (byte)18 ,  (byte)43 ,  (byte)32,  (byte)240,  (byte)177,  (byte)132,  (byte)153,
				(byte)223, (byte)76,  (byte)203,  (byte)194 ,  (byte)52,  (byte)126,  (byte)118  ,  (byte)5,  (byte)109,  (byte)183,  (byte)169 ,  (byte)49,  (byte)209 ,  (byte)23  ,  (byte)4,  (byte)215,
				(byte)20, (byte)88,  (byte)58,  (byte)97,  (byte)222 ,  (byte)27 ,  (byte)17 ,  (byte)28 ,  (byte)50 ,  (byte)15,  (byte)156 ,  (byte)22 ,  (byte)83 ,  (byte)24,  (byte)242 ,  (byte)34,
				(byte)254, (byte)68,  (byte)207,  (byte)178,  (byte)195,  (byte)181,  (byte)122,  (byte)145 ,  (byte)36  ,  (byte)8,  (byte)232,  (byte)168 ,  (byte)96,  (byte)252,  (byte)105 ,  (byte)80,
				(byte)170, (byte)208,  (byte)160,  (byte)125,  (byte)161,  (byte)137 ,  (byte)98,  (byte)151 ,  (byte)84 ,  (byte)91 ,  (byte)30,  (byte)149,  (byte)224,  (byte)255,  (byte)100,  (byte)210,
				(byte)16, (byte)196,  (byte)0,  (byte)72,  (byte)163,  (byte)247,  (byte)117,  (byte)219,  (byte)138  ,  (byte)3,  (byte)230,  (byte)218  ,  (byte)9 ,  (byte)63,  (byte)221,  (byte)148,
				(byte)135, (byte)92,  (byte)131,  (byte)2,  (byte)205 ,  (byte)74,  (byte)144 ,  (byte)51,  (byte)115,  (byte)103,  (byte)246,  (byte)243,  (byte)157,  (byte)127,  (byte)191,  (byte)226,
				(byte)82, (byte)155,  (byte)216 ,  (byte)38,  (byte)200 ,  (byte)55,  (byte)198 ,  (byte)59,  (byte)129,  (byte)150,  (byte)111 ,  (byte)75 ,  (byte)19,  (byte)190 ,  (byte)99 ,  (byte)46,
				(byte)233, (byte)121,  (byte)167,  (byte)140,  (byte)159,  (byte)110,  (byte)188,  (byte)142 ,  (byte)41,  (byte)245,  (byte)249,  (byte)182 ,  (byte)47,  (byte)253,  (byte)180 ,  (byte)89,
				(byte)120, (byte)152,  (byte)6,  (byte)106,  (byte)231 ,  (byte)70,  (byte)113,  (byte)186,  (byte)212 ,  (byte)37,  (byte)171 ,  (byte)66,  (byte)136,  (byte)162,  (byte)141,  (byte)250,
				(byte)114, (byte)7,  (byte)185 ,  (byte)85,  (byte)248,  (byte)238,  (byte)172 ,  (byte)10 ,  (byte)54 ,  (byte)73 ,  (byte)42,  (byte)104 ,  (byte)60 ,  (byte)56,  (byte)241,  (byte)164,
				(byte)64, (byte)40,  (byte)211,  (byte)123,  (byte)187,  (byte)201 ,  (byte)67,  (byte)193 ,  (byte)21,  (byte)227,  (byte)173,  (byte)244,  (byte)119,  (byte)199,  (byte)128,  (byte)158
			};

		private readonly byte[] SBOX2 = new byte[256];
		private readonly byte[] SBOX3 = new byte[256];
		private readonly byte[] SBOX4 = new byte[256];


		public CamelliaEngine()
		{
			for (int x = 0; x != 256; x++)
			{
				SBOX2[x] = lRot8(SBOX1[x], 1);
				SBOX3[x] = lRot8(SBOX1[x], 7);
				SBOX4[x] = SBOX1[lRot8((byte)x, 1) & 0xff];
			}
		}

		private void setKey(
			bool forEncryption,
			byte[]  key)
		{
			long klA, klB;
			long krA, krB;

			switch (key.Length)
			{
			case 16:
				_keyIs128 = true;
				klA = bytesToWord(key, 0);
				klB = bytesToWord(key, 8);
				krA = 0;
				krB = 0;
				break;
			case 24:
				klA = bytesToWord(key, 0);
				klB = bytesToWord(key, 8);
				krA = bytesToWord(key, 16);
				krB = ~bytesToWord(key, 16);
				_keyIs128 = false;
				break;
			case 32:
				klA = bytesToWord(key, 0);
				klB = bytesToWord(key, 8);
				krA = bytesToWord(key, 16);
				krB = bytesToWord(key, 24);
				_keyIs128 = false;
				break;
			default:
				throw new ArgumentException("only a key sizes of 128/192/256 are acceptable.");
			}

			long d1 = klA ^ krA;
			long d2 = klB ^ krB;

			d2 = d2 ^ f(d1, SIGMA1);
			d1 = d1 ^ f(d2, SIGMA2);
			d1 = d1 ^ klA;
			d2 = d2 ^ klB;
			d2 = d2 ^ f(d1, SIGMA3);
			d1 = d1 ^ f(d2, SIGMA4);

			long kaA = d1;
			long kaB = d2;

			if (_keyIs128)
			{
				if (forEncryption)
				{
					_kw1 = klA;
					_kw2 = klB;
					_kw3 = lRot128high(kaA, kaB, 111);
					_kw4 = lRot128low(kaA, kaB, 111);
					_k1  = kaA;
					_k2  = kaB;
					_k3  = lRot128high(klA, klB, 15);
					_k4  = lRot128low(klA, klB, 15);
					_k5  = lRot128high(kaA, kaB, 15);
					_k6  = lRot128low(kaA, kaB, 15);
					_k7  = lRot128high(klA, klB, 45);
					_k8  = lRot128low(klA, klB, 45);
					_k9  = lRot128high(kaA, kaB, 45);
					_k10 = lRot128low(klA, klB,  60);
					_k11 = lRot128high(kaA, kaB, 60);
					_k12 = lRot128low(kaA, kaB, 60);
					_k13 = lRot128high(klA, klB, 94);
					_k14 = lRot128low(klA, klB, 94);
					_k15 = lRot128high(kaA, kaB, 94);
					_k16 = lRot128low(kaA, kaB, 94);
					_k17 = lRot128high(klA, klB, 111);
					_k18 = lRot128low(klA, klB, 111);
					_ke1 = lRot128high(kaA, kaB, 30);
					_ke2 = lRot128low(kaA, kaB, 30);
					_ke3 = lRot128high(klA, klB, 77);
					_ke4 = lRot128low(klA, klB, 77);
				}
				else
				{
					_kw3 = klA;
					_kw4 = klB;
					_kw1 = lRot128high(kaA, kaB, 111);
					_kw2 = lRot128low(kaA, kaB, 111);
					_k18 = kaA;
					_k17 = kaB;
					_k16 = lRot128high(klA, klB, 15);
					_k15 = lRot128low(klA, klB, 15);
					_k14 = lRot128high(kaA, kaB, 15);
					_k13 = lRot128low(kaA, kaB, 15);
					_k12 = lRot128high(klA, klB, 45);
					_k11 = lRot128low(klA, klB, 45);
					_k10 = lRot128high(kaA, kaB, 45);
					_k9  = lRot128low(klA, klB,  60);
					_k8  = lRot128high(kaA, kaB, 60);
					_k7  = lRot128low(kaA, kaB, 60);
					_k6  = lRot128high(klA, klB, 94);
					_k5  = lRot128low(klA, klB, 94);
					_k4  = lRot128high(kaA, kaB, 94);
					_k3  = lRot128low(kaA, kaB, 94);
					_k2  = lRot128high(klA, klB, 111);
					_k1  = lRot128low(klA, klB, 111);
					_ke4 = lRot128high(kaA, kaB, 30);
					_ke3 = lRot128low(kaA, kaB, 30);
					_ke2 = lRot128high(klA, klB, 77);
					_ke1 = lRot128low(klA, klB, 77);
				}
			}
			else
			{
				d1 = kaA ^ krA;
				d2 = kaB ^ krB;
				d2 = d2 ^ f(d1, SIGMA5);
				d1 = d1 ^ f(d2, SIGMA6);

				long kbA = d1;
				long kbB = d2;

				if (forEncryption)
				{
					_kw1 = klA;
					_kw2 = klB;
					_k1  = kbA;
					_k2  = kbB;
					_k3  = lRot128high(krA, krB, 15);
					_k4  = lRot128low(krA, krB, 15);
					_k5  = lRot128high(kaA, kaB, 15);
					_k6  = lRot128low(kaA, kaB, 15);
					_ke1 = lRot128high(krA, krB, 30);
					_ke2 = lRot128low(krA, krB, 30);
					_k7  = lRot128high(kbA, kbB, 30);
					_k8  = lRot128low(kbA, kbB, 30);
					_k9  = lRot128high(klA, klB, 45);
					_k10 = lRot128low(klA, klB, 45);
					_k11 = lRot128high(kaA, kaB, 45);
					_k12 = lRot128low(kaA, kaB, 45);
					_ke3 = lRot128high(klA, klB, 60);
					_ke4 = lRot128low(klA, klB, 60);
					_k13 = lRot128high(krA, krB, 60);
					_k14 = lRot128low(krA, krB, 60);
					_k15 = lRot128high(kbA, kbB, 60);
					_k16 = lRot128low(kbA, kbB, 60);
					_k17 = lRot128high(klA, klB, 77);
					_k18 = lRot128low(klA, klB, 77);
					_ke5 = lRot128high(kaA, kaB, 77);
					_ke6 = lRot128low(kaA, kaB, 77);
					_k19 = lRot128high(krA, krB, 94);
					_k20 = lRot128low(krA, krB, 94);
					_k21 = lRot128high(kaA, kaB, 94);
					_k22 = lRot128low(kaA, kaB, 94);
					_k23 = lRot128high(klA, klB, 111);
					_k24 = lRot128low(klA, klB, 111);
					_kw3 = lRot128high(kbA, kbB, 111);
					_kw4 = lRot128low(kbA, kbB, 111);
				}
				else
				{
					_kw3 = klA;
					_kw4 = klB;
					_kw1 = lRot128high(kbA, kbB, 111);
					_kw2 = lRot128low(kbA, kbB, 111);
					_k24 = kbA;
					_k23 = kbB;
					_k22 = lRot128high(krA, krB, 15);
					_k21 = lRot128low(krA, krB, 15);
					_k20 = lRot128high(kaA, kaB, 15);
					_k19 = lRot128low(kaA, kaB, 15);
					_k18 = lRot128high(kbA, kbB, 30);
					_k17 = lRot128low(kbA, kbB, 30);
					_k16 = lRot128high(klA, klB, 45);
					_k15 = lRot128low(klA, klB, 45);
					_k14 = lRot128high(kaA, kaB, 45);
					_k13 = lRot128low(kaA, kaB, 45);
					_k12 = lRot128high(krA, krB, 60);
					_k11 = lRot128low(krA, krB, 60);
					_k10 = lRot128high(kbA, kbB, 60);
					_k9  = lRot128low(kbA, kbB, 60);
					_k8  = lRot128high(klA, klB, 77);
					_k7  = lRot128low(klA, klB, 77);
					_k6  = lRot128high(krA, krB, 94);
					_k5  = lRot128low(krA, krB, 94);
					_k4  = lRot128high(kaA, kaB, 94);
					_k3  = lRot128low(kaA, kaB, 94);
					_k2  = lRot128high(klA, klB, 111);
					_k1  = lRot128low(klA, klB, 111);
					_ke6 = lRot128high(krA, krB, 30);
					_ke5 = lRot128low(krA, krB, 30);
					_ke4 = lRot128high(klA, klB, 60);
					_ke3 = lRot128low(klA, klB, 60);
					_ke2 = lRot128high(kaA, kaB, 77);
					_ke1 = lRot128low(kaA, kaB, 77);
				}
			}
		}

		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			if (!(parameters is KeyParameter))
				throw new ArgumentException("only simple KeyParameter expected.");

			setKey(forEncryption, ((KeyParameter)parameters).GetKey());

			initialised = true;
		}

		public string AlgorithmName
		{
			get { return "Camellia"; }
		}

		public bool IsPartialBlockOkay
		{
			get { return false; }
		}

		public int GetBlockSize()
		{
			return BLOCK_SIZE;
		}

		public int ProcessBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			if (!initialised)
				throw new InvalidOperationException("Camellia engine not initialised");
			if ((inOff + BLOCK_SIZE) > input.Length)
				throw new DataLengthException("input buffer too short");
			if ((outOff + BLOCK_SIZE) > output.Length)
				throw new DataLengthException("output buffer too short");

			if (_keyIs128)
			{
				return processBlock128(input, inOff, output, outOff);
			}
			else
			{
				return processBlock192or256(input, inOff, output, outOff);
			}
		}

		public void Reset()
		{
			// nothing
		}

		private byte lRot8(
			byte value,
			int  rotation)
		{
//			return (byte)((value << rotation) | ((value & 0xff) >>> (8 - rotation)));
			return (byte)((value << rotation) | ((value & 0xff) >> (8 - rotation)));
		}

		private int lRot32(
			int value,
			int rotation)
		{
			uint uv = (uint) value;
//			return (value << rotation) | (value >>> (32 - rotation));
			return (int)((uv << rotation) | (uv >> (32 - rotation)));
		}

		private long lRot128high(
			long a,
			long b,
			int rotation)
		{
			ulong ua = (ulong) a, ub = (ulong) b;

			if (rotation < 64)
			{
//				a = (a << rotation) | (b >>> (64 - rotation));
				ua = (ua << rotation) | (ub >> (64 - rotation));
			}
			else if (rotation == 64)
			{
				ua = ub;
			}
			else
			{
//				a = (b << (rotation - 64)) | (a >>> (64 - (rotation - 64)));
				ua = (ub << (rotation - 64)) | (ua >> (64 - (rotation - 64)));
			}

//			return a;
			return (long) ua;
		}

		private long lRot128low(
			long a,
			long b,
			int rotation)
		{
			ulong ua = (ulong) a, ub = (ulong) b;

			if (rotation < 64)
			{
//				b = (b << rotation) | (a >>> (64 - rotation));
				ub = (ub << rotation) | (ua >> (64 - rotation));
			}
			else if (rotation == 64)
			{
				ub = ua;
			}
			else
			{
//				b = (a << (rotation - 64)) | (b >>> (64 - (rotation - 64)));
				ub = (ua << (rotation - 64)) | (ub >> (64 - (rotation - 64)));
			}

//			return b;
			return (long) ub;
		}

		private long fl(
			long input,
			long ke)
		{
			int x1 = (int)(input >> 32);
			int x2 = (int)input;
			int k1 = (int)(ke >> 32);
			int k2 = (int)ke;

			x2 = x2 ^ lRot32((x1 & k1), 1);
			x1 = x1 ^ (x2 | k2);

			return ((long)x1 << 32) | (x2 & MASK32);
		}

		private long flInv(
			long input,
			long ke)
		{
			int y1 = (int)(input >> 32);
			int y2 = (int)input;
			int k1 = (int)(ke >> 32);
			int k2 = (int)ke;

			y1 = y1 ^ (y2 | k2);
			y2 = y2 ^ lRot32((y1 & k1), 1);

			return ((long)y1 << 32) | (y2 & MASK32);
		}

		private long f(
			long input,
			long ke)
		{
			long x;
			int  a, b;
			int  t1, t2, t3, t4, t5, t6, t7, t8;
			int  y1, y2, y3, y4, y5, y6, y7, y8;

			x  = input ^ ke;

			a = (int)(x >> 32);
			b = (int)x;

			t1 = SBOX1[(a >> 24) & 0xff];
			t2 = SBOX2[(a >> 16) & 0xff];
			t3 = SBOX3[(a >>  8) & 0xff];
			t4 = SBOX4[a & 0xff];
			t5 = SBOX2[(b >> 24) & 0xff];
			t6 = SBOX3[(b >> 16) & 0xff];
			t7 = SBOX4[(b >>  8) & 0xff];
			t8 = SBOX1[b & 0xff];

			y1 = (t1 ^ t3 ^ t4 ^ t6 ^ t7 ^ t8);
			y2 = (t1 ^ t2 ^ t4 ^ t5 ^ t7 ^ t8);
			y3 = (t1 ^ t2 ^ t3 ^ t5 ^ t6 ^ t8);
			y4 = (t2 ^ t3 ^ t4 ^ t5 ^ t6 ^ t7);
			y5 = (t1 ^ t2 ^ t6 ^ t7 ^ t8);
			y6 = (t2 ^ t3 ^ t5 ^ t7 ^ t8);
			y7 = (t3 ^ t4 ^ t5 ^ t6 ^ t8);
			y8 = (t1 ^ t4 ^ t5 ^ t6 ^ t7);

			return ((long)y1 << 56) | (((long)y2 & MASK8) << 48) | (((long)y3 & MASK8) << 40)
					| (((long)y4 & MASK8) << 32) | (((long)y5 & MASK8) << 24) | (((long)y6 & MASK8) << 16)
					| (((long)y7 & MASK8) <<  8) | ((long)y8 & MASK8);
		}

		private long bytesToWord(
			byte[]  src,
			int     srcOff)
		{
			long    word = 0;

			for (int i = 0; i < 8; i++)
			{
				word = (word << 8) + (src[i + srcOff] & 0xff);
			}

			return word;
		}

		private void wordToBytes(
			long    word,
			byte[]  dst,
			int     dstOff)
		{
			ulong uw = (ulong) word;
			for (int i = 0; i < 8; i++)
			{
//				dst[(7 - i) + dstOff] = (byte)word;
				dst[(7 - i) + dstOff] = (byte)uw;
//				word >>>= 8;
				uw >>= 8;
			}
		}

		private int processBlock128(
			byte[] inBytes,
			int inOff,
			byte[] outBytes,
			int outOff)
		{
			long d1 = bytesToWord(inBytes, inOff);
			long d2 = bytesToWord(inBytes, inOff + 8);

			d1 = d1 ^ _kw1;           // Prewhitening
			d2 = d2 ^ _kw2;

			d2 = d2 ^ f(d1, _k1);     // Round 1
			d1 = d1 ^ f(d2, _k2);     // Round 2
			d2 = d2 ^ f(d1, _k3);     // Round 3
			d1 = d1 ^ f(d2, _k4);     // Round 4
			d2 = d2 ^ f(d1, _k5);     // Round 5
			d1 = d1 ^ f(d2, _k6);     // Round 6
			d1 = fl   (d1, _ke1);     // FL
			d2 = flInv(d2, _ke2);     // FLINV
			d2 = d2 ^ f(d1, _k7);     // Round 7
			d1 = d1 ^ f(d2, _k8);     // Round 8
			d2 = d2 ^ f(d1, _k9);     // Round 9
			d1 = d1 ^ f(d2, _k10);    // Round 10
			d2 = d2 ^ f(d1, _k11);    // Round 11
			d1 = d1 ^ f(d2, _k12);    // Round 12
			d1 = fl   (d1, _ke3);     // FL
			d2 = flInv(d2, _ke4);     // FLINV

			d2 = d2 ^ f(d1, _k13);    // Round 13
			d1 = d1 ^ f(d2, _k14);    // Round 14
			d2 = d2 ^ f(d1, _k15);    // Round 15
			d1 = d1 ^ f(d2, _k16);    // Round 16
			d2 = d2 ^ f(d1, _k17);    // Round 17
			d1 = d1 ^ f(d2, _k18);    // Round 18

			d2 = d2 ^ _kw3;           // Postwhitening
			d1 = d1 ^ _kw4;

			wordToBytes(d2, outBytes, outOff);
			wordToBytes(d1, outBytes, outOff + 8);

			return BLOCK_SIZE;
		}

		private int processBlock192or256(
			byte[] inBytes,
			int inOff,
			byte[] outBytes,
			int outOff)
		{
			long d1 = bytesToWord(inBytes, inOff);
			long d2 = bytesToWord(inBytes, inOff + 8);

			d1 = d1 ^ _kw1;           // Prewhitening
			d2 = d2 ^ _kw2;

			d2 = d2 ^ f(d1, _k1);     // Round 1
			d1 = d1 ^ f(d2, _k2);     // Round 2
			d2 = d2 ^ f(d1, _k3);     // Round 3
			d1 = d1 ^ f(d2, _k4);     // Round 4
			d2 = d2 ^ f(d1, _k5);     // Round 5
			d1 = d1 ^ f(d2, _k6);     // Round 6
			d1 = fl   (d1, _ke1);     // FL
			d2 = flInv(d2, _ke2);     // FLINV
			d2 = d2 ^ f(d1, _k7);     // Round 7
			d1 = d1 ^ f(d2, _k8);     // Round 8
			d2 = d2 ^ f(d1, _k9);     // Round 9
			d1 = d1 ^ f(d2, _k10);    // Round 10
			d2 = d2 ^ f(d1, _k11);    // Round 11
			d1 = d1 ^ f(d2, _k12);    // Round 12
			d1 = fl   (d1, _ke3);     // FL
			d2 = flInv(d2, _ke4);     // FLINV
			d2 = d2 ^ f(d1, _k13);    // Round 13
			d1 = d1 ^ f(d2, _k14);    // Round 14
			d2 = d2 ^ f(d1, _k15);    // Round 15
			d1 = d1 ^ f(d2, _k16);    // Round 16
			d2 = d2 ^ f(d1, _k17);    // Round 17
			d1 = d1 ^ f(d2, _k18);    // Round 18
			d1 = fl   (d1, _ke5);     // FL
			d2 = flInv(d2, _ke6);     // FLINV
			d2 = d2 ^ f(d1, _k19);    // Round 19
			d1 = d1 ^ f(d2, _k20);    // Round 20
			d2 = d2 ^ f(d1, _k21);    // Round 21
			d1 = d1 ^ f(d2, _k22);    // Round 22
			d2 = d2 ^ f(d1, _k23);    // Round 23
			d1 = d1 ^ f(d2, _k24);    // Round 24

			d2 = d2 ^ _kw3;           // Postwhitening
			d1 = d1 ^ _kw4;

			wordToBytes(d2, outBytes, outOff);
			wordToBytes(d1, outBytes, outOff + 8);

			return BLOCK_SIZE;
		}
	}
}
