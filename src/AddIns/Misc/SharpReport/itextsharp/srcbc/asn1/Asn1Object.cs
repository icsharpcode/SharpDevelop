using System.IO;

namespace Org.BouncyCastle.Asn1
{
    public abstract class Asn1Object
		: Asn1Encodable
    {
		/// <summary>Create a base ASN.1 object from a byte array.</summary>
		/// <param name="data">The byte array to parse.</param>
		/// <returns>The base ASN.1 object represented by the byte array.</returns>
		/// <exception cref="IOException">If there is a problem parsing the data.</exception>
		public static Asn1Object FromByteArray(
			byte[] data)
		{
			return new Asn1InputStream(data).ReadObject();
		}

		/// <summary>Read a base ASN.1 object from a stream.</summary>
		/// <param name="inStr">The stream to parse.</param>
		/// <returns>The base ASN.1 object represented by the byte array.</returns>
		/// <exception cref="IOException">If there is a problem parsing the data.</exception>
		public static Asn1Object FromStream(
			Stream inStr)
		{
			return new Asn1InputStream(inStr).ReadObject();
		}

		public sealed override Asn1Object ToAsn1Object()
        {
            return this;
        }

		internal abstract void Encode(DerOutputStream derOut);

		protected abstract bool Asn1Equals(Asn1Object asn1Object);
		protected abstract int Asn1GetHashCode();

		internal bool CallAsn1Equals(Asn1Object obj)
		{
			return Asn1Equals(obj);
		}

		internal int CallAsn1GetHashCode()
		{
			return Asn1GetHashCode();
		}
	}
}
