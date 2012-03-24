// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public static class DebuggerService
	{
		static IDebugger   currentDebugger;
		static DebuggerDescriptor[] debuggers;

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
			WorkbenchSingleton.Workbench.WorkbenchLayout.SwitchLayout("Debug");
			
			debugFeature = SD.AnalyticsMonitor.TrackFeature("Debugger");
			
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
			WorkbenchSingleton.Workbench.WorkbenchLayout.SwitchLayout("Default");
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
			if (DebuggerService.IsDebuggerLoaded)
				DebuggerService.CurrentDebugger.HandleToolTipRequest(e);
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
