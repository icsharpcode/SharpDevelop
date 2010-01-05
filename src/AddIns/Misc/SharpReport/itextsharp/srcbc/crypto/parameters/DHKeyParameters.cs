using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class DHKeyParameters
		: AsymmetricKeyParameter
    {
        private readonly DHParameters parameters;

		protected DHKeyParameters(
            bool			isPrivate,
            DHParameters	parameters)
			: base(isPrivate)
        {
			// TODO Should we allow parameters to be null?
            this.parameters = parameters;
        }

		public DHParameters Parameters
        {
            get { return parameters; }
        }

		public override bool Equals(
			object obj)
        {
			if (obj == this)
				return true;

			DHKeyParameters other = obj as DHKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			DHKeyParameters other)
		{
			return Platform.Equals(parameters, other.parameters)
				&& base.Equals(other);
		}

		public override int GetHashCode()
        {
			int hc = base.GetHashCode();

			if (parameters != null)
			{
				hc ^= parameters.GetHashCode();
			}

			return hc;
        }
    }
}
