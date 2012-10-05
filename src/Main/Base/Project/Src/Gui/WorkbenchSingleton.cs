// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public static class WorkbenchSingleton
	{
		const string uiIconStyle             = "IconMenuItem.IconMenuStyle";
		const string uiLanguageProperty      = "CoreProperties.UILanguage";
		const string workbenchMemento        = "WorkbenchMemento";
		const string activeContentState      = "Workbench.ActiveContent";
		
		static IWorkbench workbench;
		
		/// <summary>
		/// Gets the main form. Returns null in unit-testing mode.
		/// </summary>
		public static IWin32Window MainWin32Window {
			get {
				if (workbench != null) {
					return workbench.MainWin32Window;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the main window. Returns null in unit-testing mode.
		/// </summary>
		public static Window MainWindow {
			get {
				if (workbench != null) {
					return workbench.MainWindow;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the workbench. Returns null in unit-testing mode.
		/// </summary>
		public static IWorkbench Workbench {
			get {
				return workbench;
			}
		}
		
		[Obsolete("Use SD.StatusBar instead")]
		public static IStatusBarService StatusBar {
			get {
				return SD.StatusBar;
			}
		}
		
		public static void InitializeWorkbench(IWorkbench workbench, IWorkbenchLayout layout)
		{
			WorkbenchSingleton.workbench = workbench;
			SD.Services.AddService(typeof(IWorkbench), workbench);
			
			LanguageService.ValidateLanguage();
			
			DisplayBindingService.InitializeService();
			TaskService.Initialize();
			Project.CustomToolsService.Initialize();
			
			workbench.Initialize();
			workbench.SetMemento(PropertyService.NestedProperties(workbenchMemento));
			workbench.WorkbenchLayout = layout;
			
			var applicationStateInfoService = SD.GetService<ApplicationStateInfoService>();
			if (applicationStateInfoService != null) {
				applicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return SD.Workbench.ActiveContent; });
			}
			
			OnWorkbenchCreated();
			
			// initialize workbench-dependent services:
			Project.ProjectService.InitializeService();
			NavigationService.InitializeService();
			
			workbench.ActiveContentChanged += delegate {
				Debug.WriteLine("ActiveContentChanged to " + workbench.ActiveContent);
				LoggingService.Debug("ActiveContentChanged to " + workbench.ActiveContent);
			};
			workbench.ActiveViewContentChanged += delegate {
				Debug.WriteLine("ActiveViewContentChanged to " + workbench.ActiveViewContent);
				LoggingService.Debug("ActiveViewContentChanged to " + workbench.ActiveViewContent);
			};
			workbench.ActiveWorkbenchWindowChanged += delegate {
				Debug.WriteLine("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
				LoggingService.Debug("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
			};
		}
		
		/// <summary>
		/// Runs workbench cleanup.
		/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
		/// </summary>
		public static void OnWorkbenchUnloaded()
		{
			if (!Project.ProjectService.IsClosingCanceled()) {
				Project.ProjectService.CloseSolution();
				NavigationService.Unload();
				
				WorkbenchUnloaded(null, EventArgs.Empty);
			}
		}
		
		#region Safe Thread Caller
		public static bool InvokeRequired {
			get {
				if (workbench == null)
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
		internal static void DebugAssertMainThread()
		{
			SD.MainThread.VerifyAccess();
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// </summary>
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
		public static R SafeThreadFunction<R>(Func<R> method)
		{
			return SD.MainThread.InvokeIfRequired(method);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			return SD.MainThread.InvokeIfRequired(() => method(arg1));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall(Action method)
		{
			SD.MainThread.InvokeIfRequired(method);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1, arg2));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			SD.MainThread.InvokeIfRequired(() => method(arg1, arg2, arg3));
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall(Action method)
		{
			SD.MainThread.InvokeAsync(method).FireAndForget();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			SD.MainThread.InvokeAsync(() => method(arg1)).FireAndForget();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			SD.MainThread.InvokeAsync(() => method(arg1, arg2)).FireAndForget();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			SD.MainThread.InvokeAsync(() => method(arg1, arg2, arg3)).FireAndForget();
		}
		
		/// <summary>
		/// Calls a method on the GUI thread, but delays the call a bit.
		/// </summary>
		public static async void CallLater(TimeSpan delay, Action method)
		{
			await Task.Delay(delay).ConfigureAwait(false);
			SD.MainThread.InvokeAsync(method).FireAndForget();
		}
		#endregion
		
		static void OnWorkbenchCreated()
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
