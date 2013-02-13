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
		static void TestStatements (string statements, int expectedIssueCount = 0, string fixedStatements = null)
		{	
			var skeleton = @"
using System;
class A
{
	int GetValue () { return 0; }
	int SetValue (int i) { }

	void F (int a, int b)
	{";

			var input = skeleton + statements + @"
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(expectedIssueCount, issues.Count);

			if (expectedIssueCount == 0)
				return;

			CheckFix(context, issues [0], skeleton + fixedStatements + @"
	}
}");
		}

		[Test]
		public void TestIf()
		{
			TestStatements(@"
		int val = 2;
		if (true) {
			System.Console.WriteLine(val);
		}
", 1, @"
		if (true) {
			int val = 2;
			System.Console.WriteLine(val);
		}
");
		}

		[Test]
		public void TestIfWithMultipleVariables()
		{
			TestStatements(@"
		int val = 2;
		int val2;
		if (true) {
			val2 = 2;
		} else {
			val2 = 3;
			System.Console.WriteLine(val);
		}
", 1, @"
		int val2;
		if (true) {
			val2 = 2;
		} else {
			val2 = 3;
			int val = 2;
			System.Console.WriteLine(val);
		}
");
		}
		
		[Test]
		public void TestLoopNestedInIf()
		{
			TestStatements(@"
		int val = 2;
		if (true) {
			while (true) {
				val = 2;
			}
		}
", 1, @"
		if (true) {
			int val = 2;
			while (true) {
				val = 2;
			}
		}
");
		}

		[Test]
		public void IgnoresMultiBranchIf()
		{
			TestStatements(@"
		int val = 2;
		if (true) {
			System.Console.WriteLine(val);
		} else {
			System.Console.WriteLine(val);
		}
");
		}
		
		[Test]
		public void IgnoresMultiVariableDeclaration()
		{
			TestStatements(@"
		int val = 2, val2 = 3;
		if (true) {
			System.Console.WriteLine(val);
		}
");
		}
		
		[Test]
		public void IgnoresUnusedVariables()
		{
			TestStatements(@"
		int val = 2;
");
		}
		
		[Test]
		public void IgnoresVariablesOnlyUsedInOneScope()
		{
			TestStatements(@"
		var sb = new System.Text.StringBuilder();
		sb.Append ("""");
		sb.Append ("""");
");
		}
		
		[Test]
		public void DoesNotSuggestMovingIntoLoop()
		{
			TestStatements(@"
		int val = 2;
		while (true) {
			val = 3;
		}
");
		}
		
		[Test]
		public void DoesNotSuggestMovingPastDependentCondition()
		{
			TestStatements(@"
		int val = 2;
		if (val == 2 || val == 1) {
			val = 3;
		}
");
		}
		
		[Test]
		public void IfElseIf()
		{
			TestStatements(@"
		int val = 12;
		if (condition) {
			int val2 = GetValue ();
			Console.WriteLine(val2);
		} else if (!condition) {
			Console.WriteLine(val);
		}
", 1, @"
		if (condition) {
			int val2 = GetValue ();
			Console.WriteLine(val2);
		} else if (!condition) {
			int val = 12;
			Console.WriteLine(val);
		}
");
		}
		
		[Test]
		public void DoesNotSuggestMovingIntoTryCatch()
		{
			TestStatements(@"
		int val = 2;
		try {
			System.Console.WriteLine(val);
		} catch {
			throw;
		}
");
		}
		
		[Test]
		public void DoesNotSuggestMovingIntoBodyOfUsing()
		{	
			var input = @"
using System.IO;
class A
{
	void F()
	{
		using (FileStream fs = new FileStream("""", FileMode.Open, FileAccess.Read, FileShare.Read)) {
			fs.Read(null, 0, 0);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		[Test]
		public void DoesNotSuggestMovingIntoClosure ()
		{	
			TestStatements(@"
		int val1 = 2;
		Action a1 = () => Console.WriteLine(val1);

		int val2 = 2;
		Action a2 = () => { Console.WriteLine(val2); };

		int val3 = 2;
		Action a3 = delegate() { Console.WriteLine(val3); };
");
		}
		
		[Test]
		public void DoesNotSuggestMovingIntoLockStatement ()
		{	
			TestStatements(@"
		int i = 1;
		lock (this) {
			Console.WriteLine(i);
		}
");
		}

		[Test]
		public void TestEmbeddedNonBlockStatement ()
		{
			TestStatements(@"
		int val = 2;
		if (true)
			System.Console.WriteLine (val);
", 1, @"
		if (true) {
			int val = 2;
			System.Console.WriteLine (val);
		}
");
		}
		
		[Test]
		public void DoesNotSuggestMovingPastChangeOfInitializingExpression1 ()
		{	
			TestStatements(@"
		int i = 1;
		int j = i;
		i = 2;
		if (true)
			Console.WriteLine(j);
		Console.WriteLine(i);
");
		}
		
		[Test]
		public void DoesNotSuggestMovingPastChangeOfInitializingExpression2 ()
		{	
			TestStatements(@"
		int j = GetValue();
		SetValue(2);
		if (true)
			Console.WriteLine(j);
");
		}
		
		[Test]
		public void CommonParentStatement ()
		{	
			TestStatements(@"
		int k = 12;
		{
			SetValue (k);
			SetValue (k);
		}
", 1, @"
		{
			int k = 12;
			SetValue (k);
			SetValue (k);
		}
");
		}
		
		[Test]
		public void CaseTarget ()
		{	
			TestStatements(@"
		int j = 12;
		switch (a) {
		case 2:
			SetValue (j + 1);
			break;
		}
", 1, @"
		switch (a) {
		case 2:
			int j = 12;
			SetValue (j + 1);
			break;
		}
");
		}
		
		[Test]
		public void DoesNotExpandElseClause ()
		{	
			TestStatements(@"
		string o = ""Hello World !"";
		if (false) {
		} else if (!o.Contains (""Foo"")) {
		}
", 0);
		}
		
		[Test]
		public void DoesNotExpandForStatement ()
		{	
			TestStatements(@"
		var val = 12;
		if (false) {
			for (var i = GetValue (); ; i++)
				System.Console.WriteLine (val);
		}
", 1, @"
		if (false) {
			var val = 12;
			for (var i = GetValue (); ; i++)
				System.Console.WriteLine (val);
		}
");
		}
		
		[Test]
		public void DoesNotInsertBlockStatementInResourceAquisition ()
		{	
			TestStatements(@"
		string o = ""Hello World !"";
		using (var file = File.Open(o, FileMode.Open))
			return;
", 0);
		}

		[Ignore("FIXME")]
		[Test]
		public void DoesNotSuggestMovingIntoBodyAfterMethodCall()
		{	
			var input = @"
using System.IO;

class FooBar
{
	public int foo = 5;
	public void ChangeFoo ()
	{
		foo = 10;
	}
}

class A
{
	FooBar foo = new FooBar();

	public void F()
	{
		int length = foo.foo;
		foo.ChangeFoo ();
		if (true) {
			System.Console.WriteLine (length);
		}
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new VariableDeclaredInWideScopeIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}

		
		[Test]
		public void DoesNotSuggestMovingMethodCallInitializer()
		{
			// Method calls can have side effects therefore it's generally not possible to move them.
			TestStatements(@"
		int val = GetValue ();
		if (true) {
			System.Console.WriteLine(val);
		}
");
		}


	}
}

