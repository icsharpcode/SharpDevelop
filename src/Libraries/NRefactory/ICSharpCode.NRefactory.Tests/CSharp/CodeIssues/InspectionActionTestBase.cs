// 
// InspectionActionTestBase.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	public abstract class InspectionActionTestBase
	{
		protected static List<CodeIssue> GetIssues (CodeIssueProvider action, string input, out TestRefactoringContext context, bool expectErrors = false, CSharpParser parser = null)
		{
			context = TestRefactoringContext.Create (input, expectErrors, parser);
			
			return new List<CodeIssue> (action.GetIssues (context));
		}

		protected static List<CodeIssue> GetIssuesWithSubIssue (CodeIssueProvider action, string input, string subIssue, out TestRefactoringContext context, bool expectErrors = false)
		{
			context = TestRefactoringContext.Create (input, expectErrors);

			return new List<CodeIssue> (action.GetIssues (context, subIssue));
		}

		protected static void CheckFix (TestRefactoringContext ctx, CodeIssue issue, string expectedOutput, int fixIndex = 0)
		{
			using (var script = ctx.StartScript ())
				issue.Actions[fixIndex].Run (script);
			if (expectedOutput != ctx.Text) {
				Console.WriteLine("expected:");
				Console.WriteLine(expectedOutput);
				Console.WriteLine("got:");
				Console.WriteLine(ctx.Text);
			}
			Assert.AreEqual (expectedOutput, ctx.Text);
		}

		protected static void CheckFix (TestRefactoringContext ctx, IEnumerable<CodeIssue> issues, string expectedOutput, int fixINdex = 0)
		{
			using (var script = ctx.StartScript ()) {
				foreach (var issue in issues) {
					issue.Actions[fixINdex].Run (script);
				}
			}
			bool pass = expectedOutput == ctx.Text;
			if (!pass) {
				Console.WriteLine ("expected:");
				Console.WriteLine (expectedOutput);
				Console.WriteLine ("got:");
				Console.WriteLine (ctx.Text);
			}
			Assert.AreEqual (expectedOutput, ctx.Text);
		}

		protected static void CheckBatchFix (TestRefactoringContext ctx, IEnumerable<CodeIssue> issues, object siblingKey, string expectedOutput)
		{
			using (var script = ctx.StartScript ()) {
				foreach (var issue in issues) {
					var actions = issue.Actions.Where (a => a.SiblingKey == siblingKey).ToList ();
					Assert.IsTrue(actions.Count <= 1, "At most a single action expected per sibling key and issue.");
					actions.First (a => a.SiblingKey == siblingKey).Run (script);
				}
			}
			bool pass = expectedOutput == ctx.Text;
			if (!pass) {
				Console.WriteLine ("expected:");
				Console.WriteLine (expectedOutput);
				Console.WriteLine ("got:");
				Console.WriteLine (ctx.Text);
			}
			Assert.AreEqual (expectedOutput, ctx.Text);
		}

		protected static void Test<T> (string input, int issueCount, string output = null, int issueToFix = -1, int actionToRun = 0)
			where T : CodeIssueProvider, new ()
		{
			TestRefactoringContext context;
			var issues = GetIssues (new T (), input, out context);
			Assert.AreEqual (issueCount, issues.Count);
			if (issueCount == 0 || output == null) 
				return;
			if (issueToFix == -1)
				CheckFix (context, issues, output, actionToRun);
			else
				CheckFix (context, issues [issueToFix], output, actionToRun);
		}

		protected static void TestIssue<T> (string input, int issueCount = 1)
			where T : CodeIssueProvider, new ()
		{
			TestRefactoringContext context;
			var issues = GetIssues (new T (), input, out context);
			Assert.AreEqual (issueCount, issues.Count);
		}

		protected static void Test<T> (string input, string output, int fixIndex = 0)
			where T : CodeIssueProvider, new ()
		{
			TestRefactoringContext context;
			var issues = GetIssues (new T (), input, out context);
			if (issues.Count == 0)
				Console.WriteLine("No issues in:\n" + input);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues[0], output, fixIndex);
		}


		protected static void TestWithSubIssue<T> (string input,  string subIssue, int issueCount, string output = null, int issueToFix = -1)
			where T : CodeIssueProvider, new ()
		{
			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new T (), input, subIssue, out context);
			Assert.AreEqual (issueCount, issues.Count);
			if (issueCount == 0 || output == null) 
				return;
			if (issueToFix == -1)
				CheckFix (context, issues, output);
			else
				CheckFix (context, issues [issueToFix], output);
		}

		protected static void TestWithSubIssue<T> (string input, string output, string subIssue, int fixIndex = 0)
			where T : CodeIssueProvider, new ()
		{
			TestRefactoringContext context;
			var issues = GetIssuesWithSubIssue (new T (), input, subIssue, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues[0], output, fixIndex);
		}

		protected static void TestWrongContext<T> (string input)
			where T : CodeIssueProvider, new ()
		{
			Test<T>(input, 0);
		}

		protected static void TestWrongContextWithSubIssue<T> (string input, string subIssue)
			where T : CodeIssueProvider, new ()
		{
			TestWithSubIssue<T>(input, subIssue, 0);
		}
	}
	
}
