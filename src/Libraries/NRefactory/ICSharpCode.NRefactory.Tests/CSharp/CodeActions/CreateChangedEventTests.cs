//
// CreateChangedEventTests.cs
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
	public class CreateChangedEventTests : ContextActionTestBase
	{
		[Test]
		public void TestSimpleCase ()
		{
			Test<CreateChangedEventAction> (@"class TestClass
{
	string test;
	public string $Test {
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}", @"class TestClass
{
	string test;
	public event System.EventHandler TestChanged;
	protected virtual void OnTestChanged (System.EventArgs e)
	{
		var handler = TestChanged;
		if (handler != null)
			handler (this, e);
	}
	public string Test {
		get {
			return test;
		}
		set {
			test = value;
			OnTestChanged (System.EventArgs.Empty);
		}
	}
}");
		}

		[Test]
		public void TestStaticClassCase ()
		{
			Test<CreateChangedEventAction> (@"static class TestClass
{
	static string test;
	public static string $Test {
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}", @"static class TestClass
{
	static string test;
	public static event System.EventHandler TestChanged;
	static void OnTestChanged (System.EventArgs e)
	{
		var handler = TestChanged;
		if (handler != null)
			handler (null, e);
	}
	public static string Test {
		get {
			return test;
		}
		set {
			test = value;
			OnTestChanged (System.EventArgs.Empty);
		}
	}
}");
		}
	
		[Test]
		public void TestSealedCase ()
		{
			Test<CreateChangedEventAction> (@"sealed class TestClass
{
	string test;
	public string $Test {
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}", @"sealed class TestClass
{
	string test;
	public event System.EventHandler TestChanged;
	void OnTestChanged (System.EventArgs e)
	{
		var handler = TestChanged;
		if (handler != null)
			handler (this, e);
	}
	public string Test {
		get {
			return test;
		}
		set {
			test = value;
			OnTestChanged (System.EventArgs.Empty);
		}
	}
}");
		}

		[Test]
		public void TestWrongLocation()
		{
			TestWrongContext<CreateChangedEventAction> (@"class TestClass
{
	string test;
	public $string Test {
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}");

			TestWrongContext<CreateChangedEventAction> (@"class TestClass
{
	string test;
	public string $FooBar.Test {
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}");

			TestWrongContext<CreateChangedEventAction> (@"class TestClass
{
	string test;
	public string Test ${
		get {
			return test;
		}
		set {
			test = value;
		}
	}
}");
		}

	}
}

