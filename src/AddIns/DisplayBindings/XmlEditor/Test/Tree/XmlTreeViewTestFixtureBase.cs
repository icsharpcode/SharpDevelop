// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1924 $</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	public abstract class XmlTreeViewTestFixtureBase
	{
		protected MockXmlTreeView mockXmlTreeView;
		protected XmlTreeEditor editor;
		
		public void InitFixture()
		{
			mockXmlTreeView = new MockXmlTreeView();
			XmlCompletionDataProvider completionDataProvider = new XmlCompletionDataProvider(SchemaDataItems, DefaultSchemaCompletionData, DefaultNamespacePrefix);
			editor = new XmlTreeEditor(mockXmlTreeView, completionDataProvider);
			editor.LoadXml(GetXml());
		}
		
		protected virtual string GetXml()
		{
			return String.Empty;
		}
		
		protected virtual XmlSchemaCompletionDataCollection SchemaDataItems {
			get {
				return new XmlSchemaCompletionDataCollection();
			}
		}
	
		protected virtual XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				return null;
			}
		}
		
		/// <summary>
		/// Gets the default element prefix.
		/// </summary>
		protected virtual string DefaultNamespacePrefix {
			get {
				return String.Empty;
			}
		}
	}
}
