using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
    public class SignerInformationStore
    {
		private readonly ArrayList all; //ArrayList[SignerInformation]
		private readonly Hashtable table = new Hashtable(); // Hashtable[SignerID, ArrayList[SignerInformation]]

		public SignerInformationStore(
            ICollection signerInfos)
        {
            foreach (SignerInformation signer in signerInfos)
            {
                SignerID sid = signer.SignerID;
				ArrayList list = (ArrayList) table[sid];

				if (list == null)
				{
					table[sid] = list = new ArrayList(1);
				}

				list.Add(signer);
            }

			this.all = new ArrayList(signerInfos);
        }

        /**
        * Return the first SignerInformation object that matches the
        * passed in selector. Null if there are no matches.
        *
        * @param selector to identify a signer
        * @return a single SignerInformation object. Null if none matches.
        */
        public SignerInformation GetFirstSigner(
            SignerID selector)
        {
			ArrayList list = (ArrayList) table[selector];

			return list == null ? null : (SignerInformation) list[0];
        }

		/// <summary>The number of signers in the collection.</summary>
		public int Count
        {
			get { return all.Count; }
        }

		/// <returns>An ICollection of all signers in the collection</returns>
        public ICollection GetSigners()
        {
			return new ArrayList(all);
        }

		/**
        * Return possible empty collection with signers matching the passed in SignerID
        *
        * @param selector a signer id to select against.
        * @return a collection of SignerInformation objects.
        */
        public ICollection GetSigners(
            SignerID selector)
        {
			ArrayList list = (ArrayList) table[selector];

			return list == null ? new ArrayList() : new ArrayList(list);
        }
    }
}
