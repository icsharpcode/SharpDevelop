using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace StandaloneDesigner
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
		
		void tabControlSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (tabControl.SelectedItem == designTab) {
				designSurface.LoadDesigner(new XmlTextReader(new StringReader(CodeTextBox.Text)));
			} else {
				designSurface.UnloadDesigner();
			}
		}
	}
}
