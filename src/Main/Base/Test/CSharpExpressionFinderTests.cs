// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class CSharpExpressionFinderTests
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
		
		const string program2 = @"using System;
class Main {
	string under_score_field;
	void Method() {
		foreach (TypeName varName in ((CastTo)castTarget).PropertyOnCastExpression) {
			
		}
		throw new NotFoundException();
	}
}";
		
		CSharpExpressionFinder ef;
		
		[SetUp]
		public void Init()
		{
			HostCallback.GetParseInformation = ParserService.GetParseInformation;
			HostCallback.GetCurrentProjectContent = delegate {
				return ParserService.CurrentProjectContent;
			};
			
			ef = new CSharpExpressionFinder("test.cs");
		}
		
		void FindFull(string location, string expectedExpression, ExpressionContext expectedContext)
		{
			FindFull(document, location, expectedExpression, expectedContext);
		}
		
		void FindFull(string program, string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in program");
			ExpressionResult er = ef.FindFullExpression(program, pos);
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
		public void MethodOnCast()
		{
			FindFull("thodOnCastExpression(para", "((CastTo)castTarget).MethodOnCastExpression(parameter)", ExpressionContext.Default);
		}
		
		[Test]
		public void PropertyOnCast()
		{
			FindFull("pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", ExpressionContext.Default);
		}
		
		[Test]
		public void PropertyOnCastInForeachLoop()
		{
			FindFull(program2, "pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", ExpressionContext.Default);
		}
		
		[Test]
		public void Underscore()
		{
			FindFull(program2, "der_score_field", "under_score_field", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierBeforeKeyword()
		{
			FindFull(program2, "arName", "varName", ExpressionContext.Default);
		}
		
		[Test]
		public void NewException()
		{
			FindFull(program2, "otFoundException", "NotFoundException()", ExpressionContext.TypeDerivingFrom(ParserService.DefaultProjectContentRegistry.Mscorlib.SystemTypes.Exception, true));
		}
	}
}
