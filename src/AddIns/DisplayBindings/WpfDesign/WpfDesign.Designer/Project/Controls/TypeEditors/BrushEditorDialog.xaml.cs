using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors
{
	/// <summary>
	/// Interaction logic for BrushEditorDialog.xaml
	/// </summary>
	public partial class BrushEditorDialog : Window
	{
		static readonly Brush[] specialBrushes = {
			Brushes.White, Brushes.Black, Brushes.Transparent, null
		};
		
		ControlTemplate RadioButtonTemplate;
		
		public BrushEditorDialog()
		{
			InitializeComponent();
			
			RadioButtonTemplate = (ControlTemplate)FindResource("RadioButtonTemplate");
			
			const int bigColorSquareSize = 18;
			const int smallColorSquareSize = 12;
			
			// special brushes:
			AddColorSquare(null, null, "null", bigColorSquareSize);
			AddColorSquare(Brushes.Black, null, "Black", bigColorSquareSize);
			AddColorSquare(Brushes.White, null, "White", bigColorSquareSize);
			AddColorSquare(Brushes.Transparent, null, "Transparent", bigColorSquareSize);
			x = 0;
			y += bigColorSquareSize;
			
			AddSeparatorLine();
			
			foreach (PropertyInfo p in typeof(Brushes).GetProperties()) {
				Brush brush = (Brush)p.GetValue(null, null);
				if (!specialBrushes.Contains(brush))
					AddColorSquare(brush, null, p.Name, smallColorSquareSize);
			}
			canvas.Height = y + smallColorSquareSize;
		}
		
		int x = 0;
		int y = 0;
		
		void AddColorSquare(Brush brush, UIElement content, string tooltip, int size)
		{
			RadioButton radioButton = new RadioButton {
				Background = brush,
				Width = size,
				Height = size
			};
			radioButton.ToolTip = tooltip;
			radioButton.Template = RadioButtonTemplate;
			Canvas.SetLeft(radioButton, x);
			Canvas.SetTop(radioButton, y);
			canvas.Children.Add(radioButton);
			
			radioButton.Checked += delegate {
				if (selectedBrush != radioButton.Background) {
					selectedBrush = radioButton.Background;
					if (SelectedBrushChanged != null) {
						SelectedBrushChanged(this, EventArgs.Empty);
					}
				}
			};
			
			x += size;
			if (x > 260) {
				x = 0;
				y += size;
			}
		}
		
		void AddSeparatorLine()
		{
			Line line = new Line {
				StrokeThickness = 1,
				Stroke = Brushes.Gray,
				Height = 1,
				X2 = 260,
				Y1 = 0.5,
				Y2 = 0.5
			};
			Canvas.SetLeft(line, 0);
			Canvas.SetTop(line, y + 1);
			canvas.Children.Add(line);
			y += 3;
		}
		
		Brush selectedBrush;
		
		public Brush SelectedBrush {
			get {
				return selectedBrush;
			}
			set {
				if (selectedBrush != value) {
					selectedBrush = value;
					foreach (RadioButton btn in canvas.Children.OfType<RadioButton>()) {
						btn.IsChecked = btn.Background == value;
					}
					
					if (SelectedBrushChanged != null) {
						SelectedBrushChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler SelectedBrushChanged;
		
		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			Close();
		}
	}
}