//
// LocalDeclarationSpaceTests.cs
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
using System;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Analysis
{
	[TestFixture]
	public class LocalDeclarationSpaceTests
	{
		LocalDeclarationSpace parent;

		LocalDeclarationSpace child;

		[SetUp]
		public void SetUp()
		{
			parent = new LocalDeclarationSpace();
			child = new LocalDeclarationSpace();
		}

		[Test]
		public void AddChildSpace()
		{
			parent.AddChildSpace(child);
			Assert.AreEqual(child.Parent, parent, "The parent was not set.");
			Assert.That(parent.Children.Contains(child), "The child was not added to the parents children.");
		}

		[Test]
		public void AddChildThrowsOnDuplicateChild ()
		{
			parent.AddChildSpace(child);
			Assert.Throws<InvalidOperationException>(delegate {
				parent.AddChildSpace(child);
			});
		}

		[Test]
		public void ContainsNameSimpleVariableDeclaration()
		{
			parent.AddChildSpace(child);

			var input = @"
class Foo
{
	void Bar()
	{
		int $i;
	}
}";
			var context = TestRefactoringContext.Create(input);
			child.AddDeclaration("i", context.GetNode<VariableInitializer>());
			
			Assert.That(child.ContainsName("i", false), "The declaration was not added to child correctly.");
			Assert.That(!parent.ContainsName("i", false), "parent incorrectly contained the declaration.");
			Assert.That(parent.ContainsName("i", true), "parent did not contain the declaration, event though it should have.");
		}

		[Test]
		public void IsNameTakenSimpleVariableDeclarationInParent()
		{
			parent.AddChildSpace(child);

			var input = @"
class Foo
{
	void Bar()
	{
		int $i;
	}
}";
			var context = TestRefactoringContext.Create(input);
			parent.AddDeclaration("i", context.GetNode<VariableInitializer>());

			Assert.That(parent.IsNameUsed("i"), "The declaration was not added to parent correctly.");
			Assert.That(child.IsNameUsed("i"), "child did not contain the declaration, event though it should have.");

			Assert.That(!parent.IsNameUsed("j"), "parent contained a non-existent declaration.");
			Assert.That(!child.IsNameUsed("j"), "parent contained a non-existent declaration.");
		}

		[Test]
		public void IsNameTakenMultipleChildSpaces()
		{
			parent.AddChildSpace(child);
			var child2 = new LocalDeclarationSpace();
			parent.AddChildSpace(child2);

			var input = @"
class Foo
{
	void Bar()
	{
		{
			int $i;
		}
	}
}";
			var context = TestRefactoringContext.Create(input);
			child2.AddDeclaration("i", context.GetNode<VariableInitializer>());

			Assert.That(parent.IsNameUsed("i"), "The declaration was not added to parent correctly.");
			Assert.That(!child.IsNameUsed("i"), "child contained the declaration, event though it shouldn't.");
			Assert.That(child2.IsNameUsed("i"), "child2 did not contain the declaration, event though it should have.");

			Assert.That(!parent.IsNameUsed("j"), "parent contained a non-existent declaration.");
			Assert.That(!child.IsNameUsed("j"), "parent contained a non-existent declaration.");
			Assert.That(!child2.IsNameUsed("j"), "parent contained a non-existent declaration.");
		}

		[Test]
		public void GetNameDeclarationsNoDeclarations()
		{
			var node = new IdentifierExpression();
			child.AddDeclaration("blah", node);

			var declarations = child.GetNameDeclarations("notBlah");
			Assert.NotNull(declarations, "declarations");
			Assert.AreEqual(0, declarations.Count(), "Wrong declaration count");
		}

		[Test]
		public void GetNameDeclarationsInChildSpace()
		{
			var child2 = new LocalDeclarationSpace();
			parent.AddChildSpace(child);
			parent.AddChildSpace(child2);

			var node1 = new IdentifierExpression();
			child.AddDeclaration("blah", node1);

			var node2 = new IdentifierExpression();
			child.AddDeclaration("blah", node2);

			var declarations = parent.GetNameDeclarations("blah").ToList();
			Assert.NotNull(declarations, "declarations");
			Assert.AreEqual(2, declarations.Count, "Wrong declaration count");
			Assert.That(declarations.Contains(node1), "node1 was not one of the declarations");
			Assert.That(declarations.Contains(node2), "node2 was not one of the declarations");
		}
	}
}

