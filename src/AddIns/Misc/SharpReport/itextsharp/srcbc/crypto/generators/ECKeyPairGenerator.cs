using System;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Generators
{
    public class ECKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
		private readonly string algorithm;

		private ECDomainParameters parameters;
		private DerObjectIdentifier publicKeyParamSet;
        private SecureRandom random;

		public ECKeyPairGenerator()
			: this("EC")
		{
		}

		public ECKeyPairGenerator(
			string algorithm)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			this.algorithm = VerifyAlgorithmName(algorithm);
		}

		public void Init(
            KeyGenerationParameters parameters)
        {
			if (parameters is ECKeyGenerationParameters)
			{
				ECKeyGenerationParameters ecP = (ECKeyGenerationParameters) parameters;

				if (ecP.PublicKeyParamSet != null)
				{
					if (algorithm != "ECGOST3410")
						throw new ArgumentException("parameters invalid for algorithm: " + algorithm, "parameters");

					this.publicKeyParamSet = ecP.PublicKeyParamSet;
				}

				this.parameters = ecP.DomainParameters;
			}
			else
			{
				DerObjectIdentifier oid;
				switch (parameters.Strength)
				{
					case 192:
						oid = X9ObjectIdentifiers.Prime192v1;
						break;
					case 239:
						oid = X9ObjectIdentifiers.Prime239v1;
						break;
					case 256:
						oid = X9ObjectIdentifiers.Prime256v1;
						break;
					default:
						throw new InvalidParameterException("unknown key size.");
				}

				X9ECParameters ecps = X962NamedCurves.GetByOid(oid);

				this.parameters = new ECDomainParameters(
					ecps.Curve, ecps.G, ecps.N, ecps.H, ecps.GetSeed());
			}

			this.random = parameters.Random;
		}

		/**
         * Given the domain parameters this routine Generates an EC key
         * pair in accordance with X9.62 section 5.2.1 pages 26, 27.
         */
        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            BigInteger n = parameters.N;
            BigInteger d;

            do
            {
                d = new BigInteger(n.BitLength, random);
            }
            while (d.SignValue == 0 || (d.CompareTo(n) >= 0));

            ECPoint q = parameters.G.Multiply(d);

			if (publicKeyParamSet != null)
			{
				return new AsymmetricCipherKeyPair(
					new ECPublicKeyParameters(q, publicKeyParamSet),
					new ECPrivateKeyParameters(d, publicKeyParamSet));
			}

			return new AsymmetricCipherKeyPair(
				new ECPublicKeyParameters(algorithm, q, parameters),
				new ECPrivateKeyParameters(algorithm, d, parameters));
		}

		private string VerifyAlgorithmName(
			string algorithm)
		{
			string upper = algorithm.ToUpper(CultureInfo.InvariantCulture);

			switch (upper)
			{
				case "EC":
				case "ECDSA":
				case "ECGOST3410":
				case "ECDH":
				case "ECDHC":
					break;
				default:
					throw new ArgumentException("unrecognised algorithm: " + algorithm, "algorithm");
			}

			return upper;
		}
	}
}
