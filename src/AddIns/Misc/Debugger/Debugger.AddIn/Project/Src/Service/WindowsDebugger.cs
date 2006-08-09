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
		
		bool serviceInitialized = false;
				
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
		
		public WindowsDebugger()
		{
			properties = PropertyService.Get("DebuggerProperties", new Properties());
		}

		#region IDebugger Members

		public bool IsDebugging { 
			get { 
				return serviceInitialized && (debugger.Processes.Count > 0);
			} 
		}
		
		public bool IsProcessRunning { 
			get { 
				return IsDebugging && debugger.IsRunning; 
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
			string version = debugger.GetProgramVersion(processStartInfo.FileName);
			if (version.StartsWith("v1.0")) {
				MessageBox.Show("Debugging of .NET Framework 1.0 programs is not supported");
			} else if (version == null || version.Length == 0) {
				MessageBox.Show("Can not get .NET Framework version of program. Check that the program is managed assembly.");
			} else {
				debugger.Start(processStartInfo.FileName,
				               processStartInfo.WorkingDirectory,
							   processStartInfo.Arguments);
			}
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
			if (debugger.SelectedFunction == null || debugger.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepInto}");
			} else {
				debugger.StepInto();
			}
		}
		
		public void StepOver()
		{
			if (debugger.SelectedFunction == null || debugger.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepOver.Description}");
			} else {
				debugger.StepOver();
			}
		}
		
		public void StepOut()
		{
			if (debugger.SelectedFunction == null || debugger.IsRunning) {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}", "${res:XML.MainMenu.DebugMenu.StepOut}");
			} else {
				debugger.StepOut();
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
			Variable variable = GetVariableFromName(variableName.Trim());
			
			if (variable == null) {
				return null;
			} else {
				return new DebuggerGridControl(new DynamicTreeDebuggerRow(variable));
			}
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			if (debugger != null && debugger.IsPaused && debugger.SelectedFunction != null) {
				SourcecodeSegment seg = debugger.SelectedFunction.CanSetIP(filename, line, column);
				return seg != null;
			} else {
				return false;
			}
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			if (CanSetInstructionPointer(filename, line, column)) {
				SourcecodeSegment seg = debugger.SelectedFunction.SetIP(filename, line, column);
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
			debugger.ExceptionThrown         += ExceptionThrown;
			debugger.DebuggeeStateChanged    += DebuggeeStateChanged;
			debugger.DebuggingResumed        += DebuggingResumed;

			DebuggerService.BreakPointAdded  += delegate (object sender, BreakpointBookmarkEventArgs e) {
				AddBreakpoint(e.BreakpointBookmark);
			};

			foreach (BreakpointBookmark b in DebuggerService.Breakpoints) {
				AddBreakpoint(b);
			}
			
			if (Initialize != null) {
				Initialize(this, null);  
			}

			serviceInitialized = true;
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
		
		void TraceMessage(object sender, MessageEventArgs e)
		{
			LoggingService.Debug("Debugger: " + e.Message);
		}
		
		void ProcessStarted(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 1) {
				if (DebugStarted != null) {
					DebugStarted(this, EventArgs.Empty);
				}
			}
		}

		void ProcessExited(object sender, ProcessEventArgs e)
		{
			if (debugger.Processes.Count == 0) {
				if (DebugStopped != null) {
					DebugStopped(this, e);
				}
			}
		}
		
		void DebuggingPaused(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
		}
		
		void ExceptionThrown(object sender, ExceptionEventArgs e)
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
					debugger.SelectedThread.InterceptCurrentException();
					e.Continue = true; // HACK: Start interception
					break;
			}
		}
		
		void DebuggeeStateChanged(object sender, DebuggerEventArgs e)
		{
			JumpToCurrentLine();
		}
		
		void DebuggingResumed(object sender, DebuggerEventArgs e)
		{
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
			} else {
				DebuggerService.JumpToCurrentLine(nextStatement.SourceFullFilename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
			}
		}
	}
}
