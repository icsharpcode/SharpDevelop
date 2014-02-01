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
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	public enum UnitType : int {
		Auto = 0,
		Star = 1,
		Pixel = 2,
		Point = 3,
		Centimeter = 4,
		Inch = 5
	}
	
	/// <summary>
	/// Interaction logic for GridLengthEditor.xaml
	/// </summary>
	public partial class GridLengthEditor : UserControl
	{		
		public UnitType Unit { get; private set; }
		public int Cell { get; private set; }
		public Orientation Orientation { get; private set; }
		
		public GridLengthEditor(Orientation type, int cell, string value)
		{
			InitializeComponent();

			if (type == Orientation.Horizontal)
				this.SetValue(Grid.ColumnProperty, cell);
			else
				this.SetValue(Grid.RowProperty, cell);
			
			SetValue(value.Trim());
			
			this.Cell = cell;
			
			this.panel.Orientation = this.Orientation = type;
		}
		
		void SetValue(string value)
		{
			string stringValue = value.Trim();
			double numValue;
			bool success = double.TryParse(stringValue.ToUpperInvariant().Trim('P', 'X', 'T', 'C', 'M', 'I', 'N'), out numValue);
			
			if (stringValue.Equals("Auto", StringComparison.OrdinalIgnoreCase)) {
				Unit = UnitType.Auto;
				txtNumericValue.Text = "";
			} else {
				if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase)) {
					Unit = UnitType.Pixel;
					txtNumericValue.Text = stringValue.Remove(value.Length - 2);
				} else 	if (value.EndsWith("pt", StringComparison.OrdinalIgnoreCase)) {
					Unit = UnitType.Point;
					txtNumericValue.Text = stringValue.Remove(value.Length - 2);
				} else	if (value.EndsWith("cm", StringComparison.OrdinalIgnoreCase)) {
					Unit = UnitType.Centimeter;
					txtNumericValue.Text = stringValue.Remove(value.Length - 2);
				} else	if (value.EndsWith("in", StringComparison.OrdinalIgnoreCase)) {
					Unit = UnitType.Inch;
					txtNumericValue.Text = stringValue.Remove(value.Length - 2);
				} else if (value.EndsWith("*", StringComparison.OrdinalIgnoreCase)) {
					Unit = UnitType.Star;
					txtNumericValue.Text = stringValue.Remove(value.Length - 1);
				} else if (string.IsNullOrEmpty(stringValue)) {
					Unit = UnitType.Star;
					txtNumericValue.Text = "";
				} else	{
					Unit = UnitType.Pixel;
					txtNumericValue.Text = stringValue;
				}
			}
			
			cmbType.SelectedIndex = (int)Unit;
		}
		
		public string SelectedValue {
			get {
				string stringValue = txtNumericValue.Text.Trim().ToUpperInvariant();
				double value;
				bool success = double.TryParse(stringValue.Trim('P', 'X', 'T', 'C', 'M', 'I', 'N'),
				                               NumberStyles.Float, CultureInfo.InvariantCulture, out value);
				string result = "";
				
				switch (Unit) {
					case UnitType.Auto:
						return "Auto";
					case UnitType.Centimeter:
						result = value.ToString(CultureInfo.InvariantCulture) + "cm";
						break;
					case UnitType.Inch:
						result = value.ToString(CultureInfo.InvariantCulture) + "in";
						break;
					case UnitType.Pixel:
						result = value.ToString(CultureInfo.InvariantCulture) + "px";
						break;
					case UnitType.Point:
						result = value.ToString(CultureInfo.InvariantCulture) + "pt";
						break;
					case UnitType.Star:
						if (string.IsNullOrEmpty(stringValue))
							return "*";
						result = value.ToString(CultureInfo.InvariantCulture) + "*";
						break;
				}
				
				if (success)
					return result;
				else
					return null;
			}
		}
		
		public event EventHandler<GridLengthSelectionChangedEventArgs> SelectedValueChanged;
		
		public event EventHandler<GridLengthSelectionChangedEventArgs> Deleted;
		
		protected virtual void OnDeleted(GridLengthSelectionChangedEventArgs e)
		{
			if (Deleted != null) {
				Deleted(this, e);
			}
		}
		
		protected virtual void OnSelectedValueChanged(GridLengthSelectionChangedEventArgs e)
		{
			if (SelectedValueChanged != null) {
				SelectedValueChanged(this, e);
			}
		}
		
		void TxtNumericValueTextChanged(object sender, TextChangedEventArgs e)
		{
			if (Unit == UnitType.Star && string.IsNullOrEmpty(txtNumericValue.Text)) {
				txtNumericValue.ClearValue(TextBox.BackgroundProperty);
				txtNumericValue.ClearValue(TextBox.ForegroundProperty);
			} else {
				double value;
				if (!double.TryParse(txtNumericValue.Text.Trim('P', 'X', 'T', 'C', 'M', 'I', 'N'),
				                               NumberStyles.Any, CultureInfo.InvariantCulture, out value)) {
					txtNumericValue.Background = Brushes.Red;
					txtNumericValue.Foreground = Brushes.White;
				} else {
					txtNumericValue.ClearValue(TextBox.BackgroundProperty);
					txtNumericValue.ClearValue(TextBox.ForegroundProperty);
				}
			}
			OnSelectedValueChanged(new GridLengthSelectionChangedEventArgs(this.panel.Orientation, Cell, SelectedValue));
		}
		
		void BtnDelClick(object sender, RoutedEventArgs e)
		{
			OnDeleted(new GridLengthSelectionChangedEventArgs(this.panel.Orientation, Cell, SelectedValue));
		}
		
		void CmbTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Unit = (UnitType)cmbType.SelectedIndex;
			
			if (Unit == UnitType.Auto)
				txtNumericValue.Visibility = Visibility.Collapsed;
			else
				txtNumericValue.Visibility = Visibility.Visible;
			
			TxtNumericValueTextChanged(null, null);
		}
	}

	public class GridLengthSelectionChangedEventArgs : EventArgs
	{
		public Orientation Type { get; private set; }
		public int Cell { get; private set; }
		public string Value { get; private set; }
		
		public GridLengthSelectionChangedEventArgs(Orientation type, int cell, string value)
		{
			this.Type = type;
			this.Cell = cell;
			this.Value = value;
		}
	}
}
