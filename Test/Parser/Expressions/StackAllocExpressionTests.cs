// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class StackAllocExpressionTests
	{
		#region C#
		[Test]
		public void CSharpStackAllocExpressionTest()
		{
			string program = "class A { unsafe void A() { int* fib = stackalloc int[100]; } }";
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(program));
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
