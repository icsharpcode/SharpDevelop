//
// CS1573ParameterHasNoMatchingParamTagIssueTests.cs
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
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS1573ParameterHasNoMatchingParamTagIssueTests: InspectionActionTestBase
	{
		[Test]
		public void TestMethodMissesParameter()
		{
			Test<CS1573ParameterHasNoMatchingParamTagIssue>(@"
class Foo {
	/// <summary/>
	/// <param name = ""y""></param>
	/// <param name = ""z""></param>
	public void FooBar(int x, int y, int z)
	{
	}
}
", @"
class Foo {
	/// <summary/>
	/// <param name = ""x""></param>
	/// <param name = ""y""></param>
	/// <param name = ""z""></param>
	public void FooBar(int x, int y, int z)
	{
	}
}
");
		}

		[Test]
		public void TestNoParamDocs()
		{
			TestWrongContext<CS1573ParameterHasNoMatchingParamTagIssue>(@"
class Foo {
	/// <summary/>
	public void FooBar(int x, int y, int z)
	{
	}
}
");
		}

		[Test]
		public void TestDisable()
		{
			TestWrongContext<CS1573ParameterHasNoMatchingParamTagIssue>(@"
class Foo {
	/// <summary/>
	/// <param name = ""y""></param>
	/// <param name = ""z""></param>
// ReSharper disable once CSharpWarnings::CS1573
	public void FooBar(int x, int y, int z)
	{
	}
}
");
		}

		[Test]
		public void TestPragmaDisable()
		{
			TestWrongContext<CS1573ParameterHasNoMatchingParamTagIssue>(@"
class Foo {
	/// <summary/>
	/// <param name = ""y""></param>
	/// <param name = ""z""></param>
#pragma warning disable 1573
	public void FooBar(int x, int y, int z)
#pragma warning restore 1573
	{
	}
}
");
		}
	
	}
}

