using System;
using System.Text;

namespace Org.BouncyCastle.Asn1
{
    public class DerGeneralString
        : DerStringBase
    {
        private readonly string str;

        public static DerGeneralString GetInstance(
            object obj)
        {
            if (obj == null || obj is DerGeneralString)
            {
                return (DerGeneralString) obj;
            }
            if (obj is Asn1OctetString)
            {
                return new DerGeneralString(((Asn1OctetString) obj).GetOctets());
            }
            if (obj is Asn1TaggedObject)
            {
                return GetInstance(((Asn1TaggedObject) obj).GetObject());
            }
            throw new ArgumentException("illegal object in GetInstance: "
                    + obj.GetType().Name);
        }

        public static DerGeneralString GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(obj.GetObject());
        }

        public DerGeneralString(
			byte[] str)
			: this(Encoding.ASCII.GetString(str, 0, str.Length))
        {
        }

		public DerGeneralString(
			string str)
        {
			if (str == null)
				throw new ArgumentNullException("str");

			this.str = str;
        }

        public override string GetString()
        {
            return str;
        }

		public byte[] GetOctets()
        {
			return Encoding.ASCII.GetBytes(str);
        }

		internal override void Encode(
			DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.GeneralString, GetOctets());
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
        {
			DerGeneralString other = asn1Object as DerGeneralString;

			if (other == null)
				return false;

			return this.str.Equals(other.str);
        }
    }
}
