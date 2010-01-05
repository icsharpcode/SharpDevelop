using System;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public abstract class ECKeyParameters
		: AsymmetricKeyParameter
    {
		private readonly string algorithm;
		private readonly ECDomainParameters parameters;
		private readonly DerObjectIdentifier publicKeyParamSet;

		protected ECKeyParameters(
			string				algorithm,
            bool				isPrivate,
            ECDomainParameters	parameters)
			: base(isPrivate)
        {
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			this.algorithm = VerifyAlgorithmName(algorithm);
			this.parameters = parameters;
        }

		protected ECKeyParameters(
			string				algorithm,
			bool				isPrivate,
			DerObjectIdentifier	publicKeyParamSet)
			: base(isPrivate)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");
			if (publicKeyParamSet == null)
				throw new ArgumentNullException("publicKeyParamSet");

			this.algorithm = VerifyAlgorithmName(algorithm);
			this.parameters = LookupParameters(publicKeyParamSet);
			this.publicKeyParamSet = publicKeyParamSet;
		}

		public string AlgorithmName
		{
			get { return algorithm; }
		}

		public ECDomainParameters Parameters
        {
			get { return parameters; }
        }

		public DerObjectIdentifier PublicKeyParamSet
		{
			get { return publicKeyParamSet; }
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			ECDomainParameters other = obj as ECDomainParameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			ECKeyParameters other)
		{
			return parameters.Equals(other.parameters) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return parameters.GetHashCode() ^ base.GetHashCode();
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

		private static ECDomainParameters LookupParameters(
			DerObjectIdentifier publicKeyParamSet)
		{
			if (publicKeyParamSet == null)
				throw new ArgumentNullException("publicKeyParamSet");

			ECDomainParameters p = ECGost3410NamedCurves.GetByOid(publicKeyParamSet);

			if (p == null)
				throw new ArgumentException("OID is not a valid CryptoPro public key parameter set", "publicKeyParamSet");

			return p;
		}
	}
}
