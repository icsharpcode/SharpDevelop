// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

using ICSharpCode.SharpDevelop.Tests;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public sealed class ClassInheritanceTreeTests
	{
		#region Test helper methods
		
		readonly NRefactoryResolverTests helper = new NRefactoryResolverTests();
		ICompilationUnit cu;
		IClass objectClass, baseClass, derivedClass;
		
		void Prepare(string code, string baseClassName, string derivedClassName)
		{
			cu = helper.Parse("a.cs", code);
			
			objectClass = cu.ProjectContent.GetClass("System.Object", 0);
			Assert.That(objectClass, Is.Not.Null, "Could not find class for System.Object in project content.");
			
			baseClass = cu.ProjectContent.GetClass(baseClassName, 0);
			Assert.That(baseClass, Is.Not.Null, "Could not find class for '" + baseClassName + "' in project content.");
			
			derivedClass = cu.ProjectContent.GetClass(derivedClassName, 0);
			Assert.That(derivedClass, Is.Not.Null, "Could not find class for '" + derivedClassName + "' in project content.");
		}
		
		void CheckDerivedClassInheritanceTree()
		{
			Assert.That(derivedClass.ClassInheritanceTree, Is.EqualTo(
				new [] {derivedClass, baseClass, objectClass}
			));
		}
		
		#endregion
		
		const string CodeSameNamespace = @"
namespace A {
	class Base {
	}
	class Derived : Base {
	}
}
";
		
		[Test]
		public void DerivedClassInheritanceTreeSameNamespace()
		{
			Prepare(CodeSameNamespace, "A.Base", "A.Derived");
			CheckDerivedClassInheritanceTree();
		}
		
		[Test]
		public void DerivedClassInheritanceTreeAfterGetFullyQualifiedNameSameNamespace()
		{
			Prepare(CodeSameNamespace, "A.Base", "A.Derived");
			Assert.That(baseClass.FullyQualifiedName, Is.EqualTo("A.Base"));
			Assert.That(derivedClass.FullyQualifiedName, Is.EqualTo("A.Derived"));
			CheckDerivedClassInheritanceTree();
		}
		
		[Test]
		public void DerivedClassInheritanceTreeAfterCheckBaseTypesSameNamespace()
		{
			Prepare(CodeSameNamespace, "A.Base", "A.Derived");
			Assert.That(derivedClass.BaseTypes, Is.EquivalentTo(new [] {baseClass.DefaultReturnType}));
			CheckDerivedClassInheritanceTree();
		}
		
		
		
		const string CodeDifferentNamespaceUsing = @"
namespace A {
	class Base {
	}
}
namespace B {
	using A;
	class Derived : Base {
	}
}
";
		
		[Test]
		public void DerivedClassInheritanceTreeDifferentNamespaceUsing()
		{
			Prepare(CodeDifferentNamespaceUsing, "A.Base", "B.Derived");
			CheckDerivedClassInheritanceTree();
		}
		
		[Test]
		public void DerivedClassInheritanceTreeAfterGetFullyQualifiedNameDifferentNamespaceUsing()
		{
			Prepare(CodeDifferentNamespaceUsing, "A.Base", "B.Derived");
			Assert.That(baseClass.FullyQualifiedName, Is.EqualTo("A.Base"));
			Assert.That(derivedClass.FullyQualifiedName, Is.EqualTo("B.Derived"));
			CheckDerivedClassInheritanceTree();
		}
		
		[Test]
		public void DerivedClassInheritanceTreeAfterCheckBaseTypesDifferentNamespaceUsing()
		{
			Prepare(CodeDifferentNamespaceUsing, "A.Base", "B.Derived");
			Assert.That(derivedClass.BaseTypes, Is.EquivalentTo(new [] {baseClass.DefaultReturnType}));
			CheckDerivedClassInheritanceTree();
		}
	}
}
