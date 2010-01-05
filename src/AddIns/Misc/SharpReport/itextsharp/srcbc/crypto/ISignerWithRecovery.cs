
using System;
using System.Text;

namespace Org.BouncyCastle.Crypto
{
    /**
     * Signer with message recovery.
     */
    public interface ISignerWithRecovery
        : ISigner
    {
        /**
         * Returns true if the signer has recovered the full message as
         * part of signature verification.
         *
         * @return true if full message recovered.
         */
        bool HasFullMessage();

        /**
         * Returns a reference to what message was recovered (if any).
         *
         * @return full/partial message, null if nothing.
         */
        byte[] GetRecoveredMessage();
    }
}
