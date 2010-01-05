using System;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>An output Stream for a TLS 1.0 connection.</remarks>
	// TODO Fix name and make internal once TlsProtocolHandler.TlsOuputStream is removed
	public class TlsOuputStream
		: BaseOutputStream
	{
		private readonly TlsProtocolHandler handler;

		internal TlsOuputStream(
			TlsProtocolHandler handler)
		{
			this.handler = handler;
		}

		public override void Write(
			byte[]	buf,
			int		offset,
			int		len)
		{
			this.handler.WriteData(buf, offset, len);
		}

		[Obsolete("Use version that takes a 'byte' argument")]
		public void WriteByte(int arg0)
		{
			this.Write((byte)arg0);
		}

		public override void WriteByte(byte b)
		{
			this.Write(b);
		}

		public override void Close()
		{
			handler.Close();
			base.Close();
		}

		public override void Flush()
		{
			handler.Flush();
		}
	}
}