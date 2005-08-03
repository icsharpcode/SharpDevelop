// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class OpenTaskView : AbstractPadContent
	{
		ListView listView = new ListView();
		
		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ToolTip taskToolTip = new ToolTip();
		
		
		public override Control Control {
			get {
				return listView;
			}
		}
		
		public OpenTaskView()
		{
			type.Text = "!";
			
			RedrawContent();
			listView.Columns.Add(type);
			listView.Columns.Add(line);
			listView.Columns.Add(description);
			listView.Columns.Add(file);
			listView.Columns.Add(path);
			
			listView.FullRowSelect = true;
			listView.AutoArrange = true;
			listView.Alignment   = ListViewAlignment.Left;
			listView.View = View.Details;
			listView.Dock = DockStyle.Fill;
			listView.GridLines  = true;
			listView.Activation = ItemActivation.OneClick;
			ListViewResize(this, EventArgs.Empty);
			
			TaskService.Cleared += new EventHandler(TaskServiceCleared);
			TaskService.Added   += new TaskEventHandler(TaskServiceAdded);
			TaskService.Removed += new TaskEventHandler(TaskServiceRemoved);
			
			ProjectService.EndBuild       += SelectTaskView;
			ProjectService.SolutionLoaded += OnCombineOpen;
			ProjectService.SolutionClosed += OnCombineClosed;
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Question"));
			listView.SmallImageList = listView.LargeImageList = imglist;
			// Set up the delays for the ToolTip.
			taskToolTip.InitialDelay = 500;
			taskToolTip.ReshowDelay = 100;
			taskToolTip.AutoPopDelay = 5000;
//
//			// Force the ToolTip text to be displayed whether or not the form is active.
//			taskToolTip.ShowAlways   = false;
			
			listView.ItemActivate += new EventHandler(ListViewItemActivate);
			listView.MouseMove    += new MouseEventHandler(ListViewMouseMove);
			listView.Resize       += new EventHandler(ListViewResize);
			listView.CreateControl();
			
			ShowResults2(null, null);
		}
		
		public override void RedrawContent()
		{
			line.Text        = ResourceService.GetString("CompilerResultView.LineText");
			description.Text = ResourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = ResourceService.GetString("CompilerResultView.FileText");
			path.Text        = ResourceService.GetString("CompilerResultView.PathText");
		}
		
		void OnCombineOpen(object sender, SolutionEventArgs e)
		{
			listView.Items.Clear();
		}
		
		void OnCombineClosed(object sender, EventArgs e)
		{
			try {
				listView.Items.Clear();
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}
		
		void SelectTaskView(object sender, EventArgs e)
		{
			if (TaskService.TaskCount > 0) {
				WorkbenchSingleton.SafeThreadCall(this, "SelectTaskView2");
			}
		}
		
		void SelectTaskView2()
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this.GetType().FullName);
		}
		
		void ListViewItemActivate(object sender, EventArgs e)
		{
			if (listView.FocusedItem != null) {
				Task task = (Task)listView.FocusedItem.Tag;
				System.Diagnostics.Debug.Assert(task != null);
				task.JumpToPosition();
			}
		}
		
		ListViewItem currentListViewItem = null;
		void ListViewMouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = listView.GetItemAt(e.X, e.Y);
			if (item != currentListViewItem) {
				if (item != null) {
					Task task = (Task)item.Tag;
					string description = task.Description;
					if (description != null) {
						description = description.Replace("\t", "    ");
					}
					taskToolTip.SetToolTip(listView, description);
					taskToolTip.Active       = true;
				} else {
					taskToolTip.RemoveAll(); 
					taskToolTip.Active       = false;
				}
				currentListViewItem = item;
			}
		}
		
		void ListViewResize(object sender, EventArgs e)
		{
			type.Width = 24;
			line.Width = 50;
			int w = listView.Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;
		}
		
		public CompilerResults CompilerResults = null;
		
		void AddTask(Task task)
		{
			int imageIndex = 3;
			
			string tmpPath;
			if (task.Project != null && task.FileName != null) {
				tmpPath = FileUtility.GetRelativePath(task.Project.Directory, task.FileName);
			} else {
				tmpPath = task.FileName;
			}
			
			string fileName = tmpPath;
			string path     = tmpPath;
			
			try {
				fileName = Path.GetFileName(tmpPath);
			} catch (Exception) {}
			
			try {
				path = Path.GetDirectoryName(tmpPath);
			} catch (Exception) {}
			
			ListViewItem item = new ListViewItem(new string[] {
				String.Empty,
				(task.Line + 1).ToString(),
				FormatDescription(task.Description),
				fileName,
				path
			});
			item.ImageIndex = item.StateImageIndex = imageIndex;
			item.Tag = task;
			listView.Items.Add(item);
		}
		
		
		void TaskServiceCleared(object sender, EventArgs e)
		{
			listView.Items.Clear();
		}
		
		void TaskServiceAdded(object sender, TaskEventArgs e)
		{
			if (e.Task.TaskType == TaskType.Comment) {
				AddTask(e.Task);
			}
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			Task task = e.Task;
			for (int i = 0; i < listView.Items.Count; ++i) {
				if ((Task)listView.Items[i].Tag == task) {
					listView.Items.RemoveAt(i);
					break;
				}
			}
		}
		
		
		/// <summary>
		/// Removes new lines, carriage returns and tab characters from
		/// the list view task description and replaces them with a space.  
		/// </summary>
		/// <param name="description">The task list description.</param>
		/// <returns>A formatted task list description.</returns>
		string FormatDescription(string description)
		{
			string FormattedDescription = description.Replace("\r", " ");
			FormattedDescription = FormattedDescription.Replace("\t", " ");
			return FormattedDescription.Replace("\n", " ");
		}
		
		void ShowResults2(object sender, EventArgs e)
		{
			// listView.CreateControl is called in the constructor now.
			if (!listView.IsHandleCreated) {
				return;
			}
			listView.BeginUpdate();
			listView.Items.Clear();
			
			
			foreach (Task task in TaskService.CommentTasks) {
				AddTask(task);
			}
			
			listView.EndUpdate();
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			listView.Invoke(new EventHandler(ShowResults2));
//			SelectTaskView(null, null);
		}
	}
}
