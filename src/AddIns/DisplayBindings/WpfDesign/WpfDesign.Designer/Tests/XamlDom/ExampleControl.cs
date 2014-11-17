using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	public class ExampleControl : Control
	{
		public object Property1
		{
			get { return (object)GetValue(Property1Property); }
			set { SetValue(Property1Property, value); }
		}

		public static readonly DependencyProperty Property1Property =
			DependencyProperty.Register("Property1", typeof(object), typeof(ExampleControl), new PropertyMetadata(null));

	}
}
