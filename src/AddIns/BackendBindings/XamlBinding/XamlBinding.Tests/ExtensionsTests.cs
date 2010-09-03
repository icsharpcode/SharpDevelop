// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		public void StringReplaceTest1()
		{
			string text = "Hello World!";
			int index = 0;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest2()
		{
			string text = "My Hello World!";
			int index = 3;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "My Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest3()
		{
			string text = "Hello World!";
			int index = 6;
			int length = 5;
			string newText = "Byte";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello Byte!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest4()
		{
			string text = "Hello World!";
			int index = 11;
			int length = 1;
			string newText = "?";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello World?";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest1()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test />";
			editor.Caret.Offset = 6;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest2()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 6;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest3()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 14;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest4()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 11;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("value", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest5()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 12;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest6()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 13;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member.", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest7()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 14;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member.v", text);
		}
		
		[Test]
		public void MoveBeforeTest1()
		{
			string xml = "<Test value=\"A\"><Item value=\"B\" /><Item value=\"C\" /></Test>";
			XElement parent = XElement.Parse(xml);
			XElement item = parent.Elements().ElementAt(0);
			XElement itemToMove = parent.Elements().ElementAt(1);
			itemToMove.MoveBefore(item);
			Assert.AreEqual(itemToMove, parent.Elements().First());
		}
	}
}
