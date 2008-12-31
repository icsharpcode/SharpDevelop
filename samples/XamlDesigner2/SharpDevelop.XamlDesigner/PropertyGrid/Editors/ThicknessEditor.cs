using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	public class ThicknessEditor : Control, INotifyPropertyChanged
	{
		static ThicknessEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ThicknessEditor),
				new FrameworkPropertyMetadata(typeof(ThicknessEditor)));
		}

		Thickness result;

		public double All
		{
			get
			{
				if (Left == Right && Right == Top && Top == Bottom) {
					return Left;
				}
				return 0;
			}
			set
			{
				result = new Thickness(value);
				UpdateValue();
			}
		}

		public double Left
		{
			get
			{
				return result.Left;
			}
			set
			{
				result.Left = value;
				UpdateValue();
			}
		}

		public double Right
		{
			get
			{
				return result.Right;
			}
			set
			{
				result.Right = value;
				UpdateValue();
			}
		}

		public double Top
		{
			get
			{
				return result.Top;
			}
			set
			{
				result.Top = value;
				UpdateValue();
			}
		}

		public double Bottom
		{
			get
			{
				return result.Bottom;
			}
			set
			{
				result.Bottom = value;
				UpdateValue();
			}
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(Thickness), typeof(ThicknessEditor),
			new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public Thickness Value
		{
			get { return (Thickness)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ValueProperty) {
				result = Value;
				RaiseEvents();
			}
		}

		void UpdateValue()
		{
			Value = result;
			RaiseEvents();
		}

		void RaiseEvents()
		{
			RaisePropertyChanged("All");
			RaisePropertyChanged("Left");
			RaisePropertyChanged("Right");
			RaisePropertyChanged("Top");
			RaisePropertyChanged("Bottom");
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
