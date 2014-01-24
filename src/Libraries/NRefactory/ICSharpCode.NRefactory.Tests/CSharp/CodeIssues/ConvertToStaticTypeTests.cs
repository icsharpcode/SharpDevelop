// 
// ConvertToStaticTypeTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun
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
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertToStaticTypeTests : InspectionActionTestBase
	{
		[Test]
		public void TestInspectorCase1()
		{
			var input = @"
using System;

namespace Demo
{
	public sealed class TestClass
	{
		static public int A;
		static public int ReturnAPlusOne()
		{
			return A + 1;
		}
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new ConvertToStaticTypeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}

		[Test]
		public void TestInspectorCase2()
		{
			var input = @"
using System;

namespace Demo
{
	public sealed class TestClass
	{
		public int B;
		static public int A;
		static public int ReturnAPlusOne()
		{
			return A + 1;
		}
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new ConvertToStaticTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestInspectorCase3()
		{
			var input = @"
using System;

namespace Demo
{
	public static class TestClass
	{
		static public int A;
		static public int ReturnAPlusOne()
		{
			return A + 1;
		}
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new ConvertToStaticTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void TestInspectorCase4()
		{
			var input = @"
using System;

namespace Demo
{
	public class TestClass
	{
		TestClass(){}
		static public int A;
		static public int ReturnAPlusOne()
		{
			return A + 1;
		}
	}
}
";
			TestRefactoringContext context;
			var issues = GetIssues(new ConvertToStaticTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}


		[Test]
		public void TestEntryPoint()
		{
			TestWrongContext<ConvertToStaticTypeIssue>(@"
using System;

namespace Demo
{
	public sealed class TestClass
	{
		static public int A;
		public static int Main()
		{
			return A + 1;
		}
	}
}
");
		}

		[Test]
		public void TestAbstract()
		{
			TestWrongContext<ConvertToStaticTypeIssue>(@"
using System;

namespace Demo
{
	public abstract class TestClass
	{
		public static int Main()
		{
			return 1;
		}
	}
}
");
		}


		[Test]
		public void TestResharperDisable()
		{
			var input = @"using System;

namespace Demo
{
//Resharper disable ConvertToStaticType
	public class TestClass
	{
		TestClass(){}
		static public int A;
		static public int ReturnAPlusOne()
		{
			return A + 1;
		}
	}
//Resharper restore ConcertToStaticType
}
";
			
			TestRefactoringContext context;
			var issues = GetIssues(new ConvertToStaticTypeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		/// <summary>
		/// Bug 16844 - Convert class to static 
		/// </summary>
		[Test]
		public void TestBug16844 ()
		{
			TestIssue<ConvertToStaticTypeIssue>(@"
class ShouldBeStatic
{
    static void Func ()
    {
    }

    class OtherNotStatic
    {
    }
}
");
		}

	}
}