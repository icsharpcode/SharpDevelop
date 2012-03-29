// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
