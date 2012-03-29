// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	[Serializable()]
	public class XmlCompletionItemCollection : Collection<XmlCompletionItem>, ICompletionItemList
	{
		List<char> normalKeys = new List<char>();

		public XmlCompletionItemCollection()
		{
			normalKeys.AddRange(new char[] { ':', '.', '_' });
		}
		
		public XmlCompletionItemCollection(XmlCompletionItemCollection items)
			: this()
		{
			AddRange(items);
		}
		
		public XmlCompletionItemCollection(XmlCompletionItem[] items)
			: this()
		{
			AddRange(items);
		}
		
		public bool HasItems {
			get { return Count > 0; }
		}
		
		public bool ContainsAllAvailableItems {
			get {
				return true;
			}
		}
		
		public void Sort()
		{
			List<XmlCompletionItem> items = base.Items as List<XmlCompletionItem>;
			items.Sort();
		}
		
		public void AddRange(XmlCompletionItem[] items)
		{
			for (int i = 0; i < items.Length; i++) {
				if (!Contains(items[i].Text)) {
					Add(items[i]);
				}
			}
		}
		
		public void AddRange(XmlCompletionItemCollection item)
		{
			for (int i = 0; i < item.Count; i++) {
				if (!Contains(item[i].Text)) {
					Add(item[i]);
				}
			}
		}
		
		public bool Contains(string name)
		{			
			foreach (XmlCompletionItem data in this) {
				if (data.Text != null) {
					if (data.Text.Length > 0) {
						if (data.Text == name) {
							return true;
						}
					}
				}
			}		
			return false;
		}
		
		/// <summary>
		/// Gets a count of the number of occurrences of a particular name
		/// in the completion data.
		/// </summary>
		public int GetOccurrences(string name)
		{
			int count = 0;
			
			foreach (XmlCompletionItem item in this) {
				if (item.Text == name) {
					++count;
				}
			}
			
			return count;
		}
		
		/// <summary>
		/// Checks whether the completion item specified by name has
		/// the correct description.
		/// </summary>
		public bool ContainsDescription(string name, string description)
		{
			foreach (XmlCompletionItem item in this) {
				if (item.Text == name) {
					if (item.Description == description) {
						return true;
					}
				}
			}				
			return false;
		}
		
		public XmlCompletionItem[] ToArray()
		{
			XmlCompletionItem[] data = new XmlCompletionItem[Count];
			CopyTo(data, 0);
			return data;
		}
		
		public CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == '!' || key == '?')
				return CompletionItemListKeyResult.Cancel;
			if (char.IsLetterOrDigit(key))
				return CompletionItemListKeyResult.NormalKey;
			if (normalKeys.Contains(key))
				return CompletionItemListKeyResult.NormalKey;
			return CompletionItemListKeyResult.InsertionKey;
		}
		
		IEnumerable<ICompletionItem> ICompletionItemList.Items {
			get { return this; }
		}
		
		public ICompletionItem SuggestedItem {
			get { 
				if (HasItems && PreselectionLength == 0) {
					return this[0];
				}
				return null;
			}
		}
		
		public int PreselectionLength { get; set; }
		
		public void Complete(CompletionContext context, ICompletionItem item)
		{
			item.Complete(context);
		}
	}
}
