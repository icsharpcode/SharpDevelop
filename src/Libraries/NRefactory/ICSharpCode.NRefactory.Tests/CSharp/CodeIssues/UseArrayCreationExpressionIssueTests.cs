//
// UseArrayCreationExpressionIssueTests.cs
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class UseArrayCreationExpressionIssueTests : InspectionActionTestBase
	{
		[Test]
		public void TestTypeOfIsAssignableFrom ()
		{
			Test<UseArrayCreationExpressionIssue> (@"
class Test
{
	void Foo()
	{
		System.Array.CreateInstance(typeof(int), 10);
	}
}
", @"
class Test
{
	void Foo()
	{
		new int[10];
	}
}
");
		}

		[Test]
		public void MultiDim ()
		{
			Test<UseArrayCreationExpressionIssue> (@"
class Test
{
	void Foo(int i)
	{
		System.Array.CreateInstance(typeof(int), 10, 20, i);
	}
}
", @"
class Test
{
	void Foo(int i)
	{
		new int[10, 20, i];
	}
}
");
		}

		[Test]
		public void TestDisable ()
		{
			TestWrongContext<UseArrayCreationExpressionIssue> (@"
class Test
{
	void Foo()
	{
		// ReSharper disable once UseArrayCreationExpression
		System.Array.CreateInstance(typeof(int), 10);
	}
}
");
		}

	}
}

