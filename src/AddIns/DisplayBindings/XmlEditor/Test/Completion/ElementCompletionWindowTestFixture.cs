// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class ElementCompletionWindowTestFixture
	{
		MockTextEditor textEditor;
		CodeCompletionKeyPressResult keyPressResult;
		XmlSchemaCompletionCollection schemas;
		
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
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(associations);
			keyPressResult = completionBinding.HandleKeyPress(textEditor, '<');			
		}
		
		[Test]
		public void KeyPressResultIsCompletedAfterPressingEqualsSign()
		{
			Assert.AreEqual(CodeCompletionKeyPressResult.Completed, keyPressResult);
		}
		
		[Test]
		public void CompletionWindowWidthIsNotSetToMatchLongestNamespaceTextWidth()
		{
			Assert.AreNotEqual(double.NaN, textEditor.CompletionWindowDisplayed.Width);
		}		
	}
}
