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
		ProgressBar statusProgressBar      = new ProgressBar();
		
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
			
			Items.AddRange(new ToolStripItem[] { txtStatusBarPanel, cursorStatusBarPanel, modeStatusBarPanel });

			
	//		manager.Add(new StatusBarContributionItem("ProgressBar", statusProgressBar));
		}
		
		public void ShowErrorMessage(string message)
		{
			txtStatusBarPanel.Text = "Error : " + message;
		}
		
		public void ShowErrorMessage(Image image, string message)
		{
			txtStatusBarPanel.Text = "Error : " + message;
		}
		
		public void SetMessage(string message)
		{
			txtStatusBarPanel.Text = message;
		}
		
		public void SetMessage(Image image, string message)
		{
			txtStatusBarPanel.Text = message;
		}
		
		// Progress Monitor implementation
		string oldMessage = null;
		public void BeginTask(string name, int totalWork)
		{
			oldMessage = txtStatusBarPanel.Text;
			SetMessage(name);
			statusProgressBar.Maximum = totalWork;
			statusProgressBar.Left = txtStatusBarPanel.Width - statusProgressBar.Width - 4; 
			statusProgressBar.Visible = true;
		}
		
		public void Worked(int work)
		{
			statusProgressBar.Value = work;
		}
		
		public void Done()
		{
			SetMessage(oldMessage);
			oldMessage = null;
			statusProgressBar.Visible = false;
		}
		
		public bool Canceled {
			get {
				return oldMessage == null;
			}
			set {
				Done();
			}
		}
		
		public string TaskName {
			get {
				return "";
			}
			set {
				
			}
		}
	}
}
