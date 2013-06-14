// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using CSharpBinding.Parser;
using CSharpBinding.Refactoring;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// C#-specific extension methods.
	/// </summary>
	public static class ExtensionMethods
	{
		public static TextEditorOptions ToEditorOptions(this ITextEditor editor)
		{
			return new TextEditorOptions {
				TabsToSpaces = editor.Options.ConvertTabsToSpaces,
				TabSize = editor.Options.IndentationSize,
				IndentSize = editor.Options.IndentationSize,
				ContinuationIndent = editor.Options.IndentationSize,
				LabelIndent = -editor.Options.IndentationSize,
				WrapLineLength = editor.Options.VerticalRulerColumn,
			};
		}
		
		public static async Task<SyntaxTree> GetSyntaxTreeAsync(this EditorRefactoringContext editorContext)
		{
			var parseInfo = (await editorContext.GetParseInformationAsync().ConfigureAwait(false)) as CSharpFullParseInformation;
			if (parseInfo != null)
				return parseInfo.SyntaxTree;
			else
				return new SyntaxTree();
		}
		
		public static Task<CSharpAstResolver> GetAstResolverAsync(this EditorRefactoringContext editorContext)
		{
			return editorContext.GetCachedAsync(
				async ec => {
					var parseInfo = (await ec.GetParseInformationAsync().ConfigureAwait(false)) as CSharpFullParseInformation;
					var compilation = await ec.GetCompilationAsync().ConfigureAwait(false);
					if (parseInfo != null)
						return parseInfo.GetResolver(compilation);
					else
						return new CSharpAstResolver(compilation, new SyntaxTree(), new CSharpUnresolvedFile { FileName = ec.FileName });
				});
		}
	}
}
