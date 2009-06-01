// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.ObjectModel;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	///   A collection that stores <see cref='XmlCompletionData'/> objects.
	/// </summary>
	[Serializable()]
	public class XmlCompletionItemCollection : Collection<XmlCompletionItem> {
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlCompletionDataCollection'/>.
		/// </summary>
		public XmlCompletionItemCollection()
		{
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlCompletionDataCollection'/> based on another <see cref='XmlCompletionDataCollection'/>.
		/// </summary>
		/// <param name='val'>
		///   A <see cref='XmlCompletionDataCollection'/> from which the contents are copied
		/// </param>
		public XmlCompletionItemCollection(XmlCompletionItemCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlCompletionDataCollection'/> containing any array of <see cref='XmlCompletionData'/> objects.
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='XmlCompletionData'/> objects with which to intialize the collection
		/// </param>
		public XmlCompletionItemCollection(XmlCompletionItem[] val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Copies the elements of an array to the end of the <see cref='XmlCompletionDataCollection'/>.
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='XmlCompletionData'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='XmlCompletionDataCollection.Add'/>
		public void AddRange(XmlCompletionItem[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				if (!Contains(val[i].Text))
					this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Adds the contents of another <see cref='XmlCompletionDataCollection'/> to the end of the collection.
		/// </summary>
		/// <param name='val'>
		///    A <see cref='XmlCompletionDataCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='XmlCompletionDataCollection.Add'/>
		public void AddRange(XmlCompletionItemCollection val)
		{
			for (int i = 0; i < val.Count; i++)
				if (!Contains(val[i].Text))
					this.Add(val[i]);
		}
		
		public bool Contains(string name)
		{
			bool contains = false;
			
			foreach (XmlCompletionItem data in this) {
				if (data.Text != null) {
					if (data.Text.Length > 0) {
						if (data.Text == name) {
							contains = true;
							break;
						}
					}
				}
			}
			
			return contains;
		}
		
		/// <summary>
		/// Returns an array of <see cref="ICompletionData"/> items.
		/// </summary>
		/// <returns></returns>
		public ICompletionItem[] ToArray()
		{
			XmlCompletionItem[] data = new XmlCompletionItem[Count];
			CopyTo(data, 0);
			return data;
		}
	}
}
