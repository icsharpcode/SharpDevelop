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
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	[RequiresSTA]
	public class InsertConsoleCompletionDataTestFixture
	{
		ScriptingConsoleCompletionData completionData;
		TextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new TextEditor();
		}
		
		[Test]
		public void TextInsertedAtCursor()
		{
			textEditor.Text = "abc.n";
			textEditor.CaretOffset = 4;
			
			int startOffset = 4;
			int endOffset = 5;
			Selection selection = Selection.Create(textEditor.TextArea, startOffset, endOffset);
			
			completionData = new ScriptingConsoleCompletionData("new");
			completionData.Complete(textEditor.TextArea, selection.SurroundingSegment, null);
			
			string expectedText = 
				"abc.new";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
	}
}
