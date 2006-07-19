// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class IndexerExpressionTests
	{
		#region C#
		[Test]
		public void CSharpIndexerExpressionTest()
		{
			IndexerExpression ie = ParseUtilCSharp.ParseExpression<IndexerExpression>("field[1, \"Hello\", 'a']");
			Assert.IsTrue(ie.TargetObject is IdentifierExpression);
			
			Assert.AreEqual(3, ie.Indices.Count);
			
			Assert.IsTrue(ie.Indices[0] is PrimitiveExpression);
			Assert.AreEqual(1, (int)((PrimitiveExpression)ie.Indices[0]).Value);
			Assert.IsTrue(ie.Indices[1] is PrimitiveExpression);
			Assert.AreEqual("Hello", (string)((PrimitiveExpression)ie.Indices[1]).Value);
			Assert.IsTrue(ie.Indices[2] is PrimitiveExpression);
			Assert.AreEqual('a', (char)((PrimitiveExpression)ie.Indices[2]).Value);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
