// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using Boo.Lang.Compiler.Ast;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace Grunwald.BooBinding.CodeCompletion
{
	/*
	public class BooAdvancedHighlighter : AsynchronousAdvancedHighlighter
	{
		volatile Dictionary<int, List<int>> declarations;
		
		public override void Initialize(TextEditorControl textEditor)
		{
			this.ImmediateMarkLimit = 0; // never immediately mark lines
			base.Initialize(textEditor);
			ParserService.ParserUpdateStepFinished += OnUpdateStep;
		}
		
		public override void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= OnUpdateStep;
			base.Dispose();
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
				MessageService.ShowException(error, "VariableLookupVisitor: error processing " + node);
			}
		}
		
		void OnUpdateStep(object sender, ParserUpdateStepEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, this.TextEditor.FileName)) {
				ParseInformation parseInfo = e.ParseInformation;
				if (parseInfo == null && this.declarations == null)
					parseInfo = ParserService.GetParseInformation(this.TextEditor.FileName);
				if (parseInfo != null) {
					ICompilationUnit cu = parseInfo.CompilationUnit;
					CompileUnit booCu = cu.Tag as CompileUnit;
					if (booCu != null) {
						FindAssignmentsVisitor visitor = new FindAssignmentsVisitor();
						booCu.Accept(visitor);
						this.declarations = visitor.declarations; // volatile access!
					}
				}
				WorkbenchSingleton.SafeThreadAsyncCall(MarkOutstanding);
			}
		}
		
		bool markingOutstanding;
		int resolveCount;
		
		protected override void MarkOutstanding()
		{
			#if DEBUG
			int time = Environment.TickCount;
			#endif
			markingOutstanding = true;
			resolveCount = 0;
			base.MarkOutstanding();
			markingOutstanding = false;
			#if DEBUG
			time = Environment.TickCount - time;
			if (time > 0) {
				LoggingService.Info("BooHighlighter took " + time + "ms for " + resolveCount + " resolves");
			}
			#endif
			this.Document.CommitUpdate();
		}
		
		protected override void MarkWords(int lineNumber, LineSegment currentLine, List<TextWord> words)
		{
			Dictionary<int, List<int>> decl = this.declarations; // volatile access!
			if (decl == null) return;
			int currentLineOffset = currentLine.Offset;
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
								                           this.TextEditor.FileName,
								                           this.Document.TextContent
								                          ) as LocalResolveResult;
								if (rr != null
								    && rr.VariableDefinitionRegion.BeginLine == lineNumber + 1
								    && rr.VariableDefinitionRegion.BeginColumn == word.Offset + 1)
								{
									changedLine = true;
									word.SyntaxColor = this.Document.HighlightingStrategy.GetColorFor("LocalVariableCreation");
								} else {
									list.RemoveAt(i--);
								}
							}
						}
					}
				}
			}
			if (markingOutstanding && changedLine) {
				this.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
			}
		}
	}
	*/
}
