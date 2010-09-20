// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			SimpleSelection selection = new SimpleSelection(startOffset, endOffset);
			
			completionData = new ScriptingConsoleCompletionData("new");
			completionData.Complete(textEditor.TextArea, selection, null);
			
			string expectedText = 
				"abc.new";
			
			Assert.AreEqual(expectedText, textEditor.Text);
		}
	}
}
