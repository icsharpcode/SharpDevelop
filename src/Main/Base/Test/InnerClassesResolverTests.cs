// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class InnerClassesResolverTests
	{
		#region Test helper methods
		NRefactoryResolverTests nrrt = new NRefactoryResolverTests();
		
		ResolveResult Resolve(string program, string expression, int line)
		{
			return nrrt.Resolve(program, expression, line);
		}
		
		ResolveResult ResolveVB(string program, string expression, int line)
		{
			return nrrt.ResolveVB(program, expression, line);
		}
		#endregion
		
		[Test]
		public void SimpleInnerClass()
		{
			string program = @"class A {
	void Test() {
		
	}
	class B { }
}
";
			ResolveResult result = Resolve(program, "B", 3);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void ReflectionInnerClass()
		{
			string program = @"using System;
class A {
	void Test() {
		
	}
}
";
			ResolveResult result = Resolve(program, "Environment.SpecialFolder", 3);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("System.Environment.SpecialFolder", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void OuterclassPrivateFieldResolveTest()
		{
			string program = @"class A
{
	int myField;
	class B
	{
		void MyMethod(A a)
		{
		
		}
	}
}
";
			ResolveResult result = Resolve(program, "a", 8);
			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result is LocalResolveResult, "result is LocalResolveResult");
			ArrayList arr = result.GetCompletionData(nrrt.lastPC);
			Assert.IsNotNull(arr, "arr");
			foreach (object o in arr) {
				if (o is IField) {
					Assert.AreEqual("myField", ((IField)o).Name);
					return;
				}
			}
			Assert.Fail("private field not visible from inner class");
		}
		
		[Test]
		public void InheritedInnerClass()
		{
			string program = @"class A {
	class B { }
}
class C : A {
	void Main() {
		
	}
}
";
			ResolveResult result = Resolve(program, "B", 6);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
			
			result = Resolve(program, "C.B", 6);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
			
			result = Resolve(program, "C", 6);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("C", result.ResolvedType.FullyQualifiedName);
			foreach (object o in result.GetCompletionData(nrrt.lastPC)) {
				if (o is IClass) {
					Assert.AreEqual("A.B", ((IClass)o).FullyQualifiedName);
					return;
				}
			}
			Assert.Fail("Inherited inner class not visible.");
		}
	}
}
