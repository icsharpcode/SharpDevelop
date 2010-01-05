using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public enum PkiStatus
	{
		Granted					= 0,
		GrantedWithMods			= 1,
		Rejection				= 2,
		Waiting					= 3,
		RevocationWarning		= 4,
		RevocationNotification	= 5,
	}
}
