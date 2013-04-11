//
// TestExpressionFormatting.cs
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
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture]
	public class TestExpressionFormatting : TestBase
	{
		[Test]
		public void TestPointerType ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"unsafe class Test
{
	byte *foo;

	Test TestMethod ()
	{
		Test *  *bar;
	}
}", @"unsafe class Test
{
	byte* foo;

	Test TestMethod ()
	{
		Test** bar;
	}
}");
		}

		[Test]
		public void TestNullableType ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"unsafe class Test
{
	byte ?foo;

	Test TestMethod ()
	{
		int ? bar;
	}
}", @"unsafe class Test
{
	byte? foo;

	Test TestMethod ()
	{
		int? bar;
	}
}");
		}



		[Test]
		public void TestArrayInitializer ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"using System.Collections.Generic;

class Test
{
	void Init ()
	{
		var list = new List<int> {
1, 2,
                 3            , 4
		};
	}
}", @"using System.Collections.Generic;

class Test
{
	void Init ()
	{
		var list = new List<int> {
			1, 2,
			3, 4
		};
	}
}");
		}


		[Test]
		public void TestAllowOneLinedArrayInitialziers ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			policy.AllowOneLinedArrayInitialziers = true;
			Test (policy, @"using System.Collections.Generic;

class Test
{
	void Init ()
	{
		var list = new List<int> {1, 2, 3            , 4 };
	}
}", @"using System.Collections.Generic;

class Test
{
	void Init ()
	{
		var list = new List<int> { 1, 2, 3, 4 };
	}
}");
		}
	}
}

