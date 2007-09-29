// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdStatusBar : StatusStrip
	{
		ToolStripProgressBar statusProgressBar = new ToolStripProgressBar();
		ToolStripStatusLabel jobNamePanel      = new ToolStripStatusLabel();
		
		ToolStripStatusLabel txtStatusBarPanel    = new ToolStripStatusLabel();
		ToolStripStatusLabel cursorStatusBarPanel = new ToolStripStatusLabel();
		ToolStripStatusLabel modeStatusBarPanel   = new ToolStripStatusLabel();
		ToolStripStatusLabel springLabel          = new ToolStripStatusLabel();
		
		public ToolStripStatusLabel  CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public ToolStripStatusLabel  ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
		
		public SdStatusBar()
		{
			
			//			txtStatusBarPanel.Width = 500;
			//			txtStatusBarPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			//			Panels.Add(txtStatusBarPanel);
			//	//		manager.Add(new StatusBarContributionItem("TextPanel", txtStatusBarPanel));
			//
			//			statusProgressBar.Width  = 200;
			//			statusProgressBar.Height = 14;
			//			statusProgressBar.Location = new Point(160, 6);
			//			statusProgressBar.Minimum = 0;
			//			statusProgressBar.Visible = false;
			//			Controls.Add(statusProgressBar);
			//
			//			cursorStatusBarPanel.Width = 200;
			//			cursorStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			//			cursorStatusBarPanel.Alignment = HorizontalAlignment.Left;
			//			Panels.Add(cursorStatusBarPanel);
			//
			//			modeStatusBarPanel.Width = 44;
			//			modeStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			//			modeStatusBarPanel.Alignment = HorizontalAlignment.Right;
			//			Panels.Add(modeStatusBarPanel);
			
			springLabel.Spring = true;
			cursorStatusBarPanel.AutoSize = false;
			cursorStatusBarPanel.Width = 150;
			modeStatusBarPanel.AutoSize = false;
			modeStatusBarPanel.Width = 25;
			statusProgressBar.Visible = false;
			statusProgressBar.Width = 100;
			
			Items.AddRange(new ToolStripItem[] { txtStatusBarPanel, springLabel, jobNamePanel, statusProgressBar, cursorStatusBarPanel, modeStatusBarPanel });
		}
		
		public void ShowErrorMessage(string message)
		{
			SetMessage("Error : " + message);
		}
		
		public void ShowErrorMessage(Image image, string message)
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
					txtStatusBarPanel.BackColor = SystemColors.Highlight;
					txtStatusBarPanel.ForeColor = Color.White;
				} else if (txtStatusBarPanel.BackColor == SystemColors.Highlight) {
					txtStatusBarPanel.BackColor = SystemColors.Control;
					txtStatusBarPanel.ForeColor = SystemColors.ControlText;
				}
				txtStatusBarPanel.Text = message;
			};
			if (WorkbenchSingleton.InvokeRequired)
				WorkbenchSingleton.SafeThreadAsyncCall(setMessageAction);
			else
				setMessageAction();
		}
		
		public void SetMessage(Image image, string message)
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
						statusProgressBar.Visible = true;
						statusProgressBarIsVisible = true;
					}
					
					if (totalWork == 0) {
						statusProgressBar.Style = ProgressBarStyle.Marquee;
					} else {
						statusProgressBar.Style = ProgressBarStyle.Continuous;
						if (statusProgressBar.Maximum != totalWork) {
							if (statusProgressBar.Value > totalWork)
								statusProgressBar.Value = 0;
							statusProgressBar.Maximum = totalWork;
						}
						statusProgressBar.Value = workDone;
					}
					
					if (currentTaskName != taskName) {
						currentTaskName = taskName;
						jobNamePanel.Text = StringParser.Parse(taskName);
					}
				});
		}
		
		public void HideProgress()
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					statusProgressBarIsVisible = false;
					statusProgressBar.Visible = false;
					jobNamePanel.Text = currentTaskName = "";
				});
		}
	}
}
