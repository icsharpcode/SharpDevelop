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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/*	/// <summary>
	/// In the code editor, highlights all references to the expression under the caret (for better code readability).
	/// </summary>
	public class CaretReferencesRenderer
	{
		/// <summary>
		/// Delays the Resolve check so that it does not get called too often when user holds an arrow.
		/// </summary>
		DispatcherTimer delayMoveTimer;
		const int delayMoveMs = 100;
		
		/// <summary>
		/// Delays the Find references (and highlight) after the caret stays at one point for a while.
		/// </summary>
		DispatcherTimer delayTimer;
		const int delayMs = 800;
		
		/// <summary>
		/// Maximum time for Find references. After this time it gets cancelled and no highlight is displayed.
		/// Useful for very large files.
		/// </summary>
		const int findReferencesTimeoutMs = 200;
		
		CodeEditorView editorView;
		ITextEditor Editor { get { return editorView.Adapter; } }
		
		ExpressionHighlightRenderer highlightRenderer;
		ResolveResult lastResolveResult;
		
		public bool IsEnabled
		{
			get {
				string fileName = this.Editor.FileName;
				return CodeEditorOptions.Instance.HighlightSymbol && (fileName.EndsWith(".cs") || fileName.EndsWith(".vb"));
			}
		}
		
		/// <summary>
		/// In the code editor, highlights all references to the expression under the caret (for better code readability).
		/// </summary>
		public CaretReferencesRenderer(CodeEditorView editorView)
		{
			this.editorView = editorView;
			this.highlightRenderer = new ExpressionHighlightRenderer(this.editorView.TextArea.TextView);
			this.delayTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMs) };
			this.delayTimer.Stop();
			this.delayTimer.Tick += TimerTick;
			this.delayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMoveMs) };
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Tick += TimerMoveTick;
			this.editorView.TextArea.Caret.PositionChanged += CaretPositionChanged;
			// fixes SD-1873 - Unhandled WPF Exception when deleting text in text editor
			// clear highlights to avoid exceptions when trying to draw highlights in
			// locations that have been deleted already.
			this.editorView.Document.Changed += delegate { lastResolveResult = null; ClearHighlight(); };
		}
		
		public void ClearHighlight()
		{
			this.highlightRenderer.ClearHighlight();
		}

		/// <summary>
		/// In the current document, highlights all references to the expression
		/// which is currently under the caret (local variable, class, property).
		/// This gets called on every caret position change, so quite often.
		/// </summary>
		void CaretPositionChanged(object sender, EventArgs e)
		{
			Restart(this.delayMoveTimer);
		}
		
		void TimerTick(object sender, EventArgs e)
		{
			this.delayTimer.Stop();
			
			if (!IsEnabled)
				return;
			var referencesToBeHighlighted = FindReferencesInCurrentFile(this.lastResolveResult);
			this.highlightRenderer.SetHighlight(referencesToBeHighlighted);
		}
		
		void TimerMoveTick(object sender, EventArgs e)
		{
			this.delayMoveTimer.Stop();
			this.delayTimer.Stop();
			
			if (!IsEnabled)
				return;
			
			var resolveResult = GetExpressionAtCaret();
			if (resolveResult == null) {
				this.lastResolveResult = null;
				this.highlightRenderer.ClearHighlight();
				return;
			}
			// caret is over symbol and that symbol is different from the last time
			if (!SameResolveResult(resolveResult, lastResolveResult))
			{
				this.lastResolveResult = resolveResult;
				this.highlightRenderer.ClearHighlight();
				this.delayTimer.Start();
			} else {
				// highlight stays the same, both timers are stopped (will start again when caret moves)
			}
		}
		
		/// <summary>
		/// Resolves the current expression under caret.
		/// This gets called on every caret position change, so quite often.
		/// </summary>
		ResolveResult GetExpressionAtCaret()
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
		List<Reference> FindReferencesInCurrentFile(ResolveResult resolveResult)
		{
			if (resolveResult == null)
				return null;
			var cancellationTokenSource = new CancellationTokenSource();
			using (new Timer(
				delegate {
					LoggingService.Debug("Aborting FindReferencesInCurrentFile due to timeout");
					cancellationTokenSource.Cancel();
				}, null, findReferencesTimeoutMs, Timeout.Infinite))
			{
				var progressMonitor = new DummyProgressMonitor();
				progressMonitor.CancellationToken = cancellationTokenSource.Token;
				var references = RefactoringService.FindReferencesLocal(resolveResult, Editor.FileName, progressMonitor);
				if (references == null || references.Count == 0)
					return null;
				return references;
			}
		}
		
		/// <summary>
		/// Returns true if the 2 ResolveResults refer to the same symbol.
		/// So that when caret moves but stays inside the same symbol, symbol stays highlighted.
		/// </summary>
		bool SameResolveResult(ResolveResult resolveResult, ResolveResult resolveResult2)
		{
			//if (resolveResult == null && resolveResult2 == null)
			//	return true;
			//if (resolveResult == null && resolveResult2 != null)
			//	return false;
			//if (resolveResult != null && resolveResult2 == null)
			//	return false;
			// TODO determine if 2 ResolveResults refer to the same symbol
			return false;
		}
		
		/// <summary>
		/// Restarts a timer.
		/// </summary>
		void Restart(DispatcherTimer timer)
		{
			timer.Stop();
			timer.Start();
		}
	}
	 */
}
