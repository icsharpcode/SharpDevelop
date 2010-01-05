using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    /**
     * Base class for an application specific object
     */
    public class DerApplicationSpecific
        : Asn1Object
    {
		private readonly bool	isConstructed;
        private readonly int	tag;
        private readonly byte[]	octets;

		internal DerApplicationSpecific(
			bool	isConstructed,
			int		tag,
			byte[]	octets)
		{
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.octets = octets;
		}

		public DerApplicationSpecific(
            int		tag,
            byte[]	octets)
			: this(false, tag, octets)
        {
        }

		public DerApplicationSpecific(
			int				tag, 
			Asn1Encodable	obj) 
			: this(true, tag, obj)
		{
		}

		public DerApplicationSpecific(
			bool			isExplicit,
			int				tag,
			Asn1Encodable	obj)
		{
			if (tag >= 0x1f)
				throw new IOException("unsupported tag number");

			byte[] data = obj.GetDerEncoded();

			this.isConstructed = isExplicit;
			this.tag = tag;

			if (isExplicit)
			{
				this.octets = data;
			}
			else
			{
				int lenBytes = GetLengthOfLength(data);
				byte[] tmp = new byte[data.Length - lenBytes];
				Array.Copy(data, lenBytes, tmp, 0, tmp.Length);
				this.octets = tmp;
			}
		}

		private int GetLengthOfLength(
			byte[] data)
		{
			int count = 2;	// TODO: assumes only a 1 byte tag number

			while((data[count - 1] & 0x80) != 0)
			{
				count++;
			}

			return count;
		}

		public bool IsConstructed()
        {
			return isConstructed;
        }

		public byte[] GetContents()
        {
            return octets;
        }

		public int ApplicationTag
        {
            get { return tag; }
        }

		public Asn1Object GetObject()
        {
			return FromByteArray(GetContents());
		}

		/**
		 * Return the enclosed object assuming implicit tagging.
		 *
		 * @param derTagNo the type tag that should be applied to the object's contents.
		 * @return  the resulting object
		 * @throws IOException if reconstruction fails.
		 */
		public Asn1Object GetObject(
			int derTagNo)
		{
			if (tag >= 0x1f)
				throw new IOException("unsupported tag number");

			byte[] tmp = this.GetEncoded();
			tmp[0] = (byte) derTagNo;

			return FromByteArray(tmp);;
		}

		internal override void Encode(
			DerOutputStream derOut)
        {
			int classBits = Asn1Tags.Application;
			if (isConstructed)
			{
				classBits |= Asn1Tags.Constructed; 
			}

			derOut.WriteEncoded(classBits, tag, octets);
		}

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
        {
			DerApplicationSpecific other = asn1Object as DerApplicationSpecific;

			if (other == null)
				return false;

			return this.isConstructed == other.isConstructed
				&& this.tag == other.tag
				&& Arrays.AreEqual(this.octets, other.octets);
        }

		protected override int Asn1GetHashCode()
		{
			return isConstructed.GetHashCode() ^ tag.GetHashCode() ^ Arrays.GetHashCode(octets);
        }
    }
}
