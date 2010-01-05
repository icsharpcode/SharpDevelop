using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>An implementation of the TLS 1.0 record layer.</remarks>
	public class RecordStream
	{
		private TlsProtocolHandler handler;
		private Stream inStr;
		private Stream outStr;
		internal CombinedHash hash1;
		internal CombinedHash hash2;
		internal TlsCipherSuite readSuite = null;
		internal TlsCipherSuite writeSuite = null;

		internal RecordStream(
			TlsProtocolHandler	handler,
			Stream				inStr,
			Stream				outStr)
		{
			this.handler = handler;
			this.inStr = inStr;
			this.outStr = outStr;
			hash1 = new CombinedHash();
			hash2 = new CombinedHash();
			this.readSuite = new TlsNullCipherSuite();
			this.writeSuite = this.readSuite;
		}

		public void ReadData()
		{
			short type = TlsUtilities.ReadUint8(inStr);
			TlsUtilities.CheckVersion(inStr, handler);
			int size = TlsUtilities.ReadUint16(inStr);
			byte[] buf = DecodeAndVerify(type, inStr, size);
			handler.ProcessData(type, buf, 0, buf.Length);

		}

		internal byte[] DecodeAndVerify(
			short	type,
			Stream	inStr,
			int		len)
		{
			byte[] buf = new byte[len];
			TlsUtilities.ReadFully(buf, inStr);
			byte[] result = readSuite.DecodeCiphertext(type, buf, 0, buf.Length, handler);
			return result;
		}

		internal void WriteMessage(
			short	type,
			byte[]	message,
			int		offset,
			int		len)
		{
			if (type == 22)
			{
				hash1.BlockUpdate(message, offset, len);
				hash2.BlockUpdate(message, offset, len);
			}
			byte[] ciphertext = writeSuite.EncodePlaintext(type, message, offset, len);
			byte[] writeMessage = new byte[ciphertext.Length + 5];
			TlsUtilities.WriteUint8(type, writeMessage, 0);
			TlsUtilities.WriteUint8((short)3, writeMessage, 1);
			TlsUtilities.WriteUint8((short)1, writeMessage, 2);
			TlsUtilities.WriteUint16(ciphertext.Length, writeMessage, 3);
			Array.Copy(ciphertext, 0, writeMessage, 5, ciphertext.Length);
			outStr.Write(writeMessage, 0, writeMessage.Length);
			outStr.Flush();
		}

		internal void Close()
		{
			IOException e = null;
			try
			{
				inStr.Close();
			}
			catch (IOException ex)
			{
				e = ex;
			}

			try
			{
				// NB: This is harmless if outStr == inStr
				outStr.Close();
			}
			catch (IOException ex)
			{
				e = ex;
			}

			if (e != null)
			{
				throw e;
			}
		}

		internal void Flush()
		{
			outStr.Flush();
		}
	}
}
