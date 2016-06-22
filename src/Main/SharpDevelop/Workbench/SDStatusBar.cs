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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	class SDStatusBar : StatusBar
	{
		StatusBarItem statusProgressBarItem = new StatusBarItem();
		ProgressBar statusProgressBar = new ProgressBar();
		StatusBarItem jobNamePanel      = new StatusBarItem();
		
		StatusBarItem txtStatusBarPanel    = new StatusBarItem();
		StatusBarItem cursorStatusBarPanel = new StatusBarItem();
		StatusBarItem selectionStatusBarPanel = new StatusBarItem();
		StatusBarItem modeStatusBarPanel   = new StatusBarItem();
		
		public StatusBarItem CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public StatusBarItem SelectionStatusBarPanel {
			get {
				return selectionStatusBarPanel;
			}
		}
		
		public StatusBarItem ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
		
		public SDStatusBar()
		{
			cursorStatusBarPanel.Width = 150;
			selectionStatusBarPanel.Content = 50;
			modeStatusBarPanel.Width = 25;
			
			statusProgressBar.Minimum = 0;
			statusProgressBar.Maximum = 1;
			
			statusProgressBarItem.Visibility = Visibility.Hidden;
			statusProgressBarItem.Width = 100;
			statusProgressBarItem.Content = statusProgressBar;
			statusProgressBarItem.VerticalContentAlignment = VerticalAlignment.Stretch;
			statusProgressBarItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			
			DockPanel.SetDock(modeStatusBarPanel, Dock.Right);
			DockPanel.SetDock(selectionStatusBarPanel, Dock.Right);
			DockPanel.SetDock(cursorStatusBarPanel, Dock.Right);
			DockPanel.SetDock(statusProgressBarItem, Dock.Right);
			DockPanel.SetDock(jobNamePanel, Dock.Right);
			
			Items.Add(modeStatusBarPanel);
			Items.Add(selectionStatusBarPanel);
			Items.Add(cursorStatusBarPanel);
			Items.Add(statusProgressBarItem);
			Items.Add(jobNamePanel);
			
			Items.Add(txtStatusBarPanel);
		}
		
		public void SetMessage(string message, bool highlighted)
		{
			Action setMessageAction = delegate {
				if (highlighted) {
					txtStatusBarPanel.Background = SystemColors.HighlightBrush;
					txtStatusBarPanel.Foreground = SystemColors.HighlightTextBrush;
				} else {
					txtStatusBarPanel.Background = SystemColors.ControlBrush;
					txtStatusBarPanel.Foreground = SystemColors.ControlTextBrush;
				}
				txtStatusBarPanel.Content = message;
			};
			if (SD.MainThread.InvokeRequired) {
				SD.MainThread.InvokeAsyncAndForget(setMessageAction);
			} else {
				setMessageAction();
			}
		}
		
		// Displaying progress
		
		bool statusProgressBarIsVisible;
		string currentTaskName;
		OperationStatus currentStatus;
		SolidColorBrush progressForegroundBrush;
		
		public void DisplayProgress(string taskName, double workDone, OperationStatus status)
		{
//			LoggingService.Debug("DisplayProgress(\"" + taskName + "\", " + workDone + ", " + status + ")");
			if (!statusProgressBarIsVisible) {
				statusProgressBarItem.Visibility = Visibility.Visible;
				statusProgressBarIsVisible = true;
				StopHideProgress();
			}
			
			TaskbarItemProgressState taskbarProgressState;
			if (double.IsNaN(workDone)) {
				statusProgressBar.IsIndeterminate = true;
				status = OperationStatus.Normal; // indeterminate doesn't support foreground color
				taskbarProgressState = TaskbarItemProgressState.Indeterminate;
			} else {
				statusProgressBar.IsIndeterminate = false;
				statusProgressBar.Value = workDone;
				
				if (status == OperationStatus.Error)
					taskbarProgressState = TaskbarItemProgressState.Error;
				else
					taskbarProgressState = TaskbarItemProgressState.Normal;
			}
			
			TaskbarItemInfo taskbar = SD.Workbench.MainWindow.TaskbarItemInfo;
			if (taskbar != null) {
				taskbar.ProgressState = taskbarProgressState;
				taskbar.ProgressValue = workDone;
			}
			
			if (status != currentStatus) {
				if (progressForegroundBrush == null) {
					SolidColorBrush defaultForeground = statusProgressBar.Foreground as SolidColorBrush;
					progressForegroundBrush = new SolidColorBrush(defaultForeground != null ? defaultForeground.Color : Colors.Blue);
				}
				
				if (status == OperationStatus.Error) {
					statusProgressBar.Foreground = progressForegroundBrush;
					progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
						Colors.Red, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd));
				} else if (status == OperationStatus.Warning) {
					statusProgressBar.Foreground = progressForegroundBrush;
					progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
						Colors.YellowGreen, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd));
				} else {
					statusProgressBar.ClearValue(ProgressBar.ForegroundProperty);
					progressForegroundBrush = null;
				}
				currentStatus = status;
			}
			
			if (currentTaskName != taskName) {
				currentTaskName = taskName;
				jobNamePanel.Content = taskName;
			}
		}
		
		public void HideProgress()
		{
//			LoggingService.Debug("HideProgress()");
			statusProgressBarIsVisible = false;
			// to allow the user to see the red progress bar as a visual clue of a failed 
			// build even if it occurs close to the end of the build, we'll hide the progress bar
			// with a bit of time delay
			SD.MainThread.CallLater(
				TimeSpan.FromMilliseconds(currentStatus == OperationStatus.Error ? 500 : 150),
				new Action(DoHideProgress));
		}
		
		void DoHideProgress()
		{
			if (!statusProgressBarIsVisible) {
				// make stuff look nice and delay it a little more by using an animation
				// on the progress bar
				TimeSpan timeSpan = TimeSpan.FromSeconds(0.25);
				var animation = new DoubleAnimation(0, new Duration(timeSpan), FillBehavior.HoldEnd);
				statusProgressBarItem.BeginAnimation(OpacityProperty, animation);
				jobNamePanel.BeginAnimation(OpacityProperty, animation);
				SD.MainThread.CallLater(
					timeSpan,
					delegate{
						if (!statusProgressBarIsVisible) {
							statusProgressBarItem.Visibility = Visibility.Collapsed;
							jobNamePanel.Content = currentTaskName = "";
							var taskbar = SD.Workbench.MainWindow.TaskbarItemInfo;
							if (taskbar != null)
								taskbar.ProgressState = TaskbarItemProgressState.None;
							StopHideProgress();
						}
					});
			}
		}
		
		void StopHideProgress()
		{
			statusProgressBarItem.BeginAnimation(OpacityProperty, null);
			jobNamePanel.BeginAnimation(OpacityProperty, null);
		}
	}
}
