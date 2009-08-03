/*
 * Created by SharpDevelop.
 * User: Siegfried
 * Date: 19.06.2009
 * Time: 20:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding.Options
{
	/// <summary>
	/// Interaction logic for CodeCompletion.xaml
	/// </summary>
	public partial class CodeCompletion : OptionPanel
	{
		public CodeCompletion()
		{
			InitializeComponent();
		}
		
		void ButtonClick(object sender, RoutedEventArgs e)
		{
			SharpDevelopColorDialog dialog = new SharpDevelopColorDialog();
			
			if (dialog.ShowWpfDialog() ?? false) {
				Button b = sender as Button;
				b.Background = new SolidColorBrush(dialog.WpfColor);
				b.Content = (sender as Button).Background;
			}
		}
		
		public override bool SaveOptions()
		{
			if (base.SaveOptions()) {
				XamlColorizer.RefreshAll();
				return true;
			}
			
			return false;
		}
	}
}