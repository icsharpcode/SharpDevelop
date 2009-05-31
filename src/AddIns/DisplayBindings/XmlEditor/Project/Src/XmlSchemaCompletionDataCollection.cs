// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	///   A collection that stores <see cref='XmlSchemaCompletionData'/> objects.
	/// </summary>
	[Serializable()]
	public class XmlSchemaCompletionDataCollection : Collection<XmlSchemaCompletionData> {
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlSchemaCompletionDataCollection'/>.
		/// </summary>
		public XmlSchemaCompletionDataCollection()
		{
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlSchemaCompletionDataCollection'/> based on another <see cref='XmlSchemaCompletionDataCollection'/>.
		/// </summary>
		/// <param name='val'>
		///   A <see cref='XmlSchemaCompletionDataCollection'/> from which the contents are copied
		/// </param>
		public XmlSchemaCompletionDataCollection(XmlSchemaCompletionDataCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='XmlSchemaCompletionDataCollection'/> containing any array of <see cref='XmlSchemaCompletionData'/> objects.
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='XmlSchemaCompletionData'/> objects with which to intialize the collection
		/// </param>
		public XmlSchemaCompletionDataCollection(XmlSchemaCompletionData[] val)
		{
			this.AddRange(val);
		}
		
		public DefaultCompletionItemList GetNamespaceCompletionData()
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			
			foreach (XmlSchemaCompletionData schema in this) {
				XmlCompletionItem completionData = new XmlCompletionItem(schema.NamespaceUri, XmlCompletionDataType.NamespaceUri);
				list.Items.Add(completionData);
			}
			
			list.SortItems();
			list.SuggestedItem = list.Items.FirstOrDefault();
			return list;
		}
		
		/// <summary>
		///   Represents the <see cref='XmlSchemaCompletionData'/> entry with the specified namespace URI.
		/// </summary>
		/// <param name='namespaceUri'>The schema's namespace URI.</param>
		/// <value>The entry with the specified namespace URI.</value>
		public XmlSchemaCompletionData this[string namespaceUri] {
			get {
				return GetItem(namespaceUri);
			}
		}
		
		/// <summary>
		///   Copies the elements of an array to the end of the <see cref='XmlSchemaCompletionDataCollection'/>.
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='XmlSchemaCompletionData'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='XmlSchemaCompletionDataCollection.Add'/>
		public void AddRange(XmlSchemaCompletionData[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Adds the contents of another <see cref='XmlSchemaCompletionDataCollection'/> to the end of the collection.
		/// </summary>
		/// <param name='val'>
		///    A <see cref='XmlSchemaCompletionDataCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='XmlSchemaCompletionDataCollection.Add'/>
		public void AddRange(XmlSchemaCompletionDataCollection val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		/// Gets the schema completion data with the same filename.
		/// </summary>
		/// <returns><see langword="null"/> if no matching schema found.</returns>
		public XmlSchemaCompletionData GetSchemaFromFileName(string fileName)
		{
			foreach (XmlSchemaCompletionData schema in this) {
				if (FileUtility.IsEqualFileName(schema.FileName, fileName)) {
					return schema;
				}
			}
			return null;
		}
		
		XmlSchemaCompletionData GetItem(string namespaceUri)
		{
			XmlSchemaCompletionData matchedItem = null;
			
			foreach(XmlSchemaCompletionData item in this)
			{
				if (item.NamespaceUri == namespaceUri) {
					matchedItem = item;
					break;
				}
			}
			
			return matchedItem;
		}
	}
}
