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
