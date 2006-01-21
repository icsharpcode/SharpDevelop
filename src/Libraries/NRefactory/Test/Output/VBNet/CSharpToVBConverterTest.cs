// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.PrettyPrinter
{
	[TestFixture]
	public class CSharpToVBConverterTest
	{
		public void TestProgram(string input, string expectedOutput)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(input));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			parser.CompilationUnit.AcceptVisitor(new CSharpToVBNetConvertVisitor(), null);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			outputVisitor.Visit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput, outputVisitor.Text);
		}
		
		public void TestMember(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("Class tmp1");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("End Class");
			TestProgram("class tmp1 { \n" + input + "\n}", b.ToString());
		}
		
		public void TestStatement(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("Class tmp1");
			b.AppendLine("\tPrivate Sub tmp2()");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("\tEnd Sub");
			b.AppendLine("End Class");
			TestProgram("class tmp1 { void tmp2() {\n" + input + "\n}}", b.ToString());
		}
		
		[Test]
		public void ForWithUnknownConditionAndSingleStatement()
		{
			TestStatement("for (i = 0; unknownCondition; i++) b[i] = s[i];",
			              "i = 0\n" +
			              "While unknownCondition\n" +
			              "\tb(i) = s(i)\n" +
			              "\ti += 1\n" +
			              "End While");
		}
		
		[Test]
		public void ForWithUnknownConditionAndBlock()
		{
			TestStatement("for (i = 0; unknownCondition; i++) { b[i] = s[i]; }",
			              "i = 0\n" +
			              "While unknownCondition\n" +
			              "\tb(i) = s(i)\n" +
			              "\ti += 1\n" +
			              "End While");
		}
		
		[Test]
		public void ForWithSingleStatement()
		{
			TestStatement("for (i = 0; i < end; i++) b[i] = s[i];",
			              "For i = 0 To [end] - 1\n" +
			              "\tb(i) = s(i)\n" +
			              "Next");
		}
		[Test]
		public void ForWithBlock()
		{
			TestStatement("for (i = 0; i < end; i++) { b[i] = s[i]; }",
			              "For i = 0 To [end] - 1\n" +
			              "\tb(i) = s(i)\n" +
			              "Next");
		}
		
		[Test]
		public void AddEventHandler()
		{
			TestStatement("this.button1.Click += new System.EventHandler(this.OnButton1Click);",
			              "AddHandler Me.button1.Click, AddressOf Me.OnButton1Click");
		}
		
		[Test]
		public void RemoveEventHandler()
		{
			TestStatement("this.button1.Click -= new System.EventHandler(this.OnButton1Click);",
			              "RemoveHandler Me.button1.Click, AddressOf Me.OnButton1Click");
		}
		
		[Test]
		public void RaiseEvent()
		{
			TestStatement("if (MyEvent != null) MyEvent(this, EventArgs.Empty);",
			              "RaiseEvent MyEvent(Me, EventArgs.Empty)");
			TestStatement("if (null != MyEvent) { MyEvent(this, EventArgs.Empty); }",
			              "RaiseEvent MyEvent(Me, EventArgs.Empty)");
		}
		
		[Test]
		public void IfStatementSimilarToRaiseEvent()
		{
			TestStatement("if (FullImage != null) DrawImage();",
			              "If FullImage IsNot Nothing Then\n" +
			              "\tDrawImage()\n" +
			              "End If");
			// regression test:
			TestStatement("if (FullImage != null) e.DrawImage();",
			              "If FullImage IsNot Nothing Then\n" +
			              "\te.DrawImage()\n" +
			              "End If");
			// with braces:
			TestStatement("if (FullImage != null) { DrawImage(); }",
			              "If FullImage IsNot Nothing Then\n" +
			              "\tDrawImage()\n" +
			              "End If");
			TestStatement("if (FullImage != null) { e.DrawImage(); }",
			              "If FullImage IsNot Nothing Then\n" +
			              "\te.DrawImage()\n" +
			              "End If");
			// another bug related to the IfStatement code:
			TestStatement("if (Tiles != null) foreach (Tile t in Tiles) this.TileTray.Controls.Remove(t);",
			              "If Tiles IsNot Nothing Then\n" +
			              "\tFor Each t As Tile In Tiles\n" +
			              "\t\tMe.TileTray.Controls.Remove(t)\n" +
			              "\tNext\n" +
			              "End If");
		}
		
		[Test]
		public void AnonymousMethod()
		{
			TestMember("void A() { someEvent += delegate(int argument) { return argument * 2; }; }",
			           "Private Sub A()\n" +
			           "\tAddHandler someEvent, AddressOf ConvertedAnonymousMethod1\n" +
			           "End Sub\n" +
			           "Private Sub ConvertedAnonymousMethod1(ByVal argument As Integer)\n" +
			           "\tReturn argument * 2\n" +
			           "End Sub");
		}
		
		[Test]
		public void AnonymousMethodInVarDeclaration()
		{
			TestMember("void A() { SomeDelegate i = delegate(int argument) { return argument * 2; }; }",
			           "Private Sub A()\n" +
			           "\tDim i As SomeDelegate = AddressOf ConvertedAnonymousMethod1\n" +
			           "End Sub\n" +
			           "Private Sub ConvertedAnonymousMethod1(ByVal argument As Integer)\n" +
			           "\tReturn argument * 2\n" +
			           "End Sub");
		}
		
		[Test]
		public void RegisterEvent()
		{
			TestStatement("someEvent += tmp2;",
			              "AddHandler someEvent, AddressOf tmp2");
			TestStatement("someEvent += this.tmp2;",
			              "AddHandler someEvent, AddressOf tmp2");
			TestStatement("someEvent += new SomeDelegate(tmp2);",
			              "AddHandler someEvent, AddressOf tmp2");
			TestStatement("someEvent += new SomeDelegate(this.tmp2);",
			              "AddHandler someEvent, AddressOf tmp2");
		}
		
		[Test]
		public void StaticMethod()
		{
			TestMember("static void A() {}",
			           "Private Shared Sub A()\nEnd Sub");
		}
		
		[Test]
		public void PInvoke()
		{
			TestMember("[DllImport(\"user32.dll\", CharSet = CharSet.Auto)]\n" +
			           "public static extern int MessageBox(IntPtr hwnd, string t, string caption, UInt32 t2);",
			           "<DllImport(\"user32.dll\", CharSet := CharSet.Auto)> _\n" +
			           "Public Shared Function MessageBox(ByVal hwnd As IntPtr, ByVal t As String, ByVal caption As String, ByVal t2 As UInt32) As Integer\n" +
			           "End Function");
			
			TestMember("[DllImport(\"user32.dll\", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]\n" +
			           "public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, UIntPtr wParam, IntPtr lParam);",
			           "Public Declare Ansi Function SendMessage Lib \"user32.dll\" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As UIntPtr, ByVal lParam As IntPtr) As IntPtr");
			
			TestMember("[DllImport(\"user32.dll\", SetLastError = true, ExactSpelling = true, EntryPoint = \"SendMessageW\")]\n" +
			           "public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, UIntPtr wParam, IntPtr lParam);",
			           "Public Declare Auto Function SendMessage Lib \"user32.dll\" Alias \"SendMessageW\" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As UIntPtr, ByVal lParam As IntPtr) As IntPtr");
		}
	}
}
