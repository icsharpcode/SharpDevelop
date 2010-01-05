using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ParametersWithIV
		: ICipherParameters
    {
		private readonly ICipherParameters	parameters;
		private readonly byte[]				iv;

		public ParametersWithIV(
            ICipherParameters	parameters,
            byte[]				iv)
			: this(parameters, iv, 0, iv.Length)
		{
		}

		public ParametersWithIV(
            ICipherParameters	parameters,
            byte[]				iv,
            int					ivOff,
            int					ivLen)
        {
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			if (iv == null)
				throw new ArgumentNullException("iv");

			this.parameters = parameters;
			this.iv = new byte[ivLen];
            Array.Copy(iv, ivOff, this.iv, 0, ivLen);
        }

		public byte[] GetIV()
        {
			return (byte[]) iv.Clone();
        }

		public ICipherParameters Parameters
        {
            get { return parameters; }
        }
    }
}
