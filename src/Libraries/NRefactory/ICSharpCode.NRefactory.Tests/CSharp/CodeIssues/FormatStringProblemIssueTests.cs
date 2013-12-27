//
// FormatStringProblemIssueTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class FormatStringProblemIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TooFewArguments()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""{0}"");
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}


		[Test]
		public void SupportsFixedArguments()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		Bar(""{0}"", 1);
	}

	void Bar(string format, string arg0)
	{
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void IgnoresCallWithUnknownNumberOfArguments()
		{
			var input = @"
class TestClass
{
	string Bar(params object[] args)
	{
		return string.Format(""{1}"", args);
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void FormatItemIndexOutOfRangeOfArguments()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""{1}"", 1);
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (2, issues.Count);
		}
		
		[Test]
		public void FormatItemIndexOutOfRangeOfArguments_ExplicitArrayCreation()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""{1}"", new object[] { 1 });
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (2, issues.Count);
		}
		
		[Test]
		public void FormatItemMissingEndBrace()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""Text text text {0 text text text"", 1);
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new FormatStringProblemIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}
			
		[Test]
		public void UnescapedLeftBrace()
		{
				var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""a { a"", 1);
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (2, issues.Count);
		}

		[Test]
		public void IgnoresStringWithGoodArguments()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""{0}"", ""arg0"");
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void IgnoresStringWithGoodArguments_ExplicitArrayCreation()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string.Format(""{0} {1}"", new object[] { ""arg0"", ""arg1"" });
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void IgnoresNonFormattingCall()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		string lower = string.ToLower(""{0}"");
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void HandlesCallsWithExtraArguments()
		{
			var input = @"
class TestClass
{
	void Foo()
	{
		Foo(1);
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new FormatStringProblemIssue (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}


		[Test]
		public void TooManyArguments()
		{
			TestIssue<FormatStringProblemIssue>(@"
class TestClass
{
	void Foo()
	{
		string.Format(""{0}"", 1, 2);
	}
}");
		}

		[Test]
		public void UnreferencedArgument()
		{
			TestIssue<FormatStringProblemIssue>(@"
class TestClass
{
	void Foo()
	{
		string.Format(""{0} {2}"", 1, 2, 3);
	}
}");
		}

		/// <summary>
		/// Bug 14405 - Incorrect "argument not used in string format" warning
		/// </summary>
		[Test]
		public void TestBug14405()
		{
			TestWrongContext<FormatStringProblemIssue>(@"
using System;
class TestClass
{
	void Foo()
	{
		DateTime.ParseExact(""expiresString"", ""s"", System.Globalization.CultureInfo.InvariantCulture);
	}
}");
		}
		[Test]
		public void TestDisable()
		{
			TestWrongContext<FormatStringProblemIssue>(@"
class TestClass
{
	void Foo()
	{
		// ReSharper disable once FormatStringProblem
		string.Format(""{0}"", 1, 2);
	}
}");
		}

		/// <summary>
		/// Bug 15867 - Wrong Context for string formatting
		/// </summary>
		[Test]
		public void TestBug15867()
		{
			TestWrongContext<FormatStringProblemIssue>(@"
class TestClass
{
	void Foo()
	{
		double d = 1;
		d.ToString(""G29"", System.Globalization.CultureInfo.InvariantCulture);
	}
}");
		}
	}
}

