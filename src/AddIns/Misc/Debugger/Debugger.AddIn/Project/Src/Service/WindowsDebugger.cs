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
		
		class BreakpointMarker: TextMarker
		{			
			public BreakpointMarker(int offset, int length, TextMarkerType textMarkerType, Color color, Color fgColor) : base(offset, length, textMarkerType, color, fgColor)
			{
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
			
			WorkbenchSingleton.WorkbenchCreated += new EventHandler(WorkspaceCreated);

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
		
		void WorkspaceCreated(object sender, EventArgs args)
		{
			WorkbenchSingleton.Workbench.ViewOpened += new ViewContentEventHandler(ViewContentOpened);
			WorkbenchSingleton.Workbench.ViewClosed += new ViewContentEventHandler(ViewContentClosed);
		}
		
		void ViewContentOpened(object sender, ViewContentEventArgs e)
		{
			if (e.Content.Control is TextEditor.TextEditorControl) {
				TextArea textArea = ((TextEditor.TextEditorControl)e.Content.Control).ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown += new MarginMouseEventHandler(IconBarMouseDown);
				textArea.IconBarMargin.Painted   += new MarginPaintEventHandler(PaintIconBar);
				textArea.MouseMove               += new MouseEventHandler(TextAreaMouseMove);
				
				RefreshBreakpointMarkersInEditor(textArea.MotherTextEditorControl);
			}
		}
		
		void ViewContentClosed(object sender, ViewContentEventArgs e)
		{
			if (e.Content.Control is TextEditor.TextEditorControl) {
				TextArea textArea = ((TextEditor.TextEditorControl)e.Content.Control).ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown -= new MarginMouseEventHandler(IconBarMouseDown);
				textArea.IconBarMargin.Painted   -= new MarginPaintEventHandler(PaintIconBar);
				textArea.MouseMove               -= new MouseEventHandler(TextAreaMouseMove);
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
			RemoveCurrentLineMarker();
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
		
		TextMarker currentLineMarker;
		IDocument  currentLineMarkerParent;
		
		void RemoveCurrentLineMarker()
		{
			if (currentLineMarker != null) {
				currentLineMarkerParent.MarkerStrategy.TextMarker.Remove(currentLineMarker);
				currentLineMarkerParent.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				currentLineMarkerParent.CommitUpdate();
				currentLineMarkerParent = null;
				currentLineMarker       = null;
			}
		}

		public void JumpToCurrentLine()
		{
			RemoveCurrentLineMarker();
			
            
			
			try {
				if (selectedFunction == null) {
					return;
				}
				SourcecodeSegment nextStatement = selectedFunction.NextStatement;
				
				FileService.OpenFile(nextStatement.SourceFullFilename);
				IWorkbenchWindow window = FileService.GetOpenFile(nextStatement.SourceFullFilename);
				if (window != null) {
					IViewContent content = window.ViewContent;
				
					if (content is IPositionable) {
						((IPositionable)content).JumpTo((int)nextStatement.StartLine - 1, (int)nextStatement.StartColumn - 1);
					}
					
					if (content.Control is TextEditorControl) {
						IDocument document = ((TextEditorControl)content.Control).Document;
						LineSegment line = document.GetLineSegment((int)nextStatement.StartLine - 1);
						int offset = line.Offset + (int)nextStatement.StartColumn;
						currentLineMarker = new TextMarker(offset, (int)nextStatement.EndColumn - (int)nextStatement.StartColumn, TextMarkerType.SolidBlock, Color.Yellow);
						currentLineMarkerParent = document;
						currentLineMarkerParent.MarkerStrategy.TextMarker.Add(currentLineMarker);
						document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
						document.CommitUpdate();
					}
				}
			} catch (NextStatementNotAviableException) {
				//System.Windows.Forms.MessageBox.Show("Source code not aviable!");
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
		
		void IconBarMouseDown(AbstractMargin iconBar, Point mousepos, MouseButtons mouseButtons)
		{
			Rectangle viewRect = iconBar.TextArea.TextView.DrawingPosition;
			Point logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				NDebugger.Instance.ToggleBreakpointAt(iconBar.TextArea.MotherTextEditorControl.FileName , logicPos.Y + 1, 0);
				RefreshBreakpointMarkersInEditor(iconBar.TextArea.MotherTextEditorControl);
				iconBar.TextArea.Refresh(iconBar);
			}
		}
		
				
		public void RefreshBreakpointMarkersInEditor(TextEditorControl textEditor) 
		{
			IDocument document = textEditor.Document;
			System.Collections.Generic.List<ICSharpCode.TextEditor.Document.TextMarker> markers = textEditor.Document.MarkerStrategy.TextMarker;
			// Remove all breakpoint markers
			for (int i = 0; i < markers.Count;) {
				if (markers[i] is BreakpointMarker) {
					markers.RemoveAt(i);
				} else {
					i++; // Check next one
				}
			}
			// Add breakpoint markers
			foreach (DebuggerLibrary.Breakpoint b in NDebugger.Instance.Breakpoints) {
				if (b.SourcecodeSegment.SourceFullFilename.ToLower() == textEditor.FileName.ToLower()) {
					LineSegment lineSeg = document.GetLineSegment((int)b.SourcecodeSegment.StartLine - 1);
					document.MarkerStrategy.TextMarker.Add(new BreakpointMarker(lineSeg.Offset, lineSeg.Length , TextMarkerType.SolidBlock, Color.Red, Color.White));
				}
			}
			// Perform editor update
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			document.CommitUpdate();
		}
		
		
		TextMarker variableMarker;
		IDocument  variableMarkerParent;
		
		/// <summary>
		/// This function shows variable values as tooltips
		/// </summary>
		void TextAreaMouseMove(object sender, MouseEventArgs args)
		{			
			if (!IsDebugging) return;
			if (IsProcessRunning) return;
			
			TextArea textArea = (TextArea)sender;
			
			Point mousepos = textArea.PointToClient(Control.MousePosition);
			Rectangle viewRect = textArea.TextView.DrawingPosition;
			if (viewRect.Contains(mousepos)) {
				Point logicPos = textArea.TextView.GetLogicalPosition(mousepos.X - viewRect.Left,
				                                                      mousepos.Y - viewRect.Top);
				if (logicPos.Y >= 0 && logicPos.Y < textArea.Document.TotalNumberOfLines) {
					IDocument doc = textArea.Document;
					LineSegment seg = doc.GetLineSegment(logicPos.Y);
					string line = doc.GetText(seg.Offset, seg.Length);
					int startIndex = 0;
					int length = 0;
					string expresion = String.Empty;
					for(int index = 0; index < seg.Length; index++) {
						char chr = line[index];
						if ((Char.IsLetterOrDigit(chr) || chr == '_' || chr == '.') == false || // invalid character
						    (chr == '.' && logicPos.X <= index)) { // Start of sub-expresion at the right side of cursor
							// End of expresion...
							if ((startIndex <= logicPos.X && logicPos.X <= index) && // Correct position
							    (startIndex != index)) { // Actualy something
							    length = index - startIndex;
								expresion = line.Substring(startIndex, length);
								break;
							} else {
								// Let's try next one...
								startIndex = index + 1;
							}
						}
					}
					//Console.WriteLine("MouseMove@" + logicPos + ":" + expresion);
					if (variableMarker == null || variableMarker.Offset != (seg.Offset + startIndex) || variableMarker.Length != length) {
						// Needs update
						if (variableMarker != null) {
							// Remove old marker
							variableMarkerParent.MarkerStrategy.TextMarker.Remove(variableMarker);
							variableMarkerParent.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
							variableMarkerParent.CommitUpdate();
							variableMarkerParent = null;
							variableMarker       = null;
						}
						if (expresion != String.Empty) {
							// Look if it is variable
							try {
								string value;
								value = selectedThread.LocalVariables[expresion].Value.ToString();
								variableMarker = new TextMarker(seg.Offset + startIndex, length, TextMarkerType.Underlined, Color.Blue); 
								variableMarker.ToolTip = value;
								variableMarkerParent = doc;
								variableMarkerParent.MarkerStrategy.TextMarker.Add(variableMarker);
								variableMarkerParent.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
								variableMarkerParent.CommitUpdate();
							} catch {}
						}
					}
				}
			}
		}

		/// <summary>
		/// Draw Breakpoint icon and the yellow arrow in the margin
		/// </summary>
		void PaintIconBar(AbstractMargin iconBar, Graphics g, Rectangle rect)
		{
			foreach (DebuggerLibrary.Breakpoint breakpoint in NDebugger.Instance.Breakpoints) {
				if (Path.GetFullPath(breakpoint.SourcecodeSegment.SourceFullFilename) == Path.GetFullPath(iconBar.TextArea.MotherTextEditorControl.FileName)) {
					int lineNumber = iconBar.TextArea.Document.GetVisibleLine((int)breakpoint.SourcecodeSegment.StartLine - 1);
					int yPos = (int)(lineNumber * iconBar.TextArea.TextView.FontHeight) - iconBar.TextArea.VirtualTop.Y;
					if (yPos >= rect.Y && yPos <= rect.Bottom) {
						((IconBarMargin)iconBar).DrawBreakpoint(g, yPos, breakpoint.Enabled);
					}
				}
			}

			if (IsDebugging && !IsProcessRunning && selectedFunction != null) {
				try {
					SourcecodeSegment nextStatement = selectedFunction.NextStatement;//cache
					
					if (Path.GetFullPath(nextStatement.SourceFullFilename).ToLower() == Path.GetFullPath(iconBar.TextArea.MotherTextEditorControl.FileName).ToLower()) {
						int lineNumber = iconBar.TextArea.Document.GetVisibleLine((int)nextStatement.StartLine - 1);
						int yPos = (int)(lineNumber * iconBar.TextArea.TextView.FontHeight) - iconBar.TextArea.VirtualTop.Y;
						if (yPos >= rect.Y && yPos <= rect.Bottom) {
							((IconBarMargin)iconBar).DrawArrow(g, yPos);
						}
					}
				} 
				catch (NextStatementNotAviableException) {}
			}
		}
	}	
}
