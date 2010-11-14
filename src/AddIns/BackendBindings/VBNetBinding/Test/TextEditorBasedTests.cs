// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
{
	public class TextEditorBasedTests
	{
		protected MockTextEditor textEditor;
		
		[SetUp]
		public void SetupTest()
		{
			this.textEditor = new MockTextEditor();
		}
		
		protected void TestCtrlSpace(string file, bool expected, Action<ICompletionItemList> constraint)
		{
			int insertionPoint = file.IndexOf('|');
			
			if (insertionPoint < 0)
				Assert.Fail("insertionPoint not found in text!");
			
			this.textEditor.Document.Text = file.Replace("|", "");
			this.textEditor.Caret.Offset = insertionPoint;
			this.textEditor.CreateParseInformation();
			
			bool invoked = new VBNetCompletionBinding().CtrlSpace(textEditor);
			
			Assert.AreEqual(expected, invoked);
			
			ICompletionItemList list = this.textEditor.LastCompletionItemList;
			
			constraint(list);
		}
		
		protected void TestKeyPress(string file, char keyPressed, CodeCompletionKeyPressResult keyPressResult, Action<ICompletionItemList> constraint)
		{
			int insertionPoint = file.IndexOf('|');
			
			if (insertionPoint < 0)
				Assert.Fail("insertionPoint not found in text!");
			
			this.textEditor.Document.Text = file.Replace("|", "");
			this.textEditor.Caret.Offset = insertionPoint;
			this.textEditor.CreateParseInformation();
			
			CodeCompletionKeyPressResult result = new VBNetCompletionBinding().HandleKeyPress(textEditor, keyPressed);
			
			Assert.AreEqual(keyPressResult, result);
			
			ICompletionItemList list = this.textEditor.LastCompletionItemList;
			
			constraint(list);
		}
		
		protected void TestTextInsert(string file, char completionChar, ICompletionItemList list, ICompletionItem item, string expectedOutput, int expectedOffset)
		{
			int insertionPoint = file.IndexOf('|');
			
			if (insertionPoint < 0)
				Assert.Fail("insertionPoint not found in text!");
			
			this.textEditor.Document.Text = file.Replace("|", "");
			this.textEditor.Caret.Offset = insertionPoint;
			this.textEditor.CreateParseInformation();
			
			CompletionContext context = new CompletionContext() {
				Editor = this.textEditor,
				CompletionChar = completionChar,
				StartOffset = textEditor.Caret.Offset,
				EndOffset = textEditor.Caret.Offset
			};
			
			list.Complete(context, item);
			
			if (!context.CompletionCharHandled && context.CompletionChar != '\n')
				this.textEditor.Document.Insert(this.textEditor.Caret.Offset, completionChar + "");
			
			string insertedText = this.textEditor.Document.GetText(insertionPoint, this.textEditor.Document.TextLength - file.Length + 1);
			
			Assert.AreEqual(expectedOutput, insertedText);
			Assert.AreEqual(insertionPoint + expectedOffset, textEditor.Caret.Offset);
		}
	}
}
