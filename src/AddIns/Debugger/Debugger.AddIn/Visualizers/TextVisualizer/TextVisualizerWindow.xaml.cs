// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Highlighting;
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

namespace Debugger.AddIn.Visualizers.TextVisualizer
{
	/// <summary>
	/// Interaction logic for TextVisualizerWindow.xaml
	/// </summary>
	public partial class TextVisualizerWindow : Window
	{
		public TextVisualizerWindow()
		{
			InitializeComponent();
			Mode = TextVisualizerMode.PlainText;
			textEditor.IsReadOnly = true;
		}
		
		public TextVisualizerWindow(string title, string text)
		{
			InitializeComponent();
			
			Title= title;
			Text = text;
		}
		
		public string Text
		{
			get { return this.textEditor.Text; }
			set { this.textEditor.Text = value; }
		}
		
		private TextVisualizerMode mode;
		public TextVisualizerMode Mode
		{
			get { return mode; }
			set { 
				mode = value;
				switch (mode) {
					case TextVisualizerMode.PlainText:
						textEditor.SyntaxHighlighting = null;
						break;
					case TextVisualizerMode.Xml:
						textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
						break;
				}
			}
		}
		
		void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
		{
			textEditor.WordWrap = chbWrap.IsChecked.GetValueOrDefault(false);
		}
		
		void BtnCopy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(textEditor.Text);
		}
		
		void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
