// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;

using MbUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	public class ParseUtilCSharp
	{
		public static object ParseGlobal(string program, Type type)
		{
			return ParseGlobal(program, type, false);
		}
		
		public static object ParseGlobal(string program, Type type, bool expectError)
		{
			return ParseGlobal(program, type, expectError, false);
		}
		
		public static object ParseGlobal(string program, Type type, bool expectError, bool skipMethodBodies)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.ParseMethodBodies = !skipMethodBodies;
			parser.Parse();
			Assert.IsNotNull(parser.Errors);
			if (expectError)
				Assert.IsTrue(parser.Errors.ErrorOutput.Length > 0, "There were errors expected, but parser finished without errors.");
			else
				Assert.AreEqual("", parser.Errors.ErrorOutput);
			Assert.IsNotNull(parser.CompilationUnit);
			Assert.IsNotNull(parser.CompilationUnit.Children);
			Assert.IsNotNull(parser.CompilationUnit.Children[0]);
			Assert.IsTrue(parser.CompilationUnit.Children.Count > 0);
			Assert.IsTrue(type.IsAssignableFrom(parser.CompilationUnit.Children[0].GetType()), String.Format("Parsed expression was {0} instead of {1} ({2})", parser.CompilationUnit.Children[0].GetType(), type, parser.CompilationUnit.Children[0]));
			return parser.CompilationUnit.Children[0];
		}
		
		public static object ParseTypeMember(string typeMember, Type type)
		{
			return ParseTypeMember(typeMember, type, false);
		}
		
		public static object ParseTypeMember(string typeMember, Type type, bool expectError)
		{
			TypeDeclaration td = (TypeDeclaration)ParseGlobal("class MyClass {" + typeMember + "}", typeof(TypeDeclaration), expectError);
			Assert.IsTrue(td.Children.Count > 0);
			Assert.IsTrue(type.IsAssignableFrom(td.Children[0].GetType()), String.Format("Parsed expression was {0} instead of {1} ({2})", td.GetType(), type, td));
			return td.Children[0];
		}
		
		public static object ParseStatment(string statement, Type type)
		{
			MethodDeclaration md = (MethodDeclaration)ParseTypeMember("void A() { " + statement + " }", typeof(MethodDeclaration));
			Assert.IsTrue(md.Body.Children.Count > 0);
			Assert.IsTrue(type.IsAssignableFrom(md.Body.Children[0].GetType()), String.Format("Parsed expression was {0} instead of {1} ({2})", md.GetType(), type, md));
			return md.Body.Children[0];
		}
		
		public static object ParseExpression(string expr, Type type)
		{
			return ParseExpression(expr, type, false);
		}
		
		public static object ParseExpression(string expr, Type type, bool expectErrors)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(expr + ";"));
			object parsedExpression = parser.ParseExpression();
			if (expectErrors)
				Assert.IsTrue(parser.Errors.ErrorOutput.Length > 0, "There were errors expected, but parser finished without errors.");
			else
				Assert.AreEqual("", parser.Errors.ErrorOutput);
			Assert.IsTrue(type.IsAssignableFrom(parsedExpression.GetType()), String.Format("Parsed expression was {0} instead of {1} ({2})", parsedExpression.GetType(), type, parsedExpression));
			return parsedExpression;
		}
	}
}
