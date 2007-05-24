// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.Output
{
	[TestFixture]
	public class SnippetConversion
	{
		void CS2VB(string input, string expectedOutput)
		{
			SnippetParser parser = new SnippetParser(SupportedLanguage.CSharp);
			INode node = parser.Parse(input);
			// parser.Errors.ErrorOutput contains syntax errors, if any
			Assert.IsNotNull(node);
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			// parser.Specials is the list of comments, preprocessor directives etc.
			PreprocessingDirective.CSharpToVB(parser.Specials);
			// Convert C# constructs to VB.NET:
			node.AcceptVisitor(new CSharpConstructsVisitor(), null);
			node.AcceptVisitor(new ToVBNetConvertVisitor(), null);

			VBNetOutputVisitor output = new VBNetOutputVisitor();
			using (SpecialNodesInserter.Install(parser.Specials, output)) {
				node.AcceptVisitor(output, null);
			}
			// output.Errors.ErrorOutput contains conversion errors/warnings, if any
			// output.Text contains the converted code
			Assert.AreEqual("", output.Errors.ErrorOutput);
			Assert.AreEqual(expectedOutput, output.Text);
		}
		
		[Test]
		public void CompilationUnitCS2VB()
		{
			CS2VB(
				@"using System;

public class MyClass
{
   string abc;

   public string Abc { get { return abc; } }

    // This is a test method
    static void M<T>(params T[] args) where T : IDisposable
    {
       Console.WriteLine(""Hello!"");
    }
}",

				@"Imports System

Public Class [MyClass]
	Private m_abc As String

	Public ReadOnly Property Abc() As String
		Get
			Return m_abc
		End Get
	End Property

	' This is a test method
	Private Shared Sub M(Of T As IDisposable)(ParamArray args As T())
		Console.WriteLine(""Hello!"")
	End Sub
End Class
"
			);
		}
		
		
		
		
		[Test]
		public void TypeMembersCS2VB()
		{
			CS2VB(
				"void Test() {}\n" +
				"void Test2() {}",

				@"Private Sub Test()
End Sub
Private Sub Test2()
End Sub
"
			);
		}
		
		[Test]
		public void StatementsCS2VB()
		{
			CS2VB(
				"int a = 3;\n" +
				"a++;",

				@"Dim a As Integer = 3
a += 1
"
			);
		}
	}
}
