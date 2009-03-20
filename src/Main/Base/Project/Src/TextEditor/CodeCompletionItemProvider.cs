// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System.Collections;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Allows creating a <see cref="ICompletionDataList"/> from code-completion information.
	/// </summary>
	public class CodeCompletionItemProvider
	{
		/// <summary>
		/// Shows code completion for the specified editor.
		/// </summary>
		public virtual void ShowCompletion(ITextEditor editor)
		{
			ICompletionItemList itemList = GenerateCompletionList(editor);
			if (itemList != null)
				editor.ShowCompletionWindow(itemList);
		}
		
		public virtual ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("textEditor");
			ExpressionResult expression = GetExpression(editor);
			return GenerateCompletionListForExpression(editor, expression);
		}
		
		public virtual ExpressionResult GetExpression(ITextEditor editor)
		{
			return GetExpressionFromOffset(editor, editor.Caret.Offset);
		}
		
		protected ExpressionResult GetExpressionFromOffset(ITextEditor editor, int offset)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			IDocument document = editor.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null) {
				return ExpressionResult.Empty;
			} else {
				return expressionFinder.FindExpression(document.GetText(0, offset), offset);
			}
		}
		
		public virtual ICompletionItemList GenerateCompletionListForExpression(ITextEditor editor, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression == null) {
				return null;
			}
			if (LoggingService.IsDebugEnabled) {
				if (expressionResult.Context == ExpressionContext.Default)
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<", expressionResult.Expression);
				else
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
			}
			ResolveResult rr = Resolve(editor, expressionResult);
			return GenerateCompletionListForResolveResult(rr, expressionResult.Context);
		}
		
		public virtual ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			return ParserService.Resolve(expressionResult, editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text);
		}
		
		public virtual ICompletionItemList GenerateCompletionListForResolveResult(ResolveResult rr, ExpressionContext context)
		{
			if (rr == null)
				return null;
			IProjectContent callingContent = rr.CallingClass != null ? rr.CallingClass.ProjectContent : null;
			ArrayList arr = rr.GetCompletionData(callingContent ?? ParserService.CurrentProjectContent);
			DefaultCompletionItemList result = new DefaultCompletionItemList();
			foreach (object o in arr) {
				IEntity entity = o as IEntity;
				if (entity != null)
					result.Items.Add(new CodeCompletionItem(entity));
			}
			return result;
		}
		
		public virtual ICompletionItem CreateCompletionItem(IEntity entity)
		{
			return new CodeCompletionItem(entity);
		}
	}
	
	public class DotCodeCompletionItemProvider : CodeCompletionItemProvider
	{
		
	}
	
	public class CodeCompletionItem : ICompletionItem
	{
		readonly IEntity entity;
		
		public CodeCompletionItem(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.entity = entity;
		}
		
		public string Text {
			get {
				return entity.Name;
			}
		}
		
		public string Description {
			get {
				return entity.Documentation;
			}
		}
	}
}
