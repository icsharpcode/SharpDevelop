// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Collects progress using nested IProgressMonitors and provides it to a different thread using events.
	/// </summary>
	public sealed class ProgressCollector : INotifyPropertyChanged
	{
		readonly ISynchronizeInvoke eventThread;
		readonly CancellationToken cancellationToken;
		readonly MonitorImpl root;
		readonly LinkedList<string> namedMonitors = new LinkedList<string>();
		readonly object updateLock = new object();
		
		string taskName;
		double progress;
		OperationStatus status;
		bool showingDialog;
		bool rootMonitorIsDisposed;
		
		public ProgressCollector(ISynchronizeInvoke eventThread, CancellationToken cancellationToken)
		{
			if (eventThread == null)
				throw new ArgumentNullException("eventThread");
			this.eventThread = eventThread;
			this.cancellationToken = cancellationToken;
			this.root = new MonitorImpl(this, null, 1);
		}
		
		public event EventHandler ProgressMonitorDisposed;
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public double Progress {
			get { return progress; }
			private set {
				Debug.Assert(!eventThread.InvokeRequired);
				if (progress != value) {
					progress = value;
					// Defensive programming: parallel processes like the build could change properites even
					// after the monitor is disposed (they shouldn't do that, but it could happen),
					// and we don't want to confuse consumers like the status bar by
					// raising events from disposed monitors.
					if (!rootMonitorIsDisposed)
						OnPropertyChanged("Progress");
				}
			}
		}
		
		public bool ShowingDialog {
			get { return showingDialog; }
			set {
				Debug.Assert(!eventThread.InvokeRequired);
				if (showingDialog != value) {
					showingDialog = value;
					if (!rootMonitorIsDisposed)
						OnPropertyChanged("ShowingDialog");
				}
			}
		}
		
		public string TaskName {
			get { return taskName; }
			private set {
				Debug.Assert(!eventThread.InvokeRequired);
				if (taskName != value) {
					taskName = value;
					if (!rootMonitorIsDisposed)
						OnPropertyChanged("TaskName");
				}
			}
		}
		
		public OperationStatus Status {
			get { return status; }
			private set {
				Debug.Assert(!eventThread.InvokeRequired);
				if (status != value) {
					status = value;
					if (!rootMonitorIsDisposed)
						OnPropertyChanged("Status");
				}
			}
		}
		
		public IProgressMonitor ProgressMonitor {
			get { return root; }
		}
		
		/// <summary>
		/// Gets whether the root progress monitor was disposed.
		/// </summary>
		public bool ProgressMonitorIsDisposed {
			get { return rootMonitorIsDisposed; }
		}
		
		bool hasUpdateScheduled;
		double storedNewProgress = -1;
		OperationStatus storedNewStatus;
		
		void SetProgress(double newProgress)
		{
			// this method is always called within a lock(updateLock) block, so we don't
			// have to worry about thread safety when accessing hasUpdateScheduled and storedNewProgress
			
			storedNewProgress = newProgress;
			ScheduleUpdate();
		}
		
		void ScheduleUpdate()
		{
			// This test ensures that only 1 update is scheduled at a single point in time. If updates
			// come in faster than the GUI can process them, we'll skip some and directly update to the newest value.
			if (!hasUpdateScheduled) {
				hasUpdateScheduled = true;
				eventThread.BeginInvoke(
					(Action)delegate {
						lock (updateLock) {
							this.Progress = storedNewProgress;
							this.Status = storedNewStatus;
							hasUpdateScheduled = false;
						}
					},
					null
				);
			}
		}
		
		void SetStatus(OperationStatus newStatus)
		{
			// this method is always called within a lock(updateLock) block, so we don't
			// have to worry about thread safety when accessing hasUpdateScheduled and storedNewStatus
			
			storedNewStatus = newStatus;
			ScheduleUpdate();
		}
		
		void SetShowingDialog(bool newValue)
		{
			eventThread.BeginInvoke(
				(Action)delegate { this.ShowingDialog = newValue; },
				null
			);
		}
		
		void OnRootMonitorDisposed()
		{
			eventThread.BeginInvoke(
				(Action)delegate {
					if (rootMonitorIsDisposed) // ignore double dispose
						return;
					rootMonitorIsDisposed = true;
					if (ProgressMonitorDisposed != null) {
						ProgressMonitorDisposed(this, EventArgs.Empty);
					}
				},
				null);
		}
		
		void SetTaskName(string newName)
		{
			eventThread.BeginInvoke(
				(Action)delegate { this.TaskName = newName; },
				null);
		}
		
		LinkedListNode<string> RegisterNamedMonitor(string name)
		{
			lock (namedMonitors) {
				LinkedListNode<string> newEntry = namedMonitors.AddLast(name);
				if (namedMonitors.First == newEntry) {
					SetTaskName(name);
				}
				return newEntry;
			}
		}
		
		void UnregisterNamedMonitor(LinkedListNode<string> nameEntry)
		{
			lock (namedMonitors) {
				bool wasFirst = namedMonitors.First == nameEntry;
				namedMonitors.Remove(nameEntry);
				if (wasFirst)
					SetTaskName(namedMonitors.First != null ? namedMonitors.First.Value : null);
			}
		}
		
		void ChangeName(LinkedListNode<string> nameEntry, string newName)
		{
			lock (namedMonitors) {
				if (namedMonitors.First == nameEntry)
					SetTaskName(newName);
				nameEntry.Value = newName;
			}
		}
		
		sealed class MonitorImpl : IProgressMonitor
		{
			readonly ProgressCollector collector;
			readonly MonitorImpl parent;
			readonly double scaleFactor;
			LinkedListNode<string> nameEntry;
			double currentProgress;
			OperationStatus localStatus, currentStatus;
			int childrenWithWarnings, childrenWithErrors;
			
			public MonitorImpl(ProgressCollector collector, MonitorImpl parent, double scaleFactor)
			{
				this.collector = collector;
				this.parent = parent;
				this.scaleFactor = scaleFactor;
			}
			
			public bool ShowingDialog {
				get { return collector.ShowingDialog; }
				set { collector.SetShowingDialog(value); }
			}
			
			public string TaskName {
				get {
					if (nameEntry != null)
						return nameEntry.Value;
					else
						return null;
				}
				set {
					if (nameEntry != null) {
						if (value == null) {
							collector.UnregisterNamedMonitor(nameEntry);
							nameEntry = null;
						} else {
							if (nameEntry.Value != value)
								collector.ChangeName(nameEntry, value);
						}
					} else {
						if (value != null)
							nameEntry = collector.RegisterNamedMonitor(value);
					}
				}
			}
			
			public CancellationToken CancellationToken {
				get { return collector.cancellationToken; }
			}
			
			public double Progress {
				get { return currentProgress; }
				set {
					lock (collector.updateLock) {
						UpdateProgress(value);
					}
				}
			}
			
			void UpdateProgress(double progress)
			{
				if (parent != null)
					parent.UpdateProgress(parent.currentProgress + (progress - this.currentProgress) * scaleFactor);
				else
					collector.SetProgress(progress);
				this.currentProgress = progress;
			}
			
			public OperationStatus Status {
				get { return localStatus; }
				set {
					if (localStatus != value) {
						localStatus = value;
						lock (collector.updateLock) {
							UpdateStatus();
						}
					}
				}
			}
			
			void UpdateStatus()
			{
				OperationStatus oldStatus = currentStatus;
				if (childrenWithErrors > 0)
					currentStatus = OperationStatus.Error;
				else if (childrenWithWarnings > 0 && localStatus != OperationStatus.Error)
					currentStatus = OperationStatus.Warning;
				else
					currentStatus = localStatus;
				if (oldStatus != currentStatus) {
					if (parent != null) {
						if (oldStatus == OperationStatus.Warning)
							parent.childrenWithWarnings--;
						else if (oldStatus == OperationStatus.Error)
							parent.childrenWithErrors--;
						
						if (currentStatus == OperationStatus.Warning)
							parent.childrenWithWarnings++;
						else if (currentStatus == OperationStatus.Error)
							parent.childrenWithErrors++;
						
						parent.UpdateStatus();
					} else {
						collector.SetStatus(currentStatus);
					}
				}
			}
			
			public IProgressMonitor CreateSubTask(double workAmount)
			{
				return new MonitorImpl(collector, this, workAmount);
			}
			
			public void Dispose()
			{
				this.TaskName = null;
				if (parent == null)
					collector.OnRootMonitorDisposed();
			}
		}
	}
}
