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
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class ColorPickerComboBox : System.Windows.Forms.ComboBox
	{
		Color customColor = Color.Empty;
		int ColorRectangleLeftOffset = 4;
		int ColorRectangleTopOffset = 2;
		
		public ColorPickerComboBox()
		{
			base.DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;			
			AddBasicColors();
		}
		
		public Color SelectedColor {
			get {
				if (SelectedItem != null) {
					return (Color)SelectedItem;
				}
				return Color.Empty;
			}
			set {
				if (Items.Contains(value)) {
					SelectedItem = value;
				} else if (CustomColorExists) {
					UpdateCustomColor(value);
				} else {
					AddCustomColor(value);
				}
			}
		}
		
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0) {
				e.DrawBackground();
				Graphics g = e.Graphics;
				Color color = (Color)Items[e.Index];
				Rectangle colorRectangle = GetColorRectangle(e.Bounds.Top);
				g.FillRectangle(new SolidBrush(color), colorRectangle);
				g.DrawRectangle(Pens.Black, colorRectangle);
				int textOffset = (2 * colorRectangle.Left) + colorRectangle.Width;
				SolidBrush brush = GetTextBrush(IsSelected(e.State));
				g.DrawString(GetColorName(color), e.Font, brush, new Rectangle(textOffset, e.Bounds.Top, e.Bounds.Width - textOffset, ItemHeight));
			}
			base.OnDrawItem(e);
		}
		
		bool CustomColorExists {
			get {
				return customColor != Color.Empty;
			}
		}
		
		void AddBasicColors()
		{
			Items.Add(Color.Black);
			Items.Add(Color.White);
			Items.Add(Color.Maroon);
			Items.Add(Color.Green);
			Items.Add(Color.Olive);
			Items.Add(Color.Navy);
			Items.Add(Color.Purple);
			Items.Add(Color.Teal);
			Items.Add(Color.Silver);
			Items.Add(Color.Gray);
			Items.Add(Color.Red);
			Items.Add(Color.Lime);
			Items.Add(Color.Yellow);
			Items.Add(Color.Blue);
			Items.Add(Color.Magenta);
			Items.Add(Color.Cyan);
			SelectedIndex = 0;
		}
		
		void AddCustomColor(Color color)
		{
			customColor = color;
			SelectedIndex = Items.Add(color);
		}
		
		void UpdateCustomColor(Color color)
		{
			int index = Items.IndexOf(customColor);
			if (index >= 0) {
				customColor = color;
				Items[index] = color;
				SelectedIndex = index;
			} else {
				AddCustomColor(color);
			}
		}
		
		Rectangle GetColorRectangle(int y)
		{
			int colorRectangleHeight = ItemHeight - (2 * ColorRectangleTopOffset);
			return new Rectangle(ColorRectangleLeftOffset, y + ColorRectangleTopOffset, colorRectangleHeight, colorRectangleHeight);
		}
		
		string GetColorName(Color color)
		{
			if (CustomColorExists && color == customColor) {
				return "Custom";
			}
			return color.Name;
		}
		
		SolidBrush GetTextBrush(bool selected)
		{
			if (selected) {
				return new SolidBrush(Color.White);
			}
			return new SolidBrush(ForeColor);
		}
		
		bool IsSelected(DrawItemState state)
		{
			return (state & DrawItemState.Selected) == DrawItemState.Selected;
		}
		
		protected override void OnDropDownStyleChanged(EventArgs e)
		{
			if (DropDownStyle != ComboBoxStyle.DropDownList) {
				DropDownStyle = ComboBoxStyle.DropDownList;
			}
			base.OnDropDownStyleChanged(e);
		}
		
	}
}
