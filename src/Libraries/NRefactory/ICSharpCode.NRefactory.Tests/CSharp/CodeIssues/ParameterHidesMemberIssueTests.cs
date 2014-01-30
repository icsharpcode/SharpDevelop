// 
// ParameterHidesMemberIssueTests.cs
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
	public class ParameterHidesMemberIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestField ()
		{
			var input = @"
class TestClass
{
	int i;
	void TestMethod (int i, int j)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 1);
		}

        [Test]
		public void TestDisable ()
		{
			var input = @"
class TestClass
{
	int i;
// ReSharper disable once ParameterHidesMember
	void TestMethod (int i, int j)
	{
	}
}";
			TestWrongContext<ParameterHidesMemberIssue> (input);
		}

		[Test]
		public void TestMethod ()
		{
			var input = @"
class TestClass
{
	void TestMethod2 ()
	{ }
	void TestMethod (int TestMethod2)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 1);
		}

		[Test]
		public void TestConstructor ()
		{
			var input = @"
class TestClass
{
	int i;
	public TestClass(int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}

		[Test]
		public void TestStatic ()
		{
			var input = @"
class TestClass
{
	static int i;
	static void TestMethod2 (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 1);
		}
		
		[Test]
		public void TestStaticNoIssue ()
		{
			var input = @"
class TestClass
{
	static int i;
	int j;
	void TestMethod (int i)
	{
	}
	static void TestMethod2 (int j)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}
		
		[Test]
		public void TestAccessiblePrivate ()
		{
			var input = @"
class TestClass
{
	int i;

	void Method (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 1);
		}
		
		[Test]
		public void TestAccessiblePrivateDueToTypeNesting ()
		{
			var input = @"
class RootClass
{
	int i;

	class NestedClass : RootClass
	{
		// Issue 1
		void Method (int i) {}

		class NestedNestedClass : NestedClass
		{
			// Issue 2
			void OtherMethod (int i) {}
		}
	}
}";
			Test<ParameterHidesMemberIssue> (input, 2);
		}
		
		[Test]
		public void TestInternalAccessibility ()
		{
			var input = @"
class BaseClass
{
	internal int i;
}
class TestClass : BaseClass
{
	void Method (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 1);
		}
		
		[Test]
		public void TestInaccessiblePrivate ()
		{
			var input = @"
class BaseClass
{
	private int i;
}
class TestClass : BaseClass
{
	void Method (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}

		[Test]
		public void TestIgnorePublicMethods ()
		{
			var input = @"
class TestClass
{
	private int i;

	public void SetI (int i) { this.i = i; }
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}


		[Test]
		public void TestIgnoreAbstractMethods ()
		{
			var input = @"
abstract class TestClass
{
	private int i;

	public abstract void Method (int i);
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}

		[Test]
		public void TestIgnoreOverridenMethods ()
		{
			var input = @"
class TestClass
{
	private int i;

	protected override void Method (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}

		[Test]
		public void TestIgnoreInterfaceImplementations ()
		{
			var input = @"
interface ITest {
	void Method (int i);
}

class TestClass : ITest 
{
	private int i;

	void ITest.Method (int i)
	{
	}
}";
			Test<ParameterHidesMemberIssue> (input, 0);
		}
	}
}
