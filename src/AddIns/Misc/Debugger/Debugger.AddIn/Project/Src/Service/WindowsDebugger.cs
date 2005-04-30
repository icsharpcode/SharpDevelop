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

//using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Services
{	
	public class WindowsDebugger:IDebugger //, IService
	{
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
			InitializeService();
		}
		
		#region ICSharpCode.Core.Services.IService interface implementation
		public event System.EventHandler Initialize;

		public event System.EventHandler Unload;
		
		public void InitializeService()
		{
			NDebugger.DebuggerTraceMessage    += new MessageEventHandler(DebuggerTraceMessage);
			NDebugger.LogMessage              += new MessageEventHandler(LogMessage);
			NDebugger.DebuggingStarted        += new DebuggerEventHandler(DebuggingStarted);
			NDebugger.DebuggingPaused         += new DebuggingPausedEventHandler(DebuggingPaused);
			NDebugger.DebuggingResumed        += new DebuggerEventHandler(DebuggingResumed);
			NDebugger.DebuggingStopped        += new DebuggerEventHandler(DebuggingStopped);
			NDebugger.IsProcessRunningChanged += new DebuggerEventHandler(DebuggerStateChanged);
			NDebugger.Instance.BreakpointStateChanged += new DebuggerLibrary.BreakpointEventHandler(RestoreSharpdevelopBreakpoint);

			DebuggerService.BreakPointAdded   += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointRemoved += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointChanged += new EventHandler(RestoreNDebuggerBreakpoints);
			
			if (Initialize != null) {
				Initialize(this, null);  
			}
		}

		public void UnloadService()
		{
			NDebugger.DebuggerTraceMessage   -= new MessageEventHandler(DebuggerTraceMessage);
			NDebugger.LogMessage             -= new MessageEventHandler(LogMessage);
			NDebugger.DebuggingStarted       -= new DebuggerEventHandler(DebuggingStarted);
			NDebugger.DebuggingPaused        -= new DebuggingPausedEventHandler(DebuggingPaused);
			NDebugger.IsProcessRunningChanged -= new DebuggerEventHandler(DebuggerStateChanged);
			NDebugger.Instance.BreakpointStateChanged -= new DebuggerLibrary.BreakpointEventHandler(RestoreSharpdevelopBreakpoint);

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
				return NDebugger.IsDebugging; 
			} 
		}
		
		public bool IsProcessRunning { 
			get { 
				return NDebugger.IsProcessRunning; 
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
			NDebugger.StartWithoutDebugging(psi);
		}
		
		public void Start(string fileName, string workingDirectory, string arguments)
		{
			NDebugger.Start(fileName, workingDirectory, arguments);
		}
		
		public void Stop()
		{
			NDebugger.Terminate();
		}
		
		public void Break()
		{
			NDebugger.Break();
		}
		
		public void StepInto()
		{
			NDebugger.StepInto();
		}
		
		public void StepOver()
		{
			NDebugger.StepOver();
		}
		
		public void StepOut()
		{
			NDebugger.StepOut();
		}
		
		public void Continue()
		{
			NDebugger.Continue();
		}
		#endregion
		

		
		public void RestoreNDebuggerBreakpoints(object sender, EventArgs e)
		{
			NDebugger.Instance.ClearBreakpoints();
			foreach (ICSharpCode.Core.Breakpoint b in DebuggerService.Breakpoints) {
				DebuggerLibrary.Breakpoint newBreakpoint = new DebuggerLibrary.Breakpoint(b.FileName, b.LineNumber, 0, b.IsEnabled); 
				newBreakpoint.Tag = b;
				b.Tag = newBreakpoint;
				NDebugger.Instance.AddBreakpoint(newBreakpoint); 
			}
		}

		public void RestoreSharpdevelopBreakpoint(object sender, BreakpointEventArgs e)
		{
			ICSharpCode.Core.Breakpoint sdBreakpoint = e.Breakpoint.Tag as ICSharpCode.Core.Breakpoint;
			if (sdBreakpoint != null) {
				sdBreakpoint.IsEnabled  = e.Breakpoint.Enabled;
				sdBreakpoint.FileName   = e.Breakpoint.SourcecodeSegment.SourceFullFilename;
				sdBreakpoint.LineNumber = e.Breakpoint.SourcecodeSegment.StartLine;
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
				exceptionHistory.Add(NDebugger.CurrentThread.CurrentException);
				if (NDebugger.CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED && (NDebugger.CatchHandledExceptions == false)) {
					// Ignore the exception
					Continue();
					return;
				}
				
				
				//MessageBox.Show("Exception was thrown in debugee:\n" + NDebugger.CurrentThread.CurrentException.ToString());
				ExceptionForm form = new ExceptionForm();
				form.label.Text = "Exception " + 
				                  NDebugger.CurrentThread.CurrentException.Type +
                                  " was thrown in debugee:\n" +
				                  NDebugger.CurrentThread.CurrentException.Message;
				form.pictureBox.Image = ResourceService.GetBitmap((NDebugger.CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED)?"Icons.32x32.Warning":"Icons.32x32.Error");
				form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				switch (form.result) {
					case ExceptionForm.Result.Break: 
						break;
					case ExceptionForm.Result.Continue:
						Continue();
						return;
					case ExceptionForm.Result.Ignore:
						System.Diagnostics.Debug.Fail("Not implemented");
						Continue();
						return;
				}
			}
			
			try {
				SelectThread(NDebugger.CurrentThread);
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
		
		public void DebuggerStateChanged(object sender, DebuggerEventArgs e)
		{
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
			VariableCollection collection = NDebugger.LocalVariables;
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
