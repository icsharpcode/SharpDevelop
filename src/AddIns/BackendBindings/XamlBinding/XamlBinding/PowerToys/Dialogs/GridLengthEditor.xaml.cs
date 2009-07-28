// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for GridLengthEditor.xaml
	/// </summary>
	public partial class GridLengthEditor : UserControl
	{
		GridUnitType type = GridUnitType.Star;
		int cell;
		
		public GridLengthEditor(Orientation type, int cell, string value)
		{
			InitializeComponent();

			if (type == Orientation.Horizontal)
				this.SetValue(Grid.ColumnProperty, cell);
			else
				this.SetValue(Grid.RowProperty, cell);
			
			SetValue(value.Trim());
			
			this.cell = cell;
			
			this.panel.Orientation = type;
		}
		
		void SetValue(string value)
		{
			if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase)) {
				type = GridUnitType.Auto;
				txtNumericValue.Text = "";
			} else {
				if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase)) {
					type = GridUnitType.Pixel;
					txtNumericValue.Text = value.Remove(value.Length - 2);
				} else if (value.EndsWith("*", StringComparison.OrdinalIgnoreCase)) {
					type = GridUnitType.Star;
					txtNumericValue.Text = value.Remove(value.Length - 1);
				} else if (string.IsNullOrEmpty(value)) {
					type = GridUnitType.Star;
					txtNumericValue.Text = "";
				} else	{
					type = GridUnitType.Pixel;
					txtNumericValue.Text = value;
				}
			}
			
			switch (type) {
				case GridUnitType.Star:
					btnType.Content = "  *  ";
					txtNumericValue.Visibility = Visibility.Visible;
					break;
				case GridUnitType.Pixel:
					btnType.Content = "Pixel";
					txtNumericValue.Visibility = Visibility.Visible;
					break;
				case GridUnitType.Auto:
					btnType.Content = "Auto";
					txtNumericValue.Visibility = Visibility.Collapsed;
					break;
			}
		}
		
		void BtnTypeClick(object sender, RoutedEventArgs e)
		{
			switch (type) {
				case GridUnitType.Auto:
					type = GridUnitType.Star;
					btnType.Content = "  *  ";
					txtNumericValue.Visibility = Visibility.Visible;
					break;
				case GridUnitType.Star:
					type = GridUnitType.Pixel;
					btnType.Content = "Pixel";
					txtNumericValue.Visibility = Visibility.Visible;
					break;
				case GridUnitType.Pixel:
					type = GridUnitType.Auto;
					btnType.Content = "Auto";
					txtNumericValue.Visibility = Visibility.Collapsed;
					break;
			}
			TxtNumericValueTextChanged(null, null);
		}
		
		public GridLength? SelectedValue {
			get {
				if (type == GridUnitType.Star || type == GridUnitType.Pixel) {
					double value;
					if (type == GridUnitType.Star && string.IsNullOrEmpty(txtNumericValue.Text)) {
						return new GridLength(1, type);
					} else {
						if (double.TryParse(txtNumericValue.Text, out value))
							return new GridLength(value, type);
					}
					
					return null;
				}
				
				return GridLength.Auto;
			}
		}
		
		public event EventHandler<GridLengthSelectionChangedEventArgs> SelectedValueChanged;
		
		public event EventHandler<GridLengthSelectionChangedEventArgs> Deleted;
		
		public event EventHandler<GridLengthSelectionChangedEventArgs> Added;
		
		protected virtual void OnAdded(GridLengthSelectionChangedEventArgs e)
		{
			if (Added != null) {
				Added(this, e);
			}
		}
		
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
			if (type == GridUnitType.Star && string.IsNullOrEmpty(txtNumericValue.Text)) {
				txtNumericValue.ClearValue(TextBox.BackgroundProperty);
				txtNumericValue.ClearValue(TextBox.ForegroundProperty);
			} else {
				double x;
				if (!double.TryParse(txtNumericValue.Text, out x)) {
					txtNumericValue.Background = Brushes.Red;
					txtNumericValue.Foreground = Brushes.White;
				} else {
					txtNumericValue.ClearValue(TextBox.BackgroundProperty);
					txtNumericValue.ClearValue(TextBox.ForegroundProperty);
				}
			}
			OnSelectedValueChanged(new GridLengthSelectionChangedEventArgs(this.panel.Orientation, cell, SelectedValue));
		}
		
		void BtnDelClick(object sender, RoutedEventArgs e)
		{
			OnDeleted(new GridLengthSelectionChangedEventArgs(this.panel.Orientation, cell, SelectedValue));
		}
		
		void BtnInsertCellsClick(object sender, RoutedEventArgs e)
		{
			OnAdded(new GridLengthSelectionChangedEventArgs(this.panel.Orientation, cell, SelectedValue));
		}
	}
	
	public class GridLengthSelectionChangedEventArgs : EventArgs
	{
		public Orientation Type { get; private set; }
		public int Cell { get; private set; }
		public GridLength? Value { get; private set; }
		
		public GridLengthSelectionChangedEventArgs(Orientation type, int cell, GridLength? value)
		{
			this.Type = type;
			this.Cell = cell;
			this.Value = value;
		}
	}
}