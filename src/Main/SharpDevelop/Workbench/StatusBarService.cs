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
using System.Diagnostics;
using System.Threading;
using System.Windows;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class StatusBarService : IStatusBarService
	{
		readonly SDStatusBar statusBar;
		
		public StatusBarService(SDStatusBar statusBar)
		{
			if (statusBar == null)
				throw new ArgumentNullException("statusBar");
			this.statusBar = statusBar;
		}
		
		public bool Visible {
			get {
				return statusBar.Visibility == Visibility.Visible;
			}
			set {
				statusBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		
		public void SetCaretPosition(int x, int y, int charOffset)
		{
			statusBar.CursorStatusBarPanel.Content = StringParser.Parse(
				"${res:StatusBarService.CursorStatusBarPanelText}",
				new StringTagPair("Line", String.Format("{0,-10}", y)),
				new StringTagPair("Column", String.Format("{0,-5}", x)),
				new StringTagPair("Character", String.Format("{0,-5}", charOffset))
			);
		}
		
		public void SetSelectionSingle(int length)
		{
			if (length > 0) {
				statusBar.SelectionStatusBarPanel.Content = StringParser.Parse(
					"${res:StatusBarService.SelectionStatusBarPanelTextSingle}",
					new StringTagPair("Length", String.Format("{0,-10}", length)));
			} else {
				statusBar.SelectionStatusBarPanel.Content = null;
			}
		}
		
		public void SetSelectionMulti(int rows, int cols)
		{
			if (rows > 0 && cols > 0) {
				statusBar.SelectionStatusBarPanel.Content = StringParser.Parse(
					"${res:StatusBarService.SelectionStatusBarPanelTextMulti}",
					new StringTagPair("Rows", String.Format("{0}", rows)),
					new StringTagPair("Cols", String.Format("{0}", cols)),
					new StringTagPair("Total", String.Format("{0}", rows * cols)));
			} else {
				statusBar.SelectionStatusBarPanel.Content = null;
			}
		}
		
		public void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Content = insertMode ? StringParser.Parse("${res:StatusBarService.CaretModes.Insert}") : StringParser.Parse("${res:StatusBarService.CaretModes.Overwrite}");
		}
		
		public void SetMessage(string message, bool highlighted, IImage icon)
		{
			statusBar.SetMessage(StringParser.Parse(message), highlighted);
		}
		
		#region Progress Monitor
		Stack<ProgressCollector> waitingProgresses = new Stack<ProgressCollector>();
		ProgressCollector currentProgress;
		
		public IProgressMonitor CreateProgressMonitor(CancellationToken cancellationToken = default(CancellationToken))
		{
			ProgressCollector progress = new ProgressCollector(SD.MainThread.SynchronizingObject, cancellationToken);
			AddProgress(progress);
			return progress.ProgressMonitor;
		}
		
		public void AddProgress(ProgressCollector progress)
		{
			if (progress == null)
				throw new ArgumentNullException("progress");
			SD.MainThread.VerifyAccess();
			if (currentProgress != null) {
				currentProgress.ProgressMonitorDisposed -= progress_ProgressMonitorDisposed;
				currentProgress.PropertyChanged -= progress_PropertyChanged;
			}
			waitingProgresses.Push(currentProgress); // push even if currentProgress==null
			SetActiveProgress(progress);
		}
		
		void SetActiveProgress(ProgressCollector progress)
		{
			SD.MainThread.VerifyAccess();
			currentProgress = progress;
			if (progress == null) {
				statusBar.HideProgress();
				return;
			}
			
			progress.ProgressMonitorDisposed += progress_ProgressMonitorDisposed;
			if (progress.ProgressMonitorIsDisposed) {
				progress_ProgressMonitorDisposed(progress, null);
				return;
			}
			progress.PropertyChanged += progress_PropertyChanged;
		}
		
		void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Debug.Assert(sender == currentProgress);
			statusBar.DisplayProgress(currentProgress.TaskName, currentProgress.Progress, currentProgress.Status);
		}
		
		void progress_ProgressMonitorDisposed(object sender, EventArgs e)
		{
			Debug.Assert(sender == currentProgress);
			SetActiveProgress(waitingProgresses.Pop()); // stack is never empty: we push null as first element
		}
		#endregion
	}
}
