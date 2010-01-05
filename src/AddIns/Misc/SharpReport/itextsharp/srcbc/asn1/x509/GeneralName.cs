using System;
using System.Text;

using Org.BouncyCastle.Utilities.Net;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The GeneralName object.
     * <pre>
     * GeneralName ::= CHOICE {
     *      otherName                       [0]     OtherName,
     *      rfc822Name                      [1]     IA5String,
     *      dNSName                         [2]     IA5String,
     *      x400Address                     [3]     ORAddress,
     *      directoryName                   [4]     Name,
     *      ediPartyName                    [5]     EDIPartyName,
     *      uniformResourceIdentifier       [6]     IA5String,
     *      iPAddress                       [7]     OCTET STRING,
     *      registeredID                    [8]     OBJECT IDENTIFIER}
     *
     * OtherName ::= Sequence {
     *      type-id    OBJECT IDENTIFIER,
     *      value      [0] EXPLICIT ANY DEFINED BY type-id }
     *
     * EDIPartyName ::= Sequence {
     *      nameAssigner            [0]     DirectoryString OPTIONAL,
     *      partyName               [1]     DirectoryString }
     * </pre>
     */
    public class GeneralName
        : Asn1Encodable
    {
        public const int OtherName					= 0;
        public const int Rfc822Name					= 1;
        public const int DnsName					= 2;
        public const int X400Address				= 3;
        public const int DirectoryName				= 4;
        public const int EdiPartyName				= 5;
        public const int UniformResourceIdentifier	= 6;
        public const int IPAddress					= 7;
        public const int RegisteredID				= 8;

		internal readonly Asn1Encodable	obj;
        internal readonly int			tag;

		public GeneralName(
            X509Name directoryName)
        {
            this.obj = directoryName;
            this.tag = 4;
        }

		/**
         * When the subjectAltName extension contains an Internet mail address,
         * the address MUST be included as an rfc822Name. The format of an
         * rfc822Name is an "addr-spec" as defined in RFC 822 [RFC 822].
         *
         * When the subjectAltName extension contains a domain name service
         * label, the domain name MUST be stored in the dNSName (an IA5String).
         * The name MUST be in the "preferred name syntax," as specified by RFC
         * 1034 [RFC 1034].
         *
         * When the subjectAltName extension contains a URI, the name MUST be
         * stored in the uniformResourceIdentifier (an IA5String). The name MUST
         * be a non-relative URL, and MUST follow the URL syntax and encoding
         * rules specified in [RFC 1738].  The name must include both a scheme
         * (e.g., "http" or "ftp") and a scheme-specific-part.  The scheme-
         * specific-part must include a fully qualified domain name or IP
         * address as the host.
         *
         * When the subjectAltName extension contains a iPAddress, the address
         * MUST be stored in the octet string in "network byte order," as
         * specified in RFC 791 [RFC 791]. The least significant bit (LSB) of
         * each octet is the LSB of the corresponding byte in the network
         * address. For IP Version 4, as specified in RFC 791, the octet string
         * MUST contain exactly four octets.  For IP Version 6, as specified in
         * RFC 1883, the octet string MUST contain exactly sixteen octets [RFC
         * 1883].
         */
        public GeneralName(
            Asn1Object	name,
			int			tag)
        {
            this.obj = name;
            this.tag = tag;
        }

		public GeneralName(
            int				tag,
            Asn1Encodable	name)
        {
            this.obj = name;
            this.tag = tag;
        }

		/**
		 * Create a GeneralName for the given tag from the passed in String.
		 * <p>
		 * This constructor can handle:
		 * <ul>
		 * <li>rfc822Name</li>
		 * <li>iPAddress</li>
		 * <li>directoryName</li>
		 * <li>dNSName</li>
		 * <li>uniformResourceIdentifier</li>
		 * <li>registeredID</li>
		 * </ul>
		 * For x400Address, otherName and ediPartyName there is no common string
		 * format defined.
		 * </p><p>
		 * Note: A directory name can be encoded in different ways into a byte
		 * representation. Be aware of this if the byte representation is used for
		 * comparing results.
		 * </p>
		 *
		 * @param tag tag number
		 * @param name string representation of name
		 * @throws ArgumentException if the string encoding is not correct or
		 *             not supported.
		 */
		public GeneralName(
            int		tag,
            string	name)
        {
			this.tag = tag;

			if (tag == Rfc822Name || tag == DnsName || tag == UniformResourceIdentifier)
			{
				this.obj = new DerIA5String(name);
			}
			else if (tag == RegisteredID)
			{
				this.obj = new DerObjectIdentifier(name);
			}
			else if (tag == DirectoryName)
			{
				this.obj = new X509Name(name);
			}
			else if (tag == IPAddress)
			{
				if (!Org.BouncyCastle.Utilities.Net.IPAddress.IsValid(name))
					throw new ArgumentException("IP Address is invalid", "name");

				this.obj = new DerOctetString(Encoding.UTF8.GetBytes(name));
			}
			else
			{
				throw new ArgumentException("can't process string for tag: " + tag, "tag");
			}
		}

		public static GeneralName GetInstance(
            object obj)
        {
            if (obj == null || obj is GeneralName)
            {
                return (GeneralName) obj;
            }

            if (obj is Asn1TaggedObject)
            {
                Asn1TaggedObject	tagObj = (Asn1TaggedObject) obj;
                int					tag = tagObj.TagNo;

				switch (tag)
				{
					case OtherName:
						return new GeneralName(tag, Asn1Sequence.GetInstance(tagObj, false));
					case Rfc822Name:
						return new GeneralName(tag, DerIA5String.GetInstance(tagObj, false));
					case DnsName:
						return new GeneralName(tag, DerIA5String.GetInstance(tagObj, false));
					case X400Address:
						throw new ArgumentException("unknown tag: " + tag);
					case DirectoryName:
						return new GeneralName(tag, Asn1Sequence.GetInstance(tagObj, true));
					case EdiPartyName:
						return new GeneralName(tag, Asn1Sequence.GetInstance(tagObj, false));
					case UniformResourceIdentifier:
						return new GeneralName(tag, DerIA5String.GetInstance(tagObj, false));
					case IPAddress:
						return new GeneralName(tag, Asn1OctetString.GetInstance(tagObj, false));
					case RegisteredID:
						return new GeneralName(tag, DerObjectIdentifier.GetInstance(tagObj, false));
				}

            }

			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public static GeneralName GetInstance(
            Asn1TaggedObject	tagObj,
            bool				explicitly)
        {
            return GetInstance(Asn1TaggedObject.GetInstance(tagObj, explicitly));
        }

		public int TagNo
		{
			get { return tag; }
		}

		public Asn1Encodable Name
		{
			get { return obj; }
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(tag);
			buf.Append(": ");

			switch (tag)
			{
				case Rfc822Name:
				case DnsName:
				case UniformResourceIdentifier:
					buf.Append(DerIA5String.GetInstance(obj).GetString());
					break;
				case DirectoryName:
					buf.Append(X509Name.GetInstance(obj).ToString());
					break;
				default:
					buf.Append(obj.ToString());
					break;
			}

			return buf.ToString();
		}

		public override Asn1Object ToAsn1Object()
        {
			// Explicitly tagged if DirectoryName
			return new DerTaggedObject(tag == 4, tag, obj);
        }
    }
}
