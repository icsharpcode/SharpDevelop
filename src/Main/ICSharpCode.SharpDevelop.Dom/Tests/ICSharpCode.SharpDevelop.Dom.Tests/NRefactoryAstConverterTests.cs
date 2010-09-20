// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class NRefactoryAstConverterTests
	{
		ICompilationUnit Parse(string code, SupportedLanguage language, params IProjectContent[] references)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			foreach (var reference in references) {
				pc.AddReferencedContent(reference);
			}
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc, language);
			using (IParser p = ParserFactory.CreateParser(language, new StringReader(code))) {
				p.ParseMethodBodies = false;
				p.Parse();
				
				visitor.Specials = p.Lexer.SpecialTracker.CurrentSpecials;
				visitor.VisitCompilationUnit(p.CompilationUnit, null);
			}
			return visitor.Cu;
		}
		
		ICompilationUnit Parse(string code)
		{
			return Parse(code, SupportedLanguage.CSharp);
		}
		
		string SurroundWithSummaryTags(string text)
		{
			return " <summary>\r\n " + text + "\r\n </summary>\r\n";
		}
		
		[Test]
		public void FindDocumentationComment()
		{
			ICompilationUnit cu = Parse(@"
using System;

namespace X
{
	/// <summary>
	/// This is the comment
	/// </summary>
	public class A
	{
	}
}
");
			Assert.AreEqual(SurroundWithSummaryTags("This is the comment"), cu.Classes[0].Documentation);
		}
		
		[Test]
		public void FindDocumentationCommentAboveAttribute()
		{
			ICompilationUnit cu = Parse(@"
using System;

namespace X
{
	/// <summary>
	/// This is the comment
	/// </summary>
	[SomeAttribute]
	public class A
	{
	}
}
");
			Assert.AreEqual(SurroundWithSummaryTags("This is the comment"), cu.Classes[0].Documentation);
		}
		
		[Test]
		public void FindDocumentationCommentAboveAttribute2()
		{
			ICompilationUnit cu = Parse(@"
using System;

namespace X
{
	/// <summary>
	/// This is the comment
	/// </summary>
	[SomeAttribute] // a comment on the attribute
	public class A
	{
	}
}
");
			Assert.AreEqual(SurroundWithSummaryTags("This is the comment"), cu.Classes[0].Documentation);
		}
		
		[Test]
		public void FindDocumentationCommentAboveAttributeInRegion()
		{
			ICompilationUnit cu = Parse(@"
using System;

namespace X
{
	/// <summary>
	/// This is the comment
	/// </summary>
	#region R
	[SomeAttribute]
	#endregion
	public class A
	{
	}
}
");
			Assert.AreEqual(SurroundWithSummaryTags("This is the comment"), cu.Classes[0].Documentation);
		}
		
		[Test]
		public void GenericMethodWithConstraintsTest()
		{
			// Test that constaints can reference other type parameters.
			ICompilationUnit cu = Parse(@"
using System;

class X {
	public static A Method<A, B>(A p1, B p2) where A : IComparable<B> where B : IComparable<A> { }
}
");
			IMethod method = cu.Classes[0].Methods[0];
			Assert.AreEqual(2, method.TypeParameters.Count);
			
			ITypeParameter a = method.TypeParameters[0];
			ITypeParameter b = method.TypeParameters[1];
			
			Assert.AreSame(a, method.ReturnType.CastToGenericReturnType().TypeParameter);
			Assert.AreSame(a, method.Parameters[0].ReturnType.CastToGenericReturnType().TypeParameter);
			Assert.AreSame(b, method.Parameters[1].ReturnType.CastToGenericReturnType().TypeParameter);
			
			Assert.AreEqual(1, a.Constraints.Count);
			ConstructedReturnType crt = a.Constraints[0].CastToConstructedReturnType();
			Assert.AreEqual("IComparable", crt.Name);
			Assert.AreSame(b, crt.TypeArguments[0].CastToGenericReturnType().TypeParameter);
			
			Assert.AreEqual(1, b.Constraints.Count);
			crt = b.Constraints[0].CastToConstructedReturnType();
			Assert.AreEqual("IComparable", crt.Name);
			Assert.AreSame(a, crt.TypeArguments[0].CastToGenericReturnType().TypeParameter);
		}
		
		[Test]
		public void StaticClassTest()
		{
			ICompilationUnit cu = Parse(@"
using System;

static class X {}
");
			IClass c = cu.Classes[0];
			Assert.IsTrue(c.IsAbstract, "class should be abstract");
			Assert.IsTrue(c.IsSealed, "class should be sealed");
			Assert.IsTrue(c.IsStatic, "class should be static");
		}
		
		[Test]
		public void IndexerDefaultNameTest()
		{
			ICompilationUnit cu = Parse(@"
class X {
	public int this[int index] {
		get { return 0; }
	}
}
");
			IProperty p = cu.Classes[0].Properties[0];
			Assert.IsTrue(p.IsIndexer, "IsIndexer must be true");
			Assert.AreEqual("Item", p.Name);
			Assert.AreEqual(1, p.Parameters.Count);
		}
		
		[Test]
		public void IndexerNonDefaultNameTest()
		{
			ICompilationUnit cu = Parse(@"
using System.Runtime.CompilerServices;
class X {
	[IndexerName(""Foo"")]
	public int this[int index] {
		get { return 0; }
	}
}
", SupportedLanguage.CSharp, SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			IProperty p = cu.Classes[0].Properties[0];
			Assert.IsTrue(p.IsIndexer, "IsIndexer must be true");
			Assert.AreEqual("Foo", p.Name);
			Assert.AreEqual(1, p.Parameters.Count);
		}
		
		[Test]
		public void GenericInnerClassTest()
		{
			ICompilationUnit cu = Parse(@"
using System;

class Outer<T1> where T1 : IDisposable {
	class Inner<T2> where T2 : T1 {
		public static void Method<T3>(T1 p1, T2 p2, T3 p3) where T3 : T2 { }
	}
}
");
			IClass outer = cu.Classes[0];
			Assert.AreEqual(1, outer.TypeParameters.Count);
			
			IClass inner = outer.InnerClasses[0];
			Assert.AreEqual(2, inner.TypeParameters.Count);
			Assert.AreEqual("T1", inner.TypeParameters[0].Name);
			Assert.AreSame(inner, inner.TypeParameters[0].Class);
			Assert.AreEqual("T2", inner.TypeParameters[1].Name);
			Assert.AreSame(inner.TypeParameters[0], inner.TypeParameters[1].Constraints[0].CastToGenericReturnType().TypeParameter);
			Assert.AreEqual("IDisposable", inner.TypeParameters[0].Constraints[0].Name);
			
			IMethod method = inner.Methods.Single(m => m.Name == "Method");
			Assert.AreEqual(1, method.TypeParameters.Count);
			Assert.AreSame(inner.TypeParameters[0], method.Parameters[0].ReturnType.CastToGenericReturnType().TypeParameter);
			Assert.AreSame(inner.TypeParameters[1], method.Parameters[1].ReturnType.CastToGenericReturnType().TypeParameter);
			Assert.AreSame(method.TypeParameters[0], method.Parameters[2].ReturnType.CastToGenericReturnType().TypeParameter);
			
			Assert.AreSame(inner.TypeParameters[1], method.TypeParameters[0].Constraints[0].CastToGenericReturnType().TypeParameter);
		}
		
		[Test]
		public void DefaultConstructorTest()
		{
			ICompilationUnit cu = Parse("class X { }", SupportedLanguage.CSharp, SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			Assert.AreEqual(0, cu.Classes[0].Methods.Count);
			
			IMethod ctor = cu.Classes[0].DefaultReturnType.GetMethods().Single(m => m.IsConstructor);
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Synthetic, ctor.Modifiers);
			Assert.AreEqual(0, ctor.Parameters.Count);
		}
		
		[Test]
		public void DefaultConstructorOnAbstractClassTest()
		{
			ICompilationUnit cu = Parse("abstract class X { }", SupportedLanguage.CSharp, SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			Assert.AreEqual(0, cu.Classes[0].Methods.Count);
			
			IMethod ctor = cu.Classes[0].DefaultReturnType.GetMethods().Single(m => m.IsConstructor);
			Assert.AreEqual(ModifierEnum.Protected | ModifierEnum.Synthetic, ctor.Modifiers);
			Assert.AreEqual(0, ctor.Parameters.Count);
		}
		
		[Test]
		public void NoDefaultConstructorWithExplicitConstructorTest()
		{
			ICompilationUnit cu = Parse("class X { private X(int a) {} }", SupportedLanguage.CSharp, SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			Assert.AreEqual(1, cu.Classes[0].Methods.Count);
			
			IMethod ctor = cu.Classes[0].DefaultReturnType.GetMethods().Single(m => m.IsConstructor);
			Assert.AreEqual(ModifierEnum.Private, ctor.Modifiers);
			Assert.AreEqual(1, ctor.Parameters.Count);
		}
		
		[Test]
		public void DefaultConstructorWithExplicitConstructorOnStructTest()
		{
			ICompilationUnit cu = Parse("struct X { private X(int a) {} }", SupportedLanguage.CSharp, SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			Assert.AreEqual(1, cu.Classes[0].Methods.Count);
			
			List<IMethod> ctors = cu.Classes[0].DefaultReturnType.GetMethods().FindAll(m => m.IsConstructor);
			Assert.AreEqual(2, ctors.Count);
			
			Assert.AreEqual(ModifierEnum.Private, ctors[0].Modifiers);
			Assert.AreEqual(1, ctors[0].Parameters.Count);
			
			Assert.AreEqual(ModifierEnum.Public | ModifierEnum.Synthetic, ctors[1].Modifiers);
			Assert.AreEqual(0, ctors[1].Parameters.Count);
		}
		
		[Test]
		public void NoDefaultConstructorOnStaticClassTest()
		{
			ICompilationUnit cu = Parse("static class X { }", SupportedLanguage.CSharp);
			
			Assert.AreEqual(0, cu.Classes[0].Methods.Count);
			Assert.IsFalse(cu.Classes[0].DefaultReturnType.GetMethods().Any(m => m.IsConstructor));
		}
		
		[Test]
		public void VBNetIsExtensionMethodTest()
		{
			string code = @"Imports System.Runtime.CompilerServices

Module StringExtensions
	<Extension> _
	Sub Print(s As String)
	End Sub
End Module";
			ICompilationUnit cu = Parse(code, SupportedLanguage.VBNet, SharedProjectContentRegistryForTests.Instance.Mscorlib,
			                            SharedProjectContentRegistryForTests.Instance.GetProjectContentForReference("System.Core", "System.Core"));
			Assert.Greater(cu.Classes.Count, 0);
			Assert.AreEqual("StringExtensions", cu.Classes[0].Name);
			Assert.AreEqual(ClassType.Module, cu.Classes[0].ClassType);
			Assert.Greater(cu.Classes[0].Methods.Count, 0);
			
			Assert.IsTrue(cu.Classes[0].Methods[0].IsExtensionMethod);
			Assert.AreEqual("Print", cu.Classes[0].Methods[0].Name);
		}
	}
}
