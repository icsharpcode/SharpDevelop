using System;



namespace Org.BouncyCastle.Bcpg.Sig
{
    /**
    * packet giving signature creation time.
    */
    public class TrustSignature
        : SignatureSubpacket
    {
        private static byte[] IntToByteArray(
            int    v1,
            int    v2)
        {
            byte[]    data = new byte[2];

            data[0] = (byte)v1;
            data[1] = (byte)v2;

            return data;
        }

        public TrustSignature(
            bool    critical,
            byte[]     data)
            : base(SignatureSubpacketTag.TrustSig, critical, data)
        {
        }

        public TrustSignature(
            bool    critical,
            int        depth,
            int        trustAmount)
            : base(SignatureSubpacketTag.TrustSig, critical, IntToByteArray(depth, trustAmount))
        {
        }

        public int Depth
        {
			get
			{
				return data[0] & 0xff;
			}
        }

        public int TrustAmount
        {
			get
			{
				return data[1] & 0xff;
			}
        }
    }
}
