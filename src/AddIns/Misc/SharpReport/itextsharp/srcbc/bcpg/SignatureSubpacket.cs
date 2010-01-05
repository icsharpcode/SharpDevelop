using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	/// <remarks>Basic type for a PGP Signature sub-packet.</remarks>
    public class SignatureSubpacket
    {
        private readonly SignatureSubpacketTag type;
        private readonly bool critical;

		internal readonly byte[] data;

		protected internal SignatureSubpacket(
            SignatureSubpacketTag	type,
            bool					critical,
            byte[]					data)
        {
            this.type = type;
            this.critical = critical;
            this.data = data;
        }

		public SignatureSubpacketTag SubpacketType
        {
			get { return type; }
        }

        public bool IsCritical()
        {
            return critical;
        }

		/// <summary>Return the generic data making up the packet.</summary>
        public byte[] GetData()
        {
            return (byte[]) data.Clone();
        }

		public void Encode(
            Stream os)
        {
            int bodyLen = data.Length + 1;

            if (bodyLen < 192)
            {
                os.WriteByte((byte)bodyLen);
            }
            else if (bodyLen <= 8383)
            {
                bodyLen -= 192;

                os.WriteByte((byte)(((bodyLen >> 8) & 0xff) + 192));
                os.WriteByte((byte)bodyLen);
            }
            else
            {
                os.WriteByte(0xff);
                os.WriteByte((byte)(bodyLen >> 24));
                os.WriteByte((byte)(bodyLen >> 16));
                os.WriteByte((byte)(bodyLen >> 8));
                os.WriteByte((byte)bodyLen);
            }

            if (critical)
            {
                os.WriteByte((byte)(0x80 | (int) type));
            }
            else
            {
                os.WriteByte((byte) type);
            }

            os.Write(data, 0, data.Length);
        }
    }
}
