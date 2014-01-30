//
// ConvertEqualsToEqualityOperatorActionTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	[TestFixture]
	public class ConvertEqualsToEqualityOperatorActionTests : ContextActionTestBase
	{
		[Test]
		public void TestEquality ()
		{
			Test<ConvertEqualsToEqualityOperatorAction> (@"class FooBar
{
	public void Foo (object x , object y)
	{
		if ($Equals (x, y)) {
		}
	}
}", @"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x == y) {
		}
	}
}");
		}

		[Test]
		public void TestInEquality ()
		{
			Test<ConvertEqualsToEqualityOperatorAction> (@"class FooBar
{
	public void Foo (object x , object y)
	{
		if (!$Equals (x, y)) {
		}
	}
}", @"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x != y) {
		}
	}
}");
		}

		[Test]
		public void TestEqualsFallback ()
		{
			Test<ConvertEqualsToEqualityOperatorAction> (@"class FooBar
{
	public void Foo (object x , object y)
	{
		if (object.$Equals (x, y)) {
		}
	}
	public new static bool Equals (object o, object x)
	{
		return false;
	}
}", @"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x == y) {
		}
	}
	public new static bool Equals (object o, object x)
	{
		return false;
	}
}");
		}

		[Test]
		public void TestMemberReferencEquals ()
		{
			Test<ConvertEqualsToEqualityOperatorAction> (@"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x.$Equals (y)) {
		}
	}
}", @"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x == y) {
		}
	}
}");
		}

		[Test]
		public void TestNegatedMemberReferencEquals ()
		{
			Test<ConvertEqualsToEqualityOperatorAction> (@"class FooBar
{
	public void Foo (object x , object y)
	{
		if (!x.$Equals (y)) {
		}
	}
}", @"class FooBar
{
	public void Foo (object x , object y)
	{
		if (x != y) {
		}
	}
}");
		}
	}
}

