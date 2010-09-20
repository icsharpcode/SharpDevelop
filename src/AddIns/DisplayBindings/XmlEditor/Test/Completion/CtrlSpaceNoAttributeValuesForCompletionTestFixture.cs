// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		XmlSchemaFileAssociations associations;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "<xs:schema elementFormDefault=\"\"></xs:schema>";
			
			// Put cursor inside the double quotes following the elementFormDefault attribute
			textEditor.Caret.Offset = 31;	
			
			completionBinding = new XmlCodeCompletionBinding(associations);
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
