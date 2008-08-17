// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	internal sealed partial class AsynchronousWaitDialogForm
	{
		internal AsynchronousWaitDialogForm()
		{
			InitializeComponent();
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
		}
	}
	
	/// <summary>
	/// Shows an wait dialog on a separate thread if the action takes longer than 500ms.
	/// Usage:
	/// using (AsynchronousWaitDialog.ShowWaitDialog("title")) {
	///   long_running_action();
	/// }
	/// or:
	/// using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("title")) {
	///   long_running_action(monitor);
	/// }
	/// </summary>
	public sealed class AsynchronousWaitDialog : IProgressMonitor, IDisposable
	{
		/// <summary>
		/// Delay until the wait dialog becomes visible, in ms.
		/// </summary>
		public const int ShowWaitDialogDelay = 500;
		
		readonly object lockObject = new object();
		bool disposed;
		AsynchronousWaitDialogForm dlg;
		volatile int totalWork;
		volatile string titleName;
		volatile string taskName;
		volatile int workDone;
		volatile bool cancelled;
		volatile bool allowCancel;
		
		/// <summary>
		/// Shows a wait dialog.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <returns>AsynchronousWaitDialog object - you can use it to access the wait dialog's properties.
		/// To close the wait dialog, call Dispose() on the AsynchronousWaitDialog object</returns>
		public static AsynchronousWaitDialog ShowWaitDialog(string titleName)
		{
			if (titleName == null)
				throw new ArgumentNullException("titleName");
			AsynchronousWaitDialog h = new AsynchronousWaitDialog(titleName, false);
			h.Start();
			return h;
		}
		
		/// <summary>
		/// Shows a wait dialog.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <param name="allowCancel">Specifies whether a cancel button should be shown.</param>
		/// <returns>AsynchronousWaitDialog object - you can use it to access the wait dialog's properties.
		/// To close the wait dialog, call Dispose() on the AsynchronousWaitDialog object</returns>
		public static AsynchronousWaitDialog ShowWaitDialog(string titleName, bool allowCancel)
		{
			if (titleName == null)
				throw new ArgumentNullException("titleName");
			AsynchronousWaitDialog h = new AsynchronousWaitDialog(titleName, allowCancel);
			h.Start();
			return h;
		}
		
		private AsynchronousWaitDialog(string titleName, bool allowCancel)
		{
			this.titleName = titleName;
			Done(); // set default values for titleName
			this.allowCancel = allowCancel;
		}
		
		#region Start waiting thread
		/// <summary>
		/// Start waiting thread
		/// </summary>
		internal void Start()
		{
			Thread newThread = new Thread(Run);
			newThread.Name = "AsynchronousWaitDialog thread";
			newThread.Start();
			
			Thread.Sleep(0); // allow new thread to start
		}
		
		[STAThread]
		void Run()
		{
			Thread.Sleep(ShowWaitDialogDelay);
			bool isShowingDialog;
			lock (lockObject) {
				if (disposed)
					return;
				dlg = new AsynchronousWaitDialogForm();
				dlg.Text = StringParser.Parse(titleName);
				dlg.cancelButton.Click += CancelButtonClick;
				UpdateTask();
				dlg.CreateControl();
				IntPtr h = dlg.Handle; // force handle creation
				isShowingDialog = showingDialog;
			}
			if (isShowingDialog) {
				Application.Run();
			} else {
				Application.Run(dlg);
			}
		}
		#endregion
		
		/// <summary>
		/// Closes the wait dialog.
		/// </summary>
		public void Dispose()
		{
			lock (lockObject) {
				if (disposed) return;
				disposed = true;
				if (dlg != null) {
					dlg.BeginInvoke(new MethodInvoker(DisposeInvoked));
				}
			}
		}
		
		void DisposeInvoked()
		{
			dlg.Dispose();
			Application.ExitThread();
		}
		
		public int WorkDone {
			get {
				return workDone;
			}
			set {
				if (workDone != value) {
					lock (lockObject) {
						workDone = value;
						if (dlg != null && disposed == false) {
							dlg.BeginInvoke(new MethodInvoker(UpdateProgress));
						}
					}
				}
			}
		}
		
		public string TaskName {
			get {
				lock (lockObject) {
					return taskName;
				}
			}
			set {
				if (taskName != value) {
					lock (lockObject) {
						taskName = value;
						if (dlg != null && disposed == false) {
							dlg.BeginInvoke(new MethodInvoker(UpdateTask));
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Begins a new task with the specified name and total amount of work.
		/// </summary>
		/// <param name="name">Name of the task. Use null to display "please wait..." message</param>
		/// <param name="totalWork">Total amount of work in work units. Use 0 for unknown amount of work.</param>
		/// <param name="allowCancel">Specifies whether the task can be cancelled.</param>
		public void BeginTask(string name, int totalWork, bool allowCancel)
		{
			if (name == null)
				name = "${res:Global.PleaseWait}";
			if (totalWork < 0)
				totalWork = 0;
			
			lock (lockObject) {
				this.allowCancel = allowCancel;
				this.totalWork = totalWork;
				this.taskName = name;
				if (dlg != null && disposed == false) {
					dlg.BeginInvoke(new MethodInvoker(UpdateTask));
				}
			}
		}
		
		/// <summary>
		/// Resets the task to a generic "please wait" with marquee progress bar.
		/// </summary>
		public void Done()
		{
			workDone = 0;
			BeginTask(null, 0, false);
		}
		
		void UpdateTask()
		{
			int totalWork = this.totalWork;
			
			dlg.taskLabel.Text = StringParser.Parse(taskName);
			if (allowCancel) {
				dlg.cancelButton.Visible = true;
				dlg.progressBar.Width = dlg.cancelButton.Left - 8 - dlg.progressBar.Left;
			} else {
				dlg.cancelButton.Visible = false;
				dlg.progressBar.Width = dlg.cancelButton.Right - dlg.progressBar.Left;
			}
			
			if (totalWork > 0) {
				if (dlg.progressBar.Value > totalWork) {
					dlg.progressBar.Value = 0;
				}
				dlg.progressBar.Maximum = totalWork + 1;
				dlg.progressBar.Style = ProgressBarStyle.Continuous;
			} else {
				dlg.progressBar.Style = ProgressBarStyle.Marquee;
			}
			UpdateProgress();
		}
		
		void UpdateProgress()
		{
			int workDone = this.workDone;
			if (workDone < 0) workDone = 0;
			if (workDone > dlg.progressBar.Maximum)
				workDone = dlg.progressBar.Maximum;
			dlg.progressBar.Value = workDone;
		}
		
		bool showingDialog;
		
		public bool ShowingDialog {
			get { return showingDialog; }
			set {
				if (showingDialog != value) {
					lock (lockObject) {
						showingDialog = value;
						if (dlg != null && disposed == false) {
							if (value) {
								dlg.BeginInvoke(new MethodInvoker(dlg.Hide));
							} else {
								dlg.BeginInvoke(new MethodInvoker(delegate {
								                                  	Thread.Sleep(100);
								                                  	if (showingDialog) {
								                                  		dlg.Show();
								                                  	}
								                                  }));
							}
						}
					}
				}
			}
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			dlg.cancelButton.Enabled = false;
			if (!cancelled) {
				cancelled = true;
				EventHandler eh = Cancelled;
				if (eh != null) {
					eh(this, e);
				}
			}
		}
		
		public bool IsCancelled {
			get {
				return cancelled;
			}
		}
		
		public event EventHandler Cancelled;
	}
}
