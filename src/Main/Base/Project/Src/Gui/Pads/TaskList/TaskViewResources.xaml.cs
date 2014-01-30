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
