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
		ICompilationUnit Parse(string code, SupportedLanguage language)
		{
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(new DefaultProjectContent());
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
	}
}
