using System;
using System.Diagnostics;
using System.IO;

using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	/**
	 * a general purpose ASN.1 decoder - note: this class differs from the
	 * others in that it returns null after it has read the last object in
	 * the stream. If an ASN.1 Null is encountered a Der/BER Null object is
	 * returned.
	 */
	public class Asn1InputStream
		: FilterStream
	{
		private readonly int limit;

		public Asn1InputStream(
			Stream inputStream)
			: this(inputStream, int.MaxValue)
		{
		}

		/**
		 * Create an ASN1InputStream where no DER object will be longer than limit.
		 *
		 * @param input stream containing ASN.1 encoded data.
		 * @param limit maximum size of a DER encoded object.
		 */
		public Asn1InputStream(
			Stream	inputStream,
			int		limit)
			: base(inputStream)
		{
			this.limit = limit;
		}

		/**
		 * Create an ASN1InputStream based on the input byte array. The length of DER objects in
		 * the stream is automatically limited to the length of the input array.
		 *
		 * @param input array containing ASN.1 encoded data.
		 */
		public Asn1InputStream(
			byte[] input)
			: this(new MemoryStream(input, false), input.Length)
		{
		}

		internal Asn1EncodableVector BuildEncodableVector()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			Asn1Object o;
			while ((o = ReadObject()) != null)
			{
				v.Add(o);
			}

			return v;
		}

		internal virtual Asn1EncodableVector BuildDerEncodableVector(
			DefiniteLengthInputStream dIn)
		{
			return new Asn1InputStream(dIn).BuildEncodableVector();
		}

		internal virtual DerSequence CreateDerSequence(
			DefiniteLengthInputStream dIn)
		{
			return DerSequence.FromVector(BuildDerEncodableVector(dIn));
		}

		internal virtual DerSet CreateDerSet(
			DefiniteLengthInputStream dIn)
		{
			return DerSet.FromVector(BuildDerEncodableVector(dIn), false);
		}

		public Asn1Object ReadObject()
		{
			int tag = ReadByte();
			if (tag <= 0)
			{
				if (tag == 0)
					throw new IOException("unexpected end-of-contents marker");

				return null;
			}

			//
			// calculate tag number
			//
			int tagNo = 0;
			if ((tag & Asn1Tags.Tagged) != 0 || (tag & Asn1Tags.Application) != 0)
			{
				tagNo = ReadTagNumber(this, tag);
			}

			bool isConstructed = (tag & Asn1Tags.Constructed) != 0;
			int baseTagNo = tag & ~Asn1Tags.Constructed;

			//
			// calculate length
			//
			int length = ReadLength(this, limit);

			if (length < 0) // indefinite length method
			{
				if (!isConstructed)
					throw new IOException("indefinite length primitive encoding encountered");

				IndefiniteLengthInputStream indIn = new IndefiniteLengthInputStream(this);

				if ((tag & Asn1Tags.Tagged) != 0)
				{
					return new BerTaggedObjectParser(tag, tagNo, indIn).ToAsn1Object();
				}

				Asn1StreamParser sp = new Asn1StreamParser(indIn);

				// TODO There are other tags that may be constructed (e.g. BitString)
				switch (baseTagNo)
				{
					case Asn1Tags.OctetString:
						return new BerOctetStringParser(sp).ToAsn1Object();
					case Asn1Tags.Sequence:
						return new BerSequenceParser(sp).ToAsn1Object();
					case Asn1Tags.Set:
						return new BerSetParser(sp).ToAsn1Object();
					default:
						throw new IOException("unknown BER object encountered");
				}
			}
			else
			{
				DefiniteLengthInputStream defIn = new DefiniteLengthInputStream(this, length);

				if ((tag & Asn1Tags.Application) != 0)
				{
					return new DerApplicationSpecific(isConstructed, tagNo, defIn.ToArray());
				}

				if ((tag & Asn1Tags.Tagged) != 0)
				{
					return new BerTaggedObjectParser(tag, tagNo, defIn).ToAsn1Object();
				}

				if (isConstructed)
				{
					// TODO There are other tags that may be constructed (e.g. BitString)
					switch (baseTagNo)
					{
						case Asn1Tags.OctetString:
							//
							// yes, people actually do this...
							//
							return new BerOctetString(BuildDerEncodableVector(defIn));
						case Asn1Tags.Sequence:
							return CreateDerSequence(defIn);
						case Asn1Tags.Set:
							return CreateDerSet(defIn);
						default:
							return new DerUnknownTag(tag, defIn.ToArray());
					}
				}

				return CreatePrimitiveDerObject(tag, defIn.ToArray());
			}
		}

		internal static int ReadTagNumber(
			Stream	s,
			int		tag)
		{
			int tagNo = tag & 0x1f;

			//
			// with tagged object tag number is bottom 5 bits, or stored at the start of the content
			//
			if (tagNo == 0x1f)
			{
				tagNo = 0;

				int b = s.ReadByte();

				// X.690-0207 8.1.2.4.2
				// "c) bits 7 to 1 of the first subsequent octet shall not all be zero."
				if ((b & 0x7f) == 0) // Note: -1 will pass
				{
					throw new IOException("corrupted stream - invalid high tag number found");
				}

				while ((b >= 0) && ((b & 0x80) != 0))
				{
					tagNo |= (b & 0x7f);
					tagNo <<= 7;
					b = s.ReadByte();
				}

				if (b < 0)
					throw new EndOfStreamException("EOF found inside tag value.");

				tagNo |= (b & 0x7f);
			}

			return tagNo;
		}

		internal static int ReadLength(
			Stream	s,
			int		limit)
		{
			int length = s.ReadByte();
			if (length < 0)
				throw new EndOfStreamException("EOF found when length expected");

			if (length == 0x80)
				return -1;      // indefinite-length encoding

			if (length > 127)
			{
				int size = length & 0x7f;

				if (size > 4)
					throw new IOException("DER length more than 4 bytes");

				length = 0;
				for (int i = 0; i < size; i++)
				{
					int next = s.ReadByte();

					if (next < 0)
						throw new EndOfStreamException("EOF found reading length");

					length = (length << 8) + next;
				}

				if (length < 0)
					throw new IOException("Corrupted stream - negative length found");

				if (length >= limit)   // after all we must have read at least 1 byte
					throw new IOException("Corrupted stream - out of bounds length found");
			}

			return length;
		}

		internal static Asn1Object CreatePrimitiveDerObject(
			int		tag,
			byte[]	bytes)
		{
			Debug.Assert((tag & (Asn1Tags.Application | Asn1Tags.Constructed | Asn1Tags.Tagged)) == 0);

			switch (tag)
			{
				case Asn1Tags.BitString:
				{
					int padBits = bytes[0];
					byte[] data = new byte[bytes.Length - 1];
					Array.Copy(bytes, 1, data, 0, bytes.Length - 1);
					return new DerBitString(data, padBits);
				}
				case Asn1Tags.BmpString:
					return new DerBmpString(bytes);
				case Asn1Tags.Boolean:
					return new DerBoolean(bytes);
				case Asn1Tags.Enumerated:
					return new DerEnumerated(bytes);
				case Asn1Tags.GeneralizedTime:
					return new DerGeneralizedTime(bytes);
				case Asn1Tags.GeneralString:
					return new DerGeneralString(bytes);
				case Asn1Tags.IA5String:
					return new DerIA5String(bytes);
				case Asn1Tags.Integer:
					return new DerInteger(bytes);
				case Asn1Tags.Null:
					return DerNull.Instance;   // actual content is ignored (enforce 0 length?)
				case Asn1Tags.NumericString:
					return new DerNumericString(bytes);
				case Asn1Tags.ObjectIdentifier:
					return new DerObjectIdentifier(bytes);
				case Asn1Tags.OctetString:
					return new DerOctetString(bytes);
				case Asn1Tags.PrintableString:
					return new DerPrintableString(bytes);
				case Asn1Tags.T61String:
					return new DerT61String(bytes);
				case Asn1Tags.UniversalString:
					return new DerUniversalString(bytes);
				case Asn1Tags.UtcTime:
					return new DerUtcTime(bytes);
				case Asn1Tags.Utf8String:
					return new DerUtf8String(bytes);
				case Asn1Tags.VisibleString:
					return new DerVisibleString(bytes);
				default:
					return new DerUnknownTag(tag, bytes);
			}
		}
	}
}
