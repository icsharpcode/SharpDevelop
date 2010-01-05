using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Crypto.Tls
{
	/**
	* A representation for a certificate chain as used by an tls server.
	*/
	public class Certificate
	{
		/**
		* The certificates.
		*/
		internal X509CertificateStructure[] certs;

		/**
		* Parse the ServerCertificate message.
		*
		* @param is The stream where to parse from.
		* @return A Certificate object with the certs, the server has sended.
		* @throws IOException If something goes wrong during parsing.
		*/
		internal static Certificate Parse(
			Stream inStr)
		{
			X509CertificateStructure[] certs;
			int left = TlsUtilities.ReadUint24(inStr);
			ArrayList tmp = new ArrayList();
			while (left > 0)
			{
				int size = TlsUtilities.ReadUint24(inStr);
				left -= 3 + size;
				byte[] buf = new byte[size];
				TlsUtilities.ReadFully(buf, inStr);
				MemoryStream bis = new MemoryStream(buf, false);
				Asn1InputStream ais = new Asn1InputStream(bis);
				Asn1Object o = ais.ReadObject();
				tmp.Add(X509CertificateStructure.GetInstance(o));
//				if (bis.available() > 0)
				if (bis.Position < bis.Length)
				{
					throw new ArgumentException("Sorry, there is garbage data left after the certificate");
				}
			}
//			certs = new X509CertificateStructure[tmp.size()];
//			for (int i = 0; i < tmp.size(); i++)
//			{
//				certs[i] = (X509CertificateStructure)tmp.elementAt(i);
//			}
			certs = (X509CertificateStructure[]) tmp.ToArray(typeof(X509CertificateStructure));
			return new Certificate(certs);
		}

		/**
		* Private constructure from an cert array.
		*
		* @param certs The certs the chain should contain.
		*/
		private Certificate(
			X509CertificateStructure[] certs)
		{
			this.certs = certs;
		}

		/// <returns>An array which contains the certs, this chain contains.</returns>
		public X509CertificateStructure[] GetCerts()
		{
//			X509CertificateStructure[] result = new X509CertificateStructure[certs.Length];
//			Array.Copy(certs, 0, result, 0, certs.Length);
//			return result;
			return (X509CertificateStructure[]) certs.Clone();
		}
	}
}
