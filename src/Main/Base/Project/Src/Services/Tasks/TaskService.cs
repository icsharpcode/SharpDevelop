// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public class TaskService
	{
		static ArrayList tasks          = new ArrayList();
		static ArrayList commentTasks   = new ArrayList();
		static MessageViewCategory buildMessageViewCategory = new MessageViewCategory("Build", "${res:MainWindow.Windows.OutputWindow.BuildCategory}");
		
		public static MessageViewCategory BuildMessageViewCategory {
			get {
				return buildMessageViewCategory;
			}
		}
		
		public static ArrayList Tasks {
			get {
				return tasks;
			}
		}
		
		public static ArrayList CommentTasks {
			get {
				return commentTasks;
			}
		}
		
		static int warnings = 0;
		static int errors   = 0;
		static int messages = 0;
		static int comments = 0;
		
		public static int Warnings {
			get {
				return warnings;
			}
		}
		
		public static int Errors {
			get {
				return errors;
			}
		}
		
		public static int Messages {
			get {
				return messages;
			}
		}
		
		public static int Comments {
			get {
				return comments;
			}
		}
		
		public static bool SomethingWentWrong {
			get {
				return errors + warnings > 0;
			}
		}
		
		public static bool HasCriticalErrors(bool treatWarningsAsErrors)
		{
			if (treatWarningsAsErrors) {
				return errors + warnings > 0;
			} else {
				return errors > 0;
			}
		}
		
		static void OnTasksChanged(EventArgs e)
		{
			if (TasksChanged != null) {
				TasksChanged(null, e);
			}
		}
		
		static TaskService()
		{
			FileService.FileRenamed += new FileRenameEventHandler(CheckFileRename);
			FileService.FileRemoved += new FileEventHandler(CheckFileRemove);
			
			ProjectService.SolutionClosed += new EventHandler(ProjectServiceSolutionClosed);
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			tasks.Clear();
			commentTasks.Clear();
			NotifyTaskChange();
		}
		
		static void CheckFileRemove(object sender, FileEventArgs e)
		{
			bool somethingChanged = false;
			for (int i = 0; i < tasks.Count; ++i) {
				Task curTask = (Task)tasks[i];
				bool doRemoveTask = false;
				try {
					doRemoveTask = Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.FileName);
				} catch {
					doRemoveTask = curTask.FileName == e.FileName;
				}
				if (doRemoveTask) {
					tasks.RemoveAt(i);
					--i;
					somethingChanged = true;
				}
			}
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		static void CheckFileRename(object sender, FileRenameEventArgs e)
		{
			bool somethingChanged = false;
			foreach (Task curTask in tasks) {
				if (Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.SourceFile)) {
					curTask.FileName = Path.GetFullPath(e.TargetFile);
					somethingChanged = true;
				}
			}
			
			foreach (Task curTask in commentTasks) {
				if (Path.GetFullPath(curTask.FileName) == Path.GetFullPath(e.SourceFile)) {
					curTask.FileName = Path.GetFullPath(e.TargetFile);
					somethingChanged = true;
				}
			}
			
			
			if (somethingChanged) {
				NotifyTaskChange();
			}
		}
		
		public static void RemoveCommentTasks(string fileName)
		{
			bool removed = false;
			for (int i = 0; i < commentTasks.Count; ++i) {
				Task task = (Task)commentTasks[i];
				if (Path.GetFullPath(task.FileName) == Path.GetFullPath(fileName)) {
					commentTasks.RemoveAt(i);
					removed = true;
					--i;
				}
			}
			if (removed) {
				NotifyTaskChange();
			}
		}
		
		public static void NotifyTaskChange()
		{
			warnings = errors = comments = 0;
			foreach (Task task in tasks) {
				switch (task.TaskType) {
					case TaskType.Warning:
						++warnings;
						break;
					case TaskType.Error:
						++errors;
						break;
					default:
						++comments;
						break;
				}
			}
			OnTasksChanged(null);
		}
		
		public static event EventHandler TasksChanged;
	}

}
