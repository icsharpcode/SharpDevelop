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
			HostCallback.GetCurrentProjectContent = delegate {
				return ParserService.CurrentProjectContent;
			};
			
			ef = new CSharpExpressionFinder(null);
		}
		
		void FindFull(string program, string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in program");
			ExpressionResult er = ef.FindFullExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
		}
		
		void FindExpr(string program, string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in program");
			ExpressionResult er = ef.FindExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
		}
		
		[Test]
		public void Simple()
		{
			FindFull(document, "mple += 1", "simple", ExpressionContext.StatementStart);
		}
		
		[Test]
		public void SimpleBeginningOfExpression()
		{
			FindFull(document, "simple += 1", "simple", ExpressionContext.StatementStart);
		}
		
		[Test]
		public void PropertyColor()
		{
			FindFull(document, "olor { get", "Color", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void TypeColor()
		{
			FindFull(document, "olor Color", "Color", ExpressionContext.Type);
		}
		
		[Test]
		public void PropertyFont()
		{
			FindFull(document, "ont { get", "Font", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void TypeFont()
		{
			FindFull(document, "ont Font", "Font", ExpressionContext.Type);
		}
		
		[Test]
		[Ignore("Context inside methods not yet implemented")]
		public void MethodOnCast()
		{
			FindFull(document, "thodOnCastExpression(para", "((CastTo)castTarget).MethodOnCastExpression(parameter)", ExpressionContext.Default);
		}
		
		[Test]
		[Ignore("Context inside methods not yet implemented")]
		public void PropertyOnCast()
		{
			FindFull(document, "pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", ExpressionContext.Default);
		}
		
		[Test]
		[Ignore("Context inside methods not yet implemented")]
		public void PropertyOnCastInForeachLoop()
		{
			FindFull(program2, "pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", ExpressionContext.Default);
		}
		
		[Test]
		public void Underscore()
		{
			FindFull(program2, "der_score_field", "under_score_field", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void IdentifierBeforeKeyword()
		{
			FindFull(program2, "arName", "varName", ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void NewException()
		{
			FindFull(program2, "otFoundException", "NotFoundException()", ExpressionContext.TypeDerivingFrom(ParserService.DefaultProjectContentRegistry.Mscorlib.SystemTypes.Exception, true));
		}
		
		[Test]
		public void RemoveLastPart()
		{
			CSharpExpressionFinder f = new CSharpExpressionFinder(null);
			Assert.AreEqual("arr", f.RemoveLastPart("arr[i]"));
			Assert.AreEqual("obj", f.RemoveLastPart("obj.Field"));
			Assert.AreEqual("obj.Method", f.RemoveLastPart("obj.Method(args, ...)"));
			Assert.AreEqual("obj.Method", f.RemoveLastPart("obj.Method(complex[1].SubExpression)"));
		}
		
		
		const string program3 = @"using System; using System.Collections.Generic;
class Main {
	void Method(global::System.Exception ex) {
		List<string> a = new
	}
	Main() : this() {}
	Main(int a) : base(a + 3) {}
}";
		
		[Test]
		public void GlobalNamespace()
		{
			// context = context after the found word
			FindFull(program3, "global", "global", ExpressionContext.FirstParameterType);
			FindFull(program3, "System.Ex", "global::System", ExpressionContext.IdentifierExpected);
			FindFull(program3, "Excep", "global::System.Exception", ExpressionContext.Type);
		}
		
		[Test]
		public void GenericType()
		{
			FindFull(program3, "List<str", "List<string>", ExpressionContext.Type);
		}
		
		[Test]
		public void TypeInTypeArguments()
		{
			FindFull(program3, "string>", "string", ExpressionContext.Type);
			FindFull(program3, "tring>", "string", ExpressionContext.Type);
		}
		
		[Test]
		public void ConstructorCall()
		{
			FindFull(program3, "this(", "this()", ExpressionContext.BaseConstructorCall);
			FindFull(program3, "base(", "base(a + 3)", ExpressionContext.BaseConstructorCall);
		}
		
		[Test]
		public void UsingStatement()
		{
			FindExpr(program3, "using", null, ExpressionContext.Global);
		}
	}
}
