// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdStatusBar : StatusStrip, IProgressMonitor
	{
		ToolStripProgressBar statusProgressBar     = new ToolStripProgressBar();
		ToolStripStatusLabel jobNamePanel          = new ToolStripStatusLabel();
		
		ToolStripStatusLabel  txtStatusBarPanel    = new ToolStripStatusLabel();
		ToolStripStatusLabel  cursorStatusBarPanel = new ToolStripStatusLabel();
		ToolStripStatusLabel  modeStatusBarPanel   = new ToolStripStatusLabel();
		
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
		
		bool cancelEnabled;
		
		public bool CancelEnabled {
			get {
				return cancelEnabled;
			}
			set {
				cancelEnabled = value;
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
			ToolStripStatusLabel  springLabel = new ToolStripStatusLabel();
			springLabel.Spring = true;
			jobNamePanel.Visible = false;
			statusProgressBar.Visible = false;
			statusProgressBar.Size = new Size(100, 20);
			
			Items.AddRange(new ToolStripItem[] { txtStatusBarPanel, springLabel, jobNamePanel, statusProgressBar, cursorStatusBarPanel, modeStatusBarPanel });
		}
		
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			UpdateText();
		}
		
		public void ShowErrorMessage(string message)
		{
			SetMessage("Error : " + message);
		}
		
		public void ShowErrorMessage(Image image, string message)
		{
			SetMessage(image, "Error : " + message);
		}
		
		string currentMessage;
		
		public void SetMessage(string message)
		{
			currentMessage = message;
			if (this.IsHandleCreated)
				BeginInvoke(new MethodInvoker(UpdateText));
		}
		
		void UpdateText()
		{
			txtStatusBarPanel.Text = currentMessage;
		}
		
		public void SetMessage(Image image, string message)
		{
			SetMessage(message);
		}
		
		// Progress Monitor implementation
		int totalWork;
		
		public void BeginTask(string name, int totalWork)
		{
			taskName = name;
			this.totalWork = totalWork;
			this.BeginInvoke(new MethodInvoker(MakeVisible));
		}
		
		void MakeVisible()
		{
			statusProgressBar.Value = 0;
			statusProgressBar.Maximum = totalWork;
			jobNamePanel.Text = taskName;
			jobNamePanel.Visible = true;
			statusProgressBar.Visible = true;
		}
		
		void MakeInvisible()
		{
			jobNamePanel.Visible = false;
			statusProgressBar.Visible = false;
		}
		
		int workDone;
		
		public int WorkDone {
			get {
				return workDone;
			}
			set {
				if (workDone == value) return;
				workDone = value;
				this.BeginInvoke(new MethodInvoker(SetWorkDone));
			}
		}
		
		void SetWorkDone()
		{
			statusProgressBar.Value = workDone;
		}
		
		public void Done()
		{
			taskName = null;
			this.BeginInvoke(new MethodInvoker(MakeInvisible));
		}
		
		string taskName;
		
		public string TaskName {
			get {
				return taskName;
			}
		}
	}
}
