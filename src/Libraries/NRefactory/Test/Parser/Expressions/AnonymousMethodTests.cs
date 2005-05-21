/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 21.05.2005
 * Time: 21:33
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
	public class AnonymousMethodTests
	{
		AnonymousMethodExpression Parse(string program)
		{
			return (AnonymousMethodExpression)ParseUtilCSharp.ParseExpression(program, typeof(AnonymousMethodExpression));
		}
		
		[Test]
		public void EmptyAnonymousMethod()
		{
			AnonymousMethodExpression ame = Parse("delegate() {}");
			Assert.AreEqual(0, ame.Parameters.Count);
			Assert.AreEqual(0, ame.Body.Children.Count);
		}
		
		[Test]
		public void SimpleAnonymousMethod()
		{
			AnonymousMethodExpression ame = Parse("delegate(int a, int b) { return a + b; }");
			Assert.AreEqual(2, ame.Parameters.Count);
			// blocks can't be added without compilation unit -> anonymous method body
			// is always empty when using ParseExpression
			//Assert.AreEqual(1, ame.Body.Children.Count);
			//Assert.IsTrue(ame.Body.Children[0] is ReturnStatement);
		}
	}
}
