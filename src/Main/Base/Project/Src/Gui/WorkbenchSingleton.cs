// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		static WorkbenchSingleton()
		{
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(TrackPropertyChanges);
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
					case "CoreProperties.UILanguage":
					case "ICSharpCode.SharpDevelop.Gui.ToolBarVisible":
						workbench.RedrawAllComponents();
						break;
				}
			}
		}
		
		public static void InitializeWorkbench()
		{
			workbench = new DefaultWorkbench();
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
		public class STAThreadCaller
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
						Console.WriteLine("Stacktrace of source thread:");
						Console.WriteLine(callerStack);
						Console.WriteLine();
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
		/// operation, which can result in a dead-lock when the main thread waits for this
		/// thread to exit!
		/// </summary>
		public static object SafeThreadCall(object target, string methodName, params object[] arguments)
		{
			return caller.Call(target, methodName, arguments);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall(object target, string methodName, params object[] arguments)
		{
			caller.BeginCall(target, methodName, arguments);
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
