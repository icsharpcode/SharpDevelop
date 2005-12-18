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

namespace ICSharpCode.SharpDevelop.Services
{	
	public class WindowsDebugger:IDebugger
	{
		bool useRemotingForThreadInterop = false;
		
		NDebugger debugger;
		
		Properties properties;
		
		bool isDebuggingCache = false;
		bool isProcessRunningCache = false;
		bool serviceInitialized = false;

		List<Debugger.Exception> exceptionHistory = new List<Debugger.Exception>();

		public event EventHandler ExceptionHistoryModified;

		protected virtual void OnExceptionHistoryModified()
		{
			if (ExceptionHistoryModified != null) {
				ExceptionHistoryModified(this, EventArgs.Empty);
			}
		}
		
		public NDebugger DebuggerCore {
			get {
				return debugger;
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		public bool ServiceInitialized {
			get {
				return serviceInitialized;
			}
		}

		public IList<Debugger.Exception> ExceptionHistory {
			get {
				return exceptionHistory.AsReadOnly();
			}
		}
		
		public WindowsDebugger()
		{
			properties = PropertyService.Get("DebuggerProperties", new Properties());
		}

		#region IDebugger Members

		public bool IsDebugging { 
			get { 
				return isDebuggingCache; 
			} 
		}
		
		public bool IsProcessRunning { 
			get { 
				return isProcessRunningCache; 
			} 
		}
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public void Start(ProcessStartInfo processStartInfo)
		{
			if (!serviceInitialized) {
				InitializeService();
			}
			debugger.Start(processStartInfo.FileName,
			               processStartInfo.WorkingDirectory,
						   processStartInfo.Arguments);
		}

		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			System.Diagnostics.Process.Start(processStartInfo);
		}

		public void Stop()
		{
			debugger.Terminate();
		}
		
		// ExecutionControl:
		
		public void Break()
		{
			debugger.Break();
		}
		
		public void Continue()
		{
			debugger.Continue();
		}

		// Stepping:

		public void StepInto()
		{
			if (debugger.CurrentFunction == null) {
				MessageBox.Show("You can not step because there is no function selected to be stepped","Step into");
			} else {
				debugger.StepInto();
			}
		}
		
		public void StepOver()
		{
			if (debugger.CurrentFunction == null) {
				MessageBox.Show("You can not step because there is no function selected to be stepped","Step over");
			} else {
				debugger.StepOver();
			}
		}
		
		public void StepOut()
		{
			if (debugger.CurrentFunction == null) {
				MessageBox.Show("You can not step because there is no function selected to be stepped","Step out");
			} else {
				debugger.StepOut();
			}
		}

		public event EventHandler DebugStarted;

		protected virtual void OnDebugStarted(EventArgs e) 
		{
			if (DebugStarted != null) {
				DebugStarted(this, e);
			}
		}


		public event EventHandler IsProcessRunningChanged;
		
		protected virtual void OnIsProcessRunningChanged(EventArgs e)
		{
			if (IsProcessRunningChanged != null) {
				IsProcessRunningChanged(this, e);
			}
		}


		public event EventHandler DebugStopped;

		protected virtual void OnDebugStopped(EventArgs e) 
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		/// <summary>
		/// Gets variable of given name.
		/// Returns null if unsuccessful.
		/// </summary>
		public Variable GetVariableFromName(string variableName)
		{
			if (debugger == null || debugger.IsRunning) return null;
			
			VariableCollection collection = debugger.LocalVariables;
			
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
			Variable variable = GetVariableFromName(variableName);
			
			if (variable == null) {
				return null;
			} else {
				return new DebuggerGridControl(new DynamicTreeDebuggerRow(variable));
			}
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			if (debugger != null && debugger.IsPaused && debugger.CurrentFunction != null) {
				SourcecodeSegment seg = debugger.CurrentFunction.CanSetIP(filename, line, column);
				return seg != null;
			} else {
				return false;
			}
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			if (CanSetInstructionPointer(filename, line, column)) {
				SourcecodeSegment seg = debugger.CurrentFunction.SetIP(filename, line, column);
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

			debugger.LogMessage              += LogMessage;
			debugger.DebuggerTraceMessage    += TraceMessage;
			debugger.ProcessStarted          += ProcessStarted;
			debugger.ProcessExited           += ProcessExited;
			debugger.DebuggingPaused         += DebuggingPaused;
			debugger.DebuggeeStateChanged    += DebuggeeStateChanged;
			debugger.DebuggingResumed        += DebuggingResumed;

			debugger.BreakpointStateChanged  += delegate (object sender, BreakpointEventArgs e) {
				RestoreSharpdevelopBreakpoint(e.Breakpoint);
			};
			DebuggerService.BreakPointAdded   += delegate (object sender, BreakpointBookmarkEventArgs e) {
				AddBreakpoint(e.BreakpointBookmark);
			};
			DebuggerService.BreakPointRemoved += delegate (object sender, BreakpointBookmarkEventArgs e) {
				RemoveBreakpoint(e.BreakpointBookmark);
			};
			DebuggerService.BreakPointChanged += delegate (object sender, BreakpointBookmarkEventArgs e) {
				RemoveBreakpoint(e.BreakpointBookmark);
				AddBreakpoint(e.BreakpointBookmark);
			};

			RestoreNDebuggerBreakpoints();

			isDebuggingCache = false;
			isProcessRunningCache = true;
			
			if (Initialize != null) {
				Initialize(this, null);  
			}

			serviceInitialized = true;
		}

		void AddBreakpoint(BreakpointBookmark breakpointBookmark)
		{
			SourcecodeSegment seg = new SourcecodeSegment(breakpointBookmark.FileName, breakpointBookmark.LineNumber + 1); 
			Breakpoint b = debugger.AddBreakpoint(seg, breakpointBookmark.IsEnabled);
			breakpointBookmark.Tag = b;
		}

		void RemoveBreakpoint(BreakpointBookmark breakpointBookmark)
		{
			Breakpoint b = breakpointBookmark.Tag as Breakpoint;
			if (b != null) {
				debugger.RemoveBreakpoint(b);
			}
		}

		void RestoreNDebuggerBreakpoints()
		{
			debugger.ClearBreakpoints();
			foreach (BreakpointBookmark b in DebuggerService.Breakpoints) {
				AddBreakpoint(b);
			}
		}

		void RestoreSharpdevelopBreakpoint(Breakpoint breakpoint)
		{
			foreach (BreakpointBookmark sdBreakpoint in DebuggerService.Breakpoints) {
				if (sdBreakpoint.Tag == breakpoint) {
					sdBreakpoint.IsEnabled  = breakpoint.Enabled;
					sdBreakpoint.FileName   = breakpoint.SourcecodeSegment.SourceFullFilename;
					sdBreakpoint.LineNumber = breakpoint.SourcecodeSegment.StartLine - 1;
				}
			}
		}

		void LogMessage(object sender, MessageEventArgs e)
		{
			DebuggerService.PrintDebugMessage(e.Message);
		}
		
		void TraceMessage(object sender, MessageEventArgs e)
		{
			LoggingService.Debug("Debugger: " + e.Message);
		}
		
		void ProcessStarted(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 1) {
				OnDebugStarted(EventArgs.Empty);
				isDebuggingCache = true;
				isProcessRunningCache = true;
			}
		}

		void ProcessExited(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 0) {
				exceptionHistory.Clear();
				OnDebugStopped(EventArgs.Empty);
				isDebuggingCache = false;
				isProcessRunningCache = false;
			}
		}
		
		void DebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			isProcessRunningCache = false;
			OnIsProcessRunningChanged(EventArgs.Empty);
			
			if (e.Reason == PausedReason.Exception) {
				exceptionHistory.Add(debugger.CurrentThread.CurrentException);
				OnExceptionHistoryModified();
				if (debugger.CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED) {
					// Ignore the exception
					e.ResumeDebuggingAfterEvent();
					return;
				}
				
				JumpToCurrentLine();
				
				switch (ExceptionForm.Show(debugger.CurrentThread.CurrentException)) {
					case ExceptionForm.Result.Break: 
						break;
					case ExceptionForm.Result.Continue:
						e.ResumeDebuggingAfterEvent();
						return;
					case ExceptionForm.Result.Ignore:
						debugger.CurrentThread.InterceptCurrentException();
						break;
				}
			}
		}
		
		void DebuggeeStateChanged(object sender, DebuggerEventArgs e)
		{
			JumpToCurrentLine();
		}
		
		void DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			isProcessRunningCache = true;
			if (!debugger.Evaluating) {
				DebuggerService.RemoveCurrentLineMarker();
			}
		}

		public void JumpToCurrentLine()
		{
			WorkbenchSingleton.MainForm.Activate();
			SourcecodeSegment nextStatement = debugger.NextStatement;
			if (nextStatement == null) {
				DebuggerService.RemoveCurrentLineMarker();
				return;
			}
			DebuggerService.JumpToCurrentLine(nextStatement.SourceFullFilename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
		}
	}
}
