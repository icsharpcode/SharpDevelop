using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Dom.TypeReplacement
{
	class DesignTimeWindow : ContentControl
	{
		static DesignTimeWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignTimeWindow),
				new FrameworkPropertyMetadata(typeof(DesignTimeWindow)));
		}

		public static readonly DependencyProperty TitleProperty =
			Window.TitleProperty.AddOwner(typeof(DesignTimeWindow));

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
	}
}
