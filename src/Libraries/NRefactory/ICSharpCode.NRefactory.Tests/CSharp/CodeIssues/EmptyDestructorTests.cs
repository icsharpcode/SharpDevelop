//
// EmptyDestructorTests.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
	public class EmptyDestructorTests : InspectionActionTestBase
	{
		[Test]
		public void TestBasicCase()
		{
			var input = @"
class Foo
{
	~Foo()
	{
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new EmptyDestructorIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"
class Foo
{
}");
		}

		[Test]
		public void TestCaseWithNesting()
		{
			var input = @"
class Foo
{
	~Foo()
	{
		{}
		;
		{;}
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new EmptyDestructorIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"
class Foo
{
}");
		}

		[Test]
		public void TestDisabledForNonEmpty()
		{
			var input = @"
class Foo
{
	~Foo()
	{
		System.Console.WriteLine();
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new EmptyDestructorIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestDisable()
		{
			var input = @"
class Foo
{
	// ReSharper disable once EmptyDestructor
	~Foo()
	{
	}
}";
			TestWrongContext<EmptyDestructorIssue>(input);
		}

	}
}

