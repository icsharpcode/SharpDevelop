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
