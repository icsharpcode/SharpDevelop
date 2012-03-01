// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

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
			
			ProjectService.BeforeSolutionClosing += OnBeforeSolutionClosing;
			
			BookmarkManager.Added   += BookmarkAdded;
			BookmarkManager.Removed += BookmarkRemoved;
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
		
		static bool debuggerStarted;
		
		/// <summary>
		/// Gets whether the debugger is currently active.
		/// </summary>
		public static bool IsDebuggerStarted {
			get { return debuggerStarted; }
		}
		
		public static event EventHandler DebugStarting;
		public static event EventHandler DebugStarted;
		public static event EventHandler DebugStopped;
		
		static IAnalyticsMonitorTrackedFeature debugFeature;
		
		static void OnDebugStarting(object sender, EventArgs e)
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
			LayoutConfiguration.CurrentLayoutName = "Debug";
			
			debugFeature = AnalyticsMonitorService.TrackFeature("Debugger");
			
			ClearDebugMessages();
			
			if (DebugStarting != null)
				DebugStarting(null, e);
		}
		
		static void OnDebugStarted(object sender, EventArgs e)
		{
			debuggerStarted = true;
			if (DebugStarted != null)
				DebugStarted(null, e);
		}
		
		static void OnDebugStopped(object sender, EventArgs e)
		{
			debuggerStarted = false;
			if (debugFeature != null)
				debugFeature.EndTracking();
			
			RemoveCurrentLineMarker();
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
				foreach (SDBookmark bookmark in BookmarkManager.Bookmarks) {
					BreakpointBookmark breakpoint = bookmark as BreakpointBookmark;
					if (breakpoint != null) {
						breakpoints.Add(breakpoint);
					}
				}
				return breakpoints.AsReadOnly();
			}
		}
		
		static void BookmarkAdded(object sender, BookmarkEventArgs e)
		{
			BreakpointBookmark bb = e.Bookmark as BreakpointBookmark;
			if (bb != null) {
				bb.LineNumberChanged += BookmarkChanged;
				OnBreakPointAdded(new BreakpointBookmarkEventArgs(bb));
			}
		}
		
		static void BookmarkRemoved(object sender, BookmarkEventArgs e)
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
		
		static void OnBeforeSolutionClosing(object sender, SolutionCancelEventArgs e)
		{
			if (currentDebugger == null)
				return;
			
			if (currentDebugger.IsDebugging) {
				string caption = StringParser.Parse("${res:XML.MainMenu.DebugMenu.Stop}");
				string message = StringParser.Parse("${res:MainWindow.Windows.Debug.StopDebugging.Message}");
				string[] buttonLabels = new string[] { StringParser.Parse("${res:Global.Yes}"), StringParser.Parse("${res:Global.No}") };
				int result = MessageService.ShowCustomDialog(caption,
				                                             message,
				                                             0, // yes
				                                             1, // no
				                                             buttonLabels);
				
				if (result == 0) {
					currentDebugger.Stop();
				} else {
					e.Cancel = true;
				}
			}
		}
		
		/// <summary>
		/// Toggles a breakpoint bookmark.
		/// </summary>
		/// <param name="editor">Text editor where the bookmark is toggled.</param>
		/// <param name="lineNumber">Line number.</param>
		/// <param name="breakpointType">Type of breakpoint bookmark.</param>
		/// <param name="parameters">Optional constructor parameters.</param>
		public static void ToggleBreakpointAt(ITextEditor editor, int lineNumber, Type breakpointType, object[] parameters = null)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			
			if (breakpointType == null)
				throw new ArgumentNullException("breakpointType");
			
			if (!typeof(BreakpointBookmark).IsAssignableFrom(breakpointType))
				throw new ArgumentException("breakpointType is not a BreakpointBookmark");
			
			BookmarkManager.ToggleBookmark(
				editor, lineNumber,
				b => b.CanToggle && b is BreakpointBookmark,
				location => (BreakpointBookmark)Activator.CreateInstance(breakpointType, 
				                                                         parameters ?? new object[] { editor.FileName, location, BreakpointAction.Break, "", ""}));
		}
		
		/* TODO: reimplement this stuff
		static void ViewContentOpened(object sender, ViewContentEventArgs e)
		{
				textArea.IconBarMargin.MouseDown += IconBarMouseDown;
				textArea.ToolTipRequest          += TextAreaToolTipRequest;
				textArea.MouseLeave              += TextAreaMouseLeave;
		}*/
		
		public static void RemoveCurrentLineMarker()
		{
			CurrentLineBookmark.Remove();
		}
		
		public static void JumpToCurrentLine(string sourceFullFilename, int startLine, int startColumn, int endLine, int endColumn)
		{
			IViewContent viewContent = FileService.OpenFile(sourceFullFilename);
			if (viewContent is ITextEditorProvider)
				((ITextEditorProvider)viewContent).TextEditor.JumpTo(startLine, startColumn);
			CurrentLineBookmark.SetPosition(viewContent, startLine, startColumn, endLine, endColumn);
		}
		
		#region Tool tips
		/// <summary>
		/// Gets debugger tooltip information for the specified position.
		/// A descriptive string for the element or a DebuggerTooltipControl
		/// showing its current value (when in debugging mode) can be returned
		/// through the ToolTipRequestEventArgs.SetTooltip() method.
		/// </summary>
		internal static void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (!e.InDocument)
				return;
			Location logicPos = e.LogicalPosition;
			var doc = e.Editor.Document;
			FileName fileName = e.Editor.FileName;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null)
				return;
			
			var currentLine = doc.GetLine(logicPos.Y);
			if (logicPos.X > currentLine.Length)
				return;
			string textContent = doc.Text;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, doc.PositionToOffset(logicPos.Line, logicPos.Column));
			string expression = (expressionResult.Expression ?? "").Trim();
			if (expression.Length > 0) {
				// Look if it is variable
				ResolveResult result = ParserService.Resolve(expressionResult, logicPos.Y, logicPos.X, fileName, textContent);
				bool debuggerCanShowValue;
				string toolTipText = GetText(result, expression, out debuggerCanShowValue);
				if (Control.ModifierKeys == Keys.Control) {
					toolTipText = "expr: " + expressionResult.ToString() + "\n" + toolTipText;
					debuggerCanShowValue = false;
				}
				if (toolTipText != null) {
					if (debuggerCanShowValue && currentDebugger != null) {
						object toolTip = currentDebugger.GetTooltipControl(e.LogicalPosition, expressionResult.Expression);
						if (toolTip != null)
							e.SetToolTip(toolTip);
						else
							e.SetToolTip(toolTipText);
					} else {
						e.SetToolTip(toolTipText);
					}
				}
			} else {
				#if DEBUG
				if (Control.ModifierKeys == Keys.Control) {
					e.SetToolTip("no expr: " + expressionResult.ToString());
				}
				#endif
			}
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
					b.Append(StringParser.Parse("${res:MainWindow.Editor.Tooltip.Parameter} "));
				else
					b.Append(StringParser.Parse("${res:MainWindow.Editor.Tooltip.LocalVar} "));
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
				return StringParser.Parse("${res:MainWindow.Editor.Tooltip.Namespace} ") + ((NamespaceResolveResult)result).Name;
			} else if (result is TypeResolveResult) {
				IClass c = ((TypeResolveResult)result).ResolvedClass;
				if (c != null)
					return GetMemberText(ambience, c, expression, out debuggerCanShowValue);
				else
					return ambience.Convert(result.ResolvedType);
			} else if (result is MethodGroupResolveResult) {
				MethodGroupResolveResult mrr = result as MethodGroupResolveResult;
				IMethod m = mrr.GetMethodIfSingleOverload();
				IMethod m2 = mrr.GetMethodWithEmptyParameterList();
				if (m != null)
					return GetMemberText(ambience, m, expression, out debuggerCanShowValue);
				else if (ambience is VBNetAmbience && m2 != null)
					return GetMemberText(ambience, m2, expression, out debuggerCanShowValue);
				else
					return StringParser.Parse("${res:MainWindow.Editor.Tooltip.UnresolvedOverload} ") + ambience.Convert(mrr.ContainingType) + "." + mrr.Name;
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
				text.Append(ICSharpCode.SharpDevelop.Editor.CodeCompletion.CodeCompletionItem.ConvertDocumentation(documentation));
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
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			DebuggerService.HandleToolTipRequest(e);
		}
	}
	
	/// <summary>
	/// Interface for common debugger-decompiler mapping operations.
	/// </summary>
	public interface IDebuggerDecompilerService
	{
		/// <summary>
		/// Gets or sets the current method token and IL offset. Used for step in/out.
		/// </summary>
		Tuple<int, int> DebugStepInformation { get; set; }
		
		/// <summary>
		/// Checks the code mappings.
		/// </summary>
		bool CheckMappings(int typeToken);
		
		/// <summary>
		/// Decompiles on demand a type.
		/// </summary>
		void DecompileOnDemand(TypeDefinition type);
		
		/// <summary>
		/// Gets the IL from and IL to.
		/// </summary>
		bool GetILAndTokenByLineNumber(int typeToken, int lineNumber, out int[] ilRanges, out int memberToken);
		
		/// <summary>
		/// Gets the ILRange and source code line number.
		/// </summary>
		bool GetILAndLineNumber(int typeToken, int memberToken, int ilOffset, out int[] ilRange, out int line, out bool isMatch);
		
		/// <summary>
		/// Gets the local variables of a type and a member.
		/// </summary>
		IEnumerable<string> GetLocalVariables(int typeToken, int memberToken);
		
		/// <summary>
		/// Gets the local variable index.
		/// </summary>
		object GetLocalVariableIndex(int typeToken, int memberToken, string name);
		
		/// <summary>
		/// Gets an implementation of an assembly resolver.
		/// </summary>
		/// <param name="assemblyFile">Assembly file path.</param>
		/// <returns>An <see cref="IAssemblyResolver"/>.</returns>
		IAssemblyResolver GetAssemblyResolver(string assemblyFile);
	}
}
