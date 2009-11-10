// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class CtrlSpaceNoAttributeValuesForCompletionTestFixture
	{
		bool result;
		MockTextEditor textEditor;
		XmlCodeCompletionBinding completionBinding;
		XmlEditorOptions options;
		XmlSchemaCompletionData xsdSchema;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "<xs:schema elementFormDefault=\"\"></xs:schema>";
			
			// Put cursor inside the double quotes following the elementFormDefault attribute
			textEditor.Caret.Offset = 31;	
			
			completionBinding = new XmlCodeCompletionBinding(options);
			result = completionBinding.CtrlSpace(textEditor);
		}
		
		[Test]
		public void ShowCompletionWindowIsNotCalledWhenThereAreNoCompletionItems()
		{
			Assert.IsFalse(textEditor.IsShowCompletionWindowMethodCalled);
		}
		
		[Test]
		public void CtrlSpaceMethodReturnsFalse()
		{
			Assert.IsFalse(result);
		}
	}
}
