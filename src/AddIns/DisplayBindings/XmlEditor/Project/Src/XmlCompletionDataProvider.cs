// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2760 $</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Provides the autocomplete (intellisense) data for an
	/// xml document that specifies a known schema.
	/// </summary>
	public class XmlCompletionDataProvider
	{
		XmlSchemaCompletionDataCollection schemaCompletionDataItems;
		XmlSchemaCompletionData defaultSchemaCompletionData;
		string defaultNamespacePrefix = string.Empty;
		
		public XmlCompletionDataProvider(XmlSchemaCompletionDataCollection schemaCompletionDataItems, XmlSchemaCompletionData defaultSchemaCompletionData, string defaultNamespacePrefix)
		{
			this.schemaCompletionDataItems = schemaCompletionDataItems;
			this.defaultSchemaCompletionData = defaultSchemaCompletionData;
			this.defaultNamespacePrefix = defaultNamespacePrefix;
		}
		
		public ICompletionItemList GenerateCompletionData(string fileContent, char charTyped)
		{
			string text = String.Concat(fileContent, charTyped);
			
			DefaultCompletionItemList list = null;
			
			switch (charTyped) {
				case '=':
					// Namespace intellisense.
					if (XmlParser.IsNamespaceDeclaration(text, text.Length)) {
						list = schemaCompletionDataItems.GetNamespaceCompletionData();
					}
					break;
				case '<':
					// Child element intellisense.
					XmlElementPath parentPath = XmlParser.GetParentElementPath(text);
					if (parentPath.Elements.Count > 0) {
						list = GetChildElementCompletionData(parentPath);
					} else if (defaultSchemaCompletionData != null) {
						list = defaultSchemaCompletionData.GetElementCompletionData(defaultNamespacePrefix);
					}
					break;
					
				case ' ':
					// Attribute intellisense.
					if (!XmlParser.IsInsideAttributeValue(text, text.Length)) {
						XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
						if (path.Elements.Count > 0) {
							list = GetAttributeCompletionData(path);
						}
					}
					break;
				default:
					// Attribute value intellisense.
					if (XmlParser.IsAttributeValueChar(charTyped)) {
						string attributeName = XmlParser.GetAttributeName(text, text.Length);
						if (attributeName.Length > 0) {
							XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
							if (elementPath.Elements.Count > 0) {
								list = GetAttributeValueCompletionData(elementPath, attributeName);
							}
						}
					}
					break;
			}
			
			if (list != null) {
				list.SortItems();
				list.SuggestedItem = list.Items.FirstOrDefault();
			}
			
			return list;
		}
		
		/// <summary>
		/// Finds the schema given the xml element path.
		/// </summary>
		public XmlSchemaCompletionData FindSchema(XmlElementPath path)
		{
			if (path.Elements.Count > 0) {
				string namespaceUri = path.Elements[0].Namespace;
				if (namespaceUri.Length > 0) {
					return XmlSchemaManager.SchemaCompletionDataItems[namespaceUri];
				} else if (defaultSchemaCompletionData != null) {
					
					// Use the default schema namespace if none
					// specified in a xml element path, otherwise
					// we will not find any attribute or element matches
					// later.
					foreach (QualifiedName name in path.Elements) {
						if (name.Namespace.Length == 0) {
							name.Namespace = defaultSchemaCompletionData.NamespaceUri;
						}
					}
					return defaultSchemaCompletionData;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema given a namespace URI.
		/// </summary>
		public XmlSchemaCompletionData FindSchema(string namespaceUri)
		{
			return schemaCompletionDataItems[namespaceUri];
		}
		
		/// <summary>
		/// Gets the schema completion data that was created from the specified
		/// schema filename.
		/// </summary>
		public XmlSchemaCompletionData FindSchemaFromFileName(string fileName)
		{
			return schemaCompletionDataItems.GetSchemaFromFileName(fileName);
		}
		
		public DefaultCompletionItemList GetChildElementCompletionData(XmlElementPath path)
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetChildElementCompletionData(path));
			}
			
			list.SortItems();
			list.SuggestedItem = list.Items.FirstOrDefault();
			return list;
		}
		
		public DefaultCompletionItemList GetAttributeCompletionData(XmlElementPath path)
		{
			var list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetAttributeCompletionData(path));
			}
			
			list.SortItems();
			list.SuggestedItem = list.Items.FirstOrDefault();
			return list;
		}
		
		public DefaultCompletionItemList GetAttributeValueCompletionData(XmlElementPath path, string name)
		{
			var list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetAttributeValueCompletionData(path, name));
			}
			
			list.SortItems();
			list.SuggestedItem = list.Items.FirstOrDefault();
			return list;
		}
	}
}
