// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using CSharpBinding.Parser;
using ICSharpCode.SharpDevelop.Dom;

namespace CSharpBinding.Tests
{
	[TestFixture]
	public class ExpressionFinderTests
	{
		const string document = @"using System;
class Main<T> : BaseType
{
	public Color Color { get {} set {}}
	Font Font { get {} set {}}
	void Method() {
		simple += 1;
		int a = 0;
		((CastTo)castTarget).MethodOnCastExpression(parameter);
		int b = 0;
		return ((CastTo)castTarget).PropertyOnCastExpression;
	}
}";
		
		ExpressionFinder ef;
		
		[SetUp]
		public void Init()
		{
			ef = new ExpressionFinder("test.cs");
		}
		
		void FindFull(string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = document.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in document");
			ExpressionResult er = ef.FindFullExpression(document, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
		}
		
		[Test]
		public void Simple()
		{
			FindFull("mple += 1", "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void SimpleBeginningOfExpression()
		{
			FindFull("simple += 1", "simple", ExpressionContext.Default);
		}
		
		[Test]
		public void PropertyColor()
		{
			FindFull("olor { get", "Color", ExpressionContext.Default);
		}
		
		[Test]
		public void TypeColor()
		{
			FindFull("olor Color", "Color", ExpressionContext.Type);
		}
		
		[Test]
		public void PropertyFont()
		{
			FindFull("ont { get", "Font", ExpressionContext.Default);
		}
		
		[Test]
		public void TypeFont()
		{
			FindFull("ont Font", "Font", ExpressionContext.Type);
		}
		
		[Test]
		public void Method()
		{
			FindFull("thodOnCastExpression(para", "((CastTo)castTarget).MethodOnCastExpression(parameter)", ExpressionContext.Default);
		}
		
		[Test]
		public void Property()
		{
			FindFull("pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", ExpressionContext.Default);
		}
	}
}
