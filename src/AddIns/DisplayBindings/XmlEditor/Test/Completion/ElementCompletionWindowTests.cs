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
	public class ElementCompletionWindowTests
	{
		MockTextEditor textEditor;
		CodeCompletionKeyPressResult keyPressResult;
		XmlSchemaCompletionCollection schemas;
		XmlCodeCompletionBinding completionBinding;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();
			schemas.Add(new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema()));

			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			associations.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".xml", "http://www.w3.org/1999/xhtml"));
			
			textEditor = new MockTextEditor();
			textEditor.Document.Text = String.Empty;
			textEditor.FileName = new FileName(@"c:\projects\test.xml");
			
			textEditor.Caret.Offset = 0;
			
			completionBinding = new XmlCodeCompletionBinding(associations);
		}
		
		[Test]
		public void HandleKeyPress_LessThanKeyPressed_KeyPressResultIsCompletedAfterPressingLessThanSign()
		{
			keyPressResult = completionBinding.HandleKeyPress(textEditor, '<');
			Assert.AreEqual(CodeCompletionKeyPressResult.EatKey, keyPressResult);
		}
		
		[Test]
		public void HandleKeyPress_LessThanKeyPressed_CompletionWindowWidthIsNotSetToMatchLongestNamespaceTextWidth()
		{
			completionBinding.HandleKeyPress(textEditor, '<');
			Assert.AreNotEqual(double.NaN, textEditor.CompletionWindowDisplayed.Width);
		}
		
		[Test]
		public void HandleKeyPress_LessThanKeyPressedInsideRootHtmlElement_BodyElementExistsInChildCompletionItems()
		{
			textEditor.Document.Text = 
				"<html><\r\n" +
				"</html>";
			textEditor.Caret.Offset = 6;
			
			completionBinding.HandleKeyPress(textEditor, '<');
			ICompletionItem[] items = textEditor.CompletionItemsDisplayedToArray();
			
			XmlCompletionItem expectedItem = new XmlCompletionItem("body", XmlCompletionItemType.XmlElement);
			
			Assert.Contains(expectedItem, items);
		}
	}
}
