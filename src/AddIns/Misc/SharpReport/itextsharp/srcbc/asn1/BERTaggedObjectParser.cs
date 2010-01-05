using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class BerTaggedObjectParser
		: Asn1TaggedObjectParser
	{
		private int _baseTag;
		private int _tagNumber;
		private Stream _contentStream;

		private bool _indefiniteLength;

		internal BerTaggedObjectParser(
			int		baseTag,
			int		tagNumber,
			Stream	contentStream)
		{
			if (!contentStream.CanRead)
				throw new ArgumentException("Expected stream to be readable", "contentStream");

			_baseTag = baseTag;
			_tagNumber = tagNumber;
			_contentStream = contentStream;
			_indefiniteLength = contentStream is IndefiniteLengthInputStream;
		}

		public bool IsConstructed
		{
			get { return (_baseTag & Asn1Tags.Constructed) != 0; }
		}

		public int TagNo
		{
			get { return _tagNumber; }
		}

		public IAsn1Convertible GetObjectParser(
			int		tag,
			bool	isExplicit)
		{
			if (isExplicit)
			{
				return new Asn1StreamParser(_contentStream).ReadObject();
			}

			switch (tag)
			{
				case Asn1Tags.Set:
					if (_indefiniteLength)
					{
						return new BerSetParser(new Asn1StreamParser(_contentStream));
					}
					else
					{
						return new DerSetParser(new Asn1StreamParser(_contentStream));
					}
				case Asn1Tags.Sequence:
					if (_indefiniteLength)
					{
						return new BerSequenceParser(new Asn1StreamParser(_contentStream));
					}
					else
					{
						return new DerSequenceParser(new Asn1StreamParser(_contentStream));
					}
				case Asn1Tags.OctetString:
					// TODO Is the handling of definite length constructed encodings correct?
					if (_indefiniteLength || IsConstructed)
					{
						return new BerOctetStringParser(new Asn1StreamParser(_contentStream));
					}
					else
					{
						return new DerOctetStringParser((DefiniteLengthInputStream)_contentStream);
					}
			}

			throw Platform.CreateNotImplementedException("implicit tagging");
		}

		private Asn1EncodableVector rLoadVector(Stream inStream)
		{
			try
			{
				return new Asn1StreamParser(inStream).ReadVector();
			}
			catch (IOException e)
			{
				throw new InvalidOperationException(e.Message, e);
			}
		}

		public Asn1Object ToAsn1Object()
		{
			if (_indefiniteLength)
			{
				Asn1EncodableVector v = rLoadVector(_contentStream);

				return v.Count == 1
					?	new BerTaggedObject(true, _tagNumber, v[0])
					:	new BerTaggedObject(false, _tagNumber, BerSequence.FromVector(v));
			}

			if (IsConstructed)
			{
				Asn1EncodableVector v = rLoadVector(_contentStream);

				return v.Count == 1
					?	new DerTaggedObject(true, _tagNumber, v[0])
					:	new DerTaggedObject(false, _tagNumber, DerSequence.FromVector(v));
			}

			try
			{
				DefiniteLengthInputStream defIn = (DefiniteLengthInputStream) _contentStream;
				return new DerTaggedObject(false, _tagNumber, new DerOctetString(defIn.ToArray()));
			}
			catch (IOException e)
			{
				throw new InvalidOperationException(e.Message, e);
			}
		}
	}
}
