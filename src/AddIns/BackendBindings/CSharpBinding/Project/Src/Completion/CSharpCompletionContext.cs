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
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Project;
using CSharpBinding.Parser;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Completion
{
	sealed class CSharpCompletionContext
	{
		public readonly ITextEditor Editor;
		public readonly IDocument Document;
		public readonly CSharpFullParseInformation ParseInformation;
		public readonly ICompilation Compilation;
		public readonly IProjectContent ProjectContent;
		public readonly CSharpTypeResolveContext TypeResolveContextAtCaret;
		public readonly ICompletionContextProvider CompletionContextProvider;
		
		public static CSharpCompletionContext Get(ITextEditor editor)
		{
			// Don't require the very latest parse information, an older cached version is OK.
			var parseInfo = SD.ParserService.GetCachedParseInformation(editor.FileName) as CSharpFullParseInformation;
			if (parseInfo == null) {
				parseInfo = SD.ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			}
			if (parseInfo == null)
				return null;
			
			ICompilation compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			var projectContent = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (projectContent == null)
				return null;
			
			return new CSharpCompletionContext(editor, parseInfo, compilation, projectContent, editor.Document, editor.Caret.Location);
		}
		
		public static CSharpCompletionContext Get(ITextEditor editor, ITextSource fileContent, TextLocation currentLocation, FileName fileName)
		{
			IDocument document = new ReadOnlyDocument(fileContent);
			
			// Don't require the very latest parse information, an older cached version is OK.
			var parseInfo = SD.ParserService.Parse(fileName, document) as CSharpFullParseInformation;
			if (parseInfo == null)
				return null;
			
			ICompilation compilation = SD.ParserService.GetCompilationForFile(fileName);
			var projectContent = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (projectContent == null)
				return null;
			
			return new CSharpCompletionContext(editor, parseInfo, compilation, projectContent, document, currentLocation);
		}
		
		private CSharpCompletionContext(ITextEditor editor, CSharpFullParseInformation parseInfo, ICompilation compilation, IProjectContent projectContent, IDocument document, TextLocation caretLocation)
		{
			Debug.Assert(editor != null);
			Debug.Assert(parseInfo != null);
			Debug.Assert(compilation != null);
			Debug.Assert(projectContent != null);
			Debug.Assert(document != null);
			this.Editor = editor;
			this.Document = document;
			this.ParseInformation = parseInfo;
			this.Compilation = compilation;
			this.ProjectContent = projectContent;
			this.TypeResolveContextAtCaret = parseInfo.UnresolvedFile.GetTypeResolveContext(compilation, caretLocation);
			this.CompletionContextProvider = new DefaultCompletionContextProvider(document, parseInfo.UnresolvedFile);
		}
	}
}
