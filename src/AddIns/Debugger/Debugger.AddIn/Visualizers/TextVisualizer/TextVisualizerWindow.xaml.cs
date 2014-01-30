// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
