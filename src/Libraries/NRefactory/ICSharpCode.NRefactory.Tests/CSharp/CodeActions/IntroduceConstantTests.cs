// 
// IntroduceConstantTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
	public class IntroduceConstantTests : ContextActionTestBase
	{
		[Test]
		public void TestLocalConstant ()
		{
			Test<IntroduceConstantAction> (@"class TestClass
{
	public void Hello ()
	{
		System.Console.WriteLine ($""Hello World"");
	}
}", @"class TestClass
{
	public void Hello ()
	{
		const string helloWorld = ""Hello World"";
		System.Console.WriteLine (helloWorld);
	}
}");
		}

		[Test]
		public void TestLocalConstantHexNumber ()
		{
			Test<IntroduceConstantAction> (@"class TestClass
{
	public void Hello ()
	{
		System.Console.WriteLine ($0xAFFE);
	}
}", @"class TestClass
{
	public void Hello ()
	{
		const int i = 0xAFFE;
		System.Console.WriteLine (i);
	}
}");
		}

		[Test]
		public void TestFieldConstant ()
		{
			Test<IntroduceConstantAction> (@"class TestClass
{
	public void Hello ()
	{
		System.Console.WriteLine ($""Hello World"");
	}
}", @"class TestClass
{
	const string helloWorld = ""Hello World"";
	public void Hello ()
	{
		System.Console.WriteLine (helloWorld);
	}
}", 1);
		}

		[Test]
		public void TestLocalConstantReplaceAll ()
		{
			Test<IntroduceConstantAction> (@"class TestClass
{
	public void Hello ()
	{
		System.Console.WriteLine ($""Hello World"");
		System.Console.WriteLine (""Hello World"");
		System.Console.WriteLine (""Hello World"");
	}
}", @"class TestClass
{
	public void Hello ()
	{
		const string helloWorld = ""Hello World"";
		System.Console.WriteLine (helloWorld);
		System.Console.WriteLine (helloWorld);
		System.Console.WriteLine (helloWorld);
	}
}", 2);
		}
	
		[Test]
		public void TestAlreadyConstant ()
		{
			TestWrongContext<IntroduceConstantAction> (@"class TestClass
{
	public void Hello ()
	{
		const int i = $0xAFFE;
	}
}");
		}

		[Test]
		public void TestAlreadyConstantCase2 ()
		{
			TestWrongContext<IntroduceConstantAction> (@"class TestClass
{
	const int i = $0xAFFE;
}");
		}

		[Test]
		public void TestIntroduceConstantInInitializer ()
		{
			Test<IntroduceConstantAction> (@"class TestClass
{
	readonly int foo = new Foo ($5);
}", @"class TestClass
{
	const int i = 5;
	readonly int foo = new Foo (i);
}");
		}
	}
}