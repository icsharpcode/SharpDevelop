using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Zlib;

namespace Org.BouncyCastle.Cms
{
	/**
	* General class for generating a compressed CMS message stream.
	* <p>
	* A simple example of usage.
	* </p>
	* <pre>
	*      CMSCompressedDataStreamGenerator gen = new CMSCompressedDataStreamGenerator();
	*
	*      Stream cOut = gen.Open(outputStream, CMSCompressedDataStreamGenerator.ZLIB);
	*
	*      cOut.Write(data);
	*
	*      cOut.Close();
	* </pre>
	*/
	public class CmsCompressedDataStreamGenerator
	{
		public const string ZLib = "1.2.840.113549.1.9.16.3.8";

		/**
		* base constructor
		*/
		public CmsCompressedDataStreamGenerator()
		{
		}

		public Stream Open(
			Stream	outStream,
			string	compressionOID)
		{
			return Open(outStream, CmsObjectIdentifiers.Data.Id, compressionOID);
		}

		public Stream Open(
			Stream	outStream,
			string	contentOID,
			string	compressionOID)
		{
			BerSequenceGenerator sGen = new BerSequenceGenerator(outStream);

			sGen.AddObject(CmsObjectIdentifiers.CompressedData);

			//
			// Compressed Data
			//
			BerSequenceGenerator cGen = new BerSequenceGenerator(
				sGen.GetRawOutputStream(), 0, true);

			// CMSVersion
			cGen.AddObject(new DerInteger(0));

			// CompressionAlgorithmIdentifier
			cGen.AddObject(new AlgorithmIdentifier(new DerObjectIdentifier(ZLib)));

			//
			// Encapsulated ContentInfo
			//
			BerSequenceGenerator eiGen = new BerSequenceGenerator(cGen.GetRawOutputStream());

			eiGen.AddObject(new DerObjectIdentifier(contentOID));

			BerOctetStringGenerator octGen = new BerOctetStringGenerator(
				eiGen.GetRawOutputStream(), 0, true);

			return new CmsCompressedOutputStream(
				new ZDeflaterOutputStream(octGen.GetOctetOutputStream()), sGen, cGen, eiGen);
		}

		private class CmsCompressedOutputStream
			: BaseOutputStream
		{
			private ZDeflaterOutputStream _out;
			private BerSequenceGenerator _sGen;
			private BerSequenceGenerator _cGen;
			private BerSequenceGenerator _eiGen;

			internal CmsCompressedOutputStream(
				ZDeflaterOutputStream	outStream,
				BerSequenceGenerator	sGen,
				BerSequenceGenerator	cGen,
				BerSequenceGenerator	eiGen)
			{
				_out = outStream;
				_sGen = sGen;
				_cGen = cGen;
				_eiGen = eiGen;
			}

			public override void WriteByte(
				byte b)
			{
				_out.WriteByte(b);
			}

			public override void Write(
				byte[]	bytes,
				int		off,
				int		len)
			{
				_out.Write(bytes, off, len);
			}

			public override void Close()
			{
				_out.Close();
				_eiGen.Close();
				_cGen.Close();
				_sGen.Close();
				base.Close();
			}
		}
	}
}
