// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of TaskViewResources.
	/// </summary>
	public partial class TaskViewResources : ResourceDictionary
	{
		public TaskViewResources()
		{
			InitializeComponent();
		}
		
		static readonly System.Reflection.MethodInfo GetLineDetails =
			typeof(TextBlock).GetMethod("GetLineDetails",
			                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		
		void TextBlockSizeChanged(object sender, SizeChangedEventArgs e)
		{
			TextBlock target = sender as TextBlock;
			if (target == null) return;
			var args = new object[] { 0, 0, 0, 0, 0 };
			GetLineDetails.Invoke(target, args);
			if ((int)args[4] > 0) {
				target.ToolTip = new ToolTip { Content = new TextBlock { Text = target.Text, TextWrapping = TextWrapping.Wrap }, PlacementTarget = target, Placement = PlacementMode.Relative };
			} else {
				target.ToolTip = null;
			}
		}
		
		void ListViewSizeChanged(object sender, SizeChangedEventArgs e)
		{
			ListView target = sender as ListView;
			if (target == null) return;
			GridView view = target.View as  GridView;
			if (view == null) return;
			view.Columns[0].Width = 35;
			view.Columns[1].Width = 50;
			double w = target.ActualWidth - view.Columns[0].Width - view.Columns[1].Width;
			view.Columns[3].Width = w * 15 / 100;
			view.Columns[4].Width = w * 15 / 100;
			view.Columns[2].Width = w - view.Columns[3].Width - view.Columns[4].Width - 30;
		}
		
		public static void CopySelectionToClipboard(ListView taskView)
		{
			StringBuilder b = new StringBuilder();
			foreach (SDTask t in taskView.SelectedItems) {
				if (b.Length > 0) b.AppendLine();
				b.Append(t.Description);
				if (!string.IsNullOrEmpty(t.FileName)) {
					b.Append(" - ");
					b.Append(t.FileName);
					if (t.Line >= 1) {
						b.Append(':');
						b.Append(t.Line);
						if (t.Column > 1) {
							b.Append(',');
							b.Append(t.Column);
						}
					}
				}
			}
			SD.Clipboard.SetText(b.ToString());
		}
	}
}
