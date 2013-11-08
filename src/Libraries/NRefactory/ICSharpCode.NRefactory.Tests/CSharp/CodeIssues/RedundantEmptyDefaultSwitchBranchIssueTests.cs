//
// RedundantEmptyDefaultSwitchBranchIssueTests.cs
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
using System;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class RedundantEmptyDefaultSwitchBranchIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestDefaultRedundantCase ()
		{
			Test<RedundantEmptyDefaultSwitchBranchIssue>(@"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		case 0:
			System.Console.WriteLine();
			break;
		default:
			break;
		}
	}
}", @"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		case 0:
			System.Console.WriteLine();
			break;
		}
	}
}");
		}

		[Test]
		public void TestMinimal ()
		{
			Test<RedundantEmptyDefaultSwitchBranchIssue>(@"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		default:
			break;
		}
	}
}", @"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		}
	}
}");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<RedundantEmptyDefaultSwitchBranchIssue>(@"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		case 0:
			System.Console.WriteLine();
			break;
		// ReSharper disable once RedundantEmptyDefaultSwitchBranch
		default:
			break;
		}
	}
}");
		}
	}
}

