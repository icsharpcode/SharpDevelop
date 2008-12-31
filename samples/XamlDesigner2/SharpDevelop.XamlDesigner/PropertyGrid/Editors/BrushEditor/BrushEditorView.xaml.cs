using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors.BrushEditor
{
	public partial class BrushEditorView : UserControl, INotifyPropertyChanged
	{
		public BrushEditorView()
		{
			InitializeComponent();
			uxDataContextHolder.DataContext = this;

			GradientStopCollection stops = new GradientStopCollection();
			stops.Add(new GradientStop(Colors.White, 0));
			stops.Add(new GradientStop(Colors.Black, 1));

			linearGradientBrush = new LinearGradientBrush(stops);
			linearGradientBrush.EndPoint = new Point(1, 0);
			radialGradientBrush = new RadialGradientBrush(stops);
		}

		SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.White);
		LinearGradientBrush linearGradientBrush;
		RadialGradientBrush radialGradientBrush;

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Brush), typeof(BrushEditorView),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public Brush Value
		{
			get { return (Brush)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		BrushEditorKind kind;

		public BrushEditorKind Kind
		{
			get { return kind; }
			set 
			{ 
				kind = value;
				UpdateInternalValueFromKind();
				RaisePropertyChanged("Kind");
			}
		}

		Brush internalValue;

		public Brush InternalValue
		{
			get { return internalValue; }
			set 
			{
				if (internalValue != value) {
					internalValue = value;
					Value = InternalValue;
					RaisePropertyChanged("InternalValue");
				}
			}
		}

		public double GradientAngle
		{
			get
			{
				var x = linearGradientBrush.EndPoint.X - linearGradientBrush.StartPoint.X;
				var y = linearGradientBrush.EndPoint.Y - linearGradientBrush.StartPoint.Y;
				return Vector.AngleBetween(new Vector(1, 0), new Vector(x, -y));
			}
			set
			{
				var d = value * Math.PI / 180;
				var p = new Point(Math.Cos(d), -Math.Sin(d));
				var k = 1 / Math.Max(Math.Abs(p.X), Math.Abs(p.Y));
				p.X *= k;
				p.Y *= k;
				var p2 = new Point(-p.X, -p.Y);
				linearGradientBrush.StartPoint = new Point((p2.X + 1) / 2, (p2.Y + 1) / 2);
				linearGradientBrush.EndPoint = new Point((p.X + 1) / 2, (p.Y + 1) / 2);
				RaisePropertyChanged("GradientAngle");
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ValueProperty) {
				if (internalValue != Value) {
					InitializeInternalValue();
				}
				UpdateKindFromInternalValue();
			}
		}

		void InitializeInternalValue()
		{
			if (Value != null) {
				internalValue = Value.CloneCurrentValue();
				internalValue.Changed += new EventHandler(internalValue_Changed);
			}
			else {
				internalValue = Value;
			}
			RaisePropertyChanged("InternalValue");
		}

		void internalValue_Changed(object sender, EventArgs e)
		{
			if (Value != internalValue) {
				Value = internalValue;
			}
		}

		void UpdateKindFromInternalValue()
		{
			if (InternalValue == null) {
				kind = BrushEditorKind.None;
			}
			else if (InternalValue is SolidColorBrush) {
				solidColorBrush = InternalValue as SolidColorBrush;
				kind = BrushEditorKind.Solid;
			}
			else if (InternalValue is LinearGradientBrush) {
				linearGradientBrush = InternalValue as LinearGradientBrush;
				radialGradientBrush.GradientStops = linearGradientBrush.GradientStops;
				kind = Math.Abs(GradientAngle) == 90 ? BrushEditorKind.Vertical : BrushEditorKind.Horizontal;
			}
			else if (InternalValue is RadialGradientBrush) {
				radialGradientBrush = InternalValue as RadialGradientBrush;
				linearGradientBrush.GradientStops = linearGradientBrush.GradientStops;
				kind = BrushEditorKind.Radial;
			}
			RaisePropertyChanged("Kind");
		}

		void UpdateInternalValueFromKind()
		{
			switch (Kind) {
				case BrushEditorKind.None:
					InternalValue = null;
					break;

				case BrushEditorKind.Solid:
					InternalValue = solidColorBrush;
					break;

				case BrushEditorKind.Horizontal:
					InternalValue = linearGradientBrush;
					GradientAngle = 0;
					break;

				case BrushEditorKind.Vertical:
					InternalValue = linearGradientBrush;
					GradientAngle = -90;
					break;

				case BrushEditorKind.Radial:
					InternalValue = radialGradientBrush;
					break;
			}
		}

		public enum BrushEditorKind
		{
			None,
			Solid,
			Horizontal,
			Vertical,
			Radial
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
