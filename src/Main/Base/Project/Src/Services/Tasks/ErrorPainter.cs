// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

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
			instances.Add(this);
			this.textEditor = textEditor;
			this.markerService = this.textEditor.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			if (this.markerService == null)
				throw new InvalidOperationException("this ITextEditor has no text marker service!");
			
			TaskService.Added   += OnAdded;
			TaskService.Removed += OnRemoved;
			TaskService.Cleared += OnCleared;
			DebuggerService.DebugStarted += OnDebugStartedStopped;
			DebuggerService.DebugStopped += OnDebugStartedStopped;
			textEditor.Options.PropertyChanged += textEditor_Options_PropertyChanged;
			
			ErrorColor = Colors.Red;
			WarningColor = Colors.Orange;
			MessageColor = Colors.Blue;
			
			UpdateEnabled();
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
			TaskService.Added   -= OnAdded;
			TaskService.Removed -= OnRemoved;
			TaskService.Cleared -= OnCleared;
			DebuggerService.DebugStarted -= OnDebugStartedStopped;
			DebuggerService.DebugStopped -= OnDebugStartedStopped;
			textEditor.Options.PropertyChanged -= textEditor_Options_PropertyChanged;
			ClearErrors();
			instances.Remove(this);
		}
		
		static readonly List<ErrorPainter> instances = new List<ErrorPainter>();
		
		public static IEnumerable<ErrorPainter> Instances {
			get {
				WorkbenchSingleton.AssertMainThread();
				return instances;
			}
		}
		
		public const string ErrorColorName = "Error marker";
		public const string WarningColorName = "Warning marker";
		public const string MessageColorName = "Message marker";
		
		Color errorColor;
		
		public Color ErrorColor {
			get { return errorColor; }
			set {
				if (errorColor != value) {
					errorColor = value;
					UpdateErrors();
				}
			}
		}
		
		Color warningColor;
		
		public Color WarningColor {
			get { return warningColor; }
			set {
				if (warningColor != value) {
					warningColor = value;
					UpdateErrors();
				}
			}
		}
		
		Color messageColor;
		
		public Color MessageColor {
			get { return messageColor; }
			set {
				if (messageColor != value) {
					messageColor = value;
					UpdateErrors();
				}
			}
		}
		
		bool isEnabled;
		
		void OnDebugStartedStopped(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
		
		void textEditor_Options_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "UnderlineErrors") {
				UpdateEnabled();
			}
		}
		
		void UpdateEnabled()
		{
			bool newEnabled = textEditor.Options.UnderlineErrors;
			if (DebuggerService.IsDebuggerStarted)
				newEnabled = false;
			
			if (isEnabled != newEnabled) {
				isEnabled = newEnabled;

				ClearErrors();
				if (newEnabled) {
					foreach (Task task in TaskService.Tasks) {
						AddTask(task);
					}
				}
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
			if (!isEnabled)
				return;
			if (!CheckTask(task))
				return;
			
			if (task.Line >= 1 && task.Line <= textEditor.Document.TotalNumberOfLines) {
				LoggingService.Debug(task.ToString());
				int offset = textEditor.Document.PositionToOffset(task.Line, task.Column);
				int endOffset = TextUtilities.GetNextCaretPosition(DocumentUtilitites.GetTextSource(textEditor.Document), offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
				if (endOffset < 0) endOffset = textEditor.Document.TextLength;
				int length = endOffset - offset;
				
				if (length < 2) {
					// marker should be at least 2 characters long, but take care that we don't make
					// it longer than the document
					length = Math.Min(2, textEditor.Document.TextLength - offset);
				}
				
				ITextMarker marker = this.markerService.Create(offset, length);
				
				Color markerColor = Colors.Transparent;
				
				switch (task.TaskType) {
					case TaskType.Error:
						markerColor = ErrorColor;
						break;
					case TaskType.Message:
						markerColor = MessageColor;
						break;
					case TaskType.Warning:
						markerColor = WarningColor;
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
			if (!isEnabled)
				return;
			foreach (Task task in TaskService.Tasks) {
				AddTask(task);
			}
		}
	}
}
