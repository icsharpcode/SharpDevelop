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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// WorkbenchSingleton only consists of obsolete members.
	/// Use SD.Workbench and SD.MainThread instead.
	/// </summary>
	public static class WorkbenchSingleton
	{
		/// <summary>
		/// Gets the main form. Returns null in unit-testing mode.
		/// </summary>
		[Obsolete("Use SD.WinForms.MainWin32Window instead")]
		public static IWin32Window MainWin32Window {
			get {
				if (Workbench != null) {
					return Workbench.MainWin32Window;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the main window. Returns null in unit-testing mode.
		/// </summary>
		[Obsolete("Use SD.Workbench.MainWindow instead")]
		public static Window MainWindow {
			get {
				if (Workbench != null) {
					return Workbench.MainWindow;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the workbench. Returns null in unit-testing mode.
		/// </summary>
		[Obsolete("Use SD.Workbench instead")]
		public static IWorkbench Workbench {
			get {
				return SD.Services.GetService<IWorkbench>();
			}
		}
		
		[Obsolete("Use SD.StatusBar instead")]
		public static IStatusBarService StatusBar {
			get {
				return SD.StatusBar;
			}
		}
		
		/// <summary>
		/// Runs workbench cleanup.
		/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
		/// </summary>
		internal static void OnWorkbenchUnloaded()
		{
			SD.ProjectService.CloseSolution(allowCancel: false);
			NavigationService.Unload();
			
			WorkbenchUnloaded(null, EventArgs.Empty);
		}
		
		#region Safe Thread Caller
		[Obsolete("Use SD.MainThread.InvokeRequired instead")]
		public static bool InvokeRequired {
			get {
				if (Workbench == null)
					return false; // unit test mode, don't crash
				else
					return SD.MainThread.InvokeRequired;
			}
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// For performance reasons, the thread check is only done in debug builds.
		/// </summary>
		[Conditional("DEBUG")]
		[Obsolete("Use SD.MainThread.VerifyAccess() instead")]
		internal static void DebugAssertMainThread()
		{
			SD.MainThread.VerifyAccess();
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// </summary>
		[Obsolete("Use SD.MainThread.VerifyAccess() instead")]
		public static void AssertMainThread()
		{
			if (InvokeRequired) {
				throw new InvalidOperationException("This operation can be called on the main thread only.");
			}
		}
		
		readonly static object[] emptyObjectArray = new object[0];
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static R SafeThreadFunction<R>(Func<R> method)
		{
			return SD.MainThread.InvokeIfRequired(method);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			return SD.MainThread.InvokeIfRequired(() => method(arg1));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static void SafeThreadCall(Action method)
		{
			SD.MainThread.InvokeIfRequired(method);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1, arg2));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeIfRequired() instead")]
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1, arg2, arg3));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeAsync().FireAndForget() instead")]
		public static void SafeThreadAsyncCall(Action method)
		{
			SD.MainThread.InvokeAsyncAndForget(method);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeAsync().FireAndForget() instead")]
		public static void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			SD.MainThread.InvokeAsyncAndForget(() => method(arg1));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeAsync().FireAndForget() instead")]
		public static void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			SD.MainThread.InvokeAsyncAndForget(() => method(arg1, arg2));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		[Obsolete("Use SD.MainThread.InvokeAsync().FireAndForget() instead")]
		public static void SafeThreadAsyncCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			SD.MainThread.InvokeAsyncAndForget(() => method(arg1, arg2, arg3));
		}
		
		/// <summary>
		/// Calls a method on the GUI thread, but delays the call a bit.
		/// </summary>
		[Obsolete("Use SD.MainThread.CallLater() instead")]
		public static void CallLater(TimeSpan delay, Action method)
		{
			SD.MainThread.CallLater(delay, method);
		}
		#endregion
		
		internal static void OnWorkbenchCreated()
		{
			WorkbenchCreated(null, EventArgs.Empty);
		}
		
		/// <summary>
		/// Is called, when the workbench is created
		/// </summary>
		public static event EventHandler WorkbenchCreated = delegate {};
		
		/// <summary>
		/// Is called, when the workbench is unloaded
		/// </summary>
		public static event EventHandler WorkbenchUnloaded = delegate {};
	}
}
