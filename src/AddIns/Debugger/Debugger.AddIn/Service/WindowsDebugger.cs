// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Workbench;
using Debugger.AddIn;
using Debugger.AddIn.Breakpoints;
using Debugger.AddIn.Tooltips;
using Debugger.AddIn.TreeModel;
using Debugger.Interop.CorPublish;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using Process = Debugger.Process;
using StackFrame = Debugger.StackFrame;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Services
{
	public class WindowsDebugger : BaseDebuggerService
	{
		public static WindowsDebugger Instance { get; set; }
		
		public static NDebugger  CurrentDebugger { get; private set; }
		public static Process    CurrentProcess { get; private set; }
		public static Thread     CurrentThread { get; set; }
		public static StackFrame CurrentStackFrame { get; set; }
		
		public static PdbSymbolSource PdbSymbolSource = new PdbSymbolSource();
		
		public static Action RefreshingPads;
		
		public static void RefreshPads()
		{
			if (RefreshingPads != null) {
				RefreshingPads();
			}
		}
		
		/// <summary>
		/// Gets the thread which should be used for all evaluations.
		/// For the time being, it is the selected thread, but we might
		/// want to have a dedicated evaluation thread in the future.
		/// </summary>
		/// <remarks>
		/// This exists for two reasons:
		///  1) So that the addin has explicit control over evaluations rather than the core
		///  2) The need to pass this to calls is a reminder that they might do evaluation
		/// </remarks>
		public static Thread EvalThread {
			get {
				if (CurrentProcess == null)
					throw new GetValueException("Debugger is not running");
				if (CurrentProcess.IsRunning)
					throw new GetValueException("Process is not paused");
				if (CurrentThread == null)
					throw new GetValueException("No thread selected");
				
				return CurrentThread;
			}
		}
		
		enum StopAttachedProcessDialogResult {
			Detach = 0,
			Terminate = 1,
			Cancel = 2
		}
		
		bool attached;
		
		ICorPublish corPublish;
		
		/// <inheritdoc/>
		public override bool BreakAtBeginning { get; set; }
		
		public bool ServiceInitialized {
			get { return CurrentDebugger != null; }
		}
		
		public WindowsDebugger()
		{
			Instance = this;
		}
		
		#region IDebugger Members
		
		string errorDebugging      = "${res:XML.MainMenu.DebugMenu.Error.Debugging}";
		string errorNotDebugging   = "${res:XML.MainMenu.DebugMenu.Error.NotDebugging}";
		// string errorProcessRunning = "${res:XML.MainMenu.DebugMenu.Error.ProcessRunning}";
		// string errorProcessPaused  = "${res:XML.MainMenu.DebugMenu.Error.ProcessPaused}";
		// string errorCannotStepNoActiveFunction = "${res:MainWindow.Windows.Debug.Threads.CannotStepNoActiveFunction}";
		
		public override bool IsDebugging {
			get {
				return ServiceInitialized && CurrentProcess != null;
			}
		}
		
		public override bool IsAttached {
			get {
				return ServiceInitialized && attached;
			}
		}
		
		public override bool IsProcessRunning {
			get {
				return IsDebugging && CurrentProcess.IsRunning;
			}
		}
		
		public override bool CanDebug(IProject project)
		{
			return true;
		}
		
		public override bool Supports(DebuggerFeatures feature)
		{
			return true;
		}
		
		public override void Start(ProcessStartInfo processStartInfo)
		{
			if (IsDebugging) {
				MessageService.ShowMessage(errorDebugging);
				return;
			}
			if (!ServiceInitialized) {
				InitializeService();
			}

			string version = CurrentDebugger.GetProgramVersion(processStartInfo.FileName);
			
			if (version.StartsWith("v1.0")) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}");
			} else if (version.StartsWith("v1.1")) {
				MessageService.ShowMessage(StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}").Replace("1.0", "1.1"));
//			} else if (string.IsNullOrEmpty(version)) {
//				// Not a managed assembly
//				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.BadAssembly}");
			} else if (CurrentDebugger.IsKernelDebuggerEnabled) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.KernelDebuggerEnabled}");
			} else {
				attached = false;
				OnDebugStarting(EventArgs.Empty);
				
				UpdateBreakpointLines();
				
				try {
					CurrentProcess = CurrentDebugger.Start(processStartInfo.FileName, processStartInfo.WorkingDirectory, processStartInfo.Arguments, this.BreakAtBeginning);
					debugger_ProcessStarted();
				} catch (System.Exception e) {
					// COMException: The request is not supported. (Exception from HRESULT: 0x80070032)
					// COMException: The application has failed to start because its side-by-side configuration is incorrect. Please see the application event log for more detail. (Exception from HRESULT: 0x800736B1)
					// COMException: The requested operation requires elevation. (Exception from HRESULT: 0x800702E4)
					// COMException: The directory name is invalid. (Exception from HRESULT: 0x8007010B)
					// BadImageFormatException:  is not a valid Win32 application. (Exception from HRESULT: 0x800700C1)
					// UnauthorizedAccessException: Отказано в доступе. (Исключение из HRESULT: 0x80070005 (E_ACCESSDENIED))
					if (e is COMException || e is BadImageFormatException || e is UnauthorizedAccessException) {
						string msg = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.CannotStartProcess}");
						msg += " " + e.Message;
						if (e is COMException && ((uint)((COMException)e).ErrorCode == 0x80070032)) {
							msg += Environment.NewLine + Environment.NewLine;
							msg += "64-bit debugging is not supported.  Please set Project -> Project Options... -> Compiling -> Target CPU to 32bit.";
						}
						MessageService.ShowMessage(msg);
						
						OnDebugStopped(EventArgs.Empty);
					} else {
						throw;
					}
				}
			}
		}
		
		public override void ShowAttachDialog()
		{
			using (AttachToProcessForm attachForm = new AttachToProcessForm()) {
				if (attachForm.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					Attach(attachForm.Process);
				}
			}
		}
		
		public override void Attach(System.Diagnostics.Process existingProcess)
		{
			if (existingProcess == null)
				return;
			
			if (IsDebugging) {
				MessageService.ShowMessage(errorDebugging);
				return;
			}
			if (!ServiceInitialized) {
				InitializeService();
			}
			
			string version = CurrentDebugger.GetProgramVersion(existingProcess.MainModule.FileName);
			if (version.StartsWith("v1.0")) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}");
			} else {
				OnDebugStarting(EventArgs.Empty);
				
				UpdateBreakpointLines();
				
				try {
					CurrentProcess = CurrentDebugger.Attach(existingProcess);
					debugger_ProcessStarted();
					attached = true;
					CurrentProcess.ModuleLoaded += process_Modules_Added;
				} catch (System.Exception e) {
					// CORDBG_E_DEBUGGER_ALREADY_ATTACHED
					if (e is COMException || e is UnauthorizedAccessException) {
						string msg = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Error.CannotAttachToProcess}");
						MessageService.ShowMessage(msg + " " + e.Message);
						
						OnDebugStopped(EventArgs.Empty);
					} else {
						throw;
					}
				}
			}
		}

		public override void Detach()
		{
			ClassBrowserSupport.Detach(CurrentProcess);
			CurrentDebugger.Detach();
		}
		
		public override void StartWithoutDebugging(ProcessStartInfo processStartInfo)
		{
			System.Diagnostics.Process.Start(processStartInfo);
		}
		
		public override void Stop()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.Stop}");
				return;
			}
			if (IsAttached) {
				StopAttachedProcessDialogResult result = ShowStopAttachedProcessDialog();
				switch (result) {
					case StopAttachedProcessDialogResult.Terminate:
						CurrentProcess.Terminate();
						attached = false;
						break;
					case StopAttachedProcessDialogResult.Detach:
						Detach();
						attached = false;
						break;
				}
			} else {
				CurrentProcess.Terminate();
			}
		}
		
		public override void Break()
		{
			if (CurrentProcess != null && CurrentProcess.IsRunning) {
				CurrentProcess.Break();
			}
		}
		
		public override void Continue()
		{
			if (CurrentProcess != null && CurrentProcess.IsPaused) {
				CurrentProcess.AsyncContinue();
			}
		}
		
		public override void StepInto()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepInto();
			}
		}
		
		public override void StepOver()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepOver();
			}
		}
		
		public override void StepOut()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepOut();
			}
		}
		
		public bool IsManaged(int processId)
		{
			corPublish = new CorpubPublishClass();
			Debugger.Interop.TrackedComObjects.Track(corPublish);
			
			ICorPublishProcess process = corPublish.GetProcess((uint)processId);
			if (process != null) {
				return process.IsManaged() != 0;
			}
			return false;
		}
		
		public override bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
		{
			if (CurrentStackFrame != null) {
				if (CurrentStackFrame.SetIP(filename, line, column, dryRun)) {
					WindowsDebugger.RefreshPads();
					JumpToCurrentLine();
				}
			}
			return false;
		}
		
		#endregion
		
		public event EventHandler Initialize;
		
		public void InitializeService()
		{
			List<ISymbolSource> symbolSources = new List<ISymbolSource>();
			symbolSources.Add(PdbSymbolSource);
			symbolSources.AddRange(AddInTree.BuildItems<ISymbolSource>("/SharpDevelop/Services/DebuggerService/SymbolSource", null, false));
			
			// init NDebugger
			CurrentDebugger = new NDebugger();
			CurrentDebugger.Options = DebuggingOptions.Instance;
			CurrentDebugger.SymbolSources = symbolSources;
			
			foreach (BreakpointBookmark b in SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>()) {
				AddBreakpoint(b);
			}
			
			SD.BookmarkManager.BookmarkAdded += (sender, e) => {
				BreakpointBookmark bm = e.Bookmark as BreakpointBookmark;
				if (bm != null) {
					AddBreakpoint(bm);
				}
			};
			
			SD.BookmarkManager.BookmarkRemoved += (sender, e) => {
				BreakpointBookmark bm = e.Bookmark as BreakpointBookmark;
				if (bm != null) {
					Breakpoint bp = bm.InternalBreakpointObject as Breakpoint;
					CurrentDebugger.RemoveBreakpoint(bp);
				}
			};
			
			if (Initialize != null) {
				Initialize(this, null);
			}
		}
		
		void UpdateBreakpointLines()
		{
			foreach (BreakpointBookmark bookmark in SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>()) {
				Breakpoint breakpoint = bookmark.InternalBreakpointObject as Breakpoint;
				breakpoint.Line = bookmark.LineNumber;
				breakpoint.Column = bookmark.ColumnNumber;
			}
		}
		
		void UpdateBreakpointIcons()
		{
			foreach (BreakpointBookmark bookmark in SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>()) {
				Breakpoint breakpoint = bookmark.InternalBreakpointObject as Breakpoint;
				bookmark.IsHealthy = (CurrentProcess == null) || breakpoint.IsSet;
			}
		}
		
		void AddBreakpoint(BreakpointBookmark bookmark)
		{
			Breakpoint breakpoint = CurrentDebugger.AddBreakpoint(bookmark.FileName, bookmark.LineNumber, 0, bookmark.IsEnabled);
			bookmark.InternalBreakpointObject = breakpoint;
			bookmark.IsHealthy = (CurrentProcess == null) || breakpoint.IsSet;
			bookmark.IsEnabledChanged += delegate { breakpoint.IsEnabled = bookmark.IsEnabled; };
		}
		
		bool EvaluateCondition(string code)
		{
			try {
				if (CurrentStackFrame == null || CurrentStackFrame.NextStatement == null)
					return false;
				var val = Evaluate(code);
				if (val != null && val.Type.IsPrimitiveType() && val.PrimitiveValue is bool)
					return (bool)val.PrimitiveValue;
				else
					return false;
			} catch (GetValueException e) {
				string errorMessage = "Error while evaluating breakpoint condition " + code + ":\n" + e.Message + "\n";
				BaseDebuggerService.PrintDebugMessage(errorMessage);
				SD.MainThread.InvokeAsyncAndForget(() => MessageService.ShowWarning(errorMessage));
				return true;
			}
		}
		
		void LogMessage(object sender, MessageEventArgs e)
		{
			BaseDebuggerService.PrintDebugMessage(e.Message);
		}
		
		void debugger_ProcessStarted()
		{
			OnDebugStarted(EventArgs.Empty);
			
			CurrentProcess.ModuleLoaded   += (s, e) => UpdateBreakpointIcons();
			CurrentProcess.ModuleLoaded   += (s, e) => RefreshPads();
			CurrentProcess.ModuleUnloaded += (s, e) => RefreshPads();
			CurrentProcess.LogMessage     += LogMessage;
			CurrentProcess.Paused         += debuggedProcess_DebuggingPaused;
			CurrentProcess.Resumed        += debuggedProcess_DebuggingResumed;
			CurrentProcess.Exited         += (s, e) => debugger_ProcessExited();
			ClassBrowserSupport.Attach(CurrentProcess);
			
			UpdateBreakpointIcons();
		}
		
		void debugger_ProcessExited()
		{
			OnDebugStopped(EventArgs.Empty);
			
			ClassBrowserSupport.Detach(CurrentProcess);
			CurrentProcess = null;
			CurrentThread = null;
			CurrentStackFrame = null;
			
			UpdateBreakpointIcons();
			RefreshPads();
		}
		
		void debuggedProcess_DebuggingPaused(object sender, DebuggerPausedEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			
			CurrentProcess = e.Process;
			CurrentThread = e.Thread;
			CurrentStackFrame = CurrentThread != null ? CurrentThread.MostRecentStackFrame : null;
			
			// We can have several events happening at the same time
			bool breakProcess = e.Break;
			
			// Handle thrown exceptions
			foreach(Thread exceptionThread in e.ExceptionsThrown) {
				
				JumpToCurrentLine();
				
				Thread evalThread = exceptionThread;
				
				bool isUnhandled = (exceptionThread.CurrentExceptionType == ExceptionType.Unhandled);
				Value exception = exceptionThread.CurrentException.GetPermanentReferenceOfHeapValue();
				List<Value> innerExceptions = new List<Value>();
				for(Value innerException = exception; !innerException.IsNull; innerException = innerException.GetFieldValue("_innerException")) {
					innerExceptions.Add(innerException.GetPermanentReferenceOfHeapValue());
				}
				
				// Get the exception description
				string stacktrace = string.Empty;
				for(int i = 0; i < innerExceptions.Count; i++) {
					if (i > 0) {
						stacktrace += " ---> ";
					}
					stacktrace += innerExceptions[i].Type.FullName;
					Value messageValue = innerExceptions[i].GetFieldValue("_message");
					if (!messageValue.IsNull) {
						stacktrace += ": " + messageValue.AsString();
					}
				}
				stacktrace += Environment.NewLine + Environment.NewLine;
				
				// Get the stacktrace
				string formatSymbols   = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}");
				string formatNoSymbols = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}");
				if (isUnhandled) {
					// Need to intercept now so that we can evaluate properties
					// Intercept may fail (eg StackOverflow)
					if (exceptionThread.InterceptException()) {
						try {
							// Try to evaluate the StackTrace property to get the .NET formated stacktrace
							for(int i = innerExceptions.Count - 1; i >= 0; i--) {
								Value stackTraceValue = innerExceptions[i].GetPropertyValue(evalThread, "StackTrace");
								if (!stackTraceValue.IsNull) {
									stacktrace += stackTraceValue.AsString() + Environment.NewLine;
								}
								if (i > 0) {
									stacktrace += "   " + StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.EndOfInnerException}") + Environment.NewLine;
								}
							}
						} catch (GetValueException) {
							stacktrace += exceptionThread.GetStackTrace(formatSymbols, formatNoSymbols);
						}
					} else {
						stacktrace += StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptException}") + Environment.NewLine + Environment.NewLine;
						stacktrace += exceptionThread.GetStackTrace(formatSymbols, formatNoSymbols);
					}
				} else {
					// Do not intercept handled expetions
					stacktrace += exceptionThread.GetStackTrace(formatSymbols, formatNoSymbols);
				}
				
				string title = isUnhandled ? StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Unhandled}") : StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Handled}");
				string type = string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Message}"), exception.Type);
				Bitmap icon = WinFormsResourceService.GetBitmap(isUnhandled ? "Icons.32x32.Error" : "Icons.32x32.Warning");
				
				if (DebuggeeExceptionForm.Show(e.Process, title, type, stacktrace, icon, isUnhandled)) {
					breakProcess = true;
					// The dialog box is allowed to kill the process
					if (e.Process.HasExited) {
						return;
					}
					// Intercept handled exception *after* the user decided to break
					if (!isUnhandled) {
						if (!exceptionThread.InterceptException()) {
							MessageService.ShowError("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptHandledException}");
						}
					}
				}
			}
			
			// Handle breakpoints
			foreach (Breakpoint breakpoint in e.BreakpointsHit) {
				var bookmark = SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>().First(bm => bm.InternalBreakpointObject == breakpoint);
				
				if (string.IsNullOrEmpty(bookmark.Condition)) {
					breakProcess = true;
				} else {
					if (EvaluateCondition(bookmark.Condition)) {
						breakProcess = true;
						BaseDebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAtBecause}") + "\n", bookmark.LineNumber, bookmark.FileName, bookmark.Condition));
					}
				}
			}
			
			if (breakProcess) {
				JumpToCurrentLine();
				RefreshPads();
			} else {
				e.Process.AsyncContinue();
			}
		}
		
		void debuggedProcess_DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			RemoveCurrentLineMarker();
			
			CurrentThread = null;
			CurrentStackFrame = null;
			
			RefreshPads();
		}
		
		public static Value Evaluate(string code, bool allowMethodInvoke = true, bool allowSetValue = false)
		{
			if (CurrentStackFrame == null || CurrentStackFrame.NextStatement == null)
				throw new GetValueException("no stackframe available!");
			var location = CurrentStackFrame.NextStatement;
			var fileName = new FileName(location.Filename);
			var rr = SD.ParserService.ResolveSnippet(fileName, new TextLocation(location.StartLine, location.StartColumn), code);
			return new ExpressionEvaluationVisitor(CurrentStackFrame, EvalThread, CurrentStackFrame.AppDomain.Compilation, allowMethodInvoke, allowSetValue).Convert(rr);
		}

		public void JumpToCurrentLine()
		{
			if (CurrentStackFrame == null)
				return;
			
			SD.Workbench.MainWindow.Activate();
			RemoveCurrentLineMarker();
			SequencePoint nextStatement = CurrentStackFrame.NextStatement;
			if (nextStatement != null) {
				JumpToCurrentLine(nextStatement.Filename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
			}
		}
		
		StopAttachedProcessDialogResult ShowStopAttachedProcessDialog()
		{
			string caption = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Stop}");
			string message = StringParser.Parse("${res:MainWindow.Windows.Debug.StopProcessDialog.Message}");
			string[] buttonLabels = new string[] { StringParser.Parse("${res:XML.MainMenu.DebugMenu.Detach}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Terminate}"), StringParser.Parse("${res:Global.CancelButtonText}") };
			return (StopAttachedProcessDialogResult)MessageService.ShowCustomDialog(caption, message, (int)StopAttachedProcessDialogResult.Detach, (int)StopAttachedProcessDialogResult.Cancel, buttonLabels);
		}
		
		void process_Modules_Added(object sender, ModuleEventArgs e)
		{
			if (ProjectService.OpenSolution == null)
				return;
			
			ProjectService.OpenSolution.Projects
				.Where(p => e.Module.Name.IndexOf(p.Name) >= 0)
				.ForEach(p => e.Module.LoadSymbolsFromDisk(new []{ Path.GetDirectoryName(p.OutputAssemblyFullPath) }));
		}
		
		public override void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (!(IsDebugging && CurrentProcess.IsPaused))
				return;
			if (CurrentStackFrame == null)
				return;
			var resolveResult = SD.ParserService.Resolve(e.Editor, e.LogicalPosition, CurrentStackFrame.AppDomain.Compilation);
			if (resolveResult == null)
				return;
			if (resolveResult is LocalResolveResult || resolveResult is MemberResolveResult) {
				string text = ResolveResultPrettyPrinter.Print(resolveResult);
				Func<Value> getValue = delegate {
					ExpressionEvaluationVisitor eval = new ExpressionEvaluationVisitor(CurrentStackFrame, EvalThread, CurrentStackFrame.AppDomain.Compilation);
					return eval.Convert(resolveResult);
				};
				try {
					var rootNode = new ValueNode(ClassBrowserIconService.LocalVariable, text, getValue);
					e.SetToolTip(new DebuggerTooltipControl(rootNode));
				} catch (InvalidOperationException ex) {
					SD.Log.Warn(ex);
				}
			}
		}
		
		public override void RemoveCurrentLineMarker()
		{
			CurrentLineBookmark.Remove();
		}
		
		public override void ToggleBreakpointAt(ITextEditor editor, int lineNumber)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");

			if (!SD.BookmarkManager.RemoveBookmarkAt(editor.FileName, lineNumber, b => b is BreakpointBookmark)) {
				SD.BookmarkManager.AddMark(new BreakpointBookmark(), editor.Document, lineNumber);
			}
		}
		
		public override void JumpToCurrentLine(string sourceFullFilename, int startLine, int startColumn, int endLine, int endColumn)
		{
			if (string.IsNullOrEmpty(sourceFullFilename))
				return;
			IViewContent viewContent = FileService.OpenFile(sourceFullFilename);
			if (viewContent != null) {
				IPositionable positionable = viewContent.GetService<IPositionable>();
				if (positionable != null) {
					positionable.JumpTo(startLine, startColumn);
				}
			}
			CurrentLineBookmark.SetPosition(viewContent, startLine, startColumn, endLine, endColumn);
		}
	}
}
