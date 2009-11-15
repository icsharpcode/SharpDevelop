// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
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
			editor = new XmlTreeEditor(mockXmlTreeView, Schemas, DefaultSchemaCompletion);
			editor.LoadXml(GetXml());
		}
		
		protected virtual string GetXml()
		{
			return String.Empty;
		}
		
		protected virtual XmlSchemaCompletionCollection Schemas {
			get { return new XmlSchemaCompletionCollection(); }
		}
	
		protected virtual XmlSchemaCompletion DefaultSchemaCompletion {
			get { return null; }
		}
		
		/// <summary>
		/// Gets the default element prefix.
		/// </summary>
		protected virtual string DefaultNamespacePrefix {
			get { return String.Empty; }
		}
	}
}
