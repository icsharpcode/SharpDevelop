// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public enum TaskType {
		Error,
		Warning,
		Message,
		
		Comment,
	}
	
	public class SDTask
	{
		[Obsolete("Default path now depends on parent pad, use ErrorListPad.DefaultContextMenuAddInTreeEntry instead.")]
		public const string DefaultContextMenuAddInTreeEntry = Gui.ErrorListPad.DefaultContextMenuAddInTreeEntry;
		
		string   description;
		TaskType type;
		PermanentAnchor position;
		bool hasLocation;

		public override string ToString()
		{
			return String.Format("[Task:File={0}, Line={1}, Column={2}, Type={3}, Description={4}",
			                     FileName,
			                     Line,
			                     Column,
			                     type,
			                     description);
		}
		
		/// <summary>
		/// The line number of the task. Starts counting at 1.
		/// </summary>
		public int Line {
			get {
				if (hasLocation && !position.IsDeleted)
					return position.Line;
				else
					return 0;
			}
		}
		
		/// <summary>
		/// The column number of the task. Starts counting at 1.
		/// </summary>
		public int Column {
			get {
				if (hasLocation && !position.IsDeleted)
					return position.Column;
				else
					return 0;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public FileName FileName {
			get {
				if (position != null)
					return position.FileName;
				else
					return null;
			}
		}
		
		public string File {
			get { return position == null ? null : System.IO.Path.GetFileName(position.FileName); }
		}
		
		public string Path {
			get { return position == null ? null : System.IO.Path.GetDirectoryName(position.FileName); }
		}
		
		public TaskType TaskType {
			get {
				return type;
			}
		}
		
		public ImageSource TaskTypeImage {
			get {
				switch (type) {
					case TaskType.Error:
						return PresentationResourceService.GetBitmapSource("Icons.16x16.Error");
					case TaskType.Warning:
						return PresentationResourceService.GetBitmapSource("Icons.16x16.Warning");
					case TaskType.Message:
						return PresentationResourceService.GetBitmapSource("Icons.16x16.Information");
					case TaskType.Comment:
						return PresentationResourceService.GetBitmapSource("Icons.16x16.Question");
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		
		public string ContextMenuAddInTreeEntry { get; set; }
		
		public object Tag { get; set; }
		
		/// <summary>
		/// Contains a reference to the build error.
		/// </summary>
		public BuildError BuildError { get; private set; }
		
		/// <summary>
		/// Creates a new Task instance.
		/// </summary>
		/// <param name="fileName">The file that the task refers to. Use null if the task does not refer to a file.</param>
		/// <param name="description">Description of the task. This parameter cannot be null.</param>
		/// <param name="column">Task column (1-based), use 0 if no column is known</param>
		/// <param name="line">Task line (1-based), use 0 if no line number is known</param>
		/// <param name="type">Type of the task</param>
		public SDTask(FileName fileName, string description, int column, int line, TaskType type)
		{
			if (description == null)
				throw new ArgumentNullException("description");
			this.type        = type;
			this.description = description.Trim();
			if (fileName != null) {
				hasLocation = line >= 1;
				this.position = new PermanentAnchor(fileName, Math.Max(1, line), Math.Max(1, column));
				position.Deleted += position_Deleted;
			}
		}
		
		void position_Deleted(object sender, EventArgs e)
		{
			TaskService.Remove(this);
		}
		
		public SDTask(BuildError error)
		{
			if (error == null)
				throw new ArgumentNullException("error");
			type = error.IsWarning ? TaskType.Warning : TaskType.Error;
			int line = Math.Max(error.Line, 1);
			int column = Math.Max(error.Column, 1);
			if (!string.IsNullOrEmpty(error.FileName)) {
				hasLocation = error.Line >= 1;
				this.position = new PermanentAnchor(FileName.Create(error.FileName), line, column);
				position.Deleted += position_Deleted;
			}
			if (string.IsNullOrEmpty(error.ErrorCode)) {
				description = error.ErrorText;
			} else {
				description = error.ErrorText + " (" + error.ErrorCode + ")";
			}
			if (error.ContextMenuAddInTreeEntry != null) {
				ContextMenuAddInTreeEntry = error.ContextMenuAddInTreeEntry;
			}
			this.Tag = error.Tag;
			this.BuildError = error;
		}
		
		public SDTask(Error error)
		{
			if (error == null)
				throw new ArgumentNullException("error");
			switch (error.ErrorType) {
				case ErrorType.Error:
					type = TaskType.Error;
					break;
				case ErrorType.Warning:
					type = TaskType.Warning;
					break;
			}
			description = error.Message;
			//hasLocation = !error.Region.IsEmpty;
		}
		
		public void JumpToPosition()
		{
			if (hasLocation && !position.IsDeleted)
				FileService.JumpToFilePosition(position.FileName, position.Line, position.Column);
			else if (position != null)
				FileService.OpenFile(position.FileName);
		}
	}
}
