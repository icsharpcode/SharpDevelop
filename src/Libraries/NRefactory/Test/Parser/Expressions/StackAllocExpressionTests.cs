/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class StackAllocExpressionTests
	{
		#region C#
		[Test]
		public void CSharpStackAllocExpressionTest()
		{
			string program = "class A { unsafe void A() { int* fib = stackalloc int[100]; } }";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.Parse();
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			
//			Assert.IsTrue(expr is StackAllocExpression);
//			StackAllocExpression sae = (StackAllocExpression)expr;
//			
//			Assert.AreEqual("int", sae.TypeReference.Type);
//			Assert.IsTrue(sae.Expression is PrimitiveExpression);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
