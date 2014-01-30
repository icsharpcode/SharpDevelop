//
// DuplicateCaseLabelValueIssueTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;



namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0152DuplicateCaseLabelValueIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestConstants()
		{
			TestIssue<CS0152DuplicateCaseLabelValueIssue>(@"
class Test
{
	const int foo = 1;
	void TestMethod (int i = 0)
	{
		switch (i) {
			case 1:
				System.Console.WriteLine();
				break;
			case foo:
				System.Console.WriteLine();
				break;
			case 4:
				default:
			case 3:
				break;
		}
	}
}", 2);
		}



		[Test]
		public void TestInts()
		{
			TestIssue<CS0152DuplicateCaseLabelValueIssue>(@"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		case 0:
			System.Console.WriteLine();
			break;
		case 0:
			System.Console.WriteLine();
			break;
		default:
		case 0:
			break;
		}
	}
}", 3);
		}

		[Test]
		public void TestStrings()
		{
			TestIssue<CS0152DuplicateCaseLabelValueIssue>(@"
class Test
{
	void TestMethod (string i = 0)
	{
		switch (i) {
		case ""	"":
			System.Console.WriteLine();
			break;
		case ""\t"":
			System.Console.WriteLine();
			break;
		default:
		case @""	"":
			break;
		}
	}
}", 3);
		}

		[Test]
		public void TestNoIssue()
		{
			TestWrongContext<CS0152DuplicateCaseLabelValueIssue>(@"
class Test
{
	void TestMethod (int i = 0)
	{
		switch (i) {
		case 1:
			System.Console.WriteLine();
			break;
		case 2:
			System.Console.WriteLine();
			break;
		case 4:
		default:
		case 3:
			break;
		}
	}
}");
		}
	}
}

