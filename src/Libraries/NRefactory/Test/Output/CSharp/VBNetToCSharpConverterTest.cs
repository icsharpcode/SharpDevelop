// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class VBNetToCSharpConverterTest
	{
		public void TestProgram(string input, string expectedOutput)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(input));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			parser.CompilationUnit.AcceptVisitor(new VBNetConstructsConvertVisitor(), null);
			parser.CompilationUnit.AcceptVisitor(new ToCSharpConvertVisitor(), null);
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			outputVisitor.Options.IndentationChar = ' ';
			outputVisitor.Options.IndentSize = 2;
			outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput.Replace("\r", ""), outputVisitor.Text.Replace("\r", ""));
		}
		
		public void TestMember(string input, string expectedOutput)
		{
			TestMember(input, expectedOutput, null);
		}
		
		public void TestMember(string input, string expectedOutput, string expectedAutomaticImport)
		{
			StringBuilder b = new StringBuilder();
			if (expectedAutomaticImport != null) {
				b.Append("using ");
				b.Append(expectedAutomaticImport);
				b.AppendLine(";");
			}
			b.AppendLine("class tmp1");
			b.AppendLine("{");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("  ");
					b.AppendLine(line);
				}
			}
			b.AppendLine("}");
			TestProgram("Option Strict On \n Class tmp1 \n" + input + "\nEnd Class", b.ToString());
		}
		
		public void TestStatement(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("class tmp1");
			b.AppendLine("{");
			b.AppendLine("  public void tmp2()");
			b.AppendLine("  {");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("    ");
					b.AppendLine(line);
				}
			}
			b.AppendLine("  }");
			b.AppendLine("}");
			TestProgram("Option Strict On \n Option Infer On \n Class tmp1 \n Sub tmp2() \n" + input + "\n End Sub \n End Class", b.ToString());
		}
		
		[Test]
		public void ReferenceEquality()
		{
			TestStatement("b = a Is Nothing",
			              "b = a == null;");
			TestStatement("b = a IsNot Nothing",
			              "b = a != null;");
			TestStatement("b = Nothing Is a",
			              "b = null == a;");
			TestStatement("b = Nothing IsNot a",
			              "b = null != a;");
			TestStatement("c = a Is b",
			              "c = object.ReferenceEquals(a, b);");
			TestStatement("c = a IsNot b",
			              "c = !object.ReferenceEquals(a, b);");
		}
		
		[Test]
		public void AddHandler()
		{
			TestStatement("AddHandler someEvent, AddressOf tmp2",
			              "someEvent += tmp2;");
			TestStatement("AddHandler someEvent, AddressOf Me.tmp2",
			              "someEvent += this.tmp2;");
		}
		
		[Test]
		public void RemoveHandler()
		{
			TestStatement("RemoveHandler someEvent, AddressOf tmp2",
			              "someEvent -= tmp2;");
			TestStatement("RemoveHandler someEvent, AddressOf Me.tmp2",
			              "someEvent -= this.tmp2;");
		}
		
		[Test]
		public void RaiseEvent()
		{
			TestStatement("RaiseEvent someEvent(Me, EventArgs.Empty)",
			              "if (someEvent != null) {\n  someEvent(this, EventArgs.Empty);\n}");
		}
		
		[Test]
		public void EraseStatement()
		{
			TestStatement("Erase a, b",
			              "a = null;\nb = null;");
		}
		
		[Test]
		public void VBEvent()
		{
			TestMember("Friend Event CancelNow(ByVal a As String)",
			           "internal event CancelNowEventHandler CancelNow;\n" +
			           "internal delegate void CancelNowEventHandler(string a);");
		}
		
		[Test]
		public void StaticMethod()
		{
			TestMember("Shared Sub A()\nEnd Sub",
			           "public static void A()\n{\n}");
		}
		
		[Test]
		public void Property()
		{
			TestMember("ReadOnly Property A()\nGet\nReturn Nothing\nEnd Get\nEnd Property",
			           "public object A {\n  get { return null; }\n}");
		}
		
		[Test]
		public void ValueInPropertySetter()
		{
			TestMember("WriteOnly Property A()\nSet\nDim x As Object = Value\nEnd Set\nEnd Property",
			           "public object A {\n  set {\n    object x = value;\n  }\n}");
		}
		
		[Test]
		public void ValueInPropertySetter2()
		{
			TestMember("WriteOnly Property A()\nSet(ByVal otherName)\nDim x As Object = otherName\nEnd Set\nEnd Property",
			           "public object A {\n  set {\n    object x = value;\n  }\n}");
		}
		
		[Test]
		public void AbstractProperty1()
		{
			TestMember("Public MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary { get; set; }");
		}
		
		[Test]
		public void AbstractProperty2()
		{
			TestMember("Public ReadOnly MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary { get; }");
		}
		
		[Test]
		public void AbstractProperty3()
		{
			TestMember("Public WriteOnly MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary { set; }");
		}
		
		[Test]
		public void FieldDeclaredWithDim()
		{
			TestMember("Dim f as String",
			           "string f;");
		}
		
		[Test]
		public void MultipleFields()
		{
			TestMember("Private example, test As Single",
			           "private float example;\n" +
			           "private float test;");
		}
		
		[Test]
		public void MultipleVariables()
		{
			TestStatement("Dim example, test As Single",
			              "float example = 0;\n" +
			              "float test = 0;");
		}
		
		[Test]
		public void PInvoke()
		{
			TestMember("Declare Function SendMessage Lib \"user32.dll\" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As UIntPtr, ByVal lParam As IntPtr) As IntPtr",
			           "[DllImport(\"user32.dll\", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]\n" +
			           "public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, UIntPtr wParam, IntPtr lParam);",
			           "System.Runtime.InteropServices");
			
			TestMember("Declare Unicode Function SendMessage Lib \"user32.dll\" Alias \"SendMessageW\" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As UIntPtr, ByVal lParam As IntPtr) As IntPtr",
			           "[DllImport(\"user32.dll\", EntryPoint = \"SendMessageW\", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]\n" +
			           "public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, UIntPtr wParam, IntPtr lParam);",
			           "System.Runtime.InteropServices");
			
			TestMember("Declare Auto Function SendMessage Lib \"user32.dll\" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As UIntPtr, ByVal lParam As IntPtr) As IntPtr",
			           "[DllImport(\"user32.dll\", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]\n" +
			           "public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, UIntPtr wParam, IntPtr lParam);",
			           "System.Runtime.InteropServices");
			
			TestMember("<DllImport(\"user32.dll\", CharSet:=CharSet.Auto)> _\n" +
			           "Shared Function MessageBox(ByVal hwnd As IntPtr, ByVal t As String, ByVal caption As String, ByVal t2 As UInt32) As Integer\n" +
			           "End Function",
			           "[DllImport(\"user32.dll\", CharSet = CharSet.Auto)]\n" +
			           "public static extern int MessageBox(IntPtr hwnd, string t, string caption, UInt32 t2);");
		}
		
		[Test]
		public void PInvokeSub()
		{
			TestMember("Private Declare Sub Sleep Lib \"kernel32\" (ByVal dwMilliseconds As Long)",
			           "[DllImport(\"kernel32\", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]\n" +
			           "private static extern void Sleep(long dwMilliseconds);",
			           "System.Runtime.InteropServices");
		}
		
		[Test]
		public void Constructor()
		{
			TestMember("Sub New()\n  MyBase.New(1)\nEnd Sub",
			           "public tmp1() : base(1)\n{\n}");
			TestMember("Public Sub New()\n  Me.New(1)\nEnd Sub",
			           "public tmp1() : this(1)\n{\n}");
		}
		
		[Test]
		public void StaticConstructor()
		{
			TestMember("Shared Sub New()\nEnd Sub",
			           "static tmp1()\n{\n}");
		}
		
		[Test]
		public void Destructor()
		{
			TestMember("Protected Overrides Sub Finalize()\n" +
			           "  Try\n" +
			           "    Dead()\n" +
			           "  Finally\n" +
			           "    MyBase.Finalize()\n" +
			           "  End Try\n" +
			           "End Sub",
			           
			           "~tmp1()\n" +
			           "{\n" +
			           "  Dead();\n" +
			           "}");
		}
		
		[Test]
		public void IIFExpression()
		{
			TestStatement("a = iif(cond, trueEx, falseEx)",
			              "a = (cond ? trueEx : falseEx);");
		}
		
		[Test]
		public void IsNothing()
		{
			TestStatement("a = IsNothing(ex)",
			              "a = (ex == null);");
		}
		
		[Test]
		public void IsNotNothing()
		{
			TestStatement("a = Not IsNothing(ex)",
			              "a = (ex != null);");
		}
		
		[Test]
		public void CompatibilityMethods()
		{
			TestStatement("Beep()",
			              "Interaction.Beep();");
		}
		
		[Test]
		public void EqualsCall()
		{
			TestStatement("Equals(a, b)",
			              "Equals(a, b);");
		}
		
		[Test]
		public void Concatenation()
		{
			TestStatement("x = \"Hello \" & \"World\"",
			              "x = \"Hello \" + \"World\";");
			
			TestStatement("x &= \"Hello\"",
			              "x += \"Hello\";");
		}
		
		[Test]
		public void IntegerDivision()
		{
			TestStatement(@"x = x \ b",
			              "x = x / b;");
			TestStatement(@"x \= b",
			              "x /= b;");
		}
		
		[Test]
		public void VBConstants()
		{
			TestStatement("a = vbYesNo",
			              "a = Constants.vbYesNo;");
		}
		
		[Test]
		public void ForNextLoop()
		{
			TestStatement("For i = 0 To 10\n" +
			              "Next",
			              "for (i = 0; i <= 10; i++) {\n" +
			              "}");
			TestStatement("For l As Long = 0 To 10 Step 2\n" +
			              "Next",
			              "for (long l = 0; l <= 10; l += 2) {\n" +
			              "}");
			TestStatement("For l As Long = 10 To 0 Step -1\n" +
			              "Next",
			              "for (long l = 10; l >= 0; l += -1) {\n" +
			              "}");
			TestStatement("For i As Integer = 0 To 10 Step +2\n" +
			              "Next",
			              "for (int i = 0; i <= 10; i += +2) {\n" +
			              "}");
		}
		
		[Test]
		public void DoLoop()
		{
			TestStatement("Do \n Loop",
			              "do {\n" +
			              "} while (true);");
			TestStatement("Do \n Loop Until i = 10000",
			              "do {\n" +
			              "} while (!(i == 10000));");
		}
		
		[Test]
		public void UsingStatement()
		{
			TestStatement("Using r1 As New StreamReader(file1), r2 As New StreamReader(file2)\n" +
			              "End Using",
			              "using (StreamReader r1 = new StreamReader(file1)) {\n" +
			              "  using (StreamReader r2 = new StreamReader(file2)) {\n" +
			              "  }\n" +
			              "}");
		}
		
		[Test]
		public void SwitchStatement()
		{
			TestStatement(@"Select Case i
                  Case 0 To 5
                        i = 10
                  Case 11
                        i = 0
                  Case Else
                        i = 9
            End Select",
			              "switch (i) {\n" +
			              "  case 0:\n" +
			              "  case 1:\n" +
			              "  case 2:\n" +
			              "  case 3:\n" +
			              "  case 4:\n" +
			              "  case 5:\n" +
			              "    i = 10;\n" +
			              "    break;\n" +
			              "  case 11:\n" +
			              "    i = 0;\n" +
			              "    break;\n" +
			              "  default:\n" +
			              "    i = 9;\n" +
			              "    break;\n" +
			              "}");
		}
		
		[Test]
		public void FunctionWithoutImplicitReturn()
		{
			TestMember("Public Function run(i As Integer) As Integer\n" +
			           " Return 0\n" +
			           "End Function",
			           "public int run(int i)\n" +
			           "{\n" +
			           "  return 0;\n" +
			           "}");
		}
		
		[Test]
		public void FunctionWithImplicitReturn()
		{
			TestMember("Public Function run(i As Integer) As Integer\n" +
			           " run = 0\n" +
			           "End Function",
			           "public int run(int i)\n" +
			           "{\n" +
			           "  return 0;\n" +
			           "}");
		}
		
		[Test]
		public void FunctionWithImplicitReturn2()
		{
			TestMember("Public Function run(i As Integer) As Integer\n" +
			           " While something\n" +
			           "   run += i\n" +
			           " End While\n" +
			           "End Function",
			           "public int run(int i)\n" +
			           "{\n" +
			           "  int " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = 0;\n" +
			           "  while (something) {\n" +
			           "    " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " += i;\n" +
			           "  }\n" +
			           "  return " + VBNetConstructsConvertVisitor.FunctionReturnValueName + ";\n" +
			           "}");
		}
		
		[Test]
		public void FunctionWithImplicitReturn2b()
		{
			const string ReturnValueName = VBNetConstructsConvertVisitor.FunctionReturnValueName;
			TestMember("Public Function run(i As Integer) As Integer\n" +
			           " While something\n" +
			           "   run = run + run(i - 1)\n" +
			           " End While\n" +
			           "End Function",
			           "public int run(int i)\n" +
			           "{\n" +
			           "  int " + ReturnValueName + " = 0;\n" +
			           "  while (something) {\n" +
			           "    " + ReturnValueName + " = " + ReturnValueName + " + run(i - 1);\n" +
			           "  }\n" +
			           "  return " + ReturnValueName + ";\n" +
			           "}");
		}
		
		[Test]
		public void FunctionWithImplicitReturn3()
		{
			TestMember("Public Function run(i As Integer) As CustomType\n" +
			           " While something\n" +
			           "   run = New CustomType()\n" +
			           " End While\n" +
			           "End Function",
			           "public CustomType run(int i)\n" +
			           "{\n" +
			           "  CustomType " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = default(CustomType);\n" +
			           "  while (something) {\n" +
			           "    " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = new CustomType();\n" +
			           "  }\n" +
			           "  return " + VBNetConstructsConvertVisitor.FunctionReturnValueName + ";\n" +
			           "}");
		}
		
		[Test]
		public void Exponentiation()
		{
			TestStatement("i = i ^ 2", "i = Math.Pow(i, 2);");
			TestStatement("i ^= 2", "i = Math.Pow(i, 2);");
		}
		
		[Test]
		public void AddNotParenthesis()
		{
			TestStatement("a = Not b = c", "a = !(b == c);");
		}
		
		[Test]
		public void NotIsNothing()
		{
			TestStatement("a = Not fs Is Nothing",
			              "a = (fs != null);");
		}
		
		[Test]
		public void StaticMethodVariable()
		{
			TestMember(@"Private Sub Test
	Static j As Integer
	j += 1
End Sub
Private Shared Sub Test2
	Static j As Integer
	j += 2
End Sub",
			           @"int static_Test_j;
private void Test()
{
  static_Test_j += 1;
}
static int static_Test2_j;
private static void Test2()
{
  static_Test2_j += 2;
}");
		}
		
		[Test]
		public void StaticMethodVariableWithInitialization()
		{
			TestMember(@"Private Sub Test
	Static j As Integer = 0
	j += 1
End Sub",
			           @"readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_Test_j_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
int static_Test_j;
private void Test()
{
  lock (static_Test_j_Init) {
    try {
      if (InitStaticVariableHelper(static_Test_j_Init)) {
        static_Test_j = 0;
      }
    } finally {
      static_Test_j_Init.State = 1;
    }
  }
  static_Test_j += 1;
}
static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
{
  if (flag.State == 0) {
    flag.State = 2;
    return true;
  } else if (flag.State == 2) {
    throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
  } else {
    return false;
  }
}");
		}
		
		[Test]
		public void StaticMethodVariableWithInitialization2()
		{
			TestMember(@"Private Sub Test
	Static j As Integer = 10
	j += 1
End Sub
Private Shared Sub Test2
	Static j As Integer = 20
	j += 2
End Sub",
			           @"readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_Test_j_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
int static_Test_j;
private void Test()
{
  lock (static_Test_j_Init) {
    try {
      if (InitStaticVariableHelper(static_Test_j_Init)) {
        static_Test_j = 10;
      }
    } finally {
      static_Test_j_Init.State = 1;
    }
  }
  static_Test_j += 1;
}
static readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_Test2_j_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
static int static_Test2_j;
private static void Test2()
{
  lock (static_Test2_j_Init) {
    try {
      if (InitStaticVariableHelper(static_Test2_j_Init)) {
        static_Test2_j = 20;
      }
    } finally {
      static_Test2_j_Init.State = 1;
    }
  }
  static_Test2_j += 2;
}
static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
{
  if (flag.State == 0) {
    flag.State = 2;
    return true;
  } else if (flag.State == 2) {
    throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
  } else {
    return false;
  }
}");
		}
		
		[Test]
		public void UsingStatementForExistingVariable()
		{
			TestStatement("Using sw\nEnd Using",
			              "using (sw) {\n}");
		}
		
		[Test]
		public void WithStatementTest()
		{
			TestStatement("With Ejes\n" +
			              "  .AddLine(p1, p2)\n" +
			              "End With",
			              "var _with1 = Ejes;\n_with1.AddLine(p1, p2);");
		}
		
		[Test]
		public void NestedWithStatements()
		{
			TestStatement(
				"With tb1\n" +
				"  With .Font\n" +
				"    .Italic = True\n" +
				"  End With\n" +
				"End With",
				
				"var _with1 = tb1;\nvar _with2 = _with1.Font;\n_with2.Italic = true;");
		}
		
		[Test]
		public void NestedWithStatements2()
		{
			TestStatement(
				"With tb1\n" +
				"  With .Something.Font\n" +
				"    .Italic = True\n" +
				"  End With\n" +
				"End With",
				
				"var _with1 = tb1;\nvar _with2 = _with1.Something.Font;\n_with2.Italic = true;");
		}
		
		[Test]
		public void StructureWithImplicitPublicField()
		{
			TestMember("Public Structure Example \n Dim x As Object \n End Structure",
			           "public struct Example\n{\n  public object x;\n}");
		}
		
		[Test]
		public void InnerClassVisibility()
		{
			TestMember("Class Inner \n End Class",
			           "public class Inner\n{\n}");
		}
		
		[Test]
		public void InnerDelegateVisibility()
		{
			TestMember("Delegate Sub Test()",
			           "public delegate void Test();");
		}
		
		[Test]
		public void InterfaceVisibility()
		{
			TestMember("Public Interface ITest\n" +
			           "  Sub Test()\n" +
			           "  Property Name As String\n" +
			           "End Interface",
			           "public interface ITest\n" +
			           "{\n" +
			           "  void Test();\n" +
			           "  string Name { get; set; }\n" +
			           "}");
		}
		
		[Test]
		public void ImportAliasPrimitiveType()
		{
			TestProgram("Imports T = System.Boolean", "using T = System.Boolean;" + Environment.NewLine);
		}
		
		[Test]
		public void GlobalTypeReference()
		{
			TestStatement("Dim a As Global.System.String", "global::System.String a = null;");
		}
		
		[Test]
		public void FieldReferenceOnCastExpression()
		{
			TestStatement("CType(obj, IDisposable).Dispose()", "((IDisposable)obj).Dispose();");
		}
		
		[Test]
		public void ComparisonWithEmptyStringLiteral()
		{
			TestStatement("If a = \"\" Then Return", "if (string.IsNullOrEmpty(a))" + Environment.NewLine + "  return;");
			TestStatement("If a <> \"\" Then Return", "if (!string.IsNullOrEmpty(a))" + Environment.NewLine + "  return;");
			
			TestStatement("If \"\" = a Then Return", "if (string.IsNullOrEmpty(a))" + Environment.NewLine + "  return;");
			TestStatement("If \"\" <> a Then Return", "if (!string.IsNullOrEmpty(a))" + Environment.NewLine + "  return;");
		}
		
		[Test]
		public void ElseIfConversion()
		{
			TestStatement("If a Then\nElse If b Then\nElse\nEnd If",
			              "if (a) {\n" +
			              "} else if (b) {\n" +
			              "} else {\n" +
			              "}");
		}
		
		[Test]
		public void ArrayCreationUpperBound()
		{
			TestStatement("Dim i As String() = New String(1) {}",
			              "string[] i = new string[2];");
			TestStatement("Dim i(1) As String",
			              "string[] i = new string[2];");
			TestStatement("Dim i As String() = New String(1) {\"0\", \"1\"}",
			              "string[] i = new string[2] {" + Environment.NewLine +
			              "  \"0\"," + Environment.NewLine +
			              "  \"1\"" + Environment.NewLine +
			              "};");
			TestStatement("Dim i As String(,) = New String(5, 5) {}",
			              "string[,] i = new string[6, 6];");
		}
		
		[Test]
		public void InitializeLocalVariables()
		{
			TestStatement("Dim x As Integer", "int x = 0;");
			TestStatement("Dim x As Object", "object x = null;");
			TestStatement("Dim x As String", "string x = null;");
			TestStatement("Dim x", "object x = null;");
			TestStatement("Dim x As Char", "char x = '\\0';");
			TestStatement("Dim x As System.DateTime", "System.DateTime x = default(System.DateTime);");
		}
		
		[Test]
		public void ExpressionAsLoopVariable()
		{
			TestStatement("For Me.Field = 1 To 10 : Next Me.Field", "for (this.Field = 1; this.Field <= 10; this.Field++) {\n}");
		}
		
		[Test]
		public void ConstModuleMember()
		{
			TestProgram("Module Test : Public Const C As Integer = 0 : End Module",
			            "static class Test" + Environment.NewLine +
			            "{" + Environment.NewLine +
			            "  public const int C = 0;" + Environment.NewLine +
			            "}" + Environment.NewLine);
		}
		
		[Test]
		public void CastToInteger()
		{
			TestStatement("Dim x As Integer = CInt(obj)", "int x = Convert.ToInt32(obj);");
		}
		
		[Test]
		public void ConditionalExprPrecedence()
		{
			TestStatement("Dim x As Integer = If(If(a,b,c),d,e)", "int x = (a ? b : c) ? d : e;");
			TestStatement("Dim x As Integer = If(a,If(b,c,d),e)", "int x = a ? b ? c : d : e;");
			TestStatement("Dim x As Integer = If(a,b,If(c,d,e))", "int x = a ? b : c ? d : e;");
		}
		
		[Test]
		public void XmlElement()
		{
			TestStatement("Dim xml = <Test />",
			              @"var xml = new XElement(""Test"");");
		}
		
		[Test]
		public void XmlElement2()
		{
			TestStatement(@"Dim xml = <Test name=""test"" name2=<%= testVal %> />",
			              @"var xml = new XElement(""Test"", new XAttribute(""name"", ""test""), new XAttribute(""name2"", testVal));");
		}
		
		[Test]
		public void XmlElement3()
		{
			TestStatement(@"Dim xml = <Test name=""test"" name2=<%= testVal %> <%= testVal2 %> />",
			              @"var xml = new XElement(""Test"", new XAttribute(""name"", ""test""), new XAttribute(""name2"", testVal), testVal2);");
		}
		
		[Test]
		public void XmlNestedElement()
		{
			TestStatement("Dim xml = <Test>      <Test2 />        </Test>",
			              @"var xml = new XElement(""Test"", new XElement(""Test2""));");
		}
		
		[Test]
		public void XmlNestedElement2()
		{
			TestStatement("Dim xml = <Test>      <Test2 />    hello    </Test>",
			              @"var xml = new XElement(""Test"", new XElement(""Test2""), ""    hello    "");");
		}
		
		[Test]
		public void XmlNestedElement3()
		{
			TestStatement("Dim xml = <Test>      <Test2 a='b' />    hello    </Test>",
			              @"var xml = new XElement(""Test"", new XElement(""Test2"", new XAttribute(""a"", ""b"")), ""    hello    "");");
		}
		
		[Test]
		public void XmlNestedElement4()
		{
			TestStatement("Dim xml = <Test>      <Test2 a='b' />    hello    \t<![CDATA[any & <>]]></Test>",
			              @"var xml = new XElement(""Test"", new XElement(""Test2"", new XAttribute(""a"", ""b"")), ""    hello    \t"", new XCData(""any & <>""));");
		}
		
		[Test]
		public void XmlComment()
		{
			TestStatement("Dim xml = <!-- test -->",
			              @"var xml = new XComment("" test "");");
		}
		
		[Test]
		public void XmlCData()
		{
			TestStatement("Dim xml = <![CDATA[any & <> char]]>",
			              @"var xml = new XCData(""any & <> char"");");
		}
		
		[Test]
		public void XmlProcessingInstruction()
		{
			TestStatement("Dim xml = <?target testcontent?>",
			              @"var xml = new XProcessingInstruction(""target"", "" testcontent"");");
		}
		
		[Test]
		public void XmlDocumentTest()
		{
			TestStatement(@"Dim xml = <?xml version=""1.0""?><!-- test --><Data a='true'><A/></Data><!-- test -->",
			              @"var xml = new XDocument(new XDeclaration(""1.0"", null, null), new XComment("" test ""), new XElement(""Data"", new XAttribute(""a"", ""true""), new XElement(""A"")), new XComment("" test ""));");
		}
		
		
		[Test]
		public void XmlDocumentTest2()
		{
			TestStatement(@"Dim xml = <?xml?><!-- test --><Data a='true'><A/></Data><!-- test -->",
			              @"var xml = new XDocument(new XDeclaration(null, null, null), new XComment("" test ""), new XElement(""Data"", new XAttribute(""a"", ""true""), new XElement(""A"")), new XComment("" test ""));");
		}
		
		[Test]
		public void XmlDocumentTest3()
		{
			TestStatement(@"Dim xml = <?xml version=""1.0"" encoding=""utf-8""?><!-- test --><Data a='true'><A/></Data><!-- test -->",
			              @"var xml = new XDocument(new XDeclaration(""1.0"", ""utf-8"", null), new XComment("" test ""), new XElement(""Data"", new XAttribute(""a"", ""true""), new XElement(""A"")), new XComment("" test ""));");
		}
		
		[Test]
		public void XmlDocumentTest4()
		{
			TestStatement(@"Dim xml = <?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?><!-- test --><Data a='true'><A <%= content %> /></Data><!-- test -->",
			              @"var xml = new XDocument(new XDeclaration(""1.0"", ""utf-8"", ""yes""), new XComment("" test ""), new XElement(""Data"", new XAttribute(""a"", ""true""), new XElement(""A"", content)), new XComment("" test ""));");
		}
		
		[Test]
		public void XmlEmbeddedExpression()
		{
			TestStatement(@"Dim xml = <<%= name %>>Test</>",
			              @"var xml = new XElement(name, ""Test"");");
		}
		
		[Test]
		public void XmlEmbeddedExpression2()
		{
			TestStatement(@"Dim xml = <<%= name %>><%= content %></>",
			              @"var xml = new XElement(name, content);");
		}
		
		[Test]
		public void XmlEntityReference()
		{
			TestStatement(@"Dim xml = <A b=""&quot;""/>",
			              @"var xml = new XElement(""A"", new XAttribute(""b"", ""\""""));");
		}
		
		[Test]
		public void XmlEntityReference2()
		{
			TestStatement(@"Dim xml = <A>&quot;</A>",
			              @"var xml = new XElement(""A"", ""\"""");");
		}

        [Test]
        public void XmlLINQDescendants()
        {
            TestStatement(@"Dim element = someXml...<somename>",
                          @"var element = someXml.Descendants(""somename"");");
        }
        [Test]
        public void XmlLINQElements()
        {
            TestStatement(@"Dim element = someXml.<somename>",
                          @"var element = someXml.Elements(""somename"");");
        }

        [Test]
        public void XmlLINQAttribute()
        {
            TestStatement(@"Dim value = someXml.@attr",
                          @"var value = someXml.Attribute(""attr"").Value;");
        }

        [Test]
        public void XmlLINQAttributeSetConstant()
        {
            TestStatement(@"someElement.@someAttr = 8",
                          @"someElement.SetAttributeValue(""someAttr"", 8);");
        }

        [Test]
        public void XmlLINQAttributeSetExpression()
        {
            TestStatement(@"someElement.@someAttr = string.Format(""{0}"", 19)",
                          @"someElement.SetAttributeValue(""someAttr"", string.Format(""{0}"", 19));");
        }

        [Test]
        public void LinqQueryWhereSelect()
        {
            TestStatement(@"Dim value = From value In values Where value = ""someValue"" Select value",
                          @"var value = from value in values where value == ""someValue"" select value;");
        }

		[Test]
		public void SD2_1500a()
		{
			TestProgram(
				@"Public Class Class1
    Private PFoo As String
    Public Property Foo() As String
        Get
            Foo = PFoo
        End Get
        Set(ByVal Value As String)
            PFoo = Value
        End Set
    End Property
End Class",
				@"public class Class1
{
  private string PFoo;
  public string Foo {
    get { return PFoo; }
    set { PFoo = value; }
  }
}
"
			);
		}
		
		[Test]
		public void SD2_1500b()
		{
			TestMember(
				@"Function Test As Integer
	Test = 5
End Function",
				@"public int Test()
{
  return 5;
}"
			);
		}
		
		[Test]
		public void SD2_1500c()
		{
			TestMember(
				@"Function Test As Integer
	If True Then Test = 3
	Test = 5
End Function",
				@"public int Test()
{
  int functionReturnValue = 0;
  if (true)
    functionReturnValue = 3;
  functionReturnValue = 5;
  return functionReturnValue;
}"
			);
		}
		
		[Test]
		public void SD2_1500d()
		{
			TestMember(
				@"Function Test As Integer
	If True Then
		Test = 3
		Exit Function
	End If
	Test = 5
End Function",
				@"public int Test()
{
  int functionReturnValue = 0;
  if (true) {
    functionReturnValue = 3;
    return functionReturnValue;
  }
  functionReturnValue = 5;
  return functionReturnValue;
}"
			);
		}
		
		[Test]
		public void SD2_1497()
		{
			TestMember(
				@"Public Function Bug() As String
  With New System.Text.StringBuilder(""Hi! "")
     .Append(""you "")
     .Append(""folks "")
     .Append(""from "")
     .Append(""sharpdevelop!"")
     Return .ToString()
  End With
End Function",
				@"public string Bug()
{
  var _with1 = new System.Text.StringBuilder(""Hi! "");
  _with1.Append(""you "");
  _with1.Append(""folks "");
  _with1.Append(""from "");
  _with1.Append(""sharpdevelop!"");
  return _with1.ToString();
}
"
			);
		}
	}
}
