//
// SetterDoesNotUseValueParameterTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	public class VariableDeclaredInWideScopeTests : InspectionActionTestBase
	{
		[Test]
		public void TestIf()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
		if (true) {
			System.Console.WriteLine(val);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);

			CheckFix(context, issues [0], @"
class A
{
	void F()
	{
		if (true) {
			int val = 2;
			System.Console.WriteLine(val);
		}
	}
}");
		}

		[Test]
		public void TestIfWithMultipleVariables()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
		int val2;
		if (true) {
			val2 = 2;
		} else {
			val2 = 3;
			System.Console.WriteLine(val);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			
			CheckFix(context, issues [0], @"
class A
{
	void F()
	{
		int val2;
		if (true) {
			val2 = 2;
		} else {
			int val = 2;
			val2 = 3;
			System.Console.WriteLine(val);
		}
	}
}");
		}
		
		[Test]
		public void TestLoopNestedInIf()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
		if (true) {
			while (true) {
				val = 2;
			}
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			
			CheckFix(context, issues [0], @"
class A
{
	void F()
	{
		if (true) {
			int val = 2;
			while (true) {
				val = 2;
			}
		}
	}
}");
		}

		[Test]
		public void IgnoresMultiBranchIf()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
		if (true) {
			System.Console.WriteLine(val);
		} else {
			System.Console.WriteLine(val);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void IgnoresMultiVariableDeclaration()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2, val2 = 3;
		if (true) {
			System.Console.WriteLine(val);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void IgnoresUnusedVariables()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
		
		[Test]
		public void DoesNotSuggestMovingIntoLoop()
		{
			var input = @"
class A
{
	void F()
	{
		int val = 2;
		while (true) {
			val = 3;
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
	}
}

