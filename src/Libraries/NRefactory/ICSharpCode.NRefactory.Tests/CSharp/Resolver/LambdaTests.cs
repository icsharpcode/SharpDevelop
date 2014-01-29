// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class LambdaTests : ResolverTestBase
	{
		[Test]
		public void SimpleLambdaTest()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Test(i => Console.WriteLine(i));
	}
	public void Test(Action<int> ac) { ac(42); }
}";
			var lrr = Resolve<LocalResolveResult>(program.Replace("(i)", "($i$)"));
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
			
			lrr = Resolve<LocalResolveResult>(program.Replace("i =>", "$i$ =>"));
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInConstructorTest()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		TestClass t = new TestClass(i => Console.WriteLine($i$));
	}
	public TestClass(Action<int> ac) { ac(42); }
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInGenericConstructorTest()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		var t = new SomeClass<string>(i => Console.WriteLine($i$));
	}
}
class SomeClass<T> {
	public SomeClass(Action<T> ac) { }
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.String", lrr.Type.ReflectionName);
		}
		
		#region Lambda In Array Initializer
		[Test]
		public void LambdaInArrayInitializer1()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Converter<int, string>[] arr = {
			i => $i$.ToString()
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInArrayInitializer2()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		a = new Converter<int, string>[] {
			i => $i$.ToString()
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInArrayInitializer3()
		{
			string program = @"using System;
class TestClass {
	Converter<int, string>[] field = new Converter<int, string>[] {
		i => $i$.ToString()
	};
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInArrayInitializer4()
		{
			string program = @"using System;
class TestClass {
	Converter<int, string>[] field = {
		i => $i$.ToString()
	};
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaIn2DArrayInitializer()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Converter<int, string>[,] arr = {
			{ i => $i$.ToString() }
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInInferred2DArrayInitializer()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		var c = new [,] { { null, (Converter<int, string>)null }, { a => $a$.ToString(), b => b.ToString() }};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		#endregion
		
		#region Lambda In Collection Initializer
		[Test]
		public void LambdaInCollectionInitializer1()
		{
			string program = @"using System; using System.Collections.Generic;
class TestClass {
	static void Main() {
		a = new List<Converter<int, string>> {
			i => $i$.ToString()
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInCollectionInitializer2()
		{
			string program = @"using System; using System.Collections.Generic;
class TestClass {
	static void Main() {
		a = new Dictionary<Func<char, string>, Converter<int, string>> {
			{ i => $i$.ToString(), i => i.ToString() }
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Char", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInCollectionInitializer3()
		{
			string program = @"using System; using System.Collections.Generic;
class TestClass {
	static void Main() {
		a = new Dictionary<Func<char, string>, Converter<int, string>> {
			{ i => i.ToString(), $i$ => i.ToString() }
		};
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		#endregion
		
		[Test]
		public void LambdaInObjectInitializerTest()
		{
			string program = @"using System;
class X {
	void SomeMethod() {
		Helper h = new Helper {
			F = i => $i$.ToString()
		};
	}
}
class Helper {
	public Converter<int, string> F;
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInCastExpression()
		{
			string program = @"using System;
static class TestClass {
	static void Main(string[] args) {
		var f = (Func<int, string>) ( i => $i$ );
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInDelegateCreateExpression()
		{
			string program = @"using System;
static class TestClass {
	static void Main(string[] args) {
		var f = new Func<int, string>( i => $i$ );
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInReturnStatement()
		{
			string program = @"using System;
static class TestClass {
	static Converter<int, string> GetToString() {
		return i => $i$.ToString();
	}
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInReturnStatementInStatementLambda()
		{
			string program = @"using System;
static class TestClass {
	static void SomeMethod() {
		Func<Func<string, string>> getStringTransformer = () => {
			return s => $s$.ToUpper();
		};
	}
	public delegate R Func<T, R>(T arg);
	public delegate R Func<R>();
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.String", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInReturnStatementInAnonymousMethod()
		{
			string program = @"using System;
static class TestClass {
	static void SomeMethod() {
		Func<Func<string, string>> getStringTransformer = delegate {
			return s => $s$.ToUpper();
		};
	}
	public delegate R Func<T, R>(T arg);
	public delegate R Func<R>();
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.String", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void CurriedLambdaExpressionInCastExpression()
		{
			string program = @"using System;
static class TestClass {
	static void Main(string[] args) {
		var f = (Func<char, Func<string, int>>) ( a => b => 0 );
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program.Replace("a =>", "$a$ =>"));
			Assert.AreEqual("System.Char", lrr.Type.ReflectionName);
			
			lrr = Resolve<LocalResolveResult>(program.Replace("b =>", "$b$ =>"));
			Assert.AreEqual("System.String", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInVariableInitializer()
		{
			string program = @"using System;
static class TestClass {
	static void Main() {
		Func<int, string> f = $i$ => i.ToString();
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaExpressionInVariableAssignment()
		{
			string program = @"using System;
static class TestClass {
	static void Main() {
		Func<int, string> f;
 		f = $i$ => i.ToString();
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInDelegateCall()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Func<Func<int, string>, char> f;
		f($i$ => i.ToString());
	}
	public delegate R Func<T, R>(T arg);
}";
			var lrr = Resolve<LocalResolveResult>(program);
			Assert.AreEqual("System.Int32", lrr.Type.ReflectionName);
		}
		
		[Test]
		public void ConvertAllInGenericMethod()
		{
			string program = @"using System;
class TestClass {
	static void Method<T>(System.Collections.Generic.List<T> list) {
		$list.ConvertAll(x => (int)(object)x)$;
	}
}";
			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			var m = (IMethod)rr.Member;
			Assert.AreEqual("System.Int32", m.TypeArguments[0].ReflectionName);
			Assert.AreEqual("System.Converter`2[[``0],[System.Int32]]", m.Parameters[0].Type.ReflectionName);
			
			var crr = (ConversionResolveResult)rr.Arguments[0];
			Assert.IsTrue(crr.Conversion.IsAnonymousFunctionConversion);
			Assert.AreEqual("System.Converter`2[[``0],[System.Int32]]", crr.Type.ReflectionName);
		}
		
		[Test]
		public void AnonymousMethodWithoutParameterList()
		{
			string program = @"using System;
class TestClass {
	event EventHandler Ev = $delegate {}$;
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsFalse(rr.HasParameterList);
		}
		
		[Test]
		public void NonVoidMethodInActionLambdaIsValidConversion()
		{
			string program = @"using System;
class TestClass {
	void Run(Action a) { }
	int M() {
		Run(() => $M()$);
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void NonVoidMethodInImplicitlyTypedActionLambdaIsValidConversion()
		{
			string program = @"using System;
class TestClass {
	void Run(Action<string> a) { }
	int M() {
		Run(x => $M()$);
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void ImplicitLambdaInNewFunc()
		{
			string program = @"using System;
class Test {
	static bool b;
	object x = new Func<int, string>(a => $a$.ToString());
}";
			var r = Resolve(program);
			Assert.AreEqual("System.Int32", r.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaInNewAction()
		{
			string program = @"using System;
class Test {
	static bool b;
	object x = new Action(() => $b = true$);
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void AnonymousMethodInNewEventHandler()
		{
			// The switch statement causes the control flow analysis to ask the resolver if it's a constant,
			// which caused a bug.
			string program = @"using System;
class Test {
	static bool b;
	object x = new EventHandler<AssemblyLoadEventArgs>($delegate (object sender, AssemblyLoadEventArgs e) { switch (e.Action) {} }$);
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void ThrowingAnonymousMethodIsConvertibleToFunc()
		{
			string program = @"using System;
class Test {
	Func<string, int> x = $delegate { throw new NotImplementedException(); }$;
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void EmptyAnonymousMethodIsNotConvertibleToFunc()
		{
			string program = @"using System;
class Test {
	Func<string, int> x = $delegate { }$;
}";
			var c = GetConversion(program);
			Assert.IsFalse(c.IsValid);
		}
		
		[Test]
		public void RaisePropertyChanged_WithExpressionLambda()
		{
			string program = @"using System;
using System.Linq.Expressions;
class Test {
	void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression) {}
	void RaisePropertyChanged(string propertyName) {}
	string MyProperty { get {} }
	void Test() {
		$RaisePropertyChanged(() => MyProperty)$;
	}
}";
			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual("propertyExpression", rr.Member.Parameters.Single().Name);
		}
		
		[Test]
		public void ParenthesizedExpressionIsNotValidExpressionStatement()
		{
			string program = @"using System;
class A {
    static void Foo(string x, Action<Action> y) { Console.WriteLine(1); }
    static void Foo(object x, Func<Func<int>, int> y) { Console.WriteLine(2); }

    static void Main()
    { ";
			var rr = ResolveAtLocation<CSharpInvocationResolveResult>(program + "$Foo(null, x => x()); // Prints 1\n}}");
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual("System.String", rr.Member.Parameters[0].Type.ReflectionName);
			
			rr = ResolveAtLocation<CSharpInvocationResolveResult>(program + "$Foo(null, x => (x())); // Prints 2\n}}");
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual("System.Object", rr.Member.Parameters[0].Type.ReflectionName);
		}
		
		[Test]
		public void LambdaWithComparisonToString()
		{
			string program = @"using System;
class Test {
    static void Foo(Func<int, bool> f) {}
    static void Foo(Func<string, bool> f) {}
    static void Main() { $Foo(x => x == ""text"")$; } }";
			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			var invoke = rr.Member.Parameters.Single().Type.GetDelegateInvokeMethod();
			Assert.AreEqual("System.String", invoke.Parameters.Single().Type.ReflectionName);
		}
		
		[Test]
		public void LambdaWithComparisonToInt()
		{
			string program = @"using System;
class Test {
    static void Foo(Func<int, bool> f) {}
    static void Foo(Func<string, bool> f) {}
    static void Main() { $Foo(x => x == 42)$; } }";
			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			var invoke = rr.Member.Parameters.Single().Type.GetDelegateInvokeMethod();
			Assert.AreEqual("System.Int32", invoke.Parameters.Single().Type.ReflectionName);
		}
		
		[Test]
		public void StartNewTask()
		{
			string program = @"using System;
class Test {
	int Calculate() {}
    static void Main() {
    	$System.Threading.Tasks.Task.Factory.StartNew(() => Calculate())$;
	}}";
			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual("System.Threading.Tasks.Task`1[[System.Int32]]", rr.Type.ReflectionName);
		}
		
		[Test]
		public void LambdaParameterIdentity()
		{
			string code = @"using System;
class TestClass {
	void F() {
		Func<int, int> f = $i => i + 1$;
	}
}";
			
			var prep = PrepareResolver(code);
			var lambda = (LambdaExpression)prep.Item2;
			var identifierInLambdaBody = ((BinaryOperatorExpression)lambda.Body).Left;
			var resolver = prep.Item1;
			
			var resolvedParameter = ((LocalResolveResult)resolver.Resolve(lambda.Parameters.Single())).Variable;
			var parameterInResolveResult = ((LambdaResolveResult)resolver.Resolve(lambda)).Parameters[0];
			var referencedParameter = ((LocalResolveResult)resolver.Resolve(identifierInLambdaBody)).Variable;
			
			Assert.AreEqual("System.Int32" ,resolvedParameter.Type.ReflectionName);
			Assert.AreSame(resolvedParameter, parameterInResolveResult);
			Assert.AreSame(resolvedParameter, referencedParameter);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda()
		{
			string program = @"class MainClass {
	void Main() {
		$M(x=>x.ToUpper())$;
	}
	delegate R Func<T, R>(T arg);
	int M(Func<int, int> f){ }
	string M(Func<string, string> f){ }
}";
			var mrr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(mrr.IsError);
			Assert.AreEqual("System.String", mrr.Type.ReflectionName);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda2()
		{
			string program = @"class MainClass {
	void Main() {
		$M(x=>x.Length)$;
	}
	delegate R Func<T, R>(T arg);
	int M(Func<int, int> f){ }
	string M(Func<string, int> f){ }
}";
			var mrr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(mrr.IsError);
			Assert.AreEqual("System.String", mrr.Type.ReflectionName);
		}
		
		[Test]
		public void MultipleOverloadsWithImplicitLambda3()
		{
			string program = @"class MainClass {
	void Main() {
		$M(x=>x+x)$;
	}
	delegate R Func<T, R>(T arg);
	string M(Func<string, int> f){ }
	int M(Func<int, int> f){ }
}";
			var mrr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(mrr.IsError);
			Assert.AreEqual("System.Int32", mrr.Type.ReflectionName);
		}

		[Test]
		public void AsyncLambdaWithAwait()
		{
			string program = @"
using System;
using System.Threading.Tasks;

class A
{
	public Task OpenAsync ()
	{
		return null;
	}
}

class C
{
	async void Foo ()
	{
			await $Test (async () => { await new A().OpenAsync (); })$;
	}
	
	T Test<T> (Func<T> func)
	{
		return default (T);
	}
}
";
			var mrr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.IsFalse(mrr.IsError);
			Assert.AreEqual("System.Threading.Tasks.Task", mrr.Type.ReflectionName);
		}

		[Test]
		public void ConversionInExplicitlyTypedLambdaBody() {
			string program = @"using System;
class Test {
	public object M() {
		System.Func<int, string> f = $(int i) => null$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			Assert.That(((ConversionResolveResult)rr.Body).Conversion.IsNullLiteralConversion);
			Assert.AreEqual("System.String", rr.ReturnType.ReflectionName);
			Assert.AreEqual("System.String", rr.Body.Type.ReflectionName);
		}

		[Test]
		public void ConversionInImplicitlyTypedLambdaBody() {
			string program = @"using System;
class Test {
	public object M() {
		System.Func<int, string> f = $i => null$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			Assert.That(((ConversionResolveResult)rr.Body).Conversion.IsNullLiteralConversion);
			Assert.AreEqual("System.String", rr.ReturnType.ReflectionName);
			Assert.AreEqual("System.String", rr.Body.Type.ReflectionName);
		}

		[Test]
		public void NoConversionInVoidExplicitlyTypedLambdaBody() {
			string program = @"using System;
class Test {
	public object M() {
		System.Action<int> f = $(int i) => i++$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<OperatorResolveResult>(rr.Body);
			Assert.AreEqual("System.Void", rr.ReturnType.ReflectionName);
			Assert.AreEqual("System.Int32", rr.Body.Type.ReflectionName);
		}

		[Test]
		public void NoConversionInVoidImplicitlyTypedLambdaBody() {
			string program = @"using System;
class Test {
	public object M() {
		System.Action<int> f = $i => i++$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<OperatorResolveResult>(rr.Body);
			Assert.AreEqual("System.Void", rr.ReturnType.ReflectionName);
			Assert.AreEqual("System.Int32", rr.Body.Type.ReflectionName);
		}
		
		[Test]
		public void NumericConversion()
		{
			string program = @"using System;
class Test {
	public void M() {
		Func<int, double> f = $i => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			var bodyConv = ((ConversionResolveResult)rr.Body).Conversion;
			Assert.IsTrue(bodyConv.IsValid);
			Assert.IsTrue(bodyConv.IsNumericConversion);
			
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}
		
		[Test]
		public void InvalidNumericConversion()
		{
			string program = @"using System;
class Test {
	public void M() {
		Func<double, int> f = $i => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			var bodyConv = ((ConversionResolveResult)rr.Body).Conversion;
			Assert.IsFalse(bodyConv.IsValid);
			
			var c = GetConversion(program);
			Assert.IsFalse(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}
		
		[Test]
		public void ImplicitAsyncLambda()
		{
			string program = @"using System;
using System.Threading.Tasks;
class Test {
	public void M() {
		Func<int, Task<int>> f = $async i => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<OperatorResolveResult>(rr.Body);
			Assert.AreEqual("System.Int32", rr.Body.Type.FullName);
			
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}
		
		[Test]
		public void ImplicitAsyncLambdaWithNumericConversion()
		{
			string program = @"using System;
using System.Threading.Tasks;
class Test {
	public void M() {
		Func<int, Task<double>> f = $async i => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			Assert.AreEqual("System.Double", rr.Body.Type.FullName);
			
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}
		
		[Test]
		public void ExplicitAsyncLambda()
		{
			string program = @"using System;
using System.Threading.Tasks;
class Test {
	public void M() {
		Func<int, Task<int>> f = $async (int i) => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<OperatorResolveResult>(rr.Body);
			Assert.AreEqual("System.Int32", rr.Body.Type.FullName);
			
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}
		
		[Test]
		public void ExplicitAsyncLambdaWithNumericConversion()
		{
			string program = @"using System;
using System.Threading.Tasks;
class Test {
	public void M() {
		Func<int, Task<double>> f = $async (int i) => i + 1$;
	}
}";
			var rr = Resolve<LambdaResolveResult>(program);
			Assert.IsInstanceOf<ConversionResolveResult>(rr.Body);
			Assert.AreEqual("System.Double", rr.Body.Type.FullName);
			
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsAnonymousFunctionConversion);
		}

		[Test]
		public void AnonymousMethodConversionObjectToDynamic() {
			string program = @"using System;
class Test {
	public void M() {
		Action<dynamic> x = $delegate(object z) { z = null; }$;
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}

		[Test]
		public void AnonymousMethodConversionObjectToDynamicGenericArgument() {
			string program = @"using System;
using System.Collections.Generic;
class Test {
	public void M() {
		Action<List<dynamic>> x = $delegate(List<object> z) { z = null; }$;
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}

		[Test]
		public void AnonymousMethodConversionDynamicToObject() {
			string program = @"using System;
class Test {
	public void M() {
		Action<object> x = $delegate(dynamic z) { z = null; }$;
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}

		[Test]
		public void AnonymousMethodConversionDynamicToObjectGenericArgument() {
			string program = @"using System;
using System.Collections.Generic;
class Test {
	public void M() {
		Action<List<object>> x = $delegate(List<dynamic> z) { z = null; }$;
	}
}";
			var c = GetConversion(program);
			Assert.IsTrue(c.IsValid);
		}
		
		[Test]
		public void InvalidConversionToVoid_ImplicitStatementLambda()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Action a = () => {
			return $1$;
		};
	}
}";
			Assert.AreEqual(TypeKind.Void, GetExpectedType(program).Kind);
			Assert.IsFalse(GetConversion(program).IsValid);
		}
		
		[Test]
		public void InvalidConversionToVoid_AnonymousMethod()
		{
			string program = @"using System;
class TestClass {
	static void Main() {
		Action a = delegate {
			return $1$;
		};
	}
}";
			Assert.AreEqual(TypeKind.Void, GetExpectedType(program).Kind);
			Assert.IsFalse(GetConversion(program).IsValid);
		}
		
		[Test]
		public void VoidLamda_Uses_Identity_Conversion()
		{
			string program = @"using System;
class TestClass {
	static int GetInt() { return 4; }
	static void Main() {
		Action a = () => $GetInt()$;
	}
}";
			Assert.AreEqual("System.Int32", GetExpectedType(program).FullName);
			Assert.IsTrue(GetConversion(program).IsValid);
		}
	}
}
