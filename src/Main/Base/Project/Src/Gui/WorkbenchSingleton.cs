// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
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
		
		static STAThreadCaller caller;
		static IWorkbench workbench;
		
		/// <summary>
		/// Gets the main form. Returns null in unit-testing mode.
		/// </summary>
		public static Form MainForm {
			get {
				if (workbench != null) {
					return workbench.MainForm;
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
				ContainerControl container = WorkbenchSingleton.MainForm;
				Control ctl;
				do {
					ctl = container.ActiveControl;
					if (ctl == null)
						return container;
					container = ctl as ContainerControl;
				} while(container != null);
				return ctl;
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
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				InitializeWorkbench(new DefaultWorkbench(), new SimpleWorkbenchLayout());
			else
				InitializeWorkbench(new DefaultWorkbench(), new SdiWorkbenchLayout());
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
			
			workbench.Initialize();
			workbench.SetMemento(PropertyService.Get(workbenchMemento, new Properties()));
			
			caller = new STAThreadCaller(workbench.MainForm);
			WinFormsMessageService.DialogOwner = workbench.MainForm;
			WinFormsMessageService.DialogSynchronizeInvoke = workbench.MainForm;
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(TrackPropertyChanges);
			ResourceService.LanguageChanged += delegate { workbench.RedrawAllComponents(); };
			
			ApplicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return WorkbenchSingleton.Workbench.ActiveContent; });
			
			// attach workbench layout -> load pads
			workbench.WorkbenchLayout = layout;
			
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
		/// <summary>
		/// Helper class for invoking methods on the main thread.
		/// </summary>
		private sealed class STAThreadCaller
		{
			Control ctl;
			
			public STAThreadCaller(Control ctl)
			{
				if (ctl == null)
					throw new ArgumentNullException("ctl");
				this.ctl = ctl;
				ctl.CreateControl(); // ensure the control is created so Invoke can work
				// CreateControl() doesn't always force handle creation - fetch the handle once
				// to ensure it really gets created.
				IntPtr handle = ctl.Handle;
			}
			
			public object Call(Delegate method, object[] arguments)
			{
				if (method == null) {
					throw new ArgumentNullException("method");
				}
				return ctl.Invoke(method, arguments);
			}
			
			public void BeginCall(Delegate method, object[] arguments)
			{
				if (method == null) {
					throw new ArgumentNullException("method");
				}
				try {
					ctl.BeginInvoke(method, arguments);
				} catch (InvalidOperationException ex) {
					LoggingService.Warn("Error in SafeThreadAsyncCall", ex);
				}
			}
		}
		
		public static bool InvokeRequired {
			get {
				if (workbench == null)
					return false; // unit test mode, don't crash
				else
					return workbench.MainForm.InvokeRequired;
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
			return (R)caller.Call(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			return (R)caller.Call(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall(Action method)
		{
			caller.Call(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			caller.Call(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			caller.Call(method, new object[] { arg1, arg2 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			caller.Call(method, new object[] { arg1, arg2, arg3 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall(Action method)
		{
			caller.BeginCall(method, new object[0]);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			caller.BeginCall(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			caller.BeginCall(method, new object[] { arg1, arg2 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			caller.BeginCall(method, new object[] { arg1, arg2, arg3 });
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
