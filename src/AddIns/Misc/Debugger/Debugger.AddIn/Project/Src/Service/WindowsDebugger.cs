// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//
//  Copyright (c) 2007, ic#code
//
//  All rights reserved.
//
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.TreeModel;
using Debugger.Core.Wrappers.CorPub;
using Debugger.Expressions;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using BM = ICSharpCode.SharpDevelop.Bookmarks;
using Process = Debugger.Process;

namespace ICSharpCode.SharpDevelop.Services
{
	public class WindowsDebugger : IDebugger
	{
		enum StopAttachedProcessDialogResult {
			Detach = 0,
			Terminate = 1,
			Cancel = 2
		}
		
		bool useRemotingForThreadInterop = false;
		bool attached;
		
		NDebugger debugger;
		
		ICorPublish corPublish;
		
		Process debuggedProcess;
		
		DynamicTreeDebuggerRow currentTooltipRow;
		Expression             currentTooltipExpression;
		
		public event EventHandler<ProcessEventArgs> ProcessSelected;
		
		public NDebugger DebuggerCore {
			get {
				return debugger;
			}
		}
		
		public Process DebuggedProcess {
			get {
				return debuggedProcess;
			}
		}
		
		public static Process CurrentProcess {
			get {
				WindowsDebugger dbgr = DebuggerService.CurrentDebugger as WindowsDebugger;
				if (dbgr != null && dbgr.DebuggedProcess != null) {
					return dbgr.DebuggedProcess;
				} else {
					return null;
				}
			}
		}
		
		protected virtual void OnProcessSelected(ProcessEventArgs e)
		{
			if (ProcessSelected != null) {
				ProcessSelected(this, e);
			}
		}
		
		public bool ServiceInitialized {
			get {
				return debugger != null;
			}
		}
		
		public WindowsDebugger()
		{
			
		}
		
		#region IDebugger Members
		
		string errorDebugging      = "${res:XML.MainMenu.DebugMenu.Error.Debugging}";
		string errorNotDebugging   = "${res:XML.MainMenu.DebugMenu.Error.NotDebugging}";
		string errorProcessRunning = "${res:XML.MainMenu.DebugMenu.Error.ProcessRunning}";
		string errorProcessPaused  = "${res:XML.MainMenu.DebugMenu.Error.ProcessPaused}";
		string errorCannotStepNoActiveFunction = "${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}";
		
		public bool IsDebugging {
			get {
				return ServiceInitialized && debuggedProcess != null;
			}
		}
		
		public bool IsAttached {
			get {
				return ServiceInitialized && attached;
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
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}");
			} else if (version.StartsWith("v1.1")) {
				MessageService.ShowMessage(StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}").Replace("1.0", "1.1"));
//			} else if (string.IsNullOrEmpty(version)) {
//				// Not a managed assembly
//				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.BadAssembly}");
			} else if (debugger.IsKernelDebuggerEnabled) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.KernelDebuggerEnabled}");
			} else {
				attached = false;
				if (DebugStarting != null)
					DebugStarting(this, EventArgs.Empty);
				
				try {
					Process process = debugger.Start(processStartInfo.FileName,
					                                 processStartInfo.WorkingDirectory,
					                                 processStartInfo.Arguments);
					SelectProcess(process);
				} catch (System.Exception e) {
					// COMException: The request is not supported. (Exception from HRESULT: 0x80070032)
					// COMException: The application has failed to start because its side-by-side configuration is incorrect. Please see the application event log for more detail. (Exception from HRESULT: 0x800736B1)
					// COMException: The requested operation requires elevation. (Exception from HRESULT: 0x800702E4)
					// COMException: The directory name is invalid. (Exception from HRESULT: 0x8007010B)
					// BadImageFormatException:  is not a valid Win32 application. (Exception from HRESULT: 0x800700C1)
					// UnauthorizedAccessException: Отказано в доступе. (Исключение из HRESULT: 0x80070005 (E_ACCESSDENIED))
					if (e is COMException || e is BadImageFormatException || e is UnauthorizedAccessException) {
						string msg = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.CannotStartProcess}");
						MessageService.ShowMessage(msg + " " + e.Message);
						
						if (DebugStopped != null)
							DebugStopped(this, EventArgs.Empty);
					} else {
						throw;
					}
				}
			}
		}

		public void ShowAttachDialog()
		{
			using (AttachToProcessForm attachForm = new AttachToProcessForm()) {
				if (attachForm.ShowDialog() == DialogResult.OK) {
					Attach(attachForm.Process);
				}
			}
		}
		
		public void Attach(System.Diagnostics.Process existingProcess)
		{
			if (IsDebugging) {
				MessageService.ShowMessage(errorDebugging);
				return;
			}
			if (!ServiceInitialized) {
				InitializeService();
			}
			
			string version = debugger.GetProgramVersion(existingProcess.MainModule.FileName);
			if (version.StartsWith("v1.0")) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}");
			} else {
				if (DebugStarting != null)
					DebugStarting(this, EventArgs.Empty);
				
				try {
					Process process = debugger.Attach(existingProcess);
					attached = true;
					SelectProcess(process);
				} catch (System.Exception e) {
					// CORDBG_E_DEBUGGER_ALREADY_ATTACHED
					if (e is COMException || e is UnauthorizedAccessException) {
						string msg = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.CannotAttachToProcess}");
						MessageService.ShowMessage(msg + " " + e.Message);
						
						if (DebugStopped != null)
							DebugStopped(this, EventArgs.Empty);
					} else {
						throw;
					}
				}
			}
		}

		public void Detach()
		{
			debugger.Detach();
		}
		
		public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			System.Diagnostics.Process.Start(processStartInfo);
		}
		
		public void Stop()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.Stop}");
				return;
			}
			if (IsAttached) {
				StopAttachedProcessDialogResult result = ShowStopAttachedProcessDialog();
				switch (result) {
					case StopAttachedProcessDialogResult.Terminate:
						debuggedProcess.Terminate();
						attached = false;
						break;
					case StopAttachedProcessDialogResult.Detach:
						Detach();
						attached = false;
						break;
				}
			} else {
				debuggedProcess.Terminate();
			}
		}
		
		// ExecutionControl:
		
		public void Break()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.Break}");
				return;
			}
			if (!IsProcessRunning) {
				MessageService.ShowMessage(errorProcessPaused, "${res:XML.MainMenu.DebugMenu.Break}");
				return;
			}
			debuggedProcess.Break();
		}
		
		public void Continue()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.Continue}");
				return;
			}
			if (IsProcessRunning) {
				MessageService.ShowMessage(errorProcessRunning, "${res:XML.MainMenu.DebugMenu.Continue}");
				return;
			}
			debuggedProcess.AsyncContinue();
		}
		
		// Stepping:
		
		public void StepInto()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepInto}");
				return;
			}
			if (debuggedProcess.SelectedStackFrame == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepInto}");
			} else {
				debuggedProcess.SelectedStackFrame.AsyncStepInto();
			}
		}
		
		public void StepOver()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepOver}");
				return;
			}
			if (debuggedProcess.SelectedStackFrame == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepOver}");
			} else {
				debuggedProcess.SelectedStackFrame.AsyncStepOver();
			}
		}
		
		public void StepOut()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepOut}");
				return;
			}
			if (debuggedProcess.SelectedStackFrame == null || debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepOut}");
			} else {
				debuggedProcess.SelectedStackFrame.AsyncStepOut();
			}
		}
		
		public event EventHandler DebugStarting;
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
		public Value GetValueFromName(string variableName)
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				return null;
			} else {
				return AstEvaluator.Evaluate(variableName, SupportedLanguage.CSharp, debuggedProcess.SelectedStackFrame);
			}
		}
		
		public bool IsManaged(int processId)
		{
			if (corPublish == null) {
				corPublish = new ICorPublish();
			}
			
			ICorPublishProcess process = corPublish.GetProcess(processId);
			if (process != null) {
				return process.IsManaged;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// Returns null if unsuccessful.
		/// </summary>
		public string GetValueAsString(string variableName)
		{
			try {
				Value val = GetValueFromName(variableName);
				if (val == null) return null;
				return val.AsString;
			} catch (GetValueException) {
				return null;
			}
		}
		
		/// <summary>
		/// Gets the tooltip control that shows the value of given variable.
		/// Return null if no tooltip is available.
		/// </summary>
		public DebuggerGridControl GetTooltipControl(string variableName)
		{
			ValueNode valueNode;
			try {
				Value val = GetValueFromName(variableName);
				if (val == null) return null;
				valueNode = new ValueNode(val);
			} catch (GetValueException) {
				return null;
			}
			
			try {
				currentTooltipRow = new DynamicTreeDebuggerRow(DebuggedProcess, valueNode);
			} catch (AbortedBecauseDebuggeeResumedException) {
				return null;
			}
			currentTooltipExpression = valueNode.Expression;
			return new DebuggerGridControl(currentTooltipRow);
		}
		
		public bool CanSetInstructionPointer(string filename, int line, int column)
		{
			if (debuggedProcess != null && debuggedProcess.IsPaused && debuggedProcess.SelectedStackFrame != null) {
				SourcecodeSegment seg = debuggedProcess.SelectedStackFrame.CanSetIP(filename, line, column);
				return seg != null;
			} else {
				return false;
			}
		}
		
		public bool SetInstructionPointer(string filename, int line, int column)
		{
			if (CanSetInstructionPointer(filename, line, column)) {
				SourcecodeSegment seg = debuggedProcess.SelectedStackFrame.SetIP(filename, line, column);
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
		
		public event EventHandler Initialize;
		
		public void InitializeService()
		{
			if (useRemotingForThreadInterop) {
				// This needs to be called before instance of NDebugger is created
				string path = RemotingConfigurationHelpper.GetLoadedAssemblyPath("Debugger.Core.dll");
				new RemotingConfigurationHelpper(path).Configure();
			}
			
			debugger = new NDebugger();
			
			debugger.Options = DebuggingOptions.Instance;
			
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
		
		bool Compare(byte[] a, byte[] b)
		{
			if (a.Length != b.Length) return false;
			for(int i = 0; i < a.Length; i++) {
				if (a[i] != b[i]) return false;
			}
			return true;
		}
		
		void AddBreakpoint(BreakpointBookmark bookmark)
		{
			Breakpoint breakpoint = debugger.AddBreakpoint(bookmark.FileName, null, bookmark.LineNumber + 1, 0, bookmark.IsEnabled);
			MethodInvoker setBookmarkColor = delegate {
				if (debugger.Processes.Count == 0) {
					bookmark.IsHealthy = true;
					bookmark.Tooltip = null;
				} else if (!breakpoint.IsSet) {
					bookmark.IsHealthy = false;
					bookmark.Tooltip = "Breakpoint was not found in any loaded modules";
				} else if (breakpoint.OriginalLocation.CheckSum == null) {
					bookmark.IsHealthy = true;
					bookmark.Tooltip = null;
				} else {
					byte[] fileMD5;
					TextEditorDisplayBindingWrapper file = FileService.GetOpenFile(bookmark.FileName) as TextEditorDisplayBindingWrapper;
					if (file != null) {
						byte[] fileContent = Encoding.UTF8.GetBytesWithPreamble(file.Text);
						fileMD5 = new MD5CryptoServiceProvider().ComputeHash(fileContent);
					} else {
						fileMD5 = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(bookmark.FileName));
					}
					if (Compare(fileMD5, breakpoint.OriginalLocation.CheckSum)) {
						bookmark.IsHealthy = true;
						bookmark.Tooltip = null;
					} else {
						bookmark.IsHealthy = false;
						bookmark.Tooltip = "Check sum or file does not match to the original";
					}
				}
			};
			
			// event handlers on bookmark and breakpoint don't need deregistration
			bookmark.IsEnabledChanged += delegate {
				breakpoint.Enabled = bookmark.IsEnabled;
			};
			breakpoint.Set += delegate { setBookmarkColor(); };
			
			setBookmarkColor();
			
			EventHandler<ProcessEventArgs> bp_debugger_ProcessStarted = (sender, e) => {
				setBookmarkColor();
				// User can change line number by inserting or deleting lines
				breakpoint.Line = bookmark.LineNumber + 1;
			};
			EventHandler<ProcessEventArgs> bp_debugger_ProcessExited = (sender, e) => {
				setBookmarkColor();
			};
			
			EventHandler<BreakpointEventArgs> bp_debugger_BreakpointHit =
				new EventHandler<BreakpointEventArgs>(
					delegate(object sender, BreakpointEventArgs e)
					{
						LoggingService.Debug(bookmark.Action + " " + bookmark.ScriptLanguage + " " + bookmark.Condition);
						
						switch (bookmark.Action) {
							case BreakpointAction.Break:
								break;
							case BreakpointAction.Condition:
								if (Evaluate(bookmark.Condition, bookmark.ScriptLanguage))
									DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAtBecause}") + "\n", bookmark.LineNumber + 1, bookmark.FileName, bookmark.Condition));
								else
									this.debuggedProcess.AsyncContinue();
								break;
							case BreakpointAction.Trace:
								DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAt}") + "\n", bookmark.LineNumber + 1, bookmark.FileName));
								break;
						}
					});
			
			BookmarkEventHandler bp_bookmarkManager_Removed = null;
			bp_bookmarkManager_Removed = (sender, e) => {
				if (bookmark == e.Bookmark) {
					debugger.RemoveBreakpoint(breakpoint);
					
					// unregister the events
					debugger.ProcessStarted -= bp_debugger_ProcessStarted;
					debugger.ProcessExited -= bp_debugger_ProcessExited;
					breakpoint.Hit -= bp_debugger_BreakpointHit;
					BookmarkManager.Removed -= bp_bookmarkManager_Removed;
				}
			};
			// register the events
			debugger.ProcessStarted += bp_debugger_ProcessStarted;
			debugger.ProcessExited += bp_debugger_ProcessExited;
			breakpoint.Hit += bp_debugger_BreakpointHit;
			BookmarkManager.Removed += bp_bookmarkManager_Removed;
		}
		
		bool Evaluate(string code, string language)
		{
			try {
				SupportedLanguage supportedLanguage = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), language.Replace("#", "Sharp"), true);
				Value val = AstEvaluator.Evaluate(code, supportedLanguage, debuggedProcess.SelectedStackFrame);
				
				if (val != null && val.Type.IsPrimitive && val.PrimitiveValue is bool)
					return (bool)val.PrimitiveValue;
				else
					return false;
			} catch (GetValueException e) {
				string errorMessage = "Error while evaluating breakpoint condition " + code + ":\n" + e.Message + "\n";
				DebuggerService.PrintDebugMessage(errorMessage);
				WorkbenchSingleton.SafeThreadAsyncCall(MessageService.ShowWarning, errorMessage);
				return true;
			}
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
		
		public void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused                  -= debuggedProcess_DebuggingPaused;
				debuggedProcess.ExceptionThrown         -= debuggedProcess_ExceptionThrown;
				debuggedProcess.Resumed                 -= debuggedProcess_DebuggingResumed;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused                  += debuggedProcess_DebuggingPaused;
				debuggedProcess.ExceptionThrown         += debuggedProcess_ExceptionThrown;
				debuggedProcess.Resumed                 += debuggedProcess_DebuggingResumed;
			}
			JumpToCurrentLine();
			OnProcessSelected(new ProcessEventArgs(process));
		}
		
		void debuggedProcess_DebuggingPaused(object sender, ProcessEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			
			using(new PrintTimes("Jump to current line")) {
				JumpToCurrentLine();
			}
			if (currentTooltipRow != null && currentTooltipRow.IsShown) {
				using(new PrintTimes("Update tooltip")) {
					try {
						Utils.DoEvents(debuggedProcess);
						AbstractNode updatedNode = ValueNode.Create(currentTooltipExpression);
						currentTooltipRow.SetContentRecursive(updatedNode);
					} catch (AbortedBecauseDebuggeeResumedException) {
					}
				}
			}
		}
		
		void debuggedProcess_DebuggingResumed(object sender, ProcessEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			DebuggerService.RemoveCurrentLineMarker();
		}
		
		void debuggedProcess_ExceptionThrown(object sender, ExceptionEventArgs e)
		{
			if (!e.IsUnhandled) {
				// Ignore the exception
				e.Process.AsyncContinue();
				return;
			}
			
			JumpToCurrentLine();
			
			StringBuilder stacktraceBuilder = new StringBuilder();
			
			// Need to intercept now so that we can evaluate properties
			if (e.Process.SelectedThread.InterceptCurrentException()) {
				stacktraceBuilder.AppendLine(e.Exception.ToString());
				string stackTrace;
				try {
					stackTrace = e.Exception.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.EndOfInnerException}"));
				} catch (GetValueException) {
					stackTrace = e.Process.SelectedThread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}"));
				}
				stacktraceBuilder.Append(stackTrace);
			} else {
				// For example, happens on stack overflow
				stacktraceBuilder.AppendLine(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptException}"));
				stacktraceBuilder.AppendLine(e.Exception.ToString());
				stacktraceBuilder.Append(e.Process.SelectedThread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}")));
			}
			
			string title = e.IsUnhandled ? StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Unhandled}") : StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Handled}");
			string message = string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Message}"), e.Exception.Type, e.Exception.Message);
			Bitmap icon = WinFormsResourceService.GetBitmap(e.IsUnhandled ? "Icons.32x32.Error" : "Icons.32x32.Warning");
			
			DebuggeeExceptionForm.Show(debuggedProcess, title, message, stacktraceBuilder.ToString(), icon);
		}
		
		public void JumpToCurrentLine()
		{
			WorkbenchSingleton.MainForm.Activate();
			DebuggerService.RemoveCurrentLineMarker();
			if (debuggedProcess != null) {
				SourcecodeSegment nextStatement = debuggedProcess.NextStatement;
				if (nextStatement != null) {
					string fileName = GetFileName(nextStatement);
					DebuggerService.JumpToCurrentLine(fileName, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
				}
			}
		}
		
		string GetFileName(SourcecodeSegment segment)
		{
			string fileName = debuggedProcess.NextStatement.Filename;
			if (!Path.IsPathRooted(fileName)) {
				fileName = Path.Combine(debuggedProcess.WorkingDirectory, fileName);
			}
			return fileName;
		}
		
		StopAttachedProcessDialogResult ShowStopAttachedProcessDialog()
		{
			string caption = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Stop}");
			string message = StringParser.Parse("${res:MainWindow.Windows.Debug.StopProcessDialog.Message}");
			string[] buttonLabels = new string[] { StringParser.Parse("${res:XML.MainMenu.DebugMenu.Detach}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Terminate}"), StringParser.Parse("${res:Global.CancelButtonText}") };
			return (StopAttachedProcessDialogResult)MessageService.ShowCustomDialog(caption, message, (int)StopAttachedProcessDialogResult.Detach, (int)StopAttachedProcessDialogResult.Cancel, buttonLabels);
		}
	}
}
