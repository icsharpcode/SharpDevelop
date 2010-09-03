// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using System;
using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;

namespace ResourceToolkit.Tests
{
	public abstract class AbstractResourceResolverTestFixture : AbstractTestProjectTestFixture
	{
		protected abstract string DefaultFileName {
			get;
		}
		
		protected override void DoSetUp()
		{
			base.DoSetUp();
			TestHelper.InitializeResolvers();
		}
		
		protected void EnlistTestFile(string fileName, string code, bool parseFile)
		{
			ResourceResolverService.SetFileContentUnitTestOnly(fileName, code);
			ProjectFileDictionaryService.AddFile(fileName, this.Project);
			
			if (parseFile) {
				IParser parser = ResourceResolverService.GetParser(fileName);
				Assert.IsNotNull(parser, "Could not get parser for " + fileName+ ".");
				ICompilationUnit cu = parser.Parse(this.DefaultProjectContent, fileName, new StringTextBuffer(code));
				cu.Freeze();
				Assert.IsFalse(cu.ErrorsDuringCompile, "Errors while parsing test program.");
				ParserService.RegisterParseInformation(fileName, cu);
				this.DefaultProjectContent.UpdateCompilationUnit(null, cu, fileName);
			}
		}
		
		/// <summary>
		/// Resolves a resource reference.
		/// Line and column are 0-based.
		/// </summary>
		protected ResourceResolveResult Resolve(string fileName, string code, int caretLine, int caretColumn, char? charTyped, bool parseFile)
		{
			this.EnlistTestFile(fileName, code, parseFile);
			IDocument doc = new AvalonEditDocumentAdapter();
			doc.Text = code;
			return ResourceResolverService.Resolve(fileName, doc, caretLine, caretColumn, charTyped);
		}
		
		/// <summary>
		/// Resolves a resource reference.
		/// Line and column are 0-based.
		/// </summary>
		protected ResourceResolveResult Resolve(string code, int caretLine, int caretColumn, char? charTyped)
		{
			return this.Resolve(this.DefaultFileName, code, caretLine, caretColumn, charTyped, true);
		}
		
		protected override void DoTearDown()
		{
			base.DoTearDown();
			NRefactoryAstCacheService.DisableCache();
		}
		
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					NRefactoryAstCacheService.DisableCache();
				}
			} finally {
				base.Dispose(disposing);
			}
		}
	}
}
