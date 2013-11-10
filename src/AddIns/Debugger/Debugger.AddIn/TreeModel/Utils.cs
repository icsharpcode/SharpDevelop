// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;

using ICSharpCode.SharpDevelop;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	public static partial class Utils
	{
		public static void EnqueueWork(this Process process, Dispatcher dispatcher, Action work)
		{
			long debuggeeStateWhenEnqueued = process.DebuggeeState;
			// Always ask the scheduler to do only one piece of work at a time
			// - this might actually be completely ok as we are not waiting anywhere between thread
			dispatcher.BeginInvoke(
				DispatcherPriority.Background,
				(Action)delegate {
					// Check that the user has not stepped in the meantime - if he has, do not do anything at all
					if (process.IsPaused && debuggeeStateWhenEnqueued == process.DebuggeeState) {
						try {
							// Do the work, this may recursively enqueue more work
							work();
						} catch(System.Exception ex) {
							if (process == null || process.HasExited) {
								// Process unexpectedly exited - silently ignore
							} else {
								MessageService.ShowException(ex);
							}
						}
					}
				}
			);
		}

		public static void EnqueueForEach<T>(this Process process, Dispatcher dispatcher, IList<T> items, Action<T> work, Action<T> failAction = null)
		{
			long debuggeeStateWhenEnqueued = process.DebuggeeState;
			
			foreach (T item in items) {
				var workItem = item;
				dispatcher.BeginInvoke(
					DispatcherPriority.Normal,
					(Action)delegate {
						if (!ProcessItem(process, workItem, work, debuggeeStateWhenEnqueued) && failAction != null)
							failAction(workItem);
					}
				);
			}
		}
		
		static bool ProcessItem<T>(Process process, T item, Action<T> work, long debuggeeStateWhenEnqueued)
		{
			if (process.IsPaused && debuggeeStateWhenEnqueued == process.DebuggeeState) {
				try {
					// Do the work, this may recursively enqueue more work
					work(item);
					return true;
				} catch (System.Exception ex) {
					if (process == null || process.HasExited) {
						// Process unexpectedly exited - silently ignore
					} else {
						MessageService.ShowException(ex);
					}
					SD.Log.Error("ProcessItem cancelled", ex);
				}
			}
			return false;
		}
		
	}
	
	public class AbortedBecauseDebuggeeResumedException: System.Exception
	{
		public AbortedBecauseDebuggeeResumedException(): base()
		{
			
		}
	}
	
	public class PrintTimes: PrintTime
	{
		public PrintTimes(string text): base(text + " - end")
		{
			LoggingService.InfoFormatted("{0} - start", text);
		}
	}
	
	public class PrintTime: IDisposable
	{
		string text;
		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
		
		public PrintTime(string text)
		{
			this.text = text;
			stopwatch.Start();
		}
		
		public void Dispose()
		{
			stopwatch.Stop();
			LoggingService.InfoFormatted("{0} ({1} ms)", text, stopwatch.ElapsedMilliseconds);
		}
	}
	
	public static class ExtensionMethods
	{
		public static SharpTreeNodeAdapter ToSharpTreeNode(this TreeNode node)
		{
			return new SharpTreeNodeAdapter(node);
		}
	}
}
