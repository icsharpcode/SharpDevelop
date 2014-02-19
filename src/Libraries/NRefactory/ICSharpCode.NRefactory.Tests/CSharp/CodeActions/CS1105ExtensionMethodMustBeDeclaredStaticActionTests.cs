//
// CS1105ExtensionMethodMustBeDeclaredStaticActionTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS1105ExtensionMethodMustBeDeclaredStaticActionTests : ContextActionTestBase
	{
		[Test]
		public void TestMethod()
		{
			Test<CS1105ExtensionMethodMustBeDeclaredStaticAction> (
				@"static class Foo
{
	public void $FooBar (this string foo)
	{

	}
}",
				@"static class Foo
{
	public static void FooBar (this string foo)
	{

	}
}", 0, true);
		}

		[Test]
		public void TestWrongContext1()
		{
			TestWrongContext<CS1105ExtensionMethodMustBeDeclaredStaticAction> (
				@"static class Foo
{
	public static void $FooBar (this string foo)
	{

	}
}");
		}

		[Test]
		public void TestWrongContext2()
		{
			TestWrongContext<CS1105ExtensionMethodMustBeDeclaredStaticAction> (
				@"class Foo
{
	public void $FooBar (string foo)
	{

	}
}");
		}
	}
}

