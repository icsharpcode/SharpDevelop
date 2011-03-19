// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class VBExpressionFinderTests
	{
		const string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
		
		VBNetExpressionFinder ef;
		
		[SetUp]
		public void Init()
		{
			HostCallback.GetCurrentProjectContent = delegate {
				return ParserService.CurrentProjectContent;
			};
			
			ef = new VBNetExpressionFinder(null);
		}
		
		void FindFull(string program, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf("|");
			if (pos < 0) Assert.Fail("location not found in program");
			program = program.Remove(pos, 1);
			ExpressionResult er = ef.FindFullExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
		}
		
		void Find(string program, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf("|");
			if (pos < 0) Assert.Fail("location not found in program");
			program = program.Remove(pos, 1);
			ExpressionResult er = ef.FindExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
		}
		
		#region Find
		[Test]
		public void FindSimple()
		{
			string program2 = @"
Class MainClass
	Sub A
		Con|sole.WriteLine(""Hello World!"")
	End Sub
End Class
		";
			
			Find(program2, "Con", ExpressionContext.MethodBody);
		}
		
		[Test]
		public void FindSimple2()
		{
			string program2 = @"
Class MainClass
	Sub A
		Console.|WriteLine(""Hello World!"")
	End Sub
End Class
		";
			
			Find(program2, "Console.", ExpressionContext.Default);
		}
		
		[Test]
		public void FindSimple3()
		{
			string program3 = @"
Class MainClass
	Sub A
		Console.WriteLine|
	End Sub
End Class
		";
			
			Find(program3, "Console.WriteLine", ExpressionContext.Default);
		}
		
		[Test]
		public void FindAfterBrace()
		{
			string program2 = @"
Class MainClass
	Sub A
		Console.WriteLine(|""Hello World!"")
	End Sub
End Class
		";
			
			Find(program2, "", ExpressionContext.Default);
		}
		
		[Test]
		public void ForEachLoop()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loop|VarName In collection
		Next
	End Sub
End Class
";
			
			Find(program1, "loop", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void FindEmptyAfterImports()
		{
			string program1 = @"
Imports System
Imports System.Linq
	|
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			Find(program1, "", ExpressionContext.Global);
		}
		
		[Test]
		public void FindParameterStart()
		{
			Find(@"Module Program
	Private Function CreateFolder(|
End Module", "", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void FindAfterNewLineImport()
		{
			Find("Imports System\n|", "", ExpressionContext.Global);
		}
		
		[Test]
		public void FindInArgumentList()
		{
			Find(@"Class MainClass
	Sub Main()
		Test(Te|st2(1) + Test2(2))
	End Sub
End Class", "Te", ExpressionContext.Default);
		}
		
		[Test]
		public void FindExpressionBeforeBrace()
		{
			Find(@"Class MainClass
	Sub Main()
		Test(Test2|(1) + Test2(2))
	End Sub
End Class", "Test2", ExpressionContext.Default);
		}
		
		[Test]
		public void FindExpressionAfterThen()
		{
			Find(@"Class MainClass
	Sub Main()
		If True Then Double|
	End Sub
End Class", "Double", ExpressionContext.MethodBody);
		}
		
		[Test]
		public void FindExpressionAfterWordBegin()
		{
			Find(@"Class MainClass
	Dim test As Integer
	Sub Main()
		If Me.test i| Then
		
		End If
	End Sub
End Class", "i", ExpressionContext.Default);
		}
		#endregion
		
		#region Context Tests
		void ContextTest(string program, ExpressionContext context)
		{
			int pos = program.IndexOf("|");
			if (pos < 0) Assert.Fail("location not found in program");
			program = program.Remove(pos, 1);
			ExpressionResult er = ef.FindExpression(program, pos);
			Assert.AreEqual(context.ToString(), er.Context.ToString());
		}
		
		[Test]
		public void ContextAfterDimIdentifierSpace()
		{
			string program4 = @"
Class MainClass
	Sub A
		Dim a |
	End Sub
End Class
		";
			
			ContextTest(program4, ExpressionContext.MethodBody);
		}
		
		[Test]
		public void ContextAfterDimIdentifierAs()
		{
			string prg = @"Module Test
	Sub Test()
		Dim x As |
	End Sub
End Module";
			ContextTest(prg, ExpressionContext.Type);
		}
		
		[Test]
		public void ContextAfterDim()
		{
			string program4 = @"
Class MainClass
	Sub A
		Dim |
	End Sub
End Class
		";
			ContextTest(program4, ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void ContextInModule()
		{
			string prg = @"Module Test
	|
End Module";
			
			ContextTest(prg, ExpressionContext.TypeDeclaration);
		}
		#endregion
		
		#region FindFull
		[Test]
		public void Simple()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		si|mple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			FindFull(program1, "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleBeginningOfExpression()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		|simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			FindFull(program1, "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void Underscore()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim un|der_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "under_score_field", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierBeforeKeyword()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopV|arName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "loopVarName", ExpressionContext.Default);
		}
		
		[Test]
		public void LocalVariableDecl()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim t|ext = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "text", ExpressionContext.Default);
		}
		
		[Test]
		public void Imports1()
		{
			string program1 = @"
Imports S|ystem
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "System", ExpressionContext.Importable);
		}
		
		[Test]
		public void Imports2()
		{
			string program1 = @"
Imports System
Imports System.L|inq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			FindFull(program1, "System.Linq", ExpressionContext.Importable);
		}
		
		[Test]
		public void ClassName()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class M|ainClass ' a comment
	Dim under_score_field As Integer
	Sub SomeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "MainClass", ExpressionContext.Default);
		}
		
		[Test]
		public void SubName()
		{
			string program1 = @"
Imports System
Imports System.Linq
		
Class MainClass ' a comment
	Dim under_score_field As Integer
	Sub S|omeMethod()
		simple += 1
		Dim text = ""Text""
		For Each loopVarName In collection
		Next
	End Sub
End Class
";
			
			FindFull(program1, "SomeMethod", ExpressionContext.Default);
		}
		
		[Test]
		public void ParameterName()
		{
			FindFull(@"Module Test
	Function Fibo(|x As Integer) As Integer
	
	End Function
End Module", "x", ExpressionContext.Default);
		}
		
		[Test]
		public void TypeKeywordMember()
		{
			FindFull(@"Module Test
	Sub Main()
		String.For|mat(""{0}"", ""Test"")
	End Sub
End Module", "String.Format(\"{0}\", \"Test\")", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleXml()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim xml = <!-- te|st -->
		
		Dim x = 5
	End Sub
End Module", "<!-- test -->", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleXml2()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim xml = <!-- test -->
		
		Dim xml2 = <![CDATA[some| text]]>
	End Sub
End Module", "<![CDATA[some text]]>", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleXml3()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim xml = <!-- test -->
		
		Dim x = |5
	End Sub
End Module", "5", ExpressionContext.Default);
		}
		
		[Test]
		public void Linq1()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kv|p As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "kvp", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValueP|air(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "KeyValuePair(Of String, DataGridViewCellStyle)", ExpressionContext.Type);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCac|he.CellStyleCache _
				Select includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "styleCache", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellSty|leCache _
				Select includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "styleCache.CellStyleCache", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select includ|eStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "includeStyle(kvp.Key, kvp.Value)", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select includeStyle(kv|p.Key, kvp.Value)
	End Sub
End Module", "kvp", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select includeStyle(kvp.Key, kvp.Val|ue)
	End Sub
End Module", "kvp.Value", ExpressionContext.Default);
		}
		
		[Test]
		public void Linq2()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select da|ta As DataGridViewCellStyle = includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "data", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Dim x = From kvp As KeyValuePair(Of String, DataGridViewCellStyle) _
				In styleCache.CellStyleCache _
				Select data As DataG|ridViewCellStyle = includeStyle(kvp.Key, kvp.Value)
	End Sub
End Module", "DataGridViewCellStyle", ExpressionContext.Type);
		}
		
		[Test]
		public void Using1()
		{
			FindFull(@"Module Test
	Sub Main()
		Using |x As FileReader = New FileReader()
		
		End Using
	End Sub
End Module", "x", ExpressionContext.Default);
			
			FindFull(@"Module Test
	Sub Main()
		Using x As FileR|eader = New FileReader()
		
		End Using
	End Sub
End Module", "FileReader", ExpressionContext.Type);
			
			FindFull(@"Module Test
	Sub Main()
		Using x As New FileR|eader()
		
		End Using
	End Sub
End Module", "FileReader()", ExpressionContext.ObjectCreation);
			
			FindFull(@"Module Test
	Sub Main()
		Using FileRea|der()
		
		End Using
	End Sub
End Module", "FileReader()", ExpressionContext.Default);
		}
		
		[Test]
		public void FunctionLambda()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim f = Fun|ction(x, y) x + y
	End Sub
End Module", "Function(x, y) x + y", ExpressionContext.Default);
		}
		
		[Test]
		public void SubLambda()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim f = Su|b(x, y) Console.WriteLine(x + y)
	End Sub
End Module", "Sub(x, y) Console.WriteLine(x + y)", ExpressionContext.Default);
		}
		
		[Test]
		public void NewExpression()
		{
			FindFull(@"Module Test
	Sub Main()
		Dim list = N|ew List(Of Integer)
	End Sub
End Module", "New List(Of Integer)", ExpressionContext.Default);
		}
		
		#region Old Tests
		void OldTest(string expr, int offset)
		{
			string body = @"Class Test
	Sub A
		{0}.AnotherField
	End Sub
End Class";
			Assert.AreEqual(expr, ef.FindFullExpression(string.Format(body, expr), @"Class Test
	Sub A
		".Length + offset).Expression);
		}
		
		void OldTestFind(string expr, string expected, int offset)
		{
			string body = @"Class Test
	Sub A
		Dim x = abc + {0}
	End Sub
End Class";
			Assert.AreEqual(expected, ef.FindExpression(string.Format(body, expr), @"Class Test
	Sub A
		Dim x = abc + ".Length + offset).Expression);
		}
		
		[Test]
		public void FieldReference()
		{
			OldTest("abc", 1);
			OldTest("abc.def", 6);
		}
		
		[Test]
		public void WithFieldReference()
		{
			OldTest(".abc", 2);
			OldTest(".abc.def", 7);
		}
		
		[Test]
		public void MethodCall()
		{
			OldTest("abc.Method().Method()", 16);
		}
		
		[Test]
		public void ComplexMethodCall()
		{
			OldTest("abc.Method().Method(5, a.b, 5 + a)", 16);
		}
		
		[Test]
		public void PlusExpression()
		{
			OldTestFind("def", "de", 2);
		}
		
		[Test]
		public void PlusExpression2()
		{
			OldTestFind("def", "", 0);
		}
		#endregion
		#endregion
	}
}
