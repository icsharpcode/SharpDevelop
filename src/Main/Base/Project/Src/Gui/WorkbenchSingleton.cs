// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		
		public static Control ActiveControl {
			get {
				return null;
				/*
				ContainerControl container = WorkbenchSingleton.MainForm;
				Control ctl;
				do {
					ctl = container.ActiveControl;
					if (ctl == null)
						return container;
					container = ctl as ContainerControl;
				} while(container != null);
				return ctl;*/
			}
		}
		
		/// <remarks>
		/// This method handles the redraw all event for specific changed IDE properties
		/// </remarks>
		static void TrackPropertyChanges(object sender, PropertyChangedEventArgs e)
		{
			if (e.OldValue != e.NewValue && workbench != null) {
				switch (e.Key) {
					case "ICSharpCode.SharpDevelop.Gui.StatusBarVisible":
					case "ICSharpCode.SharpDevelop.Gui.VisualStyle":
					case "ICSharpCode.SharpDevelop.Gui.ToolBarVisible":
						workbench.RedrawAllComponents();
						break;
					case "ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer":
						workbench.UpdateRenderer();
						break;
				}
			}
		}
		
		/// <summary>
		/// Runs workbench initialization.
		/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
		/// </summary>
		public static void InitializeWorkbench()
		{
//			if (Environment.OSVersion.Platform == PlatformID.Unix)
//				InitializeWorkbench(new DefaultWorkbench(), new SimpleWorkbenchLayout());
//			else
//				InitializeWorkbench(new DefaultWorkbench(), new SdiWorkbenchLayout());
			InitializeWorkbench(new WpfWorkbench(), new AvalonDockLayout());
		}
		
		public static void InitializeWorkbench(IWorkbench workbench, IWorkbenchLayout layout)
		{
			WorkbenchSingleton.workbench = workbench;

			DisplayBindingService.InitializeService();
			LayoutConfiguration.LoadLayoutConfiguration();
			FileService.InitializeService();
			StatusBarService.Initialize();
			DomHostCallback.Register(); // must be called after StatusBarService.Initialize()
			ParserService.InitializeParserService();
			Bookmarks.BookmarkManager.Initialize();
			Project.CustomToolsService.Initialize();
			Project.BuildModifiedProjectsOnlyService.Initialize();
			
			WinFormsMessageService.DialogOwner = workbench.MainWin32Window;
			WinFormsMessageService.DialogSynchronizeInvoke = workbench.SynchronizingObject;
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(TrackPropertyChanges);
			ResourceService.LanguageChanged += delegate { workbench.RedrawAllComponents(); };
			
			workbench.Initialize();
			workbench.SetMemento(PropertyService.Get(workbenchMemento, new Properties()));
			workbench.WorkbenchLayout = layout;
			
			ApplicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return WorkbenchSingleton.Workbench.ActiveContent; });
			
			OnWorkbenchCreated();
			
			// initialize workbench-dependent services:
			Project.ProjectService.InitializeService();
			NavigationService.InitializeService();
			
			workbench.ActiveContentChanged += delegate {
				LoggingService.Debug("ActiveContentChanged to " + workbench.ActiveContent);
			};
			workbench.ActiveViewContentChanged += delegate {
				LoggingService.Debug("ActiveViewContentChanged to " + workbench.ActiveViewContent);
			};
			workbench.ActiveWorkbenchWindowChanged += delegate {
				LoggingService.Debug("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
			};
		}
		
		/// <summary>
		/// Runs workbench cleanup.
		/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
		/// </summary>
		public static void OnWorkbenchUnloaded()
		{
			Project.ProjectService.CloseSolution();
			NavigationService.Unload();
			
			ApplicationStateInfoService.UnregisterStateGetter(activeContentState);
			
			if (WorkbenchUnloaded != null) {
				WorkbenchUnloaded(null, EventArgs.Empty);
			}
			
			FileService.Unload();
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
			return (R)workbench.SynchronizingObject.Invoke(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			return (R)workbench.SynchronizingObject.Invoke(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall(Action method)
		{
			workbench.SynchronizingObject.Invoke(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			workbench.SynchronizingObject.Invoke(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			workbench.SynchronizingObject.Invoke(method, new object[] { arg1, arg2 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			workbench.SynchronizingObject.Invoke(method, new object[] { arg1, arg2, arg3 });
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
		public static void CallLater(int delayMilliseconds, Action method)
		{
			if (delayMilliseconds <= 0)
				throw new ArgumentOutOfRangeException("delayMilliseconds", delayMilliseconds, "Value must be positive");
			if (method == null)
				throw new ArgumentNullException("method");
			SafeThreadAsyncCall(
				delegate {
					Timer t = new Timer();
					t.Interval = delayMilliseconds;
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
			if (WorkbenchCreated != null) {
				WorkbenchCreated(null, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Is called, when the workbench is created
		/// </summary>
		public static event EventHandler WorkbenchCreated;
		
		/// <summary>
		/// Is called, when the workbench is unloaded
		/// </summary>
		public static event EventHandler WorkbenchUnloaded;
	}
}
