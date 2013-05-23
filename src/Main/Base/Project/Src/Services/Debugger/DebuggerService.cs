// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public static class DebuggerService
	{
		static IDebugger   currentDebugger;
		static DebuggerDescriptor[] debuggers;

		static DebuggerService()
		{
			SD.ProjectService.SolutionOpened += delegate {
				ClearDebugMessages();
			};
			
			SD.ProjectService.SolutionClosing += OnSolutionClosing;
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
//			IProject project = null;
//			if (ProjectService.OpenSolution != null) {
//				project = ProjectService.OpenSolution.StartupProject;
//			}
			foreach (DebuggerDescriptor d in debuggers) {
				if (d.Debugger != null /*&& d.Debugger.CanDebug(project)*/) {
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
			SD.Workbench.CurrentLayoutConfiguration = "Debug";
			
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
			SD.Workbench.CurrentLayoutConfiguration = "Default";
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
		
		static void OnSolutionClosing(object sender, SolutionClosingEventArgs e)
		{
			if (currentDebugger == null)
				return;
			
			if (currentDebugger.IsDebugging) {
				if (!e.AllowCancel) {
					currentDebugger.Stop();
					return;
				}
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
		public static void ToggleBreakpointAt(ITextEditor editor, int lineNumber)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			
			if (!SD.BookmarkManager.RemoveBookmarkAt(editor.FileName, lineNumber, b => b is BreakpointBookmark)) {
				SD.BookmarkManager.AddMark(new BreakpointBookmark(), editor.Document, lineNumber);
			}
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
			if (viewContent != null) {
				IPositionable positionable = viewContent.GetService<IPositionable>();
				if (positionable != null) {
					positionable.JumpTo(startLine, startColumn);
				}
			}
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
}
