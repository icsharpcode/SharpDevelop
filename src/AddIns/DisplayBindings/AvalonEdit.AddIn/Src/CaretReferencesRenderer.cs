// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// In the code editor, highlights all references to the expression under the caret (for better code readability).
	/// </summary>
	public class CaretReferencesRenderer
	{
		/// <summary>
		/// Delays the highlighting after the caret position changes, so that Find references does not get called too often.
		/// </summary>
		DispatcherTimer delayTimer;
		const int delayMilliseconds = 800;
		DispatcherTimer delayMoveTimer;
		const int delayMoveMilliseconds = 100;
		
		CodeEditorView editorView;
		ITextEditor Editor { get { return editorView.Adapter; } }
		
		ExpressionHighlightRenderer highlightRenderer;
		ResolveResult lastResolveResult;
			
		/// <summary>
		/// In the code editor, highlights all references to the expression under the caret (for better code readability).
		/// </summary>
		public CaretReferencesRenderer(CodeEditorView editorView)
		{
			this.editorView = editorView;
			this.highlightRenderer = new ExpressionHighlightRenderer(this.editorView.TextArea.TextView);
			this.delayTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMilliseconds) };
			this.delayTimer.Stop();
			this.delayTimer.Tick += TimerTick;
			this.delayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMoveMilliseconds) };
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Tick += TimerMoveTick;
			this.editorView.TextArea.Caret.PositionChanged += CaretPositionChanged;
		}

		void TimerTick(object sender, EventArgs e)
		{
			this.delayTimer.Stop();
			LoggingService.Info("tick");
			// almost the same as DebuggerService.HandleToolTipRequest
			var referencesToBeHighlighted = GetReferencesInCurrentFile(this.lastResolveResult);
			this.highlightRenderer.SetHighlight(referencesToBeHighlighted);
		}
		
		void TimerMoveTick(object sender, EventArgs e)
		{
			LoggingService.Debug("move");
			this.delayMoveTimer.Stop();
			this.delayTimer.Stop();
			var resolveResult = GetExpressionUnderCaret();
			if (resolveResult == null) {
				this.lastResolveResult = resolveResult;
				this.highlightRenderer.ClearHighlight();
				return;
			}
			// caret is over symbol and that symbol is different from the last time
			if (!SameResolveResult(resolveResult, lastResolveResult))
			{
				this.lastResolveResult = resolveResult;
				this.highlightRenderer.ClearHighlight();
				this.delayTimer.Start();
			}
		}
		
		/// <summary>
		/// In the current document, highlights all references to the expression
		/// which is currently under the caret (local variable, class, property).
		/// This gets called on every caret position change, so quite often.
		/// </summary>
		void CaretPositionChanged(object sender, EventArgs e)
		{
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Start();
		}
		
		/// <summary>
		/// Resolves the current expression under caret.
		/// This gets called on every caret position change, so quite often.
		/// </summary>
		ResolveResult GetExpressionUnderCaret()
		{
			if (string.IsNullOrEmpty(Editor.FileName) || ParserService.LoadSolutionProjectsThreadRunning)
				return null;
			int line = Editor.Caret.Position.Line;
			int column = Editor.Caret.Position.Column;
			return ParserService.Resolve(line, column, Editor.Document, Editor.FileName);
		}

		/// <summary>
		/// Finds references to resolved expression in the current file.
		/// </summary>
		List<Reference> GetReferencesInCurrentFile(ResolveResult resolveResult)
		{
			var references = RefactoringService.FindReferencesLocal(resolveResult, Editor.FileName, null);
			if (references == null || references.Count == 0)
				return null;
			return references;
		}
		
		/// <summary>
		/// Returns true if the 2 ResolveResults refer to the same symbol.
		/// </summary>
		bool SameResolveResult(ResolveResult resolveResult, ResolveResult resolveResult2)
		{
			return false;
		}
	}
}
