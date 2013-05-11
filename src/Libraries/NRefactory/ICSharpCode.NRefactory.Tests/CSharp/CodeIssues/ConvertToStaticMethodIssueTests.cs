//
// ConvertToStaticMethodIssue.cs
//
// Author:
//       Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud
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
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertToStaticMethodIssueTests : InspectionActionTestBase
	{
		
		[Test]
		public void Test()
		{
			Test<ConvertToStaticMethodIssue>(
				@"class TestClass
{
	void $Test ()
	{
		int a = 2;
	}
}",
				@"class TestClass
{
	static void Test ()
	{
		int a = 2;
	}
}"
				);
		}
		[Test]
		public void TestWithVirtualFunction() {
			
			var input = @"class TestClass
{
	public virtual void $Test ()
	{
		int a = 2;
	}
}";
            TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Ignore("Doesn't work for me.")]
		[Test]
		public void TestWithInterface() {
			
			var input = @"interface IBase {
    void Test();
}
class TestClass : IBase
{
	public virtual void $Test ()
	{
		int a = 2;
	}
}";
            TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Test]
		public void TestWithStaticFunction()
		{

			var input = @"class TestClass
{
	static void $Test ()
	{
		int a = 2;
	}
}";
			TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Test]
		public void TestDoNotWarnOnAttributes()
		{

			var input = @"using System;
class TestClass
{
	[Obsolete]
	public void $Test ()
	{
		int a = 2;
	}
}";
			TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Test]
		public void TestDoNotWarnOnEmptyMethod()
		{

			var input = @"using System;
class TestClass
{
	public void $Test ()
	{
	}
}";
			TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Test]
		public void TestDoNotWarnOnNotImplementedMethod()
		{
			var input = @"using System;
class TestClass
{
	public void $Test ()
	{
		throw new NotImplementedExceptionIssue();
	}
}";
			TestWrongContext<ConvertToStaticMethodIssue>(input);
		}

		[Ignore("Body analyzation is missing.")]
		[Test]
		public void TestPropertyAccess()
		{
			var input = @"using System;
class TestClass
{
	public int Foo { get; set; }
	public void $Test ()
	{
		System.Console.WriteLine (Foo);
	}
}";
			TestWrongContext<ConvertToStaticMethodIssue>(input);
		}


	
	}
    
}
