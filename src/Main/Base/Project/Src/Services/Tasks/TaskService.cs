// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public class TaskService
	{
		static List<Task> tasks = new List<Task>();
		
		static Dictionary<TaskType, int> taskCount = new Dictionary<TaskType, int>();
		
		static MessageViewCategory buildMessageViewCategory = new MessageViewCategory("Build", "${res:MainWindow.Windows.OutputWindow.BuildCategory}");
		
		public static MessageViewCategory BuildMessageViewCategory {
			get {
				return buildMessageViewCategory;
			}
		}
		
		public static int TaskCount {
			get {
				return tasks.Count - GetCount(TaskType.Comment);
			}
		}
		
		public static IEnumerable<Task> Tasks {
			get {
				foreach (Task task in tasks) {
					if (task.TaskType != TaskType.Comment) {
						yield return task;
					}
				}
			}
		}
		
		public static IEnumerable<Task> CommentTasks {
			get {
				foreach (Task task in tasks) {
					if (task.TaskType == TaskType.Comment) {
						yield return task;
					}
				}
			}
		}
		
		public static int GetCount(TaskType type)
		{
			if (!taskCount.ContainsKey(type)) {
				return 0;
			}
			return taskCount[type];
		}
		
		public static bool SomethingWentWrong {
			get {
				return GetCount(TaskType.Error) + GetCount(TaskType.Warning) > 0;
			}
		}
		
		public static bool HasCriticalErrors(bool treatWarningsAsErrors)
		{
			if (treatWarningsAsErrors) {
				return SomethingWentWrong;
			} else {
				return GetCount(TaskType.Error) > 0;
			}
		}
		
		static TaskService()
		{
			FileService.FileRenamed += CheckFileRename;
			FileService.FileRemoved += CheckFileRemove;
			
			ProjectService.SolutionClosed += new EventHandler(ProjectServiceSolutionClosed);
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			Clear();
		}
		
		static void CheckFileRemove(object sender, FileEventArgs e)
		{
			for (int i = 0; i < tasks.Count; ++i) {
				Task curTask = tasks[i];
				if (FileUtility.IsEqualFileName(curTask.FileName, e.FileName)) {
					Remove(curTask);
					--i;
				}
			}
		}
		
		static void CheckFileRename(object sender, FileRenameEventArgs e)
		{
			for (int i = 0; i < tasks.Count; ++i) {
				Task curTask = tasks[i];
				if (FileUtility.IsEqualFileName(curTask.FileName, e.SourceFile)) {
					Remove(curTask);
					curTask.FileName = FileUtility.NormalizePath(e.TargetFile);
					Add(curTask);
					--i;
				}
			}
		}
		
		public static void Clear()
		{
			taskCount.Clear();
			tasks.Clear();
			OnCleared(EventArgs.Empty);
		}
		
		public static void ClearExceptCommentTasks()
		{
			List<Task> commentTasks = new List<Task>(CommentTasks);
			Clear();
			foreach (Task t in commentTasks) {
				Add(t);
			}
		}
		
		public static void Add(Task task)
		{
			tasks.Add(task);
			if (!taskCount.ContainsKey(task.TaskType)) {
				taskCount[task.TaskType] = 1;
			} else {
				taskCount[task.TaskType]++;
			}
			OnAdded(new TaskEventArgs(task));
		}
		
		public static void AddRange(IEnumerable<Task> tasks)
		{
			foreach (Task task in tasks) {
				Add(task);
			}
		}
		
		public static void Remove(Task task)
		{
			if (tasks.Contains(task)) {
				tasks.Remove(task);
				taskCount[task.TaskType]--;
				OnRemoved(new TaskEventArgs(task));
			}
		}
		
		public static void UpdateCommentTags(string fileName, IList<TagComment> tagComments)
		{
			if (fileName == null || tagComments == null) {
				return;
			}
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateCommentTagsInvoked, fileName, tagComments);
		}
		
		static void UpdateCommentTagsInvoked(string fileName, IList<TagComment> tagComments)
		{
			List<Task> newTasks = new List<Task>();
			foreach (TagComment tag in tagComments) {
				newTasks.Add(new Task(fileName,
				                      tag.Key + tag.CommentString,
				                      tag.Region.BeginColumn - 1,
				                      tag.Region.BeginLine - 1,
				                      TaskType.Comment));
			}
			List<Task> oldTasks = new List<Task>();
			
			foreach (Task task in CommentTasks) {
				if (FileUtility.IsEqualFileName(task.FileName, fileName)) {
					oldTasks.Add(task);
				}
			}
			
			for (int i = 0; i < newTasks.Count; ++i) {
				for (int j = 0; j < oldTasks.Count; ++j) {
					if (oldTasks[j] != null &&
					    newTasks[i].Line        == oldTasks[j].Line &&
					    newTasks[i].Column      == oldTasks[j].Column &&
					    newTasks[i].Description == oldTasks[j].Description)
					{
						newTasks[i] = null;
						oldTasks[j] = null;
						break;
					}
				}
			}
			
			foreach (Task task in newTasks) {
				if (task != null) {
					Add(task);
				}
			}
			
			foreach (Task task in oldTasks) {
				if (task != null) {
					Remove(task);
				}
			}
		}

		static void OnCleared(EventArgs e)
		{
			if (Cleared != null) {
				Cleared(null, e);
			}
		}
		
		static void OnAdded(TaskEventArgs e)
		{
			if (Added != null) {
				Added(null, e);
			}
		}
		
		static void OnRemoved(TaskEventArgs e)
		{
			if (Removed != null) {
				Removed(null, e);
			}
		}
		
		public static event TaskEventHandler Added;
		public static event TaskEventHandler Removed;
		public static event EventHandler     Cleared;
		
		static bool inUpdate;
		
		public static bool InUpdate {
			get {
				return inUpdate;
			}
			set {
				if (inUpdate != value) {
					inUpdate = value;
					
					if (InUpdateChanged != null) {
						InUpdateChanged(null, EventArgs.Empty);
					}
				}
			}
		}
		
		public static event EventHandler InUpdateChanged;
	}
}
