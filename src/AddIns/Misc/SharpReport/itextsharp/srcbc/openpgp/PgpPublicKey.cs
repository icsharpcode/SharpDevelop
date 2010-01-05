using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>General class to handle a PGP public key object.</remarks>
    public class PgpPublicKey
    {
		private static readonly int[] MasterKeyCertificationTypes = new int[]
		{
			PgpSignature.PositiveCertification,
			PgpSignature.CasualCertification,
			PgpSignature.NoCertification,
			PgpSignature.DefaultCertification
		};

		private long				keyId;
        private byte[]				fingerprint;
        private int					keyStrength;

		internal PublicKeyPacket	publicPk;
        private TrustPacket			trustPk;
        private ArrayList			keySigs = new ArrayList();
        private ArrayList			ids = new ArrayList();
        private ArrayList			idTrusts = new ArrayList();
        private ArrayList			idSigs = new ArrayList();
        private ArrayList			subSigs;

		private void Init()
        {
            IBcpgKey key = publicPk.Key;

			if (publicPk.Version <= 3)
            {
                RsaPublicBcpgKey rK = (RsaPublicBcpgKey) key;

				this.keyId = rK.Modulus.LongValue;

				try
                {
                    IDigest digest = DigestUtilities.GetDigest("MD5");

					byte[] bytes = rK.Modulus.ToByteArrayUnsigned();
					digest.BlockUpdate(bytes, 0, bytes.Length);

					bytes = rK.PublicExponent.ToByteArrayUnsigned();
					digest.BlockUpdate(bytes, 0, bytes.Length);

					this.fingerprint = DigestUtilities.DoFinal(digest);
                }
				//catch (NoSuchAlgorithmException)
				catch (Exception)
                {
                    throw new IOException("can't find MD5");
                }

				this.keyStrength = rK.Modulus.BitLength;
            }
            else
            {
                byte[] kBytes = publicPk.GetEncodedContents();

				try
                {
                    IDigest digest = DigestUtilities.GetDigest("SHA1");

					digest.Update(0x99);
                    digest.Update((byte)(kBytes.Length >> 8));
                    digest.Update((byte)kBytes.Length);
                    digest.BlockUpdate(kBytes, 0, kBytes.Length);
                    this.fingerprint = DigestUtilities.DoFinal(digest);
                }
				//catch (NoSuchAlgorithmException)
                catch (Exception)
                {
                    throw new IOException("can't find SHA1");
                }

				this.keyId = (long)(((ulong)fingerprint[fingerprint.Length - 8] << 56)
                    | ((ulong)fingerprint[fingerprint.Length - 7] << 48)
                    | ((ulong)fingerprint[fingerprint.Length - 6] << 40)
                    | ((ulong)fingerprint[fingerprint.Length - 5] << 32)
                    | ((ulong)fingerprint[fingerprint.Length - 4] << 24)
                    | ((ulong)fingerprint[fingerprint.Length - 3] << 16)
                    | ((ulong)fingerprint[fingerprint.Length - 2] << 8)
                    | (ulong)fingerprint[fingerprint.Length - 1]);

				if (key is RsaPublicBcpgKey)
                {
                    this.keyStrength = ((RsaPublicBcpgKey)key).Modulus.BitLength;
                }
                else if (key is DsaPublicBcpgKey)
                {
                    this.keyStrength = ((DsaPublicBcpgKey)key).P.BitLength;
                }
                else if (key is ElGamalPublicBcpgKey)
                {
                    this.keyStrength = ((ElGamalPublicBcpgKey)key).P.BitLength;
                }
            }
        }

		/// <summary>
		/// Create a PgpPublicKey from the passed in lightweight one.
		/// </summary>
		/// <remarks>
		/// Note: the time passed in affects the value of the key's keyId, so you probably only want
		/// to do this once for a lightweight key, or make sure you keep track of the time you used.
		/// </remarks>
		/// <param name="algorithm">Asymmetric algorithm type representing the public key.</param>
		/// <param name="pubKey">Actual public key to associate.</param>
		/// <param name="time">Date of creation.</param>
		/// <exception cref="ArgumentException">If <c>pubKey</c> is not public.</exception>
		/// <exception cref="PgpException">On key creation problem.</exception>
        public PgpPublicKey(
            PublicKeyAlgorithmTag	algorithm,
            AsymmetricKeyParameter	pubKey,
            DateTime				time)
        {
			if (pubKey.IsPrivate)
				throw new ArgumentException("Expected a public key", "pubKey");

			IBcpgKey bcpgKey;
            if (pubKey is RsaKeyParameters)
            {
                RsaKeyParameters rK = (RsaKeyParameters) pubKey;

				bcpgKey = new RsaPublicBcpgKey(rK.Modulus, rK.Exponent);
            }
            else if (pubKey is DsaPublicKeyParameters)
            {
                DsaPublicKeyParameters dK = (DsaPublicKeyParameters) pubKey;
                DsaParameters dP = dK.Parameters;

				bcpgKey = new DsaPublicBcpgKey(dP.P, dP.Q, dP.G, dK.Y);
            }
            else if (pubKey is ElGamalPublicKeyParameters)
            {
                ElGamalPublicKeyParameters eK = (ElGamalPublicKeyParameters) pubKey;
                ElGamalParameters eS = eK.Parameters;

				bcpgKey = new ElGamalPublicBcpgKey(eS.P, eS.G, eK.Y);
            }
            else
            {
                throw new PgpException("unknown key class");
            }

			this.publicPk = new PublicKeyPacket(algorithm, time, bcpgKey);
            this.ids = new ArrayList();
            this.idSigs = new ArrayList();

			try
            {
                Init();
            }
            catch (IOException e)
            {
                throw new PgpException("exception calculating keyId", e);
            }
        }

		/// <summary>Constructor for a sub-key.</summary>
        internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            TrustPacket		trustPk,
            ArrayList		sigs)
        {
            this.publicPk = publicPk;
            this.trustPk = trustPk;
            this.subSigs = sigs;

			Init();
        }

		internal PgpPublicKey(
            PgpPublicKey	key,
            TrustPacket		trust,
            ArrayList		subSigs)
        {
            this.publicPk = key.publicPk;
            this.trustPk = trust;
            this.subSigs = subSigs;

			this.fingerprint = key.fingerprint;
            this.keyId = key.keyId;
            this.keyStrength = key.keyStrength;
        }

		/// <summary>Copy constructor.</summary>
		/// <param name="pubKey">The public key to copy.</param>
        internal PgpPublicKey(
            PgpPublicKey pubKey)
        {
            this.publicPk = pubKey.publicPk;

			this.keySigs = new ArrayList(pubKey.keySigs);
            this.ids = new ArrayList(pubKey.ids);
            this.idTrusts = new ArrayList(pubKey.idTrusts);
            this.idSigs = new ArrayList(pubKey.idSigs.Count);
            for (int i = 0; i != pubKey.idSigs.Count; i++)
            {
                this.idSigs.Add(new ArrayList((ArrayList)pubKey.idSigs[i]));
            }

			if (pubKey.subSigs != null)
            {
                this.subSigs = new ArrayList(pubKey.subSigs.Count);
                for (int i = 0; i != pubKey.subSigs.Count; i++)
                {
                    this.subSigs.Add(pubKey.subSigs[i]);
                }
            }

			this.fingerprint = pubKey.fingerprint;
            this.keyId = pubKey.keyId;
            this.keyStrength = pubKey.keyStrength;
        }

		internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            TrustPacket		trustPk,
            ArrayList		keySigs,
            ArrayList		ids,
            ArrayList		idTrusts,
            ArrayList		idSigs)
        {
            this.publicPk = publicPk;
            this.trustPk = trustPk;
            this.keySigs = keySigs;
            this.ids = ids;
            this.idTrusts = idTrusts;
            this.idSigs = idSigs;

			Init();
        }

		internal PgpPublicKey(
            PublicKeyPacket	publicPk,
            ArrayList		ids,
            ArrayList		idSigs)
        {
            this.publicPk = publicPk;
            this.ids = ids;
            this.idSigs = idSigs;
            Init();
        }

		/// <summary>The version of this key.</summary>
        public int Version
        {
			get { return publicPk.Version; }
        }

		/// <summary>The creation time of this key.</summary>
		public DateTime CreationTime
        {
			get { return publicPk.GetTime(); }
        }

		/// <summary>The number of valid days from creation time - zero means no expiry.</summary>
        public int ValidDays
        {
			get
			{
				if (publicPk.Version > 3)
				{
					return (int)(GetValidSeconds() / (24 * 60 * 60));
				}

				return publicPk.ValidDays;
			}
        }

		/// <summary>Return the trust data associated with the public key, if present.</summary>
		/// <returns>A byte array with trust data, null otherwise.</returns>
		public byte[] GetTrustData()
		{
			if (trustPk == null)
			{
				return null;
			}

			return trustPk.GetLevelAndTrustAmount();
		}

		/// <summary>The number of valid seconds from creation time - zero means no expiry.</summary>
		public long GetValidSeconds()
        {
			if (publicPk.Version > 3)
			{
				if (IsMasterKey)
				{
					for (int i = 0; i != MasterKeyCertificationTypes.Length; i++)
					{
						long seconds = GetExpirationTimeFromSig(true, MasterKeyCertificationTypes[i]);

						if (seconds >= 0)
						{
							return seconds;
						}
					}
				}
				else
				{
					long seconds = GetExpirationTimeFromSig(false, PgpSignature.SubkeyBinding);

					if (seconds >= 0)
					{
						return seconds;
					}
				}

				return 0;
			}

			return (long) publicPk.ValidDays * 24 * 60 * 60;
        }

		private long GetExpirationTimeFromSig(
			bool	selfSigned,
			int		signatureType)
		{
			foreach (PgpSignature sig in GetSignaturesOfType(signatureType))
			{
				if (!selfSigned || sig.KeyId == KeyId)
				{
					PgpSignatureSubpacketVector hashed = sig.GetHashedSubPackets();

					if (hashed != null)
					{
						return hashed.GetKeyExpirationTime();
					}

					return 0;
				}
			}

			return -1;
		}

		/// <summary>The keyId associated with the public key.</summary>
        public long KeyId
        {
            get { return keyId; }
        }

		/// <summary>The fingerprint of the key</summary>
        public byte[] GetFingerprint()
        {
			return (byte[]) fingerprint.Clone();
        }


		/// <summary>True, if this key is marked as suitable for encryption</summary>
		/// <returns>True, if this key is marked as suitable for using for encryption.</returns>
		public bool IsEncryptionKey
        {
            get
            {
				switch (publicPk.Algorithm)
				{
					case PublicKeyAlgorithmTag.ElGamalEncrypt:
					case PublicKeyAlgorithmTag.ElGamalGeneral:
					case PublicKeyAlgorithmTag.RsaEncrypt:
					case PublicKeyAlgorithmTag.RsaGeneral:
						return true;
					default:
						return false;
				}
            }
        }

		/// <summary>True, if this is a master key.</summary>
        public bool IsMasterKey
        {
            get { return subSigs == null; }
        }

		/// <summary>The algorithm code associated with the public key.</summary>
        public PublicKeyAlgorithmTag Algorithm
        {
			get { return publicPk.Algorithm; }
        }

		/// <summary>The strength of the key in bits.</summary>
        public int BitStrength
        {
            get { return keyStrength; }
        }

		/// <summary>The public key contained in the object.</summary>
		/// <returns>A lightweight public key.</returns>
		/// <exception cref="PgpException">If the key algorithm is not recognised.</exception>
        public AsymmetricKeyParameter GetKey()
        {
            try
            {
                switch (publicPk.Algorithm)
                {
                    case PublicKeyAlgorithmTag.RsaEncrypt:
                    case PublicKeyAlgorithmTag.RsaGeneral:
                    case PublicKeyAlgorithmTag.RsaSign:
                        RsaPublicBcpgKey rsaK = (RsaPublicBcpgKey) publicPk.Key;
                        return new RsaKeyParameters(false, rsaK.Modulus, rsaK.PublicExponent);
                    case PublicKeyAlgorithmTag.Dsa:
                        DsaPublicBcpgKey dsaK = (DsaPublicBcpgKey) publicPk.Key;
                        return new DsaPublicKeyParameters(dsaK.Y, new DsaParameters(dsaK.P, dsaK.Q, dsaK.G));
                    case PublicKeyAlgorithmTag.ElGamalEncrypt:
                    case PublicKeyAlgorithmTag.ElGamalGeneral:
                        ElGamalPublicBcpgKey elK = (ElGamalPublicBcpgKey) publicPk.Key;
                        return new ElGamalPublicKeyParameters(elK.Y, new ElGamalParameters(elK.P, elK.G));
                    default:
                        throw new PgpException("unknown public key algorithm encountered");
                }
            }
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("exception constructing public key", e);
            }
        }

		/// <summary>Allows enumeration of any user IDs associated with the key.</summary>
		/// <returns>An <c>IEnumerable</c> of <c>string</c> objects.</returns>
        public IEnumerable GetUserIds()
        {
            ArrayList temp = new ArrayList();

			foreach (object o in ids)
			{
				if (o is string)
				{
					temp.Add(o);
                }
            }

			return new EnumerableProxy(temp);
        }

		/// <summary>Allows enumeration of any user attribute vectors associated with the key.</summary>
		/// <returns>An <c>IEnumerable</c> of <c>PgpUserAttributeSubpacketVector</c> objects.</returns>
        public IEnumerable GetUserAttributes()
        {
            ArrayList temp = new ArrayList();

			foreach (object o in ids)
			{
				if (o is PgpUserAttributeSubpacketVector)
				{
					temp.Add(o);
				}
			}

			return new EnumerableProxy(temp);
        }

		/// <summary>Allows enumeration of any signatures associated with the passed in id.</summary>
		/// <param name="id">The ID to be matched.</param>
		/// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesForId(
            string id)
        {
			if (id == null)
				throw new ArgumentNullException("id");

			for (int i = 0; i != ids.Count; i++)
            {
                if (id.Equals(ids[i]))
                {
                    return new EnumerableProxy((ArrayList) idSigs[i]);
                }
            }

			return null;
        }

		/// <summary>Allows enumeration of signatures associated with the passed in user attributes.</summary>
		/// <param name="userAttributes">The vector of user attributes to be matched.</param>
		/// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesForUserAttribute(
            PgpUserAttributeSubpacketVector userAttributes)
        {
            for (int i = 0; i != ids.Count; i++)
            {
                if (userAttributes.Equals(ids[i]))
                {
                    return new EnumerableProxy((ArrayList) idSigs[i]);
                }
            }

			return null;
        }

		/// <summary>Allows enumeration of signatures of the passed in type that are on this key.</summary>
		/// <param name="signatureType">The type of the signature to be returned.</param>
		/// <returns>An <c>IEnumerable</c> of <c>PgpSignature</c> objects.</returns>
        public IEnumerable GetSignaturesOfType(
            int signatureType)
        {
            ArrayList temp = new ArrayList();

			foreach (PgpSignature sig in GetSignatures())
            {
                if (sig.SignatureType == signatureType)
                {
                    temp.Add(sig);
                }
            }

			return new EnumerableProxy(temp);
        }

		/// <summary>Allows enumeration of all signatures/certifications associated with this key.</summary>
		/// <returns>An <c>IEnumerable</c> with all signatures/certifications.</returns>
        public IEnumerable GetSignatures()
        {
			ArrayList sigs;
			if (subSigs != null)
			{
				sigs = subSigs;
			}
			else
			{
				sigs = new ArrayList(keySigs);

				foreach (ICollection extraSigs in idSigs)
				{
					sigs.AddRange(extraSigs);
				}
			}

			return new EnumerableProxy(sigs);
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
            BcpgOutputStream bcpgOut = BcpgOutputStream.Wrap(outStr);

			bcpgOut.WritePacket(publicPk);
            if (trustPk != null)
            {
                bcpgOut.WritePacket(trustPk);
            }

			if (subSigs == null)    // not a sub-key
            {
				foreach (PgpSignature keySig in keySigs)
				{
					keySig.Encode(bcpgOut);
				}

				for (int i = 0; i != ids.Count; i++)
                {
                    if (ids[i] is string)
                    {
                        string id = (string) ids[i];

						bcpgOut.WritePacket(new UserIdPacket(id));
                    }
                    else
                    {
                        PgpUserAttributeSubpacketVector v = (PgpUserAttributeSubpacketVector)ids[i];
                        bcpgOut.WritePacket(new UserAttributePacket(v.ToSubpacketArray()));
                    }

					if (idTrusts[i] != null)
                    {
                        bcpgOut.WritePacket((ContainedPacket)idTrusts[i]);
                    }

					foreach (PgpSignature sig in (ArrayList) idSigs[i])
					{
						sig.Encode(bcpgOut);
					}
                }
            }
            else
            {
				foreach (PgpSignature subSig in subSigs)
				{
					subSig.Encode(bcpgOut);
				}
            }
        }

		/// <summary>Check whether this (sub)key has a revocation signature on it.</summary>
		/// <returns>True, if this (sub)key has been revoked.</returns>
        public bool IsRevoked()
        {
            int ns = 0;
            bool revoked = false;
            if (IsMasterKey)	// Master key
            {
                while (!revoked && (ns < keySigs.Count))
                {
                    if (((PgpSignature)keySigs[ns++]).SignatureType == PgpSignature.KeyRevocation)
                    {
                        revoked = true;
                    }
                }
            }
            else	// Sub-key
            {
                while (!revoked && (ns < subSigs.Count))
                {
                    if (((PgpSignature)subSigs[ns++]).SignatureType == PgpSignature.SubkeyRevocation)
                    {
                        revoked = true;
                    }
                }
            }
            return revoked;
        }

		/// <summary>Add a certification for an id to the given public key.</summary>
		/// <param name="key">The key the certification is to be added to.</param>
		/// <param name="id">The ID the certification is associated with.</param>
		/// <param name="certification">The new certification.</param>
		/// <returns>The re-certified key.</returns>
        public static PgpPublicKey AddCertification(
            PgpPublicKey	key,
            string			id,
            PgpSignature	certification)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            ArrayList sigList = null;

			for (int i = 0; i != returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    sigList = (ArrayList) returnKey.idSigs[i];
                }
            }

			if (sigList != null)
            {
                sigList.Add(certification);
            }
            else
            {
                sigList = new ArrayList();

				sigList.Add(certification);
                returnKey.ids.Add(id);
                returnKey.idTrusts.Add(null);
                returnKey.idSigs.Add(sigList);
            }

			return returnKey;
        }

		/// <summary>Add a certification for the given UserAttributeSubpackets to the given public key.</summary>
		/// <param name="key">The key the certification is to be added to.</param>
		/// <param name="userAttributes">The attributes the certification is associated with.</param>
		/// <param name="certification">The new certification.</param>
		/// <returns>The re-certified key.</returns>
		public static PgpPublicKey AddCertification(
			PgpPublicKey					key,
			PgpUserAttributeSubpacketVector	userAttributes,
			PgpSignature					certification)
		{
			PgpPublicKey returnKey = new PgpPublicKey(key);
			IList sigList = null;

			for (int i = 0; i != returnKey.ids.Count; i++)
			{
				if (userAttributes.Equals(returnKey.ids[i]))
				{
					sigList = (IList) returnKey.idSigs[i];
				}
			}

			if (sigList != null)
			{
				sigList.Add(certification);
			}
			else
			{
				sigList = new ArrayList();
				sigList.Add(certification);
				returnKey.ids.Add(userAttributes);
				returnKey.idTrusts.Add(null);
				returnKey.idSigs.Add(sigList);
			}

			return returnKey;
		}

		/// <summary>
		/// Remove any certifications associated with a user attribute subpacket on a key.
		/// </summary>
		/// <param name="key">The key the certifications are to be removed from.</param>
		/// <param name="userAttributes">The attributes to be removed.</param>
		/// <returns>
		/// The re-certified key, or null if the user attribute subpacket was not found on the key.
		/// </returns>
		public static PgpPublicKey RemoveCertification(
			PgpPublicKey					key,
			PgpUserAttributeSubpacketVector	userAttributes)
		{
			PgpPublicKey returnKey = new PgpPublicKey(key);
			bool found = false;
        
			for (int i = 0; i < returnKey.ids.Count; i++)
			{
				if (userAttributes.Equals(returnKey.ids[i]))
				{
					found = true;
					returnKey.ids.RemoveAt(i);
					returnKey.idTrusts.RemoveAt(i);
					returnKey.idSigs.RemoveAt(i);
				}
			}

			if (!found)
			{
				return null;
			}

			return returnKey;
		}

		/// <summary>Remove any certifications associated with a given ID on a key.</summary>
		/// <param name="key">The key the certifications are to be removed from.</param>
		/// <param name="id">The ID that is to be removed.</param>
		/// <returns>The re-certified key, or null if the ID was not found on the key.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey	key,
            string			id)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            bool found = false;

			for (int i = 0; i < returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    found = true;
                    returnKey.ids.RemoveAt(i);
                    returnKey.idTrusts.RemoveAt(i);
                    returnKey.idSigs.RemoveAt(i);
                }
            }

			return found ? returnKey : null;
        }

		/// <summary>Remove any certifications associated with a given ID on a key.</summary>
		/// <param name="key">The key the certifications are to be removed from.</param>
		/// <param name="id">The ID that the certfication is to be removed from.</param>
		/// <param name="certification">The certfication to be removed.</param>
		/// <returns>The re-certified key, or null if the certification was not found.</returns>
        public static PgpPublicKey RemoveCertification(
            PgpPublicKey	key,
            string			id,
            PgpSignature	certification)
        {
            PgpPublicKey returnKey = new PgpPublicKey(key);
            bool found = false;

			for (int i = 0; i < returnKey.ids.Count; i++)
            {
                if (id.Equals(returnKey.ids[i]))
                {
                    ArrayList certs = (ArrayList) returnKey.idSigs[i];
                    found = certs.Contains(certification);

					if (found)
					{
						certs.Remove(certification);
					}
                }
            }

			return found ? returnKey : null;
        }

		/// <summary>Add a revocation or some other key certification to a key.</summary>
		/// <param name="key">The key the revocation is to be added to.</param>
		/// <param name="certification">The key signature to be added.</param>
		/// <returns>The new changed public key object.</returns>
        public static PgpPublicKey AddCertification(
            PgpPublicKey	key,
            PgpSignature	certification)
        {
            if (key.IsMasterKey)
            {
                if (certification.SignatureType == PgpSignature.SubkeyRevocation)
                {
                    throw new ArgumentException("signature type incorrect for master key revocation.");
                }
            }
            else
            {
                if (certification.SignatureType == PgpSignature.KeyRevocation)
                {
                    throw new ArgumentException("signature type incorrect for sub-key revocation.");
                }
            }

			PgpPublicKey returnKey = new PgpPublicKey(key);

			if (returnKey.subSigs != null)
            {
                returnKey.subSigs.Add(certification);
            }
            else
            {
                returnKey.keySigs.Add(certification);
            }

			return returnKey;
        }
    }
}
