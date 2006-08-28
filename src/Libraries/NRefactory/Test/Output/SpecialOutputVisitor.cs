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
using ICSharpCode.NRefactory.Ast;
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
				outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
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
				outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			}
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(program.Replace("\r", ""), outputVisitor.Text.TrimEnd().Replace("\r", ""));
			parser.Dispose();
		}
		
		void TestProgramCS2VB(string programCS, string programVB)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(programCS));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(),
			                                    outputVisitor)) {
				outputVisitor.VisitCompilationUnit(parser.CompilationUnit, null);
			}
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(programVB.Replace("\r", ""), outputVisitor.Text.TrimEnd().Replace("\r", ""));
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
			TestProgram("/* before class */\n" +
			            "class A\n" +
			            "{\n" +
			            "\t/* in class */\n" +
			            "}\n" +
			            "/* after class */");
		}
		
		[Test]
		public void ComplexCommentMix()
		{
			TestProgram("/* before class */\n" +
			            "// line comment before\n" +
			            "/* block comment before */\n" +
			            "class A\n" +
			            "{\n" +
			            "\t/* in class */\n" +
			            "\t// in class 2" +
			            "\t/* in class 3 */\n" +
			            "}\n" +
			            "/* after class */\n" +
			            "// after class 2\n" +
			            "/* after class 3*/");
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
		
		[Test]
		public void BlankLinesVB()
		{
			TestProgramVB("Imports System\n" +
			              "\n" +
			              "Imports System.IO");
			TestProgramVB("Imports System\n" +
			              "\n" +
			              "\n" +
			              "Imports System.IO");
			TestProgramVB("\n" +
			              "' Some comment\n" +
			              "\n" +
			              "Imports System.IO");
		}
		
		[Test]
		public void CommentAfterAttribute()
		{
			TestProgramCS2VB("class A { [PreserveSig] public void B(// comment\nint c) {} }",
			                 "Class A\n" +
			                 "\t\t' comment\n" +
			                 "\t<PreserveSig()> _\n" +
			                 "\tPublic Sub B(ByVal c As Integer)\n" +
			                 "\tEnd Sub\n" +
			                 "End Class");
		}
	}
}
