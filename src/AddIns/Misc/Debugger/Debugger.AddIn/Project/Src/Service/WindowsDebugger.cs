// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Debugger;

using ICSharpCode.Core;
using System.CodeDom.Compiler;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.TreeGrid;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;
using System.Runtime.Remoting;
using System.Reflection;
using System.Security.Policy;
using System.Diagnostics;
using BM = ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Services
{	
	public class WindowsDebugger:IDebugger
	{
		bool useRemotingForThreadInterop = false;
		
		NDebugger debugger;
		
		Properties properties;
		
		Debugger.Process debuggedProcess;
		
		public event EventHandler<ProcessEventArgs> ProcessSelected;
		
		public NDebugger DebuggerCore {
			get {
				return debugger;
			}
		}
		
		public Debugger.Process DebuggedProcess {
			get {
				return debuggedProcess;
			}
		}
		
		protected virtual void OnProcessSelected(ProcessEventArgs e)
		{
			if (ProcessSelected != null) {
				ProcessSelected(this, e);
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		public bool ServiceInitialized {
			get {
				return debugger != null;
			}
		}
		
		public WindowsDebugger()
		{
			properties = PropertyService.Get("DebuggerProperties", new Properties());
		}
		
		#region IDebugger Members
		
		string errorDebugging      = "Can not preform action because some process is debugged.";
		string errorNotDebugging   = "Can not preform action because no process is debugged.";
		string errorProcessRunning = "Can not preform action because process is running.";
		string errorProcessPaused  = "Can not preform action because process is paused.";
		
		public bool IsDebugging { 
			get { 
				return ServiceInitialized && debuggedProcess != null;
			} 
		}
		
		public bool IsProcessRunning { 
			get { 
				return IsDebugging && debuggedProcess.IsRunning;
			} 
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			if (IsDebugging) {
				MessageService.ShowMessage(errorDebugging);
				return;
			}
			if (!ServiceInitialized) {
				InitializeService();
			}
			string version = debugger.GetProgramVersion(processStartInfo.FileName);
			if (version.StartsWith("v1.0")) {
				MessageBox.Show("Debugging of .NET Framework 1.0 programs is not supported");
			} else if (version == null || version.Length == 0) {
				MessageBox.Show("Can not get .NET Framework version of program. Check that the program is managed assembly.");
			} else {
				Debugger.Process process = debugger.Start(processStartInfo.FileName,
				                                          processStartInfo.WorkingDirectory,
				                                          processStartInfo.Arguments);
				SelectProcess(process);
			}
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			System.Diagnostics.Process.Start(processStartInfo);
		}
		
		public void Stop()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			debuggedProcess.Terminate();
		}
		
		// ExecutionControl:
		
		public void Break()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			if (!IsProcessRunning) {
				MessageService.ShowMessage(errorProcessPaused);
				return;
			}
			debuggedProcess.Break();
		}
		
		public void Continue()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			if (IsProcessRunning) {
				MessageService.ShowMessage(errorProcessRunning);
				return;
			}
			debuggedProcess.Continue();
		}
		
		// Stepping:
		
		public void StepInto()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			if (debuggedProcess.SelectedFunction == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepInto}");
			} else {
				debuggedProcess.StepInto();
			}
		}
		
		public void StepOver()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			if (debuggedProcess.SelectedFunction == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepOver.Description}");
			} else {
				debuggedProcess.StepOver();
			}
		}
		
		public void StepOut()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging);
				return;
			}
			if (debuggedProcess.SelectedFunction == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepOut}");
			} else {
				debuggedProcess.StepOut();
			}
		}
		
		public event EventHandler DebugStarted;
		public event EventHandler DebugStopped;
		public event EventHandler IsProcessRunningChanged;
		
		protected virtual void OnIsProcessRunningChanged(EventArgs e)
		{
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(this, e);
			}
		}
		
		/// <summary>
		/// Gets variable of given name.
		/// Returns null if unsuccessful.
		/// </summary>
		public Variable GetVariableFromName(string variableName)
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning) return null;
			
			VariableCollection collection = debuggedProcess.LocalVariables;
			
			if (collection == null) return null;
			
			try {
				return collection[variableName];
			} catch (DebuggerException) {
				return null;
			}
		}
		
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// Returns null if unsuccessful.
		/// </summary>
		public string GetValueAsString(string variableName)
		{
			Variable variable = GetVariableFromName(variableName);
			
			if (variable == null) {
				return null;
			} else {
				return variable.Value.AsString;
			}
		}
		
		/// <summary>
		/// Gets the tooltip control that shows the value of given variable.
		/// Return null if no tooltip is available.
		/// </summary>
		public DebuggerGridControl GetTooltipControl(string variableName)
		{
			Variable variable = GetVariableFromName(variableName.Trim());
			
			if (variable == null) {
				return null;
			} else {
				return new DebuggerGridControl(new DynamicTreeDebuggerRow(variable));
			}
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			if (debuggedProcess != null && debuggedProcess.IsPaused && debuggedProcess.SelectedFunction != null) {
				SourcecodeSegment seg = debuggedProcess.SelectedFunction.CanSetIP(filename, line, column);
				return seg != null;
			} else {
				return false;
			}
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			if (CanSetInstructionPointer(filename, line, column)) {
				SourcecodeSegment seg = debuggedProcess.SelectedFunction.SetIP(filename, line, column);
				return seg != null;
			} else {
				return false;
			}
		}
		
		public void Dispose() 
		{
			Stop();
		}
		
		#endregion
		
		public event System.EventHandler Initialize;
		
		public void InitializeService()
		{
			if (useRemotingForThreadInterop) {
				// This needs to be called before instance of NDebugger is created
				string path = RemotingConfigurationHelpper.GetLoadedAssemblyPath("Debugger.Core.dll");
				new RemotingConfigurationHelpper(path).Configure();
			}
			
			debugger = new NDebugger();
			
			debugger.DebuggerTraceMessage    += debugger_TraceMessage;
			debugger.ProcessStarted          += debugger_ProcessStarted;
			debugger.ProcessExited           += debugger_ProcessExited;
			
			DebuggerService.BreakPointAdded  += delegate (object sender, BreakpointBookmarkEventArgs e) {
				AddBreakpoint(e.BreakpointBookmark);
			};
			
			foreach (BreakpointBookmark b in DebuggerService.Breakpoints) {
				AddBreakpoint(b);
			}
			
			if (Initialize != null) {
				Initialize(this, null);  
			}
		}
		
		void AddBreakpoint(BreakpointBookmark bookmark)
		{
			SourcecodeSegment seg = new SourcecodeSegment(bookmark.FileName, bookmark.LineNumber + 1); 
			Breakpoint breakpoint = debugger.AddBreakpoint(seg, bookmark.IsEnabled);
			MethodInvoker setBookmarkColor = delegate {
				bookmark.WillBeHit  = breakpoint.HadBeenSet || debugger.Processes.Count == 0;
			};
			breakpoint.Changed += delegate { setBookmarkColor(); };
			debugger.ProcessStarted += delegate {
				setBookmarkColor();
				// User can change line number by inserting or deleting lines
				breakpoint.SourcecodeSegment.StartLine = bookmark.LineNumber + 1;
			};
			debugger.ProcessExited  += delegate { setBookmarkColor(); };
			setBookmarkColor();
			
			BM.BookmarkManager.Removed += delegate (object sender, BM.BookmarkEventArgs e) {
				if (bookmark == e.Bookmark) {
					debugger.RemoveBreakpoint(breakpoint);
				}
			};
			bookmark.IsEnabledChanged += delegate {
				breakpoint.Enabled = bookmark.IsEnabled;
			};
		}
		
		void LogMessage(object sender, MessageEventArgs e)
		{
			DebuggerService.PrintDebugMessage(e.Message);
		}
		
		void debugger_TraceMessage(object sender, MessageEventArgs e)
		{
			LoggingService.Debug("Debugger: " + e.Message);
		}
		
		void debugger_ProcessStarted(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 1) {
				if (DebugStarted != null) {
					DebugStarted(this, EventArgs.Empty);
				}
			}
			e.Process.LogMessage += LogMessage;
		}
		
		void debugger_ProcessExited(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 0) {
				if (DebugStopped != null) {
					DebugStopped(this, e);
				}
				SelectProcess(null);
			} else {
				SelectProcess(debugger.Processes[0]);
			}
		}
		
		public void SelectProcess(Debugger.Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.DebuggingPaused         -= debuggedProcess_DebuggingPaused;
				debuggedProcess.ExceptionThrown         -= debuggedProcess_ExceptionThrown;
				debuggedProcess.DebuggeeStateChanged    -= debuggedProcess_DebuggeeStateChanged;
				debuggedProcess.DebuggingResumed        -= debuggedProcess_DebuggingResumed;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.DebuggingPaused         += debuggedProcess_DebuggingPaused;
				debuggedProcess.ExceptionThrown         += debuggedProcess_ExceptionThrown;
				debuggedProcess.DebuggeeStateChanged    += debuggedProcess_DebuggeeStateChanged;
				debuggedProcess.DebuggingResumed        += debuggedProcess_DebuggingResumed;
			}
			JumpToCurrentLine();
			OnProcessSelected(new ProcessEventArgs(process));
		}
		
		void debuggedProcess_DebuggingPaused(object sender, ProcessEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
		}
		
		void debuggedProcess_ExceptionThrown(object sender, ExceptionEventArgs e)
		{
			if (e.Exception.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED) {
				// Ignore the exception
				e.Continue = true;
				return;
			}
			
			JumpToCurrentLine();
			
			switch (ExceptionForm.Show(e.Exception)) {
				case ExceptionForm.Result.Break: 
					break;
				case ExceptionForm.Result.Continue:
					e.Continue = true;
					return;
				case ExceptionForm.Result.Ignore:
					e.Process.SelectedThread.InterceptCurrentException();
					e.Continue = true; // HACK: Start interception
					break;
			}
		}
		
		void debuggedProcess_DebuggeeStateChanged(object sender, ProcessEventArgs e)
		{
			JumpToCurrentLine();
		}
		
		void debuggedProcess_DebuggingResumed(object sender, ProcessEventArgs e)
		{
			if (!e.Process.Evaluating) {
				DebuggerService.RemoveCurrentLineMarker();
			}
		}
		
		public void JumpToCurrentLine()
		{
			WorkbenchSingleton.MainForm.Activate();
			DebuggerService.RemoveCurrentLineMarker();
			if (debuggedProcess != null) {
				SourcecodeSegment nextStatement = debuggedProcess.NextStatement;
				if (nextStatement != null) {
					DebuggerService.JumpToCurrentLine(nextStatement.SourceFullFilename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
				}
			}
		}
	}
}
