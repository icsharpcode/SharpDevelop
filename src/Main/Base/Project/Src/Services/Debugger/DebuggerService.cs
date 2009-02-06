// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using BM = ICSharpCode.SharpDevelop.Bookmarks;
using ITextAreaToolTipProvider = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextAreaToolTipProvider;
using ITextEditorControlProvider = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider;
using ToolTipInfo = ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ToolTipInfo;

namespace ICSharpCode.SharpDevelop.Debugging
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
				debuggers = AddInTree.BuildItems<DebuggerDescriptor>("/SharpDevelop/Services/DebuggerService/Debugger", null, false).ToArray();
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
				if (d.Debugger != null && d.Debugger.CanDebug(project)) {
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
					currentDebugger.DebugStarting += new EventHandler(OnDebugStarting);
					currentDebugger.DebugStarted += new EventHandler(OnDebugStarted);
					currentDebugger.DebugStopped += new EventHandler(OnDebugStopped);
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
		
		public static event EventHandler DebugStarting;
		public static event EventHandler DebugStarted;
		public static event EventHandler DebugStopped;
		
		static void OnDebugStarting(object sender, EventArgs e)
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
			LayoutConfiguration.CurrentLayoutName = "Debug";

			ClearDebugMessages();
			
			if (DebugStarting != null)
				DebugStarting(null, e);
		}
		
		static void OnDebugStarted(object sender, EventArgs e)
		{
			if (DebugStarted != null)
				DebugStarted(null, e);
		}
		
		static void OnDebugStopped(object sender, EventArgs e)
		{
			CurrentLineBookmark.Remove();
			WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
			LayoutConfiguration.CurrentLayoutName = oldLayoutConfiguration;
			if (DebugStopped != null)
				DebugStopped(null, e);
		}

		
		static MessageViewCategory debugCategory = null;
		
		static void EnsureDebugCategory()
		{
			if (debugCategory == null) {
				MessageViewCategory.Create(ref debugCategory, "Debug", "${res:MainWindow.Windows.OutputWindow.DebugCategory}");
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
				return breakpoints.AsReadOnly();
			}
		}
		
		static void BookmarkAdded(object sender, BM.BookmarkEventArgs e)
		{
			BreakpointBookmark bb = e.Bookmark as BreakpointBookmark;
			if (bb != null) {
				bb.LineNumberChanged += BookmarkChanged;
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
		
		static void BookmarkChanged(object sender, EventArgs e)
		{
			BreakpointBookmark bb = sender as BreakpointBookmark;
			if (bb != null) {
				OnBreakPointChanged(new BreakpointBookmarkEventArgs(bb));
			}
		}
		
		public static void ToggleBreakpointAt(IDocument document, string fileName, int lineNumber)
		{
			ReadOnlyCollection<Bookmark> bookmarks = document.BookmarkManager.Marks;
			for (int i = bookmarks.Count - 1; i >= 0; --i) {
				BreakpointBookmark breakpoint = bookmarks[i] as BreakpointBookmark;
				if (breakpoint != null) {
					if (breakpoint.LineNumber == lineNumber) {
						document.BookmarkManager.RemoveMark(breakpoint);
						return;
					}
				}
			}
			int column = 0;
			foreach (char ch in document.GetText(document.GetLineSegment(lineNumber))) {
				if (!char.IsWhiteSpace(ch)) {
					document.BookmarkManager.AddMark(new BreakpointBookmark(fileName, document, new TextLocation(column, lineNumber), BreakpointAction.Break, "", ""));
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
					document.CommitUpdate();
					break;
				}
				column++;
			}
		}
		
		static void WorkspaceCreated(object sender, EventArgs args)
		{
			WorkbenchSingleton.Workbench.ViewOpened += new ViewContentEventHandler(ViewContentOpened);
			WorkbenchSingleton.Workbench.ViewClosed += new ViewContentEventHandler(ViewContentClosed);
		}
		
		static void ViewContentOpened(object sender, ViewContentEventArgs e)
		{
			if (e.Content is ITextEditorControlProvider) {
				TextArea textArea = ((ITextEditorControlProvider)e.Content).TextEditorControl.ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown += IconBarMouseDown;
				textArea.ToolTipRequest          += TextAreaToolTipRequest;
				textArea.MouseLeave              += TextAreaMouseLeave;
			}
		}
		
		static void ViewContentClosed(object sender, ViewContentEventArgs e)
		{
			if (e.Content is ITextEditorControlProvider) {
				TextArea textArea = ((ITextEditorControlProvider)e.Content).TextEditorControl.ActiveTextAreaControl.TextArea;
				
				textArea.IconBarMargin.MouseDown -= IconBarMouseDown;
				textArea.ToolTipRequest          -= TextAreaToolTipRequest;
				textArea.MouseLeave              -= TextAreaMouseLeave;
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
			TextLocation logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				ToggleBreakpointAt(iconBar.TextArea.Document, iconBar.TextArea.MotherTextEditorControl.FileName, logicPos.Y);
				iconBar.TextArea.Refresh(iconBar);
			}
		}
		
		#region Tool tips
		const string ToolTipProviderAddInTreePath = "/SharpDevelop/ViewContent/DefaultTextEditor/ToolTips";
		
		static DebuggerGridControl oldToolTipControl;
		
		/// <summary>
		/// This function shows variable values as tooltips
		/// </summary>
		static void TextAreaToolTipRequest(object sender, ToolTipRequestEventArgs e)
		{
			DebuggerGridControl toolTipControl = null;
			try {
				TextArea textArea = (TextArea)sender;
				if (e.ToolTipShown) return;
				if (oldToolTipControl != null && !oldToolTipControl.AllowClose) return;
				if (!CodeCompletionOptions.EnableCodeCompletion) return;
				if (!CodeCompletionOptions.TooltipsEnabled) return;
				
				if (CodeCompletionOptions.TooltipsOnlyWhenDebugging) {
					if (currentDebugger == null) return;
					if (!currentDebugger.IsDebugging) return;
				}
				
				if (e.InDocument) {
					// Query all registered tooltip providers using the AddInTree.
					// The first one that does not return null will be used.
					ToolTipInfo ti = null;
					foreach (ITextAreaToolTipProvider toolTipProvider in AddInTree.BuildItems<ITextAreaToolTipProvider>(ToolTipProviderAddInTreePath, null, false)) {
						if ((ti = toolTipProvider.GetToolTipInfo(textArea, e)) != null) {
							break;
						}
					}
					
					if (ti != null) {
						toolTipControl = ti.ToolTipControl as DebuggerGridControl;
						if (ti.ToolTipText != null) {
							e.ShowToolTip(ti.ToolTipText);
						}
					}
					CloseOldToolTip();
					if (toolTipControl != null) {
						toolTipControl.ShowForm(textArea, e.LogicalPosition);
					}
					oldToolTipControl = toolTipControl;
					
				}
			} catch (Exception ex) {
				ICSharpCode.Core.MessageService.ShowError(ex, "Error while requesting tooltip for location " + e.LogicalPosition);
			} finally {
				if (toolTipControl == null && CanCloseOldToolTip)
					CloseOldToolTip();
			}
		}
		
		static bool CanCloseOldToolTip {
			get {
				return oldToolTipControl != null && oldToolTipControl.AllowClose;
			}
		}
		
		static void CloseOldToolTip()
		{
			if (oldToolTipControl != null) {
				Form frm = oldToolTipControl.FindForm();
				if (frm != null) frm.Close();
				oldToolTipControl = null;
			}
		}
		
		static void TextAreaMouseLeave(object source, EventArgs e)
		{
			if (CanCloseOldToolTip && !oldToolTipControl.IsMouseOver)
				CloseOldToolTip();
		}
		
		/// <summary>
		/// Gets debugger tooltip information for the specified position.
		/// A descriptive text for the element or a DebuggerGridControl
		/// showing its current value (when in debugging mode) can be returned
		/// through the ToolTipInfo object.
		/// Returns <c>null</c>, if no tooltip information is available.
		/// </summary>
		internal static ToolTipInfo GetToolTipInfo(TextArea textArea, ToolTipRequestEventArgs e)
		{
			TextLocation logicPos = e.LogicalPosition;
			IDocument doc = textArea.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
			if (expressionFinder == null)
				return null;
			LineSegment seg = doc.GetLineSegment(logicPos.Y);
			if (logicPos.X > seg.Length - 1)
				return null;
			string textContent = doc.TextContent;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + logicPos.X);
			string expression = (expressionResult.Expression ?? "").Trim();
			if (expression.Length > 0) {
				// Look if it is variable
				ResolveResult result = ParserService.Resolve(expressionResult, logicPos.Y + 1, logicPos.X + 1, textArea.MotherTextEditorControl.FileName, textContent);
				bool debuggerCanShowValue;
				string toolTipText = GetText(result, expression, out debuggerCanShowValue);
				if (Control.ModifierKeys == Keys.Control) {
					toolTipText = "expr: " + expressionResult.ToString() + "\n" + toolTipText;
					debuggerCanShowValue = false;
				}
				if (toolTipText != null) {
					if (debuggerCanShowValue && currentDebugger != null) {
						return new ToolTipInfo(currentDebugger.GetTooltipControl(expressionResult.Expression));
					}
					return new ToolTipInfo(toolTipText);
				}
			} else {
				#if DEBUG
				if (Control.ModifierKeys == Keys.Control) {
					return new ToolTipInfo("no expr: " + expressionResult.ToString());
				}
				#endif
			}
			return null;
		}
		
		static string GetText(ResolveResult result, string expression, out bool debuggerCanShowValue)
		{
			debuggerCanShowValue = false;
			if (result == null) {
				// when pressing control, show the expression even when it could not be resolved
				return (Control.ModifierKeys == Keys.Control) ? "" : null;
			}
			if (result is MixedResolveResult)
				return GetText(((MixedResolveResult)result).PrimaryResult, expression, out debuggerCanShowValue);
			else if (result is DelegateCallResolveResult)
				return GetText(((DelegateCallResolveResult)result).Target, expression, out debuggerCanShowValue);
			
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.UseFullyQualifiedMemberNames;
			if (result is MemberResolveResult) {
				return GetMemberText(ambience, ((MemberResolveResult)result).ResolvedMember, expression, out debuggerCanShowValue);
			} else if (result is LocalResolveResult) {
				LocalResolveResult rr = (LocalResolveResult)result;
				ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedTypeNames
					| ConversionFlags.ShowReturnType | ConversionFlags.ShowDefinitionKeyWord;
				StringBuilder b = new StringBuilder();
				if (rr.IsParameter)
					b.Append("parameter ");
				else
					b.Append("local variable ");
				b.Append(ambience.Convert(rr.Field));
				if (currentDebugger != null) {
					string currentValue = currentDebugger.GetValueAsString(rr.VariableName);
					if (currentValue != null) {
						debuggerCanShowValue = true;
						b.Append(" = ");
						if (currentValue.Length > 256)
							currentValue = currentValue.Substring(0, 256) + "...";
						b.Append(currentValue);
					}
				}
				return b.ToString();
			} else if (result is NamespaceResolveResult) {
				return "namespace " + ((NamespaceResolveResult)result).Name;
			} else if (result is TypeResolveResult) {
				IClass c = ((TypeResolveResult)result).ResolvedClass;
				if (c != null)
					return GetMemberText(ambience, c, expression, out debuggerCanShowValue);
				else
					return ambience.Convert(result.ResolvedType);
			} else if (result is MethodGroupResolveResult) {
				MethodGroupResolveResult mrr = result as MethodGroupResolveResult;
				IMethod m = mrr.GetMethodIfSingleOverload();
				if (m != null)
					return GetMemberText(ambience, m, expression, out debuggerCanShowValue);
				else
					return "Overload of " + ambience.Convert(mrr.ContainingType) + "." + mrr.Name;
			} else {
				if (Control.ModifierKeys == Keys.Control) {
					if (result.ResolvedType != null)
						return "expression of type " + ambience.Convert(result.ResolvedType);
					else
						return "ResolveResult without ResolvedType";
				} else {
					return null;
				}
			}
		}
		
		static string GetMemberText(IAmbience ambience, IEntity member, string expression, out bool debuggerCanShowValue)
		{
			bool tryDisplayValue = false;
			debuggerCanShowValue = false;
			StringBuilder text = new StringBuilder();
			if (member is IField) {
				text.Append(ambience.Convert(member as IField));
				tryDisplayValue = true;
			} else if (member is IProperty) {
				text.Append(ambience.Convert(member as IProperty));
				tryDisplayValue = true;
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
			if (tryDisplayValue && currentDebugger != null) {
				LoggingService.Info("asking debugger for value of '" + expression + "'");
				string currentValue = currentDebugger.GetValueAsString(expression);
				if (currentValue != null) {
					debuggerCanShowValue = true;
					text.Append(" = ");
					text.Append(currentValue);
				}
			}
			string documentation = member.Documentation;
			if (documentation != null && documentation.Length > 0) {
				text.Append('\n');
				text.Append(ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.ConvertDocumentation(documentation));
			}
			return text.ToString();
		}
		#endregion
	}
	
	/// <summary>
	/// Provides the default debugger tooltips on the text area.
	/// </summary>
	/// <remarks>
	/// This class must be public because it is accessed via the AddInTree.
	/// </remarks>
	public class DebuggerTextAreaToolTipProvider : ITextAreaToolTipProvider
	{
		public ToolTipInfo GetToolTipInfo(TextArea textArea, ToolTipRequestEventArgs e)
		{
			return DebuggerService.GetToolTipInfo(textArea, e);
		}
	}
}
