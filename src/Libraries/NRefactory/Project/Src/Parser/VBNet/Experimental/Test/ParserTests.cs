// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>


using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using VB = ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.VBNet.Experimental;
using NUnit.Framework;

namespace VBParserExperiment.Tests
{
	[TestFixture]
	public class ParserTests
	{
		[Test]
		public void SimpleGlobal()
		{
			RunTest(
				@"Option Explicit",
				@"enter Global
exit Global
"
			);
		}
		
		[Test]
		public void VariableWithXmlLiteral()
		{
			RunTest(
				@"Class Test
	Public Sub New()
		Dim x = <a>
	End Sub
End Class
",
				@"enter Global
	enter Type
		enter Member
			enter IdentifierExpected
			exit IdentifierExpected
			enter Body
				enter IdentifierExpected
				exit IdentifierExpected
				enter Xml
				exit Xml
			exit Body
		exit Member
	exit Type
exit Global
"
			);
		}
		
		[Test]
		public void MemberWithXmlLiteral()
		{
			RunTest(
				@"Class Test
	Private xml As XElement = <b>
	
	Public Sub New()
		Dim x = <a>
	End Sub
End Class
",
				@"enter Global
	enter Type
		enter Member
			enter IdentifierExpected
			exit IdentifierExpected
			enter IdentifierExpected
			exit IdentifierExpected
			enter Xml
			exit Xml
		exit Member
		enter Member
			enter IdentifierExpected
			exit IdentifierExpected
			enter Body
				enter IdentifierExpected
				exit IdentifierExpected
				enter Xml
				exit Xml
			exit Body
		exit Member
	exit Type
exit Global
"
			);
		}
		
		[Test]
		public void GlobalAttributeTest()
		{
			RunTest(
				@"<assembly: CLSCompliant(True)>
Class Test
	Public Sub New()
		Dim x = 5
	End Sub
End Class
",
								@"enter Global
	enter Attribute
	exit Attribute
	enter Type
		enter Member
			enter IdentifierExpected
			exit IdentifierExpected
			enter Body
				enter IdentifierExpected
				exit IdentifierExpected
			exit Body
		exit Member
	exit Type
exit Global
"
			);
		}
		
		[Test]
		public void ClassAttributeTest()
		{
			RunTest(
				@"<Serializable>
Class Test
	Public Sub New()
		Dim x = 5
	End Sub
End Class
",
								@"enter Global
	enter Attribute
	exit Attribute
	enter Type
		enter Member
			enter IdentifierExpected
			exit IdentifierExpected
			enter Body
				enter IdentifierExpected
				exit IdentifierExpected
			exit Body
		exit Member
	exit Type
exit Global
"
			);
		}
		
		[Test]
		public void MethodAttributeTest()
		{
			RunTest(
				@"Class Test
	<Test>
	Public Sub New()
		Dim x = 5
	End Sub
End Class
",
								@"enter Global
	enter Type
		enter Member
			enter Attribute
			exit Attribute
			enter IdentifierExpected
			exit IdentifierExpected
			enter Body
				enter IdentifierExpected
				exit IdentifierExpected
			exit Body
		exit Member
	exit Type
exit Global
"
			);
		}
		
		void RunTest(string code, string expectedOutput)
		{
			ExpressionFinder p = new ExpressionFinder();
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(code));
			Token t;
			
			do {
				t = lexer.NextToken();
				p.InformToken(t);
			} while (t.Kind != VB.Tokens.EOF);
			
			Assert.AreEqual(expectedOutput, p.Output);
		}
	}
}
