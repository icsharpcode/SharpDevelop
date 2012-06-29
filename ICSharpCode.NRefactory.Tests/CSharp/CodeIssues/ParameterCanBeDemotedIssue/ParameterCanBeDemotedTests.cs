//
// ParameterCouldBeDeclaredWithBaseTypeTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.CodeActions;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
		[TestFixture]
		public class ParameterCanBeDemotedTests : InspectionActionTestBase
		{
		
				[Test]
				public void BasicTest()
				{
						var input = @"
class A
{
	public virtual void Foo() {}
}
class B : A
{
	public virtual void Bar() {}
}
class C
{
	void F(B b)
	{
		b.Foo();
	}
}";
						TestRefactoringContext context;
						var issues = GetIssues(new ParameterCanBeDemotedIssue(), input, out context);
						Assert.AreEqual(1, issues.Count);
			
						CheckFix(context, issues [0], @"
class A
{
	public virtual void Foo() {}
}
class B : A
{
	public virtual void Bar() {}
}
class C
{
	void F(A b)
	{
		b.Foo();
	}
}"
						);
				}

				[Test]
				public void InterfaceTest()
				{
						var input = @"
interface IA
{
	void Foo();
}
class B : IA
{
	public virtual void Foo() {}
	public virtual void Bar() {}
}
class C
{
	void F(B b)
	{
		b.Foo();
	}
}";
						TestRefactoringContext context;
						var issues = GetIssues(new ParameterCanBeDemotedIssue(), input, out context);
						Assert.AreEqual(1, issues.Count);
			
						CheckFix(context, issues [0], @"
interface IA
{
	void Foo();
}
class B : IA
{
	public virtual void Foo() {}
	public virtual void Bar() {}
}
class C
{
	void F(IA b)
	{
		b.Foo();
	}
}"
						);
				}

		string baseInput = @"
interface IA
{
	void Foo();
}
interface IB : IA
{
	void Bar();
}
interface IC : IA
{
	new void Foo();
	void Baz();
}
class D : IB
{
	public void Foo() {}
	public void Bar() {}
}
class E : D, IC
{
	public void Baz() {}
	void IC.Foo() {}
}";
		
		[Test]
		public void FindsTopInterface()
		{
			var input = baseInput + @"
class Test
{
	void F(E d)
	{
		d.Foo();
	}
}";
			TestRefactoringContext context;
			var issues = GetIssues(new ParameterCanBeDemotedIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
			var fixes = issues [0].Actions;
			CheckFix(context, issues [0], baseInput + @"
class Test
{
	void F(IA d)
	{
		d.Foo();
	}
}");
		}
	}
}

