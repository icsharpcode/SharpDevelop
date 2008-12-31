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

namespace SharpDevelop.XamlDesigner.Commanding
{
	public partial class DefaultContainerStyles : ResourceDictionary
	{
		public DefaultContainerStyles()
		{
			InitializeComponent();
		}

		void ButtonClick(object sender, RoutedEventArgs e)
		{
			CommandHelper.TryExecuteCommand(sender);
		}

		void MenuItemClick(object sender, RoutedEventArgs e)
		{
			CommandHelper.TryExecuteCommand(sender);
		}
	}
}
