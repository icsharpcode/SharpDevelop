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
	public class NamespaceCompletionWindowTestFixture : NamespaceCompletionWindowTestFixtureBase
	{		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(options);
			keyPressResult = completionBinding.HandleKeyPress(textEditor, '=');			
		}
		
		[Test]
		public void KeyPressResultIsCompletedAfterPressingEqualsSign()
		{
			Assert.AreEqual(CodeCompletionKeyPressResult.Completed, keyPressResult);
		}
		
		[Test]
		public void CompletionWindowWidthSetToMatchLongestNamespaceTextWidth()
		{
			Assert.AreEqual(double.NaN, textEditor.CompletionWindowDisplayed.Width);
		}
		
		[Test]
		public void ExpectedCompletionDataItems()
		{
			XmlCompletionItemCollection expectedItems = new XmlCompletionItemCollection();
			expectedItems.Add(new XmlCompletionItem("a", XmlCompletionDataType.NamespaceUri));
			expectedItems.Add(new XmlCompletionItem("b", XmlCompletionDataType.NamespaceUri));
			expectedItems.Add(new XmlCompletionItem("c", XmlCompletionDataType.NamespaceUri));
			
			Assert.AreEqual(expectedItems.ToArray(), textEditor.CompletionItemsDisplayedToArray());
		}
		
		[Test]
		public void TextEditorDocumentGetTextCalledWithOffsetAndLength()
		{
			TextSection expectedTextSection = new TextSection(0, 8);
			Assert.AreEqual(expectedTextSection, textEditor.MockDocument.GetTextSectionUsedWithGetTextMethod());
		}
	}
}
