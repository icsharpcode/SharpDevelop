// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextEditorTests
	{
		MockTextEditor editor;

		[SetUp]
		public void Init()
		{
			editor = new MockTextEditor();
		}
			
		[Test]
		public void CanCheckCompletionWindowFromShowCompletionHasWidthPropertyModified()
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			list.Items.Add(new XmlCompletionItem("a"));
			ICompletionListWindow window = editor.ShowCompletionWindow(list);
			window.Width = double.NaN;
			
			Assert.AreEqual(double.NaN, editor.CompletionWindowDisplayed.Width);
		}
		
		[Test]
		public void CanGetCompletionItemsUsedAsShowCompletionMethodParameters()
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			list.Items.Add(new XmlCompletionItem("a"));
			editor.ShowCompletionWindow(list);

			Assert.AreSame(list, editor.CompletionItemsDisplayed);
		}
		
		[Test]
		public void CanConvertCompletionItemsUsedAsShowCompletionMethodParametersToArray()
		{
			XmlCompletionItem item = new XmlCompletionItem("a");
			List<XmlCompletionItem> expectedArray = new List<XmlCompletionItem>();
			expectedArray.Add(item);
			
			XmlCompletionItemList list = new XmlCompletionItemList();
			list.Items.Add(item);
			editor.ShowCompletionWindow(list);

			Assert.AreEqual(expectedArray.ToArray(), editor.CompletionItemsDisplayedToArray());
		}
		
		[Test]
		public void TextEditorDocumentCanGetAndSetText()
		{
			editor.Document.Text = "test";
			Assert.AreEqual("test", editor.Document.Text);
		}
		
		[Test]
		public void TextEditorCaretCanGetAndSetCaretOffset()
		{
			editor.Caret.Offset = 5;
			Assert.AreEqual(5, editor.Caret.Offset);
		}
		
		[Test]
		public void TextEditorMockDocumentPropertyIsSameAsTextEditorDocumentProperty()
		{
			Assert.AreSame(editor.MockDocument, editor.Document);
		}
		
		[Test]
		public void TextEditorCanGetAndSetFileName()
		{
			editor.FileName = new FileName(@"c:\projects\a.xml");
			Assert.AreEqual(@"c:\projects\a.xml", editor.FileName.ToString());
		}
		
		[Test]
		public void TextEditorShowCompletionListReturnsNullWhenConfigured()
		{
			XmlCompletionItemList list = new XmlCompletionItemList();
			list.Items.Add(new XmlCompletionItem("a"));
			
			editor.ShowCompletionWindowReturnsNull = true;
			
			Assert.IsNull(editor.ShowCompletionWindow(list));
		}
		
		[Test]
		public void TextEditorShowCompletionListIsCalledReturnsTrueIfShowCompletionListMethodReturnsNullWindow()
		{
			TextEditorShowCompletionListReturnsNullWhenConfigured();
			Assert.IsTrue(editor.IsShowCompletionWindowMethodCalled);
		}
		
		[Test]
		public void IsShowCompletionWindowMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(editor.IsShowCompletionWindowMethodCalled);
		}
	}
}
