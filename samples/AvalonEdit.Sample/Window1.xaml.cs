// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace AvalonEdit.Sample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			propertyGridComboBox.SelectedIndex = 2;
			
			textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
			//textEditor.TextArea.TextView.ElementGenerators.Add(new WordFilterElementGenerator());
		}
		
		string currentFileName;
		
		void openFileClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.CheckFileExists = true;
			if (dlg.ShowDialog() ?? false) {
				currentFileName = dlg.FileName;
				textEditor.Load(currentFileName);
				textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
			}
		}
		
		void saveFileClick(object sender, EventArgs e)
		{
			if (currentFileName == null) {
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt";
				if (dlg.ShowDialog() ?? false) {
					currentFileName = dlg.FileName;
				} else {
					return;
				}
			}
			textEditor.Save(currentFileName);
		}
		
		void propertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (propertyGrid == null)
				return;
			switch (propertyGridComboBox.SelectedIndex) {
				case 0:
					propertyGrid.SelectedObject = textEditor;
					break;
				case 1:
					propertyGrid.SelectedObject = textEditor.TextArea;
					break;
				case 2:
					propertyGrid.SelectedObject = textEditor.Options;
					break;
			}
		}
	}
}