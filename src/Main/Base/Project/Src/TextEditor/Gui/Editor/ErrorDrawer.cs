// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// reperesents a visual error, this class is needed by the errordrawer.
	/// </summary>
	public class VisualError : TextMarker
	{
		public VisualError(int offset, int length, string description, bool isError) : base(offset, length, TextMarkerType.WaveLine, isError ? Color.Red : Color.Orange)
		{
			base.ToolTip = description.Replace("&", "&&&");
		}
	}
	
	/// <summary>
	/// This class draws error underlines.
	/// </summary>
	public class ErrorDrawer
	{
		ArrayList       errors = new ArrayList();
		TextEditorControl textEditor;
		
		public ErrorDrawer(TextEditorControl textEditor)
		{
			this.textEditor = textEditor;
			
			
			TaskService.TasksChanged += new EventHandler(SetErrors);
			
			textEditor.FileNameChanged += new EventHandler(SetErrors);
		}
		
		void ClearErrors()
		{
			List<TextMarker> markers = textEditor.Document.MarkerStrategy.TextMarker;
			for (int i = 0; i < markers.Count;) {
				if (markers[i] is VisualError) {
					markers.RemoveAt(i);
				} else {
					i++; // Check next one
				}
			}
		}
		
		void SetErrors(object sender, EventArgs e)
		{
			ClearErrors();
			if (textEditor.FileName == null) {
				return;
			}
			
			foreach (Task task in TaskService.Tasks) {
				if (task.FileName == null || task.FileName.Length == 0 || task.Column < 0) {
					continue;
				}
				if (Path.GetFullPath(task.FileName).ToLower() == Path.GetFullPath(textEditor.FileName).ToLower() && (task.TaskType == TaskType.Warning || task.TaskType == TaskType.Error)) {
					if (task.Line >= 0 && task.Line < textEditor.Document.TotalNumberOfLines) {
						LineSegment line = textEditor.Document.GetLineSegment(task.Line);
						if (line.Words != null) {
							int offset = line.Offset + task.Column;
							foreach (TextWord tw in line.Words) {
								if (task.Column >= tw.Offset && task.Column < (tw.Offset + tw.Length)) {
									textEditor.Document.MarkerStrategy.TextMarker.Add(new VisualError(offset, tw.Length, task.Description, task.TaskType == TaskType.Error));
									break;
								}
							}
						}
						/*
						int startOffset = offset;//Math.Min(textEditor.Document.TextLength, TextUtilities.FindWordStart(textEditor.Document, offset));
						int endOffset   = Math.Max(1, TextUtilities.FindWordEnd(textEditor.Document, offset));
						textEditor.Document.MarkerStrategy.TextMarker.Add(new VisualError(startOffset, endOffset - startOffset + 1, task.Description, task.TaskType == TaskType.Error));*/
					}
				}
			}
			
			textEditor.Refresh();
		}
	}
}
