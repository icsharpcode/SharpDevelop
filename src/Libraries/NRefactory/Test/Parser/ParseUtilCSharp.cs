/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 18.09.2004
 * Time: 06:36
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	public class ParseUtilCSharp
	{
		public static object ParseGlobal(string program, Type type)
		{
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.Parse();
			Assert.IsNotNull(parser.Errors);
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
			TypeDeclaration td = (TypeDeclaration)ParseGlobal("class MyClass {" + typeMember + "}", typeof(TypeDeclaration));
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
