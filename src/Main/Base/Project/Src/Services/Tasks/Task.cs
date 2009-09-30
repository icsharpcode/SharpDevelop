// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
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
	
	public class Task
	{
		public const string DefaultContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/TaskContextMenu";
		
		string   description;
		TaskType type;
		PermanentAnchor position;
		bool hasLocation;
		string contextMenuAddInTreeEntry = DefaultContextMenuAddInTreeEntry;
		object tag;

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
		
		public TaskType TaskType {
			get {
				return type;
			}
		}
		
		public string ContextMenuAddInTreeEntry {
			get {
				return contextMenuAddInTreeEntry;
			}
			set {
				contextMenuAddInTreeEntry = value;
			}
		}
		
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
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
		public Task(FileName fileName, string description, int column, int line, TaskType type)
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
		
		public Task(BuildError error)
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
				contextMenuAddInTreeEntry = error.ContextMenuAddInTreeEntry;
			}
			tag = error.Tag;
			this.BuildError = error;
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
