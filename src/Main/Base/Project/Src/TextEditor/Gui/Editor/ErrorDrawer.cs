// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// reperesents a visual error, this class is needed by the errordrawer.
	/// </summary>
	public class VisualError : TextMarker
	{
		Task task;
		
		public Task Task {
			get {
				return task;
			}
		}
		
		public VisualError(int offset, int length, Task task)
			: base(offset, length, TextMarkerType.WaveLine, (task.TaskType == TaskType.Error) ? Color.Red : Color.Orange)
		{
			this.task = task;
			base.ToolTip = task.Description;
		}
	}
	
	/// <summary>
	/// This class draws error underlines.
	/// </summary>
	public class ErrorDrawer : IDisposable
	{
		TextEditorControl textEditor;
		
		public ErrorDrawer(TextEditorControl textEditor)
		{
			this.textEditor = textEditor;
			
			TaskService.Added   += new TaskEventHandler(OnAdded);
			TaskService.Removed += new TaskEventHandler(OnRemoved);
			TaskService.Cleared += new EventHandler(OnCleared);
			TaskService.InUpdateChanged += OnInUpdateChanged;
			DebuggerService.DebugStarted += OnDebugStarted;
			DebuggerService.DebugStopped += OnDebugStopped;
		}
		
		bool isDisposed;
		bool requireTextEditorRefresh;
		
		void RefreshTextEditor()
		{
			if (TaskService.InUpdate) {
				requireTextEditorRefresh = true;
			} else {
				textEditor.Refresh();
			}
		}
		
		void OnInUpdateChanged(object sender, EventArgs e)
		{
			if (requireTextEditorRefresh) {
				requireTextEditorRefresh = false;
				textEditor.Refresh();
			}
		}
		
		/// <summary>
		/// Deregisters the event handlers so the error drawer (and associated TextEditorControl)
		/// can be garbage collected.
		/// </summary>
		public void Dispose()
		{
			if (isDisposed)
				return;
			isDisposed = true;
			TaskService.Added   -= new TaskEventHandler(OnAdded);
			TaskService.Removed -= new TaskEventHandler(OnRemoved);
			TaskService.Cleared -= new EventHandler(OnCleared);
			TaskService.InUpdateChanged -= OnInUpdateChanged;
			DebuggerService.DebugStarted -= OnDebugStarted;
			DebuggerService.DebugStopped -= OnDebugStopped;
			ClearErrors();
		}
		
		void OnDebugStarted(object sender, EventArgs e)
		{
			ClearErrors();
		}
		
		void OnDebugStopped(object sender, EventArgs e)
		{
			foreach (Task task in TaskService.Tasks) {
				AddTask(task, false);
			}
			textEditor.Refresh();
		}
		
		void OnAdded(object sender, TaskEventArgs e)
		{
			AddTask(e.Task, true);
		}
		
		void OnRemoved(object sender, TaskEventArgs e)
		{
			Task t = e.Task;
			foreach (TextMarker marker in textEditor.Document.MarkerStrategy.TextMarker) {
				VisualError ve = marker as VisualError;
				if (ve != null && ve.Task == t) {
					textEditor.Document.MarkerStrategy.RemoveMarker(marker);
					RefreshTextEditor();
					break;
				}
			}
		}
		
		void OnCleared(object sender, EventArgs e)
		{
			if (ClearErrors()) {
				RefreshTextEditor();
			}
		}
		
		/// <summary>
		/// Clears all TextMarkers representing errors.
		/// </summary>
		/// <returns>Returns true when there were markers deleted, false when there were no error markers.</returns>
		bool ClearErrors()
		{
			bool removed = false;
			textEditor.Document.MarkerStrategy.RemoveAll(delegate (TextMarker marker) {
			                                             	if (marker is VisualError) {
			                                             		removed = true;
			                                             		return true;
			                                             	}
			                                             	return false;});
			return removed;
		}
		
		bool CheckTask(Task task)
		{
			if (textEditor.FileName == null)
				return false;
			if (task.FileName == null || task.FileName.Length == 0 || task.Column < 0)
				return false;
			if (task.TaskType != TaskType.Warning && task.TaskType != TaskType.Error)
				return false;
			return FileUtility.IsEqualFileName(task.FileName, textEditor.FileName);
		}
		
		void AddTask(Task task, bool refresh)
		{
			if (!CheckTask(task)) return;
			if (task.Line >= 0 && task.Line < textEditor.Document.TotalNumberOfLines) {
				LineSegment line = textEditor.Document.GetLineSegment(task.Line);
				int offset = line.Offset + task.Column;
				int length = 1;
				if (line.Words != null) {
					foreach (TextWord tw in line.Words) {
						if (task.Column == tw.Offset) {
							length = tw.Length;
							break;
						}
					}
				}
				if (length == 1 && task.Column < line.Length) {
					length = 2; // use minimum length
				}
				textEditor.Document.MarkerStrategy.AddMarker(new VisualError(offset, length, task));
				if (refresh) RefreshTextEditor();
			}
		}
		
		/// <summary>
		/// Clears all errors and adds them again.
		/// </summary>
		public void UpdateErrors()
		{
			ClearErrors();
			foreach (Task task in TaskService.Tasks) {
				AddTask(task, false);
			}
			textEditor.Refresh();
		}
	}
}
