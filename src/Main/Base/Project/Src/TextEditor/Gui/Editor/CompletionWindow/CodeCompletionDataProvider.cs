// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
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
		
		protected void GenerateCompletionData(TextArea textArea, string expression)
		{
			if (expression == null || expression.Length == 0) {
				return;
			}
			AddResolveResults(ParserService.Resolve(expression,
			                                        caretLineNumber,
			                                        caretColumn,
			                                        fileName,
			                                        textArea.Document.TextContent));
		}
	}
	
	public class CtrlSpaceCompletionDataProvider : CodeCompletionDataProvider
	{
		public CtrlSpaceCompletionDataProvider(ExpressionContext context)
		{
			this.context = context;
		}
		
		bool forceNewExpression;
		
		/// <summary>
		/// Gets/Sets whether the CtrlSpaceCompletionDataProvider creates a new completion
		/// dropdown instead of completing an old expression.
		/// Default value is false.
		/// </summary>
		public bool ForceNewExpression {
			get {
				return forceNewExpression;
			}
			set {
				forceNewExpression = value;
			}
		}
		
		protected override void GenerateCompletionData(TextArea textArea, char charTyped)
		{
			string expression = forceNewExpression ? null : GetExpression(textArea);
			preSelection = null;
			if (expression == null || expression.Length == 0) {
				preSelection = "";
				if (charTyped != '\0') {
					preSelection = null;
				}
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent));
				return;
			}
			
			int idx = expression.LastIndexOf('.');
			if (idx > 0) {
				preSelection = expression.Substring(idx + 1);
				expression = expression.Substring(0, idx);
				if (charTyped != '\0') {
					preSelection = null;
				}
				GenerateCompletionData(textArea, expression);
			} else {
				preSelection = expression;
				if (charTyped != '\0') {
					preSelection = null;
				}
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent));
			}
		}
	}
}
