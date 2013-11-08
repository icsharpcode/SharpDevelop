// 
// CreateClassDeclarationTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class CreateClassDeclarationTests : ContextActionTestBase
	{
		[Test()]
		public void TestCreateClass ()
		{
			Test<CreateClassDeclarationAction> (
@"
class TestClass
{
	void TestMethod ()
	{
		$new Foo (0, ""Hello"");
	}
}
", @"
class Foo
{
	public Foo (int i, string hello)
	{
		throw new System.NotImplementedException ();
	}
}
class TestClass
{
	void TestMethod ()
	{
		new Foo (0, ""Hello"");
	}
}
");
		}

		[Test]
		public void TestNestedCreateClass()
		{
			Test<CreateClassDeclarationAction>(
@"
class TestClass
{
	void TestMethod ()
	{
		$new Foo (0);
	}
}
", @"
class TestClass
{
	class Foo
	{
		public Foo (int i)
		{
			throw new System.NotImplementedException ();
		}
	}
	void TestMethod ()
	{
		new Foo (0);
	}
}
", 1);
		}

		[Test]
		public void TestEmptyConstructor ()
		{
			Test<CreateClassDeclarationAction> (
@"
class TestClass
{
	void TestMethod ()
	{
		$new Foo ();
	}
}
", @"
class Foo
{
}
class TestClass
{
	void TestMethod ()
	{
		new Foo ();
	}
}
");
		}

		[Test]
		public void TestCreatePublicEventArgs ()
		{
			Test<CreateClassDeclarationAction> (
@"
class TestClass
{
	public event EventHandler<$MyEventArgs> evt;
}
", @"
public class MyEventArgs : System.EventArgs
{
}
class TestClass
{
	public event EventHandler<MyEventArgs> evt;
}
");
		}

		[Test]
		public void TestCreateInternalEventArgs ()
		{
			Test<CreateClassDeclarationAction> (
@"
class TestClass
{
	internal event EventHandler<$MyEventArgs> evt;
}
", @"
class MyEventArgs : System.EventArgs
{
}
class TestClass
{
	internal event EventHandler<MyEventArgs> evt;
}
");
		}

		[Test]
		public void TestCreateAttribute ()
		{
			Test<CreateClassDeclarationAction> (
@"
[$MyAttribute]
class TestClass
{
}
", @"
class MyAttribute : System.Attribute
{
}
[MyAttribute]
class TestClass
{
}
");
		}

		[Test]
		public void TestCreateAttributeCase2 ()
		{
			Test<CreateClassDeclarationAction> (
@"
[$My]
class TestClass
{
}
", @"
class MyAttribute : System.Attribute
{
}
[My]
class TestClass
{
}
");
		}

		[Test]
		public void TestCreateException ()
		{
			Test<CreateClassDeclarationAction> (
@"
class TestClass
{
	void TestMethod ()
	{
		throw $new MyException ();
	}
}
", @"
class MyException : System.Exception
{
}
class TestClass
{
	void TestMethod ()
	{
		throw new MyException ();
	}
}
");
		}

		[Test]
		public void TestNotShowInEventTypes()
		{
			TestWrongContext<CreateClassDeclarationAction>(
@"
class TestClass
{
	event $MyEventHandler evt;
}
");
		}

		[Test]
		public void TestCreateClassImplementingInterface()
		{
			Test<CreateClassDeclarationAction>(
@"
class TestClass
{
	void TestMethod (System.IDisposable d)
	{
		TestMethod ($new Foo ());
	}
}
", @"
class Foo : System.IDisposable
{
	public void Dispose ()
	{
		throw new System.NotImplementedException ();
	}
}
class TestClass
{
	void TestMethod (System.IDisposable d)
	{
		TestMethod (new Foo ());
	}
}
");
		}

		[Test]
		public void TestCreateClassExtendingAbstractClass()
		{
			Test<CreateClassDeclarationAction>(
@"
class TestClass
{
	abstract class FooBar { protected abstract void SomeFoo (); public abstract int MoreFoo { get; } }
	void TestMethod (FooBar d)
	{
		TestMethod ($new Foo ());
	}
}
", @"
class Foo : FooBar
{
	public override int MoreFoo {
		get;
	}

	protected override void SomeFoo ()
	{
		throw new System.NotImplementedException ();
	}
}
class TestClass
{
	abstract class FooBar { protected abstract void SomeFoo (); public abstract int MoreFoo { get; } }
	void TestMethod (FooBar d)
	{
		TestMethod (new Foo ());
	}
}
");
		}

		[Test]
		public void TestModifierBug ()
		{
			Test<CreateClassDeclarationAction> (
				@"
class TestClass
{
	private readonly $Foo _foo;
}
", @"
class Foo
{
}
class TestClass
{
	private readonly Foo _foo;
}
");
		}


		[Test]
		public void TestCreateClassFromMemberReferenceExpression ()
		{
			Test<CreateClassDeclarationAction> (
				@"
class TestClass
{
	void TestMethod ()
	{
		$Foo.Bar (1);
	}
}
", @"
class Foo
{
}
class TestClass
{
	void TestMethod ()
	{
		Foo.Bar (1);
	}
}
");
		}


		/// <summary>
		/// Bug 10671 - Auto-Fix of Base Class is wrong (generates invalid code) 
		/// </summary>
		[Test]
		public void TestBug10671 ()
		{
			var input = @"
namespace TestConsole
{
    public class Test : $BaseMissing
    {
    }
}
";
			// action allowed to create a nested class
			var context = TestRefactoringContext.Create (input, false);
			var actions = new CreateClassDeclarationAction().GetActions (context);
			Assert.AreEqual (1, actions.Count ());
		}

		
		[Test]
		public void TestCreateInterface ()
		{
			Test<CreateClassDeclarationAction> (
				@"
class TestClass
{
	private readonly $IFoo _foo;
}
", @"
interface IFoo
{
}
class TestClass
{
	private readonly IFoo _foo;
}
");
		}

		/// <summary>
		/// Bug 10672 - Auto-Fix of Generate Class to fill Generic params does not take in account constraints
		/// </summary>
		[Test]
		public void TestBug10672 ()
		{
			Test<CreateClassDeclarationAction> (
				@"
namespace TestConsole
{
    public interface IBase
    {
    }
    public class Test 
    {
        public void Generate<S, T>() where T:IBase, new()
        {

        }
    }
    class MainClass
    {
        public static void Main (string[] args)
        {
            var testConsole = new Test ();
            testConsole.Generate<int, $Data> ();
        }
    }
}
", @"
public class Data : IBase
{
	public Data ()
	{
	}
}
namespace TestConsole
{
    public interface IBase
    {
    }
    public class Test 
    {
        public void Generate<S, T>() where T:IBase, new()
        {

        }
    }
    class MainClass
    {
        public static void Main (string[] args)
        {
            var testConsole = new Test ();
            testConsole.Generate<int, Data> ();
        }
    }
}
");
		}
		
		[Test]
		public void TestStructConstraint ()
		{
			Test<CreateClassDeclarationAction> (
				@"
public class Test 
{
	public void Generate<T> () where T : struct
	{
	}

	public void FooBar ()
	{
		Generate<$Data> ();
	}
}
", @"
public struct Data
{
}
public class Test 
{
	public void Generate<T> () where T : struct
	{
	}

	public void FooBar ()
	{
		Generate<Data> ();
	}
}
");
		}

		[Test]
		public void TestClassTypeParameter ()
		{
			Test<CreateClassDeclarationAction> (
				@"
public class Test 
{
	public class Generate<T> where T : struct {}

	public void FooBar ()
	{
		Generate<$Data> foo;
	}
}
", @"
public struct Data
{
}
public class Test 
{
	public class Generate<T> where T : struct {}

	public void FooBar ()
	{
		Generate<Data> foo;
	}
}
");
		}
		[Test]
		public void TestClassTypeParameterCase2 ()
		{
			Test<CreateClassDeclarationAction> (
				@"
public class Test 
{
	public class Generate<T> where T : struct {}

	public void FooBar ()
	{
		new Generate<$Data> ();
	}
}
", @"
public struct Data
{
}
public class Test 
{
	public class Generate<T> where T : struct {}

	public void FooBar ()
	{
		new Generate<Data> ();
	}
}
");
		}



	}
}
