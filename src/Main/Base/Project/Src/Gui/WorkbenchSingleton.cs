// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

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
		
		public static IStatusBarService StatusBar {
			get {
				return workbench != null ? workbench.StatusBar : null;
			}
		}
		
		public static void InitializeWorkbench(IWorkbench workbench, IWorkbenchLayout layout)
		{
			WorkbenchSingleton.workbench = workbench;
			
			LanguageService.ValidateLanguage();
			
			DisplayBindingService.InitializeService();
			LayoutConfiguration.LoadLayoutConfiguration();
			FileService.InitializeService();
			DomHostCallback.Register(); // must be called after StatusBarService.Initialize()
			ParserService.InitializeParserService();
			TaskService.Initialize();
			Bookmarks.BookmarkManager.Initialize();
			Project.CustomToolsService.Initialize();
			Project.BuildModifiedProjectsOnlyService.Initialize();
			
			var messageService = Core.Services.ServiceManager.Instance.MessageService as IDialogMessageService;
			if (messageService != null) {
				messageService.DialogOwner = workbench.MainWin32Window;
				Debug.Assert(messageService.DialogOwner != null);
				messageService.DialogSynchronizeInvoke = workbench.SynchronizingObject;
			}
			
			workbench.Initialize();
			workbench.SetMemento(PropertyService.Get(workbenchMemento, new Properties()));
			workbench.WorkbenchLayout = layout;
			
			ApplicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return WorkbenchSingleton.Workbench.ActiveContent; });
			
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
				
				ApplicationStateInfoService.UnregisterStateGetter(activeContentState);
				
				WorkbenchUnloaded(null, EventArgs.Empty);
				
				FileService.Unload();
			}
		}
		
		#region Safe Thread Caller
		public static bool InvokeRequired {
			get {
				if (workbench == null)
					return false; // unit test mode, don't crash
				else
					return workbench.SynchronizingObject.InvokeRequired;
			}
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// For performance reasons, the thread check is only done in debug builds.
		/// </summary>
		[Conditional("DEBUG")]
		internal static void DebugAssertMainThread()
		{
			AssertMainThread();
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
			// InvokeRequired test is necessary so that we don't run other actions in the message queue
			// when we're already running on the main thread (unexpected reentrancy)
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				return (R)workbench.SynchronizingObject.Invoke(method, emptyObjectArray);
			else
				return method();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				return (R)si.Invoke(method, new object[] { arg1 });
			else
				return method(arg1);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall(Action method)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, emptyObjectArray);
			else
				method();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1 });
			else
				method(arg1);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1, arg2 });
			else
				method(arg1, arg2);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1, arg2, arg3 });
			else
				method(arg1, arg2, arg3);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall(Action method)
		{
			workbench.SynchronizingObject.BeginInvoke(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1, arg2 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1, arg2, arg3 });
		}
		
		/// <summary>
		/// Calls a method on the GUI thread, but delays the call a bit.
		/// </summary>
		public static void CallLater(TimeSpan delay, Action method)
		{
			int delayMilliseconds = (int)delay.TotalMilliseconds;
			if (delayMilliseconds < 0)
				throw new ArgumentOutOfRangeException("delay", delay, "Value must be positive");
			if (method == null)
				throw new ArgumentNullException("method");
			SafeThreadAsyncCall(
				delegate {
					Timer t = new Timer();
					t.Interval = Math.Max(1, delayMilliseconds);
					t.Tick += delegate {
						t.Stop();
						t.Dispose();
						method();
					};
					t.Start();
				});
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
