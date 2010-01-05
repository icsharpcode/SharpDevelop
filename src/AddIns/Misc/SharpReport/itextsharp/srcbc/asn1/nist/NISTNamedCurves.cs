using System;
using System.Collections;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.Nist
{
	/**
	* Utility class for fetching curves using their NIST names as published in FIPS-PUB 186-2
	*/
	public sealed class NistNamedCurves
	{
		private NistNamedCurves()
		{
		}

		private static readonly Hashtable objIds = new Hashtable();
		private static readonly Hashtable names = new Hashtable();

		private static void DefineCurve(
			string				name,
			DerObjectIdentifier	oid)
		{
			objIds.Add(name, oid);
			names.Add(oid, name);
		}

		static NistNamedCurves()
		{
			DefineCurve("B-571", SecObjectIdentifiers.SecT571r1);
			DefineCurve("B-409", SecObjectIdentifiers.SecT409r1);
			DefineCurve("B-283", SecObjectIdentifiers.SecT283r1);
			DefineCurve("B-233", SecObjectIdentifiers.SecT233r1);
			DefineCurve("B-163", SecObjectIdentifiers.SecT163r2);
			DefineCurve("P-521", SecObjectIdentifiers.SecP521r1);
			DefineCurve("P-256", SecObjectIdentifiers.SecP256r1);
			DefineCurve("P-224", SecObjectIdentifiers.SecP224r1);
			DefineCurve("P-384", SecObjectIdentifiers.SecP384r1);
		}

		public static X9ECParameters GetByName(
			string name)
		{
			DerObjectIdentifier oid = (DerObjectIdentifier) objIds[name.ToUpper(CultureInfo.InvariantCulture)];

			if (oid != null)
			{
				return GetByOid(oid);
			}

			return null;
		}

		/**
		* return the X9ECParameters object for the named curve represented by
		* the passed in object identifier. Null if the curve isn't present.
		*
		* @param oid an object identifier representing a named curve, if present.
		*/
		public static X9ECParameters GetByOid(
			DerObjectIdentifier oid)
		{
			return SecNamedCurves.GetByOid(oid);
		}

		/**
		* return the object identifier signified by the passed in name. Null
		* if there is no object identifier associated with name.
		*
		* @return the object identifier associated with name, if present.
		*/
		public static DerObjectIdentifier GetOid(
			string name)
		{
			return (DerObjectIdentifier) objIds[name.ToUpper(CultureInfo.InvariantCulture)];
		}

		/**
		* return the named curve name represented by the given object identifier.
		*/
		public static string GetName(
			DerObjectIdentifier  oid)
		{
			return (string) names[oid];
		}

		/**
		* returns an enumeration containing the name strings for curves
		* contained in this structure.
		*/
		public static IEnumerable Names
		{
			get { return new EnumerableProxy(objIds.Keys); }
		}
	}
}
