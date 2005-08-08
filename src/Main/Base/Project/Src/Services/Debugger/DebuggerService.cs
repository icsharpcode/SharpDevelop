// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using System.Drawing;
using System.Windows.Forms;
using BM = ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.Core
{
	public static class DebuggerService
	{
		static IDebugger   currentDebugger;
		static DebuggerDescriptor[] debuggers;
		static string      oldLayoutConfiguration = "Default";

		static DebuggerService()
		{
			ProjectService.SolutionLoaded += delegate {
				ClearDebugMessages();
			};
			
			WorkbenchSingleton.WorkbenchCreated += new EventHandler(WorkspaceCreated);
			BM.BookmarkManager.Added   += BookmarkAdded;
			BM.BookmarkManager.Removed += BookmarkRemoved;
		}
		
		static void GetDescriptors()
		{
			if (debuggers == null) {
				debuggers = (DebuggerDescriptor[])AddInTree.BuildItems("/SharpDevelop/Services/DebuggerService/Debugger", null, false).ToArray(typeof(DebuggerDescriptor));
			}
		}
		
		static IDebugger GetCompatibleDebugger()
		{
			GetDescriptors();
			IProject project = null;
			if (ProjectService.OpenSolution != null) {
				project = ProjectService.OpenSolution.StartupProject;
			}
			foreach (DebuggerDescriptor d in debuggers) {
				if (d.Debugger.CanDebug(project)) {
					return d.Debugger;
				}
			}
			return new DefaultDebugger();
		}
		
		/// <summary>
		/// Gets the current debugger. The debugger addin is loaded on demand; so if you
		/// just want to check a property like IsDebugging, check <see cref="IsDebuggerLoaded"/>
		/// before using this property.
		/// </summary>
		public static IDebugger CurrentDebugger {
			get {
				if (currentDebugger == null) {
					currentDebugger = GetCompatibleDebugger();
					currentDebugger.DebugStarted += new EventHandler(DebugStarted);
					currentDebugger.DebugStopped += new EventHandler(DebugStopped);
				}
				return currentDebugger;
			}
		}
		
		public static DebuggerDescriptor Descriptor {
			get {
				GetDescriptors();
				if (debuggers.Length > 0)
					return debuggers[0];
				return null;
			}
		}
		
		/// <summary>
		/// Returns true if debugger is already loaded.
		/// </summary>
		public static bool IsDebuggerLoaded {
			get {
				return currentDebugger != null;
			}
		}
		
		static void DebugStarted(object sender, EventArgs e)
		{
			oldLayoutConfiguration = LayoutConfiguration.CurrentLayoutName;
			LayoutConfiguration.CurrentLayoutName = "Debug";

			ClearDebugMessages();
		}
		
		static void DebugStopped(object sender, EventArgs e)
		{
			CurrentLineBookmark.Remove();
			LayoutConfiguration.CurrentLayoutName = oldLayoutConfiguration;
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
			EnsureDebugCategory();
			debugCategory.AppendText(msg);
		}


		public static event EventHandler<BreakpointBookmarkEventArgs> BreakPointChanged;
		public static event EventHandler<BreakpointBookmarkEventArgs> BreakPointAdded;
		public static event EventHandler<BreakpointBookmarkEventArgs> BreakPointRemoved;
		
		static void OnBreakPointChanged(BreakpointBookmarkEventArgs e)
		{
			if (BreakPointChanged != null) {
				BreakPointChanged(null, e);
			}
		}
		
		static void OnBreakPointAdded(BreakpointBookmarkEventArgs e)
		{
			if (BreakPointAdded != null) {
				BreakPointAdded(null, e);
			}
		}
		
		static void OnBreakPointRemoved(BreakpointBookmarkEventArgs e)
		{
			if (BreakPointRemoved != null) {
				BreakPointRemoved(null, e);
			}
		}
		
		public static IList<BreakpointBookmark> Breakpoints {
			get {
				List<BreakpointBookmark> breakpoints = new List<BreakpointBookmark>();
				foreach (BM.SDBookmark bookmark in BM.BookmarkManager.Bookmarks) {
					BreakpointBookmark breakpoint = bookmark as BreakpointBookmark;
					if (breakpoint != null) {
						breakpoints.Add(breakpoint);
					}
				}
				return breakpoints;
			}
		}
		
		static void BookmarkAdded(object sender, BM.BookmarkEventArgs e)
		{
			BreakpointBookmark bb = e.Bookmark as BreakpointBookmark;
			if (bb != null) {
				OnBreakPointAdded(new BreakpointBookmarkEventArgs(bb));
			}
		}
		
		static void BookmarkRemoved(object sender, BM.BookmarkEventArgs e)
		{
			BreakpointBookmark bb = e.Bookmark as BreakpointBookmark;
			if (bb != null) {
				bb.RemoveMarker();
				OnBreakPointRemoved(new BreakpointBookmarkEventArgs(bb));
			}
		}
		
		static void ToggleBreakpointAt(IDocument document, string fileName, int lineNumber)
		{
			foreach (Bookmark m in document.BookmarkManager.Marks) {
				BreakpointBookmark breakpoint = m as BreakpointBookmark;
				if (breakpoint != null) {
					if (breakpoint.LineNumber == lineNumber) {
						document.BookmarkManager.RemoveMark(m);
						return;
					}
				}
			}
			document.BookmarkManager.AddMark(new BreakpointBookmark(fileName, document, lineNumber));
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
				textArea.MouseMove               += new MouseEventHandler(TextAreaMouseMove);
			}
		}
		
		static void ViewContentClosed(object sender, ViewContentEventArgs e)
		{
			if (e.Content.Control is TextEditor.TextEditorControl) {
				TextArea textArea = ((TextEditor.TextEditorControl)e.Content.Control).ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown -= new MarginMouseEventHandler(IconBarMouseDown);
				textArea.MouseMove               -= new MouseEventHandler(TextAreaMouseMove);
			}
		}
		
		public static void RemoveCurrentLineMarker()
		{
			CurrentLineBookmark.Remove();
		}
		
		public static void JumpToCurrentLine(string SourceFullFilename, int StartLine, int StartColumn, int EndLine, int EndColumn)
		{
			IViewContent viewContent = FileService.JumpToFilePosition(SourceFullFilename, StartLine - 1, StartColumn - 1);
			CurrentLineBookmark.SetPosition(viewContent, StartLine, StartColumn, EndLine, EndColumn);
		}
		
		static void IconBarMouseDown(AbstractMargin iconBar, Point mousepos, MouseButtons mouseButtons)
		{
			if (mouseButtons != MouseButtons.Left) return;
			Rectangle viewRect = iconBar.TextArea.TextView.DrawingPosition;
			Point logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				ToggleBreakpointAt(iconBar.TextArea.Document, iconBar.TextArea.MotherTextEditorControl.FileName, logicPos.Y);
				iconBar.TextArea.Refresh(iconBar);
			}
		}
		
		#region Tool tips
		static string oldExpression, oldToolTip;
		static int oldLine;
		
		public class SetIPArgs: EventArgs
		{
			public string filename;
			public int line;
			public int column;
		}
		
		public static event EventHandler<SetIPArgs> SetIPRequest;
		
		/// <summary>
		/// This function shows variable values as tooltips
		/// </summary>
		static void TextAreaMouseMove(object sender, MouseEventArgs args)
		{
			try {
				TextArea textArea = (TextArea)sender;
				if (textArea.ToolTipVisible) return;
				if (!CodeCompletionOptions.TooltipsEnabled) return;
				
				if (CodeCompletionOptions.TooltipsOnlyWhenDebugging) {
					if (currentDebugger == null) return;
					if (!currentDebugger.IsDebugging) return;
				}
				
				Point mousepos = textArea.PointToClient(Control.MousePosition);
				Rectangle viewRect = textArea.TextView.DrawingPosition;
				if (viewRect.Contains(mousepos)) {
					Point logicPos = textArea.TextView.GetLogicalPosition(mousepos.X - viewRect.Left,
					                                                      mousepos.Y - viewRect.Top);
					if (logicPos.Y >= 0 && logicPos.Y < textArea.Document.TotalNumberOfLines) {
						// This is for testing olny - it must be reworked properly
						if (Control.ModifierKeys == Keys.Control) {
							SetIPArgs a = new SetIPArgs();
							a.filename = textArea.MotherTextEditorControl.FileName;
							a.line = logicPos.Y;
							a.column = logicPos.X;
							if (SetIPRequest != null) {
								SetIPRequest(null, a);
							}
							return;
						}
						
						IDocument doc = textArea.Document;
						IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
						if (expressionFinder == null)
							return;
						LineSegment seg = doc.GetLineSegment(logicPos.Y);
						if (logicPos.X > seg.Length - 1)
							return;
						string textContent = doc.TextContent;
						ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + logicPos.X);
						string expression = expressionResult.Expression;
						if (expression != null && expression.Length > 0) {
							if (expression == oldExpression && oldLine == logicPos.Y) {
								// same expression in same line -> reuse old tooltip
								if (oldToolTip != null) {
									textArea.SetToolTip(oldToolTip);
								}
								// SetToolTip must be called in every mousemove event,
								// otherwise textArea will close the tooltip.
							} else {
								// Look if it is variable
								ResolveResult result = ParserService.Resolve(expressionResult, logicPos.Y + 1, logicPos.X + 1, textArea.MotherTextEditorControl.FileName, textContent);
								string value = GetText(result);
								if (value != null) {
									#if DEBUG
									value = "expr: >" + expression + "<\n" + value;
									#endif
									textArea.SetToolTip(value);
								}
								oldToolTip = value;
							}
						}
						oldLine = logicPos.Y;
						oldExpression = expression;
					}
				}
			} catch (Exception e) {
				ICSharpCode.Core.MessageService.ShowError(e);
			}
		}
		
		static string GetText(ResolveResult result)
		{
			if (result == null)
				return null;
			if (result is MixedResolveResult)
				return GetText(((MixedResolveResult)result).PrimaryResult);
			IAmbience ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowAccessibility;
			if (result is MemberResolveResult) {
				return GetText(ambience, ((MemberResolveResult)result).ResolvedMember);
			} else if (result is LocalResolveResult) {
				LocalResolveResult rr = (LocalResolveResult)result;
				ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedNames
					| ConversionFlags.ShowReturnType
					| ConversionFlags.QualifiedNamesOnlyForReturnTypes;
				StringBuilder b = new StringBuilder();
				if (rr.IsParameter)
					b.Append("parameter ");
				else
					b.Append("local variable ");
				b.Append(ambience.Convert(rr.Field));
				if (currentDebugger != null) {
					string currentValue = currentDebugger.GetValueAsString(rr.Field.Name);
					if (currentValue != null) {
						b.Append(" = ");
						b.Append(currentValue);
					}
				}
				return b.ToString();
			} else if (result is NamespaceResolveResult) {
				return "namespace " + ((NamespaceResolveResult)result).Name;
			} else if (result is TypeResolveResult) {
				IClass c = ((TypeResolveResult)result).ResolvedClass;
				if (c != null)
					return GetText(ambience, c);
				else
					return ambience.Convert(result.ResolvedType);
			} else if (result is MethodResolveResult) {
				MethodResolveResult mrr = result as MethodResolveResult;
				IMethod m = mrr.GetMethodIfSingleOverload();
				if (m != null)
					return GetText(ambience, m);
				else
					return "Overload of " + ambience.Convert(mrr.ContainingType) + "." + mrr.Name;
			} else {
//				if (result.ResolvedType != null)
//					return "expression of type " + ambience.Convert(result.ResolvedType);
				return null;
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
			string documentation = member.Documentation;
			if (documentation != null && documentation.Length > 0) {
				text.Append('\n');
				text.Append(ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.GetDocumentation(documentation));
			}
			return text.ToString();
		}
		#endregion
	}
}
