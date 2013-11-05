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
		
		void CharacterTypedIntoTextEditor(char ch)
		{
			textEditor.Document.Text += ch.ToString();
			textEditor.Caret.Offset++;
		}
		
		[Test]
		public void HandleKeyPress_AnyKey_ReturnsNone()
		{
			keyPressResult = completionBinding.HandleKeyPress(textEditor, 'a');
			
			Assert.AreEqual(CodeCompletionKeyPressResult.None, keyPressResult);
		}
		
		[Test]
		public void HandleKeyPressed_LessThanKeyPressed_ReturnsTrue()
		{
			CharacterTypedIntoTextEditor('<');
			bool result = completionBinding.HandleKeyPressed(textEditor, '<');
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HandleKeyPressed_SpaceCharacterPressed_ReturnsFalse()
		{
			CharacterTypedIntoTextEditor(' ');
			bool result = completionBinding.HandleKeyPressed(textEditor, ' ');
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HandleKeyPressed_LessThanKeyPressed_CompletionWindowWidthIsNotSetToMatchLongestNamespaceTextWidth()
		{
			CharacterTypedIntoTextEditor('<');
			completionBinding.HandleKeyPressed(textEditor, '<');
			
			Assert.AreNotEqual(double.NaN, textEditor.CompletionWindowDisplayed.Width);
		}
		
		[Test]
		public void HandleKeyPressed_LessThanKeyPressedInsideRootHtmlElement_BodyElementExistsInChildCompletionItems()
		{
			textEditor.Document.Text = 
				"<html><\r\n" +
				"</html>";
			textEditor.Caret.Offset = 7;
			
			completionBinding.HandleKeyPressed(textEditor, '<');
			ICompletionItem[] items = textEditor.CompletionItemsDisplayedToArray();
			
			XmlCompletionItem expectedItem = new XmlCompletionItem("body", XmlCompletionItemType.XmlElement);
			
			Assert.Contains(expectedItem, items);
		}
	}
}
