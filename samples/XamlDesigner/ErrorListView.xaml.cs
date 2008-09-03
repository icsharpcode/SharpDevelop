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
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.XamlDesigner
{
	public partial class ErrorListView
	{
		public ErrorListView()
		{
			InitializeComponent();
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			var error = e.GetDataContext() as XamlError;
			if (error != null) {
				Shell.Instance.JumpToError(error);
			}
		}
	}
}
