// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	/// <summary>
	/// Description of CodeCompletionListView.	
	/// </summary>
	public class CodeCompletionListView : System.Windows.Forms.UserControl
	{
		ICompletionData[] completionData;
		int               firstItem    = 0;
		int               selectedItem = 0;
		ImageList         imageList;
		
		public ImageList ImageList {
			get {
				return imageList;
			}
			set {
				imageList = value;
			}
		}
		
		public int FirstItem {
			get {
				return firstItem;
			}
			set {
				firstItem = value;
				OnFirstItemChanged(EventArgs.Empty);
			}
		}
		
		public ICompletionData SelectedCompletionData {
			get {
				if (selectedItem < 0) {
					return null;
				}
				return completionData[selectedItem];
			}
		}
		
		public int ItemHeight {
			get {
				return Math.Max(imageList.ImageSize.Height, (int)(Font.Height * 1.25));
			}
		}
		
		public int MaxVisibleItem {
			get {
				return Height / ItemHeight;
			}
		}
		
		public CodeCompletionListView(ICompletionData[] completionData)
		{
			if (this.completionData != null) {
				Array.Clear(this.completionData, 0, completionData.Length);
			}
			
			Array.Sort(completionData);
			this.completionData = completionData;
			
//			this.KeyDown += new System.Windows.Forms.KeyEventHandler(OnKey);
//			SetStyle(ControlStyles.Selectable, false);
//			SetStyle(ControlStyles.UserPaint, true);
//			SetStyle(ControlStyles.DoubleBuffer, false);
		}
		
		public void Close() 
		{
			if (completionData != null) {
				Array.Clear(completionData, 0, completionData.Length);
			}
			base.Dispose();
		}
		
		public void SelectIndex(int index)
		{
			index = Math.Max(0, index);
			int oldSelectedItem = selectedItem;
			int oldFirstItem    = firstItem;
			selectedItem = Math.Max(0, Math.Min(completionData.Length - 1, index));
			if (selectedItem < firstItem) {
				FirstItem = selectedItem;
			}
			if (firstItem + MaxVisibleItem <= selectedItem) {
				FirstItem = selectedItem - MaxVisibleItem + 1;
			}
			if (oldSelectedItem != selectedItem) {
				if (firstItem != oldFirstItem) {
					Invalidate();
				} else {
					int min = Math.Min(selectedItem, oldSelectedItem) - firstItem;
					int max = Math.Max(selectedItem, oldSelectedItem) - firstItem;
					Invalidate(new Rectangle(0, 1 + min * ItemHeight, Width, (max - min + 1) * ItemHeight));
				}
				Update();
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
		
		public void PageDown()
		{
			SelectIndex(selectedItem + MaxVisibleItem);
		}
		
		public void PageUp()
		{
			SelectIndex(selectedItem - MaxVisibleItem);
		}
		
		public void SelectNextItem()
		{
			SelectIndex(selectedItem + 1);
		}
		
		public void SelectPrevItem()
		{
			SelectIndex(selectedItem - 1);
		}
		
		public void SelectItemWithStart(char startCh)
		{
			for (int i = Math.Min(selectedItem + 1, completionData.Length - 1); i < completionData.Length; ++i) {
				if (completionData[i].Text[0].ToLower()[0] == startCh) {
					SelectIndex(i);
					return;
				}
			}
			
			// now loop from start to current one
			for (int i = 0; i < selectedItem; ++i) {
				if (completionData[i].Text[0].ToLower()[0] == startCh) {
					SelectIndex(i);
					return;
				}
			}
			
			// if not found leave selection as it is
			Refresh();
			OnSelectedItemChanged(EventArgs.Empty);
		}
		
	
		public void SelectItemWithStart(string startText)
		{
			startText = startText.ToLower();
			for (int i = 0; i < completionData.Length; ++i) {
				if (completionData[i].Text[0].ToLower().StartsWith(startText)) {
					SelectIndex(i);
					return;
				}
			}
			selectedItem = -1;
			Refresh();
			OnSelectedItemChanged(EventArgs.Empty);
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			float yPos       = 1;
			float itemHeight = ItemHeight;
			// Maintain aspect ratio
			int imageWidth = (int)(itemHeight * imageList.ImageSize.Width / imageList.ImageSize.Height);
			
			int curItem = firstItem;
			Graphics g  = pe.Graphics;
			while (curItem < completionData.Length && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.IntersectsWith(pe.ClipRectangle)) {
					// draw Background
					if (curItem == selectedItem) {
						g.FillRectangle(SystemBrushes.Highlight, drawingBackground);
					} else {
						g.FillRectangle(SystemBrushes.Window, drawingBackground);
					}
					
					// draw Icon
					int   xPos   = 0;
					if (imageList != null && completionData[curItem].ImageIndex < imageList.Images.Count) {
						g.DrawImage(imageList.Images[completionData[curItem].ImageIndex], new RectangleF(1, yPos, imageWidth, itemHeight));
						xPos = imageWidth;
					}
					
					// draw text
					if (curItem == selectedItem) {
						g.DrawString(completionData[curItem].Text[0], Font, SystemBrushes.HighlightText, xPos, yPos);
					} else {
						g.DrawString(completionData[curItem].Text[0], Font, SystemBrushes.WindowText, xPos, yPos);
					}
				}
				
				yPos += itemHeight;
				++curItem;
			}
			g.DrawRectangle(SystemPens.Control, new Rectangle(0, 0, Width - 1, Height - 1));
		}
		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			float yPos       = 1;
			int curItem = firstItem;
			float itemHeight = ItemHeight;
			
			while (curItem < completionData.Length && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.Contains(e.X, e.Y)) {
					SelectIndex(curItem);
					break;
				}
				yPos += itemHeight;
				++curItem;
			}
		}
		
		protected override void OnMouseWheel(MouseEventArgs mea) 
		{
			int numberOfLines = mea.Delta * SystemInformation.MouseWheelScrollLines / 120;
			//BeginUpdate();
			while (numberOfLines>0) {
				SelectPrevItem();
				numberOfLines--;
			}
			while (numberOfLines<0) {
				SelectNextItem();
				numberOfLines++;
			}
			//EndUpdate();			
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
		}
		
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (SelectedItemChanged != null) {
				SelectedItemChanged(this, e);
			}
		}
		
		protected virtual void OnFirstItemChanged(EventArgs e)
		{
			if (FirstItemChanged != null) {
				FirstItemChanged(this, e);
			}
		}
		
		public event EventHandler SelectedItemChanged;
		public event EventHandler FirstItemChanged;
	}
}
