// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision$</version>
// </file>

// much of TaskView's code has been refactored from 
//  TaskList.cs (formerly OpenTaskView.cs) & ErrorList.cs

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public enum TaskViewCols : int {
		Icon = 0,
		Line = 1,
		Description = 2,
		File = 3,
		Path = 4
	}
	
	/// <summary>
	/// Description of TaskView.
	/// </summary>
	public class TaskView : ListView
	{
		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ToolTip taskToolTip = new ToolTip();

		public Task SelectedTask {
			get {
				if (this.FocusedItem==null) {
					return null;
				}
				return (Task)this.FocusedItem.Tag;
			}
		}
		
		public bool TaskIsSelected {
			get {
				return this.FocusedItem!=null;
			}
		}
		
		public TaskView() : base()
		{
			RefreshColumnNames();

			this.Columns.Add(type);
			this.Columns.Add(line);
			this.Columns.Add(description);
			this.Columns.Add(file);
			this.Columns.Add(path);
			
			this.FullRowSelect = true;
			this.AutoArrange = true;
			this.Alignment   = ListViewAlignment.Left;
			this.View = View.Details;
			this.Dock = DockStyle.Fill;
			this.GridLines  = true;
			this.Activation = ItemActivation.OneClick;
			OnResize(EventArgs.Empty);

			// ...
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			// HACK: these images must be in the same order as the TaskType enumeration.
			//       because of how the ListViewItem's ImageIndex is set in TaskView.AddTask();
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Question"));
			// TODO: use reflection and a custom attribute on the TaskType enumeration to tie this ImageList to TaskType
//			imglist.Images.Add(ResourceService.GetBitmap(Task.GetBitmapName(TaskType.Error)));
//			imglist.Images.Add(ResourceService.GetBitmap(Task.GetBitmapName(TaskType.Warning)));
//			imglist.Images.Add(ResourceService.GetBitmap(Task.GetBitmapName(TaskType.Message)));
//			imglist.Images.Add(ResourceService.GetBitmap(Task.GetBitmapName(TaskType.Comment)));
			this.SmallImageList = this.LargeImageList = imglist;

			// Set up the delays for the ToolTip.
			taskToolTip.InitialDelay = 500;
			taskToolTip.ReshowDelay = 100;
			taskToolTip.AutoPopDelay = 5000;
//
//			// Force the ToolTip text to be displayed whether or not the form is active.
//			taskToolTip.ShowAlways   = false;
			
			SortBy(TaskViewCols.Path);
		}
		
		public void RefreshColumnNames()
		{
			type.Text        = "!";
			line.Text        = ResourceService.GetString("CompilerResultView.LineText");
			description.Text = ResourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = ResourceService.GetString("CompilerResultView.FileText");
			path.Text        = ResourceService.GetString("CompilerResultView.PathText");
		}
		
		#region Event Overrides
		protected override void OnResize(EventArgs e) {
			// recalculate column widths
			type.Width = 24;
			line.Width = 50;
			int w = this.Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;

			base.OnResize(e);
		}
		
		protected override void OnColumnClick(ColumnClickEventArgs e)
		{	
			SortBy(e.Column);
			base.OnColumnClick(e);
		}
		
		protected override void OnItemActivate(EventArgs e) {
			if (this.FocusedItem != null) {
				System.Diagnostics.Debug.Assert(SelectedTask != null);
				SelectedTask.JumpToPosition();
			}
			base.OnItemActivate(e);		
		}

		ListViewItem currentListViewItem = null;
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			ListViewItem item = GetItemAt(e.X, e.Y);
			if (item != currentListViewItem) {
				if (item != null) {
					Task task = (Task)item.Tag;
					string description = task.Description;
					if (description != null) {
						description = description.Replace("\t", "    ");
					}
					taskToolTip.SetToolTip(this, description);
					taskToolTip.Active       = true;
				} else {
					taskToolTip.RemoveAll();
					taskToolTip.Active       = false;
				}
				currentListViewItem = item;
			}
		}
		#endregion
		
		#region Task Management
		
		public void ClearTasks()
		{
			this.Items.Clear();
		}
		
		public void AddTask(Task task)
		{
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
			item.ImageIndex = item.StateImageIndex = (int)task.TaskType;
			item.Tag = task;
			
			// insert new item into sorted list (binary search)
			// ... using the current ListViewItemSorter
			int left = 0;
			int right = this.Items.Count - 1;
			while (left <= right) {
				int m = left + (right - left) / 2;
				if (this.ListViewItemSorter.Compare(item, this.Items[m]) > 0) {
					left = m + 1;
				} else {
					right = m - 1;
				}
			}
			this.Items.Insert(left, item);
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
		
		public void RemoveTask(Task task)
		{
			for (int i = 0; i < Items.Count; ++i) {
				if ((Task)Items[i].Tag == task) {
					Items.RemoveAt(i);
					break;
				}
			}			
		}
		
		public void UpdateResults(System.Collections.Generic.IEnumerable<ICSharpCode.Core.Task> taskSet)
		{
			this.BeginUpdate();
			this.ClearTasks();
			
			foreach (Task task in taskSet) {
				this.AddTask(task);
			}
			
			this.EndUpdate();
		}
					
		#endregion
		
		#region Custom IComparer for sorting TaskView.

		int currentSortColumn = -1;
		SortOrder currentSortOrder = SortOrder.Ascending;
		
		/// <summary>
		/// Applies the specified sort request by creating, 
		/// configuring, and installing a 
		/// <see cref="TaskViewSorter"/>.
		/// </summary>
		private void SortBy(TaskViewCols col)
		{
			SortBy((int)col);
		}
		private void SortBy(int col)
		{
			if (col==currentSortColumn) {
				// same as last sort; toggle the current sort order.
				if (currentSortOrder == SortOrder.Ascending) {
					currentSortOrder = SortOrder.Descending;
				} else {
					currentSortOrder = SortOrder.Ascending;
				}
			} else {
				currentSortColumn = col;
				currentSortOrder = SortOrder.Ascending;
			}
			
			this.ListViewItemSorter = 
				new TaskViewSorter(currentSortColumn, currentSortOrder);
		}
		
		/// <summary>
		/// Custom <see cref="IComparer"/> for TaskView.
		/// </summary>
		private class TaskViewSorter : IComparer
		{
			int sortCol = -1;
			SortOrder sortOrder = SortOrder.Ascending;
			
			public TaskViewSorter(int col, SortOrder order)
			{
				sortCol = col;
				sortOrder = order;
			}
						
			protected int CompareLineNumbers(ListViewItem a, ListViewItem b)
			{
				return ((Task)a.Tag).Line.CompareTo(((Task)b.Tag).Line);
			}

			protected int CompareAsText(ListViewItem a, ListViewItem b, TaskViewCols col)
			{
				return CompareAsText(a, b, (int)col);
			}
			protected int CompareAsText(ListViewItem a, ListViewItem b, int col)
			{
				return a.SubItems[col].Text.CompareTo(b.SubItems[col].Text);;
			}
			
			#region System.Collections.IComparer interface implementation

			/// <summary>
			/// The meat of this IComparer class; intelligently compares
			/// two TaskView ListItems.
			/// </summary>
			/// <returns>
			/// A signed integer indicating the relative sort ranking
			/// of item <paramref name="x"/> relative to item
			/// <paramref name="y"/>.
			/// Return value greater than zero: x > y.  
			/// Return value is zero: x == y.  
			/// Return value is less than zero: x \< y.
			/// </returns>
			public int Compare(object x, object y) {
				ListViewItem a = (ListViewItem)x;   // List item a
				ListViewItem b = (ListViewItem)y;   // List item b
				int r = 0;                          // the result
				
				if (sortCol==(int)TaskViewCols.Line) {
					
					// Sorts as Line Number
					r = CompareLineNumbers(a, b);
					
				} else { // all others
					
					r = CompareAsText(a, b, sortCol);
					
					// doing this test first is an optimization; we only
					// need to check for secondary sort conditions if
					// the primary test is a tie.
					if (r == 0) {

						if (sortCol==(int)TaskViewCols.Path) {
							
							// Sorts as Path -> File
							r = CompareAsText(a, b, TaskViewCols.File);
							
							if (r == 0) {

								// Sorts as Path -> File -> Line
								r = CompareLineNumbers(a, b);
							}
							
						} else if (sortCol==(int)TaskViewCols.File) {

							// Sorts as File -> Line
							r = CompareLineNumbers(a, b);
						}
					}
				}
				
				if (sortOrder == SortOrder.Descending) {
					// Return the opposite ranking
					return -r;
				}
				
				return r;
			}
			#endregion
		}
		#endregion
	}
}
