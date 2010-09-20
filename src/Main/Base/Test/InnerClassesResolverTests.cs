// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
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
		
		T Resolve<T>(string program, string expression, int line) where T : ResolveResult
		{
			return nrrt.Resolve<T>(program, expression, line);
		}
		
		T ResolveVB<T>(string program, string expression, int line) where T : ResolveResult
		{
			return nrrt.ResolveVB<T>(program, expression, line);
		}
		
		List<ICompletionEntry> CtrlSpace(string program, int line)
		{
			return nrrt.CtrlSpaceResolveCSharp(program, line, ExpressionContext.Default);
		}
		#endregion
		
		#region Ctrl-Space tests
		[Test]
		public void CtrlSpaceIncludesInnerClass()
		{
			string program = @"class A {
		class Inner { }
		
	}";
			Assert.IsTrue(IsInnerClassVisible(CtrlSpace(program, 3)));
		}
		
		[Test]
		public void CtrlSpaceIncludesInheritedInnerClass()
		{
			string program = @"class A : Outer {
		
	}
	class Outer { protected class Inner { } }
";
			Assert.IsTrue(IsInnerClassVisible(CtrlSpace(program, 2)));
		}
		
		[Test]
		public void CtrlSpaceDoesNotIncludeInheritedPrivateInnerClass()
		{
			string program = @"class A : Outer {
		
	}
	class Outer { class Inner { } }
";
			Assert.IsFalse(IsInnerClassVisible(CtrlSpace(program, 2)));
		}
		
		[Test]
		public void CtrlSpaceIncludesInnerClassFromOtherPart()
		{
			string program = @"partial class A {
		
	}
	partial class A { class Inner { } }
";
			Assert.IsTrue(IsInnerClassVisible(CtrlSpace(program, 2)));
		}
		#endregion
		
		[Test]
		public void InnerClassTest()
		{
			string program = @"using System;
class A {
	
}
";
			ResolveResult result = Resolve<TypeResolveResult>(program, "Environment.SpecialFolder", 3);
			Assert.AreEqual("System.Environment.SpecialFolder", result.ResolvedType.FullyQualifiedName);
		}
		
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
		public void InnerClassWithStaticFieldOfSameType()
		{
			string program = @"class A {
	void Test() {
		
	}
	class B {
		public static B Instance;
	}
}
";
			ResolveResult result = Resolve(program, "B.Instance", 3);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassWithStaticFieldOfSameTypeInPartialClass1()
		{
			string program = @"partial class A {
	void Test() {
		
	}
}
partial class A {
	class B {
		public static B Instance;
	}
}
";
			ResolveResult result = Resolve(program, "B.Instance", 3);
			Assert.IsTrue(result is MemberResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassWithStaticFieldOfSameTypeInPartialClass2()
		{
			string program = @"partial class A {
	void Test() {
		
	}
	class B {
		public static B Instance;
	}
}
partial class A {
}
";
			ResolveResult result = Resolve(program, "B.Instance", 3);
			Assert.IsTrue(result is MemberResolveResult);
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
		public void OuterclassPrivateFieldCtrlSpaceTest()
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
			var arr = result.GetCompletionData(nrrt.lastPC);
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
		public void OuterclassStaticFieldResolveTest()
		{
			string program = @"class A
{
	static int myField;
	class B
	{
		void MyMethod()
		{
		
		}
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "myField", 8);
			Assert.AreEqual("A.myField", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void OuterclassStaticMethodCallResolveTest()
		{
			string program = @"class A
{
	static void Test(int arg);
	class B
	{
		void MyMethod()
		{
		
		}
	}
}
";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "Test(4)", 8);
			Assert.AreEqual("A.Test", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void InheritedInnerClass()
		{
			string program = @"class A {
	protected class B { }
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
		
		[Test]
		public void NestedClassHidingHidesAllMethods()
		{
			string program = @"using System;
class A {
	static void Test(int arg) {}
	static void Test(string arg) {}
	class B {
		void MyMethod() {
		
		}
		static void Test(long arg) {}
	}
}";
			MemberResolveResult result = Resolve<MemberResolveResult>(program, "Test(4)", 7);
			Assert.AreEqual("A.B.Test", result.ResolvedMember.FullyQualifiedName);
		}
		
		[Test]
		public void NestedInnerClasses()
		{
			string program = @"using System;
public sealed class GL {
	void Test() {
		
	}
	
	public class Enums
	{
		public enum BeginMode {QUADS, LINES }
	}
}
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "GL.Enums.BeginMode", 4);
			Assert.AreEqual("GL.Enums.BeginMode", trr.ResolvedClass.FullyQualifiedName);
			
			trr = Resolve<TypeResolveResult>(program, "Enums.BeginMode", 4);
			Assert.AreEqual("GL.Enums.BeginMode", trr.ResolvedClass.FullyQualifiedName);
			
			MemberResolveResult mrr = Resolve<MemberResolveResult>(program, "GL.Enums.BeginMode.LINES", 4);
			Assert.AreEqual("GL.Enums.BeginMode.LINES", mrr.ResolvedMember.FullyQualifiedName);
			
			mrr = Resolve<MemberResolveResult>(program, "Enums.BeginMode.LINES", 4);
			Assert.AreEqual("GL.Enums.BeginMode.LINES", mrr.ResolvedMember.FullyQualifiedName);
			
			// ensure that GetClass works correctly:
			IClass c = trr.ResolvedClass.ProjectContent.GetClass("GL.Enums.BeginMode", 0);
			Assert.IsNotNull(c);
			Assert.AreEqual("GL.Enums.BeginMode", c.FullyQualifiedName);
		}
		
		[Test]
		public void DoNotShowInaccessibleInnerClass()
		{
			string program = @"using System;
class C {
	
}
class Outer { private class Inner { } }
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "Outer", 3);
			var l = trr.GetCompletionData(trr.ResolvedClass.ProjectContent);
			Assert.IsFalse(IsInnerClassVisible(l));
		}
		
		[Test]
		public void ShowProtectedInnerClassFromDerivedClass()
		{
			string program = @"using System;
class Derived : Outer {
	
}
class Outer { protected class Inner {} }
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "Outer", 3);
			var l = trr.GetCompletionData(trr.ResolvedClass.ProjectContent);
			Assert.IsTrue(IsInnerClassVisible(l));
		}
		
		[Test]
		public void ShowProtectedInnerClassThroughForeignDerivedClass()
		{
			string program = @"using System;
class Derived : Outer {
	
}
class Derived2 : Outer { }
class Outer { protected class Inner {} }
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "Derived2", 3);
			var l = trr.GetCompletionData(trr.ResolvedClass.ProjectContent);
			Assert.IsTrue(IsInnerClassVisible(l));
		}
		
		[Test]
		public void DoNotShowProtectedInnerClassThroughUnrelatedClass()
		{
			string program = @"using System;
class Unrelated {
	
}
class Derived : Outer { }
class Outer { protected class Inner {} }
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "Derived", 3);
			var l = trr.GetCompletionData(trr.ResolvedClass.ProjectContent);
			Assert.IsFalse(IsInnerClassVisible(l));
		}
		
		bool IsInnerClassVisible(IEnumerable l)
		{
			foreach (object o in l) {
				IClass c = o as IClass;
				if (c != null && c.Name == "Inner")
					return true;
			}
			return false;
		}
		
		[Test]
		public void GenericInnerClassOrNonGenericOuterClass()
		{
			string program = @"using System;
class Test {
	
	class TheClass<T> {}
}
class TheClass { }
";
			TypeResolveResult trr = Resolve<TypeResolveResult>(program, "TheClass<string>", 3);
			Assert.AreEqual("Test.TheClass", trr.ResolvedClass.FullyQualifiedName);
		}
		
		[Test]
		public void GenericInnerClassOrNonGenericOuterClass2()
		{
			string program = @"using System;
class Test {
	TheClass<string> x;
	
	class TheClass<T> {}
}
class TheClass { }
";
			MemberResolveResult rr = Resolve<MemberResolveResult>(program, "x", 3);
			Assert.AreEqual("Test.TheClass", rr.ResolvedType.FullyQualifiedName);
		}
		
		[Test]
		public void GenericInnerClassOrNonGenericInnerClass()
		{
			string program = @"using System;
class Test {
	TheClass<string> x1;
	TheClass x2;
	Test.TheClass<string> y1;
	Test.TheClass y2;
	global::Test.TheClass<string> z1;
	global::Test.TheClass z2;
	
	public class TheClass { }
	public class TheClass<T> {}
}
";
			MemberResolveResult rr = Resolve<MemberResolveResult>(program, "x1", 3);
			Assert.AreEqual(1, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
			
			rr = Resolve<MemberResolveResult>(program, "y1", 3);
			Assert.AreEqual(1, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
			
			rr = Resolve<MemberResolveResult>(program, "z1", 3);
			Assert.AreEqual(1, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
			
			rr = Resolve<MemberResolveResult>(program, "x2", 3);
			Assert.AreEqual(0, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
			
			rr = Resolve<MemberResolveResult>(program, "y2", 3);
			Assert.AreEqual(0, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
			
			rr = Resolve<MemberResolveResult>(program, "z2", 3);
			Assert.AreEqual(0, rr.ResolvedType.GetUnderlyingClass().TypeParameters.Count);
		}
		
		[Test]
		public void SimpleInnerClassInStruct()
		{
			string program = @"struct A {
	void Test() {
		
	}
	class B { }
}
";
			ResolveResult result = Resolve(program, "B", 3);
			Assert.IsTrue(result is TypeResolveResult);
			Assert.AreEqual("A.B", result.ResolvedType.FullyQualifiedName);
		}
	}
}
