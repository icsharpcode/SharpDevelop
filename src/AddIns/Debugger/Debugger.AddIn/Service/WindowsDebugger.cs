// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Debugger;
using Debugger.AddIn;
using Debugger.AddIn.Tooltips;
using Debugger.AddIn.TreeModel;
using Debugger.Interop.CorPublish;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;
using Process = Debugger.Process;
using StackFrame = Debugger.StackFrame;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Services
{
	public class WindowsDebugger : IDebugger
	{
		public static WindowsDebugger Instance { get; set; }
		
		public static NDebugger  CurrentDebugger { get; private set; }
		public static Process    CurrentProcess { get; private set; }
		public static Thread     CurrentThread { get; set; }
		public static StackFrame CurrentStackFrame { get; set; }
		
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
		
		internal IDebuggerDecompilerService debuggerDecompilerService;
		
		/// <inheritdoc/>
		public bool BreakAtBeginning { get; set; }
		
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
		
		public bool IsDebugging {
			get {
				return ServiceInitialized && CurrentProcess != null;
			}
		}
		
		public bool IsAttached {
			get {
				return ServiceInitialized && attached;
			}
		}
		
		public bool IsProcessRunning {
			get {
				return IsDebugging && CurrentProcess.IsRunning;
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
				if (DebugStarting != null)
					DebugStarting(this, EventArgs.Empty);
				
				UpdateBreakpointLines();
				
				try {
					// set the JIT flag for evaluating optimized code
					Process.DebugMode = DebugModeFlag.Debug;
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
				if (attachForm.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					Attach(attachForm.Process);
				}
			}
		}
		
		public void Attach(System.Diagnostics.Process existingProcess)
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
				if (DebugStarting != null)
					DebugStarting(this, EventArgs.Empty);
				
				UpdateBreakpointLines();
				
				try {
					// set the JIT flag for evaluating optimized code
					Process.DebugMode = DebugModeFlag.Debug;
					CurrentProcess = CurrentDebugger.Attach(existingProcess);
					debugger_ProcessStarted();
					attached = true;
					CurrentProcess.ModuleLoaded += process_Modules_Added;
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
			CurrentDebugger.Detach();
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
		
		public void Break()
		{
			if (CurrentProcess != null && CurrentProcess.IsRunning) {
				CurrentProcess.Break();
			}
		}
		
		public void Continue()
		{
			if (CurrentProcess != null && CurrentProcess.IsPaused) {
				CurrentProcess.AsyncContinue();
			}
		}
		
		public void StepInto()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepInto();
			}
		}
		
		public void StepOver()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepOver();
			}
		}
		
		public void StepOut()
		{
			if (CurrentStackFrame != null) {
				CurrentStackFrame.AsyncStepOut();
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
		
		public bool SetInstructionPointer(string filename, int line, int column, bool dryRun)
		{
			if (CurrentStackFrame != null) {
				SourcecodeSegment seg = CurrentStackFrame.SetIP(filename, line, column, dryRun);
				WindowsDebugger.RefreshPads();
				JumpToCurrentLine();
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
			// get decompiler service
			debuggerDecompilerService = SD.GetService<IDebuggerDecompilerService>();
			
			// init NDebugger
			CurrentDebugger = new NDebugger();
			CurrentDebugger.Options = DebuggingOptions.Instance;
			
			DebuggerService.BreakPointAdded  += delegate (object sender, BreakpointBookmarkEventArgs e) {
				AddBreakpoint(e.BreakpointBookmark);
			};
			
			foreach (BreakpointBookmark b in DebuggerService.Breakpoints) {
				AddBreakpoint(b);
			}
			
			SD.BookmarkManager.BookmarkRemoved += (sender, e) => {
				BreakpointBookmark bm = e.Bookmark as BreakpointBookmark;
				if (bm != null) {
					Breakpoint bp = bm.InternalBreakpointObject as Breakpoint;
					bp.IsEnabled = false;
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
			Breakpoint breakpoint = null;
			
			if (bookmark is DecompiledBreakpointBookmark) {
				try {
					if (debuggerDecompilerService == null) {
						LoggingService.Warn("No IDebuggerDecompilerService found!");
						return;
					}
					var dbb = (DecompiledBreakpointBookmark)bookmark;
					MemberReference memberReference = null;
					
					string assemblyFile, typeName;
					if (DecompiledBreakpointBookmark.GetAssemblyAndType(dbb.FileName, out assemblyFile, out typeName)) {
						memberReference = dbb.GetMemberReference(debuggerDecompilerService.GetAssemblyResolver(assemblyFile));
					}
					
					int token = memberReference.MetadataToken.ToInt32();
					if (!debuggerDecompilerService.CheckMappings(token))
						debuggerDecompilerService.DecompileOnDemand(memberReference as TypeDefinition);
					
					int[] ilRanges;
					int methodToken;
					#warning decompiler
//					if (debuggerDecompilerService.GetILAndTokenByLineNumber(token, dbb.LineNumber, out ilRanges, out methodToken)) {
//						CurrentDebugger.AddILBreakpoint(memberReference.FullName, dbb.LineNumber, memberReference.MetadataToken.ToInt32(), methodToken, ilRanges[0], dbb.IsEnabled);
//					}
				} catch (System.Exception ex) {
					LoggingService.Error("Error on DecompiledBreakpointBookmark: " + ex.Message);
				}
			} else {
				breakpoint = CurrentDebugger.AddBreakpoint(bookmark.FileName, bookmark.LineNumber, 0, bookmark.IsEnabled);
			}
			
			if (breakpoint == null) {
				LoggingService.Warn(string.Format("unable to create breakpoint: {0}", bookmark.ToString()));
				return;
			}
			
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
				DebuggerService.PrintDebugMessage(errorMessage);
				SD.MainThread.InvokeAsync(() => MessageService.ShowWarning(errorMessage)).FireAndForget();
				return true;
			}
		}
		
		void LogMessage(object sender, MessageEventArgs e)
		{
			DebuggerService.PrintDebugMessage(e.Message);
		}
		
		void debugger_ProcessStarted()
		{
			if (DebugStarted != null) {
				DebugStarted(this, EventArgs.Empty);
			}
			
			CurrentProcess.ModuleLoaded   += (s, e) => UpdateBreakpointIcons();
			CurrentProcess.ModuleLoaded   += (s, e) => RefreshPads();
			CurrentProcess.ModuleUnloaded += (s, e) => RefreshPads();
			CurrentProcess.LogMessage     += LogMessage;
			CurrentProcess.Paused         += debuggedProcess_DebuggingPaused;
			CurrentProcess.Resumed        += debuggedProcess_DebuggingResumed;
			CurrentProcess.Exited         += (s, e) => debugger_ProcessExited();
			
			UpdateBreakpointIcons();
		}
		
		void debugger_ProcessExited()
		{
			if (DebugStopped != null) {
				DebugStopped(this, EventArgs.Empty);
			}
			
			CurrentProcess = null;
			CurrentThread = null;
			CurrentStackFrame = null;
			
			UpdateBreakpointIcons();
			RefreshPads();
		}
		
		void debuggedProcess_DebuggingPaused(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			
			CurrentProcess = e.Process;
			CurrentThread = e.Thread;
			CurrentStackFrame = CurrentThread != null ? CurrentThread.MostRecentUserStackFrame : null;
			
			LoggingService.Info("Jump to current line");
			JumpToCurrentLine();
			
			if (e.ExceptionThrown != null) {
				HandleException(e);
			}
			
			foreach (Breakpoint breakpoint in e.BreakpointsHit) {
				var bookmark = SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>().First(bm => bm.InternalBreakpointObject == breakpoint);
				
				LoggingService.Debug(bookmark.Action + " " + bookmark.ScriptLanguage + " " + bookmark.Condition);
				
				switch (bookmark.Action) {
					case BreakpointAction.Break:
						break;
					case BreakpointAction.Condition:
						if (EvaluateCondition(bookmark.Condition))
							DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAtBecause}") + "\n", bookmark.LineNumber, bookmark.FileName, bookmark.Condition));
						else
							CurrentProcess.AsyncContinue();
						break;
					case BreakpointAction.Trace:
						DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAt}") + "\n", bookmark.LineNumber, bookmark.FileName));
						break;
				}
			}
			
			RefreshPads();
		}
		
		void debuggedProcess_DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			DebuggerService.RemoveCurrentLineMarker();
			
			CurrentThread = null;
			CurrentStackFrame = null;
			
			RefreshPads();
		}
		
		void HandleException(DebuggerEventArgs e)
		{
			JumpToCurrentLine();
			
			StringBuilder stacktraceBuilder = new StringBuilder();
			
			if (e.ExceptionThrown.IsUnhandled) {
				// Need to intercept now so that we can evaluate properties
				if (e.Thread.InterceptException(e.ExceptionThrown)) {
					stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
					string stackTrace;
					try {
						stackTrace = e.ExceptionThrown.GetStackTrace(e.Thread, StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.EndOfInnerException}"));
					} catch (GetValueException) {
						stackTrace = e.Thread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}"));
					}
					stacktraceBuilder.Append(stackTrace);
				} else {
					// For example, happens on stack overflow
					stacktraceBuilder.AppendLine(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptException}"));
					stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
					stacktraceBuilder.Append(e.Thread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}")));
				}
			} else {
				stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
				stacktraceBuilder.Append(e.Thread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}")));
			}
			
			string title = e.ExceptionThrown.IsUnhandled ? StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Unhandled}") : StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Handled}");
			string message = string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Message}"), e.ExceptionThrown.Type);
			Bitmap icon = WinFormsResourceService.GetBitmap(e.ExceptionThrown.IsUnhandled ? "Icons.32x32.Error" : "Icons.32x32.Warning");
			
			DebuggeeExceptionForm.Show(e.Process, title, message, stacktraceBuilder.ToString(), icon, e.ExceptionThrown.IsUnhandled, e.ExceptionThrown);
		}
		
		public bool BreakAndInterceptHandledException(Debugger.Exception exception)
		{
			if (!CurrentThread.InterceptException(exception)) {
				MessageService.ShowError("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptHandledException}");
				return false;
			}
			JumpToCurrentLine();
			return true;
		}
		
		public static Value Evaluate(string code)
		{
			if (CurrentStackFrame == null || CurrentStackFrame.NextStatement == null)
				throw new GetValueException("no stackframe available!");
			var location = CurrentStackFrame.NextStatement;
			var fileName = new FileName(location.Filename);
			var rr = SD.ParserService.ResolveSnippet(fileName, new TextLocation(location.StartLine, location.StartColumn), new ParseableFileContentFinder().Create(fileName), code, null, System.Threading.CancellationToken.None);
			return new ExpressionEvaluationVisitor(CurrentStackFrame, EvalThread, CurrentStackFrame.AppDomain.Compilation).Convert(rr);
		}

		public void JumpToCurrentLine()
		{
			if (CurrentThread == null)
				return;
			
			SD.Workbench.MainWindow.Activate();
			
			// if (debuggedProcess.IsSelectedFrameForced()) {
			if (CurrentThread != null && CurrentStackFrame.HasSymbols) {
				JumpToSourceCode();
			} else {
				#warning		JumpToDecompiledCode(CurrentStackFrame);
			}
//			} else {
//				var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
//				// other pause reasons
//				if (frame != null && frame.HasSymbols) {
//					JumpToSourceCode();
//				} else {
//					// use most recent stack frame because we don't have the symbols
//					JumpToDecompiledCode(debuggedProcess.SelectedThread.MostRecentStackFrame);
//				}
//			}
		}

		void JumpToSourceCode()
		{
			if (CurrentProcess == null || CurrentStackFrame == null)
				return;
			
			SourcecodeSegment nextStatement = CurrentStackFrame.NextStatement;
			if (nextStatement != null) {
				DebuggerService.RemoveCurrentLineMarker();
				DebuggerService.JumpToCurrentLine(nextStatement.Filename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
			}
		}
		
		/*
		void JumpToDecompiledCode(Debugger.StackFrame frame)
		{
			if (frame == null) {
				LoggingService.Error("No stack frame!");
				return;
			}
			
			if (debuggerDecompilerService == null) {
				LoggingService.Warn("No IDebuggerDecompilerService found!");
				return;
			}
			
			// check for options - if these options are enabled, debugging decompiled code should not continue
			if (!CurrentProcess.Options.DecompileCodeWithoutSymbols) {
				LoggingService.Info("Decompiled code debugging is disabled!");
				return;
			}
			DebuggerService.RemoveCurrentLineMarker();
			// get external data
			int typeToken = frame.MethodInfo.DeclaringType.MetadataToken;
			int methodToken = frame.MethodInfo.MetadataToken;
			int ilOffset = frame.IP;
			int[] ilRanges = null;
			int line = -1;
			bool isMatch = false;
			var debugType = (DebugType)frame.MethodInfo.DeclaringType;
			debuggerDecompilerService.DebugStepInformation = Tuple.Create(methodToken, ilOffset);
			
			if (debuggerDecompilerService.GetILAndLineNumber(typeToken, methodToken, ilOffset, out ilRanges, out line, out isMatch)) {
				// update marker & navigate to line
				NavigationService.NavigateTo(debugType.DebugModule.FullPath,
				                             debugType.FullNameWithoutGenericArguments,
				                             IDStringProvider.GetIDString(frame.MethodInfo),
				                             line);
			} else
			{
				// no line => do decompilation
				NavigationService.NavigateTo(debugType.DebugModule.FullPath,
				                             debugType.FullNameWithoutGenericArguments,
				                             IDStringProvider.GetIDString(frame.MethodInfo));
				
			}
		}
		 */
		
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
		
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (!(IsDebugging && CurrentProcess.IsPaused))
				return;
			if (e.ResolveResult == null)
				return;
			if (e.ResolveResult is LocalResolveResult || e.ResolveResult is MemberResolveResult || e.ResolveResult is InvocationResolveResult) {
				try {
					ExpressionEvaluationVisitor eval = new ExpressionEvaluationVisitor(CurrentStackFrame, EvalThread, CurrentStackFrame.AppDomain.Compilation);
					Value result = eval.Convert(e.ResolveResult);
					string text = new ResolveResultPrettyPrinter().Print(e.ResolveResult);
					e.SetToolTip(new DebuggerTooltipControl(ValueNode.GetTooltipFor(text, result)));
				} catch (GetValueException ex) {
					e.SetToolTip(ex.Message);
				} catch (InvalidOperationException) {
					return;
				}
			}
		}
	}
}
