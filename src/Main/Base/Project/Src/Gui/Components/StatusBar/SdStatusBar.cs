// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdStatusBar : StatusBar
	{
		StatusBarItem statusProgressBarItem = new StatusBarItem();
		ProgressBar statusProgressBar = new ProgressBar();
		StatusBarItem jobNamePanel      = new StatusBarItem();
		
		StatusBarItem txtStatusBarPanel    = new StatusBarItem();
		StatusBarItem cursorStatusBarPanel = new StatusBarItem();
		StatusBarItem modeStatusBarPanel   = new StatusBarItem();
		
		public StatusBarItem CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public StatusBarItem ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
		
		public SdStatusBar()
		{
			cursorStatusBarPanel.Width = 150;
			modeStatusBarPanel.Width = 25;
			statusProgressBarItem.Visibility = Visibility.Hidden;
			statusProgressBarItem.Width = 100;
			statusProgressBarItem.Content = statusProgressBar;
			statusProgressBarItem.VerticalContentAlignment = VerticalAlignment.Stretch;
			statusProgressBarItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			
			DockPanel.SetDock(modeStatusBarPanel, Dock.Right);
			DockPanel.SetDock(cursorStatusBarPanel, Dock.Right);
			DockPanel.SetDock(statusProgressBarItem, Dock.Right);
			DockPanel.SetDock(jobNamePanel, Dock.Right);
			
			Items.Add(modeStatusBarPanel);
			Items.Add(cursorStatusBarPanel);
			Items.Add(statusProgressBarItem);
			Items.Add(jobNamePanel);
			
			Items.Add(txtStatusBarPanel);
		}
		
		public void ShowErrorMessage(string message)
		{
			SetMessage("Error : " + message);
		}
		
		public void ShowErrorMessage(BitmapSource image, string message)
		{
			SetMessage(image, "Error : " + message);
		}
		
		public void SetMessage(string message)
		{
			SetMessage(message, false);
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
			if (WorkbenchSingleton.InvokeRequired)
				WorkbenchSingleton.SafeThreadAsyncCall(setMessageAction);
			else
				setMessageAction();
		}
		
		public void SetMessage(BitmapSource image, string message)
		{
			SetMessage(message);
		}
		
		// Displaying progress
		
		bool statusProgressBarIsVisible;
		string currentTaskName;
		
		public void DisplayProgress(string taskName, int workDone, int totalWork)
		{
			if (taskName == null)
				taskName = "";
			if (totalWork < 0)
				totalWork = 0;
			if (workDone < 0)
				workDone = 0;
			if (workDone > totalWork)
				workDone = totalWork;
			
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					if (!statusProgressBarIsVisible) {
						statusProgressBarItem.Visibility = Visibility.Visible;
						statusProgressBarIsVisible = true;
					}
					
					if (totalWork == 0) {
						statusProgressBar.IsIndeterminate = true;
					} else {
						statusProgressBar.IsIndeterminate = false;
						if (statusProgressBar.Maximum != totalWork) {
							if (statusProgressBar.Value > totalWork)
								statusProgressBar.Value = 0;
							statusProgressBar.Maximum = totalWork;
						}
						statusProgressBar.Value = workDone;
					}
					
					if (currentTaskName != taskName) {
						currentTaskName = taskName;
						jobNamePanel.Content = StringParser.Parse(taskName);
					}
				});
		}
		
		public void HideProgress()
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					statusProgressBarIsVisible = false;
					statusProgressBarItem.Visibility = Visibility.Collapsed;
					jobNamePanel.Content = currentTaskName = "";
				});
		}
	}
}
