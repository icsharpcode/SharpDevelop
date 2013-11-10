//
// ConvertForToWhileActionTests.cs
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

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ConvertForToWhileActionTests  : ContextActionTestBase
	{
		[Test]
		public void TestSimpleFor ()
		{
			Test<ConvertForToWhileAction>(@"
class Test
{
	void Foo (object[] o)
	{
		$for (int i = 0, oLength = o.Length; i < oLength; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		int i = 0, oLength = o.Length;
		while (i < oLength) {
			var p = o [i];
			System.Console.WriteLine (p);
			i++;
		}
	}
}");
		}

		[Test]
		public void TestMissingInitializer ()
		{
			Test<ConvertForToWhileAction>(@"
class Test
{
	void Foo ()
	{
		$for (; i < oLength; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo ()
	{
		while (i < oLength) {
			var p = o [i];
			System.Console.WriteLine (p);
			i++;
		}
	}
}");
		}

		[Test]
		public void TestMissingCondition ()
		{
			Test<ConvertForToWhileAction>(@"
class Test
{
	void Foo (object[] o)
	{
		$for (int i = 0, oLength = o.Length;; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		int i = 0, oLength = o.Length;
		while (true) {
			var p = o [i];
			System.Console.WriteLine (p);
			i++;
		}
	}
}");
		}

		[Test]
		public void TestInfiniteLoop ()
		{
			Test<ConvertForToWhileAction>(@"
class Test
{
	void Foo (object[] o)
	{
		$for (;;) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		while (true) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}");
		}

		[Test]
		public void TestMultipleInitializersAndIterators ()
		{
			Test<ConvertForToWhileAction>(@"
class Test
{
	void Foo (object[] o)
	{
		$for (i=0,j=0; i < oLength; i++,j++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		i = 0;
		j = 0;
		while (i < oLength) {
			var p = o [i];
			System.Console.WriteLine (p);
			i++;
			j++;
		}
	}
}");
		}

	}
}

