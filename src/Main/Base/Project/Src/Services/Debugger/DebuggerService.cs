// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public static class DebuggerService
	{
		static System.Diagnostics.Process standardProcess = null;
		static bool                       isRunning       = false;
		static IDebugger                  defaultDebugger = null;
		static IDebugger                  currentDebugger = null;
		static ArrayList                  debugger        = null;
		//static ArrayList                  breakpoints     = new ArrayList();
		
		/*public static ArrayList Breakpoints {
			get {
				return breakpoints;
			}
		}*/
		
		public static IDebugger CurrentDebugger {
			get {
				if (currentDebugger != null) {
					return currentDebugger;
				}
				if (debugger == null) {
					InitializeService();
				}
				if (debugger != null) {
					IProject project = null;
					if (ProjectService.OpenSolution != null) {
						project = ProjectService.OpenSolution.StartupProject;
					}
					foreach (IDebugger d in debugger) {
						if (d.CanDebug(project)) {
							currentDebugger = d;
							return d;
						}
					}
				}
				if (defaultDebugger == null) {
					defaultDebugger = new DefaultDebugger();
				}
				currentDebugger = defaultDebugger;
				return defaultDebugger;
			}
		}
		
		public static bool IsProcessRuning {
			get {
				if (standardProcess != null) {
					return isRunning;
				}
				if (currentDebugger != null) {
					return currentDebugger.IsProcessRunning;
				}
				return false;
			}
		}
		
		public static bool IsDebugging {
			get {
				if (currentDebugger == null) {
					return false;
				}
				return currentDebugger.IsDebugging;
			}
		}
		
		static DebuggerService()
		{
			InitializeService();
			InitializeService2();
		}
		
		static MessageViewCategory debugCategory = null;
		
		static void EnsureDebugCategory()
		{
			if (debugCategory == null) {
				debugCategory = new MessageViewCategory("Debug", "${res:MainWindow.Windows.OutputWindow.DebugCategory}");
				CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
				compilerMessageView.AddCategory(debugCategory);
			}
		}
		public static void ClearDebugMessages()
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		public static void PrintDebugMessage(string msg)
		{
			try {
				EnsureDebugCategory();
				debugCategory.AppendText(msg);
			} catch (Exception) {}
		}
		
		static string oldLayoutConfiguration = "Default";
		static void HandleDebugStopped(object sender, EventArgs e)
		{
//			LayoutConfiguration.CurrentLayoutName = oldLayoutConfiguration;
			//// Alex: if stopped - kill process which might be running or stuck
			if (standardProcess != null) {
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess = null;
			}
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Stop();
			}
			
			debugger.DebugStopped -= new EventHandler(HandleDebugStopped);
			debugger.Dispose();
			
			isRunning = false;
		}
		
		#region ICSharpCode.Core.IService interface implementation
		public static void InitializeService()
		{
			AddInTreeNode treeNode = null;
			try {
				treeNode = AddInTree.GetTreeNode("/SharpDevelop/Services/DebuggerService/Debugger");
			} catch (Exception) {
			}
			if (treeNode != null) {
				debugger = treeNode.BuildChildItems(null);
			}
			
			ProjectService.SolutionLoaded += new SolutionEventHandler(ClearOnCombineEvent);
		}
		
		static void DebuggerServiceStarted(object sender, EventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
			CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
			compilerMessageView.SelectCategory("Debug");
		}
		
		static void ClearOnCombineEvent(object sender, SolutionEventArgs e)
		{
			EnsureDebugCategory();
			debugCategory.ClearText();
		}
		#endregion
		
		public static void GotoSourceFile(string fileName, int lineNumber, int column)
		{
			
			FileService.JumpToFilePosition(fileName, lineNumber, column);
		}
		
		public static void StartWithoutDebugging(System.Diagnostics.ProcessStartInfo psi)
		{
			if (IsProcessRuning) {
				return;
			}
			try {
				standardProcess = new System.Diagnostics.Process();
				standardProcess.StartInfo = psi;
				standardProcess.Exited += new EventHandler(StandardProcessExited);
				standardProcess.EnableRaisingEvents = true;
				standardProcess.Start();
				isRunning = true;
			} catch (Exception e) {
				MessageService.ShowError(e, "Can't execute " + "\"" + psi.FileName + "\"\n");
			}
		}
		
		public static void StartWithoutDebugging(string fileName, string workingDirectory, string arguments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.UseShellExecute  = false;
			StartWithoutDebugging(startInfo);
		}
		
		public static void Start(string fileName, string workingDirectory, string arguments)
		{
			if (IsProcessRuning) {
				return;
			}
			oldLayoutConfiguration = LayoutConfiguration.CurrentLayoutName;
//			LayoutConfiguration.CurrentLayoutName = "Debug";
			
			IDebugger debugger = CurrentDebugger;
			if (debugger != null) {
				debugger.Start(fileName, workingDirectory, arguments);
				debugger.DebugStopped += new EventHandler(HandleDebugStopped);
			}
			
//			lock (breakpoints) {
//				foreach (Breakpoint breakpoint in breakpoints) {
//					if (breakpoint.Enabled) {
//						brea.AddBreakpoint(fileName, breakpoint.FileName, breakpoint.Line);
//					}
//				}
//			}
			isRunning = true;
		}
		
		public static void Break()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Break();
			}
		}
		
		public static void Continue()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger != null && debugger.SupportsExecutionControl) {
				debugger.Continue();
			}
		}
		
		public static void Step(bool stepInto)
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			if (stepInto) {
				debugger.StepInto();
			} else {
				debugger.StepOver();
			}
		}
		
		public static void StepOut()
		{
			IDebugger debugger = CurrentDebugger;
			if (debugger == null || !debugger.SupportsStepping) {
				return;
			}
			debugger.StepOut();
		}
		
		public static void Stop()
		{
			if (standardProcess != null) {
//				OnTextMessage(new TextMessageEventArgs(String.Format("Killing {0}{1}\n",standardProcess.ProcessName,Environment.NewLine)));
				standardProcess.Exited -= new EventHandler(StandardProcessExited);
				standardProcess.Kill();
				standardProcess.Close();
				standardProcess.Dispose();
				standardProcess = null;
			} else {
				IDebugger debugger = CurrentDebugger;
				if (debugger != null) {
					debugger.Stop();
				}
			}
			isRunning = false;
		}
		
		static void StandardProcessExited(object sender, EventArgs e)
		{
			standardProcess.Exited -= new EventHandler(StandardProcessExited);
			standardProcess.Dispose();
			standardProcess = null;
			isRunning       = false;
		}
		
//		protected override void OnException(ExceptionEventArgs e)
//		{
//			base.OnException(e);
//			OnTextMessage(new TextMessageEventArgs("Got Exception\n"));
//			StopDebugger();
//		}
		//
//		protected override void OnProcessExited(ProcessEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("The program '[{1}] {0}' exited with code {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Process.ID,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnProcessExited(e);
//		}
//		protected override void OnModuleLoaded(ModuleEventArgs e)
//		{
//			OnTextMessage(new TextMessageEventArgs(String.Format("'{0}' : '{1}' loaded, {2}.{3}\n",
//			                                                 "Unknown",
//			                                                 e.Module.Name,
//			                                                 "Unknown",Environment.NewLine)));
//			base.OnModuleLoaded(e);
//		}
		
		public static event EventHandler BreakPointChanged;
		public static event EventHandler BreakPointAdded;
		public static event EventHandler BreakPointRemoved;
		
		static void OnBreakPointChanged(EventArgs e)
		{
			if (BreakPointChanged != null) {
				BreakPointChanged(null, e);
			}
		}
		
		static void OnBreakPointAdded(EventArgs e)
		{
			if (BreakPointAdded != null) {
				BreakPointAdded(null, e);
			}
		}
		
		static void OnBreakPointRemoved(EventArgs e)
		{
			if (BreakPointRemoved != null) {
				BreakPointRemoved(null, e);
			}
		}
		
		
		static List<Breakpoint> breakpoints = new List<Breakpoint>();
		
		public static IList<Breakpoint> Breakpoints {
			get {
				return breakpoints;
			}
		}
		
		public static void ToggleBreakpointAt(string fileName, int line, int column)
		{
			foreach(Breakpoint b in breakpoints) {
				if (b.FileName == fileName && b.LineNumber == line) {
					breakpoints.Remove(b);
					OnBreakPointRemoved(EventArgs.Empty);
					return;
				}
			}
			breakpoints.Add(new Breakpoint(fileName, line));
			OnBreakPointAdded(EventArgs.Empty);
		}
		
		
		
		
		
		
		
		class BreakpointMarker: TextMarker
		{
			public BreakpointMarker(int offset, int length, TextMarkerType textMarkerType, Color color, Color foreColor):base(offset, length, textMarkerType, color, foreColor)
			{
			}
		}
		
		class CurrentLineMarker: TextMarker
		{
			public CurrentLineMarker(int offset, int length, TextMarkerType textMarkerType, Color color, Color foreColor):base(offset, length, textMarkerType, color, foreColor)
			{
			}
		}
		
		public static void InitializeService2()
		{
			WorkbenchSingleton.WorkbenchCreated += new EventHandler(WorkspaceCreated);
		}
		
		static void WorkspaceCreated(object sender, EventArgs args)
		{
			WorkbenchSingleton.Workbench.ViewOpened += new ViewContentEventHandler(ViewContentOpened);
			WorkbenchSingleton.Workbench.ViewClosed += new ViewContentEventHandler(ViewContentClosed);
		}
		
		static void ViewContentOpened(object sender, ViewContentEventArgs e)
		{
			if (e.Content.Control is TextEditor.TextEditorControl) {
				TextArea textArea = ((TextEditor.TextEditorControl)e.Content.Control).ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown += new MarginMouseEventHandler(IconBarMouseDown);
				textArea.IconBarMargin.Painted   += new MarginPaintEventHandler(PaintIconBar);
				textArea.MouseMove               += new MouseEventHandler(TextAreaMouseMove);
				
				RefreshBreakpointMarkersInEditor(textArea.MotherTextEditorControl);
			}
		}
		
		static void ViewContentClosed(object sender, ViewContentEventArgs e)
		{
			if (e.Content.Control is TextEditor.TextEditorControl) {
				TextArea textArea = ((TextEditor.TextEditorControl)e.Content.Control).ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown -= new MarginMouseEventHandler(IconBarMouseDown);
				textArea.IconBarMargin.Painted   -= new MarginPaintEventHandler(PaintIconBar);
				textArea.MouseMove               -= new MouseEventHandler(TextAreaMouseMove);
			}
		}
		
		
		static TextMarker currentLineMarker;
		static IDocument  currentLineMarkerParent;
		
		static public void RemoveCurrentLineMarker()
		{
			if (currentLineMarker != null) {
				currentLineMarkerParent.MarkerStrategy.TextMarker.Remove(currentLineMarker);
				currentLineMarkerParent.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				currentLineMarkerParent.CommitUpdate();
				currentLineMarkerParent = null;
				currentLineMarker       = null;
			}
		}
		
		static public void JumpToCurrentLine(string SourceFullFilename, int StartLine, int StartColumn, int EndLine, int EndColumn)
		{
			RemoveCurrentLineMarker();
			
			FileService.OpenFile(SourceFullFilename);
			IWorkbenchWindow window = FileService.GetOpenFile(SourceFullFilename);
			if (window != null) {
				IViewContent content = window.ViewContent;
				
				if (content is IPositionable) {
					((IPositionable)content).JumpTo((int)StartLine - 1, (int)StartColumn - 1);
				}
				
				if (content.Control is TextEditorControl) {
					IDocument document = ((TextEditorControl)content.Control).Document;
					LineSegment line = document.GetLineSegment((int)StartLine - 1);
					int offset = line.Offset + (int)StartColumn;
					currentLineMarker = new CurrentLineMarker(offset, (int)EndColumn - (int)StartColumn, TextMarkerType.SolidBlock, Color.Yellow, Color.Blue);
					currentLineMarkerParent = document;
					currentLineMarkerParent.MarkerStrategy.TextMarker.Add(currentLineMarker);
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
					document.CommitUpdate();
				}
			}
		}
		
		
		
		static void IconBarMouseDown(AbstractMargin iconBar, Point mousepos, MouseButtons mouseButtons)
		{
			Rectangle viewRect = iconBar.TextArea.TextView.DrawingPosition;
			Point logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				ToggleBreakpointAt(iconBar.TextArea.MotherTextEditorControl.FileName , logicPos.Y + 1, 0);
				RefreshBreakpointMarkersInEditor(iconBar.TextArea.MotherTextEditorControl);
				iconBar.TextArea.Refresh(iconBar);
			}
		}
		
		
		static void RefreshBreakpointMarkersInEditor(TextEditorControl textEditor)
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
			foreach (Breakpoint b in Breakpoints) {
				if (b.FileName.ToLower() == textEditor.FileName.ToLower()) {
					LineSegment lineSeg = document.GetLineSegment((int)b.LineNumber - 1);
					document.MarkerStrategy.TextMarker.Add(new BreakpointMarker(lineSeg.Offset, lineSeg.Length , TextMarkerType.SolidBlock, Color.Red, Color.White));
				}
			}
			// Perform editor update
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			document.CommitUpdate();
		}
		
		/// <summary>
		/// Draw Breakpoint icon and the yellow arrow in the margin
		/// </summary>
		static void PaintIconBar(AbstractMargin iconBar, Graphics g, Rectangle rect)
		{
			foreach (Breakpoint breakpoint in Breakpoints) {
				if (Path.GetFullPath(breakpoint.FileName) == Path.GetFullPath(iconBar.TextArea.MotherTextEditorControl.FileName)) {
					int lineNumber = iconBar.TextArea.Document.GetVisibleLine((int)breakpoint.LineNumber - 1);
					int yPos = (int)(lineNumber * iconBar.TextArea.TextView.FontHeight) - iconBar.TextArea.VirtualTop.Y;
					if (yPos >= rect.Y && yPos <= rect.Bottom) {
						((IconBarMargin)iconBar).DrawBreakpoint(g, yPos, breakpoint.IsEnabled);
					}
				}
			}
			
			foreach (TextMarker textMarker in iconBar.TextArea.Document.MarkerStrategy.TextMarker) {
				CurrentLineMarker currentLineMarker = textMarker as CurrentLineMarker;
				if (currentLineMarker != null) {
					int lineNumber = iconBar.TextArea.Document.GetVisibleLine((int)iconBar.TextArea.Document.GetLineNumberForOffset(currentLineMarker.Offset));
					int yPos = (int)(lineNumber * iconBar.TextArea.TextView.FontHeight) - iconBar.TextArea.VirtualTop.Y;
					if (yPos >= rect.Y && yPos <= rect.Bottom) {
						((IconBarMargin)iconBar).DrawArrow(g, yPos);
					}
				}
			}
		}
		
		/// <summary>
		/// This function shows variable values as tooltips
		/// </summary>
		static void TextAreaMouseMove(object sender, MouseEventArgs args)
		{
			try {
				TextArea textArea = (TextArea)sender;
				
				Point mousepos = textArea.PointToClient(Control.MousePosition);
				Rectangle viewRect = textArea.TextView.DrawingPosition;
				if (viewRect.Contains(mousepos)) {
					Point logicPos = textArea.TextView.GetLogicalPosition(mousepos.X - viewRect.Left,
					                                                      mousepos.Y - viewRect.Top);
					if (logicPos.Y >= 0 && logicPos.Y < textArea.Document.TotalNumberOfLines) {
						IDocument doc = textArea.Document;
						IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
						LineSegment seg = doc.GetLineSegment(logicPos.Y);
						int xPosition = Math.Min(seg.Length - 1, logicPos.X);
						string textContent = doc.TextContent;
						string expression = expressionFinder.FindFullExpression(textContent, seg.Offset + xPosition);
						//Console.WriteLine("MouseMove@" + logicPos + ":" + expression);
						if (expression != null && expression != String.Empty) {
							// Look if it is variable
							//value = selectedThread.LocalVariables[expresion].Value.ToString();
							ResolveResult result = ParserService.Resolve(expression, logicPos.Y + 1, xPosition + 1, textArea.MotherTextEditorControl.FileName, textContent);
							string value = GetText(result);
							if (value != null) {
								value = "expr: >" + expression + "<\n" + value;
								textArea.SetToolTip(value);
							}
						}
					}
				}
			} catch (Exception e) {
				Console.Beep();
				Console.WriteLine();
				Console.WriteLine(e);
			}
		}
		
		static string GetText(ResolveResult result)
		{
			if (result == null)
				return null;
			IAmbience ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags
				| ConversionFlags.ShowAccessibility;
			if (result is MemberResolveResult) {
				return GetText(ambience, ((MemberResolveResult)result).ResolvedMember);
			} else if (result is LocalResolveResult) {
				LocalResolveResult rr = (LocalResolveResult)result;
				ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedNames
					| ConversionFlags.ShowReturnType
					| ConversionFlags.QualifiedNamesOnlyForReturnTypes;
				if (rr.IsParameter)
					return "parameter " + ambience.Convert(rr.Field);
				else
					return "local variable " + ambience.Convert(rr.Field);
			} else if (result is NamespaceResolveResult) {
				return "namespace " + ((NamespaceResolveResult)result).Name;
			} else if (result is TypeResolveResult) {
				return GetText(ambience, ((TypeResolveResult)result).ResolvedClass);
			} else {
				if (result.ResolvedType == null)
					return null;
				else
					return "expression of type " + ambience.Convert(result.ResolvedType);
			}
		}
		
		static string GetText(IAmbience ambience, IDecoration member)
		{
			StringBuilder text = new StringBuilder();
			if (member is IIndexer) {
				text.Append(ambience.Convert(member as IIndexer));
			} else if (member is IField) {
				text.Append(ambience.Convert(member as IField));
			} else if (member is IProperty) {
				text.Append(ambience.Convert(member as IProperty));
			} else if (member is IEvent) {
				text.Append(ambience.Convert(member as IEvent));
			} else if (member is IMethod) {
				text.Append(ambience.Convert(member as IMethod));
			} else if (member is IClass) {
				text.Append(ambience.Convert(member as IClass));
			} else {
				text.Append("unknown member ");
				text.Append(member.ToString());
			}
			string documentation = ParserService.CurrentProjectContent.GetXmlDocumentation(member.DocumentationTag);
			if (documentation != null && documentation.Length > 0) {
				text.Append('\n');
				text.Append(ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.GetDocumentation(documentation));
			}
			return text.ToString();
		}
	}
}
