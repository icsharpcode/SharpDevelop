//
// UseMethodAnyIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class UseMethodAnyIssueTests: InspectionActionTestBase
	{
		static string ConstructExpression(string expr)
		{
			return @"
using System;
using System.Linq;
class Bar
{
	public void Foo (string[] args)
	{
		if (" + expr + @")
			Console.WriteLine ();
	}
}
";
		}

		[Test]
		public void TestAnyNotEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () != 0"), ConstructExpression("args.Any ()"));
		}

		[Test]
		public void TestAnyGreater ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () > 0"), ConstructExpression("args.Any ()"));
		}

		[Test]
		public void TestAnyLower ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("0 < args.Count ()"), ConstructExpression("args.Any ()"));
		}


		[Test]
		public void TestAnyGreaterEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () >= 1"), ConstructExpression("args.Any ()"));
		}

		[Test]
		public void TestAnyLessEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("1 <= args.Count ()"), ConstructExpression("args.Any ()"));
		}


		[Test]
		public void TestNotAnyEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () == 0"), ConstructExpression("!args.Any ()"));
		}

		[Test]
		public void TestNotAnyLess ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () < 1"), ConstructExpression("!args.Any ()"));
		}

		[Test]
		public void TestNotAnyGreater ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("1 > args.Count ()"), ConstructExpression("!args.Any ()"));
		}

		[Test]
		public void TestNotAnyLessEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("args.Count () <= 0"), ConstructExpression("!args.Any ()"));
		}

		[Test]
		public void TestNotAnyGreaterEqual ()
		{
			Test<UseMethodAnyIssue>(ConstructExpression("0 >= args.Count ()"), ConstructExpression("!args.Any ()"));
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<UseMethodAnyIssue>(@"
using System;
using System.Linq;
class Bar
{
	public void Foo (string[] args)
	{
		// ReSharper disable once UseMethodAny
		if (args.Count () > 0)
			Console.WriteLine();
	}
}
");
		}

		[Test]
		public void TestWrongMethod ()
		{
			TestWrongContext<UseMethodAnyIssue>(@"
using System;
using System.Linq;
class Bar
{
	public int Count () { return 5; }

	public void Foo (Bar args)
	{
		if (args.Count () > 0)
			Console.WriteLine();
	}
}
");
		}



	}
}

