// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Debugger.AddIn.Visualizers.TextVisualizer
{
	/// <summary>
	/// Interaction logic for TextVisualizerWindow.xaml
	/// </summary>
	public partial class TextVisualizerWindow : Window
	{
		public TextVisualizerWindow(string title, string text, string highlighting = null)
		{
			InitializeComponent();
			
			Title = title;
			this.textEditor.Text = text;
			if (highlighting != null)
				this.textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(highlighting);
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
