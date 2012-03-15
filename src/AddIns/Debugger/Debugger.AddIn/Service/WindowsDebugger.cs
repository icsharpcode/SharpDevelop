// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using Debugger;
using Debugger.AddIn.Tooltips;
using Debugger.AddIn.TreeModel;
using Debugger.Interop.CorPublish;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;
using Process = Debugger.Process;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace ICSharpCode.SharpDevelop.Services
{
	public class WindowsDebugger : IDebugger
	{
		enum StopAttachedProcessDialogResult {
			Detach = 0,
			Terminate = 1,
			Cancel = 2
		}
		
		bool attached;
		
		NDebugger debugger;
		
		ICorPublish corPublish;
		
		Process debuggedProcess;
		
		internal IDebuggerDecompilerService debuggerDecompilerService;
		
		public NDebugger DebuggerCore { get { return debugger; } }
		
		public Process DebuggedProcess { get { return debuggedProcess; } }
		
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
		
		public static event EventHandler<DebuggerEventArgs> RefreshingPads;
		
		public static void RefreshPads()
		{
			RefreshPads(new DebuggerEventArgs(CurrentProcess));
		}
		
		public static void RefreshPads(DebuggerEventArgs e)
		{
			if (RefreshingPads != null) {
				RefreshingPads(null, e);
			}
		}
		
		/// <inheritdoc/>
		public bool BreakAtBeginning {
			get;
			set;
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
				
				UpdateBreakpointLines();
				
				try {
					// set the JIT flag for evaluating optimized code
					Process.DebugMode = DebugModeFlag.Debug;
					this.debuggedProcess = debugger.Start(processStartInfo.FileName, processStartInfo.WorkingDirectory, processStartInfo.Arguments, this.BreakAtBeginning);
					debugger_ProcessStarted(this.debuggedProcess);
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
				if (attachForm.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
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
			
			string version = debugger.GetProgramVersion(existingProcess.MainModule.FileName);
			if (version.StartsWith("v1.0")) {
				MessageService.ShowMessage("${res:XML.MainMenu.DebugMenu.Error.Net10NotSupported}");
			} else {
				if (DebugStarting != null)
					DebugStarting(this, EventArgs.Empty);
				
				UpdateBreakpointLines();
				
				try {
					// set the JIT flag for evaluating optimized code
					Process.DebugMode = DebugModeFlag.Debug;
					this.debuggedProcess = debugger.Attach(existingProcess);
					debugger_ProcessStarted(this.debuggedProcess);
					attached = true;
					this.debuggedProcess.ModuleLoaded += process_Modules_Added;
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
		Debugger.StackFrame GetStackFrame()
		{
			bool isMatch = false;
			int line = -1;
			int[] ilRange = null;
			
			var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
			int typeToken = frame.MethodInfo.DeclaringType.MetadataToken;
			int methodToken = frame.MethodInfo.MetadataToken;
			
			// get the mapped instruction from the current line marker or the next one
			if (!debuggerDecompilerService.GetILAndLineNumber(typeToken, methodToken, frame.IP, out ilRange, out line, out isMatch)){
				frame.SourceCodeLine = 0;
				frame.ILRanges = new [] { 0, 1 };
			} else {
				frame.SourceCodeLine = line;
				frame.ILRanges = ilRange;
			}
			
			return frame;
		}
		
		public void StepInto()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepInto}");
				return;
			}
			
			if (debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorProcessRunning, "${res:XML.MainMenu.DebugMenu.StepInto}");
				return;
			}
			
			var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
			if (frame == null) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepInto}");
			} else {
				if (!frame.HasSymbols) {
					// get frame info from external code mappings
					frame = GetStackFrame();
				}
				
				frame.AsyncStepInto();
			}
		}
		
		public void StepOver()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepOver}");
				return;
			}
			
			if (debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorProcessRunning, "${res:XML.MainMenu.DebugMenu.StepOver}");
				return;
			}
			
			var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
			if (frame == null) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepOver}");
			} else {
				if (!frame.HasSymbols) {
					// get frame info from external code mappings
					frame = GetStackFrame();
				}
				
				frame.AsyncStepOver();
			}
		}
		
		public void StepOut()
		{
			if (!IsDebugging) {
				MessageService.ShowMessage(errorNotDebugging, "${res:XML.MainMenu.DebugMenu.StepOut}");
				return;
			}
			
			if (debuggedProcess.IsRunning) {
				MessageService.ShowMessage(errorProcessRunning, "${res:XML.MainMenu.DebugMenu.StepOut}");
				return;
			}
			
			var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
			if (frame == null) {
				MessageService.ShowMessage(errorCannotStepNoActiveFunction, "${res:XML.MainMenu.DebugMenu.StepInto}");
			} else {
				if (!frame.HasSymbols) {
					// get frame info from external code mappings
					frame = GetStackFrame();
				}
				
				frame.AsyncStepOut();
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
		/// Returns null if unsuccessful. Can throw GetValueException.
		/// <exception cref="GetValueException">Thrown when evaluation fails. Exception message explains reason.</exception>
		/// </summary>
		public Value GetValueFromName(string variableName)
		{
			if (!CanEvaluate) {
				return null;
			}
			
			var frame = debuggedProcess.GetCurrentExecutingFrame();
			if (frame == null)
				return null;
			object data = debuggerDecompilerService.GetLocalVariableIndex(frame.MethodInfo.DeclaringType.MetadataToken,
			                                                              frame.MethodInfo.MetadataToken,
			                                                              variableName);
			// evaluate expression
			return ExpressionEvaluator.Evaluate(variableName, SupportedLanguage.CSharp, frame, data);
		}

		/// <summary>
		/// Gets Expression for given variable. Can throw GetValueException.
		/// <exception cref="GetValueException">Thrown when getting expression fails. Exception message explains reason.</exception>
		/// </summary>
		public Expression GetExpression(string variableName)
		{
			if (!CanEvaluate) {
				throw new GetValueException("Cannot evaluate now - debugged process is either null or running or has no selected stack frame");
			}
			return ExpressionEvaluator.ParseExpression(variableName, SupportedLanguage.CSharp);
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
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// Returns null if unsuccessful.
		/// </summary>
		public string GetValueAsString(string variableName)
		{
			try {
				Value val = GetValueFromName(variableName);
				if (val == null) return null;
				return val.AsString();
			} catch (GetValueException) {
				return null;
			}
		}
		
		bool CanEvaluate
		{
			get {
				return debuggedProcess != null && !debuggedProcess.IsRunning &&
					(debuggedProcess.SelectedStackFrame != null || debuggedProcess.SelectedThread.MostRecentStackFrame != null);
			}
		}
		
		/// <summary>
		/// Gets the tooltip control that shows the value of given variable.
		/// Return null if no tooltip is available.
		/// </summary>
		public object GetTooltipControl(Location logicalPosition, string variableName)
		{
			try {
				var tooltipExpression = GetExpression(variableName);
				var valueNode = new ValueNode("Icons.16x16.Local", variableName, () => tooltipExpression.Evaluate(this.DebuggedProcess));
				return new DebuggerTooltipControl(new TreeNode[] { valueNode });
			} catch (System.Exception ex) {
				LoggingService.Error("Error on GetTooltipControl: " + ex.Message);
				return null;
			}
		}
		
		public Debugger.AddIn.TreeModel.TreeNode GetNode(string variable, string currentImageName = null)
		{
			try {
				return new ValueNode(currentImageName ?? "Icons.16x16.Local", variable, () => GetExpression(variable).Evaluate(this.DebuggedProcess));
			} catch (GetValueException) {
				return null;
			}
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
			// get decompiler service
			var items = AddInTree.BuildItems<IDebuggerDecompilerService>("/SharpDevelop/Services/DebuggerDecompilerService", null, false);
			if (items.Count > 0)
				debuggerDecompilerService = items[0];
			
			// init NDebugger
			debugger = new NDebugger();
			debugger.Options = DebuggingOptions.Instance;
			
			DebuggerService.BreakPointAdded  += delegate (object sender, BreakpointBookmarkEventArgs e) {
				AddBreakpoint(e.BreakpointBookmark);
			};
			
			foreach (BreakpointBookmark b in DebuggerService.Breakpoints) {
				AddBreakpoint(b);
			}
			
			BookmarkManager.Removed += (sender, e) => {
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
			foreach (BreakpointBookmark bookmark in BookmarkManager.Bookmarks.OfType<BreakpointBookmark>()) {
				Breakpoint breakpoint = bookmark.InternalBreakpointObject as Breakpoint;
				breakpoint.Line = bookmark.LineNumber;
				breakpoint.Column = bookmark.ColumnNumber;
			}
		}
		
		void UpdateBreakpointIcons()
		{
			foreach (BreakpointBookmark bookmark in BookmarkManager.Bookmarks.OfType<BreakpointBookmark>()) {
				Breakpoint breakpoint = bookmark.InternalBreakpointObject as Breakpoint;
				bookmark.IsHealthy = (debuggedProcess == null) || breakpoint.IsSet;
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
					if (debuggerDecompilerService.GetILAndTokenByLineNumber(token, dbb.LineNumber, out ilRanges, out methodToken)) {
						debugger.AddILBreakpoint(memberReference.FullName, dbb.LineNumber, memberReference.MetadataToken.ToInt32(), methodToken, ilRanges[0], dbb.IsEnabled);
					}
				} catch (System.Exception ex) {
					LoggingService.Error("Error on DecompiledBreakpointBookmark: " + ex.Message);
				}
			} else {
				breakpoint = debugger.AddBreakpoint(bookmark.FileName, bookmark.LineNumber, 0, bookmark.IsEnabled);
			}
			
			if (breakpoint == null) {
				LoggingService.Warn(string.Format("unable to create breakpoint: {0}", bookmark.ToString()));
				return;
			}
			
			bookmark.InternalBreakpointObject = breakpoint;
			bookmark.IsHealthy = (debuggedProcess == null) || breakpoint.IsSet;
			bookmark.IsEnabledChanged += delegate { breakpoint.IsEnabled = bookmark.IsEnabled; };
		}
		
		bool Evaluate(string code, string language)
		{
			try {
				SupportedLanguage supportedLanguage = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), language, true);
				Value val = ExpressionEvaluator.Evaluate(code, supportedLanguage, debuggedProcess.SelectedStackFrame);
				
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
		
		void debugger_ProcessStarted(Process process)
		{
			if (DebugStarted != null) {
				DebugStarted(this, EventArgs.Empty);
			}
			
			process.ModuleLoaded   += (s, e) => UpdateBreakpointIcons();
			process.ModuleLoaded   += (s, e) => RefreshPads(e);
			process.ModuleUnloaded += (s, e) => RefreshPads(e);
			process.LogMessage     += LogMessage;
			process.Paused         += debuggedProcess_DebuggingPaused;
			process.Resumed        += debuggedProcess_DebuggingResumed;
			process.Exited         += (s, e) => debugger_ProcessExited(e.Process);
			
			UpdateBreakpointIcons();
		}
		
		void debugger_ProcessExited(Process process)
		{
			if (DebugStopped != null) {
				DebugStopped(this, EventArgs.Empty);
			}
			debuggedProcess = null;
			UpdateBreakpointIcons();
		}
		
		void debuggedProcess_DebuggingPaused(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			
			LoggingService.Info("Jump to current line");
			JumpToCurrentLine();
			
			if (e.ExceptionThrown != null) {
				HandleException(e);
			}
			
			foreach (Breakpoint breakpoint in e.BreakpointsHit) {
				var bookmark = BookmarkManager.Bookmarks.OfType<BreakpointBookmark>().First(bm => bm.InternalBreakpointObject == breakpoint);
				
				LoggingService.Debug(bookmark.Action + " " + bookmark.ScriptLanguage + " " + bookmark.Condition);
				
				switch (bookmark.Action) {
					case BreakpointAction.Break:
						break;
					case BreakpointAction.Condition:
						if (Evaluate(bookmark.Condition, bookmark.ScriptLanguage))
							DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAtBecause}") + "\n", bookmark.LineNumber, bookmark.FileName, bookmark.Condition));
						else
							this.debuggedProcess.AsyncContinue();
						break;
					case BreakpointAction.Trace:
						DebuggerService.PrintDebugMessage(string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.BreakpointHitAt}") + "\n", bookmark.LineNumber, bookmark.FileName));
						break;
				}
			}
			
			RefreshPads(e);
		}
		
		void debuggedProcess_DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			OnIsProcessRunningChanged(EventArgs.Empty);
			DebuggerService.RemoveCurrentLineMarker();
			
			RefreshPads(e);
		}
		
		void HandleException(DebuggerEventArgs e)
		{
			JumpToCurrentLine();
			
			StringBuilder stacktraceBuilder = new StringBuilder();
			
			if (e.ExceptionThrown.IsUnhandled) {
				// Need to intercept now so that we can evaluate properties
				if (e.Process.SelectedThread.InterceptException(e.ExceptionThrown)) {
					stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
					string stackTrace;
					try {
						stackTrace = e.ExceptionThrown.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.EndOfInnerException}"));
					} catch (GetValueException) {
						stackTrace = e.Process.SelectedThread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}"));
					}
					stacktraceBuilder.Append(stackTrace);
				} else {
					// For example, happens on stack overflow
					stacktraceBuilder.AppendLine(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptException}"));
					stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
					stacktraceBuilder.Append(e.Process.SelectedThread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}")));
				}
			} else {
				stacktraceBuilder.AppendLine(e.ExceptionThrown.ToString());
				stacktraceBuilder.Append(e.Process.SelectedThread.GetStackTrace(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.Symbols}"), StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.LineFormat.NoSymbols}")));
			}
			
			string title = e.ExceptionThrown.IsUnhandled ? StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Unhandled}") : StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Title.Handled}");
			string message = string.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Message}"), e.ExceptionThrown.Type);
			Bitmap icon = WinFormsResourceService.GetBitmap(e.ExceptionThrown.IsUnhandled ? "Icons.32x32.Error" : "Icons.32x32.Warning");
			
			DebuggeeExceptionForm.Show(debuggedProcess, title, message, stacktraceBuilder.ToString(), icon, e.ExceptionThrown.IsUnhandled, e.ExceptionThrown);
		}
		
		public bool BreakAndInterceptHandledException(Debugger.Exception exception)
		{
			if (!debuggedProcess.SelectedThread.InterceptException(exception)) {
				MessageService.ShowError("${res:MainWindow.Windows.Debug.ExceptionForm.Error.CannotInterceptHandledException}");
				return false;
			}
			JumpToCurrentLine();
			return true;
		}
		
		public void JumpToCurrentLine()
		{
			if (debuggedProcess == null || debuggedProcess.SelectedThread == null)
				return;
			
			WorkbenchSingleton.MainWindow.Activate();
			
			if (debuggedProcess.IsSelectedFrameForced()) {
				if (debuggedProcess.SelectedStackFrame != null && debuggedProcess.SelectedStackFrame.HasSymbols) {
					JumpToSourceCode();
				} else {
					JumpToDecompiledCode(debuggedProcess.SelectedStackFrame);
				}
			} else {
				var frame = debuggedProcess.SelectedThread.MostRecentStackFrame;
				// other pause reasons
				if (frame != null && frame.HasSymbols) {
					JumpToSourceCode();
				} else {
					// use most recent stack frame because we don't have the symbols
					JumpToDecompiledCode(debuggedProcess.SelectedThread.MostRecentStackFrame);
				}
			}
		}

		void JumpToSourceCode()
		{
			if (debuggedProcess == null || debuggedProcess.SelectedThread == null)
				return;
			
			SourcecodeSegment nextStatement = debuggedProcess.NextStatement;
			if (nextStatement != null) {
				DebuggerService.RemoveCurrentLineMarker();
				DebuggerService.JumpToCurrentLine(nextStatement.Filename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);
			}
		}
		
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
			if (!debuggedProcess.Options.DecompileCodeWithoutSymbols) {
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
			} else {
				// no line => do decompilation
				NavigationService.NavigateTo(debugType.DebugModule.FullPath,
				                             debugType.FullNameWithoutGenericArguments,
				                             IDStringProvider.GetIDString(frame.MethodInfo));
				
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
	}
}
