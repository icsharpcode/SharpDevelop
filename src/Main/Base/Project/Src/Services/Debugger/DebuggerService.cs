// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public static class DebuggerService
	{
		static System.Diagnostics.Process standardProcess = null;
		static bool                       isRunning       = false;
		static IDebugger                  defaultDebugger = null;
		static IDebugger                  currentDebugger = null;
		static ArrayList                  debugger        = null;
		static ArrayList                  breakpoints     = new ArrayList();
		
		public static ArrayList Breakpoints {
			get {
				return breakpoints;
			}
		}
		
		public static IDebugger CurrentDebugger {
			get {
				if (currentDebugger != null) {
					return currentDebugger;
				}
				if (debugger != null) {
					
					IProject project = ProjectService.OpenSolution.StartupProject;
					foreach (IDebugger d in debugger) {
						if (d.CanDebug(project)) {
							currentDebugger = d;
							return d;
						}
					}
				}
				if (defaultDebugger == null) {
					defaultDebugger = new DefaultDebugger();
				}
				currentDebugger = defaultDebugger;
				return defaultDebugger;
			}
		}
		
		public static bool IsProcessRuning {
			get {
				if (standardProcess != null) {
					return isRunning;
				}
				if (currentDebugger != null) {
					return currentDebugger.IsProcessRunning;
				}
				return false;
			}
		}
		
		public static bool IsDebugging {
			get {
				if (currentDebugger == null) {
					return false;
				}
				return currentDebugger.IsDebugging;
			}
		}
		
		static DebuggerService()
		{
		}
		
		public static void ToggleBreakpointAt(string fileName, int line, int column)
		{
			try {
				for (int i = 0; i < breakpoints.Count; ++i) {
					Breakpoint bp = (Breakpoint)breakpoints[i];
					if (bp.FileName == fileName && bp.LineNumber == line) {
						breakpoints.RemoveAt(i);
						return;
					}
				}
				breakpoints.Add(new Breakpoint(fileName, line));
			} finally {
				OnBreakPointChanged(EventArgs.Empty);
			}
		}
		static MessageViewCategory debugCategory = null;
		
		static void EnsureDebugCategory()
		{
			if (debugCategory == null) {
				debugCategory = new MessageViewCategory("Debug", "${res:MainWindow.Windows.OutputWindow.DebugCategory}");
				CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
				compilerMessageView.AddCategory(debugCategory);
			}
		}
		public static void ClearDebugMessages()
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		public static void PrintDebugMessage(string msg)
		{
			try {
				EnsureDebugCategory();
				debugCategory.AppendText(msg);
			} catch (Exception) {}
		}
		
		static string oldLayoutConfiguration = "Default";
		static void HandleDebugStopped(object sender, EventArgs e)
		{
//			LayoutConfiguration.CurrentLayoutName = oldLayoutConfiguration;
			//// Alex: if stopped - kill process which might be running or stuck
			if (standardProcess != null) {
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess = null;
			}
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Stop();
			}
						
			debugger.DebugStopped -= new EventHandler(HandleDebugStopped);
			debugger.Dispose();
			
			isRunning = false;
		}
		
		#region ICSharpCode.Core.IService interface implementation
		public static void InitializeService()
		{
			AddInTreeNode treeNode = null;
			try {
				treeNode = AddInTree.GetTreeNode("/SharpDevelop/Services/DebuggerService/Debugger");
			} catch (Exception) {
			}
			if (treeNode != null) {
				debugger = treeNode.BuildChildItems(null);
			}
			
			ProjectService.SolutionLoaded += new SolutionEventHandler(ClearOnCombineEvent);
		}
		
		static void DebuggerServiceStarted(object sender, EventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
			CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
			compilerMessageView.SelectCategory("Debug");
		}
		
		static void ClearOnCombineEvent(object sender, SolutionEventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		#endregion
		
		public static void GotoSourceFile(string fileName, int lineNumber, int column)
		{
			
			FileService.JumpToFilePosition(fileName, lineNumber, column);
		}
		
		public static void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{
			if (IsProcessRuning) {
				return;
			}
			try {
				standardProcess = new System.Diagnostics.Process();
				standardProcess.StartInfo = psi;
				standardProcess.Exited += new EventHandler(StandardProcessExited);
				standardProcess.EnableRaisingEvents = true;
				standardProcess.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + psi.FileName + "\"\n");
			}
			isRunning = true;
		}
		
		public static void StartWithoutDebugging(string fileName, string workingDirectory, string arguments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.UseShellExecute  = false;
			StartWithoutDebugging(startInfo);
		}

		public static void Start(string fileName, string workingDirectory, string arguments)
		{
			if (IsProcessRuning) {
				return;
			}
			oldLayoutConfiguration = LayoutConfiguration.CurrentLayoutName;
//			LayoutConfiguration.CurrentLayoutName = "Debug";
			
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Start(fileName, workingDirectory, arguments);
				debugger.DebugStopped += new EventHandler(HandleDebugStopped);
			}
			
//			lock (breakpoints) {
//				foreach (Breakpoint breakpoint in breakpoints) {
//					if (breakpoint.Enabled) {
//						brea.AddBreakpoint(fileName, breakpoint.FileName, breakpoint.Line);
//					}
//				}
//			}
			isRunning = true;
		}
		
		public static void Break()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Break();
			}
		}
		
		public static void Continue()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Continue();
			}
		}

		public static void Step(bool stepInto)
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			if (stepInto) {
				debugger.StepInto();
			} else {
				debugger.StepOver();
			}
		}
		
		public static void StepOut()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			debugger.StepOut();
		}
		
		public static void Stop()
		{
			if (standardProcess != null) {
//				OnTextMessage(new TextMessageEventArgs(String.Format("Killing {0}{1}\n",standardProcess.ProcessName,Environment.NewLine)));
				standardProcess.Exited -= new EventHandler(StandardProcessExited);
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess.Dispose();
				standardProcess = null;
			} else {
				IDebugger debugger = CurrentDebugger;
				if (debugger != null) {
					debugger.Stop();
				}
			}
			isRunning = false;
		}
		
		static void StandardProcessExited(object sender, EventArgs e)
		{
			standardProcess.Exited -= new EventHandler(StandardProcessExited);
			standardProcess.Dispose();
			standardProcess = null;	
			isRunning       = false;
		}
		
//		protected override void OnException(ExceptionEventArgs e)
//		{
//			base.OnException(e);
//			OnTextMessage(new TextMessageEventArgs("Got Exception\n"));
//			StopDebugger();
//		}
//		
//		protected override void OnProcessExited(ProcessEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("The program '[{1}] {0}' exited with code {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Process.ID,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnProcessExited(e);
//		}
//		protected override void OnModuleLoaded(ModuleEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("'{0}' : '{1}' loaded, {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Module.Name,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnModuleLoaded(e);
//		}
		
		static void OnBreakPointChanged(EventArgs e) 
		{
			if (BreakPointChanged != null) {
				BreakPointChanged(null, e);
			}
		}
		
		public static event EventHandler BreakPointChanged;
	}
}
