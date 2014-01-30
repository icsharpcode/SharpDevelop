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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	// assign short name to the fake reflection type
	using dynamic = ICSharpCode.NRefactory.TypeSystem.ReflectionHelper.Dynamic;
	
	[TestFixture]
	public unsafe class UnaryOperatorTests : ResolverTestBase
	{
		CSharpResolver resolver;
		
		public override void SetUp()
		{
			base.SetUp();
			resolver = new CSharpResolver(compilation);
		}
		
		[Test]
		public void TestAddressOf()
		{
			TestOperator(UnaryOperatorType.AddressOf, MakeResult(typeof(int)),
			             Conversion.IdentityConversion, typeof(int*));
			
			TestOperator(UnaryOperatorType.AddressOf, MakeResult(typeof(byte*)),
			             Conversion.IdentityConversion, typeof(byte**));
			
			TestOperator(UnaryOperatorType.AddressOf, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
		}
		
		[Test]
		public void TestDereference()
		{
			TestOperator(UnaryOperatorType.Dereference, MakeResult(typeof(int*)),
			             Conversion.IdentityConversion, typeof(int));
			
			TestOperator(UnaryOperatorType.Dereference, MakeResult(typeof(long**)),
			             Conversion.IdentityConversion, typeof(long*));
			
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Dereference, MakeResult(typeof(int))).IsError);
			
			TestOperator(UnaryOperatorType.Dereference, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
		}
		
		[Test]
		public void TestIncrementDecrement()
		{
			TestOperator(UnaryOperatorType.Increment, MakeResult(typeof(byte)),
			             Conversion.IdentityConversion, typeof(byte));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(char)),
			             Conversion.IdentityConversion, typeof(char));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(float)),
			             Conversion.IdentityConversion, typeof(float));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(decimal)),
			             Conversion.IdentityConversion, typeof(decimal));
			
			TestOperator(UnaryOperatorType.Decrement, MakeResult(typeof(ulong)),
			             Conversion.IdentityConversion, typeof(ulong));
			
			TestOperator(UnaryOperatorType.PostDecrement, MakeResult(typeof(short?)),
			             Conversion.IdentityConversion, typeof(short?));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(TypeCode)),
			             Conversion.IdentityConversion, typeof(TypeCode));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(TypeCode?)),
			             Conversion.IdentityConversion, typeof(TypeCode?));
			
			TestOperator(UnaryOperatorType.PostIncrement, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
			
			AssertError(typeof(object), resolver.ResolveUnaryOperator(UnaryOperatorType.Increment, MakeResult(typeof(object))));
			
			TestOperator(UnaryOperatorType.Increment, MakeResult(typeof(int*)),
			             Conversion.IdentityConversion, typeof(int*));
			
			TestOperator(UnaryOperatorType.PostDecrement, MakeResult(typeof(uint*)),
			             Conversion.IdentityConversion, typeof(uint*));
		}
		
		[Test]
		public void TestUnaryPlus()
		{
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((sbyte)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((byte)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((short)1)));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((ushort)1)));
			AssertConstant(65, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant('A')));
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant(1)));
			AssertConstant((uint)1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((uint)1)));
			AssertConstant(1L, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((long)1)));
			AssertConstant((ulong)1, resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant((ulong)1)));
			
			TestOperator(UnaryOperatorType.Plus, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
			
			TestOperator(UnaryOperatorType.Plus, MakeResult(typeof(ushort?)),
			             Conversion.ImplicitNullableConversion, typeof(int?));
		}
		
		[Test]
		public void TestUnaryMinus()
		{
			AssertConstant(-1, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1)));
			AssertConstant(-1L, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant((uint)1)));
			AssertConstant(-2147483648L, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(2147483648)));
			AssertConstant(-1.0f, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1.0f)));
			AssertConstant(-1.0, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(1.0)));
			AssertConstant(1m, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-1m)));
			AssertConstant(-65, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant('A')));
			
			TestOperator(UnaryOperatorType.Minus, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
			
			TestOperator(UnaryOperatorType.Minus, MakeResult(typeof(uint?)),
			             Conversion.ImplicitNullableConversion, typeof(long?));
		}
		
		[Test]
		public void TestUnaryMinusUncheckedOverflow()
		{
			AssertConstant(-2147483648, resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-2147483648)));
		}
		
		[Test]
		public void TestUnaryMinusCheckedOverflow()
		{
			AssertError(typeof(int), resolver.WithCheckForOverflow(true).ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(-2147483648)));
		}
		
		[Test]
		public void TestBitwiseNot()
		{
			AssertConstant(1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(-2)));
			AssertConstant(~'A', resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant('A')));
			AssertConstant(~(sbyte)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((sbyte)1)));
			AssertConstant(~(byte)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((byte)1)));
			AssertConstant(~(short)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((short)1)));
			AssertConstant(~(ushort)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((ushort)1)));
			AssertConstant(~(uint)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((uint)1)));
			AssertConstant(~(long)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((long)1)));
			AssertConstant(~(ulong)1, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant((ulong)1)));
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(1.0)).IsError);
			
			TestOperator(UnaryOperatorType.BitNot, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
			
			TestOperator(UnaryOperatorType.BitNot, MakeResult(typeof(uint)),
			             Conversion.IdentityConversion, typeof(uint));
			
			TestOperator(UnaryOperatorType.BitNot, MakeResult(typeof(sbyte)),
			             Conversion.ImplicitNumericConversion, typeof(int));
			
			TestOperator(UnaryOperatorType.BitNot, MakeResult(typeof(ushort?)),
			             Conversion.ImplicitNullableConversion, typeof(int?));
		}
		
		[Test]
		public void TestLogicalNot()
		{
			AssertConstant(true, resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeConstant(false)));
			AssertConstant(false, resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeConstant(true)));
			
			TestOperator(UnaryOperatorType.Not, MakeResult(typeof(dynamic)),
			             Conversion.IdentityConversion, typeof(dynamic));
			
			TestOperator(UnaryOperatorType.Not, MakeResult(typeof(bool)),
			             Conversion.IdentityConversion, typeof(bool));
			
			TestOperator(UnaryOperatorType.Not, MakeResult(typeof(bool?)),
			             Conversion.IdentityConversion, typeof(bool?));
		}
		
		[Test]
		public void TestInvalidUnaryOperatorsOnEnum()
		{
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeConstant(StringComparison.Ordinal)).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeConstant(StringComparison.Ordinal)).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeConstant(StringComparison.Ordinal)).IsError);
			
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeResult(typeof(StringComparison))).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeResult(typeof(StringComparison))).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeResult(typeof(StringComparison))).IsError);
			
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Not, MakeResult(typeof(StringComparison?))).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Plus, MakeResult(typeof(StringComparison?))).IsError);
			Assert.IsTrue(resolver.ResolveUnaryOperator(UnaryOperatorType.Minus, MakeResult(typeof(StringComparison?))).IsError);
		}
		
		[Test]
		public void TestBitwiseNotOnEnum()
		{
			AssertConstant(~StringComparison.Ordinal, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(StringComparison.Ordinal)));
			AssertConstant(~StringComparison.CurrentCultureIgnoreCase, resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeConstant(StringComparison.CurrentCultureIgnoreCase)));
			AssertType(typeof(StringComparison), resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeResult(typeof(StringComparison))));
			AssertType(typeof(StringComparison?), resolver.ResolveUnaryOperator(UnaryOperatorType.BitNot, MakeResult(typeof(StringComparison?))));
		}
		
#if __MonoCS__
		[Ignore("Broken on mcs")]
#endif
		[Test]
		public void IntMinValue()
		{
			// int:
			AssertConstant(-2147483648, Resolve("class A { object x = $-2147483648$; }"));
			AssertConstant(-/**/2147483648, Resolve("class A { object x = $-/**/2147483648$; }"));
			// long:
			AssertConstant(-2147483648L, Resolve("class A { object x = $-2147483648L$; }"));
			AssertConstant(-(2147483648), Resolve("class A { object x = $-(2147483648)$; }"));
		}
		
		[Test]
		public void LongMinValue()
		{
			// long:
			AssertConstant(-9223372036854775808, Resolve("class A { object x = $-9223372036854775808$; }"));
			// compiler error:
			AssertError(typeof(ulong), Resolve("class A { object x = $-(9223372036854775808)$; }"));
		}
		
		[Test]
		public void IsLiftedProperty()
		{
			string program = @"
class Test {
	static void Inc() {
		int? a = 0;
		a = $-a$;
	}
}";
			var irr = Resolve<OperatorResolveResult>(program);
			Assert.IsFalse(irr.IsError);
			Assert.IsTrue(irr.IsLiftedOperator);
		}
		
		[Test]
		public void UShortEnumNegation()
		{
			string program = @"
class Test {
	enum UShortEnum : ushort { Three = 3 }
	static void Inc() {
		checked { // even in checked context, the implicit cast back to enum is unchecked
			var a = $~UShortEnum.Three$;
		}
	}
}";
			var rr = Resolve<ConstantResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual(unchecked( (ushort)~3 ), rr.ConstantValue);
		}

#if NET_4_5
		[Test]
		public void Await() {
			string program = @"
using System;
class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter() { return null; }
	public MyAwaiter GetAwaiter(int i) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("MyAwaitable.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(0, getAwaiterInvocation.Member.Parameters.Count);

			Assert.AreEqual("MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void AwaitWithGenericAwaiterType() {
			string program = @"
using System;
class MyAwaiter<T> : System.Runtime.CompilerServices.INotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public T GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter<int> GetAwaiter() { return null; }
	public MyAwaiter<int> GetAwaiter(int i) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("MyAwaitable.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(0, getAwaiterInvocation.Member.Parameters.Count);

			Assert.AreEqual("MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void AwaitWithCriticalGenericAwaiterType() {
			string program = @"
using System;
class MyAwaiter<T> : System.Runtime.CompilerServices.ICriticalNotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public void UnsafeOnCompleted(Action continuation) {}
	public T GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter<int> GetAwaiter() { return null; }
	public MyAwaiter<int> GetAwaiter(int i) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("MyAwaitable.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(0, getAwaiterInvocation.Member.Parameters.Count);

			Assert.AreEqual("MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("MyAwaiter.UnsafeOnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void AwaitWhenGetAwaiterIsAnExtensionMethod() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("N.MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("N.MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void GetAwaiterMethodWithDefaultArgumentCannotBeUsed() {
			string program = @"
using System;
class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter(int i = 0) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.GetAwaiterInvocation.IsError);
		}

		[Test, Ignore("TODO: MS C# refuses to use an extension method GetAwaiter() when there is an instance GetAwaiter() with only optional arguments. I do not know, however, if this is by design, and I could not find a simple, nice way to do the implementation")]
		public void GetAwaiterMethodWithDefaultArgumentHidesExtensionMethodAndResultsInError() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public MyAwaiter GetAwaiter(int i = 0) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.AreEqual(SpecialType.UnknownType, rr.Type);
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			Assert.IsTrue(rr.GetAwaiterInvocation.IsError);

			Assert.AreEqual(rr.AwaiterType, SpecialType.UnknownType);

			Assert.IsNull(rr.IsCompletedProperty);
			Assert.IsNull(rr.OnCompletedMethod);
			Assert.IsNull(rr.GetResultMethod);
		}

		[Test]
		public void GetAwaiterMethodWithArgumentDoesNotHideExtensionMethod() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("N.MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("N.MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void GenericGetAwaiterResultsInError() {
			string program = @"
using System;
class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter<T>() { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
		}

		[Test]
		public void AwaiterWithNoSuitableGetResult() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		public int GetResult(int i) { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.AreEqual(SpecialType.UnknownType, rr.Type);
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("N.MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterWithInaccessibleGetResult() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		private int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.AreEqual(SpecialType.UnknownType, rr.Type);
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("N.MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterWithNoIsCompletedProperty() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted() { return false; }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNull(rr.IsCompletedProperty);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterWithIsCompletedPropertyThatIsNotBoolean() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public string IsCompleted { get { return false; } }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNull(rr.IsCompletedProperty);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterWithIsCompletedPropertyThatIsNotReadable() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		public bool IsCompleted { set {} }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNull(rr.IsCompletedProperty);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterWithIsCompletedPropertyThatIsNotAccessible() {
			string program = @"
using System;
namespace N {
	class MyAwaiter : System.Runtime.CompilerServices.INotifyCompletion {
		private bool IsCompleted { get; set; }
		public void OnCompleted(Action continuation) {}
		public int GetResult() { return 0; }
	}
	class MyAwaitable {
		public static MyAwaiter GetAwaiter(int i) { return null; }
	}
	static class MyAwaitableExtensions {
		public static MyAwaiter GetAwaiter(this MyAwaitable x) { return null; }
	}
	public class C {
		public async void M() {
			MyAwaitable x = null;
			int i = $await x$;
		}
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(1, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("N.MyAwaitableExtensions.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(1, getAwaiterInvocation.Member.Parameters.Count);
			Assert.IsTrue(getAwaiterInvocation.Arguments[0] is LocalResolveResult && ((LocalResolveResult)getAwaiterInvocation.Arguments[0]).Variable.Name == "x");

			Assert.AreEqual("N.MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNull(rr.IsCompletedProperty);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("N.MyAwaiter.OnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
		}

		[Test]
		public void AwaiterThatDoesNotImplementINotifyCompletion() {
			string program = @"
using System;
class MyAwaiter {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter() { return null; }
	public MyAwaiter GetAwaiter(int i) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsTrue(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("MyAwaitable.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(0, getAwaiterInvocation.Member.Parameters.Count);

			Assert.AreEqual("MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNull(rr.OnCompletedMethod);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void AwaiterThatImplementsICriticalNotifyCompletion() {
			string program = @"
using System;
class MyAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion {
	public bool IsCompleted { get { return false; } }
	public void OnCompleted(Action continuation) {}
	public void UnsafeOnCompleted(Action continuation) {}
	public int GetResult() { return 0; }
}
class MyAwaitable {
	public MyAwaiter GetAwaiter() { return null; }
	public MyAwaiter GetAwaiter(int i) { return null; }
}
public class C {
	public async void M() {
		MyAwaitable x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.IsTrue(rr.Type.IsKnownType(KnownTypeCode.Int32));
			Assert.IsInstanceOf<CSharpInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (CSharpInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.AreEqual("MyAwaitable.GetAwaiter", getAwaiterInvocation.Member.FullName);
			Assert.AreEqual(0, getAwaiterInvocation.Member.Parameters.Count);

			Assert.AreEqual("MyAwaiter", rr.AwaiterType.FullName);

			Assert.IsNotNull(rr.IsCompletedProperty);
			Assert.AreEqual("MyAwaiter.IsCompleted", rr.IsCompletedProperty.FullName);

			Assert.IsNotNull(rr.OnCompletedMethod);
			Assert.AreEqual("MyAwaiter.UnsafeOnCompleted", rr.OnCompletedMethod.FullName);

			Assert.IsNotNull(rr.GetResultMethod);
			Assert.AreEqual("MyAwaiter.GetResult", rr.GetResultMethod.FullName);
		}

		[Test]
		public void AwaitDynamic() {
			string program = @"
public class C {
	public async void M() {
		dynamic x = null;
		int i = $await x$;
	}
}";
		
			var rr = Resolve<AwaitResolveResult>(program);
			Assert.IsFalse(rr.IsError);
			Assert.AreEqual(SpecialType.Dynamic, rr.Type);
			Assert.IsInstanceOf<DynamicInvocationResolveResult>(rr.GetAwaiterInvocation);
			var getAwaiterInvocation = (DynamicInvocationResolveResult)rr.GetAwaiterInvocation;
			Assert.IsFalse(rr.GetAwaiterInvocation.IsError);
			Assert.AreEqual(DynamicInvocationType.Invocation, getAwaiterInvocation.InvocationType);
			Assert.AreEqual(0, getAwaiterInvocation.Arguments.Count);
			Assert.IsInstanceOf<DynamicMemberResolveResult>(getAwaiterInvocation.Target);
			var target = (DynamicMemberResolveResult)getAwaiterInvocation.Target;
			Assert.IsInstanceOf<LocalResolveResult>(target.Target);
			Assert.AreEqual("GetAwaiter", target.Member);

			Assert.AreEqual(SpecialType.Dynamic, rr.AwaiterType);

			Assert.IsNull(rr.IsCompletedProperty);
			Assert.IsNull(rr.OnCompletedMethod);
			Assert.IsNull(rr.GetResultMethod);
		}
#endif // NET_4_5
	}
}
