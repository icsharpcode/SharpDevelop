using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
    public class BerOctetString
        : DerOctetString, IEnumerable
    {
		private const int MaxLength = 1000;

		/**
         * convert a vector of octet strings into a single byte string
         */
        private static byte[] ToBytes(
            IEnumerable octs)
        {
            MemoryStream bOut = new MemoryStream();
			foreach (DerOctetString o in octs)
			{
                byte[] octets = o.GetOctets();
                bOut.Write(octets, 0, octets.Length);
            }
			return bOut.ToArray();
        }

		private readonly IEnumerable octs;

		/// <param name="str">The octets making up the octet string.</param>
		public BerOctetString(
			byte[] str)
			: base(str)
		{
		}

		public BerOctetString(
			IEnumerable octets)
			: base(ToBytes(octets))
        {
            this.octs = octets;
        }

        public BerOctetString(
			Asn1Object obj)
			: base(obj)
        {
        }

        public BerOctetString(
			Asn1Encodable obj)
			: base(obj.ToAsn1Object())
        {
        }

        public override byte[] GetOctets()
        {
            return str;
        }

        /**
         * return the DER octets that make up this string.
         */
		public IEnumerator GetEnumerator()
		{
			if (octs == null)
			{
				return GenerateOcts().GetEnumerator();
			}

			return octs.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
        public IEnumerator GetObjects()
        {
			return GetEnumerator();
		}

		private ArrayList GenerateOcts()
        {
            int start = 0;
            int end = 0;
            ArrayList vec = new ArrayList();

            while ((end + 1) < str.Length)
            {
                if (str[end] == 0 && str[end + 1] == 0)
                {
                    byte[] nStr = new byte[end - start + 1];

                    Array.Copy(str, start, nStr, 0, nStr.Length);

                    vec.Add(new DerOctetString(nStr));
                    start = end + 1;
                }
                end++;
            }

			byte[] nStr2 = new byte[str.Length - start];

			Array.Copy(str, start, nStr2, 0, nStr2.Length);

			vec.Add(new DerOctetString(nStr2));

			return vec;
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
            if (derOut is Asn1OutputStream || derOut is BerOutputStream)
            {
                derOut.WriteByte(Asn1Tags.Constructed | Asn1Tags.OctetString);

                derOut.WriteByte(0x80);

                //
                // write out the octet array
                //
                if (octs != null)
                {
                    foreach (DerOctetString oct in octs)
                    {
                        derOut.WriteObject(oct);
                    }
                }
                else
                {
					for (int i = 0; i < str.Length; i += MaxLength)
					{
						int end = System.Math.Min(str.Length, i + MaxLength);

						byte[] nStr = new byte[end - i];

						Array.Copy(str, i, nStr, 0, nStr.Length);

						derOut.WriteObject(new DerOctetString(nStr));
					}
                }

                derOut.WriteByte(0x00);
                derOut.WriteByte(0x00);
            }
            else
            {
                base.Encode(derOut);
            }
        }
    }
}
