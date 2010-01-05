using System;
using System.Text;

namespace Org.BouncyCastle.Asn1
{
    /**
     * Der PrintableString object.
     */
    public class DerPrintableString
        : DerStringBase
    {
        private readonly string str;

		/**
         * return a printable string from the passed in object.
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerPrintableString GetInstance(
            object obj)
        {
            if (obj == null || obj is DerPrintableString)
            {
                return (DerPrintableString)obj;
            }

            if (obj is Asn1OctetString)
            {
                return new DerPrintableString(((Asn1OctetString)obj).GetOctets());
            }

            if (obj is Asn1TaggedObject)
            {
                return GetInstance(((Asn1TaggedObject)obj).GetObject());
            }

            throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return a Printable string from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerPrintableString GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(obj.GetObject());
        }

        /**
         * basic constructor - byte encoded string.
         */
        public DerPrintableString(
            byte[] str)
			: this(Encoding.ASCII.GetString(str, 0, str.Length), false)
        {
        }

		/**
		 * basic constructor - this does not validate the string
		 */
		public DerPrintableString(
			string str)
			: this(str, false)
		{
		}

		/**
		* Constructor with optional validation.
		*
		* @param string the base string to wrap.
		* @param validate whether or not to check the string.
		* @throws ArgumentException if validate is true and the string
		* contains characters that should not be in a PrintableString.
		*/
		public DerPrintableString(
			string	str,
			bool	validate)
		{
			if (str == null)
				throw new ArgumentNullException("str");
			if (validate && !IsPrintableString(str))
				throw new ArgumentException("string contains illegal characters", "str");

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
            derOut.WriteEncoded(Asn1Tags.PrintableString, GetOctets());
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerPrintableString other = asn1Object as DerPrintableString;

			if (other == null)
				return false;

			return this.str.Equals(other.str);
        }

		/**
		 * return true if the passed in String can be represented without
		 * loss as a PrintableString, false otherwise.
		 *
		 * @return true if in printable set, false otherwise.
		 */
		public static bool IsPrintableString(
			string str)
		{
			foreach (char ch in str)
			{
				if (ch > 0x007f)
					return false;

				if (char.IsLetterOrDigit(ch))
					continue;

//				if (char.IsPunctuation(ch))
//					continue;

				switch (ch)
				{
					case ' ':
					case '\'':
					case '(':
					case ')':
					case '+':
					case '-':
					case '.':
					case ':':
					case '=':
					case '?':
					case '/':
					case ',':
						continue;
				}

				return false;
			}

			return true;
		}
	}
}
