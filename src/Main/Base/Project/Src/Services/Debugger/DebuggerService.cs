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
using BM = ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.Core
{
	public static class DebuggerService
	{
		static IDebugger   currentDebugger;
		static ArrayList   debuggers;
		static string      oldLayoutConfiguration = "Default";

		static DebuggerService()
		{
			AddInTreeNode treeNode = null;
			try {
				treeNode = AddInTree.GetTreeNode("/SharpDevelop/Services/DebuggerService/Debugger");
			} catch (Exception) {
			}
			if (treeNode != null) {
				debuggers = treeNode.BuildChildItems(null);
			}
			if (debuggers == null) {
				debuggers = new ArrayList();
			}
			
			ProjectService.SolutionLoaded += delegate {
				ClearDebugMessages();
			};

			WorkbenchSingleton.WorkbenchCreated += new EventHandler(WorkspaceCreated);
			BM.BookmarkManager.Added   += BookmarkAdded;
			BM.BookmarkManager.Removed += BookmarkRemoved;
		}
		
		static IDebugger GetCompatibleDebugger()
		{
			IProject project = null;
			if (ProjectService.OpenSolution != null) {
				project = ProjectService.OpenSolution.StartupProject;
			}
			foreach (IDebugger d in debuggers) {
				if (d.CanDebug(project)) {
					return d;
				}
			}
			return new DefaultDebugger();
		}

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

		static void DebugStarted(object sender, EventArgs e)
		{
			//oldLayoutConfiguration = LayoutConfiguration.CurrentLayoutName;
			//LayoutConfiguration.CurrentLayoutName = "Debug";

			ClearDebugMessages();
		}

		static void DebugStopped(object sender, EventArgs e)
		{
			//LayoutConfiguration.CurrentLayoutName = oldLayoutConfiguration;
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
				RefreshBreakpointMarkersInDocument(bb.Document);
				OnBreakPointAdded(new BreakpointBookmarkEventArgs(bb));
			}
		}
		
		static void BookmarkRemoved(object sender, BM.BookmarkEventArgs e)
		{
			BreakpointBookmark bb = e.Bookmark as BreakpointBookmark;
			if (bb != null) {
				RefreshBreakpointMarkersInDocument(bb.Document);
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
		
		static void RefreshBreakpointMarkersInDocument(IDocument document)
		{
			if (document == null) return;
			List<TextMarker> markers = document.MarkerStrategy.TextMarker;
			// Remove all breakpoint markers
			for (int i = 0; i < markers.Count;) {
				if (markers[i] is BreakpointMarker) {
					markers.RemoveAt(i);
				} else {
					i++; // Check next one
				}
			}
			// Add breakpoint markers
			foreach (BreakpointBookmark b in Breakpoints) {
				LineSegment lineSeg = document.GetLineSegment(b.LineNumber);
				document.MarkerStrategy.TextMarker.Add(new BreakpointMarker(lineSeg.Offset, lineSeg.Length, TextMarkerType.SolidBlock, Color.Red, Color.White));
			}
			// Perform editor update
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			document.CommitUpdate();
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
				
				RefreshBreakpointMarkersInDocument(textArea.MotherTextEditorControl.Document);
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
			if (mouseButtons != MouseButtons.Left) return;
			Rectangle viewRect = iconBar.TextArea.TextView.DrawingPosition;
			Point logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				ToggleBreakpointAt(iconBar.TextArea.Document, iconBar.TextArea.MotherTextEditorControl.FileName, logicPos.Y);
				iconBar.TextArea.Refresh(iconBar);
			}
		}
		
		/// <summary>
		/// Draw Breakpoint icon and the yellow arrow in the margin
		/// </summary>
		static void PaintIconBar(AbstractMargin iconBar, Graphics g, Rectangle rect)
		{
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
		
		static string oldExpression, oldToolTip;
		static int oldLine;
		
		/// <summary>
		/// This function shows variable values as tooltips
		/// </summary>
		static void TextAreaMouseMove(object sender, MouseEventArgs args)
		{
			try {
				TextArea textArea = (TextArea)sender;
				if (textArea.ToolTipVisible) return;
				if (!CodeCompletionOptions.TooltipsEnabled) return;
				
				// TODO: if (CodeCompletionOptions.TooltipsOnlyWhenDebugging && !isDebugging) return;
				
				Point mousepos = textArea.PointToClient(Control.MousePosition);
				Rectangle viewRect = textArea.TextView.DrawingPosition;
				if (viewRect.Contains(mousepos)) {
					Point logicPos = textArea.TextView.GetLogicalPosition(mousepos.X - viewRect.Left,
					                                                      mousepos.Y - viewRect.Top);
					if (logicPos.Y >= 0 && logicPos.Y < textArea.Document.TotalNumberOfLines) {
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
				IDebugger debugger = CurrentDebugger;
				if (debugger != null) {
					string currentValue = debugger.GetValueAsString(rr.Field.Name);
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
				IReturnType container = ((MethodResolveResult)result).ContainingType;
				List<IMethod> methods = container.GetMethods();
				methods = methods.FindAll(delegate(IMethod m) {
				                          	return m.Name == ((MethodResolveResult)result).Name;
				                          });
				if (methods.Count == 1)
					return GetText(ambience, methods[0]);
				else
					return "Overload of " + ambience.Convert(container) + "." + ((MethodResolveResult)result).Name;
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
	}
}
