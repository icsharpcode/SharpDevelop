using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities.Zlib;

namespace Org.BouncyCastle.Cms
{
    /**
    * containing class for an CMS Compressed Data object
    */
    public class CmsCompressedData
    {
        internal ContentInfo contentInfo;

		public CmsCompressedData(
            byte[] compressedData)
            : this(CmsUtilities.ReadContentInfo(compressedData))
        {
        }

		public CmsCompressedData(
            Stream compressedDataStream)
            : this(CmsUtilities.ReadContentInfo(compressedDataStream))
        {
        }

		public CmsCompressedData(
            ContentInfo contentInfo)
        {
            this.contentInfo = contentInfo;
        }

		public byte[] GetContent()
        {
            CompressedData comData = CompressedData.GetInstance(contentInfo.Content);
            ContentInfo content = comData.EncapContentInfo;

			Asn1OctetString bytes = (Asn1OctetString) content.Content;
			ZInflaterInputStream zIn = new ZInflaterInputStream(bytes.GetOctetStream());

			try
			{
				return CmsUtilities.StreamToByteArray(zIn);
			}
			catch (IOException e)
			{
				throw new CmsException("exception reading compressed stream.", e);
			}
			finally
			{
				zIn.Close();
			}
        }

		/**
		 * return the ContentInfo 
		 */
		public ContentInfo ContentInfo
		{
			get { return contentInfo; }
		}

		/**
        * return the ASN.1 encoded representation of this object.
        */
        public byte[] GetEncoded()
        {
			return contentInfo.GetEncoded();
        }
    }
}
