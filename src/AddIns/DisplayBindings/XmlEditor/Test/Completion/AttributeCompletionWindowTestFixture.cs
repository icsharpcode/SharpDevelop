// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class AttributeCompletionWindowTestFixture
	{
		MockTextEditor textEditor;
		CodeCompletionKeyPressResult keyPressResult;
		XmlSchemaCompletionCollection schemas;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();
			schemas.Add(new XmlSchemaCompletion(ResourceManager.ReadXsdSchema()));

			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			associations.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".xsd", "http://www.w3.org/2001/XMLSchema", "xs"));
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "<xs:schema></xs:schema>";
			
			// Put cursor after the first 'a' in "<xs:schema>"
			textEditor.Caret.Offset = 10;			
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(associations);
			keyPressResult = completionBinding.HandleKeyPress(textEditor, ' ');			
		}
		
		[Test]
		public void KeyPressResultIsNotCompletedAfterPressingSpaceBar()
		{
			Assert.AreEqual(CodeCompletionKeyPressResult.None, keyPressResult);
		}
		
		[Test]
		public void CompletionListUsedInShowCompletionWindowHasNoItems()
		{
			Assert.IsTrue(textEditor.CompletionItemsDisplayedToArray().Length == 0);
		}
	}
}
