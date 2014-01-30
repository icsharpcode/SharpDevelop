//
// TextPasteIndentEngineTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using System.Text;

namespace ICSharpCode.NRefactory.IndentationTests
{
	[TestFixture]
	public class TextPasteIndentEngineTests
	{
		public static CacheIndentEngine CreateEngine(string text, CSharpFormattingOptions formatOptions = null, TextEditorOptions options = null)
		{
			if (formatOptions == null) {
				formatOptions = FormattingOptionsFactory.CreateMono();
				formatOptions.AlignToFirstIndexerArgument = formatOptions.AlignToFirstMethodCallArgument = true;
			}
			
			var sb = new StringBuilder();
			int offset = 0;
			for (int i = 0; i < text.Length; i++) {
				var ch = text [i];
				if (ch == '$') {
					offset = i;
					continue;
				}
				sb.Append(ch);
			}
			
			var document = new ReadOnlyDocument(sb.ToString());
			options = options ?? new TextEditorOptions { EolMarker = "\n" };
			
			var result = new CacheIndentEngine(new CSharpIndentEngine(document, options, formatOptions));
			result.Update(offset);
			return result;
		}

		static TextEditorOptions CreateInvariantOptions()
		{
			var options = new TextEditorOptions();
			options.EolMarker = "\n";
			return options;
		}

		[Test]
		public void TestSimplePaste()
		{
			var indent = CreateEngine(@"
class Foo
{
	void Bar ()
	{
		System.Console.WriteLine ($);
	}
}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "Foo", null);
			Assert.AreEqual("Foo", text);
		}

		[Test]
		public void TestMultiLinePaste()
		{
			var indent = CreateEngine(@"
namespace FooBar
{
	class Foo
	{
		void Bar ()
		{
			System.Console.WriteLine ();
		}
		$
	}
}
");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\n" }, FormattingOptionsFactory.CreateMono());
			
			var text = handler.FormatPlainText(indent.Offset, "void Bar ()\n{\nSystem.Console.WriteLine ();\n}", null);
			Assert.AreEqual("void Bar ()\n\t\t{\n\t\t\tSystem.Console.WriteLine ();\n\t\t}", text);
		}

		[Test]
		public void TestMultiplePastes()
		{
			var indent = CreateEngine(@"
class Foo
{
	void Bar ()
	{
		System.Console.WriteLine ();
	}
	$
}


");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\n" }, FormattingOptionsFactory.CreateMono());
			
			for (int i = 0; i < 2; i++) {
				var text = handler.FormatPlainText(indent.Offset, "void Bar ()\n{\nSystem.Console.WriteLine ();\n}", null);
				Assert.AreEqual("void Bar ()\n\t{\n\t\tSystem.Console.WriteLine ();\n\t}", text);
			}
		}
		

		[Test]
		public void TestPasteNewLine()
		{
			var indent = CreateEngine(@"
class Foo
{
	$void Bar ()
	{
	}
}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\n" }, FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "int i;\n", null);
			Assert.AreEqual("int i;\n\t", text);
		}

		[Test]
		public void TestPasteNewLineCase2()
		{
			var indent = CreateEngine(@"
class Foo
{
$	void Bar ()
	{
	}
}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\n" }, FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "int i;\n", null);
			Assert.AreEqual("\tint i;\n", text);
		}

		[Test]
		public void PasteVerbatimString()
		{
			var indent = CreateEngine(@"
class Foo
{
void Bar ()
{
	
}
}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\n" }, FormattingOptionsFactory.CreateMono());
			var str = "string str = @\"\n1\n\t2 \n\t\t3\n\";";
			var text = handler.FormatPlainText(indent.Offset, str, null);
			Assert.AreEqual(str, text);
		}

		[Test]
		public void TestWindowsLineEnding()
		{
			var indent = CreateEngine("\r\nclass Foo\r\n{\r\n\tvoid Bar ()\r\n\t{\r\n\t\t$\r\n\t}\r\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "Foo();\r\nBar();\r\nTest();", null);
			Assert.AreEqual("Foo();\n\t\tBar();\n\t\tTest();", text);
		}

		[Test]
		public void TestPasteBlankLines()
		{
			var indent = CreateEngine("class Foo\n{\n\tvoid Bar ()\n\t{\n\t\tSystem.Console.WriteLine ($);\n\t}\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "\n\n\n", null);
			Assert.AreEqual("\n\n\n\t\t\t", text);
		}

		[Test]
		public void TestPasteBlankLinesAndIndent()
		{
			var indent = CreateEngine("class Foo\n{\n\tvoid Bar ()\n\t{\n\t\tSystem.Console.WriteLine ($);\n\t}\n}");
			var options = FormattingOptionsFactory.CreateMono();
			options.EmptyLineFormatting = EmptyLineFormatting.Indent;
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), options);
			var text = handler.FormatPlainText(indent.Offset, "\n\n\n", null);
			Assert.AreEqual("\n\t\t\t\n\t\t\t\n\t\t\t", text);
		}

		[Test]
		public void TestWindowsLineEndingCase2()
		{
			var textEditorOptions = CreateInvariantOptions ();
			textEditorOptions.EolMarker = "\r\n";
			var indent = CreateEngine("\r\nclass Foo\r\n{\r\n\tvoid Bar ()\r\n\t{\r\n\t\t$\r\n\t}\r\n}", FormattingOptionsFactory.CreateMono(), textEditorOptions);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, textEditorOptions, FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "if (true)\r\nBar();\r\nTest();", null);
			Assert.AreEqual("if (true)\r\n\t\t\tBar();\r\n\t\tTest();", text);
		}

		[Test]
		public void PasteVerbatimStringBug1()
		{
			var textEditorOptions = CreateInvariantOptions ();
			textEditorOptions.EolMarker = "\r\n";
			var indent = CreateEngine("\r\nclass Foo\r\n{\r\n\tvoid Bar ()\r\n\t{\r\n\t\t$\r\n\t}\r\n}", FormattingOptionsFactory.CreateMono(), textEditorOptions);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, textEditorOptions, FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "Console.WriteLine (@\"Hello World!\");\n", null);
			Assert.AreEqual("Console.WriteLine (@\"Hello World!\");\r\n\t\t", text);
		}

		[Test]
		public void PasteVerbatimStringBug2()
		{
			var indent = CreateEngine("\nclass Foo\n{\n\tvoid Bar ()\n\t{\n\t\t$\n\t}\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "if (true)\nConsole.WriteLine (@\"Hello\n World!\");\n", null);
			Assert.AreEqual("if (true)\n\t\t\tConsole.WriteLine (@\"Hello\n World!\");\n\t\t", text);
		}

		[Test]
		public void PasteVerbatimStringBug3()
		{
			var indent = CreateEngine("\nclass Foo\n{\n\tvoid Bar ()\n\t{\n$\n\t}\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());

			var text = handler.FormatPlainText(indent.Offset, "\t\tSystem.Console.WriteLine(@\"<evlevlle>\");\n", null);
			Assert.AreEqual("\t\tSystem.Console.WriteLine(@\"<evlevlle>\");\n\t\t", text);
		}

		[Test]
		public void PasteVerbatimStringBug4()
		{
			var indent = CreateEngine("\nclass Foo\n{\n\tvoid Bar ()\n\t{\n$\n\t}\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());

			var text = handler.FormatPlainText(indent.Offset, "var str1 = \n@\"hello\";", null);
			Assert.AreEqual("\t\tvar str1 = \n\t\t\t@\"hello\";", text);
		}

		[Test]
		public void TestPasteComments()
		{
			var indent = CreateEngine(@"
class Foo
{
	$
}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "// Foo\n\t// Foo 2\n\t// Foo 3", null);
			Assert.AreEqual("// Foo\n\t// Foo 2\n\t// Foo 3", text);
		}

		[Test]
		public void PastemultilineAtFirstColumnCorrection()
		{
			var indent = CreateEngine("class Foo\n{\n$\n}");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "void Bar ()\n{\n\tSystem.Console.WriteLine ();\n}", null);
			Assert.AreEqual("\tvoid Bar ()\n\t{\n\t\tSystem.Console.WriteLine ();\n\t}", text);
		}

		[Test]
		public void TestPasteToWindowsEol()
		{
			var indent = CreateEngine("$");
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, new TextEditorOptions { EolMarker = "\r\n" }, FormattingOptionsFactory.CreateMono());
			var text = handler.FormatPlainText(indent.Offset, "namespace Foo\n{\n\tpublic static class FooExtensions\n\t{\n\t\tpublic static int ObjectExtension (this object value)\n\t\t{\n\t\t\treturn 0;\n\t\t}\n\n\t\tpublic static int IntExtension (this int value)\n\t\t{\n\t\t\treturn 0;\n\t\t}\n\t}\n\n\tpublic class Client\n\t{\n\t\tpublic void Method ()\n\t\t{\n\t\t\t0.ToString ();\n\t\t}\n\t}\n}", null);
			Assert.AreEqual("namespace Foo\r\n{\r\n\tpublic static class FooExtensions\r\n\t{\r\n\t\tpublic static int ObjectExtension (this object value)\r\n\t\t{\r\n\t\t\treturn 0;\r\n\t\t}\r\n\r\n\t\tpublic static int IntExtension (this int value)\r\n\t\t{\r\n\t\t\treturn 0;\r\n\t\t}\r\n\t}\r\n\r\n\tpublic class Client\r\n\t{\r\n\t\tpublic void Method ()\r\n\t\t{\r\n\t\t\t0.ToString ();\r\n\t\t}\r\n\t}\r\n}", text);
		}

		[Test]
		public void PastePreProcessorDirectivesNoIndent()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			opt.IndentPreprocessorDirectives = false;

			var indent = CreateEngine(@"
class Foo
{
$
}", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "#if DEBUG\n\tvoid Foo()\n\t{\n\t}\n#endif", null);
			Assert.AreEqual("#if DEBUG\n\tvoid Foo()\n\t{\n\t}\n#endif", text);
		}

		[Test]
		public void PasteInUnterminatedString ()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			opt.IndentPreprocessorDirectives = false;

			var indent = CreateEngine(@"
var foo = ""hello$
", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "Hi \" + username;", null);
			Assert.AreEqual("Hi \" + username;", text);
		}

		[Test]
		public void PasteInTerminatedString ()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			opt.IndentPreprocessorDirectives = false;

			var indent = CreateEngine(@"
var foo = ""hello$"";
", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "Hi \" + username;", null);
			Assert.AreEqual("Hi \\\" + username;", text);
		}

		[Test]
		public void PasteInUnterminatedVerbatimString ()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			opt.IndentPreprocessorDirectives = false;

			var indent = CreateEngine(@"
var foo = @""hello$
", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "Hi \" + username;", null);
			Assert.AreEqual("Hi \" + username;", text);
		}

		[Test]
		public void PasteInTerminatedVerbatimString ()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			opt.IndentPreprocessorDirectives = false;

			var indent = CreateEngine(@"
var foo = @""hello$"";
", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "Hi \" + username;", null);
			Assert.AreEqual("Hi \"\" + username;", text);
		}


		/// <summary>
		/// Bug 16415 - Formatter - Copy paste comments 
		/// </summary>
		[Test]
		public void TestBug16415 ()
		{
			var opt = FormattingOptionsFactory.CreateMono();
			var indent = CreateEngine("class Foo\n{\n\tpublic static void Main (string[] args)\n\t{\n\t\tConsole.WriteLine ();$\n\t}\n}\n", opt);
			ITextPasteHandler handler = new TextPasteIndentEngine(indent, CreateInvariantOptions (), opt);
			var text = handler.FormatPlainText(indent.Offset, "// Line 1\n// Line 2\n// Line 3", null);
			Assert.AreEqual("// Line 1\n\t\t// Line 2\n\t\t// Line 3", text);
		}
	}
}

