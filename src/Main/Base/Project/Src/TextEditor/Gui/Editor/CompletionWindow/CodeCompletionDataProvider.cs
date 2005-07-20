// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CodeCompletionDataProvider : AbstractCompletionDataProvider
	{
		protected override void GenerateCompletionData(TextArea textArea, char charTyped)
		{
			preSelection = null;
			GenerateCompletionData(textArea, GetExpression(textArea));
		}
		
		#if DEBUG
		public bool DebugMode = false;
		#endif
		
		protected void GenerateCompletionData(TextArea textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression == null || expressionResult.Expression.Length == 0) {
				return;
			}
			#if DEBUG
			if (DebugMode) {
				Debugger.Break();
			}
			#endif
			AddResolveResults(ParserService.Resolve(expressionResult,
			                                        caretLineNumber,
			                                        caretColumn,
			                                        fileName,
			                                        textArea.Document.TextContent),
			                  expressionResult.Context);
		}
	}
}
