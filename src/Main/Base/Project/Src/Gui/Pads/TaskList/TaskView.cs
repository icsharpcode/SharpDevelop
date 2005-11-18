// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision$</version>
// </file>

// much of TaskView's code has been refactored from OpenTaskView.cs & ErrorList.cs

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
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(ResourceService.GetBitmap("Icons.16x16.Question"));
			this.SmallImageList = this.LargeImageList = imglist;

			// Set up the delays for the ToolTip.
			taskToolTip.InitialDelay = 500;
			taskToolTip.ReshowDelay = 100;
			taskToolTip.AutoPopDelay = 5000;
//
//			// Force the ToolTip text to be displayed whether or not the form is active.
//			taskToolTip.ShowAlways   = false;
			
		}

		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		ToolTip taskToolTip = new ToolTip();
		
		public void RefreshColumnNames()
		{
			type.Text = "!";
			line.Text        = ResourceService.GetString("CompilerResultView.LineText");
			description.Text = ResourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = ResourceService.GetString("CompilerResultView.FileText");
			path.Text        = ResourceService.GetString("CompilerResultView.PathText");
		}
		
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);

			// recalculate column widths
			type.Width = 24;
			line.Width = 50;
			int w = this.Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;
		}
		
		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			base.OnColumnClick(e);
			
			this.ListViewItemSorter = new TaskViewSorter(e.Column);
		}
		
		protected override void OnItemActivate(EventArgs e) {
			base.OnItemActivate(e);
		
			if (this.FocusedItem != null) {
				Task task = (Task)this.FocusedItem.Tag;
				System.Diagnostics.Debug.Assert(task != null);
				task.JumpToPosition();
			}
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
		
		public void AddTask(Task task)
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
			// insert new item into sorted list (binary search)
			int left = 0;
			int right = this.Items.Count - 1;
			while (left <= right) {
				int m = left + (right - left) / 2;
				if (CompareItems(item, this.Items[m])) {
					left = m + 1;
				} else {
					right = m - 1;
				}
			}
			this.Items.Insert(left, item);
		}
		
		// TODO: DavidAlpert - convert CompareItems into an IComparer implementation and
		//                     install it as the default ListViewItemSorter; then modify
		//                     AddTask to use the current ListViewItemSorter when adding
		//                     new items.

		/// <summary>Returns true when a &gt; b</summary>
		bool CompareItems(ListViewItem a, ListViewItem b)
		{
			// TODO: DavidAlpert - convert ListViewItem.SubItem index references into TaskViewCol enum.

			// insert sorted by: Directory, FileName, Line
			int res = string.Compare(a.SubItems[3].Text, b.SubItems[4].Text, StringComparison.InvariantCultureIgnoreCase);
			if (res > 0) return true;
			if (res < 0) return false;
			res = string.Compare(a.SubItems[3].Text, b.SubItems[3].Text, StringComparison.InvariantCultureIgnoreCase);
			if (res > 0) return true;
			if (res < 0) return false;
			Task x = (Task)a.Tag;
			Task y = (Task)b.Tag;
			return x.Line > y.Line;
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
		
		#region Custom IComparer for sorting TaskView.

		/// <summary>
		/// Custom <see cref="IComparer"/> for TaskView.
		/// </summary>
		private class TaskViewSorter : IComparer
		{
			static int currentSortColumn = -1;
			static SortOrder currentSortOrder = SortOrder.Ascending;
			
			public TaskViewSorter(int col)
			{
				if (col==currentSortColumn) {
					if (currentSortOrder == SortOrder.Ascending) {
						currentSortOrder = SortOrder.Descending;
					} else {
						currentSortOrder = SortOrder.Ascending;
					}
				} else {
					currentSortColumn = col;
					currentSortOrder = SortOrder.Ascending;
				}
			}
			
			/// <summary>
			/// Compares 2 Int32 objects using <see cref="Int32.CompareTo"/>.
			/// </summary>
			/// <returns>
			/// A signed number indicating the relative values of two Int32
			/// objects; less than zero if b is less than a.
			/// </returns>
			protected int CompareAsInt32(object a, object b)
			{
				return Convert.ToInt32(a).CompareTo(Convert.ToInt32(b));
			}
			
			protected int CompareAsText(ListViewItem a, ListViewItem b, TaskViewCols c)
			{
				return CompareAsText(a, b, (int)c);
			}
			
			protected int CompareAsText(ListViewItem a, ListViewItem b, int c)
			{
				return a.SubItems[(int)c].Text.CompareTo(b.SubItems[(int)c].Text);;
			}
			
			#region System.Collections.IComparer interface implementation

			/// <summary>
			/// The meat of this IComparer class; intelligently compares
			/// two FileReportResults items.
			/// </summary>
			/// <returns>
			/// A signed integer indicating the relative sort ranking
			/// of item <paramref name="x"/> relative to item
			/// <paramref name="y"/>.
			/// </returns>
			public int Compare(object x, object y) {
				ListViewItem a = (ListViewItem)x;   // List item a
				ListViewItem b = (ListViewItem)y;   // List item b
				int r = 0;                          // the result
				
				if (currentSortColumn==(int)TaskViewCols.Line) {
					r = CompareAsInt32(a.SubItems[currentSortColumn].Text,
					                   b.SubItems[currentSortColumn].Text);
				} else { // all others
					
					r = CompareAsText(a, b, currentSortColumn);
					
					// doing this test first is an optimization; we only
					// need to check for secondary sort conditions if
					// the primary test is a tie.
					if (r == 0) {

						if (currentSortColumn==(int)TaskViewCols.Path) {
							
							// Sorts as Path -> File
							r = CompareAsText(a, b, TaskViewCols.File);
							
							if (r == 0) {

								// Sorts as Path -> File -> Line
								r = CompareAsText(a, b, TaskViewCols.Line);
							}
							
						} else if (currentSortColumn==(int)TaskViewCols.File) {

							// Sorts as File -> Line
							r = CompareAsInt32(a.SubItems[(int)TaskViewCols.Line].Text,
							                   b.SubItems[(int)TaskViewCols.Line].Text);
						}
					}
				}
				
				if (currentSortOrder == SortOrder.Descending) {
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
