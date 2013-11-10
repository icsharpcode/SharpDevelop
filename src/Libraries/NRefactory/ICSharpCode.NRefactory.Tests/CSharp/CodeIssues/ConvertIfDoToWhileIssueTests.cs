//
// ConvertIfDoToWhileIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class ConvertIfDoToWhileIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestBasicCase ()
		{
			Test<ConvertIfDoToWhileIssue>(@"class FooBar
{
	public void FooFoo (int x)
	{
		if (x < 10) {
			do {
				Console.WriteLine (x++);
			} while (x < 10);
		}
	}
}", @"class FooBar
{
	public void FooFoo (int x)
	{
		while (x < 10) {
			Console.WriteLine (x++);
		}
	}
}");
		}

		[Test]
		public void TestWithoutBlocks ()
		{
			Test<ConvertIfDoToWhileIssue>(@"class FooBar
{
	public void FooFoo (int x)
	{
		if (x < 10)
			do 
				Console.WriteLine (x++);
			while (x < 10);
	}
}", @"class FooBar
{
	public void FooFoo (int x)
	{
		while (x < 10)
			Console.WriteLine (x++);
	}
}");
		}

		[Test]
		public void TestInvalidCase ()
		{
			TestWrongContext<ConvertIfDoToWhileIssue>(@"class FooBar
{
	public void FooFoo (int x)
	{
		if (x < 10) {
			do {
				Console.WriteLine (x++);
			} while (x < 11);
		}
	}
}");
		}


		[Test]
		public void TestDisable ()
		{
			TestWrongContext<ConvertIfDoToWhileIssue>(@"class FooBar
{
	public void FooFoo (int x)
	{
		// ReSharper disable once ConvertIfDoToWhile
		if (x < 10) {
			do {
				Console.WriteLine (x++);
			} while (x < 10);
		}
	}
}");
		}
	}
}

