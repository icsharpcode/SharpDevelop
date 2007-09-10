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
		((CastTo)castTarget).MethodOnCastExpression(par.a, par.b);
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
			if (expectedContext != null) {
				Assert.AreEqual(expectedContext.ToString(), er.Context.ToString());
			}
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
			FindFull(document, "mple += 1", "simple", ExpressionContext.MethodBody);
		}
		
		[Test]
		public void SimpleBeginningOfExpression()
		{
			FindFull(document, "simple += 1", "simple", ExpressionContext.MethodBody);
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
		public void MethodOnCast()
		{
			FindFull(document, "thodOnCastExpression(pa", "((CastTo)castTarget).MethodOnCastExpression(par.a, par.b)", null);
		}
		
		[Test]
		public void PropertyOnCast()
		{
			FindFull(document, "pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", null);
		}
		
		[Test]
		public void PropertyOnCastInForeachLoop()
		{
			FindFull(program2, "pertyOnCastExpression", "((CastTo)castTarget).PropertyOnCastExpression", null);
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
	void Method(global::System.Exception ex, int arg) {
		if (arg < foo1) {
			
		} else if (boolVar) {
			
		} else if (arg > foo2) {
			
		}
		List<string> a = new
	}
	Main() : this() {}
	Main(int arg) : base(arg + 3) {
		
	}
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
			FindFull(program3, "base(", "base(arg + 3)", ExpressionContext.BaseConstructorCall);
		}
		
		[Test]
		public void UsingStatement()
		{
			FindExpr(program3, "using", null, ExpressionContext.Global);
		}
		
		[Test]
		public void ConditionInIfStatement()
		{
			FindFull(program3, "oolVar)", "boolVar", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierFollowedByLessThan()
		{
			FindFull(program3, "rg < foo", "arg", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierAfterLessThan()
		{
			FindFull(program3, "oo1)", "foo1", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierFollowedByGreaterThan()
		{
			FindFull(program3, "rg > foo", "arg", ExpressionContext.Default);
		}
		
		[Test]
		public void IdentifierAfterGreaterThan()
		{
			FindFull(program3, "oo2)", "foo2", ExpressionContext.Default);
		}
		
		
		[Test]
		public void NestedClass()
		{
			const string nestedClassProgram = @"using System; using System.Collections.Generic;
class Main {
	/* in main */
	class Nested {
		/* in nested */
	}
	/* still in main */
	enum NestedEnum {
		/* in enum */
	}
}";
			
			FindExpr(nestedClassProgram, "\t/* in main", null, ExpressionContext.TypeDeclaration);
			FindExpr(nestedClassProgram, "\t/* in nested ", null, ExpressionContext.TypeDeclaration);
			FindExpr(nestedClassProgram, "\t/* still in main", null, ExpressionContext.TypeDeclaration);
			FindExpr(nestedClassProgram, "\t/* in enum", null, ExpressionContext.IdentifierExpected);
		}
		
		[Test]
		public void PropertyClass()
		{
			const string propertyProgram = @"using System; using System.Collections.Generic;
class Main {
	public int Prop {
		/* in prop */
		get {
			/* in getter */
		}
		set {
			/* in setter */
		}
		/* still in prop */
	}
}";
			
			FindExpr(propertyProgram, "\t/* in prop", null, ExpressionContext.PropertyDeclaration);
			FindExpr(propertyProgram, "\t/* in getter ", null, ExpressionContext.MethodBody);
			FindExpr(propertyProgram, "\t/* in setter", null, ExpressionContext.MethodBody);
			FindExpr(propertyProgram, "\t/* still in prop", null, ExpressionContext.PropertyDeclaration);
		}
		
		[Test]
		public void FindObjectCreationContextForNewKeywordCompletion()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		StringBuilder b = new";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.ObjectCreation.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void FindObjectCreationContextForConstructorInsight()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		StringBuilder b = new StringBuilder";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("StringBuilder", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void FindObjectCreationContextForConstructorInsight2()
		{
			const string program = @"using System; using System.Text;
class Main {
	StringBuilder field = new StringBuilder";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("StringBuilder", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void FindObjectCreationContextForConstructorInsight3()
		{
			const string program = @"using System;
class Main {
	void M() {
		System.Text.StringBuilder b = new System.Text.StringBuilder";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("System.Text.StringBuilder", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void ExpressionContextInFieldInitializer()
		{
			const string program = @"using System; using System.Collections.Generic;
class Main {
	int field1 =  1;
	int field2 =  StaticMethod(  2);
	int field3 = MakeArray()[  3];
}";
			FindExpr(program, " 1", null, ExpressionContext.Default);
			FindExpr(program, " 2", null, ExpressionContext.Default);
			FindFull(program, "taticMethod", "StaticMethod(  2)", ExpressionContext.Default);
			FindExpr(program, " 3", null, ExpressionContext.Default);
		}
		
		[Test]
		public void ExpressionContextInConstructorDeclaration()
		{
			const string program = @"using System; using System.Text;
class Main {
	public Main() : base(arg) {
		body;
} }";
			
			FindFull(program, "base", "base(arg)", ExpressionContext.BaseConstructorCall);
			FindFull(program, "body", "body", ExpressionContext.MethodBody);
			FindFull(program, "arg", "arg", ExpressionContext.Default);
		}
		
		[Test]
		public void FieldInGenericClass()
		{
			const string program = @"using System;
class MyList<T> where T : IComparable<T> {
  List<T> ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("List<T> ", result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void MultipleFields()
		{
			const string program = @"using System;
class MyClass {
  List<T> field1, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void MultipleFieldsWithInitializers()
		{
			const string program = @"using System;
class MyClass {
  int field1 = 1, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void FieldWithNullableType()
		{
			const string program = @"using System;
class MyClass {
  int? ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void FieldWithArrayType()
		{
			const string program = @"using System;
class MyClass {
  int[,][] ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration()
		{
			const string program = @"using System;
class List<";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration2()
		{
			const string program = @"using System;
class Dictionary<K, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration3()
		{
			const string program = @"using System;
class List<T> ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("List<T> ", result.Expression);
			Assert.AreEqual(ExpressionContext.ConstraintsStart.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration4()
		{
			const string program = @"using System;
class List<T> where ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("where ", result.Expression);
			Assert.AreEqual(ExpressionContext.Constraints.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration5()
		{
			const string program = @"using System;
class List<T> where T : ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.Constraints.ToString(), result.Context.ToString());
		}
		
		[Test]
		public void GenericClassDeclaration6()
		{
			const string program = @"using System;
class List<T> where T : class, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.Constraints.ToString(), result.Context.ToString());
		}
	}
}

