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
		public void TestNoUneccessaryChangesToImplicitArrayInitializer ()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			const string input = @"using System.Collections.Generic;

class Test
{
	void Init ()
	{
		var list = { 1, 2 };
	}
}";

			TestNoUnnecessaryChanges(policy, input);
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

		[Test]
		public void TestMemberReferenceFormatting ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"
class Test
{
	void Init ()
	{
		System.   Console     .WriteLine ();
	}
}", @"
class Test
{
	void Init ()
	{
		System.Console.WriteLine ();
	}
}");
		}


		[Test]
		public void AnonymousBlocksAsParameter ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();

			Test (policy, @"class Test
{
	Test TestMethod ()
	{
		FooBar (
			foo,
			delegate {
Bar ();
}, 
test
);
	}
}", @"class Test
{
	Test TestMethod ()
	{
		FooBar (
			foo,
			delegate {
				Bar ();
			}, 
			test
		);
	}
}");
		}


		[Test]
		public void TestAnonymousMethodBlocks ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();

			policy.AnonymousMethodBraceStyle = BraceStyle.NextLine;

			Test (policy, @"class Test
{
	Test TestMethod ()
	{
		Action act = delegate{};
	}
}", @"class Test
{
	Test TestMethod ()
	{
		Action act = delegate
		{
		};
	}
}");
		}

		[Test]
		public void TestLambdaBraceStyle ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono ();

			policy.AnonymousMethodBraceStyle = BraceStyle.NextLine;

			Test (policy, @"class Test
{
	Test TestMethod ()
	{
		Action act = () => {};
	}
}", @"class Test
{
	Test TestMethod ()
	{
		Action act = () =>
		{
		};
	}
}");
		}

		[Test]
		public void TestBinaryExpressionsInInitializer ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"class Test
{
	private void Foo ()
	{
		int x =    5+ 
23 - 
43*12 - 44*
5;
	}
}
", @"class Test
{
	private void Foo ()
	{
		int x = 5 +
		        23 -
		        43 * 12 - 44 *
		        5;
	}
}
");
		}

		[Test]
		public void TestBinaryExpressionsInInitializerBreakAfterAssign ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"class Test
{
	private void Foo ()
	{
		int x =
 5+ 
23 
- 43
*12 
 - 
44*
5;
	}
}
", @"class Test
{
	private void Foo ()
	{
		int x =
			5 +
			23
			- 43
			* 12
			-
			44 *
			5;
	}
}
");
			}

		[Test]
		public void TestBinaryExpressionsInInitializerWithParenthesizedExpr ()
		{
			var policy = FormattingOptionsFactory.CreateMono ();
			Test (policy, @"class Test
{
	private void Foo ()
	{
		int x = 5
		+ (2343 *  12)            - (
		44 * 5
		);
	}
}
", @"class Test
{
	private void Foo ()
	{
		int x = 5
		        + (2343 * 12) - (
		            44 * 5
		        );
	}
}
");
		}

		[Test]
		public void TestArrayInitializerFormattingBugSimple ()
		{
			var policy = FormattingOptionsFactory.CreateSharpDevelop ();
			Test (policy, @"class Test
{
	static readonly AstNode forPattern =
		new Choice {
			new BinaryOperatorExpression(
				PatternHelper.OptionalParentheses(
					new AnyNode(""upperBound"")
				)
			)
		};
}
", @"class Test
{
	static readonly AstNode forPattern =
		new Choice {
			new BinaryOperatorExpression(
				PatternHelper.OptionalParentheses(
					new AnyNode(""upperBound"")
				)
			)
		};
}
");
		}
	}
}

