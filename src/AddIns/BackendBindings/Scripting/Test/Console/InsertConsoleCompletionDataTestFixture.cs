// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
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
