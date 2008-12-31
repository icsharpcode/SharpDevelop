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
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner
{
	public partial class ToolPanel : UserControl, IHasContext
	{
		public ToolPanel(DesignContext context)
		{
			Context = context;
			InitializeComponent();
		}

		public DesignContext Context { get; private set; }

		public static readonly DependencyProperty ShowModeSelectorProperty =
			DependencyProperty.Register("ShowModeSelector", typeof(bool), typeof(ToolPanel));

		public bool ShowModeSelector
		{
			get { return (bool)GetValue(ShowModeSelectorProperty); }
			set { SetValue(ShowModeSelectorProperty, value); }
		}

		void uxResetZoom_Click(object sender, RoutedEventArgs e)
		{
			Context.DesignView.Zoom = 1;
		}
	}
}
