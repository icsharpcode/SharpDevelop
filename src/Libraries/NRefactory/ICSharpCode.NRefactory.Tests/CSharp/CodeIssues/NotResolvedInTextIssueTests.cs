//
// IncorrectExceptionParametersOrderingTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class NotResolvedInTextIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestBadExamples()
		{
			TestIssue<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F()
	{
		throw new ArgumentNullException (""The parameter 'blah' can not be null"", ""blah"");
		throw new ArgumentException (""blah"", ""The parameter 'blah' can not be null"");
		throw new ArgumentOutOfRangeException (""The parameter 'blah' can not be null"", ""blah"");
		throw new DuplicateWaitObjectException (""The parameter 'blah' can not be null"", ""blah"");
	}
}", 4);
		}

		[Test]
		public void TestArgumentNullExceptionSwap()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentNullException (""bar"", ""foo"");
	}
}", 2, @"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentNullException (""foo"", ""bar"");
	}
}", 0);
		}

		[Test]
		public void TestArgumentExceptionSwap()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentException (""foo"", ""bar"");
	}
}", 2, @"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentException (""bar"", ""foo"");
	}
}", 0);
		}

		[Test]
		public void TestArgumentOutOfRangeExceptionSwap()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentOutOfRangeException (""bar"", ""foo"");
	}
}", 2, @"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentOutOfRangeException (""foo"", ""bar"");
	}
}", 0);
		}

		[Test]
		public void TestArgumentOutOfRangeExceptionSwapCase2()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentOutOfRangeException (""bar"", 3, ""foo"");
	}
}", 2, @"
using System;
class A
{
	void F(int foo)
	{
		throw new ArgumentOutOfRangeException (""foo"", 3, ""bar"");
	}
}", 0);
		}

		[Test]
		public void TestDuplicateWaitObjectExceptionSwap()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F(int foo)
	{
		throw new DuplicateWaitObjectException (""bar"", ""foo"");
	}
}", 2, @"
using System;
class A
{
	void F(int foo)
	{
		throw new DuplicateWaitObjectException (""foo"", ""bar"");
	}
}", 0);
		}


		[Test]
		public void TestInvalidArgumentException()
		{
			TestWrongContext<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentException (""bar"");
	}
}");
		}

		[Test]
		public void TestArgumentExceptionGuessing()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentException (""bar"", ""bar"");
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentException (""bar"", ""foo"");
	}
}");
		}

		[Test]
		public void TestArgumentExceptionGuessingCase2()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentException (""bar"", ""bar"", new Exception ());
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentException (""bar"", ""foo"", new Exception ());
	}
}");
		}

		[Test]
		public void TestArgumentNullGuessing()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""bar"");
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""foo"");
	}
}", 0);
		}

		[Test]
		public void TestArgumentNullGuessingResolve2()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""bar"");
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""foo"", ""bar"");
	}
}", 1);
		}

		[Test]
		public void TestArgumentNullGuessingCase2()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""bar"", ""test"");
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentNullException (""foo"", ""test"");
	}
}");
		}

		[Test]
		public void TestArgumentOutOfRangeExceptionGuessing()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (int foo, int bar)
	{
		if (foo < 0 || foo > 10)
			throw new ArgumentOutOfRangeException (""foobar"", ""foobar"");
	}
}", @"
using System;
class A
{
	void F (int foo, int bar)
	{
		if (foo < 0 || foo > 10)
			throw new ArgumentOutOfRangeException (""foo"", ""foobar"");
	}
}");
		}

		[Test]
		public void TestArgumentOutOfRangeExceptionGuessingCase2()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentOutOfRangeException (""bar"", null, ""bar"");
	}
}", @"
using System;
class A
{
	void F (object foo)
	{
		if (foo != null)
			throw new ArgumentOutOfRangeException (""foo"", null, ""bar"");
	}
}");
		}

		[Test]
		public void TestConstructorValidCase()
		{
			TestWrongContext<NotResolvedInTextIssue>(@"
using System;
class A
{
	public A(BaseRefactoringContext context, Statement rootStatement, IEnumerable<ParameterDeclaration> parameters, CancellationToken cancellationToken)
	{
		if (rootStatement == null)
			throw new ArgumentNullException(""rootStatement"");
		if (context == null)
			throw new ArgumentNullException(""context"");
	}
}");
		}

		/// <summary>
		/// Bug 15039 - source analysis can't resolve 'value' in property setter 
		/// </summary>
		[Test]
		public void TestBug15039()
		{
			TestWrongContext<NotResolvedInTextIssue>(@"
using System;
class A
{
	public string Foo {
		get {}
		set {
			if (value == null)
				throw new ArgumentNullException(""value"");
		}
	}
}");
		}

		[Test]
		public void TestValue()
		{
			Test<NotResolvedInTextIssue>(@"
using System;
class A
{
	public string Foo {
		get {}
		set {
			if (value == null)
				throw new ArgumentNullException (""val"");
		}
	}
}", @"
using System;
class A
{
	public string Foo {
		get {}
		set {
			if (value == null)
				throw new ArgumentNullException (""value"");
		}
	}
}", 0);
		}
	}
}
