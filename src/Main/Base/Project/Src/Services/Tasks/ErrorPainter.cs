// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Synchronizes the ITextEditors with the TaskService. It adds and removes error markers.
	/// </summary>
	public class ErrorPainter : IDisposable
	{
		ITextEditor textEditor;
		ITextMarkerService markerService;
		
		public ErrorPainter(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
			this.markerService = this.textEditor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			if (this.markerService == null)
				throw new InvalidOperationException("this ITextEditor has no text marker service!");
			
			TaskService.Added   += new TaskEventHandler(OnAdded);
			TaskService.Removed += new TaskEventHandler(OnRemoved);
			TaskService.Cleared += new EventHandler(OnCleared);
			DebuggerService.DebugStarted += OnDebugStarted;
			DebuggerService.DebugStopped += OnDebugStopped;
			
			UpdateErrors();
		}
		
		bool isDisposed;
		
		/// <summary>
		/// Deregisters the event handlers so the error painter (and associated TextEditor)
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
				AddTask(task);
			}
		}
		
		void OnAdded(object sender, TaskEventArgs e)
		{
			AddTask(e.Task);
		}
		
		void OnRemoved(object sender, TaskEventArgs e)
		{
			markerService.RemoveAll(marker => marker.Tag == e.Task);
		}
		
		void OnCleared(object sender, EventArgs e)
		{
			ClearErrors();
		}
		
		/// <summary>
		/// Clears all TextMarkers representing errors.
		/// </summary>
		/// <returns>Returns true when there were markers deleted, false when there were no error markers.</returns>
		void ClearErrors()
		{
			markerService.RemoveAll(marker => marker.Tag is Task);
		}
		
		bool CheckTask(Task task)
		{
			if (textEditor.FileName == null)
				return false;
			if (task.FileName == null || task.Column <= 0)
				return false;
			if (task.TaskType != TaskType.Warning && task.TaskType != TaskType.Error)
				return false;
			return FileUtility.IsEqualFileName(task.FileName, textEditor.FileName);
		}
		
		void AddTask(Task task)
		{
			if (DebuggerService.IsDebuggerLoaded && DebuggerService.CurrentDebugger.IsDebugging)
				return;
			if (!CheckTask(task))
				return;
			
			if (task.Line >= 1 && task.Line <= textEditor.Document.TotalNumberOfLines) {
				LoggingService.Debug(task.ToString());
				int offset = textEditor.Document.PositionToOffset(task.Line, task.Column);
				int length = textEditor.Document.GetWordAt(offset).Length;
				
				if (length < 2)
					length = 2;
				
				ITextMarker marker = this.markerService.Create(offset, length);
				
				Color markerColor = Colors.Transparent;
				
				switch (task.TaskType) {
					case TaskType.Error:
						markerColor = Colors.Red;
						break;
					case TaskType.Message:
						markerColor = Colors.Blue;
						break;
					case TaskType.Warning:
						markerColor = Colors.Orange;
						break;
				}
				
				marker.MarkerColor = markerColor;
				marker.MarkerType = TextMarkerType.SquigglyUnderline;
				
				marker.ToolTip = task.Description;
				
				marker.Tag = task;
			}
		}
		
		/// <summary>
		/// Clears all errors and adds them again.
		/// </summary>
		public void UpdateErrors()
		{
			if (isDisposed)
				return;
			ClearErrors();
			foreach (Task task in TaskService.Tasks) {
				AddTask(task);
			}
		}
	}
}
