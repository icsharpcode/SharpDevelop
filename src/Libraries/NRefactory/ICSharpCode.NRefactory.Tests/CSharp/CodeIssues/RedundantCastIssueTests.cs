// 
// RedundantCastIssueTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantCastIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestSameType ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		int i = 0;
		var i2 = ((int)i);
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
		int i = 0;
		var i2 = i;
	}
}";
			Test<RedundantCastIssue> (input, 1, output);
		}

		[Test]
		public void TestInvocation ()
		{
			var input = @"
class TestClass
{
	void Test (object obj)
	{
	}
	void TestMethod (object obj)
	{
		Test ((int)obj);
	}
}";
			var output = @"
class TestClass
{
	void Test (object obj)
	{
	}
	void TestMethod (object obj)
	{
		Test (obj);
	}
}";
			Test<RedundantCastIssue> (input, 1, output);
		}

		[Test]
		public void TestLambdaInvocation ()
		{
			var input = @"
class TestClass
{
	void TestMethod (object obj)
	{
		System.Action<object> a;
		a ((int)obj);
	}
}";
			var output = @"
class TestClass
{
	void TestMethod (object obj)
	{
		System.Action<object> a;
		a (obj);
	}
}";
			Test<RedundantCastIssue> (input, 1, output);
		}

		[Test]
		public void TestMember ()
		{
			var input = @"
class TestClass
{
	void TestMethod (object obj)
	{
		var str = (obj as TestClass).ToString ();
	}
}";
			var output = @"
class TestClass
{
	void TestMethod (object obj)
	{
		var str = obj.ToString ();
	}
}";
			Test<RedundantCastIssue> (input, 1, output);
		}

		[Test]
		public void TestNoIssue ()
		{
			var input = @"
class TestClass
{
	void Test (int k) { }
	void TestMethod (object obj)
	{
		int i = (int)obj + 1;
		Test ((long) obj);
		(obj as TestClass).Test (0);
	}
}";
			Test<RedundantCastIssue> (input, 0);
		}

		/// <summary>
		/// Bug 7065 - "remove redundant type cast" false positive for explicit interface implementation
		/// </summary>
		[Test]
		public void TestBug7065 ()
		{
			var input = @"
using System;
public class TestClass : IDisposable
{
	void IDisposable.Dispose()
	{
	}

	void Foo()
	{
	    ((IDisposable)this).Dispose();
	}
}
";
			Test<RedundantCastIssue> (input, 0);
		}

		
		/// <summary>
		/// Bug 14081 - Incorrect redundant cast analysis with explicitly implemented interface member
		/// </summary>
		[Test]
		public void TestBug14081 ()
		{
			var input = @"
using System;
using System.Collections.Generic;
public class TestClass
{
	public IEnumerator<int> GetEnumerator ()
	{
		return ((IEnumerable<int>)new[] { 5 }).GetEnumerator ();
	}
}
";
			Test<RedundantCastIssue> (input, 0);
		}


		[Test]
		public void TestConditionalOperator ()
		{
			TestWrongContext<RedundantCastIssue> (@"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (object obj)
	{	
		var a = obj != null ? (A)new A () : new B ();
	}
}");
		}

		[Test]
		public void TestRedundantConditionalOperator ()
		{
			Test<RedundantCastIssue> (@"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (object obj)
	{	
		var a = obj != null ? (A)new A () : new A ();
	}
}", @"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (object obj)
	{	
		var a = obj != null ? new A () : new A ();
	}
}");
		}

		[Test]
		public void TestNullCoalesingOperator ()
		{
			TestWrongContext<RedundantCastIssue> (@"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (A obj)
	{	
		var a = (A)obj ?? new B ();
	}
}");
		}

		[Test]
		public void TestRedundantNullCoalesingOperatior ()
		{
			Test<RedundantCastIssue> (@"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (A obj)
	{	
		var a = (A)obj ?? new A ();
	}
}", @"class A {} class B : A {} class C : A {}
class TestClass
{
	void TestMethod (A obj)
	{	
		var a = obj ?? new A ();
	}
}");
		}


		[Test]
		public void TestNonRedundantDoubleCast ()
		{
			TestWrongContext<RedundantCastIssue> (@"
class Foo
{
	void Bar ()
	{	
		float f = 5.6f;
		double d = 5.6f;
		int i = d + (int)f;
	}
}");
		}

		[Test]
		public void TestNonRedundantFloatCast ()
		{
			TestWrongContext<RedundantCastIssue> (@"
class Foo
{
	void Bar ()
	{	
		float f = 5.6f;
		Console.WriteLine (""foo "" + (int)f);
	}
}");
		}

		[Test]
		public void TestNonRedundantFloatCastCase2 ()
		{
			TestWrongContext<RedundantCastIssue> (@"
using System;

class Foo
{
	void Bar ()
	{	
		float f = 5.6f;
		Console.WriteLine (""foo {0}"", (int)f);
	}
}");
		}
		
		[Test, Ignore("https://github.com/icsharpcode/NRefactory/issues/118")]
		public void TestNonRedundantCastDueToOverloading ()
		{
			TestWrongContext<RedundantCastIssue> (@"
class Foo
{
	void F(string a) {}
	void F(object a) {}

	void Bar ()
	{	
		F((object)string.Empty);
	}
}");
		}

		/// <summary>
		/// Bug 17945 - Bad 'unnecessary cast' warning
		/// </summary>
		[Test]
		public void TestBug17945 ()
		{
			TestWrongContext<RedundantCastIssue> (@"
namespace Bug
{
    public class C
    {
        public C (object o)
        {
        }
 
        public C (string o)
            : this (o as object)
        {
        }
    }
}
");
		}

		[Test]
		public void TestOverloadSelector ()
		{
			TestWrongContext<RedundantCastIssue> (@"
public class Foo
{
	public void Bar (object o)
	{
	}

	public void Bar (string o)
	    
	{
 		Bar ((object)o);
	}
}

");
		}
	}
}
