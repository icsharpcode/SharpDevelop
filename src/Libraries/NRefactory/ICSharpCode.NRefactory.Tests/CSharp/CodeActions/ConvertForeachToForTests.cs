// 
// ConvertForeachToForTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc.
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
	public class ConvertForeachToForTests : ContextActionTestBase
	{
		[Test]
		public void TestArray ()
		{
			string result = RunContextAction (
				new ConvertForeachToForAction (),
				"using System;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	void Test (string[] args)" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		$foreach (var v in args) {" + Environment.NewLine +
				"			Console.WriteLine (v);" + Environment.NewLine +
				"		}" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}"
			);
			
			Assert.AreEqual (
				"using System;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	void Test (string[] args)" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		for (int i = 0; i < args.Length; i++) {" + Environment.NewLine +
				"			var v = args [i];" + Environment.NewLine +
				"			Console.WriteLine (v);" + Environment.NewLine +
				"		}" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}", result);
		}
		
		[Test]
		public void TestListOfT ()
		{
			string result = RunContextAction (
				new ConvertForeachToForAction (),
				"using System;" + Environment.NewLine +
				"using System.Collections.Generic;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	void Test (List<string> args)" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		$foreach (var v in args) {" + Environment.NewLine +
				"			Console.WriteLine (v);" + Environment.NewLine +
				"		}" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}"
			);
			
			Assert.AreEqual (
				"using System;" + Environment.NewLine +
				"using System.Collections.Generic;" + Environment.NewLine +
				"class TestClass" + Environment.NewLine +
				"{" + Environment.NewLine +
				"	void Test (List<string> args)" + Environment.NewLine +
				"	{" + Environment.NewLine +
				"		for (int i = 0; i < args.Count; i++) {" + Environment.NewLine +
				"			var v = args [i];" + Environment.NewLine +
				"			Console.WriteLine (v);" + Environment.NewLine +
				"		}" + Environment.NewLine +
				"	}" + Environment.NewLine +
				"}", result);
		}

		/// <summary>
		/// Bug 9876 - Convert to for loop created invalid code if iteration variable is called i
		/// </summary>
		[Test]
		public void TestBug9876 ()
		{
			Test<ConvertForeachToForAction> (@"class TestClass
{
	void TestMethod ()
	{
		$foreach (var i in new[] { 1, 2, 3 }) {
			Console.WriteLine (i);
		}
	}
}", @"class TestClass
{
	void TestMethod ()
	{
		var list = new[] {
			1,
			2,
			3
		};
		for (int j = 0; j < list.Length; j++) {
			var i = list [j];
			Console.WriteLine (i);
		}
	}
}");
		}
	
		[Test]
		public void TestOptimizedForLoop ()
		{
			Test<ConvertForeachToForAction>(@"
class Test
{
	void Foo (object[] o)
	{
		$foreach (var p in o) {
			System.Console.WriteLine (p);
		}
	}
}", @"
class Test
{
	void Foo (object[] o)
	{
		for (int i = 0, oLength = o.Length; i < oLength; i++) {
			var p = o [i];
			System.Console.WriteLine (p);
		}
	}
}", 1);
		}

		[Test]
		public void TestEnumerableConversion ()
		{
			Test<ConvertForeachToForAction>(@"
using System;
using System.Collections.Generic;

class Test
{
	public void Foo (IEnumerable<string> bar)
	{
		$foreach (var b in bar) {
			Console.WriteLine (b);
		}
	}
}", @"
using System;
using System.Collections.Generic;

class Test
{
	public void Foo (IEnumerable<string> bar)
	{
		for (var i = bar.GetEnumerator (); i.MoveNext ();) {
			var b = i.Current;
			Console.WriteLine (b);
		}
	}
}");
		}
	}
}