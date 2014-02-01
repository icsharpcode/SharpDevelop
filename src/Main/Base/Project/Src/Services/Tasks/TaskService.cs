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
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public class TaskService
	{
		static List<SDTask> tasks = new List<SDTask>();
		
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
		
		public static IEnumerable<SDTask> Tasks {
			get {
				foreach (SDTask task in tasks) {
					if (task.TaskType != TaskType.Comment) {
						yield return task;
					}
				}
			}
		}
		
		public static IEnumerable<SDTask> CommentTasks {
			get {
				foreach (SDTask task in tasks) {
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
		
		internal static void Initialize()
		{
			// avoid trouble with double initialization
			SD.ParserService.ParseInformationUpdated -= ParserService_ParseInformationUpdated;
			SD.ParserService.ParseInformationUpdated += ParserService_ParseInformationUpdated;
			SD.ProjectService.SolutionClosed -= ProjectServiceSolutionClosed;
			SD.ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
		}
		
		static void ParserService_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			if (e.NewUnresolvedFile == SD.ParserService.GetExistingUnresolvedFile(e.FileName)) {
				// Call UpdateCommentTags only for the main parse information (if a file is in multiple projects),
				// and only if the results haven't already been replaced with a more recent ParseInformation.
				if (e.NewParseInformation != null) {
					UpdateCommentTags(e.FileName, e.NewParseInformation.TagComments);
				} else {
					UpdateCommentTags(e.FileName, new List<TagComment>());
				}
			}
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			Clear();
		}
		
		public static void Clear()
		{
			taskCount.Clear();
			tasks.Clear();
			OnCleared(EventArgs.Empty);
		}
		
		public static void ClearExceptCommentTasks()
		{
			bool wasInUpdate = InUpdate;
			InUpdate = true;
			try {
				List<SDTask> commentTasks = new List<SDTask>(CommentTasks);
				Clear();
				foreach (SDTask t in commentTasks) {
					Add(t);
				}
			} finally {
				InUpdate = wasInUpdate;
			}
		}
		
		public static void Add(SDTask task)
		{
			tasks.Add(task);
			if (!taskCount.ContainsKey(task.TaskType)) {
				taskCount[task.TaskType] = 1;
			} else {
				taskCount[task.TaskType]++;
			}
			OnAdded(new TaskEventArgs(task));
		}
		
		public static void AddRange(IEnumerable<SDTask> tasks)
		{
			foreach (SDTask task in tasks) {
				Add(task);
			}
		}
		
		public static void Remove(SDTask task)
		{
			if (tasks.Contains(task)) {
				tasks.Remove(task);
				taskCount[task.TaskType]--;
				OnRemoved(new TaskEventArgs(task));
			}
		}
		
		static void UpdateCommentTags(FileName fileName, IEnumerable<TagComment> tagComments)
		{
			List<SDTask> newTasks = new List<SDTask>();
			foreach (TagComment tag in tagComments) {
				newTasks.Add(new SDTask(fileName,
				                        tag.Key + tag.CommentString,
				                        tag.Region.BeginColumn,
				                        tag.Region.BeginLine,
				                        TaskType.Comment));
			}
			List<SDTask> oldTasks = new List<SDTask>();
			
			foreach (SDTask task in CommentTasks) {
				if (task.FileName == fileName) {
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
			
			foreach (SDTask task in newTasks) {
				if (task != null) {
					Add(task);
				}
			}
			
			foreach (SDTask task in oldTasks) {
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
