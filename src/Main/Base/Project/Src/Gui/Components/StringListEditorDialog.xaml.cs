/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.04.2012
 * Time: 18:35
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

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Interaction logic for StringListEditorDialog.xaml
	/// </summary>
	public partial class StringListEditorDialog : Window
	{
		public StringListEditorDialog()
		{
			InitializeComponent();
		}
		
		
	public bool BrowseForDirectory {
			get {return stringListEditor.BrowseForDirectory;}
			set {stringListEditor.BrowseForDirectory = value;}
		}
		
		public string ListCaption {
			get {return stringListEditor.ListCaption; }
			set {stringListEditor.ListCaption = value;}
		}

		public string TitleText {
			get {return stringListEditor.TitleText;}
			set {stringListEditor.TitleText = value;}
		}
		
		public string[] GetList() {
			return stringListEditor.GetList();
		}
		
		public void LoadList(IEnumerable<string> list) {
			stringListEditor.LoadList(list);
		}		
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
			
	}
}