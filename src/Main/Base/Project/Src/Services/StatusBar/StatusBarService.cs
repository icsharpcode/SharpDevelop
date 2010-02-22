// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	public static class StatusBarService
	{
		static SdStatusBar statusBar = null;
		
		internal static void Initialize()
		{
			statusBar = new SdStatusBar();
		}
		
		public static bool Visible {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar.Visibility == Visibility.Visible;
			}
			set {
				System.Diagnostics.Debug.Assert(statusBar != null);
				statusBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		
		internal static SdStatusBar Control {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public static void SetCaretPosition(int x, int y, int charOffset)
		{
			statusBar.CursorStatusBarPanel.Content = StringParser.Parse(
				"${res:StatusBarService.CursorStatusBarPanelText}",
				new string[,] {
					{"Line", String.Format("{0,-10}", y)},
					{"Column", String.Format("{0,-5}", x)},
					{"Character", String.Format("{0,-5}", charOffset)}
				});
		}
		
		public static void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Content = insertMode ? StringParser.Parse("${res:StatusBarService.CaretModes.Insert}") : StringParser.Parse("${res:StatusBarService.CaretModes.Overwrite}");
		}
		
		public static void ShowErrorMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(StringParser.Parse(message));
		}
		
		public static void SetMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(StringParser.Parse(message));
		}
		
		public static void SetMessage(BitmapSource image, string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.SetMessage(image, StringParser.Parse(message));
		}
		
		public static void SetMessage(string message, bool highlighted)
		{
			statusBar.SetMessage(message, highlighted);
		}
		
		static bool   wasError    = false;
		static string lastMessage = "";
		
		public static void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
			
			Visible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
		}
		
		#region Progress Monitor
		static Stack<ProgressCollector> waitingProgresses = new Stack<ProgressCollector>();
		static ProgressCollector currentProgress;
		
		public static IProgressMonitor CreateProgressMonitor(CancellationToken cancellationToken)
		{
			ProgressCollector progress = new ProgressCollector(WorkbenchSingleton.Workbench.SynchronizingObject, cancellationToken);
			AddProgress(progress);
			return progress.ProgressMonitor;
		}
		
		public static void AddProgress(ProgressCollector progress)
		{
			if (progress == null)
				throw new ArgumentNullException("progress");
			System.Diagnostics.Debug.Assert(statusBar != null);
			WorkbenchSingleton.AssertMainThread();
			if (currentProgress != null) {
				currentProgress.ProgressMonitorDisposed -= progress_ProgressMonitorDisposed;
				currentProgress.PropertyChanged -= progress_PropertyChanged;
			}
			waitingProgresses.Push(currentProgress); // push even if currentProgress==null
			SetActiveProgress(progress);
		}
		
		static void SetActiveProgress(ProgressCollector progress)
		{
			WorkbenchSingleton.AssertMainThread();
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
		
		static void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Debug.Assert(sender == currentProgress);
			statusBar.DisplayProgress(currentProgress.TaskName, currentProgress.Progress, currentProgress.Status);
		}
		
		static void progress_ProgressMonitorDisposed(object sender, EventArgs e)
		{
			Debug.Assert(sender == currentProgress);
			SetActiveProgress(waitingProgresses.Pop()); // stack is never empty: we push null as first element
		}
		#endregion
	}
}
