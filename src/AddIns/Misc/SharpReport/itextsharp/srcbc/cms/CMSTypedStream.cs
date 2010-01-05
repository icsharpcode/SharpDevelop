using System;
using System.IO;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsTypedStream
	{
		private const int BufferSize = 32 * 1024;

		private readonly string	_oid;
		private readonly Stream	_in;
		private readonly int	_bufSize;

		public CmsTypedStream(
			Stream inStream)
			: this(PkcsObjectIdentifiers.Data.Id, inStream, BufferSize)
		{
		}

		public CmsTypedStream(
			string oid,
			Stream inStream)
			: this(oid, inStream, BufferSize)
		{
		}

		public CmsTypedStream(
			string	oid,
			Stream	inStream,
			int		bufSize)
		{
			_oid = oid;
			_bufSize = bufSize;
			_in = new FullReaderStream(inStream, bufSize);
		}

		public string ContentType
		{
			get { return _oid; }
		}

		public Stream ContentStream
		{
			get { return _in; }
		}

		public void Drain()
		{
			Streams.Drain(_in);
			_in.Close();
		}

		private class FullReaderStream
			: BaseInputStream
		{
			internal Stream _stream;

			internal FullReaderStream(
				Stream	inStream,
				int		bufSize)
			{
				_stream = inStream;
			}

			public override int ReadByte()
			{
				return _stream.ReadByte();
			}

			public override int Read(
				byte[]	buf,
				int		off,
				int		len)
			{
				return Streams.ReadFully(_stream, buf, off, len);
			}

			public override void Close()
			{
				_stream.Close();
				base.Close();
			}
		}
	}
}
