using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Nist
{
    public sealed class NistObjectIdentifiers
    {
		private NistObjectIdentifiers()
		{
		}

        //
        // NIST
        //     iso/itu(2) joint-assign(16) us(840) organization(1) gov(101) csor(3)

        //
        // nistalgorithms(4)
        //
        public static readonly DerObjectIdentifier NistAlgorithm = new DerObjectIdentifier("2.16.840.1.101.3.4");

        public static readonly DerObjectIdentifier IdSha256 = new DerObjectIdentifier(NistAlgorithm + ".2.1");
        public static readonly DerObjectIdentifier IdSha384 = new DerObjectIdentifier(NistAlgorithm + ".2.2");
        public static readonly DerObjectIdentifier IdSha512 = new DerObjectIdentifier(NistAlgorithm + ".2.3");
        public static readonly DerObjectIdentifier IdSha224 = new DerObjectIdentifier(NistAlgorithm + ".2.4");

        public static readonly DerObjectIdentifier Aes = new DerObjectIdentifier(NistAlgorithm + ".1");

        public static readonly DerObjectIdentifier IdAes128Ecb	= new DerObjectIdentifier(Aes + ".1");
        public static readonly DerObjectIdentifier IdAes128Cbc	= new DerObjectIdentifier(Aes + ".2");
        public static readonly DerObjectIdentifier IdAes128Ofb	= new DerObjectIdentifier(Aes + ".3");
        public static readonly DerObjectIdentifier IdAes128Cfb	= new DerObjectIdentifier(Aes + ".4");
        public static readonly DerObjectIdentifier IdAes128Wrap	= new DerObjectIdentifier(Aes + ".5");

        public static readonly DerObjectIdentifier IdAes192Ecb	= new DerObjectIdentifier(Aes + ".21");
        public static readonly DerObjectIdentifier IdAes192Cbc	= new DerObjectIdentifier(Aes + ".22");
        public static readonly DerObjectIdentifier IdAes192Ofb	= new DerObjectIdentifier(Aes + ".23");
        public static readonly DerObjectIdentifier IdAes192Cfb	= new DerObjectIdentifier(Aes + ".24");
        public static readonly DerObjectIdentifier IdAes192Wrap	= new DerObjectIdentifier(Aes + ".25");

        public static readonly DerObjectIdentifier IdAes256Ecb	= new DerObjectIdentifier(Aes + ".41");
        public static readonly DerObjectIdentifier IdAes256Cbc	= new DerObjectIdentifier(Aes + ".42");
        public static readonly DerObjectIdentifier IdAes256Ofb	= new DerObjectIdentifier(Aes + ".43");
        public static readonly DerObjectIdentifier IdAes256Cfb	= new DerObjectIdentifier(Aes + ".44");
        public static readonly DerObjectIdentifier IdAes256Wrap	= new DerObjectIdentifier(Aes + ".45");

		//
		// signatures
		//
		public static readonly DerObjectIdentifier IdDsaWithSha2 = new DerObjectIdentifier(NistAlgorithm + ".3");

		public static readonly DerObjectIdentifier DsaWithSha224 = new DerObjectIdentifier(IdDsaWithSha2 + ".1");
		public static readonly DerObjectIdentifier DsaWithSha256 = new DerObjectIdentifier(IdDsaWithSha2 + ".2");
		public static readonly DerObjectIdentifier DsaWithSha384 = new DerObjectIdentifier(IdDsaWithSha2 + ".3");
		public static readonly DerObjectIdentifier DsaWithSha512 = new DerObjectIdentifier(IdDsaWithSha2 + ".4"); 
	}
}
