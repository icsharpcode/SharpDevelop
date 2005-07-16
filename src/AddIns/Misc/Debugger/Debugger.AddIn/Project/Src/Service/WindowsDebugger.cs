// <file>
//     <owner name="David Srbeck" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DebuggerLibrary;

using ICSharpCode.Core;
//using ICSharpCode.Core.Services;
//using ICSharpCode.Core.AddIns;

//using ICSharpCode.Core.Properties;
//using ICSharpCode.Core.AddIns.Codons;
//using ICSharpCode.Core.AddIns.Conditions;
using System.CodeDom.Compiler;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Gui.Components;
//using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Internal.Project;
//using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Services;
using System.Runtime.Remoting;
using System.Reflection;
using System.Security.Policy;

//using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Services
{	
	public class WindowsDebugger:IDebugger //, IService
	{
		[Serializable]
		public class RemotingConfigurationHelpper
		{
			public string path;

			public RemotingConfigurationHelpper(string path)
			{
				this.path = path;
			}

			public void Configure()
			{
				AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
				
				RemotingConfiguration.Configure(Path.Combine(path, "Client.config"));

				string baseDir = Directory.GetDirectoryRoot(AppDomain.CurrentDomain.BaseDirectory);
				string relDirs = AppDomain.CurrentDomain.BaseDirectory + ";" + path;
				AppDomain serverAppDomain = AppDomain.CreateDomain("Debugging server",
				                                                   new Evidence(AppDomain.CurrentDomain.Evidence),
																   baseDir,
																   relDirs,
																   AppDomain.CurrentDomain.ShadowCopyFiles);
				serverAppDomain.DoCallBack(new CrossAppDomainDelegate(ConfigureServer));
			}

			private void ConfigureServer()
			{
				AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
				RemotingConfiguration.Configure(Path.Combine(path, "Server.config"));
			}

			Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					string fullFilename = assembly.Location;
					if (Path.GetFileNameWithoutExtension(fullFilename).ToLower() == args.Name.ToLower() ||
						assembly.FullName == args.Name) {
						return assembly;
					}
				}
				return null;
			}
		}

		bool useRemotingForThreadInterop = false;

		NDebugger debugger;
		bool isDebuggingCache;
		bool isProcessRunningCache;

		public event EventHandler DebugStopped; // FIX: unused

		List<DebuggerLibrary.Exception> exceptionHistory = new List<DebuggerLibrary.Exception>();
		
		protected virtual void OnDebugStopped(EventArgs e)
		{
			if (DebugStopped != null) {
				DebugStopped(this, e);
			}
		}
		
		public void Dispose()// FIX: unused
		{
			
		}
		
		public NDebugger DebuggerCore {
			get {
				return debugger;
			}
		}
		
		MessageViewCategory messageViewCategoryDebug;
		MessageViewCategory messageViewCategoryDebuggerLog;
		public Thread selectedThread;
		public Function selectedFunction;
		
		public bool CanDebug(IProject project)
		{
			return true;
		}
		
		public bool SupportsStepping {
			get {
				return true;
			}
		}

		public IList<DebuggerLibrary.Exception> ExceptionHistory {
			get {
				return exceptionHistory.AsReadOnly();
			}
		}
		
		public WindowsDebugger()
		{
			if (useRemotingForThreadInterop) {
				// This needs to be called before instance of NDebugger is created
				string path = null;
				foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					string fullFilename = assembly.Location;
					if (Path.GetFileName(fullFilename).ToLower() == "debugger.core.dll") {
						path = Path.GetDirectoryName(fullFilename);
						break;
					}
				}

				if (path == null) {
					throw new System.Exception("Debugger.Core.dll is not loaded");
				}
				new RemotingConfigurationHelpper(path).Configure();
			}

			debugger = new NDebugger();

			InitializeService();
		}
		
		#region ICSharpCode.Core.Services.IService interface implementation
		public event System.EventHandler Initialize;

		public event System.EventHandler Unload;
		
		public void InitializeService()
		{
			debugger.DebuggerTraceMessage    += new MessageEventHandler(DebuggerTraceMessage);
			debugger.LogMessage              += new MessageEventHandler(LogMessage);
			debugger.DebuggingStarted        += new DebuggerEventHandler(DebuggingStarted);
			debugger.DebuggingPaused         += new DebuggingPausedEventHandler(DebuggingPaused);
			debugger.DebuggingResumed        += new DebuggerEventHandler(DebuggingResumed);
			debugger.DebuggingStopped        += new DebuggerEventHandler(DebuggingStopped);
			debugger.IsDebuggingChanged      += new DebuggerEventHandler(OnIsDebuggingChanged);
			debugger.IsProcessRunningChanged += new DebuggerEventHandler(DebuggerStateChanged);
			debugger.BreakpointStateChanged  += new DebuggerLibrary.BreakpointEventHandler(RestoreSharpdevelopBreakpoint);

			DebuggerService.BreakPointAdded   += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointRemoved += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointChanged += new EventHandler(RestoreNDebuggerBreakpoints);

			isDebuggingCache = debugger.IsDebugging;
			isProcessRunningCache = debugger.IsProcessRunning;
			
			if (Initialize != null) {
				Initialize(this, null);  
			}
		}

		public void UnloadService()
		{
			debugger.DebuggerTraceMessage    -= new MessageEventHandler(DebuggerTraceMessage);
			debugger.LogMessage              -= new MessageEventHandler(LogMessage);
			debugger.DebuggingStarted        -= new DebuggerEventHandler(DebuggingStarted);
			debugger.DebuggingPaused         -= new DebuggingPausedEventHandler(DebuggingPaused);
			debugger.DebuggingResumed        -= new DebuggerEventHandler(DebuggingResumed);
			debugger.DebuggingStopped        -= new DebuggerEventHandler(DebuggingStopped);
			debugger.IsDebuggingChanged      -= new DebuggerEventHandler(OnIsDebuggingChanged);
			debugger.IsProcessRunningChanged -= new DebuggerEventHandler(DebuggerStateChanged);
			debugger.BreakpointStateChanged  -= new DebuggerLibrary.BreakpointEventHandler(RestoreSharpdevelopBreakpoint);

			DebuggerService.BreakPointAdded   -= new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointRemoved -= new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointChanged -= new EventHandler(RestoreNDebuggerBreakpoints);
			
			if (Unload != null) {
				Unload(this, null);	
			}
		}
		#endregion		
		
		#region ICSharpCode.SharpDevelop.Services.IDebugger interface implementation
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
		
		public bool SupportsStartStop { 
			get { 
				return true; 
			} 
		}
		
		public bool SupportsExecutionControl { 
			get { 
				return true; 
			} 
		}
		
		public void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{
			debugger.StartWithoutDebugging(psi);
		}
		
		public void Start(string fileName, string workingDirectory, string arguments)
		{
			debugger.Start(fileName, workingDirectory, arguments);
		}
		
		public void Stop()
		{
			debugger.Terminate();
		}
		
		public void Break()
		{
			debugger.Break();
		}
		
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
		
		public void Continue()
		{
			debugger.Continue();
		}
		#endregion
		

		
		public void RestoreNDebuggerBreakpoints(object sender, EventArgs e)
		{
			debugger.ClearBreakpoints();
			foreach (ICSharpCode.Core.Breakpoint b in DebuggerService.Breakpoints) {
				DebuggerLibrary.Breakpoint newBreakpoint = debugger.AddBreakpoint(b.FileName, b.LineNumber, 0, b.IsEnabled);
				b.Tag = newBreakpoint;
			}
		}

		public void RestoreSharpdevelopBreakpoint(object sender, BreakpointEventArgs e)
		{
			foreach (ICSharpCode.Core.Breakpoint sdBreakpoint in DebuggerService.Breakpoints) {
				if (sdBreakpoint.Tag == e.Breakpoint) {
					if (sdBreakpoint != null) {
						sdBreakpoint.IsEnabled  = e.Breakpoint.Enabled;
						sdBreakpoint.FileName   = e.Breakpoint.SourcecodeSegment.SourceFullFilename;
						sdBreakpoint.LineNumber = e.Breakpoint.SourcecodeSegment.StartLine;
					}
				}
			}
		}
		
		// Output messages that report status of debugger
		void DebuggerTraceMessage(object sender, MessageEventArgs e)
		{
			if (messageViewCategoryDebuggerLog != null) {
				messageViewCategoryDebuggerLog.AppendText(e.Message + "\n");
				System.Console.WriteLine(e.Message);
			}
		}
		
		// Output messages form debuged program that are caused by System.Diagnostics.Trace.WriteLine(), etc...
		void LogMessage(object sender, MessageEventArgs e)
		{
			DebuggerTraceMessage(this, e);
			if (messageViewCategoryDebug != null) {
				messageViewCategoryDebug.AppendText(e.Message + "\n");
			}
		}
		
		void DebuggingStarted(object sender, DebuggerEventArgs e)
		{
			// Initialize 
			/*PadDescriptor cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
			if (messageViewCategoryDebug == null) {	
				messageViewCategoryDebug = cmv.GetCategory("Debug");
			}
			messageViewCategoryDebug.ClearText();
			if (messageViewCategoryDebuggerLog == null) {	
				messageViewCategoryDebuggerLog = new MessageViewCategory("DebuggerLog", "Debugger log");
				//cmv.AddCategory(messageViewCategoryDebuggerLog);
			}
			messageViewCategoryDebuggerLog.ClearText();*/
		}

		void DebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			if (e.Reason == PausedReason.Exception) {
				exceptionHistory.Add(debugger.CurrentThread.CurrentException);
				if (debugger.CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED && (debugger.CatchHandledExceptions == false)) {
					// Ignore the exception
					e.ResumeDebuggingAfterEvent();
					return;
				}
				
				
				//MessageBox.Show("Exception was thrown in debugee:\n" + debugger.CurrentThread.CurrentException.ToString());
				ExceptionForm form = new ExceptionForm();
				form.label.Text = "Exception " + 
				                  debugger.CurrentThread.CurrentException.Type +
                                  " was thrown in debugee:\n" +
				                  debugger.CurrentThread.CurrentException.Message;
				form.pictureBox.Image = ResourceService.GetBitmap((debugger.CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED)?"Icons.32x32.Warning":"Icons.32x32.Error");
				form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				switch (form.result) {
					case ExceptionForm.Result.Break: 
						break;
					case ExceptionForm.Result.Continue:
						e.ResumeDebuggingAfterEvent();
						return;
					case ExceptionForm.Result.Ignore:
						System.Diagnostics.Debug.Fail("Not implemented");
						e.ResumeDebuggingAfterEvent();
						return;
				}
			}
			
			try {
				SelectThread(debugger.CurrentThread);
			} catch (CurrentThreadNotAviableException) {}
			JumpToCurrentLine();
		}
		
		void DebuggingResumed(object sender, DebuggerEventArgs e)
		{
			selectedThread = null;
			selectedFunction = null;
			DebuggerService.RemoveCurrentLineMarker();
		}
		
		void DebuggingStopped(object sender, DebuggerEventArgs e)
		{
			exceptionHistory.Clear();
			//DebuggerService.Stop();//TODO: delete
		}
		
		public void SelectThread(Thread thread) 
		{
			selectedThread = thread;
			try {
				selectedFunction = thread.CurrentFunction;
				// Prefer first function on callstack that has symbols (source code)
				if (selectedFunction.Module.SymbolsLoaded == false) {
					foreach (Function f in thread.Callstack) {
						if (f.Module.SymbolsLoaded) {
							selectedFunction = f;
							break;
						}
					}
				}
			} catch (CurrentFunctionNotAviableException) {}
		}
		


		public void JumpToCurrentLine()
		{
			//StatusBarService.SetMessage("Source code not aviable!");
			try {
				if (selectedFunction == null) {
					return;
				}
				SourcecodeSegment nextStatement = selectedFunction.NextStatement;
				DebuggerService.JumpToCurrentLine(nextStatement.SourceFullFilename, nextStatement.StartLine, nextStatement.StartColumn, nextStatement.EndLine, nextStatement.EndColumn);

				string stepRanges = "";
				foreach (int i in nextStatement.StepRanges) {
					stepRanges += i.ToString("X") + " ";
				}
				//StatusBarService.SetMessage("IL:" + nextStatement.ILOffset.ToString("X") + " StepRange:" + stepRanges + "    ");
			} catch (NextStatementNotAviableException) {
				System.Diagnostics.Debug.Fail("Source code not aviable!");
			}
		}

		void OnIsDebuggingChanged(object sender, DebuggerEventArgs e)
		{
			isDebuggingCache = debugger.IsDebugging;
			isProcessRunningCache = debugger.IsProcessRunning;
		}
		
		public void DebuggerStateChanged(object sender, DebuggerEventArgs e)
		{
			isDebuggingCache = debugger.IsDebugging;
			isProcessRunningCache = debugger.IsProcessRunning;
			UpdateToolbars();
		}
		
		void UpdateToolbars() 
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).Update();
			//if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
			//	  WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.RedrawContent();
			//}
		}
		
		/// <summary>
		/// Gets the current value of the variable as string that can be displayed in tooltips.
		/// </summary>
		public string GetValueAsString(string variableName)
		{
			if (!debugger.IsDebugging || debugger.IsProcessRunning) return null;
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
	}
}
