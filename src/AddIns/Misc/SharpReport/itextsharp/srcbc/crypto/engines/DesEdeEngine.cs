using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Engines
{
	/// <remarks>A class that provides a basic DESede (or Triple DES) engine.</remarks>
    public class DesEdeEngine
		: DesEngine
    {
        private int[] workingKey1, workingKey2, workingKey3;
        private bool forEncryption;

		/**
        * initialise a DESede cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public override void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
            if (!(parameters is KeyParameter))
            {
                throw new ArgumentException("invalid parameter passed to DESede init - " + parameters.GetType().ToString());
            }

			byte[] keyMaster = ((KeyParameter)parameters).GetKey();
            byte[] key1 = new byte[8], key2 = new byte[8], key3 = new byte[8];
            this.forEncryption = forEncryption;
            if (keyMaster.Length == 24)
            {
                Array.Copy(keyMaster, 0, key1, 0, key1.Length);
                Array.Copy(keyMaster, 8, key2, 0, key2.Length);
                Array.Copy(keyMaster, 16, key3, 0, key3.Length);
                workingKey1 = GenerateWorkingKey(forEncryption, key1);
                workingKey2 = GenerateWorkingKey(!forEncryption, key2);
                workingKey3 = GenerateWorkingKey(forEncryption, key3);
            }
            else        // 16 byte key
            {
                Array.Copy(keyMaster, 0, key1, 0, key1.Length);
                Array.Copy(keyMaster, 8, key2, 0, key2.Length);
                workingKey1 = GenerateWorkingKey(forEncryption, key1);
                workingKey2 = GenerateWorkingKey(!forEncryption, key2);
                workingKey3 = workingKey1;
            }
        }

		public override string AlgorithmName
        {
            get { return "DESede"; }
        }

		public override int GetBlockSize()
        {
            return BLOCK_SIZE;
        }

		public override int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
            if (workingKey1 == null)
                throw new InvalidOperationException("DESede engine not initialised");
            if ((inOff + BLOCK_SIZE) > input.Length)
                throw new DataLengthException("input buffer too short");
            if ((outOff + BLOCK_SIZE) > output.Length)
                throw new DataLengthException("output buffer too short");

			if (forEncryption)
            {
                DesFunc(workingKey1, input, inOff, output, outOff);
                DesFunc(workingKey2, output, outOff, output, outOff);
                DesFunc(workingKey3, output, outOff, output, outOff);
            }
            else
            {
                DesFunc(workingKey3, input, inOff, output, outOff);
                DesFunc(workingKey2, output, outOff, output, outOff);
                DesFunc(workingKey1, output, outOff, output, outOff);
            }

			return BLOCK_SIZE;
        }

		public override void Reset()
        {
        }
    }
}
