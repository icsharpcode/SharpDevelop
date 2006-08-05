// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
	public static class WorkbenchSingleton
	{
		const string uiIconStyle             = "IconMenuItem.IconMenuStyle";
		const string uiLanguageProperty      = "CoreProperties.UILanguage";
		const string workbenchMemento        = "WorkbenchMemento";
		
		static STAThreadCaller caller;
		static DefaultWorkbench workbench    = null;
		
		public static Form MainForm {
			get {
				return (Form)workbench;
			}
		}
		
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
		
		public static void InitializeWorkbench()
		{
			LayoutConfiguration.LoadLayoutConfiguration();
			StatusBarService.Initialize();
			DomHostCallback.Register(); // must be called after StatusBarService.Initialize()
			
			workbench = new DefaultWorkbench();
			MessageService.MainForm = workbench;
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(TrackPropertyChanges);
			ResourceService.LanguageChanged += delegate { workbench.RedrawAllComponents(); };
			
			caller = new STAThreadCaller(workbench);
			
			workbench.InitializeWorkspace();
			
			workbench.SetMemento(PropertyService.Get(workbenchMemento, new Properties()));
			
			workbench.WorkbenchLayout = new SdiWorkbenchLayout();
			
			OnWorkbenchCreated();
		}
		
		#region Safe Thread Caller
		/// <summary>
		/// Description of STAThreadCaller.
		/// </summary>
		private class STAThreadCaller
		{
			delegate object PerformCallDelegate(object target, string methodName, object[] arguments);
			
			Control ctl;
			PerformCallDelegate performCallDelegate;
			
			#if DEBUG
			string callerStack;
			#endif
			
			public STAThreadCaller(Control ctl)
			{
				this.ctl = ctl;
				performCallDelegate = new PerformCallDelegate(DoPerformCall);
			}
			
			public object Call(Delegate method, object[] arguments)
			{
				if (method == null) {
					throw new ArgumentNullException("method");
				}
				return ctl.Invoke(method, arguments);
			}
			
			public object Call(object target, string methodName, object[] arguments)
			{
				if (target == null) {
					throw new ArgumentNullException("target");
				}
				
				#if DEBUG
				callerStack = Environment.StackTrace;
				#endif
				
				return ctl.Invoke(performCallDelegate, new object[] {target, methodName, arguments});
			}
			
			public void BeginCall(Delegate method, object[] arguments)
			{
				if (method == null) {
					throw new ArgumentNullException("method");
				}
				ctl.BeginInvoke(method, arguments);
			}
			
			public void BeginCall(object target, string methodName, object[] arguments)
			{
				if (target == null) {
					throw new ArgumentNullException("target");
				}
				
				#if DEBUG
				callerStack = Environment.StackTrace;
				#endif
				
				ctl.BeginInvoke(performCallDelegate, new object[] {target, methodName, arguments});
			}
			
			object DoPerformCall(object target, string methodName, object[] arguments)
			{
				MethodInfo methodInfo = null;
				if (target is Type) {
					methodInfo = ((Type)target).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
				} else {
					methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
				}
				
				if (methodInfo == null) {
					throw new System.ArgumentException("method not found : " + methodName);
				} else {
					try {
						if (target is Type) {
							return methodInfo.Invoke(null, arguments);
						} else {
							return methodInfo.Invoke(target, arguments);
						}
					} catch (Exception ex) {
						if (ex is TargetInvocationException && ex.InnerException != null) {
							ex = ex.InnerException;
						}
						MessageService.ShowError(ex, "Exception got.");
						#if DEBUG
						LoggingService.Info("Stacktrace of source thread:\n" + callerStack);
						#endif
					}
				}
				return null;
			}
		}
		
		public static bool InvokeRequired {
			get {
				return ((Form)workbench).InvokeRequired;
			}
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<R>(Func<R> method)
		{
			return (R)caller.Call(method, new object[0]);
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
			caller.Call(method, new object[0]);
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
	}
}
