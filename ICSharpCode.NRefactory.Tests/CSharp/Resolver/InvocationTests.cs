// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	[TestFixture]
	public class InvocationTests : ResolverTestBase
	{
		// TODO: do we want to return the MemberResolveResult for the InvocationExpression, or only for it's target?
		
		[Test]
		public void MethodCallTest()
		{
			string program = @"class A {
	void Method() {
		$TargetMethod()$;
	}
	
	int TargetMethod() {
		return 3;
	}
}
";
			InvocationResolveResult result = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("A.TargetMethod", result.Member.FullName);
			Assert.AreEqual("System.Int32", result.Type.ReflectionName);
		}
		
		[Test]
		public void InvalidMethodCall()
		{
			string program = @"class A {
	void Method(string b) {
		$b.ThisMethodDoesNotExistOnString(b)$;
	}
}
";
			UnknownMethodResolveResult result = Resolve<UnknownMethodResolveResult>(program);
			Assert.AreEqual("ThisMethodDoesNotExistOnString", result.MemberName);
			Assert.AreEqual("System.String", result.TargetType.FullName);
			Assert.AreEqual(1, result.Parameters.Count);
			Assert.AreEqual("b", result.Parameters[0].Name);
			Assert.AreEqual("System.String", result.Parameters[0].Type.Resolve(context).ReflectionName);
			
			Assert.AreSame(SharedTypes.UnknownType, result.Type);
		}
		
		[Test, Ignore("Resolver returns the member from the base class, which is correct according to C# spec, but not what we want to show in tooltips")]
		public void OverriddenMethodCall()
		{
			string program = @"class A {
	void Method() {
		$new B().GetRandomNumber()$;
	}
	
	public abstract int GetRandomNumber();
}
class B : A {
	public override int GetRandomNumber() {
		return 4; // chosen by fair dice roll.
		          // guaranteed to be random
	}
}
";
			InvocationResolveResult result = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("B.GetRandomNumber", result.Member.FullName);
		}
		
		[Test, Ignore("Resolver returns the member from the base class, which is correct according to C# spec, but not what we want to show in tooltips")]
		public void OverriddenMethodCall2()
		{
			string program = @"class A {
	void Method() {
		$new B().GetRandomNumber(""x"", this)$;
	}
	
	public abstract int GetRandomNumber(string a, A b);
}
class B : A {
	public override int GetRandomNumber(string b, A a) {
		return 4; // chosen by fair dice roll.
		          // guaranteed to be random
	}
}
";
			InvocationResolveResult result = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("B.GetRandomNumber", result.Member.FullName);
		}
		
		[Test]
		public void ThisMethodCallTest()
		{
			string program = @"class A {
	void Method() {
		$this.TargetMethod()$;
	}
	
	int TargetMethod() {
		return 3;
	}
}
";
			InvocationResolveResult result = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("A.TargetMethod", result.Member.FullName);
			Assert.AreEqual("System.Int32", result.Type.ReflectionName);
		}
		
		[Test]
		public void VoidTest()
		{
			string program = @"using System;
class A {
	void TestMethod() {
		$TestMethod()$;
	}
}
";
			Assert.AreEqual("System.Void", Resolve(program).Type.ReflectionName);
		}
		
		[Test]
		public void EventCallTest()
		{
			string program = @"using System;
class A {
	void Method() {
		$TestEvent(this, EventArgs.Empty)$;
	}
	
	public event EventHandler TestEvent;
}
";
			Assert.AreEqual("System.Void", Resolve(program).Type.ReflectionName);
		}
		
		[Test]
		public void DelegateCallTest()
		{
			string program = @"using System; using System.Reflection;
class A {
	void Method(ModuleResolveEventHandler eh) {
		$eh(this, new ResolveEventArgs())$;
	}
}
";
			Assert.AreEqual("System.Reflection.Module", Resolve(program).Type.ReflectionName);
		}
		
		[Test]
		public void DelegateReturnedFromMethodCallTest()
		{
			string program = @"using System;
class A {
	void Method() {
		$GetHandler()(abc)$;
	}
	abstract Predicate<string> GetHandler();
}
";
			Assert.AreEqual("System.Boolean", Resolve(program).Type.ReflectionName);
		}
		
		/* TODO
		[Test]
		public void MethodGroupResolveTest()
		{
			string program = @"class A {
	void Method() {
		
	}
	
	void TargetMethod(int a) { }
	void TargetMethod<T>(T a) { }
}
";
			MethodGroupResolveResult result = Resolve<MethodGroupResolveResult>(program, "TargetMethod", 3);
			Assert.AreEqual("TargetMethod", result.Name);
			Assert.AreEqual(2, result.Methods.Count);
			
			result = Resolve<MethodGroupResolveResult>(program, "TargetMethod<string>", 3);
			Assert.AreEqual("TargetMethod", result.Name);
			Assert.AreEqual(1, result.Methods[0].Count);
			Assert.AreEqual("System.String", result.GetMethodIfSingleOverload().Parameters[0].ReturnType.FullyQualifiedName);
		}
		 */
		
		[Test]
		public void TestOverloadingByRef()
		{
			string program = @"using System;
class Program {
	public static void Main() {
		int a = 42;
		T(a);
		T(ref a);
	}
	static void T(int x) {}
	static void T(ref int y) {}
}";
			
			InvocationResolveResult mrr = Resolve<InvocationResolveResult>(program, "T(a)");
			Assert.IsFalse(mrr.Member.Parameters[0].IsRef);
			
			mrr = Resolve<InvocationResolveResult>(program, "T(ref a)");
			Assert.IsTrue(mrr.Member.Parameters[0].IsRef);
		}
		
		[Test, Ignore("Grouping by declaring type not yet implemented")]
		public void AddedOverload()
		{
			string program = @"class BaseClass {
	static void Main(DerivedClass d) {
		$d.Test(3)$;
	}
	public void Test(int a) { }
}
class DerivedClass : BaseClass {
	public void Test(object a) { }
}";
			InvocationResolveResult mrr = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("DerivedClass.Test", mrr.Member.FullName);
		}
		
		[Test]
		public void AddedNonApplicableOverload()
		{
			string program = @"class BaseClass {
	static void Main(DerivedClass d) {
		$d.Test(3)$;
	}
	public void Test(int a) { }
}
class DerivedClass : BaseClass {
	public void Test(string a) { }
}";
			InvocationResolveResult mrr = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("BaseClass.Test", mrr.Member.FullName);
			
			mrr = Resolve<InvocationResolveResult>(program.Replace("(3)", "(\"3\")"));
			Assert.AreEqual("DerivedClass.Test", mrr.Member.FullName);
		}
		
		[Test, Ignore("Grouping by declaring type not yet implemented")]
		public void OverrideShadowed()
		{
			string program = @"using System;
class BaseClass {
	static void Main() {
		$new DerivedClass().Test(3)$;
	}
	public virtual void Test(int a) { }
}
class MiddleClass : BaseClass {
	public void Test(object a) { }
}
class DerivedClass : MiddleClass {
	public override void Test(int a) { }
}";
			
			InvocationResolveResult mrr = Resolve<InvocationResolveResult>(program);
			Assert.AreEqual("MiddleClass.Test", mrr.Member.FullName);
		}
		
		[Test]
		public void SubstituteClassAndMethodTypeParametersAtOnce()
		{
			string program = @"class C<X> { static void M<T>(X a, T b) { $C<T>.M(b, a)$; } }";
			var rr = Resolve<InvocationResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			
			var m = (SpecializedMethod)rr.Member;
			Assert.AreEqual("X", m.TypeArguments.Single().Name);
			Assert.AreEqual("T", m.Parameters[0].Type.Resolve(context).Name);
			Assert.AreEqual("X", m.Parameters[1].Type.Resolve(context).Name);
		}
	}
}
