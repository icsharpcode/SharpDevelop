// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class VBNetOutputTest
	{
		void TestProgram(string program)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(program));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			outputVisitor.Visit(parser.CompilationUnit, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(StripWhitespace(program), StripWhitespace(outputVisitor.Text));
		}
		
		string StripWhitespace(string text)
		{
			return text.Trim().Replace("\t", "").Replace("\r", "").Replace("\n", " ").Replace("  ", " ");
		}
		
		void TestTypeMember(string program)
		{
			TestProgram("Class A\n" + program + "\nEnd Class");
		}
		
		void TestStatement(string statement)
		{
			TestTypeMember("Sub Method()\n" + statement + "\nEnd Sub");
		}
		
		void TestExpression(string expression)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(expression));
			Expression e = parser.ParseExpression();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			VBNetOutputVisitor outputVisitor = new VBNetOutputVisitor();
			e.AcceptVisitor(outputVisitor, null);
			Assert.AreEqual("", outputVisitor.Errors.ErrorOutput);
			Assert.AreEqual(StripWhitespace(expression), StripWhitespace(outputVisitor.Text));
		}
		
		[Test]
		public void Field()
		{
			TestTypeMember("Private a As Integer");
		}
		
		[Test]
		public void Method()
		{
			TestTypeMember("Sub Method()\nEnd Sub");
		}
		
		[Test]
		public void PartialModifier()
		{
			TestProgram("Public Partial Class Foo\nEnd Class");
		}
		
		[Test]
		public void GenericClassDefinition()
		{
			TestProgram("Public Class Foo(Of T As {IDisposable, ICloneable})\nEnd Class");
		}
		
		[Test]
		public void GenericClassDefinitionWithBaseType()
		{
			TestProgram("Public Class Foo(Of T As IDisposable)\nInherits BaseType\nEnd Class");
		}
		
		[Test]
		public void GenericMethodDefinition()
		{
			TestTypeMember("Public Sub Foo(Of T As {IDisposable, ICloneable})(ByVal arg As T)\nEnd Sub");
		}
		
		[Test]
		public void ArrayRank()
		{
			TestStatement("Dim a As Object(,,)");
		}
		
		[Test]
		public void Assignment()
		{
			TestExpression("a = b");
		}
		
		[Test]
		public void Integer()
		{
			TestExpression("12");
		}
		
		[Test]
		public void GenericMethodInvocation()
		{
			TestExpression("GenericMethod(Of T)(arg)");
		}
		
		[Test]
		public void SpecialIdentifierName()
		{
			TestExpression("[Class]");
		}
		
		[Test]
		public void GenericDelegate()
		{
			TestProgram("Public Delegate Function Predicate(Of T)(ByVal item As T) As String");
		}
		
		[Test]
		public void Enum()
		{
			TestProgram("Enum MyTest\nRed\n Green\n Blue\nYellow\n End Enum");
		}
		
		[Test]
		public void EnumWithInitializers()
		{
			TestProgram("Enum MyTest\nRed = 1\n Green = 2\n Blue = 4\n Yellow = 8\n End Enum");
		}
		
		[Test]
		public void SyncLock()
		{
			TestStatement("SyncLock a\nWork()\nEnd SyncLock");
		}
		
		[Test]
		public void Using()
		{
			TestStatement("Using a As A = New A()\na.Work()\nEnd Using");
		}
	}
}
