// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public enum TaskType {
		Error,
		Warning,
		Message,
		
		Comment,
	}
	
	public class Task
	{
		string   description;
		string   fileName;
		TaskType type;
		IProject project;
		int      line;
		int      column;
		

		public override string ToString()
		{
			return String.Format("[Task:File={0}, Line={1}, Column={2}, Type={3}, Description={4}",
			                     fileName,
			                     line,
			                     column,
			                     type,
			                     description);
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		/// <summary>
		/// The line number of the task. Zero-based (text editor coordinate)
		/// </summary>
		public int Line {
			get {
				return line;
			}
		}
		
		/// <summary>
		/// The column number of the task. Zero-based (text editor coordinate)
		/// </summary>
		public int Column {
			get {
				return column;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public TaskType TaskType {
			get {
				return type;
			}
		}
		
		public Task(string fileName, string description, int column, int line, TaskType type, IProject project)
			: this(fileName, description, column, line, type)
		{
			this.project = project;
		}
		
		public Task(string fileName, string description, int column, int line, TaskType type)
		{
			this.type        = type;
			this.fileName    = fileName;
			this.description = description.Trim();
			this.column      = column;
			this.line        = line;
		}
		
		public Task(CompilerError error)
		{
			type         = error.IsWarning ? TaskType.Warning : TaskType.Error;
			column       = Math.Max(error.Column - 1, 0);
			line         = Math.Max(error.Line - 1, 0);
			description  = error.ErrorText + "(" + error.ErrorNumber + ")";
			fileName     = error.FileName;
		}
		
		public void JumpToPosition()
		{
			FileService.JumpToFilePosition(fileName, line, column);
		}
	}
}
