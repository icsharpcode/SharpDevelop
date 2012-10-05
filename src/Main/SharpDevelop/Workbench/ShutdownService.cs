// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class ShutdownService : IShutdownService
	{
		CancellationTokenSource shutdownCTS = new CancellationTokenSource();
		CancellationTokenSource delayedShutdownCTS = new CancellationTokenSource();
		
		public CancellationToken ShutdownToken {
			get { return shutdownCTS.Token; }
		}
		
		public CancellationToken DelayedShutdownToken {
			get { return delayedShutdownCTS.Token; }
		}
		
		internal void SignalShutdownToken()
		{
			shutdownCTS.Cancel();
			delayedShutdownCTS.CancelAfter(2000);
		}
		
		public bool Shutdown()
		{
			SD.Workbench.MainWindow.Close();
			return ((WpfWorkbench)SD.Workbench).WorkbenchLayout == null;
		}
		
		#region PreventShutdown
		List<string> reasonsPreventingShutdown = new List<string>();
		
		public IDisposable PreventShutdown(string reason)
		{
			lock (reasonsPreventingShutdown) {
				reasonsPreventingShutdown.Add(reason);
			}
			return new CallbackOnDispose(
				delegate {
					lock (reasonsPreventingShutdown) {
						reasonsPreventingShutdown.Remove(reason);
					}
				});
		}
		
		public string CurrentReasonPreventingShutdown {
			get {
				lock (reasonsPreventingShutdown) {
					return reasonsPreventingShutdown.FirstOrDefault();
				}
			}
		}
		#endregion
		
		#region Background Tasks
		int outstandingBackgroundTasks;
		ManualResetEventSlim backgroundTaskEvent = new ManualResetEventSlim(true);
		
		public void AddBackgroundTask(Task task)
		{
			backgroundTaskEvent.Reset();
			Interlocked.Increment(ref outstandingBackgroundTasks);
			task.ContinueWith(
				delegate {
					if (Interlocked.Decrement(ref outstandingBackgroundTasks) == 0) {
						backgroundTaskEvent.Set();
					}
				});
		}
		
		internal void WaitForBackgroundTasks()
		{
			if (!backgroundTaskEvent.IsSet) {
				SD.Log.Info("Waiting for background tasks to finish...");
				backgroundTaskEvent.Wait();
				SD.Log.Info("Background tasks have finished.");
			}
		}
		#endregion
	}
}
