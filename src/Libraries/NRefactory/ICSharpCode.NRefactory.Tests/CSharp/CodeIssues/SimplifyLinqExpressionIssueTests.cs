//
// SimplifyLinqExpressionIssueTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class SimplifyLinqExpressionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestNotAny ()
		{
			Test<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (!args.Any (d => d.Length == 0));
	}
}
", @"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (args.All (d => d.Length != 0));
	}
}
");
		}

		[Test]
		public void TestNotAll ()
		{
			Test<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (!args.All (d => d.Length == 0));
	}
}
", @"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (args.Any (d => d.Length != 0));
	}
}
");
		}
	
		[Test]
		public void TestLambdaBlockNotAny ()
		{
			Test<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (!args.Any (d => {
			return d.Length == 0;
		}));
	}
}
", @"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (args.All (d => {
			return d.Length != 0;
		}));
	}
}
");
		}


		[Test]
		public void TestAnonymousMethodBlockNotAny ()
		{
			Test<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (!args.Any (delegate (string d) {
			return d.Length == 0;
		}));
	}
}
", @"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		Console.WriteLine (args.All (delegate (string d) {
			return d.Length != 0;
		}));
	}
}
");
		}

		[Test]
		public void TestNoSimplifiction ()
		{
			TestWrongContext<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (object[] args)
	{
		Console.WriteLine (!args.Any (d => d is FooBar));
	}
}
");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<SimplifyLinqExpressionIssue>(@"
using System;
using System.Linq;

class FooBar
{
	void Test (string[] args)
	{
		// ReSharper disable once SimplifyLinqExpression
		Console.WriteLine (!args.Any (d => d.Length == 0));
	}
}
");
		}

	}
}

