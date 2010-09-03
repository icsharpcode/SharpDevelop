// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using CSharpBinding.FormattingStrategy;
using System;
using NUnit.Framework;

namespace CSharpBinding.Tests
{
	[TestFixture]
	public class CSharpFormattingTests
	{
		[Test]
		public void EnterInsideString()
		{
			const string start = "class X {\n" + 
				"\tstring text = \"a";
			const string insertedMiddle = "\" +\n\t\"";
			const string end = "b\";\n" +
				"}";
			MockTextEditor textEditor = new MockTextEditor();
			textEditor.Document.Text = start + end;
			textEditor.Select(start.Length, 0);
			CSharpFormattingStrategy formattingStrategy = new CSharpFormattingStrategy();
			textEditor.TextEditor.TextArea.PerformTextInput("\n");
			formattingStrategy.FormatLine(textEditor, '\n');
			Assert.AreEqual(start + insertedMiddle + end, textEditor.Document.Text);
			Assert.AreEqual(start.Length + insertedMiddle.Length, textEditor.SelectionStart);
			Assert.AreEqual(0, textEditor.SelectionLength);
		}
	}
}
