using System;
using System.Collections;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Macs
{
    /**
    * HMAC implementation based on RFC2104
    *
    * H(K XOR opad, H(K XOR ipad, text))
    */
    public class HMac : IMac
    {
        private const byte IPAD = (byte)0x36;
        private const byte OPAD = (byte)0x5C;

        private readonly IDigest digest;
        private readonly int digestSize;
        private readonly int blockLength;

		private byte[] inputPad;
        private byte[] outputPad;

        public HMac(
            IDigest digest)
        {
            this.digest = digest;
            digestSize = digest.GetDigestSize();

            blockLength = digest.GetByteLength();

            inputPad = new byte[blockLength];
            outputPad = new byte[blockLength];
        }

        public string AlgorithmName
        {
            get { return digest.AlgorithmName + "/HMAC"; }
        }

		public IDigest GetUnderlyingDigest()
        {
            return digest;
        }

        public void Init(
            ICipherParameters parameters)
        {
            digest.Reset();

            byte[] key = ((KeyParameter)parameters).GetKey();

            if (key.Length > blockLength)
            {
                digest.BlockUpdate(key, 0, key.Length);
                digest.DoFinal(inputPad, 0);
                for (int i = digestSize; i < inputPad.Length; i++)
                {
                    inputPad[i] = 0;
                }
            }
            else
            {
                Array.Copy(key, 0, inputPad, 0, key.Length);
                for (int i = key.Length; i < inputPad.Length; i++)
                {
                    inputPad[i] = 0;
                }
            }

            outputPad = new byte[inputPad.Length];
            Array.Copy(inputPad, 0, outputPad, 0, inputPad.Length);

            for (int i = 0; i < inputPad.Length; i++)
            {
                inputPad[i] ^= IPAD;
            }

            for (int i = 0; i < outputPad.Length; i++)
            {
                outputPad[i] ^= OPAD;
            }

            digest.BlockUpdate(inputPad, 0, inputPad.Length);
        }

        public int GetMacSize()
        {
            return digestSize;
        }

        public void Update(
            byte input)
        {
            digest.Update(input);
        }

        public void BlockUpdate(
            byte[] input,
            int inOff,
            int len)
        {
            digest.BlockUpdate(input, inOff, len);
        }

        public int DoFinal(
            byte[] output,
            int outOff)
        {
            byte[] tmp = new byte[digestSize];
            digest.DoFinal(tmp, 0);

            digest.BlockUpdate(outputPad, 0, outputPad.Length);
            digest.BlockUpdate(tmp, 0, tmp.Length);

            int     len = digest.DoFinal(output, outOff);

            Reset();

            return len;
        }

        /**
        * Reset the mac generator.
        */
        public void Reset()
        {
            /*
            * reset the underlying digest.
            */
            digest.Reset();

            /*
            * reinitialize the digest.
            */
            digest.BlockUpdate(inputPad, 0, inputPad.Length);
        }
    }
}
