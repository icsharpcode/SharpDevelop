// 
// ImplementAbstractMembersTest.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ImplementAbstractMembersTest : ContextActionTestBase
	{
		[Test]
		public void TestSimpleBaseType()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Simple {
	public abstract void FooBar (string foo, int bar);
}

class Foo : $Simple
{
}
", @"abstract class Simple {
	public abstract void FooBar (string foo, int bar);
}

class Foo : Simple
{
	#region implemented abstract members of Simple
	public override void FooBar (string foo, int bar)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}

		[Test]
		public void TestProtectedMembers()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Simple {
	protected abstract string ServiceName { get; }
}

class Foo : $Simple
{
}
", @"abstract class Simple {
	protected abstract string ServiceName { get; }
}

class Foo : Simple
{
	#region implemented abstract members of Simple
	protected override string ServiceName {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}

		[Test]
		public void TestProtectedInternalMembers()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Simple {
	protected internal abstract string ServiceName { get; }
}

class Foo : $Simple
{
}
", @"abstract class Simple {
	protected internal abstract string ServiceName { get; }
}

class Foo : Simple
{
	#region implemented abstract members of Simple
	protected internal override string ServiceName {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}
	
		[Test]
		public void TestAbstractOverrideMethodCase1()
		{
			Test<ImplementAbstractMembersAction>(@"class A {
	public virtual void Foo() {
		Console.WriteLine(""A:Foo()"");
	}
}

abstract class B : A {
	public abstract override void Foo();
	public abstract void FooBar();
}

class C : $B
{
}
", @"class A {
	public virtual void Foo() {
		Console.WriteLine(""A:Foo()"");
	}
}

abstract class B : A {
	public abstract override void Foo();
	public abstract void FooBar();
}

class C : B
{
	#region implemented abstract members of B
	public override void Foo ()
	{
		throw new System.NotImplementedException ();
	}
	public override void FooBar ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}

		[Test]
		public void TestAbstractOverrideMethodCase2()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Base {
	public abstract void Foo ();
}

abstract class Base2 : Base {
	public abstract override void Foo ();
}

class MockTypeInfo : $Base2
{
}
", @"abstract class Base {
	public abstract void Foo ();
}

abstract class Base2 : Base {
	public abstract override void Foo ();
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override void Foo ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}

		[Test]
		public void TestAbstractOverrideEventCase1()
		{
			Test<ImplementAbstractMembersAction>(@"using System;
class A {
	public virtual event EventHandler Foo { add {} remove {} }
}

abstract class B : A {
	public abstract override event EventHandler Foo;
	public abstract event EventHandler FooBar;
}

class C : $B
{
}
", @"using System;
class A {
	public virtual event EventHandler Foo { add {} remove {} }
}

abstract class B : A {
	public abstract override event EventHandler Foo;
	public abstract event EventHandler FooBar;
}

class C : B
{
	#region implemented abstract members of B
	public override event EventHandler Foo;
	public override event EventHandler FooBar;
	#endregion
}
");
		}

		
		[Test]
		public void TestAbstractOverridePropertyCase2()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Base {
	public abstract int Foo { get;  }
}

abstract class Base2 : Base {
	public abstract override int Foo { get; }
}

class MockTypeInfo : $Base2
{
}
", @"abstract class Base {
	public abstract int Foo { get;  }
}

abstract class Base2 : Base {
	public abstract override int Foo { get; }
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override int Foo {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}



		[Test]
		public void TestAbstractOverrideVirtualMethod()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Base {
	public virtual void Foo () { }
}

abstract class Base2 : Base {
	public abstract override void Foo () { }
}

class MockTypeInfo : $Base2
{
}
", @"abstract class Base {
	public virtual void Foo () { }
}

abstract class Base2 : Base {
	public abstract override void Foo () { }
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override void Foo ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}



		[Test]
		public void TestAbstractOverrideVirtualProperty()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Base {
	public virtual int Foo { get { return 1; } }
}

abstract class Base2 : Base {
	public abstract override int Foo { get; }
}

class MockTypeInfo : $Base2
{
}
", @"abstract class Base {
	public virtual int Foo { get { return 1; } }
}

abstract class Base2 : Base {
	public abstract override int Foo { get; }
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override int Foo {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}

		[Test]
		public void TestAbstractOverrideVirtualIndexer()
		{
			Test<ImplementAbstractMembersAction>(@"abstract class Base {
	public virtual int this [int i] { get { return 1; } }
}

abstract class Base2 : Base {
	public abstract override int this [int i] { get; }
}

class MockTypeInfo : $Base2
{
}
", @"abstract class Base {
	public virtual int this [int i] { get { return 1; } }
}

abstract class Base2 : Base {
	public abstract override int this [int i] { get; }
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override int this [int i] {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}

		[Test]
		public void TestAbstractOverrideVirtualEvent()
		{
			Test<ImplementAbstractMembersAction>(@"using System;

abstract class Base {
	public virtual event EventHandler Foo {
		add {}
		remove{}
	}
}

abstract class Base2 : Base {
	public override abstract event EventHandler Foo;
}

class MockTypeInfo : $Base2
{
}
", @"using System;

abstract class Base {
	public virtual event EventHandler Foo {
		add {}
		remove{}
	}
}

abstract class Base2 : Base {
	public override abstract event EventHandler Foo;
}

class MockTypeInfo : Base2
{
	#region implemented abstract members of Base2
	public override event EventHandler Foo;
	#endregion
}
");
		}

		[Test]
		public void TestAlreadyImplementedMethods()
		{
			Test<ImplementAbstractMembersAction>(@"class A {
	public abstract void Foo();
	public abstract void FooBar();
}

abstract class B : A {
	public override void Foo() {}
}

class C : $B
{
}
", @"class A {
	public abstract void Foo();
	public abstract void FooBar();
}

abstract class B : A {
	public override void Foo() {}
}

class C : B
{
	#region implemented abstract members of A
	public override void FooBar ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}

		[Test]
		public void TestAlreadyImplementedProperties()
		{
			Test<ImplementAbstractMembersAction>(@"class A {
	public abstract int Foo { get; }
	public abstract int FooBar { get; }
}

abstract class B : A {
	public override int Foo { get { return 1; } }
}

class C : $B
{
}
", @"class A {
	public abstract int Foo { get; }
	public abstract int FooBar { get; }
}

abstract class B : A {
	public override int Foo { get { return 1; } }
}

class C : B
{
	#region implemented abstract members of A
	public override int FooBar {
		get {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
");
		}


		[Test]
		public void TestConstraints()
		{
			Test<ImplementAbstractMembersAction>(@"class NSObject
{
}

abstract class A
{
	public abstract void Foo<T, U> () where T : NSObject, U where U : NSObject;
}

class B : $A
{
}
", @"class NSObject
{
}

abstract class A
{
	public abstract void Foo<T, U> () where T : NSObject, U where U : NSObject;
}

class B : A
{
	#region implemented abstract members of A
	public override void Foo<T, U> ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
");
		}


	}
}

