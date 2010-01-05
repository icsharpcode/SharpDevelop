using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>
	/// Class to hold a single master secret key and its subkeys.
	/// <p>
	/// Often PGP keyring files consist of multiple master keys, if you are trying to process
	/// or construct one of these you should use the <c>PgpSecretKeyRingBundle</c> class.
	/// </p>
	/// </remarks>
	public class PgpSecretKeyRing
		: PgpKeyRing
    {
        private readonly ArrayList keys;

		internal PgpSecretKeyRing(
			ArrayList keys)
        {
            this.keys = keys;
        }

        public PgpSecretKeyRing(
            byte[] encoding)
            : this(new MemoryStream(encoding))
        {
        }

		public PgpSecretKeyRing(
            Stream inputStream)
        {
			this.keys = new ArrayList();

			BcpgInputStream bcpgInput = BcpgInputStream.Wrap(inputStream);

			PacketTag initialTag = bcpgInput.NextPacketTag();
			if (initialTag != PacketTag.SecretKey && initialTag != PacketTag.SecretSubkey)
            {
                throw new IOException("secret key ring doesn't start with secret key tag: "
					+ "tag 0x" + ((int)initialTag).ToString("X"));
            }

			SecretKeyPacket secret = (SecretKeyPacket) bcpgInput.ReadPacket();

			//
            // ignore GPG comment packets if found.
            //
            while (bcpgInput.NextPacketTag() == PacketTag.Experimental2)
            {
                bcpgInput.ReadPacket();
            }

			TrustPacket trust = ReadOptionalTrustPacket(bcpgInput);

			// revocation and direct signatures
			ArrayList keySigs = ReadSignaturesAndTrust(bcpgInput);

			ArrayList ids, idTrusts, idSigs;
			ReadUserIDs(bcpgInput, out ids, out idTrusts, out idSigs);

			keys.Add(new PgpSecretKey(secret, trust, keySigs, ids, idTrusts, idSigs));


			// Read subkeys
			while (bcpgInput.NextPacketTag() == PacketTag.SecretSubkey)
            {
                SecretSubkeyPacket sub = (SecretSubkeyPacket) bcpgInput.ReadPacket();

                //
                // ignore GPG comment packets if found.
                //
                while (bcpgInput.NextPacketTag() == PacketTag.Experimental2)
                {
                    bcpgInput.ReadPacket();
                }

				TrustPacket subTrust = ReadOptionalTrustPacket(bcpgInput);
				ArrayList sigList = ReadSignaturesAndTrust(bcpgInput);

				keys.Add(new PgpSecretKey(sub, subTrust, sigList));
            }
        }

		/// <summary>Return the public key for the master key.</summary>
        public PgpPublicKey GetPublicKey()
        {
            return ((PgpSecretKey) keys[0]).PublicKey;
        }

		/// <summary>Return the master private key.</summary>
        public PgpSecretKey GetSecretKey()
        {
            return (PgpSecretKey) keys[0];
        }

		/// <summary>Allows enumeration of the secret keys.</summary>
		/// <returns>An <c>IEnumerable</c> of <c>PgpSecretKey</c> objects.</returns>
		public IEnumerable GetSecretKeys()
        {
            return new EnumerableProxy(keys);
        }

        public PgpSecretKey GetSecretKey(
            long keyId)
        {
			foreach (PgpSecretKey k in keys)
			{
				if (keyId == k.KeyId)
				{
					return k;
				}
			}

			return null;
        }

		public byte[] GetEncoded()
        {
            MemoryStream bOut = new MemoryStream();

            Encode(bOut);

            return bOut.ToArray();
        }

        public void Encode(
            Stream outStr)
        {
			if (outStr == null)
				throw new ArgumentNullException("outStr");

			foreach (PgpSecretKey k in keys)
			{
				k.Encode(outStr);
			}
        }

		/// <summary>
		/// Returns a new key ring with the secret key passed in either added or
		/// replacing an existing one with the same key ID.
		/// </summary>
		/// <param name="secRing">The secret key ring to be modified.</param>
		/// <param name="secKey">The secret key to be inserted.</param>
		/// <returns>A new <c>PgpSecretKeyRing</c></returns>
		public static PgpSecretKeyRing InsertSecretKey(
            PgpSecretKeyRing  secRing,
            PgpSecretKey      secKey)
        {
            ArrayList keys = new ArrayList(secRing.keys);
            bool found = false;
			bool masterFound = false;

			for (int i = 0; i != keys.Count; i++)
            {
                PgpSecretKey key = (PgpSecretKey) keys[i];

				if (key.KeyId == secKey.KeyId)
                {
                    found = true;
                    keys[i] = secKey;
                }
				if (key.IsMasterKey)
				{
					masterFound = true;
				}
			}

            if (!found)
            {
				if (secKey.IsMasterKey && masterFound)
					throw new ArgumentException("cannot add a master key to a ring that already has one");

				keys.Add(secKey);
            }

            return new PgpSecretKeyRing(keys);
        }

		/// <summary>Returns a new key ring with the secret key passed in removed from the key ring.</summary>
		/// <param name="secRing">The secret key ring to be modified.</param>
		/// <param name="secKey">The secret key to be removed.</param>
		/// <returns>A new <c>PgpSecretKeyRing</c>, or null if secKey is not found.</returns>
        public static PgpSecretKeyRing RemoveSecretKey(
            PgpSecretKeyRing  secRing,
            PgpSecretKey      secKey)
        {
            ArrayList keys = new ArrayList(secRing.keys);
            bool found = false;

			for (int i = 0; i < keys.Count; i++)
            {
                PgpSecretKey key = (PgpSecretKey)keys[i];

				if (key.KeyId == secKey.KeyId)
                {
                    found = true;
                    keys.RemoveAt(i);
                }
            }

			return found ? new PgpSecretKeyRing(keys) : null;
        }
    }
}
