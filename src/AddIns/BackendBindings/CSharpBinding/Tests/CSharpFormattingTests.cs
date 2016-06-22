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
