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
