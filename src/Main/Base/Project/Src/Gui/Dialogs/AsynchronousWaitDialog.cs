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
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui
{
	internal sealed partial class AsynchronousWaitDialogForm
	{
		internal AsynchronousWaitDialogForm(bool allowCancel)
		{
			InitializeComponent();
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			
			if (allowCancel) {
				cancelButton.Visible = true;
				progressBar.Width = cancelButton.Left - 8 - progressBar.Left;
			} else {
				cancelButton.Visible = false;
				progressBar.Width = cancelButton.Right - progressBar.Left;
			}
		}
	}
	
	/// <summary>
	/// Shows an wait dialog on a separate thread if the action takes longer than 500ms.
	/// Usage:
	/// using (AsynchronousWaitDialog.ShowWaitDialog("title")) {
	///   long_running_action();
	/// }
	/// or:
	/// using (IProgressMonitor monitor = AsynchronousWaitDialog.ShowWaitDialog("title")) {
	///   long_running_action(monitor);
	/// }
	/// </summary>
	public sealed class AsynchronousWaitDialog : IProgressMonitor
	{
		/// <summary>
		/// Delay until the wait dialog becomes visible, in ms.
		/// </summary>
		public const int ShowWaitDialogDelay = 500;
		
		readonly string titleName;
		readonly string defaultTaskName;
		readonly CancellationTokenSource cancellation;
		readonly ProgressCollector collector;
		readonly bool runningInOwnThread;
		readonly Action<IProgressMonitor> asyncOperation;
		
		readonly SynchronizationHelper synchronizationHelper = new SynchronizationHelper();
		AsynchronousWaitDialogForm dlg;
		
		#region Constructors
		/// <summary>
		/// Shows a wait dialog.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <returns>AsynchronousWaitDialog object - you can use it to access the wait dialog's properties.
		/// To close the wait dialog, call Dispose() on the AsynchronousWaitDialog object</returns>
		public static AsynchronousWaitDialog ShowWaitDialog(string titleName)
		{
			return ShowWaitDialog(titleName, null, false);
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
			return ShowWaitDialog(titleName, null, allowCancel);
		}
		
		/// <summary>
		/// Shows a wait dialog that does not support cancelling.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <param name="allowCancel">Specifies whether a cancel button should be shown.</param>
		/// <param name="defaultTaskName">The default description text, if no named task is active.</param>
		/// <returns>AsynchronousWaitDialog object - you can use it to access the wait dialog's properties.
		/// To close the wait dialog, call Dispose() on the AsynchronousWaitDialog object</returns>
		public static AsynchronousWaitDialog ShowWaitDialog(string titleName, string defaultTaskName, bool allowCancel)
		{
			if (titleName == null)
				throw new ArgumentNullException("titleName");
			AsynchronousWaitDialog h = new AsynchronousWaitDialog(titleName, defaultTaskName, allowCancel, null);
			h.StartInThread();
			return h;
		}
		
		/// <summary>
		/// Shows a wait dialog that does supports cancelling.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <param name="defaultTaskName">The default description text, if no named task is active.</param>
		/// <param name="action">The action to run within the wait dialog</param>
		public static void RunInCancellableWaitDialog(string titleName, string defaultTaskName, Action<AsynchronousWaitDialog> action)
		{
			if (titleName == null)
				throw new ArgumentNullException("titleName");
			using (AsynchronousWaitDialog h = new AsynchronousWaitDialog(titleName, defaultTaskName, true, null)) {
				h.StartInThread();
				try {
					action(h);
				} catch (OperationCanceledException ex) {
					// consume OperationCanceledException
					if (ex.CancellationToken != h.CancellationToken)
						throw;
				}
			}
		}
		
		/// <summary>
		/// Shows a wait dialog that does not support cancelling and waits for an asynchronous operation to end.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <param name="asyncOperation">Asynchronous operation to be executed.</param>
		public static void ShowWaitDialogForAsyncOperation(string titleName, Action<IProgressMonitor> asyncOperation)
		{
			ShowWaitDialogForAsyncOperation(titleName, null, asyncOperation);
		}
		
		/// <summary>
		/// Shows a wait dialog that does not support cancelling and waits for an asynchronous operation to end.
		/// </summary>
		/// <param name="titleName">Title of the wait dialog</param>
		/// <param name="allowCancel">Specifies whether a cancel button should be shown.</param>
		/// <param name="defaultTaskName">The default description text, if no named task is active.</param>
		/// <param name="asyncOperation">Asynchronous operation to be executed.</param>
		/// <returns>AsynchronousWaitDialog object - you can use it to access the wait dialog's properties.
		/// To close the wait dialog, call Dispose() on the AsynchronousWaitDialog object</returns>
		public static void ShowWaitDialogForAsyncOperation(string titleName, string defaultTaskName, Action<IProgressMonitor> asyncOperation)
		{
			if (titleName == null)
				throw new ArgumentNullException("titleName");
			AsynchronousWaitDialog h = new AsynchronousWaitDialog(titleName, defaultTaskName, false, asyncOperation);
			h.StartWithAsyncOperation();
		}
		
		private AsynchronousWaitDialog(string titleName, string defaultTaskName, bool allowCancel, Action<IProgressMonitor> asyncOperation)
		{
			this.titleName = StringParser.Parse(titleName);
			this.defaultTaskName = StringParser.Parse(defaultTaskName ?? "${res:Global.PleaseWait}");
			if (allowCancel)
				this.cancellation = new CancellationTokenSource();
			this.asyncOperation = asyncOperation;
			this.runningInOwnThread = (asyncOperation == null);
			this.collector = new ProgressCollector(synchronizationHelper, allowCancel ? cancellation.Token : CancellationToken.None);
		}
		#endregion
		
		#region SynchronizationHelper
		// this class works around the issue that we don't initially have an ISynchronizeInvoke implementation
		// for the target thread, we only create it after ShowWaitDialogDelay.
		sealed class SynchronizationHelper : ISynchronizeInvoke
		{
			volatile ISynchronizeInvoke targetSynchronizeInvoke;
			
			public bool InvokeRequired {
				get {
					ISynchronizeInvoke si = targetSynchronizeInvoke;
					return si != null && si.InvokeRequired;
				}
			}
			
			public IAsyncResult BeginInvoke(Delegate method, object[] args)
			{
				ISynchronizeInvoke si = targetSynchronizeInvoke;
				if (si != null) {
					var winForm = si as System.Windows.Forms.Form;
					if ((winForm == null) || !winForm.IsDisposed)
						return si.BeginInvoke(method, args);
				}
				
				// When target is not available, invoke method on current thread, but use a lock
				// to ensure we don't run multiple methods concurrently.
				lock (this) {
					method.DynamicInvoke(args);
					return null;
				}
				// yes this is morally questionable - maybe it would be better to enqueue all invocations and run them later?
				// ProgressCollector would have to avoid enqueuing stuff multiple times for all kinds of updates
				// (currently it does this only with updates to the Progress property)
			}
			
			public void SetTarget(ISynchronizeInvoke targetSynchronizeInvoke)
			{
				lock (this) {
					this.targetSynchronizeInvoke = targetSynchronizeInvoke;
				}
			}
			
			public object EndInvoke(IAsyncResult result)
			{
				throw new NotSupportedException();
			}
			
			public object Invoke(Delegate method, object[] args)
			{
				throw new NotSupportedException();
			}
		}
		#endregion
		
		#region Start waiting thread
		/// <summary>
		/// Start waiting thread
		/// </summary>
		internal void StartInThread()
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
			
			if (CreateDialogForm()) {
				if (collector.ShowingDialog) {
					Application.Run();
				} else {
					Application.Run(dlg);
				}
			}
		}
		#endregion
		
		internal void StartWithAsyncOperation()
		{
			// Start asynchronous operation
			if (asyncOperation == null)
				return;
			asyncOperation(collector.ProgressMonitor);
			
			// Wait delay before showing dialog
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = ShowWaitDialogDelay;
			timer.Tick += (sender, e) =>
			{
				timer.Stop();
				timer.Dispose();
				
				// Create and show dialog
				if (CreateDialogForm())
					dlg.ShowDialog();
			};
			timer.Start();
		}
		
		bool CreateDialogForm()
		{
			if (collector.ProgressMonitorIsDisposed)
				return false;
			
			dlg = new AsynchronousWaitDialogForm(cancellation != null);
			dlg.Text = titleName;
			dlg.cancelButton.Click += CancelButtonClick;
			dlg.CreateControl();
			IntPtr h = dlg.Handle; // force handle creation
			
			// ensure events occur on this thread, then register event handlers
			synchronizationHelper.SetTarget(dlg);
			collector.ProgressMonitorDisposed += progress_ProgressMonitorDisposed;
			collector.PropertyChanged += progress_PropertyChanged;
			
			// check IsDisposed once again (we might have missed an event while we initialized the dialog):
			if (collector.ProgressMonitorIsDisposed) {
				dlg.Dispose();
				return false;
			}
			
			progress_PropertyChanged(null, new System.ComponentModel.PropertyChangedEventArgs("TaskName"));
			
			return true;
		}
		
		/// <summary>
		/// Closes the wait dialog.
		/// </summary>
		void progress_ProgressMonitorDisposed(object sender, EventArgs e)
		{
			dlg.Dispose();
			if (runningInOwnThread)
				Application.ExitThread();
		}
		
		bool reshowTimerRunning = false;
		
		void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// show/hide dialog as required by ShowingDialog
			if (dlg.Visible == collector.ShowingDialog) {
				if (collector.ShowingDialog) {
					dlg.Hide();
				} else if (!reshowTimerRunning) {
					reshowTimerRunning = true;
					var timer = new System.Windows.Forms.Timer();
					timer.Interval = 100;
					timer.Tick += delegate {
						timer.Dispose();
						reshowTimerRunning = false;
						if (!collector.ShowingDialog) {
							if (runningInOwnThread)
								dlg.Show();
							else
								if (!dlg.Visible)
									dlg.ShowDialog();
						}
					};
					timer.Start();
				}
			}
			
			// update task name and progress
			if (e.PropertyName == "TaskName")
				dlg.taskLabel.Text = collector.TaskName ?? defaultTaskName;
			if (double.IsNaN(collector.Progress)) {
				dlg.progressBar.Style = ProgressBarStyle.Marquee;
			} else {
				dlg.progressBar.Style = ProgressBarStyle.Continuous;
				dlg.progressBar.Value = Math.Max(0, Math.Min(100, (int)(collector.Progress * 100)));
			}
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			dlg.cancelButton.Enabled = false;
			if (cancellation != null)
				cancellation.Cancel();
		}
		
		#region IProgressMonitor interface impl (forwards to progress.ProgressMonitor)
		/// <inheritdoc/>
		public string TaskName {
			get { return collector.ProgressMonitor.TaskName; }
			set { collector.ProgressMonitor.TaskName = value; }
		}
		
		/// <inheritdoc/>
		public double Progress {
			get { return collector.ProgressMonitor.Progress; }
			set { collector.ProgressMonitor.Progress = value; }
		}
		
		void IProgress<double>.Report(double value)
		{
			this.Progress = value;
		}
		
		/// <inheritdoc/>
		public bool ShowingDialog {
			get { return collector.ProgressMonitor.ShowingDialog; }
			set { collector.ProgressMonitor.ShowingDialog = value; }
		}
		
		/// <inheritdoc/>
		public OperationStatus Status {
			get { return collector.ProgressMonitor.Status; }
			set { collector.ProgressMonitor.Status = value; }
		}
		
		/// <inheritdoc/>
		public CancellationToken CancellationToken {
			get { return collector.ProgressMonitor.CancellationToken; }
		}
		
		/// <inheritdoc/>
		public IProgressMonitor CreateSubTask(double workAmount)
		{
			return collector.ProgressMonitor.CreateSubTask(workAmount);
		}
		
		/// <inheritdoc/>
		public IProgressMonitor CreateSubTask(double workAmount, CancellationToken cancellationToken)
		{
			return collector.ProgressMonitor.CreateSubTask(workAmount, cancellationToken);
		}
		
		public void Dispose()
		{
			collector.ProgressMonitor.Dispose();
		}
		#endregion
	}
}
