// 
// PublicConstructorInAbstractionClassIssueTests.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class PublicConstructorInAbstractClassIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestInspectorCase1()
		{
			var input = @"
abstract class TestClass
{
	public TestClass ()
	{
	}
}";
			var output = @"
abstract class TestClass
{
	protected TestClass ()
	{
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new PublicConstructorInAbstractClassIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			
			CheckFix(context, issues, output);
		}
		
		[Test]
		public void TestInspectorCase2()
		{
			var input = @"
abstract class TestClass
{
	static TestClass ()
	{
	}
	void TestMethod ()
	{
		var i = 1;
	}
}";

			TestRefactoringContext context;
			var issues = GetIssues(new PublicConstructorInAbstractClassIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void TestInspectorCase3()
		{
			var input = @"
abstract class TestClass
{
	public TestClass ()
	{
	}

	private TestClass ()
	{
	}
	
	public TestClass (string str)
	{
		Console.WriteLine(str);
	}
}";
			var output = @"
abstract class TestClass
{
	protected TestClass ()
	{
	}

	private TestClass ()
	{
	}
	
	protected TestClass (string str)
	{
		Console.WriteLine(str);
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new PublicConstructorInAbstractClassIssue(), input, out context);
			Assert.AreEqual(2, issues.Count);
			
			CheckFix(context, issues, output);
		}

		[Test]
		public void TestResharperDisable()
		{
			var input = @"
//Resharper disable PublicConstructorInAbstractClass
abstract class TestClass
{
	public TestClass ()
	{
	}
//Resharper restore PublicConstructorInAbstractClass
}
";

			TestRefactoringContext context;
			var issues = GetIssues(new PublicConstructorInAbstractClassIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
	}
}