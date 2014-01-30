//
// LocalVariableLocalDeclarationSpaceVisitor.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2013 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.Analysis
{
	[TestFixture]
	public class LocalDeclarationSpaceVisitorTests
	{
		static LocalDeclarationSpaceVisitor GetVisitor(out TestRefactoringContext context, string input)
		{
			context = TestRefactoringContext.Create(input);
			var visitor = new LocalDeclarationSpaceVisitor();
			context.RootNode.AcceptVisitor(visitor);
			return visitor;
		}

		[Test]
		public void SimpleDeclaration()
		{
			TestRefactoringContext context;
			var visitor = GetVisitor(out context, @"
class A
{
	void B()
	{
		int $c = 1;
	}
}");
			var declarationSpace = visitor.GetDeclarationSpace(context.GetNode<VariableInitializer>());
			Assert.That(declarationSpace.ContainsName("c", false), "The variable name was missing in the declaration space.");
		}

		[Test]
		public void FieldDeclaration()
		{
			TestRefactoringContext context;
			var visitor = GetVisitor(out context, @"
class A
{
	int $c = 1;
	void B()
	{
	}
}");
			var declarationSpace = visitor.GetDeclarationSpace(context.GetNode<VariableInitializer>());
			Assert.IsNull(declarationSpace);
		}

		[Test]
		public void MethodWithParameters()
		{
			TestRefactoringContext context;
			var visitor = GetVisitor(out context, @"
class A
{
	void B(int c)
	{
		int $d = 0;
	}
}");
			var bodyDeclarationSpace = visitor.GetDeclarationSpace(context.GetNode<VariableInitializer>());
			Assert.That(bodyDeclarationSpace.ContainsName("d", false), "The variable 'd' was not in the declaration space.");
			Assert.That(!bodyDeclarationSpace.ContainsName("c", false), "The variable 'c' was in the wrong declaration space.");

			var methodDeclarationSpace = bodyDeclarationSpace.Parent;
			Assert.NotNull(methodDeclarationSpace, "The declaration space did not have a parent.");
			Assert.That(methodDeclarationSpace.ContainsName("c", false), "The variable 'c' was not in the declaration space.");
		}

		[Test]
		public void CustomEventDeclaration()
		{
			TestRefactoringContext context;
			var visitor = GetVisitor(out context, @"
class A
{
	event Action SomethingHappened
	{
		add {
			int $i = 1;
		}
		remove {
			int i = 2;
		}
	}
}");
			var bodyDeclarationSpace = visitor.GetDeclarationSpace(context.GetNode<VariableInitializer>());
			Assert.That(bodyDeclarationSpace.ContainsName("i", false), "The variable 'i' was not in the declaration space.");
			Assert.AreEqual(2, bodyDeclarationSpace.Parent.Children.Count, "Wrong number of child declaration spaces in the event declaration");
		}
	}
}

