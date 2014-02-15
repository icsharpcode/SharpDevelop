// 
// MethodNeverReturnsIssueTests.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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

using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class FunctionNeverReturnsIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestEnd ()
		{
			var input = @"
class TestClass
{
	void TestMethod () 
	{
		int i = 1;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestReturn ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		return;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestThrow ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		throw new System.NotImplementedException();	
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestNeverReturns ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		while (true) ;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestIfWithoutElse ()
		{
			var input = @"
class TestClass
{
	string TestMethod (int x)
	{
		if (x <= 0) return ""Hi"";
		return ""_"" + TestMethod(x - 1);
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestRecursive ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		TestMethod ();
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestNonRecursive ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		TestMethod (0);
	}
	void TestMethod (int i)
	{
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestVirtualNonRecursive ()
		{
			var input = @"
class Base
{
	public Base parent;
	public virtual string Result {
		get { return parent.Result; }
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestNonRecursiveProperty ()
		{
			var input = @"
class TestClass
{
	int foo;
	int Foo
	{
		get { return foo; }
		set
		{
			if (Foo != value)
				foo = value;
		}
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}


		[Test]
		public void TestGetterNeverReturns ()
		{
			var input = @"
class TestClass
{
	int TestProperty
	{
		get {
			while (true) ;
		}
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestRecursiveGetter ()
		{
			var input = @"
class TestClass
{
	int TestProperty
	{
		get {
			return TestProperty;
		}
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestRecursiveSetter ()
		{
			var input = @"
class TestClass
{
	int TestProperty
	{
		set {
			TestProperty = value;
		}
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestAutoProperty ()
		{
			var input = @"
class TestClass
{
	int TestProperty
	{
		get;
		set;
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestMethodGroupNeverReturns ()
		{
			var input = @"
class TestClass
{
	int TestMethod()
	{
		return TestMethod();
	}
	int TestMethod(object o)
	{
		return TestMethod();
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestIncrementProperty()
		{
			var input = @"
class TestClass
{
	int TestProperty
	{
		get { return TestProperty++; }
		set { TestProperty++; }
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 2);
		}

		[Test]
		public void TestLambdaNeverReturns ()
		{
			var input = @"
class TestClass
{
	void TestMethod()
	{
		System.Action action = () => { while (true) ; };
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestDelegateNeverReturns ()
		{
			var input = @"
class TestClass
{
	void TestMethod()
	{
		System.Action action = delegate() { while (true) ; };
	}
}";

			Test<FunctionNeverReturnsIssue> (input, 1);
		}
		[Test]
		public void YieldBreak ()
		{
			var input = @"
class TestClass
{
	System.Collections.Generic.IEnumerable<string> TestMethod ()
	{
		yield break;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 0);
		}

		[Test]
		public void TestDisable ()
		{
			var input = @"
class TestClass
{
	// ReSharper disable once FunctionNeverReturns
	void TestMethod ()
	{
		while (true) ;
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestBug254 ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
class TestClass
{
	int state = 0;

	bool Foo()
	{
		return state < 10;
	}

	void TestMethod()
	{
		if (Foo()) {
			++state;
			TestMethod ();	
		}
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestSwitch ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
class TestClass
{
	int foo;
	void TestMethod()
	{
		switch (foo) {
			case 0: TestMethod();
		}
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestSwitchWithDefault ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
class TestClass
{
	int foo;
	void TestMethod()
	{
		switch (foo) {
			case 0: case 1: TestMethod();
			default: TestMethod();
		}
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestSwitchValue ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
class TestClass
{
	int foo;
	int TestMethod()
	{
		switch (TestMethod()) {
			case 0: return 0;
		}
		return 1;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestLinqFrom ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	IEnumerable<int> TestMethod()
	{
		return from y in TestMethod() select y;
	}
}";
			Test<FunctionNeverReturnsIssue> (input, 1);
		}

		[Test]
		public void TestWrongLinqContexts ()
		{
			//https://github.com/icsharpcode/NRefactory/issues/254
			var input = @"
using System.Linq;
using System.Collections.Generic;
class TestClass
{
	IEnumerable<int> TestMethod()
	{
		return from y in Enumerable.Empty<int>()
		       from z in TestMethod()
		       select y;
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestForeach ()
		{
			//https://bugzilla.xamarin.com/show_bug.cgi?id=14732
			var input = @"
using System.Linq;
class TestClass
{
	void TestMethod()
	{
		foreach (var x in new int[0])
			TestMethod();
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestNoExecutionFor ()
		{
			var input = @"
using System.Linq;
class TestClass
{
	void TestMethod()
	{
		for (int i = 0; i < 0; ++i)
			TestMethod ();
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestNullCoalescing ()
		{
			//https://bugzilla.xamarin.com/show_bug.cgi?id=14732
			var input = @"
using System.Linq;
class TestClass
{
	TestClass parent;
	int? value;
	int TestMethod()
	{
		return value ?? parent.TestMethod();
	}
}";
			TestWrongContext<FunctionNeverReturnsIssue> (input);
		}

		[Test]
		public void TestPropertyGetterInSetter ()
		{
			TestWrongContext<FunctionNeverReturnsIssue> (@"using System;
class TestClass
{
	int a;
	int Foo {
		get { return 1; }
		set { a = Foo; }
	}
}");
		}

		[Test]
		public void TestRecursiveFunctionBug ()
		{
			TestWrongContext<FunctionNeverReturnsIssue> (@"using System;
class TestClass
{
	bool Foo (int i)
	{
		return i < 0 || Foo (i - 1);
	}
}");
		}

		/// <summary>
		/// Bug 17769 - Incorrect "method never returns" warning
		/// </summary>
		[Test]
		public void TestBug17769 ()
		{
			TestWrongContext<FunctionNeverReturnsIssue> (@"
using System.Linq;
class A
{
    A[] list = new A[0];

    public bool Test ()
    {
        return list.Any (t => t.Test ());
    }
}
");
		}
	}
}
