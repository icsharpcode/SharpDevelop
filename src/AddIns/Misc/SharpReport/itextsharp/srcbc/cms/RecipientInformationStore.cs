using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class RecipientInformationStore
	{
		private readonly ArrayList all; //ArrayList[RecipientInformation]
		private readonly Hashtable table = new Hashtable(); // Hashtable[RecipientID, ArrayList[RecipientInformation]]

		public RecipientInformationStore(
			ICollection recipientInfos)
		{
			foreach (RecipientInformation recipientInformation in recipientInfos)
			{
				RecipientID rid = recipientInformation.RecipientID;
				ArrayList list = (ArrayList) table[rid];

				if (list == null)
				{
					table[rid] = list = new ArrayList(1);
				}

				list.Add(recipientInformation);
			}

			this.all = new ArrayList(recipientInfos);
		}

		/**
		* Return the first RecipientInformation object that matches the
		* passed in selector. Null if there are no matches.
		*
		* @param selector to identify a recipient
		* @return a single RecipientInformation object. Null if none matches.
		*/
		public RecipientInformation GetFirstRecipient(
			RecipientID selector)
		{
			ArrayList list = (ArrayList) table[selector];

			return list == null ? null : (RecipientInformation) list[0];
		}

		/**
		* Return the number of recipients in the collection.
		*
		* @return number of recipients identified.
		*/
		public int Count
		{
			get { return all.Count; }
		}

		/**
		* Return all recipients in the collection
		*
		* @return a collection of recipients.
		*/
		public ICollection GetRecipients()
		{
			return new ArrayList(all);
		}

		/**
		* Return possible empty collection with recipients matching the passed in RecipientID
		*
		* @param selector a recipient id to select against.
		* @return a collection of RecipientInformation objects.
		*/
		public ICollection GetRecipients(
			RecipientID selector)
		{
			ArrayList list = (ArrayList) table[selector];

			return list == null ? new ArrayList() : new ArrayList(list);
		}
	}
}
