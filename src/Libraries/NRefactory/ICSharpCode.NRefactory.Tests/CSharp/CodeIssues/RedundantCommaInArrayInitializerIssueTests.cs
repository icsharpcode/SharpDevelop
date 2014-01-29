// 
// RedundantCommaInArrayInitializerIssueTests.cs
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

using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantCommaInArrayInitializerIssueTests : InspectionActionTestBase
	{
		[Test]
		public void Test ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		var a = new int[] { 1, 2, };
	}
}";
			var output = @"
class TestClass
{
	void TestMethod ()
	{
		var a = new int[] { 1, 2 };
	}
}";
			Test<RedundantCommaInArrayInitializerIssue> (input, 1, output);
		}

		[Test]
		public void TestArrayInitializerDescription ()
		{
			var input = @"
class TestClass
{
	void TestMethod ()
	{
		var a = new int[] { 1, 2, };
	}
}";
			TestRefactoringContext ctx;
			var issues = GetIssues (new RedundantCommaInArrayInitializerIssue (), input, out ctx);
			Assert.AreEqual (issues.Count, 1);
		}

		[Test]
		public void TestObjectInitializerDescription ()
		{
			var input = @"
class TestClass
{
	int Prop { get; set; }
	void TestMethod ()
	{
		var a = new TestClass { Prop = 1, };
	}
}";
			TestRefactoringContext ctx;
			var issues = GetIssues (new RedundantCommaInArrayInitializerIssue (), input, out ctx);
			Assert.AreEqual (issues.Count, 1);
		}

        [Test]
        public void TestCollectionInitializerDescrition()
        {
            var input = @"
class TestClass
{
	void TestMethod ()
	{
		var a = new TestClass { 1, };
	}
}";
            TestRefactoringContext ctx;
            var issues = GetIssues(new RedundantCommaInArrayInitializerIssue(), input, out ctx);
            Assert.AreEqual(issues.Count, 1);
        }

        [Test]
        public void TestDisable()
        {
            var input = @"
class TestClass
{
	void TestMethod ()
	{            
// ReSharper disable once RedundantCommaInArrayInitializer
		var a = new TestClass { 1, };
	}
}";
            TestWrongContext<RedundantCommaInArrayInitializerIssue>(input);
        }
	}
}
