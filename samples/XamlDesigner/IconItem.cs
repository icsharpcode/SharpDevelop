using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.XamlDesigner
{
	public class IconItem : Control
	{
		static IconItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(IconItem), 
				new FrameworkPropertyMetadata(typeof(IconItem)));
		}

		public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(IconItem));

        public ImageSource Icon {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(IconItem));

		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
