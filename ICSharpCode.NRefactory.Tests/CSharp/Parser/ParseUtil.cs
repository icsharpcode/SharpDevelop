// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser
{
	/// <summary>
	/// Helper methods for parser unit tests.
	/// </summary>
	public class ParseUtilCSharp
	{
		public static T ParseGlobal<T>(string code, bool expectErrors = false) where T : INode
		{
			CSharpParser parser = new CSharpParser();
			CompilationUnit cu = parser.Parse(new StringReader(code));
			
			// TODO check for parser errors
			/*if (expectErrors)
				Assert.IsTrue(parser.Errors.ErrorOutput.Length > 0, "There were errors expected, but parser finished without errors.");
			else
				Assert.AreEqual("", parser.Errors.ErrorOutput);*/
			
			INode node = cu.Children.Single();
			Type type = typeof(T);
			Assert.IsTrue(type.IsAssignableFrom(node.GetType()), String.Format("Parsed node was {0} instead of {1} ({2})", node.GetType(), type, node));
			return (T)node;
		}
		
		public static T ParseStatement<T>(string stmt, bool expectErrors = false) where T : INode
		{
			Assert.Ignore("ParseExpression not yet implemented");
			
			CSharpParser parser = new CSharpParser();
			BlockStatement parsedBlock = parser.ParseBlock(new StringReader(stmt));
			
			// TODO check for parser errors
			/*if (expectErrors)
				Assert.IsTrue(parser.Errors.ErrorOutput.Length > 0, "There were errors expected, but parser finished without errors.");
			else
				Assert.AreEqual("", parser.Errors.ErrorOutput);*/
			
			Assert.AreEqual(1, parsedBlock.Children.Count());
			INode statement = parsedBlock.Children.First();
			Type type = typeof(T);
			Assert.IsTrue(type.IsAssignableFrom(statement.GetType()), String.Format("Parsed statement was {0} instead of {1} ({2})", statement.GetType(), type, statement));
			return (T)statement;
		}
		
		public static T ParseExpression<T>(string expr, bool expectErrors = false) where T : INode
		{
			Assert.Ignore("ParseExpression not yet implemented");
			
			CSharpParser parser = new CSharpParser();
			INode parsedExpression = parser.ParseExpression(new StringReader(expr));
			
			// TODO check for parser errors
			/*if (expectErrors)
				Assert.IsTrue(parser.Errors.ErrorOutput.Length > 0, "There were errors expected, but parser finished without errors.");
			else
				Assert.AreEqual("", parser.Errors.ErrorOutput);*/
			
			Type type = typeof(T);
			Assert.IsTrue(type.IsAssignableFrom(parsedExpression.GetType()), String.Format("Parsed expression was {0} instead of {1} ({2})", parsedExpression.GetType(), type, parsedExpression));
			return (T)parsedExpression;
		}
	}
}
