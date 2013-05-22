// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
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
			
			return new CSharpCompletionContext(editor, parseInfo, compilation, projectContent);
		}
		
		private CSharpCompletionContext(ITextEditor editor, CSharpFullParseInformation parseInfo, ICompilation compilation, IProjectContent projectContent)
		{
			Debug.Assert(editor != null);
			Debug.Assert(parseInfo != null);
			Debug.Assert(compilation != null);
			Debug.Assert(projectContent != null);
			this.Editor = editor;
			this.ParseInformation = parseInfo;
			this.Compilation = compilation;
			this.ProjectContent = projectContent;
			this.TypeResolveContextAtCaret = parseInfo.UnresolvedFile.GetTypeResolveContext(compilation, editor.Caret.Location);
			this.CompletionContextProvider = new DefaultCompletionContextProvider(editor.Document, parseInfo.UnresolvedFile);
		}
	}
}
