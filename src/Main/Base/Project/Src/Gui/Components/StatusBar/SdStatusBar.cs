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
using System.Windows.Media.Animation;
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
			
			statusProgressBar.Minimum = 0;
			statusProgressBar.Maximum = 1;
			
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
		OperationStatus currentStatus;
		SolidColorBrush progressForegroundBrush;
		
		public void DisplayProgress(string taskName, double workDone, OperationStatus status)
		{
			if (!statusProgressBarIsVisible) {
				statusProgressBarItem.Visibility = Visibility.Visible;
				statusProgressBarIsVisible = true;
			}
			
			if (double.IsNaN(workDone)) {
				statusProgressBar.IsIndeterminate = true;
				status = OperationStatus.Normal; // indeterminate doesn't support foreground color
			} else {
				statusProgressBar.IsIndeterminate = false;
				statusProgressBar.Value = workDone;
			}
			
			if (status != currentStatus) {
				if (progressForegroundBrush == null) {
					SolidColorBrush defaultForeground = statusProgressBar.Foreground as SolidColorBrush;
					progressForegroundBrush = new SolidColorBrush(defaultForeground != null ? defaultForeground.Color : Colors.Blue);
				}
				
				if (status == OperationStatus.Error) {
					statusProgressBar.Foreground = progressForegroundBrush;
					progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
						Colors.Red, new Duration(TimeSpan.FromSeconds(0.6)), FillBehavior.HoldEnd));
				} else if (status == OperationStatus.Warning) {
					statusProgressBar.Foreground = progressForegroundBrush;
					progressForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(
						Colors.YellowGreen, new Duration(TimeSpan.FromSeconds(0.6)), FillBehavior.HoldEnd));
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
			statusProgressBarIsVisible = false;
			statusProgressBarItem.Visibility = Visibility.Collapsed;
			jobNamePanel.Content = currentTaskName = "";
		}
	}
}
