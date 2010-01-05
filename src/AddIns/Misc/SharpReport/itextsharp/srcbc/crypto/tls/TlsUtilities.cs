using System;
using System.IO;
using System.Text;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>Some helper fuctions for MicroTLS.</remarks>
	public class TlsUtilities
	{
		internal static byte[] ToByteArray(string str)
		{
			return Strings.ToByteArray(str);
		}

		internal static void WriteUint8(short i, Stream os)
		{
			os.WriteByte((byte)i);
		}

		internal static void WriteUint8(short i, byte[] buf, int offset)
		{
			buf[offset] = (byte)i;
		}

		internal static void WriteUint16(int i, Stream os)
		{
			os.WriteByte((byte)(i >> 8));
			os.WriteByte((byte)i);
		}

		internal static void WriteUint16(int i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 8);
			buf[offset + 1] = (byte)i;
		}

		internal static void WriteUint24(int i, Stream os)
		{
			os.WriteByte((byte)(i >> 16));
			os.WriteByte((byte)(i >> 8));
			os.WriteByte((byte)i);
		}

		internal static void WriteUint24(int i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 16);
			buf[offset + 1] = (byte)(i >> 8);
			buf[offset + 2] = (byte)(i);
		}

		internal static void WriteUint32(long i, Stream os)
		{
			os.WriteByte((byte)(i >> 24));
			os.WriteByte((byte)(i >> 16));
			os.WriteByte((byte)(i >> 8));
			os.WriteByte((byte)i);
		}

		internal static void WriteUint32(long i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 24);
			buf[offset + 1] = (byte)(i >> 16);
			buf[offset + 2] = (byte)(i >> 8);
			buf[offset + 3] = (byte)(i);
		}

		internal static void WriteUint64(long i, Stream os)
		{
			os.WriteByte((byte)(i >> 56));
			os.WriteByte((byte)(i >> 48));
			os.WriteByte((byte)(i >> 40));
			os.WriteByte((byte)(i >> 32));
			os.WriteByte((byte)(i >> 24));
			os.WriteByte((byte)(i >> 16));
			os.WriteByte((byte)(i >> 8));
			os.WriteByte((byte)i);
		}

		internal static void WriteUint64(long i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 56);
			buf[offset + 1] = (byte)(i >> 48);
			buf[offset + 2] = (byte)(i >> 40);
			buf[offset + 3] = (byte)(i >> 32);
			buf[offset + 4] = (byte)(i >> 24);
			buf[offset + 5] = (byte)(i >> 16);
			buf[offset + 6] = (byte)(i >> 8);
			buf[offset + 7] = (byte)(i);
		}

		internal static short ReadUint8(Stream inStr)
		{
			int i = inStr.ReadByte();
			if (i < 0)
			{
				throw new EndOfStreamException();
			}
			return (short)i;
		}

		internal static int ReadUint16(Stream inStr)
		{
			int i1 = inStr.ReadByte();
			int i2 = inStr.ReadByte();
			if ((i1 | i2) < 0)
			{
				throw new EndOfStreamException();
			}
			return i1 << 8 | i2;
		}

		internal static int ReadUint24(Stream inStr)
		{
			int i1 = inStr.ReadByte();
			int i2 = inStr.ReadByte();
			int i3 = inStr.ReadByte();
			if ((i1 | i2 | i3) < 0)
			{
				throw new EndOfStreamException();
			}
			return (i1 << 16) | (i2 << 8) | i3;
		}

		internal static long ReadUint32(Stream inStr)
		{
			int i1 = inStr.ReadByte();
			int i2 = inStr.ReadByte();
			int i3 = inStr.ReadByte();
			int i4 = inStr.ReadByte();
			if ((i1 | i2 | i3 | i4) < 0)
			{
				throw new EndOfStreamException();
			}
			// TODO Examine this
//			return (((long)i1) << 24) | (((long)i2) << 16) | (((long)i3) << 8) | ((long)i4);
			return ((long)i1 << 24) | ((long)i2 << 16) | ((long)i3 <<  8) | (uint)i4;
		}

		internal static void ReadFully(byte[] buf, Stream inStr)
		{
			if (Streams.ReadFully(inStr, buf, 0, buf.Length) < buf.Length)
				throw new EndOfStreamException();
		}

		internal static void CheckVersion(byte[] readVersion, TlsProtocolHandler handler)
		{
			if ((readVersion[0] != 3) || (readVersion[1] != 1))
			{
				handler.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_protocol_version);
			}
		}

		internal static void CheckVersion(Stream inStr, TlsProtocolHandler handler)
		{
			int i1 = inStr.ReadByte();
			int i2 = inStr.ReadByte();
			if ((i1 != 3) || (i2 != 1))
			{
				handler.FailWithError(TlsProtocolHandler.AL_fatal, TlsProtocolHandler.AP_protocol_version);
			}
		}

		internal static void WriteVersion(Stream os)
		{
			os.WriteByte(3);
			os.WriteByte(1);
		}

		private static void hmac_hash(IDigest digest, byte[] secret, byte[] seed, byte[] output)
		{
			HMac mac = new HMac(digest);
			KeyParameter param = new KeyParameter(secret);
			byte[] a = seed;
			int size = digest.GetDigestSize();
			int iterations = (output.Length + size - 1) / size;
			byte[] buf = new byte[mac.GetMacSize()];
			byte[] buf2 = new byte[mac.GetMacSize()];
			for (int i = 0; i < iterations; i++)
			{
				mac.Init(param);
				mac.BlockUpdate(a, 0, a.Length);
				mac.DoFinal(buf, 0);
				a = buf;
				mac.Init(param);
				mac.BlockUpdate(a, 0, a.Length);
				mac.BlockUpdate(seed, 0, seed.Length);
				mac.DoFinal(buf2, 0);
				Array.Copy(buf2, 0, output, (size * i), System.Math.Min(size, output.Length - (size * i)));
			}
		}

		internal static void PRF(
			byte[]	secret,
			byte[]	label,
			byte[]	seed,
			byte[]	buf)
		{
			int s_half = (secret.Length + 1) / 2;
			byte[] s1 = new byte[s_half];
			byte[] s2 = new byte[s_half];
			Array.Copy(secret, 0, s1, 0, s_half);
			Array.Copy(secret, secret.Length - s_half, s2, 0, s_half);

			byte[] ls = new byte[label.Length + seed.Length];
			Array.Copy(label, 0, ls, 0, label.Length);
			Array.Copy(seed, 0, ls, label.Length, seed.Length);

			byte[] prf = new byte[buf.Length];
			hmac_hash(new MD5Digest(), s1, ls, prf);
			hmac_hash(new Sha1Digest(), s2, ls, buf);
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] ^= prf[i];
			}
		}
	}
}
