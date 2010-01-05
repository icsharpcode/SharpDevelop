using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Generators
{
    /**
     * a basic Diffie-Helman key pair generator.
     *
     * This Generates keys consistent for use with the basic algorithm for
     * Diffie-Helman.
     */
    public class DHBasicKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
        private DHKeyGenerationParameters param;

        public virtual void Init(
			KeyGenerationParameters parameters)
        {
            this.param = (DHKeyGenerationParameters) parameters;
        }

        public virtual AsymmetricCipherKeyPair GenerateKeyPair()
        {
			DHKeyGeneratorHelper helper = DHKeyGeneratorHelper.Instance;
			DHParameters dhParams = param.Parameters;

			BigInteger p = dhParams.P;
			BigInteger x = helper.CalculatePrivate(p, param.Random, dhParams.L);
			BigInteger y = helper.CalculatePublic(p, dhParams.G, x);

			return new AsymmetricCipherKeyPair(
                new DHPublicKeyParameters(y, dhParams),
                new DHPrivateKeyParameters(x, dhParams));
        }
    }

}
