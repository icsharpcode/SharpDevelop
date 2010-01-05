using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Ocsp
{
	/**
	 * Carrier for a ResponderID.
	 */
	public class RespID
	{
		internal readonly ResponderID id;

		public RespID(
			ResponderID id)
		{
			this.id = id;
		}

		public RespID(
			X509Name name)
		{
		    try
		    {
		        this.id = new ResponderID(name);
		    }
		    catch (Exception e)
		    {
		        throw new ArgumentException("can't decode name.", e);
		    }
		}

		public RespID(
			AsymmetricKeyParameter publicKey)
		{
			try
			{
				IDigest digest = DigestUtilities.GetDigest("SHA1");

				SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);

				byte[] encoded = info.PublicKeyData.GetBytes();
				digest.BlockUpdate(encoded, 0, encoded.Length);

				byte[] hash = DigestUtilities.DoFinal(digest);

				Asn1OctetString keyHash = new DerOctetString(hash);

				this.id = new ResponderID(keyHash);
			}
			catch (Exception e)
			{
				throw new OcspException("problem creating ID: " + e, e);
			}
		}

		public ResponderID ToAsn1Object()
		{
			return id;
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			RespID other = obj as RespID;

			if (other == null)
				return false;

			return id.Equals(other.id);
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
