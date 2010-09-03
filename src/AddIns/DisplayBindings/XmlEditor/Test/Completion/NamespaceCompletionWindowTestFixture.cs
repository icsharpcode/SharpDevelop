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
	public class NamespaceCompletionWindowTestFixture : NamespaceCompletionWindowTestFixtureBase
	{		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(associations);
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
			expectedItems.Add(new XmlCompletionItem("a", XmlCompletionItemType.NamespaceUri));
			expectedItems.Add(new XmlCompletionItem("b", XmlCompletionItemType.NamespaceUri));
			expectedItems.Add(new XmlCompletionItem("c", XmlCompletionItemType.NamespaceUri));
			
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
