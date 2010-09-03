// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Grunwald.BooBinding.CodeCompletion;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace Grunwald.BooBinding.Tests
{
	[TestFixture]
	public class ExpressionFinderTests
	{
		const string code = "class A:\n\tpublic simple = 1\n// comment\ndef main():\n\tpass";
		ExpressionFinder ef = new ExpressionFinder();
		
		void FindFull(string program, string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in program");
			ExpressionResult er = ef.FindFullExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			if (expectedContext != null) {
				Assert.AreEqual(expectedContext, er.Context);
			}
		}
		
		[Test]
		public void Simple()
		{
			FindFull(code, "mple = 1", "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleBeginningOfExpression()
		{
			FindFull(code, "simple = 1", "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void NoMatchForComment1()
		{
			FindFull(code, "// comment", null, null);
		}
		
		[Test]
		public void NoMatchForComment2()
		{
			FindFull(code, "/ comment", null, null);
		}
		
		[Test]
		public void NoMatchForComment3()
		{
			FindFull(code, " comment", null, null);
		}
		
		[Test]
		public void NoMatchForComment4()
		{
			FindFull(code, "comment", null, null);
		}
	}
}
