using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.CryptoPro
{
    /**
    * table of the available named parameters for GOST 3410-2001.
    */
    public sealed class ECGost3410NamedCurves
    {
		private ECGost3410NamedCurves()
		{
		}

        internal static readonly Hashtable objIds = new Hashtable();
        internal static readonly Hashtable parameters = new Hashtable();
        internal static readonly Hashtable names = new Hashtable();

        static ECGost3410NamedCurves()
        {
            BigInteger mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319");
            BigInteger mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323");

            FpCurve curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"), // a
                new BigInteger("166")); // b

            ECDomainParameters ecParams = new ECDomainParameters(
                curve,
				curve.CreatePoint(
					BigInteger.One, // x
					new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612"), // y
					false),
                mod_q);

			parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProA] = ecParams;

            mod_p = new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639319");
            mod_q = new BigInteger("115792089237316195423570985008687907853073762908499243225378155805079068850323");

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("115792089237316195423570985008687907853269984665640564039457584007913129639316"),
                new BigInteger("166"));

            ecParams = new ECDomainParameters(
                curve,
				curve.CreatePoint(
					BigInteger.One, // x
					new BigInteger("64033881142927202683649881450433473985931760268884941288852745803908878638612"), // y
					false),
                mod_q);

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA] = ecParams;

            mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823193"); //p
            mod_q = new BigInteger("57896044618658097711785492504343953927102133160255826820068844496087732066703"); //q

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564823190"), // a
                new BigInteger("28091019353058090096996979000309560759124368558014865957655842872397301267595")); // b

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
					BigInteger.One, // x
					new BigInteger("28792665814854611296992347458380284135028636778229113005756334730996303888124"), // y
					false),
                mod_q); // q

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProB] = ecParams;

            mod_p = new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502619");
            mod_q = new BigInteger("70390085352083305199547718019018437840920882647164081035322601458352298396601");

            curve = new FpCurve(
                mod_p, // p
                new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502616"),
                new BigInteger("32858"));

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
					BigInteger.Zero, // x
					new BigInteger("29818893917731240733471273240314769927240550812383695689146495261604565990247"), // y
					false),
                mod_q);

            parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB] = ecParams;

            mod_p = new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502619"); //p
            mod_q = new BigInteger("70390085352083305199547718019018437840920882647164081035322601458352298396601"); //q
            curve = new FpCurve(
                mod_p, // p
                new BigInteger("70390085352083305199547718019018437841079516630045180471284346843705633502616"), // a
                new BigInteger("32858")); // b

            ecParams = new ECDomainParameters(
                curve,
                curve.CreatePoint(
					BigInteger.Zero, // x
					new BigInteger("29818893917731240733471273240314769927240550812383695689146495261604565990247"), // y
					false),
                mod_q); // q

			parameters[CryptoProObjectIdentifiers.GostR3410x2001CryptoProC] = ecParams;

            objIds["GostR3410-2001-CryptoPro-A"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProA;
            objIds["GostR3410-2001-CryptoPro-B"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProB;
            objIds["GostR3410-2001-CryptoPro-C"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProC;
            objIds["GostR3410-2001-CryptoPro-XchA"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA;
            objIds["GostR3410-2001-CryptoPro-XchB"] = CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB;

            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProA] = "GostR3410-2001-CryptoPro-A";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProB] = "GostR3410-2001-CryptoPro-B";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProC] = "GostR3410-2001-CryptoPro-C";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchA] = "GostR3410-2001-CryptoPro-XchA";
            names[CryptoProObjectIdentifiers.GostR3410x2001CryptoProXchB] = "GostR3410-2001-CryptoPro-XchB";
        }

        /**
        * return the ECDomainParameters object for the given OID, null if it
        * isn't present.
        *
        * @param oid an object identifier representing a named parameters, if present.
        */
        public static ECDomainParameters GetByOid(
            DerObjectIdentifier oid)
        {
            return (ECDomainParameters) parameters[oid];
        }

		/**
		 * returns an enumeration containing the name strings for curves
		 * contained in this structure.
		 */
		public static IEnumerable Names
		{
			get { return new EnumerableProxy(objIds.Keys); }
		}

        public static ECDomainParameters GetByName(
            string name)
        {
            DerObjectIdentifier oid = (DerObjectIdentifier) objIds[name];

            if (oid != null)
            {
                return (ECDomainParameters) parameters[oid];
            }

            return null;
        }

        /**
        * return the named curve name represented by the given object identifier.
        */
        public static string GetName(
            DerObjectIdentifier oid)
        {
            return (string) names[oid];
        }

		public static DerObjectIdentifier GetOid(
			string name)
        {
            return (DerObjectIdentifier) objIds[name];
        }
    }
}
