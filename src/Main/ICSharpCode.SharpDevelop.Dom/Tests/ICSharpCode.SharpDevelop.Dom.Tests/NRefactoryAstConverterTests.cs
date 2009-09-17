// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class NRefactoryAstConverterTests
	{
		ICompilationUnit Parse(string code, SupportedLanguage language, bool referenceMscorlib)
		{
			DefaultProjectContent pc = new DefaultProjectContent();
			if (referenceMscorlib) {
				pc.AddReferencedContent(SharedProjectContentRegistryForTests.Instance.Mscorlib);
			}
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc);
			using (IParser p = ParserFactory.CreateParser(language, new StringReader(code))) {
				p.ParseMethodBodies = false;
				p.Parse();
				
				visitor.Specials = p.Lexer.SpecialTracker.CurrentSpecials;
				visitor.VisitCompilationUnit(p.CompilationUnit, null);
			}
			return visitor.Cu;
		}
		
		ICompilationUnit Parse(string code, SupportedLanguage language)
		{
			return Parse(code, language, false);
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
", SupportedLanguage.CSharp, true);
			
			IProperty p = cu.Classes[0].Properties[0];
			Assert.IsTrue(p.IsIndexer, "IsIndexer must be true");
			Assert.AreEqual("Foo", p.Name);
			Assert.AreEqual(1, p.Parameters.Count);
		}
	}
}
