using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner
{
	class ResizeInfo : Control
	{
		public ResizeInfo(Canvas parent)
		{
			this.parent = parent;
			parent.Children.Add(this);
		}

		Canvas parent;

		static ResizeInfo()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeInfo),
				new FrameworkPropertyMetadata(typeof(ResizeInfo)));
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ResizeInfo));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public void Update(Size size)
		{
			Text = size.Width.ToInvariantString() + " x " + size.Height.ToInvariantString();
			var p = Mouse.GetPosition(parent);
			Canvas.SetLeft(this, p.X);
			Canvas.SetTop(this, p.Y);
		}

		public void Remove()
		{
			parent.Children.Remove(this);
		}
	}
}
