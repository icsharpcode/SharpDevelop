// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
			caller = new STAThreadCaller();
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
			delegate object PerformCallDelegate(/*object target, string methodName, object[] arguments*/);
			
			object target;
			string methodName;
			object[] arguments;
			Form  form = (Form)WorkbenchSingleton.Workbench;
			PerformCallDelegate performCallDelegate;
			
			public STAThreadCaller()
			{
				performCallDelegate = new PerformCallDelegate(DoPerformCall);
			}
			
			public object Call(object target, string methodName, params object[] arguments)
			{
				if (target == null) {
					throw new System.ArgumentNullException("target");
				}

				this.target     = target;
				this.methodName = methodName;
				this.arguments  = arguments;
				
				return DoPerformCall();
			}
			
			object DoPerformCall( /*object target, string methodName, object[] arguments */)
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
						
						MessageService.ShowError(ex, "Exception got. ");
					}
				}
				return null;
			}
			
			object InternalSafeThreadCall(/*object target, string methodName, params object[] arguments*/)
			{
				if (form.InvokeRequired) {
					return form.Invoke(performCallDelegate, null /*new object[] { target, methodName, arguments } */);
				} else {
					return DoPerformCall(/*target, methodName, arguments*/);
				}
			}
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe.
		/// </summary>
		public static object SafeThreadCall(object target, string methodName, params object[] arguments)
		{
			return caller.Call(target, methodName, arguments);
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
