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
	public class VBToCSharpConverterTest
	{
		public void TestProgram(string input, string expectedOutput)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(input));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			parser.CompilationUnit.AcceptVisitor(new VBNetToCSharpConvertVisitor(), null);
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput, outputVisitor.Text);
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
					b.Append("\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("}");
			TestProgram("Class tmp1 \n" + input + "\nEnd Class", b.ToString());
		}
		
		public void TestStatement(string input, string expectedOutput)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("class tmp1");
			b.AppendLine("{");
			b.AppendLine("\tpublic void tmp2()");
			b.AppendLine("\t{");
			using (StringReader r = new StringReader(expectedOutput)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("\t\t");
					b.AppendLine(line);
				}
			}
			b.AppendLine("\t}");
			b.AppendLine("}");
			TestProgram("Class tmp1 \n Sub tmp2() \n" + input + "\n End Sub \n End Class", b.ToString());
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
			              "if (someEvent != null) {\n\tsomeEvent(this, EventArgs.Empty);\n}");
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
			           "public object A {\n\tget { return null; }\n}");
		}
		
		[Test]
		public void ValueInPropertySetter()
		{
			TestMember("WriteOnly Property A()\nSet\nDim x As Object = Value\nEnd Set\nEnd Property",
			           "public object A {\n\tset {\n\t\tobject x = value;\n\t}\n}");
		}
		
		[Test]
		public void ValueInPropertySetter2()
		{
			TestMember("WriteOnly Property A()\nSet(ByVal otherName)\nDim x As Object = otherName\nEnd Set\nEnd Property",
			           "public object A {\n\tset {\n\t\tobject x = value;\n\t}\n}");
		}
		
		[Test]
		public void AbstractProperty1()
		{
			TestMember("Public MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary {\n\tget;\n\tset;\n}");
		}
		
		[Test]
		public void AbstractProperty2()
		{
			TestMember("Public ReadOnly MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary {\n\tget;\n}");
		}
		
		[Test]
		public void AbstractProperty3()
		{
			TestMember("Public WriteOnly MustOverride Property Salary() As Decimal",
			           "public abstract decimal Salary {\n\tset;\n}");
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
			              "float example;\n" +
			              "float test;");
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
		public void Constructor()
		{
			TestMember("Sub New()\n\tMyBase.New(1)\nEnd Sub",
			           "public tmp1() : base(1)\n{\n}");
			TestMember("Public Sub New()\n\tMe.New(1)\nEnd Sub",
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
			           "\tTry\n" +
			           "\t\tDead()\n" +
			           "\tFinally\n" +
			           "\t\tMyBase.Finalize()\n" +
			           "\tEnd Try\n" +
			           "End Sub",
			           
			           "~tmp1()\n" +
			           "{\n" +
			           "\tDead();\n" +
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
		}
		
		[Test]
		public void DoLoop()
		{
			TestStatement("Do \n Loop",
			              "do {\n" +
			              "}\n" +
			              "while (true);");
			TestStatement("Do \n Loop Until i = 10000",
			              "do {\n" +
			              "}\n" +
			              "while (!(i == 10000));");
		}
		
		[Test]
		public void UsingStatement()
		{
			TestStatement("Using r1 As New StreamReader(file1), r2 As New StreamReader(file2)\n" +
			              "End Using",
			              "using (StreamReader r1 = new StreamReader(file1)) {\n" +
			              "\tusing (StreamReader r2 = new StreamReader(file2)) {\n" +
			              "\t}\n" +
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
			              "\tcase 0:\n" +
			              "\tcase 1:\n" +
			              "\tcase 2:\n" +
			              "\tcase 3:\n" +
			              "\tcase 4:\n" +
			              "\tcase 5:\n" +
			              "\t\ti = 10;\n" +
			              "\t\tbreak;\n" +
			              "\tcase 11:\n" +
			              "\t\ti = 0;\n" +
			              "\t\tbreak;\n" +
			              "\tdefault:\n" +
			              "\t\ti = 9;\n" +
			              "\t\tbreak;\n" +
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
			           "\treturn 0;\n" +
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
			           "\treturn 0;\n" +
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
			           "\tint " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = 0;\n" +
			           "\twhile (something) {\n" +
			           "\t\t" + VBNetConstructsConvertVisitor.FunctionReturnValueName + " += i;\n" +
			           "\t}\n" +
			           "\treturn " + VBNetConstructsConvertVisitor.FunctionReturnValueName + ";\n" +
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
			           "\tint " + ReturnValueName + " = 0;\n" +
			           "\twhile (something) {\n" +
			           "\t\t" + ReturnValueName + " = " + ReturnValueName + " + run(i - 1);\n" +
			           "\t}\n" +
			           "\treturn " + ReturnValueName + ";\n" +
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
			           "\tCustomType " + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = null;\n" +
			           "\twhile (something) {\n" +
			           "\t\t" + VBNetConstructsConvertVisitor.FunctionReturnValueName + " = new CustomType();\n" +
			           "\t}\n" +
			           "\treturn " + VBNetConstructsConvertVisitor.FunctionReturnValueName + ";\n" +
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
	Static j As Integer = 0
	j += 1
End Sub",
			           @"private void Test()
{
	static_Test_j += 1;
}
static int static_Test_j = 0;");
		}
		
		[Test]
		public void StaticMethodVariable2()
		{
			TestMember(@"Private Sub Test
	Static j As Integer = 0
	j += 1
End Sub
Private Sub Test2
	Static j As Integer = 0
	j += 2
End Sub",
			           @"private void Test()
{
	static_Test_j += 1;
}
static int static_Test_j = 0;
private void Test2()
{
	static_Test2_j += 2;
}
static int static_Test2_j = 0;");
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
			              "\t.AddLine(p1, p2)\n" +
			              "End With",
			              "{\n\tEjes.AddLine(p1, p2);\n}");
		}
		
		[Test]
		public void StructureWithImplicitPublicField()
		{
			TestMember("Public Structure Example \n Dim x As Object \n End Structure",
			           "public struct Example\n{\n\tpublic object x;\n}");
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
			           "\tSub Test()\n" +
			           "\tProperty Name As String\n" +
			           "End Interface",
			           "public interface ITest\n" +
			           "{\n" +
			           "\tvoid Test();\n" +
			           "\tstring Name {\n" +
			           "\t\tget;\n" +
			           "\t\tset;\n" +
			           "\t}\n" +
			           "}");
		}
		
		[Test]
		public void ImportAliasPrimitiveType()
		{
			TestProgram("Imports T = System.Boolean", "using T = System.Boolean;\r\n");
		}
		
		[Test]
		public void GlobalTypeReference()
		{
			TestStatement("Dim a As Global.System.String", "global::System.String a;");
		}
		
		[Test]
		public void FieldReferenceOnCastExpression()
		{
			TestStatement("CType(obj, IDisposable).Dispose()", "((IDisposable)obj).Dispose();");
		}
	}
}
