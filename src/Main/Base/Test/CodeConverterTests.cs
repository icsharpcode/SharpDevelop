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

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using ICSharpCode.NRefactory;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests advanced code-conversion features that rely on the type system.
	/// </summary>
	[TestFixture]
	public class CodeConverterTests
	{
		#region TestProgram (converting code)
		void TestProgramCS2VB(string sourceCode, string expectedOutput)
		{
			Assert.Ignore("Code converter is not implemented");
		}
		
		void TestProgramVB2CS(string sourceCode, string expectedOutput)
		{
			Assert.Ignore("Code converter is not implemented");
		}
		
		/*
		void TestProgram(SupportedLanguage sourceLanguage, string sourceCode, string expectedOutput)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(projectContentRegistry.Mscorlib);
			pc.ReferencedContents.Add(projectContentRegistry.GetProjectContentForReference("System.Windows.Forms", typeof(System.Windows.Forms.Form).Module.FullyQualifiedName));
			if (sourceLanguage == SupportedLanguage.VBNet) {
				pc.ReferencedContents.Add(projectContentRegistry.GetProjectContentForReference("Microsoft.VisualBasic", typeof(Microsoft.VisualBasic.Constants).Module.FullyQualifiedName));
				pc.DefaultImports = new DefaultUsing(pc);
				pc.DefaultImports.Usings.Add("System");
				pc.DefaultImports.Usings.Add("Microsoft.VisualBasic");
			}
			pc.Language = sourceLanguage == SupportedLanguage.CSharp ? LanguageProperties.CSharp : LanguageProperties.VBNet;
			HostCallback.GetCurrentProjectContent = delegate {
				return pc;
			};
			
			ICSharpCode.NRefactory.IParser parser = ParserFactory.CreateParser(sourceLanguage, new StringReader(sourceCode));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc, sourceLanguage);
			visitor.VisitSyntaxTree(parser.SyntaxTree, null);
			visitor.Cu.FileName = sourceLanguage == SupportedLanguage.CSharp ? "a.cs" : "a.vb";
			foreach (IClass c in visitor.Cu.Classes) {
				pc.AddClassToNamespaceList(c);
			}
			
			ParseInformation parseInfo = new ParseInformation(visitor.Cu);
			
			if (sourceLanguage == SupportedLanguage.CSharp) {
				CSharpToVBNetConvertVisitor convertVisitor = new CSharpToVBNetConvertVisitor(pc, parseInfo);
				convertVisitor.RootNamespaceToRemove = "RootNamespace";
				parser.SyntaxTree.AcceptVisitor(convertVisitor, null);
			} else {
				VBNetToCSharpConvertVisitor convertVisitor = new VBNetToCSharpConvertVisitor(pc, parseInfo);
				parser.SyntaxTree.AcceptVisitor(convertVisitor, null);
			}
			
			IOutputAstVisitor outputVisitor = sourceLanguage == SupportedLanguage.CSharp ? (IOutputAstVisitor)new VBNetOutputVisitor() : new CSharpOutputVisitor();
			outputVisitor.Options.IndentationChar = ' ';
			outputVisitor.Options.IndentSize = 2;
			using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(),
			                                    outputVisitor)) {
				outputVisitor.VisitSyntaxTree(parser.SyntaxTree, null);
			}
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput.Replace("\r", ""), outputVisitor.Text.Trim().Replace("\r", ""));
		}*/
		#endregion
		
		#region TestMember/TestStatement/TestExpression
		string IndentAllLines(string code)
		{
			StringBuilder b = new StringBuilder();
			using (StringReader r = new StringReader(code)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					b.Append("  ");
					b.AppendLine(line);
				}
			}
			return b.ToString();
		}
		
		void TestMemberCS2VB(string sourceCode, string expectedCode)
		{
			TestProgramCS2VB("using System;\n" +
			                 "class MyClassName {\n" +
			                 IndentAllLines(sourceCode) +
			                 "}",
			                 
			                 "Imports System\n" +
			                 "Class MyClassName\n" +
			                 IndentAllLines(expectedCode) +
			                 "End Class");
		}
		
		const string DefaultUsingsCSharp = "using System;\nusing Microsoft.VisualBasic;\n";
		
		void TestMemberVB2CS(string sourceCode, string expectedCode)
		{
			TestProgramVB2CS("Class MyClassName\n" +
			                 IndentAllLines(sourceCode) +
			                 "End Class",
			                 
			                 DefaultUsingsCSharp +
			                 "class MyClassName\n{\n" +
			                 IndentAllLines(expectedCode) +
			                 "}");
		}
		
		void TestStatementsCS2VB(string sourceCode, string expectedCode)
		{
			TestMemberCS2VB("void T() {\n" +
			                IndentAllLines(sourceCode) +
			                "}",
			                
			                "Private Sub T()\n" +
			                IndentAllLines(expectedCode) +
			                "End Sub");
		}
		
		void TestStatementsVB2CS(string sourceCode, string expectedCode)
		{
			TestMemberVB2CS("Private Sub T()\n" +
			                IndentAllLines(sourceCode) +
			                "End Sub",
			                "private void T()\n{\n" +
			                IndentAllLines(expectedCode) +
			                "}");
		}
		#endregion
		
		#region Events and delegates
		[Test]
		public void RaiseEventCS2VB()
		{
			TestMemberCS2VB("public event EventHandler Click;" +
			                "void T() { if (Click != null) { Click(this, EventArgs.Empty); } }",
			                "Public Event Click As EventHandler\n" +
			                "Private Sub T()\n" +
			                "  RaiseEvent Click(Me, EventArgs.Empty)\n" +
			                "End Sub");
			
			TestMemberCS2VB("public event EventHandler Click;" +
			                "void T() { Click(this, EventArgs.Empty); }",
			                "Public Event Click As EventHandler\n" +
			                "Private Sub T()\n" +
			                "  RaiseEvent Click(Me, EventArgs.Empty)\n" +
			                "End Sub");
		}
		
		
		[Test]
		public void EventHandlerTest()
		{
			TestMemberCS2VB("public event EventHandler Click;" +
			                "void T() {" +
			                "  Click += T;" +
			                "  Click -= this.T;" +
			                "  Click += new EventHandler(T);" +
			                "  Click += new EventHandler(this.T);" +
			                "  EventHandler eh = new EventHandler(T);" +
			                "  eh = T;" +
			                "  eh += eh;" +
			                "  eh -= eh;" +
			                "  this.Click += eh;" +
			                "}",
			                "Public Event Click As EventHandler\n" +
			                "Private Sub T()\n" +
			                "  AddHandler Click, AddressOf T\n" +
			                "  RemoveHandler Click, AddressOf Me.T\n" +
			                "  AddHandler Click, New EventHandler(AddressOf T)\n" +
			                "  AddHandler Click, New EventHandler(AddressOf Me.T)\n" +
			                "  Dim eh As New EventHandler(AddressOf T)\n" +
			                "  eh = AddressOf T\n" +
			                "  eh = DirectCast([Delegate].Combine(eh, eh), EventHandler)\n" +
			                "  eh = DirectCast([Delegate].Remove(eh, eh), EventHandler)\n" +
			                "  AddHandler Me.Click, eh\n" +
			                "End Sub");
		}
		
		[Test]
		public void CreateDelegateCS2VB()
		{
			TestProgramCS2VB("using System; using System.Text.RegularExpressions;\n" +
			                 "class Test {\n" +
			                 "  object M() {\n" +
			                 "    return new MatchEvaluator(X);\n" +
			                 "  }\n" +
			                 "  string X(Match match) {}" +
			                 "}",
			                 "Imports System\n" +
			                 "Imports System.Text.RegularExpressions\n" +
			                 "Class Test\n" +
			                 "  Private Function M() As Object\n" +
			                 "    Return New MatchEvaluator(AddressOf X)\n" +
			                 "  End Function\n" +
			                 "  Private Function X(match As Match) As String\n" +
			                 "  End Function\n" +
			                 "End Class");
		}
		
		[Test]
		public void ImplicitlyCreateDelegateCS2VB()
		{
			TestProgramCS2VB("using System; using System.Text.RegularExpressions;\n" +
			                 "class Test {\n" +
			                 "  void M(Regex regex, string text) {\n" +
			                 "    regex.Replace(text, X);\n" +
			                 "  }\n" +
			                 "  string X(Match match) {}" +
			                 "}",
			                 "Imports System\n" +
			                 "Imports System.Text.RegularExpressions\n" +
			                 "Class Test\n" +
			                 "  Private Sub M(regex As Regex, text As String)\n" +
			                 "    regex.Replace(text, AddressOf X)\n" +
			                 "  End Sub\n" +
			                 "  Private Function X(match As Match) As String\n" +
			                 "  End Function\n" +
			                 "End Class");
		}
		
		[Test]
		public void HandlesClassEvent()
		{
			TestProgramVB2CS("Imports System.Windows.Forms\n" +
			                 "Class Test\n" +
			                 "  Inherits Form\n" +
			                 "  Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load\n" +
			                 "  End Sub\n" +
			                 "  Sub Base_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint\n" +
			                 "  End Sub\n" +
			                 "End Class",
			                 DefaultUsingsCSharp +
			                 "using System.Windows.Forms;\n" +
			                 "class Test : Form\n" +
			                 "{\n" +
			                 "  public void Me_Load(object sender, EventArgs e)\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public void Base_Paint(object sender, PaintEventArgs e)\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public Test()\n" +
			                 "  {\n" +
			                 "    Paint += Base_Paint;\n" +
			                 "    Load += Me_Load;\n" +
			                 "  }\n" +
			                 "}");
		}
		#endregion
		
		#region ReferenceEqualityAndValueEquality
		[Test]
		public void ReferenceEqualityAndValueEquality()
		{
			// Reference equality:
			TestStatementsCS2VB("object a = new object();\n" +
			                    "object b = new object();\n" +
			                    "if (a == b) {\n" +
			                    "}",
			                    
			                    "Dim a As New Object()\n" +
			                    "Dim b As New Object()\n" +
			                    "If a Is b Then\n" +
			                    "End If");
			
			// Value type equality:
			TestStatementsCS2VB("int a = 3;\n" +
			                    "int b = 4;\n" +
			                    "if (a == b) {\n" +
			                    "}",
			                    
			                    "Dim a As Integer = 3\n" +
			                    "Dim b As Integer = 4\n" +
			                    "If a = b Then\n" +
			                    "End If");
			
			// String equality:
			TestStatementsCS2VB("string a = \"3\";\n" +
			                    "string b = \"4\";\n" +
			                    "if (a == b) {\n" +
			                    "}",
			                    
			                    "Dim a As String = \"3\"\n" +
			                    "Dim b As String = \"4\"\n" +
			                    "If a = b Then\n" +
			                    "End If");
		}
		#endregion
		
		#region FixVBCasing
		[Test]
		public void FixVBCasing()
		{
			TestStatementsVB2CS("Dim obj as iDisposable\n" +
			                    "Obj.dispose()",
			                    "IDisposable obj = null;\n" +
			                    "obj.Dispose();");
		}
		
		[Test]
		public void FixVBCasingAndAddMethodCallParenthesis()
		{
			TestStatementsVB2CS("Dim i as Integer = appdomain.getcurrentthreadid",
			                    "int i = AppDomain.GetCurrentThreadId();");
		}
		
		[Test]
		public void FixVBCasingAndAddMethodCallParenthesis2()
		{
			TestStatementsVB2CS("console.writeline(appdomain.getcurrentthreadid)",
			                    "Console.WriteLine(AppDomain.GetCurrentThreadId());");
		}
		
		[Test]
		public void FixVBCasingAndAddMethodCallParenthesis3()
		{
			TestStatementsVB2CS("console.writeline(T)",
			                    "Console.WriteLine(T());");
		}
		#endregion
		
		#region Redim
		[Test]
		public void Redim()
		{
			TestStatementsVB2CS("Dim i(10) as Integer\n" +
			                    "Redim i(20)",
			                    "int[] i = new int[11];\n" +
			                    "i = new int[21];");
		}
		
		[Test]
		public void RedimPreserve()
		{
			TestStatementsVB2CS("Dim i(10) as Integer\n" +
			                    "Redim Preserve i(20)",
			                    "int[] i = new int[11];\n" +
			                    "Array.Resize(ref i, 21);");
		}
		
		[Test]
		public void RedimMultidimensional()
		{
			TestStatementsVB2CS("Dim MyArray(,) as Integer\n" +
			                    "ReDim MyArray(5, 5)",
			                    "int[,] MyArray = null;\n" +
			                    "MyArray = new int[6, 6];\n");
		}
		
		[Test]
		public void RedimMultidimensionalPreserve()
		{
			TestStatementsVB2CS("Dim MyArray(5, 5) as Integer\n" +
			                    "ReDim Preserve MyArray(10, 10)",
			                    "int[,] MyArray = new int[6, 6];\n" +
			                    "MyArray = (int[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(MyArray, new int[11, 11]);");
		}
		#endregion
		
		[Test]
		public void FullyQualifyNamespaceReferencesInIdentifiers()
		{
			TestStatementsVB2CS("IO.Path.GetTempPath",
			                    "System.IO.Path.GetTempPath();");
		}
		
		[Test]
		public void FullyQualifyNamespaceReferencesInTypeName()
		{
			TestStatementsVB2CS("Dim a As Collections.ICollection = New Collections.ArrayList",
			                    "System.Collections.ICollection a = new System.Collections.ArrayList();");
		}
		
		[Test]
		public void AutomaticInitializeComponentCall()
		{
			TestProgramVB2CS("Imports System.Windows.Forms\n" +
			                 "<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _\n" +
			                 "Class Test\n" +
			                 "  Inherits Form\n" +
			                 "  Private Sub InitializeComponent()\n" +
			                 "  End Sub\n" +
			                 "End Class",
			                 DefaultUsingsCSharp +
			                 "using System.Windows.Forms;\n" +
			                 "[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]\n" +
			                 "class Test : Form\n" +
			                 "{\n" +
			                 "  private void InitializeComponent()\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public Test()\n" +
			                 "  {\n" +
			                 "    InitializeComponent();\n" +
			                 "  }\n" +
			                 "}");
		}
		
		[Test]
		public void IndexerExpression()
		{
			TestStatementsVB2CS("Dim i(10) as Integer\n" +
			                    "Dim i2 As Integer = i(4)",
			                    "int[] i = new int[11];\n" +
			                    "int i2 = i[4];");
			TestStatementsVB2CS("Dim s as string = appdomain.currentdomain.GetAssemblies()(1).location",
			                    "string s = AppDomain.CurrentDomain.GetAssemblies()[1].Location;");
		}
		
		[Test]
		public void StringConcatWithException()
		{
			TestStatementsCS2VB("Exception ex;\n" +
			                    "string a = \"Error: \" + ex + \"!\";",
			                    "Dim ex As Exception\n" +
			                    "Dim a As String = \"Error: \" & Convert.ToString(ex) & \"!\"\n");
		}
		
		[Test]
		public void PerformIntegerDivision()
		{
			TestStatementsCS2VB("int a = 5; int b = 2;\n" +
			                    "int c = a / b;",
			                    "Dim a As Integer = 5\n" +
			                    "Dim b As Integer = 2\n" +
			                    "Dim c As Integer = a \\ b\n");
		}
		
		[Test]
		public void OperatorPrecedenceChange()
		{
			TestStatementsCS2VB("int a = 5;\n" +
			                    "int c = a / a * a;",
			                    "Dim a As Integer = 5\n" +
			                    "Dim c As Integer = (a \\ a) * a\n");
		}
		
		[Test]
		public void ForeachOnExistingVariable()
		{
			TestStatementsVB2CS("Dim a As String\n" +
			                    "For Each a In b\n" +
			                    "  Console.WriteLine(a)\n" +
			                    "Next",
			                    "string a = null;\n" +
			                    "foreach (string a_loopVariable in b) {\n" +
			                    "  a = a_loopVariable;\n" +
			                    "  Console.WriteLine(a);\n" +
			                    "}");
		}
		
		[Test]
		public void ConvertDefaultPropertyToIndexer()
		{
			TestStatementsVB2CS("Dim c As Collection\n" +
			                    "a = c.Item(2)",
			                    "Collection c = null;\n" +
			                    "a = c[2];");
		}
		
		[Test]
		public void PointerUsage()
		{
			TestStatementsCS2VB("int a = 2;" +
			                    "int* b = &a;" +
			                    "*b += 40;" +
			                    "Console.WriteLine(*b);",
			                    
			                    "Dim a As Integer = 2\n" +
			                    "Dim b As New Pointer(Of Integer)(a)\n" +
			                    "b.Target += 40\n" +
			                    "Console.WriteLine(b.Target)");
		}
		
		[Test]
		public void RemoveImportDuplicatedByProjectLevelImport()
		{
			TestProgramVB2CS("Imports System\nClass Test\nEnd Class",
			                 DefaultUsingsCSharp + "class Test\n{\n}");
		}
		
		[Test]
		public void CallMethodOnModule()
		{
			TestProgramVB2CS("Class Test\n" +
			                 "  Sub A\n" +
			                 "    Method(Field)\n" +
			                 "  End Sub\n" +
			                 "End Class\n" +
			                 "Module TheModule\n" +
			                 "  Sub Method(a As Integer)\n" +
			                 "  End Sub\n" +
			                 "  Public Field As Integer\n" +
			                 "End Module",
			                 DefaultUsingsCSharp +
			                 "class Test\n" +
			                 "{\n" +
			                 "  public void A()\n" +
			                 "  {\n" +
			                 "    TheModule.Method(TheModule.Field);\n" +
			                 "  }\n" +
			                 "}\n" +
			                 "static class TheModule\n" +
			                 "{\n" +
			                 "  public static void Method(int a)\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public static int Field;\n" +
			                 "}");
		}
		
		#region Casting
		[Test]
		public void CastToEnum()
		{
			TestStatementsCS2VB("DayOfWeek dow = (DayOfWeek)obj;\n",
			                    "Dim dow As DayOfWeek = CType(obj, DayOfWeek)\n");
		}
		
		[Test]
		public void CastToValueType()
		{
			TestStatementsCS2VB("Guid g = (Guid)obj;\n",
			                    "Dim g As Guid = CType(obj, Guid)\n");
		}
		
		[Test]
		public void CastToReferenceType()
		{
			TestStatementsCS2VB("Exception ex = (Exception)obj;\n",
			                    "Dim ex As Exception = DirectCast(obj, Exception)\n");
		}
		
		[Test]
		public void CastToInterface()
		{
			TestStatementsCS2VB("IDisposable ex = (IDisposable)obj;\n",
			                    "Dim ex As IDisposable = DirectCast(obj, IDisposable)\n");
		}
		
		[Test]
		public void CastIntegerToChar()
		{
			TestStatementsCS2VB("char c = (char)42;",
			                    "Dim c As Char = ChrW(42)");
		}
		#endregion
		
		#region MoveUsingOutOfNamespace
		[Test]
		public void MoveUsingOutOfNamespace()
		{
			TestProgramCS2VB("namespace Test\n" +
			                 "{\n" +
			                 "  using System;\n" +
			                 "  class Test {}" +
			                 "}\n",
			                 "Imports System\n" +
			                 "Namespace Test\n" +
			                 "  Class Test\n" +
			                 "  End Class\n" +
			                 "End Namespace");
		}
		
		[Test]
		public void MoveUsingOutOfNamespaceWithComments()
		{
			TestProgramCS2VB("// comment 1\n" +
			                 "namespace Test\n" +
			                 "{\n" +
			                 "  // comment 2\n" +
			                 "  using System;\n" +
			                 "  // comment 3\n" +
			                 "  class Test {}" +
			                 "}\n",
			                 "' comment 1\n" +
			                 "Imports System\n" +
			                 "Namespace Test\n" +
			                 "  ' comment 2\n" +
			                 "  ' comment 3\n" +
			                 "  Class Test\n" +
			                 "  End Class\n" +
			                 "End Namespace");
		}
		
		[Test]
		public void MoveUsingOutOfRootNamespace()
		{
			TestProgramCS2VB("namespace RootNamespace\n" +
			                 "{\n" +
			                 "  using System;\n" +
			                 "  class Test {}" +
			                 "}\n",
			                 "Imports System\n" +
			                 "Class Test\n" +
			                 "End Class");
		}
		
		[Test]
		public void MultipleNamespaces()
		{
			TestProgramCS2VB("namespace RootNamespace {\n" +
			                 "  class Test { }" +
			                 "}\n" +
			                 "namespace RootNamespace.SubNamespace {\n" +
			                 "  class Test2 { }" +
			                 "}",
			                 "Class Test\n" +
			                 "End Class\n" +
			                 "Namespace SubNamespace\n" +
			                 "  Class Test2\n" +
			                 "  End Class\n" +
			                 "End Namespace");
		}
		#endregion
		
		#region InterfaceImplementation
		[Test]
		public void InterfaceImplementation1()
		{
			TestProgramCS2VB("using System;\n" +
			                 "class Test : IDisposable {\n" +
			                 "  public void Dispose() { }" +
			                 "}",
			                 "Imports System\n" +
			                 "Class Test\n" +
			                 "  Implements IDisposable\n" +
			                 "  Public Sub Dispose() Implements IDisposable.Dispose\n" +
			                 "  End Sub\n" +
			                 "End Class");
		}
		
		[Test]
		public void InterfaceImplementation2()
		{
			TestProgramCS2VB("using System;\n" +
			                 "class Test : IServiceProvider {\n" +
			                 "  public object GetService(IntPtr a) { }\n" +
			                 "  public object GetService(Type a) { }\n" +
			                 "}",
			                 "Imports System\n" +
			                 "Class Test\n" +
			                 "  Implements IServiceProvider\n" +
			                 "  Public Function GetService(a As IntPtr) As Object\n" +
			                 "  End Function\n" +
			                 "  Public Function GetService(a As Type) As Object Implements IServiceProvider.GetService\n" +
			                 "  End Function\n" +
			                 "End Class");
		}
		
		const string VBIEnumeratorOfStringImplementation =
			"Imports System.Collections.Generic\n" +
			"Class Test\n" +
			"  Implements IEnumerator(Of String)\n" +
			"  Public ReadOnly Property Current() As String Implements IEnumerator(Of String).Current\n" +
			"    Get\n" +
			"    End Get\n" +
			"  End Property\n" +
			"  Private ReadOnly Property System_Collections_IEnumerator_Current() As Object Implements System.Collections.IEnumerator.Current\n" +
			"    Get\n" +
			"    End Get\n" +
			"  End Property\n" +
			"  Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext\n" +
			"  End Function\n" +
			"  Public Sub Reset() Implements System.Collections.IEnumerator.Reset\n" +
			"  End Sub\n" +
			"  Public Sub Dispose() Implements System.IDisposable.Dispose\n" +
			"  End Sub\n" +
			"End Class";
		
		[Test]
		public void InterfaceImplementation3()
		{
			TestProgramCS2VB("using System.Collections.Generic;\n" +
			                 "class Test : IEnumerator<string> {\n" +
			                 "  public string Current { get { } }\n" +
			                 "  object System.Collections.IEnumerator.Current { get { } }\n" +
			                 "  public bool MoveNext() { }\n" +
			                 "  public void Reset() { }\n" +
			                 "  public void Dispose() { }\n" +
			                 "}",
			                 VBIEnumeratorOfStringImplementation);
		}
		
		[Test]
		public void InterfaceImplementation4()
		{
			TestProgramVB2CS(VBIEnumeratorOfStringImplementation,
			                 DefaultUsingsCSharp +
			                 "using System.Collections.Generic;\n" +
			                 "class Test : IEnumerator<string>\n" +
			                 "{\n" +
			                 "  public string Current {\n" +
			                 "    get { }\n" +
			                 "  }\n" +
			                 "  private object System_Collections_IEnumerator_Current {\n" +
			                 "    get { }\n" +
			                 "  }\n" +
			                 "  object System.Collections.IEnumerator.Current {\n" +
			                 "    get { return System_Collections_IEnumerator_Current; }\n" +
			                 "  }\n" +
			                 "  public bool MoveNext()\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public void Reset()\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "  public void Dispose()\n" +
			                 "  {\n" +
			                 "  }\n" +
			                 "}");
		}
		
		[Test]
		public void InterfaceImplementation5()
		{
			TestProgramCS2VB("using System;\n" +
			                 "interface IObj { void T(object a); }\n" +
			                 "interface IStr { void T(string a); }\n" +
			                 "class Test : IObj, IStr {\n" +
			                 "  public void T(string a) { }\n" +
			                 "  public void T(int a) { }\n" +
			                 "  public void T(object a) { }\n" +
			                 "}",
			                 "Imports System\n" +
			                 "Interface IObj\n" +
			                 "  Sub T(a As Object)\n" +
			                 "End Interface\n" +
			                 "Interface IStr\n" +
			                 "  Sub T(a As String)\n" +
			                 "End Interface\n" +
			                 "Class Test\n" +
			                 "  Implements IObj\n" +
			                 "  Implements IStr\n" +
			                 "  Public Sub T(a As String) Implements IStr.T\n" +
			                 "  End Sub\n" +
			                 "  Public Sub T(a As Integer)\n" +
			                 "  End Sub\n" +
			                 "  Public Sub T(a As Object) Implements IObj.T\n" +
			                 "  End Sub\n" +
			                 "End Class");
		}
		#endregion
		
		[Test]
		public void OverloadMembersInBaseClass()
		{
			TestProgramCS2VB("class Test : System.Collections.Generic.List<int> {" +
			                 "   public void RemoveAt(string strangeIndex) {}" +
			                 "}",
			                 "Class Test\n" +
			                 "  Inherits System.Collections.Generic.List(Of Integer)\n" +
			                 "  Public Overloads Sub RemoveAt(strangeIndex As String)\n" +
			                 "  End Sub\n" +
			                 "End Class");
		}
	}
}
