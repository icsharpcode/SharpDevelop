using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class DHKeyGenerationParameters
		: KeyGenerationParameters
    {
        private readonly DHParameters parameters;

		public DHKeyGenerationParameters(
            SecureRandom	random,
            DHParameters	parameters)
			: base(random, parameters.P.BitLength)
        {
            this.parameters = parameters;
        }

		public DHParameters Parameters
        {
            get { return parameters; }
        }
    }
}
