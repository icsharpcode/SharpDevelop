// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The list box used inside the CompletionList.
	/// </summary>
	public class CompletionListBox : ListBox
	{
		internal ScrollViewer scrollViewer;
		
		/// <inheritdoc/>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			// Find the scroll viewer:
			scrollViewer = null;
			if (this.VisualChildrenCount > 0) {
				Border border = this.GetVisualChild(0) as Border;
				if (border != null)
					scrollViewer = border.Child as ScrollViewer;
			}
		}
		
		/// <summary>
		/// Gets the number of the first visible item.
		/// </summary>
		public int FirstVisibleItem {
			get {
				if (scrollViewer == null) {
					return 0;
				} else {
					return (int)(this.Items.Count * scrollViewer.VerticalOffset / scrollViewer.ExtentHeight);
				}
			}
		}
		
		/// <summary>
		/// Gets the number of visible items.
		/// </summary>
		public int VisibleItemCount {
			get {
				if (scrollViewer == null) {
					return 10;
				} else {
					return Math.Max(
						3,
						(int)Math.Ceiling(this.Items.Count * scrollViewer.ViewportHeight
						                  / scrollViewer.ExtentHeight));
				}
			}
		}
	}
}
