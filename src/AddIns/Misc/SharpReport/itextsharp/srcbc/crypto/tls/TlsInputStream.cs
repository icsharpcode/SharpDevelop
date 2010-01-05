using System;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>An input Stream for a TLS 1.0 connection.</remarks>
	// TODO Fix name and make internal once TlsProtocolHandler.TlsInputStream is removed
	public class TlsInputStream
		: BaseInputStream
	{
		private readonly TlsProtocolHandler handler;

		internal TlsInputStream(
			TlsProtocolHandler handler)
		{
			this.handler = handler;
		}

		public override int Read(
			byte[]	buf,
			int		offset,
			int		len)
		{
			return this.handler.ReadApplicationData(buf, offset, len);
		}

		public override int ReadByte()
		{
			byte[] buf = new byte[1];
			if (this.Read(buf, 0, 1) <= 0)
				return -1;
			return buf[0];
		}

		public override void Close()
		{
			handler.Close();
			base.Close();
		}
	}
}
