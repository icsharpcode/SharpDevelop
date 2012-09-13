// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpDotCompletionDataProvider : DotCodeCompletionItemProvider
	{
		public override ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			resolver.LimitMethodExtractionUntilLine = editor.Caret.Line;
			return resolver.Resolve(expressionResult, parseInfo, editor.Document.Text);
		}
	}
}
