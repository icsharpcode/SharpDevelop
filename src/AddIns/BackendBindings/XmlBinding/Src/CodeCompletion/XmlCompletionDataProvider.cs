// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2760 $</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
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
		
		public ICompletionItemList GetChildElementCompletionData(XmlElementPath path)
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetChildElementCompletionData(path));
			}
			
			list.SortItems();
			
			return list;
		}
		
		public ICompletionItemList GetAttributeCompletionData(XmlElementPath path)
		{
			var list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetAttributeCompletionData(path));
			}
			
			list.SortItems();
			
			return list;
		}
		
		public ICompletionItemList GetAttributeValueCompletionData(XmlElementPath path, string name)
		{
			var list = new XmlCompletionItemList();
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				list.Items.AddRange(schema.GetAttributeValueCompletionData(path, name));
			}
			
			list.SortItems();
			
			return list;
		}
	}
}
