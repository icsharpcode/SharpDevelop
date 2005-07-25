// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DebuggerLibrary;

using ICSharpCode.Core;
using System.CodeDom.Compiler;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
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

		bool isDebuggingCache = false;
		bool isProcessRunningCache = false;
		bool serviceInitialized = false;

		List<DebuggerLibrary.Exception> exceptionHistory = new List<DebuggerLibrary.Exception>();

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

		public bool ServiceInitialized {
			get {
				return serviceInitialized;
			}
		}

		public IList<DebuggerLibrary.Exception> ExceptionHistory {
			get {
				return exceptionHistory.AsReadOnly();
			}
		}
		
		public WindowsDebugger()
		{

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

		public bool SupportsStart {
			get {
				return true;
			}
		}

		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			System.Diagnostics.Process process;
			process = new System.Diagnostics.Process();
			process.StartInfo = processStartInfo;
			process.Start();
		}

		public bool SupportsStartWithoutDebugging {
			get {
				return true;
			}
		}

		public void Stop()
		{
			debugger.Terminate();
		}

		public bool SupportsStop {
			get {
				return true;
			}
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

		public bool SupportsExecutionControl {
			get {
				return true;
			}
		}

		// Stepping:

		public void StepInto()
		{
			debugger.StepInto();
		}
		
		public void StepOver()
		{
			debugger.StepOver();
		}
		
		public void StepOut()
		{
			debugger.StepOut();
		}

		public bool SupportsStepping {
			get {
				return true;
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
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		public string GetValueAsString(string variableName)
		{
			if (debugger == null || !debugger.IsCurrentProcessSafeForInspection) return null;
			VariableCollection collection = debugger.LocalVariables;
			if (collection == null)
				return null;
			foreach (Variable v in collection) {
				if (v.Name == variableName) {
					object val = v.Value;
					if (val == null)
						return "<null>";
					else if (val is string)
						return "\"" + val.ToString() + "\"";
					else
						return val.ToString();
				}
			}
			return null;
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

			debugger.LogMessage              += new EventHandler<MessageEventArgs>(LogMessage);
			debugger.DebuggerTraceMessage    += new EventHandler<MessageEventArgs>(TraceMessage);
			debugger.ProcessStarted          += new EventHandler<ProcessEventArgs>(ProcessStarted);
			debugger.ProcessExited           += new EventHandler<ProcessEventArgs>(ProcessExited);
			debugger.DebuggingPaused         += new EventHandler<DebuggingPausedEventArgs>(DebuggingPaused);
			debugger.DebuggingResumed        += new EventHandler<DebuggerEventArgs>(DebuggingResumed);

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
			
			DebuggerService.SetIPRequest += delegate (object sender, DebuggerService.SetIPArgs args) {
				SourcecodeSegment seg = debugger.CurrentThread.CurrentFunction.SetIP(args.filename, args.line + 1, args.column);
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
			DebuggerService.PrintDebugMessage(">" + e.Message);
		}
		
		void TraceMessage(object sender, MessageEventArgs e)
		{
			DebuggerService.PrintDebugMessage(e.Message + "\n");
		}
		
		void ProcessStarted(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 1) {
				OnDebugStarted(EventArgs.Empty);
				isDebuggingCache = true;
			}
		}

		void ProcessExited(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 0) {
				exceptionHistory.Clear();
				OnDebugStopped(EventArgs.Empty);
				isDebuggingCache = false;
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
						return;
				}
			} else {
				JumpToCurrentLine();
			}
		}
		
		void DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			isProcessRunningCache = true;
			DebuggerService.RemoveCurrentLineMarker();
		}

		public void JumpToCurrentLine()
		{
			SourcecodeSegment nextStatement = debugger.NextStatement;
			if (nextStatement == null) {
				return;
			}
			DebuggerService.JumpToCurrentLine(nextStatement.SourceFullFilename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
		}
	}
}
