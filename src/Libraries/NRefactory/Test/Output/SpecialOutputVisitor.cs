// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.PrettyPrinter
{
	[TestFixture]
	public class SpecialOutputVisitorTest
	{
		void TestProgram(string program)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
			using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(),
			                                    outputVisitor)) {
				outputVisitor.Visit(parser.CompilationUnit, null);
			}
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(program.Replace("\r", ""), outputVisitor.Text.TrimEnd().Replace("\r", ""));
			parser.Dispose();
		}
		
		void TestProgramVB(string program)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(),
			                                    outputVisitor)) {
				outputVisitor.Visit(parser.CompilationUnit, null);
			}
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(program.Replace("\r", ""), outputVisitor.Text.TrimEnd().Replace("\r", ""));
			parser.Dispose();
		}
		
		[Test]
		public void BlankLine()
		{
			TestProgram("using A;\n\nusing B;");
		}
		
		[Test]
		public void BlankLineAtBeginning()
		{
			TestProgram("\nusing A;\n\nusing B;");
		}
		
		[Test]
		public void SimpleComments()
		{
			TestProgram("// before class\n" +
			            "class A\n" +
			            "{\n" +
			            "\t// in class\n" +
			            "}\n" +
			            "// after class");
		}
		
		[Test]
		public void BlockComment()
		{
			System.Diagnostics.Debugger.Break();
			TestProgram("/* before class */\n" +
			            "class A\n" +
			            "{\n" +
			            "\t/* in class */\n" +
			            "}\n" +
			            "/* after class */");
		}
		
		[Test]
		public void PreProcessing()
		{
			TestProgram("#if WITH_A\n" +
			            "class A\n" +
			            "{\n" +
			            "}\n" +
			            "#end if");
		}
		
		[Test]
		public void Enum()
		{
			TestProgram("enum Test\n" +
			            "{\n" +
			            "\t// a\n" +
			            "\tm1,\n" +
			            "\t// b\n" +
			            "\tm2\n" +
			            "\t// c\n" +
			            "}\n" +
			            "// d");
		}
		
		[Test]
		public void EnumVB()
		{
			TestProgramVB("Enum Test\n" +
			              "\t' a\n" +
			              "\tm1\n" +
			              "\t' b\n" +
			              "\tm2\n" +
			              "\t' c\n" +
			              "End Enum\n" +
			              "' d");
		}
		
		[Test]
		public void RegionInsideMethod()
		{
			TestProgram(@"public class Class1
{
	private bool test(int l, int lvw)
	{
		#region Metodos Auxiliares
		int i = 1;
		return false;
		#endregion
	}
}");
		}
		
		[Test]
		public void RegionInsideMethodVB()
		{
			TestProgramVB(@"Public Class Class1
	Private Function test(ByVal l As Integer, ByVal lvw As Integer) As Boolean
		' Begin
		Dim i As Integer = 1
		Return False
		' End of method
	End Function
End Class");
		}
	}
}
