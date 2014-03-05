//
// AstTests.cs
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
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp
{
	[TestFixture]

	public class AstTests
	{
		[Test]
		public void TestDescendants ()
		{
			var tree = SyntaxTree.Parse(@"class Test
{
	void Foo()
	{
		Call1();
		{
			Call2();
		}
		Call3();
	}
}");
			var method = tree.GetNodeAt<MethodDeclaration>(6, 1);
			// Body, Call1, Block, Call2 and Call 3
			Assert.AreEqual(5, method.DescendantNodes().Count(n => n is Statement)); 
		}


		[Test]
		public void TestDescendantsWithPredicate ()
		{
			var tree = SyntaxTree.Parse(@"class Test
{
	void Foo()
	{
		Call1();
		{
			Call2();
		}
		Call3();
	}
}");
			var method = tree.GetNodeAt<MethodDeclaration>(6, 1);
			// Body, Call1, Block and Call 3 - NOT call2
			var childs = method.DescendantNodes(child => !(child is BlockStatement) || (((BlockStatement)child).Parent is MethodDeclaration)).Where(n => n is Statement).ToList();
			Assert.AreEqual(4, childs.Count); 
		}
		
		[Test]
		public void GetNodesBetween()
		{
			var parser = new CSharpParser();
			var expr = parser.ParseExpression("a*b+c*d");
			var nodes = expr.GetNodesBetween(1, 3, 1, 6).ToList();
			Assert.AreEqual(new[] { "b", "+", "c" }, nodes.Select(n => n.ToString()).ToList());
		}
	}
}

