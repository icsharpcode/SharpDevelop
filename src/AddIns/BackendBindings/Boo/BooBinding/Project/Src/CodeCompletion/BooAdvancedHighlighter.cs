// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Boo.Lang.Compiler.Ast;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class BooAdvancedHighlighter : IAdvancedHighlighter
	{
		public event EventHandler MarkOutstandingRequests;
		
		TextEditorControl textEditor;
		volatile Dictionary<int, List<int>> declarations;
		
		public void Initialize(TextEditorControl textEditor)
		{
			LoggingService.Debug("AdvancedHighlighter created");
			this.textEditor = textEditor;
			ParserService.ParserUpdateStepFinished += OnUpdateStep;
		}
		
		public void Dispose()
		{
			LoggingService.Debug("AdvancedHighlighter destroyed");
			ParserService.ParserUpdateStepFinished -= OnUpdateStep;
		}
		
		private class FindAssignmentsVisitor : DepthFirstVisitor
		{
			public Dictionary<int, List<int>> declarations = new Dictionary<int, List<int>>();
			// Possible declarations:
			// Dictionary<LineNumber, List<ColumnNumber>>
			// LineNumber and ColumnNumber are 0-based (text editor coordinates)
			
			public override void OnBinaryExpression(BinaryExpression node)
			{
				ReferenceExpression reference = node.Left as ReferenceExpression;
				if (node.Operator == BinaryOperatorType.Assign
				    && reference != null
				    && !(reference is MemberReferenceExpression)
				    && reference.LexicalInfo.IsValid)
				{
					int lineNumber = reference.LexicalInfo.Line - 1;
					
					List<int> l;
					if (!declarations.TryGetValue(lineNumber, out l)) {
						l = new List<int>();
						declarations.Add(lineNumber, l);
					}
					
					l.Add(reference.LexicalInfo.Column - 1);
				}
				base.OnBinaryExpression(node);
			}
			
			protected override void OnError(Node node, Exception error)
			{
				MessageService.ShowError(error, "VariableLookupVisitor: error processing " + node);
			}
		}
		
		void OnUpdateStep(object sender, ParserUpdateStepEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, textEditor.FileName)) {
				ParseInformation parseInfo = e.ParseInformation;
				if (parseInfo == null && this.declarations == null)
					parseInfo = ParserService.GetParseInformation(textEditor.FileName);
				if (parseInfo != null) {
					ICompilationUnit cu = parseInfo.MostRecentCompilationUnit;
					CompileUnit booCu = cu.Tag as CompileUnit;
					if (booCu != null) {
						FindAssignmentsVisitor visitor = new FindAssignmentsVisitor();
						booCu.Accept(visitor);
						this.declarations = visitor.declarations; // volatile access!
					}
				}
				WorkbenchSingleton.SafeThreadAsyncCall(this, "RaiseMarkOutstandingRequests", e);
			}
		}
		
		bool markingOutstanding;
		int resolveCount;
		
		void RaiseMarkOutstandingRequests(EventArgs e)
		{
			#if DEBUG
			int time = Environment.TickCount;
			#endif
			markingOutstanding = true;
			resolveCount = 0;
			if (MarkOutstandingRequests != null) {
				MarkOutstandingRequests(this, e);
			}
			markingOutstanding = false;
			#if DEBUG
			if (resolveCount > 0) {
				LoggingService.Debug("AdvancedHighlighter took " + (Environment.TickCount - time) + " ms for " + resolveCount + " resolves");
			}
			#endif
			textEditor.Document.CommitUpdate();
		}
		
		public void MarkLine(IDocument document, LineSegment currentLine, List<TextWord> words)
		{
			Dictionary<int, List<int>> decl = this.declarations; // volatile access!
			if (decl == null) return;
			int currentLineOffset = currentLine.Offset;
			int lineNumber = document.GetLineNumberForOffset(currentLineOffset);
			List<int> list;
			bool changedLine = false;
			if (decl.TryGetValue(lineNumber, out list)) {
				foreach (TextWord word in words) {
					if (word.Type == TextWordType.Word) {
						for (int i = 0; i < list.Count; i++) {
							if (word.Offset == list[i]) {
								LocalResolveResult rr;
								resolveCount++;
								rr = ParserService.Resolve(new ExpressionResult(word.Word),
								                           lineNumber + 1, word.Offset + 1,
								                           textEditor.FileName,
								                           textEditor.Document.TextContent
								                          ) as LocalResolveResult;
								if (rr != null
								    && rr.Field.Region.BeginLine == lineNumber + 1
								    && rr.Field.Region.BeginColumn == word.Offset + 1)
								{
									changedLine = true;
									word.SyntaxColor = textEditor.Document.HighlightingStrategy.GetColorFor("LocalVariableCreation");
								} else {
									list.RemoveAt(i--);
								}
							}
						}
					}
				}
			}
			if (markingOutstanding && changedLine) {
				textEditor.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
			}
		}
	}
}
