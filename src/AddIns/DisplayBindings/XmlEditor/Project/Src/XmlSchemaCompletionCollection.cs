// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	[Serializable()]
	public class XmlSchemaCompletionCollection : Collection<XmlSchemaCompletion> 
	{	
		public XmlSchemaCompletionCollection()
		{
		}
		
		public XmlSchemaCompletionCollection(XmlSchemaCompletionCollection schemas)
		{
			AddRange(schemas);
		}
		
		public XmlSchemaCompletionCollection(XmlSchemaCompletion[] schemas)
		{
			AddRange(schemas);
		}
		
		public XmlCompletionItemCollection GetNamespaceCompletion(string textUpToCursor)
		{
			if (XmlParser.IsNamespaceDeclaration(textUpToCursor, textUpToCursor.Length)) {
				return GetNamespaceCompletion();
			}
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetNamespaceCompletion()
		{
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			
			foreach (XmlSchemaCompletion schema in this) {
				XmlCompletionItem completionData = new XmlCompletionItem(schema.NamespaceUri, XmlCompletionItemType.NamespaceUri);
				completionItems.Add(completionData);
			}
			
			return completionItems;
		}
		
		/// <summary>
		///   Represents the <see cref='XmlSchemaCompletionData'/> entry with the specified namespace URI.
		/// </summary>
		/// <param name='namespaceUri'>The schema's namespace URI.</param>
		/// <value>The entry with the specified namespace URI.</value>
		public XmlSchemaCompletion this[string namespaceUri] {
			get { return GetItem(namespaceUri); }
		}
		
		public bool Contains(string namespaceUri)
		{
			return GetItem(namespaceUri) != null;
		}
		
		XmlSchemaCompletion GetItem(string namespaceUri)
		{
			foreach(XmlSchemaCompletion item in this) {
				if (item.NamespaceUri == namespaceUri) {
					return item;
				}
			}	
			return null;
		}		
		
		public void AddRange(XmlSchemaCompletion[] schema)
		{
			for (int i = 0; i < schema.Length; i++) {
				Add(schema[i]);
			}
		}
		
		public void AddRange(XmlSchemaCompletionCollection schemas)
		{
			for (int i = 0; i < schemas.Count; i++) {
				this.Add(schemas[i]);
			}
		}
		
		/// <summary>
		/// Gets the schema completion data with the same filename.
		/// </summary>
		/// <returns><see langword="null"/> if no matching schema found.</returns>
		public XmlSchemaCompletion GetSchemaFromFileName(string fileName)
		{
			foreach (XmlSchemaCompletion schema in this) {
				if (FileUtility.IsEqualFileName(schema.FileName, fileName)) {
					return schema;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema given the xml element path.
		/// </summary>
		public XmlSchemaCompletion FindSchema(XmlElementPath path, XmlSchemaCompletion defaultSchema)
		{
			if (path.Elements.HasItems) {
				string namespaceUri = path.GetRootNamespace();
				if (namespaceUri.Length > 0) {
					return this[namespaceUri];
				} else if (defaultSchema != null) {
					path.SetNamespaceForUnqualifiedNames(defaultSchema.NamespaceUri);
					return defaultSchema;
				}
			}
			return null;
		}
		
		public XmlCompletionItemCollection GetChildElementCompletion(XmlElementPath path, XmlSchemaCompletion defaultSchema)
		{
			XmlSchemaCompletion schema = FindSchema(path, defaultSchema);
			if (schema != null) {
				return schema.GetChildElementCompletion(path);
			}			
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetAttributeCompletion(XmlElementPath path, XmlSchemaCompletion defaultSchema)
		{
			XmlSchemaCompletion schema = FindSchema(path, defaultSchema);
			if (schema != null) {
				return schema.GetAttributeCompletion(path);
			}
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetAttributeValueCompletion(XmlElementPath path, string attributeName, XmlSchemaCompletion defaultSchema)
		{
			XmlSchemaCompletion schema = FindSchema(path, defaultSchema);
			if (schema != null) {
				return schema.GetAttributeValueCompletion(path, attributeName);
			}
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetElementCompletion(string textUpToCursor, XmlSchemaCompletion defaultSchema)
		{
			XmlElementPath parentPath = XmlParser.GetParentElementPath(textUpToCursor);
			if (parentPath.Elements.HasItems) {
				return GetChildElementCompletion(parentPath, defaultSchema);
			} else if (defaultSchema != null) {
				return defaultSchema.GetRootElementCompletion();
			}
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetAttributeCompletion(string textUpToCursor, XmlSchemaCompletion defaultSchema)
		{
			if (!XmlParser.IsInsideAttributeValue(textUpToCursor, textUpToCursor.Length)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(textUpToCursor, textUpToCursor.Length);
				if (path.Elements.HasItems) {
					return GetAttributeCompletion(path, defaultSchema);
				}
			}
			return new XmlCompletionItemCollection();
		}
		
		public XmlCompletionItemCollection GetAttributeValueCompletion(char charTyped, string textUpToCursor, XmlSchemaCompletion defaultSchema)
		{
			if (XmlParser.IsAttributeValueChar(charTyped)) {
				string attributeName = XmlParser.GetAttributeName(textUpToCursor, textUpToCursor.Length);
				if (attributeName.Length > 0) {
					XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(textUpToCursor, textUpToCursor.Length);
					if (elementPath.Elements.HasItems) {
						return GetAttributeValueCompletion(elementPath, attributeName, defaultSchema);
					}
				}
			}
			return new XmlCompletionItemCollection();
		}

		public XmlCompletionItemCollection GetAttributeValueCompletion(string text, int offset, XmlSchemaCompletion defaultSchema)
		{
			if (XmlParser.IsInsideAttributeValue(text, offset)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(text, offset);
				string attributeName = XmlParser.GetAttributeNameAtIndex(text, offset);
				return GetAttributeValueCompletion(path, attributeName, defaultSchema);
			}
			return new XmlCompletionItemCollection();
		}
	}
}
