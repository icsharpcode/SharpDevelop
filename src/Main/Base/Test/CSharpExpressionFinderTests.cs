// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				Assert.AreEqual(expectedContext, er.Context);
			}
			Assert.AreEqual(expectedExpression, ExtractRegion(program, er.Region));
		}
		
		void FindExpr(string program, string location, string expectedExpression, ExpressionContext expectedContext)
		{
			int pos = program.IndexOf(location);
			if (pos < 0) Assert.Fail("location not found in program");
			ExpressionResult er = ef.FindExpression(program, pos);
			Assert.AreEqual(expectedExpression, er.Expression);
			Assert.AreEqual(expectedContext, er.Context);
			Assert.AreEqual(expectedExpression, ExtractRegion(program, er.Region));
		}
		
		static string ExtractRegion(string text, DomRegion region)
		{
			if (region.IsEmpty)
				return null;
			int start = GetOffsetByPosition(text, region.BeginLine, region.BeginColumn);
			int end = GetOffsetByPosition(text, region.EndLine, region.EndColumn);
			return text.Substring(start, end - start);
		}
		
		static int GetOffsetByPosition(string text, int line, int column)
		{
			if (line < 1)
				throw new ArgumentOutOfRangeException("line");
			if (line == 1)
				return column - 1;
			for (int i = 0; i < text.Length; i++) {
				if (text[i] == '\n') {
					if (--line == 1) {
						return i + column;
					}
				}
			}
			throw new ArgumentOutOfRangeException("line");
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
			FindExpr(program2, "oundException", "NotF", ExpressionContext.TypeDerivingFrom(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib.SystemTypes.Exception, true));
			FindFull(program2, "oundException", "new NotFoundException()", ExpressionContext.TypeDerivingFrom(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib.SystemTypes.Exception, true));
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
			FindFull(program3, "global", "global", CSharpExpressionContext.FirstParameterType);
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
			FindFull(program3, "this(", "this()", CSharpExpressionContext.BaseConstructorCall);
			FindFull(program3, "base(", "base(arg + 3)", CSharpExpressionContext.BaseConstructorCall);
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
			
			FindExpr(propertyProgram, "\t/* in prop", null, CSharpExpressionContext.PropertyDeclaration);
			FindExpr(propertyProgram, "\t/* in getter ", null, ExpressionContext.MethodBody);
			FindExpr(propertyProgram, "\t/* in setter", null, ExpressionContext.MethodBody);
			FindExpr(propertyProgram, "\t/* still in prop", null, CSharpExpressionContext.PropertyDeclaration);
		}
		
		[Test]
		public void FindObjectCreationContextForNewKeywordCompletion()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		StringBuilder b = new";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
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
			Assert.AreEqual("StringBuilder", ExtractRegion(program, result.Region));
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
		}
		
		[Test]
		public void FindObjectCreationContextForConstructorInsight2()
		{
			const string program = @"using System; using System.Text;
class Main {
	StringBuilder field = new StringBuilder";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("StringBuilder", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
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
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
		}
		
		[Test]
		public void FindObjectCreationContextForConstructorInsight4()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		StringBuilder b = new StringBuilderBla";
			
			ExpressionResult result = ef.FindExpression(program, program.Length - 3);
			Assert.AreEqual("StringBuilder", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
		}
		
		[Test]
		public void FindDefaultContextAfterConstructorCall()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		StringBuilder b = new StringBuilder()";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("new StringBuilder()", result.Expression);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void FindDefaultContextAfterConstructorCall2()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		new StringBuilder().Property.Method";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("new StringBuilder().Property.Method", result.Expression);
		}
		
		[Test]
		public void FindFullExpressionAfterConstructorCall()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		new StringBuilder().Property.MethodCall()";
			
			ExpressionResult result = ef.FindFullExpression(program, program.IndexOf("ringBuilder"));
			Assert.AreEqual("new StringBuilder()", result.Expression);
			Assert.AreEqual(ExpressionContext.ObjectCreation, result.Context);
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
			
			FindFull(program, "base", "base(arg)", CSharpExpressionContext.BaseConstructorCall);
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
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void MultipleFields()
		{
			const string program = @"using System;
class MyClass {
  List<T> field1, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void MultipleFieldsWithInitializers()
		{
			const string program = @"using System;
class MyClass {
  int field1 = 1, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void FieldWithNullableType()
		{
			const string program = @"using System;
class MyClass {
  int? ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void FieldWithArrayType()
		{
			const string program = @"using System;
class MyClass {
  int[,][] ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void MultipleLocalVariables()
		{
			const string program = @"using System;
class MyClass {
  void M() {
    int a, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void MultipleLocalVariablesWithInitializers()
		{
			const string program = @"using System;
class MyClass {
  void M() {
    int a = 1, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration()
		{
			const string program = @"using System;
class List<";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration2()
		{
			const string program = @"using System;
class Dictionary<K, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration3()
		{
			const string program = @"using System;
class List<T> ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("List<T> ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ConstraintsStart, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration4()
		{
			const string program = @"using System;
class List<T> where ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("where ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.Constraints, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration5()
		{
			const string program = @"using System;
class List<T> where T : ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(CSharpExpressionContext.Constraints, result.Context);
		}
		
		[Test]
		public void GenericClassDeclaration6()
		{
			const string program = @"using System;
class List<T> where T : class, ";
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(CSharpExpressionContext.Constraints, result.Context);
		}
		
		[Test]
		public void ObjectInitializer1()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer1b()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType<TypeArgument[], int?> { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer1c()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new global::MyNamespace.MyType { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer2()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType(){ ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer2b()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType<TypeArgument[], int?> (){ ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer3()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType(arg1, ')', arg3) { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer4()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType { P1 = expr, ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer5()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType { P1 = ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void ObjectInitializer5b()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new MyType { P1 = someBoolean == ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void ObjectInitializer6()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer7()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new { a.B, ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer8()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new SomeType { SomeProperty = { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ObjectInitializer8b()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new SomeType { SomeProperty = new SomeOtherType { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ObjectInitializer, result.Context);
		}
		
		[Test]
		public void ArrayInitializer()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new [] { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.MethodBody, result.Context);
		}
		
		[Test]
		public void ArrayInitializer2()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new SomeType [] { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.MethodBody, result.Context);
		}
		
		[Test]
		public void FieldArrayInitializer()
		{
			const string program = @"using System;
class Main {
	int[] myField = { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void GlobalAttribute1()
		{
			const string program = @"using System;
[";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Attribute, result.Context);
		}
		
		[Test]
		public void GlobalAttribute2()
		{
			const string program = @"using System;
[TestFixture]
public class X { }";
			
			FindFull(program, "stFix", "TestFixture", ExpressionContext.Attribute);
		}
		
		[Test]
		public void Indexer1()
		{
			const string program = @"using System;
class Main {
	public int this[";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.ParameterType, result.Context);
		}
		
		[Test]
		public void Indexer2()
		{
			const string program = @"using System;
class Main {
	public int this[int index] { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(CSharpExpressionContext.PropertyDeclaration, result.Context);
		}
		
		[Test]
		public void IndexerAccessInObjectInitializer()
		{
			const string program = @"using System;
class Main {
	void M() {
		a = new SomeObject { b = c[ ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void ContextInCondition()
		{
			const string program = @"using System;
class Main {
	void M() {
		if ( ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void ContextInCall()
		{
			const string program = @"using System;
class Main {
	void M() {
		MethodCall( ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void BlockLambdaInCondition()
		{
			const string program = @"using System;
class Main {
	void M() {
		if (Method(a => { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.MethodBody, result.Context);
		}
		
		[Test]
		public void BlockLambdaInField()
		{
			const string program = @"using System;
class Main {
	Func<int, int> f = a => { ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(ExpressionContext.MethodBody, result.Context);
		}
		
		[Test]
		public void StringLiteral()
		{
			const string program = @"using System;
class Main {
	string a = ""hello, world!""";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("\"hello, world!\"", result.Expression);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void InsideStringLiteral()
		{
			const string program = @"using System;
class Main {
	string a = ""hello, ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.Default, result.Context);
		}
		
		[Test]
		public void IsOperatorTest()
		{
			const string program = @"using System;
class Main {
	void M() {
		if (x is ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.Type, result.Context);
		}
		
		[Test]
		public void TypeOfTest()
		{
			const string program = @"using System;
class Main {
	void M() {
		Type t = typeof(";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.Type, result.Context);
		}
		
		[Test]
		public void AsOperatorTest()
		{
			const string program = @"using System;
class Main {
	void M() {
		X x = a as ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.Type, result.Context);
		}
		
		[Test]
		public void DelegateTest1()
		{
			const string program = @"using System;
delegate ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("delegate ", result.Expression);
			Assert.AreEqual(ExpressionContext.Type, result.Context);
		}
		
		
		[Test]
		public void DelegateTest2()
		{
			const string program = @"using System;
delegate void ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("void ", result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest1()
		{
			const string program = @"using System;
delegate void Test<";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest2()
		{
			const string program = @"using System;
delegate void Test<T>(";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ParameterType, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest3()
		{
			const string program = @"using System;
delegate void Test<T>(ref ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ParameterType, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest4()
		{
			const string program = @"using System;
delegate void Test<T>(ref T ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("T ", result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest5()
		{
			const string program = @"using System;
delegate void Test<T>(ref T name) ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("Test<T>(ref T name) ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ConstraintsStart, result.Context);
		}
		
		[Test]
		public void GenericDelegateTest6()
		{
			const string program = @"using System;
delegate void Test<T>(ref T name) where ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("where ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.Constraints, result.Context);
		}
		
		[Test]
		public void GenericMethodTest1()
		{
			const string program = @"using System; class T {
void Test<";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericMethodTest2()
		{
			const string program = @"using System; class T {
void Test<T>(";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(CSharpExpressionContext.FirstParameterType, result.Context);
		}
		
		[Test]
		public void GenericMethodTest3()
		{
			const string program = @"using System; class T {
void Test<T>(ref ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.IsNull(result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ParameterType, result.Context);
		}
		
		[Test]
		public void GenericMethodTest4()
		{
			const string program = @"using System; class T {
void Test<T>(ref T ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("T ", result.Expression);
			Assert.AreEqual(ExpressionContext.IdentifierExpected, result.Context);
		}
		
		[Test]
		public void GenericMethodTest5()
		{
			const string program = @"using System; class T {
void Test<T>(ref T name) ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("Test<T>(ref T name) ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.ConstraintsStart, result.Context);
		}
		
		[Test]
		public void GenericMethodTest6()
		{
			const string program = @"using System; class T {
void Test<T>(ref T name) where ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("where ", result.Expression);
			Assert.AreEqual(CSharpExpressionContext.Constraints, result.Context);
		}
		
		[Test]
		public void FindFullExpressionAfterCastAfterForLoop()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		for (;;) ((TargetType)variable).MethodCall()
}}";
			
			ExpressionResult result = ef.FindFullExpression(program, program.IndexOf("Call"));
			Assert.AreEqual(" ((TargetType)variable).MethodCall()", result.Expression);
		}
		
		[Test]
		public void FindFullExpressionAfterCastAfterCondition()
		{
			const string program = @"using System; using System.Text;
class Main {
	void M() {
		if (true) ((TargetType)variable).MethodCall()
}}";
			
			ExpressionResult result = ef.FindFullExpression(program, program.IndexOf("Call"));
			Assert.AreEqual(" ((TargetType)variable).MethodCall()", result.Expression);
		}
		
		[Test]
		public void DontCrashOnLoneCarriageReturn()
		{
			// the following program was causing an ExpressionFinder crash due to the lone \r
			string program = "\t\t}\r\t\t\r\n\t\tstatic void SetCaretPosition()\r\n\t\t{\r\n\t\t\tTextLocation newLocation = textArea.Document.GetLocation(newOffset);\r\n}\r\n\t}\r\n}\r\n";
			for (int i = 0; i < program.Length; i++) {
				ef.FindFullExpression(program, i);
			}
		}
		
		[Test]
		public void UsingNamespaceContext1()
		{
			const string program = @"using ";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual(null, result.Expression);
			Assert.AreEqual(ExpressionContext.Namespace, result.Context);
		}
		
		[Test]
		public void UsingNamespaceContext2()
		{
			const string program = @"using System";
			
			ExpressionResult result = ef.FindExpression(program, program.Length);
			Assert.AreEqual("System", result.Expression);
			Assert.AreEqual(ExpressionContext.Namespace, result.Context);
		}
		
		[Test]
		public void SD2_1469()
		{
			// Test that this doesn't crash
			const string program = @"class MainWindow
{
	'
	void MainWindowDeleteEvent()
	{
	}
}";
			ef.FindFullExpression(program, 25);
		}
	}
}
