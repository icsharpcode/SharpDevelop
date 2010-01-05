using System;
using System.Collections;
using System.Text;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Asn1.Utilities
{
    public sealed class Asn1Dump
    {
		private static readonly string NewLine = Platform.NewLine;

		private Asn1Dump()
        {
        }

        private const string Tab = "    ";

        /**
         * dump a Der object as a formatted string with indentation
         *
         * @param obj the Asn1Object to be dumped out.
         */
        private static string AsString(
            string		indent,
            Asn1Object	obj)
        {
            if (obj is Asn1Sequence)
            {
                StringBuilder buf = new StringBuilder(indent);

				string tab = indent + Tab;

                if (obj is BerSequence)
                {
                    buf.Append("BER Sequence");
                }
                else if (obj is DerSequence)
                {
                    buf.Append("DER Sequence");
                }
                else
                {
                    buf.Append("Sequence");
                }

                buf.Append(NewLine);

				foreach (Asn1Encodable o in ((Asn1Sequence)obj))
				{
                    if (o == null || o is Asn1Null)
                    {
                        buf.Append(tab);
                        buf.Append("NULL");
                        buf.Append(NewLine);
                    }
                    else
                    {
                        buf.Append(AsString(tab, o.ToAsn1Object()));
                    }
                }
                return buf.ToString();
            }
            else if (obj is DerTaggedObject)
            {
                StringBuilder buf = new StringBuilder();
                string tab = indent + Tab;

				buf.Append(indent);
                if (obj is BerTaggedObject)
                {
                    buf.Append("BER Tagged [");
                }
                else
                {
                    buf.Append("Tagged [");
                }

				DerTaggedObject o = (DerTaggedObject)obj;

				buf.Append(((int)o.TagNo).ToString());
                buf.Append(']');

				if (!o.IsExplicit())
                {
                    buf.Append(" IMPLICIT ");
                }

				buf.Append(NewLine);

				if (o.IsEmpty())
                {
                    buf.Append(tab);
                    buf.Append("EMPTY");
                    buf.Append(NewLine);
                }
                else
                {
                    buf.Append(AsString(tab, o.GetObject()));
                }

				return buf.ToString();
            }
            else if (obj is BerSet)
            {
                StringBuilder buf = new StringBuilder();
                string tab = indent + Tab;

				buf.Append(indent);
                buf.Append("BER Set");
                buf.Append(NewLine);

				foreach (Asn1Encodable o in ((Asn1Set)obj))
				{
                    if (o == null)
                    {
                        buf.Append(tab);
                        buf.Append("NULL");
                        buf.Append(NewLine);
                    }
                    else
                    {
                        buf.Append(AsString(tab, o.ToAsn1Object()));
                    }
                }

				return buf.ToString();
            }
            else if (obj is DerSet)
            {
                StringBuilder buf = new StringBuilder();
                string tab = indent + Tab;

				buf.Append(indent);
                buf.Append("DER Set");
                buf.Append(NewLine);

				foreach (Asn1Encodable o in ((Asn1Set)obj))
				{
                    if (o == null)
                    {
                        buf.Append(tab);
                        buf.Append("NULL");
                        buf.Append(NewLine);
                    }
                    else
                    {
                        buf.Append(AsString(tab, o.ToAsn1Object()));
                    }
                }

				return buf.ToString();
            }
            else if (obj is DerObjectIdentifier)
            {
                return indent + "ObjectIdentifier(" + ((DerObjectIdentifier)obj).Id + ")" + NewLine;
            }
            else if (obj is DerBoolean)
            {
                return indent + "Boolean(" + ((DerBoolean)obj).IsTrue + ")" + NewLine;
            }
            else if (obj is DerInteger)
            {
                return indent + "Integer(" + ((DerInteger)obj).Value + ")" + NewLine;
            }
			else if (obj is BerOctetString)
			{
				return indent + "BER Octet String" + "[" + ((Asn1OctetString)obj).GetOctets().Length + "] " + NewLine;
			}
            else if (obj is DerOctetString)
            {
				return indent + "DER Octet String" + "[" + ((Asn1OctetString)obj).GetOctets().Length + "] " + NewLine;
			}
			else if (obj is DerBitString)
			{
				return indent + "DER Bit String" + "[" + ((DerBitString)obj).GetBytes().Length + ", " + ((DerBitString)obj).PadBits + "] " + NewLine;
			}
            else if (obj is DerIA5String)
            {
                return indent + "IA5String(" + ((DerIA5String)obj).GetString() + ") " + NewLine;
            }
			else if (obj is DerUtf8String)
			{
				return indent + "UTF8String(" + ((DerUtf8String)obj).GetString() + ") " + NewLine;
			}
            else if (obj is DerPrintableString)
            {
                return indent + "PrintableString(" + ((DerPrintableString)obj).GetString() + ") " + NewLine;
            }
            else if (obj is DerVisibleString)
            {
                return indent + "VisibleString(" + ((DerVisibleString)obj).GetString() + ") " + NewLine;
            }
            else if (obj is DerBmpString)
            {
                return indent + "BMPString(" + ((DerBmpString)obj).GetString() + ") " + NewLine;
            }
            else if (obj is DerT61String)
            {
                return indent + "T61String(" + ((DerT61String)obj).GetString() + ") " + NewLine;
            }
            else if (obj is DerUtcTime)
            {
                return indent + "UTCTime(" + ((DerUtcTime)obj).TimeString + ") " + NewLine;
            }
			else if (obj is DerGeneralizedTime)
			{
				return indent + "GeneralizedTime(" + ((DerGeneralizedTime)obj).GetTime() + ") " + NewLine;
			}
            else if (obj is DerUnknownTag)
            {
				byte[] hex = Hex.Encode(((DerUnknownTag)obj).GetData());
                return indent + "Unknown " + ((int)((DerUnknownTag)obj).Tag).ToString("X") + " "
                    + Encoding.ASCII.GetString(hex, 0, hex.Length) + NewLine;
            }
            else
            {
                return indent + obj.ToString() + NewLine;
            }
        }

		[Obsolete("Use version accepting Asn1Encodable")]
		public static string DumpAsString(
            object   obj)
        {
            if (obj is Asn1Object)
            {
                return AsString("", (Asn1Object)obj);
            }
            else if (obj is Asn1Encodable)
            {
                return AsString("", ((Asn1Encodable)obj).ToAsn1Object());
            }

            return "unknown object type " + obj.ToString();
        }

		/**
		 * dump out a DER object as a formatted string
		 *
		 * @param obj the Asn1Encodable to be dumped out.
		 */
		public static string DumpAsString(
			Asn1Encodable obj)
		{
			return AsString("", obj.ToAsn1Object());
		}
    }
}
