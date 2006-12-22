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
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;

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
			outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
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
		public void MoveImportsStatement()
		{
			TestProgram("namespace test { using SomeNamespace; }",
			            "Imports SomeNamespace\r\n" +
			            "Namespace test\r\n" +
			            "End Namespace\r\n");
		}
		
		[Test]
		public void ClassImplementsInterface()
		{
			TestProgram("class test : IComparable { }",
			            "Class test\r\n" +
			            "\tImplements IComparable\r\n" +
			            "End Class\r\n");
		}
		
		[Test]
		public void ClassImplementsInterface2()
		{
			TestProgram("class test : System.IComparable { }",
			            "Class test\r\n" +
			            "\tImplements System.IComparable\r\n" +
			            "End Class\r\n");
		}
		
		[Test]
		public void ClassInheritsClass()
		{
			TestProgram("class test : InvalidDataException { }",
			            "Class test\r\n" +
			            "\tInherits InvalidDataException\r\n" +
			            "End Class\r\n");
		}
		
		[Test]
		public void ClassInheritsClass2()
		{
			TestProgram("class test : System.IO.InvalidDataException { }",
			            "Class test\r\n" +
			            "\tInherits System.IO.InvalidDataException\r\n" +
			            "End Class\r\n");
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
		
		/*
		[Test, Ignore("NRefactory cannot guess the anonymous method's return type")]
		public void AnonymousMethodInVarDeclaration()
		{
			TestMember("void A() { Converter<int, int> i = delegate(int argument) { return argument * 2; }; }",
			           "Private Sub A()\n" +
			           "\tDim i As Converter(Of Integer, Integer) = AddressOf ConvertedAnonymousMethod1\n" +
			           "End Sub\n" +
			           "Private Function ConvertedAnonymousMethod1(ByVal argument As Integer) As Integer\n" +
			           "\tReturn argument * 2\n" +
			           "End Function");
		}
		 */
		
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
		
		[Test]
		public void PInvokeSub()
		{
			TestMember("[DllImport(\"kernel32\", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]\n" +
			           "private static extern void Sleep(long dwMilliseconds);",
			           "Private Declare Ansi Sub Sleep Lib \"kernel32\" (ByVal dwMilliseconds As Long)");
		}
		
		[Test]
		public void Constructor()
		{
			TestMember("public tmp1() : base(1) { }",
			           "Public Sub New()\n\tMyBase.New(1)\nEnd Sub");
			TestMember("public tmp1() : this(1) { }",
			           "Public Sub New()\n\tMe.New(1)\nEnd Sub");
		}
		
		[Test]
		public void StaticConstructor()
		{
			TestMember("static tmp1() { }",
			           "Shared Sub New()\nEnd Sub");
		}
		
		[Test]
		public void Destructor()
		{
			TestMember("~tmp1() { Dead(); }",
			           "Protected Overrides Sub Finalize()\n" +
			           "\tTry\n" +
			           "\t\tDead()\n" +
			           "\tFinally\n" +
			           "\t\tMyBase.Finalize()\n" +
			           "\tEnd Try\n" +
			           "End Sub");
		}
		
		[Test]
		public void Indexer()
		{
			TestMember("public CategoryInfo this[int index] { get { return List[index] as CategoryInfo; } }",
			           "Public Default ReadOnly Property Item(ByVal index As Integer) As CategoryInfo\n" +
			           "\tGet\n" +
			           "\t\tReturn TryCast(List(index), CategoryInfo)\n" +
			           "\tEnd Get\n" +
			           "End Property");
		}
		
		[Test]
		public void RenameConflictingNames()
		{
			TestMember("int count;" +
			           "public int Count { get { return count; } }" +
			           "void Test1(int count) { count = 3; }" +
			           "void Test2() { int count; count = 3; }" +
			           "void Test3() { foreach (int count in someList) { count = 3; } }",
			           
			           "Private m_count As Integer\n" +
			           "Public ReadOnly Property Count() As Integer\n" +
			           "\tGet\n" +
			           "\t\tReturn m_count\n" +
			           "\tEnd Get\n" +
			           "End Property\n" +
			           "Private Sub Test1(ByVal count As Integer)\n" +
			           "\tcount = 3\n" +
			           "End Sub\n" +
			           "Private Sub Test2()\n" +
			           "\tDim count As Integer\n" +
			           "\tcount = 3\n" +
			           "End Sub\n" +
			           "Private Sub Test3()\n" +
			           "\tFor Each count As Integer In someList\n" +
			           "\t\tcount = 3\n" +
			           "\tNext\n" +
			           "End Sub");
		}
		
		[Test]
		public void NullCoalescing()
		{
			TestStatement("c = a ?? b;",
			              "c = IIf(a Is Nothing, b, a)");
		}
		
		[Test]
		public void ConvertedLoop()
		{
			TestStatement("while (cond) example();",
			              "While cond\n" +
			              "\texample()\n" +
			              "End While");
		}
		
		[Test]
		public void UIntVariableDeclaration()
		{
			TestStatement("uint s = 0;", "Dim s As UInteger = 0");
		}
		
		[Test]
		public void BreakInWhileLoop()
		{
			TestStatement("while (test != null) { break; }",
			              "While test IsNot Nothing\n" +
			              "\tExit While\n" +
			              "End While");
		}
		
		[Test]
		public void BreakInDoLoop()
		{
			TestStatement("do { break; } while (test != null);",
			              "Do\n" +
			              "\tExit Do\n" +
			              "Loop While test IsNot Nothing");
		}
		
		[Test]
		public void StructFieldVisibility()
		{
			TestMember("public struct A { int field; }",
			           "Public Structure A\n" +
			           "\tPrivate field As Integer\n" +
			           "End Structure");
		}
		
		[Test]
		public void InnerClassVisibility()
		{
			TestMember("class Inner\n{\n}",
			           "Private Class Inner\n" +
			           "End Class");
		}
		
		[Test]
		public void InnerDelegateVisibility()
		{
			TestMember("delegate void Test();",
			           "Private Delegate Sub Test()");
		}
		
		[Test]
		public void InterfaceVisibility()
		{
			TestMember("public interface ITest {\n" +
			           "  void Test();\n" +
			           "  string Name { get; set; }\n" +
			           "}",
			           "Public Interface ITest\n" +
			           "\tSub Test()\n" +
			           "\tProperty Name() As String\n" +
			           "End Interface");
		}
		
		[Test]
		public void ImportAliasPrimitiveType()
		{
			TestProgram("using T = System.Boolean;", "Imports T = System.Boolean\r\n");
		}
		
		[Test]
		public void DefaultExpression()
		{
			TestStatement("T oldValue = default(T);", "Dim oldValue As T = Nothing");
		}
		
		[Test]
		public void StaticClass()
		{
			TestProgram("public static class Test {}", @"Public NotInheritable Class Test
	Private Sub New()
	End Sub
End Class
");
		}
		
		[Test]
		public void GlobalTypeReference()
		{
			TestStatement("global::System.String a;", "Dim a As Global.System.String");
		}
		
		[Test]
		public void TestMethodCallOnCastExpression()
		{
			TestStatement("((IDisposable)o).Dispose();", "DirectCast(o, IDisposable).Dispose()");
		}
		
		[Test]
		public void PrimitiveCast()
		{
			TestStatement("a = (int)number;", "a = CInt(number)");
		}
		
		[Test]
		public void ArrayCreationUpperBound()
		{
			TestStatement("string[] i = new string[2];",
			              "Dim i As String() = New String(1) {}");
			TestStatement("string[] i = new string[2] { \"0\", \"1\" };",
			              "Dim i As String() = New String(1) {\"0\", \"1\"}");
			TestStatement("string[,] i = new string[6, 6];",
			              "Dim i As String(,) = New String(5, 5) {}");
		}
	}
}
