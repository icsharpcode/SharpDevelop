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
	public class WindowsDebugger:NDebugger, IDebugger //, IService
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
			DebuggerService.BreakPointAdded   += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointRemoved += new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointChanged += new EventHandler(RestoreNDebuggerBreakpoints);
			
			if (Initialize != null) {
				Initialize(this, null);  
			}
		}

		public void UnloadService()
		{
			DebuggerService.BreakPointAdded   -= new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointRemoved -= new EventHandler(RestoreNDebuggerBreakpoints);
			DebuggerService.BreakPointChanged -= new EventHandler(RestoreNDebuggerBreakpoints);
			
			if (Unload != null) {
				Unload(this, null);	
			}
		}
		#endregion		
		
		#region ICSharpCode.SharpDevelop.Services.IDebugger interface implementation
		
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
		
		public void Stop()
		{
			this.Terminate();
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

		protected override void OnBreakpointStateChanged(object sender, BreakpointEventArgs e)
		{
			RestoreSharpdevelopBreakpoint(e.Breakpoint);
		}

		public void RestoreSharpdevelopBreakpoint(DebuggerLibrary.Breakpoint breakpoint)
		{
			ICSharpCode.Core.Breakpoint sdBreakpoint = breakpoint.Tag as ICSharpCode.Core.Breakpoint;
			if (sdBreakpoint != null) {
				sdBreakpoint.IsEnabled  = breakpoint.Enabled;
				sdBreakpoint.FileName   = breakpoint.SourcecodeSegment.SourceFullFilename;
				sdBreakpoint.LineNumber = breakpoint.SourcecodeSegment.StartLine;
			}
		}
		
		// Output messages that report status of debugger
		protected override void OnDebuggerTraceMessage(string message)
		{
			base.OnDebuggerTraceMessage(message);
			if (messageViewCategoryDebuggerLog != null) {
				messageViewCategoryDebuggerLog.AppendText(message + "\n");
				System.Console.WriteLine(message);
			}
		}

		// Output messages form debuged program that are caused by System.Diagnostics.Trace.WriteLine(), etc...
		protected override void OnLogMessage(string message)
		{
			base.OnLogMessage(message);
			OnDebuggerTraceMessage(message);
			if (messageViewCategoryDebug != null) {
				messageViewCategoryDebug.AppendText(message + "\n");
			}
		}
		
		protected override void OnDebuggingStarted()
		{
			base.OnDebuggingStarted();
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

		protected override void OnDebuggingPaused(PausedReason reason)
		{
			base.OnDebuggingPaused(reason);
			if (reason == PausedReason.Exception) {
				exceptionHistory.Add(CurrentThread.CurrentException);
				if (CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED && (CatchHandledExceptions == false)) {
					// Ignore the exception
					Continue();
					return;
				}
				
				
				//MessageBox.Show("Exception was thrown in debugee:\n" + NDebugger.CurrentThread.CurrentException.ToString());
				ExceptionForm form = new ExceptionForm();
				form.label.Text = "Exception " + 
				                  CurrentThread.CurrentException.Type +
                                  " was thrown in debugee:\n" +
				                  CurrentThread.CurrentException.Message;
				form.pictureBox.Image = ResourceService.GetBitmap((CurrentThread.CurrentException.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED)?"Icons.32x32.Warning":"Icons.32x32.Error");
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
				SelectThread(CurrentThread);
			} catch (CurrentThreadNotAviableException) {}
			JumpToCurrentLine();
		}
		
		protected override void OnDebuggingResumed()
		{
			base.OnDebuggingResumed();
			selectedThread = null;
			selectedFunction = null;
			DebuggerService.RemoveCurrentLineMarker();
		}
		
		protected override void OnDebuggingStopped()
		{
			base.OnDebuggingStopped();
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
		
		protected override void OnIsProcessRunningChanged()
		{
			base.OnIsProcessRunningChanged();
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
			if (!IsDebugging || IsProcessRunning) return null;
			VariableCollection collection = LocalVariables;
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
