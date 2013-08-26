// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.SharpDevelop.Gui
{
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
				target.ToolTip = new ToolTip {
					Content = new TextBlock { Text = target.Text, TextWrapping = TextWrapping.Wrap },
					PlacementTarget = target,
					Placement = PlacementMode.Relative,
					VerticalOffset = -2,
					HorizontalOffset = -6
				};
			} else {
				target.ToolTip = null;
			}
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
