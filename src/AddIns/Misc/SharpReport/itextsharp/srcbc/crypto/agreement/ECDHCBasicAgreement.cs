using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Agreement
{
    /**
     * P1363 7.2.2 ECSVDP-DHC
     *
     * ECSVDP-DHC is Elliptic Curve Secret Value Derivation Primitive,
     * Diffie-Hellman version with cofactor multiplication. It is based on
     * the work of [DH76], [Mil86], [Kob87], [LMQ98] and [Kal98a]. This
     * primitive derives a shared secret value from one party's private key
     * and another party's public key, where both have the same set of EC
     * domain parameters. If two parties correctly execute this primitive,
     * they will produce the same output. This primitive can be invoked by a
     * scheme to derive a shared secret key; specifically, it may be used
     * with the schemes ECKAS-DH1 and DL/ECKAS-DH2. It does not assume the
     * validity of the input public key (see also Section 7.2.1).
     * <p>
     * Note: As stated P1363 compatibility mode with ECDH can be preset, and
     * in this case the implementation doesn't have a ECDH compatibility mode
     * (if you want that just use ECDHBasicAgreement and note they both implement
     * BasicAgreement!).</p>
     */
    public class ECDHCBasicAgreement
		: IBasicAgreement
    {
        private ECPrivateKeyParameters key;

        public void Init(
            ICipherParameters parameters)
        {
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom) parameters).Parameters;
			}

			this.key = (ECPrivateKeyParameters)parameters;
        }

        public BigInteger CalculateAgreement(
            ICipherParameters pubKey)
        {
            ECPublicKeyParameters pub = (ECPublicKeyParameters) pubKey;
            ECDomainParameters parameters = pub.Parameters;
            ECPoint P = pub.Q.Multiply(parameters.H.Multiply(key.D));

            // if ( p.IsInfinity ) throw new Exception("Invalid public key");

            return P.X.ToBigInteger();
        }
    }

}
